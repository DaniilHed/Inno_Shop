
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagmentMicroservice.Aplication.Dtos;
using UserManagmentMicroservice.Aplication.Interfaces;
using UserManagmentMicroservice.Aplication.Mappers;
using UserManagmentMicroservice.Aplication.Models;
using UserManagmentMicroservice.Domain.Entities;
using UserManagmentMicroservice.Domain.Interfaces;

namespace UserManagmentMicroservice.Aplication.Services
{
	public class UserService:IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly TokenGenerator _tokenGenerator;
		private readonly EmailService _emailService;
		private readonly IUrlHelper _urlHelper;
		private readonly HttpRequest _request;
		public UserService(IUserRepository userRepository, TokenGenerator tokenGenerator,EmailService emailService, IUrlHelper urlHelper, IHttpContextAccessor httpContextAccessor)
		{
			_userRepository = userRepository;
			_tokenGenerator = tokenGenerator;
			_emailService = emailService;
			_urlHelper = urlHelper;
			_request = httpContextAccessor.HttpContext?.Request;
		}
		public UserService(IUserRepository userRepository, EmailService emailService)
		{
			_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
			_emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
		}

		public async Task<OperationResult<UserDto>> GetUserByIdAsync(Guid id)
		{
			var user = await _userRepository.GetUserByIdAsync(id);
			
			return OperationResult<UserDto>.SuccessResult(user.ToUserDto());
		}

		public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
		{
			var users = await _userRepository.GetAllUsersAsync();
			return users.Select(u=>u.ToUserDto());
		}

		public async Task<OperationResult<bool>> CreateUserAsync(UserDto userDto, string password)
		{
			var existingUser = await _userRepository.FindUserByEmailAsync(userDto.Email);
			if (existingUser != null) return OperationResult<bool>.Failure("User with this email already exists");

			var newUser = new UserIdentity
			{
				Id = Guid.NewGuid(),
				Name = userDto.Name,
				Email = userDto.Email,
				Role = userDto.Role,
				PasswordHash = HashPassword(password),
				IsEmailConfirmed = false
			};
			await _userRepository.AddUserAsync(newUser);
			return OperationResult<bool>.SuccessResult(true);

		}

		public async Task<OperationResult<bool>> UpdateUserAsync(UserDto userDto)
		{
			var user = await _userRepository.GetUserByIdAsync(userDto.Id);
			if (user is null) return OperationResult<bool>.Failure("User not found");
			

			user.Name = userDto.Name;
			user.Email = userDto.Email;
			user.Role = userDto.Role;
			await _userRepository.UpdateUserAsync(user);
			return OperationResult<bool>.SuccessResult(true) ;

		}

		public async Task<OperationResult<bool>> DeleteUserAsync(Guid id)

		{
			var user = await _userRepository.GetUserByIdAsync(id);
			if (user is null) return OperationResult<bool>.Failure("User not found");


			await _userRepository.DeleteUserAsync(id);
			return OperationResult<bool>.SuccessResult(true);

		}


		public async Task<OperationResult<bool>> RegisterUserAsync(UserDto userDto,string password)
		{
			var existingUser = await _userRepository.FindUserByEmailAsync(userDto.Email);
			if (existingUser != null) return OperationResult<bool>.Failure("User with the provided email already exists.");

			// Создаем пользователя
			var user = new UserIdentity
			{
				Id = Guid.NewGuid(),
				Name = userDto.Name,
				Email = userDto.Email,
				Role = userDto.Role,
				PasswordHash = HashPassword(password),
				IsEmailConfirmed = false
			};

			await _userRepository.AddUserAsync(user);

			// Генерируем токен подтверждения
			var token = _tokenGenerator.GenerateEmailConfirmationToken(user);

			// Ссылка для подтверждения
			var confirmationLink = _urlHelper.Action(
			action: "ConfirmEmail",
		    controller: "Auth",
		    values: new { userId = user.Id, token = token },
		    protocol: "https",
		    host: "yourdomain.com"
		    );

			// Отправляем подтверждающее письмо
			await _emailService.SendEmailAsync(user.Email, "Confirm your account",
				$"Please confirm your account by clicking <a href=\"{confirmationLink}\">here</a>.");
			return OperationResult<bool>.SuccessResult(true);
		}

		public async Task<OperationResult<bool>> ConfirmEmailAsync( string token)
		{
			string userId;

			
			if (!_tokenGenerator.ValidateEmailConfirmationToken(token, out userId)) 
				return OperationResult<bool>.Failure("Invalid email confirmation token.");
			

			var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));

			if (user == null) return OperationResult<bool>.Failure("User not found.");
			
			user.IsEmailConfirmed = true;
			await _userRepository.UpdateUserAsync(user);
			
			return OperationResult<bool>.SuccessResult(true);

		}

		public async Task<OperationResult<bool>> ForgotPasswordAsync(string email)
		{
			
			var user = await _userRepository.FindUserByEmailAsync(email);
			if (user == null)
				return OperationResult<bool>.Failure("User with this email does not exist.");

			var token = _tokenGenerator.GeneratePasswordResetToken(user);

			var resetLink = _urlHelper.Action(
			action: "ConfirmEmail", 
			controller: "Auth", 
			values: new { email = email, token = token }, 
			protocol: _request.Scheme 
		);



			await _emailService.SendEmailAsync(user.Email, "Password Reset",
				$"You can reset your password by clicking <a href=\"{resetLink}\">here</a>.");

			return OperationResult<bool>.SuccessResult(true);
		}

		public async Task<OperationResult<bool>> ResetPasswordAsync(string token, string newPassword)
		{
			string userId;

			if (!_tokenGenerator.ValidateEmailConfirmationToken(token, out userId))
			{
				return OperationResult<bool>.Failure("Invalid password reset token.");
			}

			var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));
			if (user == null) return OperationResult<bool>.Failure("User not found.");
			
			user.PasswordHash = HashPassword(newPassword); 
			await _userRepository.UpdateUserAsync(user);
			
			return OperationResult<bool>.SuccessResult(true);
		}

		public async Task<OperationResult<string>> AuthenticateAsync(LoginRequestDto loginRequest)
		{
			var user = await _userRepository.FindUserByEmailAsync(loginRequest.Email);
			if (user == null || !VerifyPassword(user.PasswordHash, loginRequest.Password))
				return OperationResult<string>.Failure("Invalid email or password.");

			var token = _tokenGenerator.GenerateToken(user);
			return OperationResult<string>.SuccessResult(token);
		}


		private bool VerifyPassword(string password, string storedHash)
		{
			return password == storedHash; // Implement password hashing for production use
		}

		private string HashPassword(string password)
		{
			return password; // Implement password hashing for production use
		}

		
	}
}

