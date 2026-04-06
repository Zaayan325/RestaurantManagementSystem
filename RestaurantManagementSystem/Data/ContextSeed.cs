using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantManagementSystem.Models;

namespace RestaurantManagementSystem.Data
{
    public static class ContextSeed
    {
        public static async Task SeedDatabaseAsync(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            // 1. Seed Roles
            string[] roleNames = { "Admin", "Chef", "Waiter" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            // 2. Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Fast Food" },
                    new Category { Name = "Beverages" },
                    new Category { Name = "Desserts" }
                };
                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // 3. Seed Menu Items
            if (!await context.MenuItems.AnyAsync())
            {
                // We fetch the category IDs we just created
                var fastFoodId = context.Categories.First(c => c.Name == "Fast Food").Id;
                var drinkId = context.Categories.First(c => c.Name == "Beverages").Id;

                var items = new List<MenuItem>
                {
                    new MenuItem { Name = "Zinger Burger", Price = 550, CategoryId = fastFoodId, IsAvailable = true, Description = "Crispy chicken fillet" },
                    new MenuItem { Name = "Club Sandwich", Price = 450, CategoryId = fastFoodId, IsAvailable = true, Description = "Classic 3-layer sandwich" },
                    new MenuItem { Name = "Fresh Lime", Price = 150, CategoryId = drinkId, IsAvailable = true, Description = "Chilled soda with lemon" }
                };
                await context.MenuItems.AddRangeAsync(items);
                await context.SaveChangesAsync();
            }

            //4. Seed Tables
            if (!await context.Tables.AnyAsync())
            {
                var tables = new List<Table>
                {
                    new Table { TableName = "Table 01", Capacity = 2, Status = TableStatus.Available },
                    new Table { TableName = "Table 02", Capacity = 4, Status = TableStatus.Available },
                    new Table { TableName = "VIP Lounge", Capacity = 6, Status = TableStatus.Available },
                    new Table { TableName = "Window Side", Capacity = 2, Status = TableStatus.Available }
                };
                await context.Tables.AddRangeAsync(tables);
                await context.SaveChangesAsync();
            }

            //5. Seed Orders
            if (!await context.Orders.AnyAsync())
            {
                var tableId = (await context.Tables.FirstAsync()).Id;
                var burger = await context.MenuItems.FirstAsync(m => m.Name == "Zinger Burger");
                var drink = await context.MenuItems.FirstAsync(m => m.Name == "Fresh Lime");

                var sampleOrder = new Order
                {
                    TableId = tableId,
                    OrderDate = DateTime.Now.AddHours(-1),
                    ServiceType = ServiceType.DineIn,
                    Status = OrderStatus.Pending,
                    TotalAmount = (burger.Price * 2) + drink.Price,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { MenuItemId = burger.Id, Quantity = 2, UnitPrice = burger.Price },
                        new OrderItem { MenuItemId = drink.Id, Quantity = 1, UnitPrice = drink.Price }
                    }
                };

                // When we add the order, the Table should show as Occupied
                var table = await context.Tables.FindAsync(tableId);
                if (table != null) table.Status = TableStatus.Occupied;

                await context.Orders.AddAsync(sampleOrder);
                await context.SaveChangesAsync();
            }
        }
    }
}
