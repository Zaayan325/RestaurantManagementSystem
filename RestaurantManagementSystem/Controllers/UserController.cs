using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RestaurantManagementSystem.Models.ViewModels;
using RestaurantManagementSystem.Services.Interface;

namespace RestaurantManagementSystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUserService userService, RoleManager<IdentityRole> roleManager)
        {
            _userService = userService;
            _roleManager = roleManager;
        }

        //View
        public async Task<IActionResult> Index()
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(await _userService.GetAllUsersWithRolesAsync());
        }

        //Update Roles

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string userId, string newRole)
        {
            await _userService.UpdateUserRoleAsync(userId, newRole);
            return RedirectToAction(nameof(Index));
        }

        //Create
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_roleManager.Roles.Select(r => r.Name).ToList());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(_roleManager.Roles.Select(r => r.Name).ToList());
                return View(model);
            }

            var result = await _userService.CreateUserAsync(model);

            if (result.Succeeded) return RedirectToAction(nameof(Index));

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewBag.Roles = new SelectList(_roleManager.Roles.Select(r => r.Name).ToList());
            return View(model);
        }

        //Verify Email
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(string userId)
        {
            var result = await _userService.VerifyEmailAsync(userId);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return RedirectToAction(nameof(Index));
        }

        //Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var result = await _userService.DeleteUserAsync(id);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("Index", await _userService.GetAllUsersWithRolesAsync());
        }
    }
}
