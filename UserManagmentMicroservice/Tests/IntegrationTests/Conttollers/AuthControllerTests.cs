using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagmentMicroservice.API.Controllers;
using UserManagmentMicroservice.Aplication.Dtos;
using UserManagmentMicroservice.Aplication.Interfaces;
using UserManagmentMicroservice.Aplication.Models;

using Xunit;

namespace UserManagmentMicroservice.Tests.IntegrationTests.Conttollers
{
	public class AuthControllerTests
	{
		private readonly Mock<IUserService> _userServiceMock;
		private readonly AuthController _authController;

		public AuthControllerTests()
		{
			_userServiceMock = new Mock<IUserService>();
			_authController = new AuthController(_userServiceMock.Object);
		}

		[Fact]
		public async Task Register_ReturnsOk_WhenRegistrationIsSuccessful()
		{
			// Arrange
			var userDto = new UserDto { Name = "Test User", Email = "test@example.com" };
			string password = "Password123";

			_userServiceMock.Setup(us => us.RegisterUserAsync(userDto, password))
				.ReturnsAsync(OperationResult<bool>.SuccessResult(true));

			// Act
			var result = await _authController.Register(userDto, password);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Registration successful! Please check your email to confirm your account.", okResult.Value);
		}

		[Fact]
		public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
		{
			// Arrange
			var userDto = new UserDto { Name = "Test User", Email = "test@example.com" };
			string password = "Password123";

			_userServiceMock.Setup(us => us.RegisterUserAsync(userDto, password))
				.ReturnsAsync(OperationResult<bool>.Failure("Registration failed."));

			// Act
			var result = await _authController.Register(userDto, password);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Registration failed.", badRequestResult.Value);
		}

		[Fact]
		public async Task Login_ReturnsOk_WhenLoginIsSuccessful()
		{
			// Arrange
			var loginRequest = new LoginRequestDto { Email = "test@example.com", Password = "Password123" };
			var token = "dummyToken";

			_userServiceMock.Setup(us => us.AuthenticateAsync(loginRequest))
				.ReturnsAsync(OperationResult<string>.SuccessResult(token));

			// Act
			var result = await _authController.Login(loginRequest);

			// Assert
			var responseData = Assert.IsType<Dictionary<string, object>>(OkResult.Equals);
			Assert.True(responseData.ContainsKey("Token"));
			Assert.Equal(token, responseData["Token"]);
		}

		[Fact]
		public async Task Login_ReturnsUnauthorized_WhenLoginFails()
		{
			// Arrange
			var loginRequest = new LoginRequestDto { Email = "test@example.com", Password = "WrongPassword" };

			_userServiceMock.Setup(us => us.AuthenticateAsync(loginRequest))
				.ReturnsAsync(OperationResult<string>.Failure("Invalid credentials."));

			// Act
			var result = await _authController.Login(loginRequest);

			// Assert
			var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
			Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
		}

		[Fact]
		public async Task ConfirmEmail_ReturnsOk_WhenEmailIsConfirmed()
		{
			// Arrange
			string token = "validToken";

			_userServiceMock.Setup(us => us.ConfirmEmailAsync(token))
				.ReturnsAsync(OperationResult<bool>.SuccessResult(true));

			// Act
			var result = await _authController.ConfirmEmail(token);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Email confirmed successfully!", okResult.Value);
		}

		[Fact]
		public async Task ConfirmEmail_ReturnsBadRequest_WhenConfirmationFails()
		{
			// Arrange
			string token = "invalidToken";

			_userServiceMock.Setup(us => us.ConfirmEmailAsync(token))
				.ReturnsAsync(OperationResult<bool>.Failure("Invalid token."));

			// Act
			var result = await _authController.ConfirmEmail(token);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid token.", badRequestResult.Value);
		}

		[Fact]
		public async Task ForgotPassword_ReturnsOk_WhenEmailIsSent()
		{
			// Arrange
			string email = "test@example.com";

			_userServiceMock.Setup(us => us.ForgotPasswordAsync(email))
				.ReturnsAsync(OperationResult<bool>.SuccessResult(true));

			// Act
			var result = await _authController.ForgotPassword(email);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Password reset link has been sent to your email.", okResult.Value);
		}

		[Fact]
		public async Task ForgotPassword_ReturnsBadRequest_WhenSendingFails()
		{
			// Arrange
			string email = "test@example.com";

			_userServiceMock.Setup(us => us.ForgotPasswordAsync(email))
				.ReturnsAsync(OperationResult<bool>.Failure("Error sending reset email."));

			// Act
			var result = await _authController.ForgotPassword(email);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Error sending reset email.", badRequestResult.Value);
		}

		[Fact]
		public async Task ResetPassword_ReturnsOk_WhenPasswordIsReset()
		{
			// Arrange
			var resetPasswordDto = new ResetPasswordDto { Token = "validToken", NewPassword = "NewPassword123" };

			_userServiceMock.Setup(us => us.ResetPasswordAsync(resetPasswordDto.Token, resetPasswordDto.NewPassword))
				.ReturnsAsync(OperationResult<bool>.SuccessResult(true));

			// Act
			var result = await _authController.ResetPassword(resetPasswordDto);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			Assert.Equal("Password has been reset successfully.", okResult.Value);
		}

		[Fact]
		public async Task ResetPassword_ReturnsBadRequest_WhenResetFails()
		{
			// Arrange
			var resetPasswordDto = new ResetPasswordDto { Token = "invalidToken", NewPassword = "NewPassword123" };

			_userServiceMock.Setup(us => us.ResetPasswordAsync(resetPasswordDto.Token, resetPasswordDto.NewPassword))
				.ReturnsAsync(OperationResult<bool>.Failure("Invalid token."));

			// Act
			var result = await _authController.ResetPassword(resetPasswordDto);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
			Assert.Equal("Invalid token.", badRequestResult.Value);
		}
	}
}
