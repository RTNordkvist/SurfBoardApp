using SurfBoardApp.Data.Models;

namespace SurfBoardApp.Test
{
    [TestClass]
    public class ApplicationUserTests
    {
        [TestMethod]
        public void FullNameTest()
        {
            // Arrange
            var user = new ApplicationUser()
            {
                FirstName = "John",
                LastName = "Doe"
            };

            // Act
            var fullName = user.FullName;

            // Assert
            Assert.AreEqual("John Doe", fullName);
        }
    }
}