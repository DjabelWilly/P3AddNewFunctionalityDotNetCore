using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Resources.Models;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    // Classe mock pour tester Regex sur Stock avec un type string
    public class ProductViewModelMock
    {
        [Required(
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "ErrorMissingStock")]
        [RegularExpression(@"^-?\d+$",
            ErrorMessageResourceType = typeof(Product),
            ErrorMessageResourceName = "StockNotAnInteger")]
        public string Stock { get; set; }
    }

    public class ProductViewModelTests
    {
        private static List<ValidationResult> ValidateModel(object model)
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

        [Fact]
        public void Stock_WhenNotAnInteger_ShouldReturnStockNotAnInteger()
        {
            // Arrange
            var model = new ProductViewModelMock
            {
                Stock = "abc" // string renvoie erreur
            };

            // Act
            var results = ValidateModel(model);

            // Assert
            Assert.Contains(results, v => v.MemberNames.Contains(nameof(ProductViewModelMock.Stock)));
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(-100)]
        public void Stock_WhenOutOfRange_ShouldReturnErrorStockValue(int invalidStock)
        {
            // Arrange
            var model = new ProductViewModel
            {
                Name = "Produit test",
                Price = 10.0,
                Stock = invalidStock
            };

            // Act
            var result = ValidateModel(model);

            // Assert
            Assert.Contains(result, v => v.MemberNames.Contains(nameof(ProductViewModel.Stock)));

        }
    }
}
