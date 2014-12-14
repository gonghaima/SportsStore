using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Can_Login_With_Valid_Credentials()
        {
            //Arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "secret")).Returns(true);
            
            
            ////Arrange - create the view model
            LoginViewModel lvm = new LoginViewModel() { UserName="admin", Password="secret" };
            
            //Arrange - create the controller
            AccountController target = new AccountController(mock.Object);
            
            ////Act - authenticate using valid credentials
            ActionResult result = target.Login(lvm, "/MyURL");
            

            //Assert
            //Assert.AreEqual(true, mock.Object.Authenticate("admin", "secret"));
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invalid_Credentials() 
        {
            //Arrange
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("badUser", "badPass")).Returns(false);

            //Arrange - create the view model
            LoginViewModel model = new LoginViewModel
            {
                UserName = "badUser",
                Password = "badPass"
            };

            //Arrange - create the controller
            AccountController target = new AccountController(mock.Object);

            //Act
            ActionResult result = target.Login(model, "myURL");

            //Assert
            Assert.IsInstanceOfType(result,typeof(ViewResult));
            //Assert.AreEqual("",((ViewResult)result)
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}
