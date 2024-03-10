using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using SecureNetworks.Models.DBModels;

namespace SecureNetworks.Helpers
{
    public class UserServiceHepler
    {
        private readonly UserManager<ApplicationUserDBEntity> _userManager;

        public UserServiceHepler(UserManager<ApplicationUserDBEntity> userManager)
        {
            _userManager = userManager;
        }

        public async Task<int?> GetIsAdminUserAsync(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            if (currentUser != null)
            {
                return currentUser.IsAdminUser;
            }
            return null;
        }
    }
}
