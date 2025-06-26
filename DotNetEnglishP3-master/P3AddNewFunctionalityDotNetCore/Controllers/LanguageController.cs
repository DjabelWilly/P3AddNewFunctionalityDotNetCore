using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Controllers
{
    /// <summary>
    /// Controller responsible for managing the different languages
    /// </summary>
    public class LanguageController : Controller
    {
        private readonly ILanguageService _languageService;


        /// <summary>
        /// Initialize new instance de <see cref="LanguageController"/>.
        /// </summary>
        /// <param name="languageService">The service used to manage UI language settings.</param>
        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }


        /// <summary>
        /// Changes the user interface language based on the selected language in the model.
        /// </summary>
        /// <param name="model">The view model containing the selected language.</param>
        /// <param name="returnUrl">The URL to redirect to after changing the language.</param>
        /// <returns>Redirects to the specified return URL.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeUiLanguage(LanguageViewModel model, string returnUrl)
        {
            if (model.Language != null)
            {
                _languageService.ChangeUiLanguage(HttpContext, model.Language);
            }

            return Redirect(returnUrl);
        }
    }
}
