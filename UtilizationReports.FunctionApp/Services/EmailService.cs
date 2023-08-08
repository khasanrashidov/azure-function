using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using UtilizationReports.FunctionApp.Models;

namespace UtilizationReports.FunctionApp.Services
{
	public class EmailService : IEmailService
	{
		private readonly ILogger<EmailService> _logger;
		private readonly IEnvironmentVarService _envVar;

		public EmailService(ILogger<EmailService> logger, IEnvironmentVarService envVar)
		{
			_logger = logger;
			_envVar = envVar;
		}

		public (string subject, string body) GenerateEmail(UtilizationReport utilizationReport)
		{
			var subject = $"Utilization Report ({_envVar.GetEnvironmentVariable("ReportIsForDayFromDayTo")})";

			var templatePath = Path.Combine("Templates", "EmailTemplate.html");
			var templateContent = File.ReadAllText(templatePath);

			var template = Handlebars.Compile(templateContent);
			var templateData = new
			{
				AccountName = utilizationReport.AccountName,
				DateFrom = utilizationReport.DateFrom.ToString("yyyy-MM-dd"),
				DateTo = utilizationReport.DateTo.ToString("yyyy-MM-dd"),
				TotalSpentTime = utilizationReport.TotalSpentTime / 3600,
				ProjectTime = utilizationReport.ProjectTimeTotal / 3600,
				ProjectTimePercentage = utilizationReport.ProjectTimePercentage,
				VacationTime = utilizationReport.VacationTimeTotal / 3600,
				VacationTimePercentage = utilizationReport.VacationTimePercentage,
				BenchTime = utilizationReport.BenchTimeTotal / 3600,
				BenchTimePercentage = utilizationReport.BenchTimePercentage
			};

			// Render the template with the dynamic data
			string body = template(templateData);

			_logger.LogInformation("Email body generated successfully.");

			return (subject, body);
		}

		public async Task SendEmailAsync(string toAddress, string subject, string body)
		{
			MailMessage message = new(_envVar.GetEnvironmentVariable("SenderEmail"), toAddress, subject, body)
			{
				IsBodyHtml = true
			};

			using (SmtpClient smtpClient = new(_envVar.GetEnvironmentVariable("SmtpClientHost"),
											int.Parse(_envVar.GetEnvironmentVariable("SmtpClientPort")))
			{
				EnableSsl = true,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(_envVar.GetEnvironmentVariable("SenderEmail"),
											_envVar.GetEnvironmentVariable("SenderEmailPassword"))
			})
			{
				try
				{
					await smtpClient.SendMailAsync(message);
					_logger.LogInformation("Email sent successfully to {toAddress}.", toAddress);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error sending email");
					throw;
				}
			}
		}
	}
}
