using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using NuGet.DependencyResolver;
using System;

namespace P3AddNewFunctionalityDotNetCore.ModelBinders
{
    /// <summary>
    /// Provides a custom model binder for double types.
    /// Returns an instance of <see cref="InvariantDoubleModelBinder"/> when the model type is <c>double</c>.
    /// </summary>
    public class InvariantDoubleModelBinderProvider : IModelBinderProvider
    {
        // Méthode appelée automatiquement par le framework.
        // Elle décide quel binder utiliser pour le type de propriété que le binding essaie de traiter.
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            // Vérifie que le paramètre context n'est pas nul, sinon on lance une exception.
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // Si le type à binder est un double, retourne le custom binder
            if (context.Metadata.ModelType == typeof(double) )
            {
                return new BinderTypeModelBinder(typeof(InvariantDoubleModelBinder));
            }
            // Si le type n’est pas double, on ne fournit aucun binder
            return null;
        }
    }
}
