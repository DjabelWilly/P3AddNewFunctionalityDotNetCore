using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.ModelBinders
{
    /// <summary>
    /// Provides a custom model binder for double types,
    /// allowing commas and dots as decimal separators.
    /// Uses <see cref="CultureInfo.InvariantCulture"/> for parsing.
    /// </summary>
    public class InvariantDoubleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Récupère la valeur du champ en fonction de son nom
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            // Retourne "Task successfull" si aucune valeur n'est fournit.
            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            // Retourne "Task successfull" si la valeur est vide ou ne contient que des espaces,
            string value = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(value))
                return Task.CompletedTask;

            // Accepte virgule ou point
            value = value.Replace(',', '.');

            //Tente de convertir la chaîne en double selon la culture "invariante" (CultureInfo.InvariantCulture), qui utilise le point . comme séparateur.
            // Si la conversion réussit, on affecte la valeur dans bindingContext.Result
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                bindingContext.Result = ModelBindingResult.Success(result);

            // Si la conversion échoue (ex: "abc", "12..3"), on ajoute une erreur de validation à ModelState.
            else
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Format de prix invalide.");

            return Task.CompletedTask;
        }
    }
}
