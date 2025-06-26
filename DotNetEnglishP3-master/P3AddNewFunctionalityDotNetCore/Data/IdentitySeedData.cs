using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.Data
{
    /// <summary>
    /// Provides a method to seed the identity system with an initial admin user.
    /// </summary>
    public static class IdentitySeedData
    {
        private const string AdminUser = "Admin";
        private const string AdminPassword = "P@ssword123";

        /// <summary>
        /// Ensures that the identity system contains an admin user.
        /// If the user does not exist, it is created with a predefined password.
        /// </summary>
        /// <param name="app">The application builder used to access application services.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task EnsurePopulated(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var userManager = (UserManager<IdentityUser>)scope.ServiceProvider.GetService(typeof(UserManager<IdentityUser>));

            IdentityUser user = await userManager.FindByIdAsync(AdminUser);

            if (user == null)
            {
                user = new IdentityUser("Admin");
                await userManager.CreateAsync(user, AdminPassword);
            }
        }
    }
}
