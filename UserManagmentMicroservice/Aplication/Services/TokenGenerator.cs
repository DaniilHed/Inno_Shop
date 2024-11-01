using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagmentMicroservice.Aplication.Models;
using UserManagmentMicroservice.Domain.Entities;

namespace UserManagmentMicroservice.Aplication.Services
{
	public class TokenGenerator
	{
		private readonly JwtSettings _jwtSettings;
		private readonly IConfiguration _configuration;

		public TokenGenerator(IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
		{
			_jwtSettings = jwtSettings.Value;
			_configuration = configuration;
		}

		public string GenerateToken(UserIdentity user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

			var claims = new List<Claim>
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(JwtRegisteredClaimNames.Email, user.Email),
			new Claim(ClaimTypes.Role, user.Role.ToString())
			};

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
				Issuer = _jwtSettings.Issuer,
				Audience = _jwtSettings.Audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key.Key), SecurityAlgorithms.HmacSha256Signature)
			};


			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = tokenHandler.CreateToken(tokenDescriptor);


			return tokenHandler.WriteToken(token);
		}
		public string GeneratePasswordResetToken(UserIdentity user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

			var claims = new List<Claim>
			{
			 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // ID пользователя
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Уникальный идентификатор токена
             new Claim(JwtRegisteredClaimNames.Email, user.Email) // Email пользователя
            };

			// Настройка токена, с более коротким сроком действия для безопасности
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddHours(1), // Токен действителен в течение 1 часа
				Issuer = _jwtSettings.Issuer,
				Audience = _jwtSettings.Audience,
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
		public string GenerateEmailConfirmationToken(UserIdentity user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

			var claims = new List<Claim>
			{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // ID пользователя
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Уникальный идентификатор токена
            new Claim(JwtRegisteredClaimNames.Email, user.Email) // Email пользователя
            };

			// Настройка токена с истечением через 24 часа
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(1), // Токен действителен в течение 1 дня
				Issuer = _jwtSettings.Issuer,
				Audience = _jwtSettings.Audience,
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		public bool ValidateEmailConfirmationToken(string token, out string userId)
		{
			userId = null;

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

			try
			{
				var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = true,
					ValidIssuer = _jwtSettings.Issuer,
					ValidateAudience = true,
					ValidAudience = _jwtSettings.Audience,
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				// Получение userId из токена
				userId = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
