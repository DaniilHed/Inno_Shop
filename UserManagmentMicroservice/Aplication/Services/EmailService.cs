using System.Net.Mail;
using System.Net;

namespace UserManagmentMicroservice.Aplication.Services
{
	public class EmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string message)
		{
			var smtpClient = new SmtpClient
			{
				Host = _configuration["EmailSettings:SmtpHost"],
				Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
				Credentials = new NetworkCredential(_configuration["EmailSettings:SmtpUser"], _configuration["EmailSettings:SmtpPass"]),
				EnableSsl = true,
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
				Subject = subject,
				Body = message,
				IsBodyHtml = true,
			};

			mailMessage.To.Add(toEmail);

			await smtpClient.SendMailAsync(mailMessage);
		}
	}
}
