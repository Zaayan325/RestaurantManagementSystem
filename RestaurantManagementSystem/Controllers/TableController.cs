using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Controllers
{
    [Authorize]
    public class TableController : Controller
    {
        private readonly ITableService _tableService;
        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }

        //View
        public async Task<IActionResult> Index()
        {
            var tables = await _tableService.GetAllTablesAsync();
            return View(tables);
        }

        //Create
        [Authorize(Roles = "Admin,Chef")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Chef")]
        public async Task<IActionResult> Create(Table table)
        {
            if (ModelState.IsValid)
            {
                await _tableService.CreateTableAsync(table);
                return RedirectToAction(nameof(Index));
            }
            return View(table);
        }

        //Edit
        [Authorize(Roles = "Admin,Chef")]
        public async Task<IActionResult> Edit(int id)
        {
            var table = await _tableService.GetTableByIdAsync(id);
            if (table == null) return NotFound();
            return View(table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Table table)
        {
            if (id != table.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                await _tableService.UpdateTableAsync(table);
                return RedirectToAction(nameof(Index));
            }
            return View(table);
        }

        // Status Update
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, TableStatus status)
        {
            await _tableService.UpdateTableStatusAsync(id, status);
            return RedirectToAction(nameof(Index));
        }
    }
}
