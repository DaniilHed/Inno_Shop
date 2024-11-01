using Microsoft.AspNetCore.Mvc;
using UserManagmentMicroservice.Aplication.Dtos;
using UserManagmentMicroservice.Aplication.Interfaces;

namespace UserManagmentMicroservice.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IUserService _userService;

		public AuthController(IUserService userService)
		{
			_userService = userService;
		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] UserDto userDto, string password)
		{
			var result = await _userService.RegisterUserAsync(userDto, password);
			if (!result.Success)
				return BadRequest(result.ErrorMessage);

			return Ok("Registration successful! Please check your email to confirm your account.");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
		{
			if (!ModelState.IsValid)
            {
                return BadRequest("Invalid login request.");
            }

            var result = await _userService.AuthenticateAsync(loginRequest);
            
            if (!result.Success)
            {
                return Unauthorized(result.ErrorMessage);
            }

            return Ok(new { Token = result.Data });
		}
		[HttpGet("confirm-email")]
		public async Task<IActionResult> ConfirmEmail( string token)
		{
			var result = await _userService.ConfirmEmailAsync(token);
			if (!result.Success)
				return BadRequest(result.ErrorMessage);

			return Ok("Email confirmed successfully!");
		}
		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] string email)
		{
			var result = await _userService.ForgotPasswordAsync(email);
			if (!result.Success)
				return BadRequest(result.ErrorMessage);

			return Ok("Password reset link has been sent to your email.");
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
		{
			var result = await _userService.ResetPasswordAsync( resetPasswordDto.Token, resetPasswordDto.NewPassword);
			if (!result.Success)
				return BadRequest(result.ErrorMessage);

			return Ok("Password has been reset successfully.");
		}
	}
}
