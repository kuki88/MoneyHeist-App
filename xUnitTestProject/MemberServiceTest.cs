using MonesyHeist_App;
using MonesyHeist_App.Controllers;
using MonesyHeist_App.Data;
using MonesyHeist_App.Data.Services;
using Moq;

namespace xUnitTestProject
{
    public class MemberServiceTest
    {
        private readonly Mock<AppDbContext> dbMock = new Mock<AppDbContext>();
        private readonly MemberService _service;
        private readonly MemberController _controller;

        public MemberServiceTest()
        {
            //_service = new MemberService(dbMock.Object);
            _controller = new MemberController(dbMock.Object, _service);
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