using MonesyHeist_App;
using MonesyHeist_App.Controllers;
using MonesyHeist_App.Data;
using MonesyHeist_App.Data.Services;
using Moq;

namespace xUnitTestProject
{
    public class UnitTest
    {
        [Fact]
        public void FirstTest()
        {
            //Arrange
            var mock = new Mock<AppDbContext>();
            var service = new MemberService(mock.Object);
            var controller = new MemberController(mock.Object, service);
            //Act

            //Assert
            Assert.Equal(1, 1);
        }
    }
}