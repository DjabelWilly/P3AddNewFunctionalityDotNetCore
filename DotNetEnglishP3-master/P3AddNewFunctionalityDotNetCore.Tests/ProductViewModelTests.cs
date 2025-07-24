using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductViewModelTests
    {
        private static List<ValidationResult> ValidateModel(ProductViewModel model)
        {
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        //================= Validation Name =======================

        [Fact]
        public void Name_WhenEmpty_ShouldReturnErrorMissingName()
        {
            // Arrange
            var model = new ProductViewModel
            {
                //Name absent
                Price = 10.0,
                Stock = 5
            };

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Contains(results, v => v.MemberNames.Contains(nameof(ProductViewModel.Name)));
        }

        [Fact]
        public void Name_WhenInvalidFormat_ShouldReturnErrorInvalidName()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "@@@@", // caractères non autorisés par la regex
                Price = 10.0,
                Stock = 5
            };

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Contains(results, v => v.MemberNames.Contains(nameof(ProductViewModel.Name)));
        }

        //=================== Validation Price =======================

        [Fact]
        public void Price_WhenMissing_ShouldReturnErrorMissingPrice()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "Produit test",
                //Price absent
                Stock = 5
            };

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Contains(results, v => v.MemberNames.Contains(nameof(ProductViewModel.Price)));
        }

        [Fact]
        public void Price_WhenOutOfRange_ShouldReturnErrorPriceValue()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "Produit test",
                Price = -1,  // invalide car Range(0.01, double.MaxValue)
                Stock = 5
            };

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Contains(results, v => v.MemberNames.Contains(nameof(ProductViewModel.Price)));

        }

        //=================== Validation Stock =======================

        [Fact]
        public void Stock_WhenMissing_ShouldReturnErrorMissingStock()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "Produit test",
                Price = 10.0,
                // Stock absent
            };

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Contains(results, v => v.MemberNames.Contains(nameof(ProductViewModel.Stock)));
        }

        //[Fact]
        //public void Stock_WhenNotAnInteger_ShouldReturnStockNotAnInteger()
        //{
        //    // Arrange
        //    var model = new ProductViewModel
        //    {
        //        Name = "Produit test",
        //        Price = 10.0,
        //        Stock = "abc"
        //    };

        //    // Act
        //    var results = ValidateModel(model);

        //    // Assert
        //    Assert.Contains(results, v => v.MemberNames.Contains(nameof(ProductViewModel.Stock)));
        //}

        [Fact]
        public void Stock_WhenOutOfRange_ShouldReturnErrorStockValue()
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "Produit test",
                Price = 10.0,
                Stock = -2  //invalide car Range(1, int.MaxValue)
            };

            // Act
            var result = ValidateModel(model);

            // Assert
            Assert.Contains(result, v => v.MemberNames.Contains(nameof(ProductViewModel.Stock)));

        }
    }
}
