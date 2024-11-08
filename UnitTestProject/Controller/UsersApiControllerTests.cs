using FakeItEasy;
using Microsoft.Extensions.Configuration;
using RAZSmartDesk.DataAccess.Repositories.IRepositories;
using RAZSmartDesk.Entities;
using RAZSmartDesk.API.Controllers;

namespace UnitTests.Controller
{
    public class UsersApiControllerTests
    {
        private readonly IAppUserRepository _usersRepository;

        public UsersApiControllerTests()
        {
            _usersRepository = A.Fake<IAppUserRepository>();
        }

        [Fact]
        public void UsersApiController_Get_ReturnOK()
        {
            // Arrange
            var users = A.Fake<ICollection<User>>();
            var usersList = A.Fake<List<User>>();
            //A.CallTo(() => _usersRepository.)

            // Act
            // var controller = new UsersApiController();
            


            //Assert
        }
    }
}