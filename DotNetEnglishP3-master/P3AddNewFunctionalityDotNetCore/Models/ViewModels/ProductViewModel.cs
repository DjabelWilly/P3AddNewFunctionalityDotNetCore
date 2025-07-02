using P3AddNewFunctionalityDotNetCore.Resources.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public string Name { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorMissingStock")]
        [Range(1, int.MaxValue,
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorStockValue")]
        public int Stock { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorMissingPrice")]
        [Range(0.01, double.MaxValue,
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorPriceValue")]
        public double Price { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }
    }
}
