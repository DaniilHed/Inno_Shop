using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagmentMicroservice.Aplication.Dtos;
using UserManagmentMicroservice.Aplication.Interfaces;
using UserManagmentMicroservice.Aplication.Services;
using UserManagmentMicroservice.Domain.Entities;
using UserManagmentMicroservice.Domain.Enums;
using UserManagmentMicroservice.Domain.Interfaces;
using Xunit;

namespace UserManagmentMicroservice.Tests.UnitTests.Services
{
	public class UserServiceTests
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<TokenGenerator> _tokenGeneratorMock;
		private readonly Mock<EmailService> _emailServiceMock;
		private readonly Mock<IUrlHelper> _urlHelperMock;
		private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
		private readonly UserService _userService;

		public UserServiceTests()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			_tokenGeneratorMock = new Mock<TokenGenerator>();
			_emailServiceMock = new Mock<EmailService>();
			_urlHelperMock = new Mock<IUrlHelper>();
			_httpContextAccessorMock = new Mock<IHttpContextAccessor>();

			_userService = new UserService(
				_userRepositoryMock.Object,
				_tokenGeneratorMock.Object,
				_emailServiceMock.Object,
				_urlHelperMock.Object,
				_httpContextAccessorMock.Object);
		}
		[Fact]
		public async Task RegisterUserAsync_ShouldReturnFailure_WhenUserExists()
		{
			
			var userDto = new UserDto
			{
				Name = "Test User",
				Email = "test@example.com",
				Role = UserRole.User
			};

			_userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(userDto.Email)).ReturnsAsync(new UserIdentity
			{
				Id = Guid.NewGuid(),
				Name = userDto.Name,
				Email = userDto.Email,
				Role = userDto.Role,
				PasswordHash = "hashedPassword",
				IsEmailConfirmed = false
			});

			
			var result = await _userService.RegisterUserAsync(userDto, "password");

			
			Assert.False(result.Success);
			Assert.Equal("User with the provided email already exists.", result.ErrorMessage);
		}

		[Fact]
		public async Task RegisterUserAsync_ShouldSendEmail_WhenUserIsRegistered()
		{
			
			var userDto = new UserDto
			{
				Name = "Test User",
				Email = "test@example.com",
				Role = UserRole.User
			};

			_userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(userDto.Email)).ReturnsAsync((UserIdentity)null);

			// Setup the user to be added
			_userRepositoryMock.Setup(repo => repo.AddUserAsync(It.IsAny<UserIdentity>())).Returns(Task.CompletedTask);

			// Generate token for email confirmation
			_tokenGeneratorMock.Setup(tg => tg.GenerateEmailConfirmationToken(It.IsAny<UserIdentity>())).Returns("token");

			// Setup URL for confirmation link
			_urlHelperMock.Setup(url => url.Action(
				"ConfirmEmail",
				"Auth",
				It.IsAny<object>(),
				"https",
				"yourdomain.com"))
				.Returns("https://yourdomain.com/Auth/ConfirmEmail?userId=someId&token=token");

			
			var result = await _userService.RegisterUserAsync(userDto, "password");

			
			Assert.True(result.Success);
			_emailServiceMock.Verify(es => es.SendEmailAsync(userDto.Email, "Confirm your account",
				It.Is<string>(s => s.Contains("Please confirm your account by clicking <a href=\"https://yourdomain.com/Auth/ConfirmEmail?userId=someId&token=token\">here</a>."))), Times.Once);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ShouldReturnFailure_WhenTokenIsInvalid()
		{
			
			string token = "invalidToken";
			string userId = Guid.NewGuid().ToString();

			_tokenGeneratorMock.Setup(tg => tg.ValidateEmailConfirmationToken(token, out userId)).Returns(false);

			
			var result = await _userService.ConfirmEmailAsync(token);

			
			Assert.False(result.Success);
			Assert.Equal("Invalid email confirmation token.", result.ErrorMessage);
		}

		[Fact]
		public async Task ConfirmEmailAsync_ShouldReturnSuccess_WhenTokenIsValid()
		{
			
			string token = "validToken";
			string userId = Guid.NewGuid().ToString();
			var user = new UserIdentity
			{
				Id = Guid.Parse(userId),
				Name = "Test User",
				Email = "test@example.com",
				Role = UserRole.User,
				PasswordHash = "hashedPassword",
				IsEmailConfirmed = false
			};

			_tokenGeneratorMock.Setup(tg => tg.ValidateEmailConfirmationToken(token, out userId)).Returns(true);
			_userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(Guid.Parse(userId))).ReturnsAsync(user);

			
			var result = await _userService.ConfirmEmailAsync(token);

			
			Assert.True(result.Success);
			Assert.True(user.IsEmailConfirmed);
			_userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
		}

		[Fact]
		public async Task ForgotPasswordAsync_ShouldReturnFailure_WhenUserDoesNotExist()
		{
			
			string email = "nonexistent@example.com";

			_userRepositoryMock.Setup(repo => repo.FindUserByEmailAsync(email)).ReturnsAsync((UserIdentity)null);

			
			var result = await _userService.ForgotPasswordAsync(email);

			
			Assert.False(result.Success);
			Assert.Equal("User with this email does not exist.", result.ErrorMessage);
		}

		[Fact]
		public async Task ResetPasswordAsync_ShouldReturnFailure_WhenTokenIsInvalid()
		{
		
			string token = "invalidToken";
			string newPassword = "newPassword";

			_tokenGeneratorMock.Setup(tg => tg.ValidateEmailConfirmationToken(token, out It.Ref<string>.IsAny)).Returns(false);

			
			var result = await _userService.ResetPasswordAsync(token, newPassword);

			
			Assert.False(result.Success);
			Assert.Equal("Invalid password reset token.", result.ErrorMessage);
		}

		[Fact]
		public async Task ResetPasswordAsync_ShouldReturnSuccess_WhenTokenIsValid()
		{
			
			string token = "validToken";
			string userId = Guid.NewGuid().ToString();
			var user = new UserIdentity
			{
				Id = Guid.Parse(userId),
				Name = "Test User",
				Email = "test@example.com",
				Role = UserRole.User,
				PasswordHash = "oldHash",
				IsEmailConfirmed = false
			};

			_tokenGeneratorMock.Setup(tg => tg.ValidateEmailConfirmationToken(token, out userId)).Returns(true);
			_userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(Guid.Parse(userId))).ReturnsAsync(user);
			_userRepositoryMock.Setup(repo => repo.UpdateUserAsync(user)).Returns(Task.CompletedTask);

			
			var result = await _userService.ResetPasswordAsync(token, "newPassword");

			
			Assert.True(result.Success);
			_userRepositoryMock.Verify(repo => repo.UpdateUserAsync(user), Times.Once);
			Assert.Equal("newPassword", user.PasswordHash); 
		}

		[Fact]
		public async Task DeleteUserAsync_ShouldReturnFailure_WhenUserDoesNotExist()
		{
			
			Guid userId = Guid.NewGuid();
			_userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((UserIdentity)null);

			
			var result = await _userService.DeleteUserAsync(userId);

			
			Assert.False(result.Success);
			Assert.Equal("User not found", result.ErrorMessage);
		}

		[Fact]
		public async Task DeleteUserAsync_ShouldReturnSuccess_WhenUserExists()
		{
			
			Guid userId = Guid.NewGuid();
			var user = new UserIdentity
			{
				Id = userId,
				Name = "Test User",
				Email = "test@example.com",
				Role = UserRole.User,
				PasswordHash = "hashedPassword",
				IsEmailConfirmed = false
			};

			_userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
			_userRepositoryMock.Setup(repo => repo.DeleteUserAsync(userId)).Returns(Task.CompletedTask);

			
			var result = await _userService.DeleteUserAsync(userId);

			
			Assert.True(result.Success);
			_userRepositoryMock.Verify(repo => repo.DeleteUserAsync(userId), Times.Once);
		}
	}
}
