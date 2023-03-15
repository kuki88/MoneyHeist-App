using MonesyHeist_App;
using MonesyHeist_App.Controllers;
using MonesyHeist_App.Data;
using MonesyHeist_App.Data.Services;
using Moq;

namespace xUnitTestProject
{
    public class HeistServiceTest
    {
        private readonly Mock<AppDbContext> dbMock = new Mock<AppDbContext>();
        private readonly HeistService _service;
        private readonly HeistController _controller;

        public HeistServiceTest()
        {
             _service = new HeistService(dbMock.Object);
             _controller = new HeistController(dbMock.Object, _service);
        }


        [Fact]
        public void FirstTest()
        {
            //Arrange


            //Act


            //Assert
            Assert.Equal(1, 1);
        }
    }
}