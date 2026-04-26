using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.Models.ViewModels;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Controllers
{
    [Authorize(Roles = "Admin,Chef")]
    public class InventoryController : Controller
    {
        private readonly IRawMaterialService _rawMaterialService;

        public InventoryController(IRawMaterialService rawMaterialService)
        {
            _rawMaterialService = rawMaterialService;
        }

        public async Task<IActionResult> Index()
        {
            var materials = await _rawMaterialService.GetAllAsync();
            ViewBag.LowStockCount = (await _rawMaterialService.GetLowStockItemsAsync()).Count();
            return View(materials);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RawMaterial material)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid) return View(material);

            await _rawMaterialService.CreateAsync(material);
            TempData["Notification"] = "Raw material added successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var material = await _rawMaterialService.GetByIdAsync(id);
            if (material == null) return NotFound();
            return View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RawMaterial material)
        {
            if (id != material.Id) return BadRequest();
            if (!ModelState.IsValid) return View(material);

            await _rawMaterialService.UpdateAsync(material);
            TempData["Notification"] = "Raw material updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _rawMaterialService.DeleteAsync(id);
            TempData["Notification"] = "Raw material deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AddStock(int id)
        {
            var material = await _rawMaterialService.GetByIdAsync(id);
            if (material == null) return NotFound();

            return View(new RawMaterialStockUpdateViewModel
            {
                Id = material.Id,
                Name = material.Name,
                CurrentStock = material.CurrentStock,
                Unit = material.Unit
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStock(RawMaterialStockUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _rawMaterialService.AddStockAsync(model.Id, model.Quantity, model.PurchasePrice);
                TempData["Notification"] = "Stock purchase recorded successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> ConsumeStock(int id)
        {
            var material = await _rawMaterialService.GetByIdAsync(id);
            if (material == null) return NotFound();

            return View(new RawMaterialStockUpdateViewModel
            {
                Id = material.Id,
                Name = material.Name,
                CurrentStock = material.CurrentStock,
                Unit = material.Unit
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConsumeStock(RawMaterialStockUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _rawMaterialService.ConsumeStockAsync(model.Id, model.Quantity);
                TempData["Notification"] = "Stock consumption recorded successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }
    }
}
