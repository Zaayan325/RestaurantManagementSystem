using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly ICategoryService _categoryService;

        public MenuController(IMenuService menuService, ICategoryService categoryService)
        {
            _menuService = menuService;
            _categoryService = categoryService;
        }

        //view
        public async Task<IActionResult> Index()
        {
            var items = await _menuService.GetAllItemsAsync();
            return View(items);
        }

        [Authorize(Roles = "Admin,Chef")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");
            return View();
        }

        //Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Chef")]
        public async Task<IActionResult> Create(MenuItem item)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                await _menuService.CreateItemAsync(item);
                return RedirectToAction(nameof(Index));
            }
            // If we fail, reload the categories for the dropdown
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name");
            return View(item);
        }

        //Delete
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _menuService.DeleteItemAsync(id);
            return RedirectToAction(nameof(Index));
        }

        //Update
        [Authorize(Roles = "Admin,Chef")]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _menuService.GetItemByIdAsync(id);
            if (item == null) return NotFound();

            // Fill the dropdown so the current category is selected
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name", item.CategoryId);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Chef")]
        public async Task<IActionResult> Edit(int id, MenuItem item)
        {
            if (id != item.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _menuService.UpdateItemAsync(item);
                return RedirectToAction(nameof(Index));
            }
            // If validation fails, reload categories
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.CategoryList = new SelectList(categories, "Id", "Name", item.CategoryId);
            return View(item);
        }
        //Update Status
        [HttpPost]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            await _menuService.ToggleAvailabilityAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
