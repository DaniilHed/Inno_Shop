using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagmentMicroservice.API.Controllers;
using UserManagmentMicroservice.Aplication.Dtos;
using UserManagmentMicroservice.Aplication.Interfaces;
using UserManagmentMicroservice.Aplication.Models;
using Xunit;

namespace UserManagmentMicroservice.Tests.IntegrationTests.Conttollers
{
	public class UserControllerTests
	{
		private readonly Mock<IUserService> _userServiceMock;
		private readonly UserController _userController;

		public UserControllerTests()
		{
			_userServiceMock = new Mock<IUserService>();
			_userController = new UserController(_userServiceMock.Object);
		}

		[Fact]
		public async Task GetUserById_ReturnsOk_WhenUserExists()
		{
			
			var userId = Guid.NewGuid();
			var userDto = new UserDto { Id = userId, Name = "Test User", Email = "test@example.com" };
			_userServiceMock.Setup(us => us.GetUserByIdAsync(userId))
				.ReturnsAsync(OperationResult<UserDto>.SuccessResult(userDto));

			
			var result = await _userController.GetUserById(userId);

			
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedUser = Assert.IsType<UserDto>(okResult.Value);
			Assert.Equal(userId, returnedUser.Id);
		}

		[Fact]
		public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
		{
			
			var userId = Guid.NewGuid();
			_userServiceMock.Setup(us => us.GetUserByIdAsync(userId))
				.ReturnsAsync(OperationResult<UserDto>.Failure("User not found"));

			
			var result = await _userController.GetUserById(userId);

			
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("User not found", notFoundResult.Value);
		}

		[Fact]
		public async Task CreateUser_ReturnsCreated_WhenUserIsCreated()
		{
			
			var userDto = new UserDto { Id = Guid.NewGuid(), Name = "New User", Email = "newuser@example.com" };
			string password = "Password123";
			_userServiceMock.Setup(us => us.CreateUserAsync(userDto, password))
				.ReturnsAsync(OperationResult<bool>.SuccessResult(true));

			
			var result = await _userController.CreateUser(userDto, password);

			
			var createdResult = Assert.IsType<CreatedAtActionResult>(result);
			Assert.Equal(nameof(UserController.GetUserById), createdResult.ActionName);
			Assert.Equal(userDto.Id, createdResult.RouteValues["id"]);
		}

		[Fact]
		public async Task CreateUser_ReturnsBadRequest_WhenUserCannotBeCreated()
		{
			
			var userDto = new UserDto { Name = "Existing User", Email = "existinguser@example.com" };
			string password = "Password123";
			_userServiceMock.Setup(us => us.CreateUserAsync(userDto, password))
				.ReturnsAsync(OperationResult<bool>.Failure("User with this email already exists"));

			
			var result = await _userController.CreateUser(userDto, password);

			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("User with this email already exists", badRequestResult.Value);
		}

		[Fact]
		public async Task UpdateUser_ReturnsNoContent_WhenUserIsUpdated()
		{
			
			var userId = Guid.NewGuid();
			var userDto = new UserDto { Id = userId, Name = "Updated User", Email = "updateduser@example.com" };

			_userServiceMock.Setup(us => us.UpdateUserAsync(userDto))
				.ReturnsAsync(OperationResult<bool>.SuccessResult(true));

			
			var result = await _userController.UpdateUser(userId, userDto);

			
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
		{
			
			var userId = Guid.NewGuid();
			var userDto = new UserDto { Id = userId, Name = "Updated User", Email = "updateduser@example.com" };

			_userServiceMock.Setup(us => us.UpdateUserAsync(userDto))
				.ReturnsAsync(OperationResult<bool>.Failure("User not found"));

			
			var result = await _userController.UpdateUser(userId, userDto);

			
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("User not found", notFoundResult.Value);
		}

		[Fact]
		public async Task DeleteUser_ReturnsNoContent_WhenUserIsDeleted()
		{
			
			var userId = Guid.NewGuid();
			_userServiceMock.Setup(us => us.DeleteUserAsync(userId))
				.ReturnsAsync(OperationResult<bool>.SuccessResult(true));

			
			var result = await _userController.DeleteUser(userId);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
		{
			
			var userId = Guid.NewGuid();
			_userServiceMock.Setup(us => us.DeleteUserAsync(userId))
				.ReturnsAsync(OperationResult<bool>.Failure("User not found"));

		
			var result = await _userController.DeleteUser(userId);

			
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal("User not found", notFoundResult.Value);
		}

		[Fact]
		public async Task GetAllUsers_ReturnsOk_WhenUsersExist()
		{
			
			var userDtoList = new List<UserDto>
		{
			new UserDto { Id = Guid.NewGuid(), Name = "User 1", Email = "user1@example.com" },
			new UserDto { Id = Guid.NewGuid(), Name = "User 2", Email = "user2@example.com" }
		};

			_userServiceMock.Setup(us => us.GetAllUsersAsync())
				.ReturnsAsync(userDtoList);

			
			var result = await _userController.GetAllUsers();

			
			var okResult = Assert.IsType<OkObjectResult>(result);
			var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
			Assert.Equal(2, returnedUsers.Count());
		}
	}
}
