using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.Controllers

{
    /// <summary>
    /// Controller handles user accounts (login/logout).
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        /// <summary>
        /// Initialize new instance de <see cref="AccountController"/>.
        /// </summary>
        /// <param name="userMgr">user manager Identity.</param>
        /// <param name="signInMgr">connection manager Identity.</param>
        public AccountController(UserManager<IdentityUser> userMgr,
                                 SignInManager<IdentityUser> signInMgr)
        {
            _userManager = userMgr;
            _signInManager = signInMgr;
        }

        /// <summary>
        /// Displays connection page.
        /// </summary>
        /// <param name="returnUrl">URL to redirect after connection.</param>
        /// <returns>A view connection form .</returns>
        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }

        /// <summary>
        /// Handles connection form submission.
        /// </summary>
        /// <param name="loginModel">Model that represents connection informations.</param>
        /// <returns>Redirects to URL, otherwise displays errors view.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByNameAsync(loginModel.Name);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    if ((await _signInManager.PasswordSignInAsync(user,
                        loginModel.Password, false, false)).Succeeded)
                    {
                        return Redirect(loginModel.ReturnUrl ?? "/Admin/Index");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid name or password");
            return View(loginModel);
        }

        /// <summary>
        /// Logs out the user and redirects to a specified URL.
        /// </summary>
        /// <param name="returnUrl">Redirect URL after logout (default "/").</param>
        /// <returns>Redirects to the specified URL.</returns>
        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}