using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(c => c.Products).Returns(new Product[]{
                new Product{ ProductID=1, Name="P1"},
                new Product{ ProductID=2, Name="P2"},
                new Product{ ProductID=3, Name="P3"}
            });

            //Arrange
            AdminController target = new AdminController(mock.Object);

            //Action
            Product[] products = ((IEnumerable<Product>)target.Index().Model).ToArray();

            //Assert
            Assert.AreEqual(products.Length, 3);
            Assert.AreEqual("P1", products[0].Name);
            Assert.AreEqual("P2", products[1].Name);
            Assert.AreEqual("P3", products[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ ProductID=1, Name="P1"},
                new Product{ ProductID=2, Name="P2"},
                new Product{ ProductID=3, Name="P3"}
            });

            //Arrange
            AdminController target = new AdminController(mock.Object);

            //Action
            Product p1 = (Product)target.Edit(1).ViewData.Model;
            Product p2 = (Product)target.Edit(2).ViewData.Model;
            Product p3 = (Product)target.Edit(3).ViewData.Model;

            //Assert
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ ProductID=1, Name="P1"},
                new Product{ ProductID=2, Name="P2"},
                new Product{ ProductID=3, Name="P3"}
            });

            //Arrange
            AdminController target = new AdminController(mock.Object);

            //Action
            Product p1 = (Product)target.Edit(4).ViewData.Model;
            //Assert
            Assert.IsNull(p1);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ Name="P1", ProductID=1}
            });

            //Arrange
            Product product = new Product {  ProductID=4, Name="Product 4"};
            AdminController target = new AdminController(mock.Object);

            //Act
            ActionResult ac= target.Edit(product);
            
            //Assert - check that the repository was called
            mock.Verify(m => m.SaveProduct(product));

            //Assert - check the method result type
            Assert.IsNotInstanceOfType(ac, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            
            //Arrange
            AdminController target = new AdminController(mock.Object);

            //Arrange - create a product
            Product product = new Product { Name = "Test" };

            //Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");

            //Act - try to save the product
            ActionResult result = target.Edit(product);

            //Assert - check that the repository was not called
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            //Assert - check the method result type
            Assert.IsInstanceOfType(result,typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            //Arrange - create a product
            Product prod = new Product { ProductID=2,Name="Test" };

            //Arrange
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]{
                new Product{ProductID=1,Name="P1"},
                prod,
                new Product{ProductID=3,Name="P3"}
            });

            //Arrange
            AdminController target = new AdminController(mock.Object);

            //Act - delete the product
            target.Delete(prod.ProductID);

            //Assert
            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }
    }
}
