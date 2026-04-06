using Microsoft.AspNetCore.Identity;
using RestaurantManagementSystem.Models.ViewModels;

namespace RestaurantManagementSystem.Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserListViewModel>> GetAllUsersWithRolesAsync();
        Task<IdentityResult> CreateUserAsync(CreateUserViewModel model);
        Task<bool> UpdateUserRoleAsync(string userId, string newRole);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IdentityResult> VerifyEmailAsync(string userId);
    }
}
