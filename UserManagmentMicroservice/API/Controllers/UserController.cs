using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagmentMicroservice.Aplication.Dtos;
using UserManagmentMicroservice.Aplication.Interfaces;

namespace UserManagmentMicroservice.API.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserById(Guid id)
		{
			var user = await _userService.GetUserByIdAsync(id);
			if (user == null) return NotFound("User not found");
			return Ok(user);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> CreateUser([FromBody] UserDto userDto, string password)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var result = await _userService.CreateUserAsync(userDto, password);
			if (!result.Success) return BadRequest(result.ErrorMessage);

			await _userService.CreateUserAsync(userDto, password);
			return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
		}
		[Authorize(Roles = "Admin,User")]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto userDto)
		{
			userDto.Id = id;
			var result = await _userService.UpdateUserAsync(userDto);
			if (!result.Success) return NotFound(result.ErrorMessage);
			return NoContent(); // Return 204 if update is successful
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteUser(Guid id)
		{
			var result = await _userService.DeleteUserAsync(id);
			if (!result.Success) return NotFound(result.ErrorMessage);
			return NoContent(); // Return 204 if delete is successful
		}

		[HttpGet]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await _userService.GetAllUsersAsync();
			return Ok(users);
		}
	}
}
