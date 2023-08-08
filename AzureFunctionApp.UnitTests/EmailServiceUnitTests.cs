using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Mail;
using System.Net.Sockets;
using UtilizationReports.FunctionApp.Models;
using UtilizationReports.FunctionApp.Services;

namespace AzureFunctionApp.UnitTests
{
	public class EmailServiceUnitTests
	{
		#region Initialization

		private readonly Mock<ILogger<EmailService>> _loggerMock;
		private readonly Mock<IEnvironmentVarService> _envVarMock;

		public EmailServiceUnitTests()
		{
			_loggerMock = new Mock<ILogger<EmailService>>();
			_envVarMock = new Mock<IEnvironmentVarService>();
		}

		#endregion

		#region Tests

		[Fact]
		public void GenerateEmail_WithValidUtilizationReport_ReturnsTupleWithSubjectAndBody()
		{
			// Arrange
			var utilizationReport = new UtilizationReport()
			{
				AccountName = "TestAccount",
				DateFrom = DateTime.Now.AddDays(-7),
				DateTo = DateTime.Now.AddDays(-1),
				TotalSpentTime = 40,
				ProjectTimeTotal = 30,
				VacationTimeTotal = 5,
				BenchTimeTotal = 5,
				ProjectTimePercentage = 75,
				VacationTimePercentage = 12.5,
				BenchTimePercentage = 12.5
			};

			var emailService = new EmailService(_loggerMock.Object, _envVarMock.Object);

			// Act
			var result = emailService.GenerateEmail(utilizationReport);

			// Assert
			result.Should().BeOfType<(string subject, string body)>();
		}

		#endregion
	}
}
