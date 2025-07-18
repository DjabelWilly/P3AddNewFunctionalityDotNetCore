using Microsoft.AspNetCore.Mvc.ModelBinding;
using P3AddNewFunctionalityDotNetCore.Resources.Models;
using System.ComponentModel.DataAnnotations;

namespace P3AddNewFunctionalityDotNetCore.Models.ViewModels
{
    public class ProductViewModel
    {
        [BindNever]
        public int Id { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorMissingName")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ0-9\s'-]{2,100}$",
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorInvalidName")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorMissingPrice")]
        [Range(0.01, double.MaxValue,
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorPriceValue")]
        public double Price { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorMissingStock")]
        [RegularExpression(@"^-?\d+$",
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "StockNotAnInteger")]
        [Range(1, int.MaxValue,
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorStockValue")]
        public int Stock { get; set; }
    }
}
