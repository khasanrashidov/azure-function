using Microsoft.Extensions.Logging;
using Moq;
using UtilizationReports.FunctionApp.Functions;
using UtilizationReports.FunctionApp.Models;
using UtilizationReports.FunctionApp.Services;

namespace AzureFunctionApp.UnitTests
{
	public class AzureFunctionAppUnitTests
	{
		[Fact]
		public async Task TimerTriggerFunction_RunAsync_Success()
		{
			// Arrange
			var loggerMock = new Mock<ILogger<TimerTriggerFunction>>();
			var utilizationReportsServiceMock = new Mock<IUtilizationReportsService>();
			var emailServiceMock = new Mock<IEmailService>();
			var envVarServiceMock = new Mock<IEnvironmentVarService>();

			var timerTriggerFunction = new TimerTriggerFunction(
				loggerMock.Object,
				utilizationReportsServiceMock.Object,
				emailServiceMock.Object,
				envVarServiceMock.Object
			);

			var myInfo = new MyInfo { ScheduleStatus = new MyScheduleStatus(), IsPastDue = false };

			var fakeAccounts = "Account1;Account2";
			var fakeEmails = "email1@example.com;email2@example.com";
			var fakeDateFrom = "-7";
			var fakeDateTo = "-1";
			var fakeNumberOfConcurrentTasks = "2";

			envVarServiceMock.Setup(e => e.GetEnvironmentVariable("Accounts")).Returns(fakeAccounts);
			envVarServiceMock.Setup(e => e.GetEnvironmentVariable("ReceiverEmails")).Returns(fakeEmails);
			envVarServiceMock.Setup(e => e.GetEnvironmentVariable("DateFrom")).Returns(fakeDateFrom);
			envVarServiceMock.Setup(e => e.GetEnvironmentVariable("DateTo")).Returns(fakeDateTo);
			envVarServiceMock.Setup(e => e.GetEnvironmentVariable("NumberOfConcurrentTasks")).Returns(fakeNumberOfConcurrentTasks);

			var fakeUtilizationReport = new List<UtilizationReport>
			{
				new UtilizationReport()
				{
					AccountName = "AccountAny",
					DateFrom = DateTime.Now.AddDays(-7),
					DateTo = DateTime.Now.AddDays(-1),
					TotalSpentTime = 40,
					ProjectTimeTotal = 30,
					VacationTimeTotal = 5,
					BenchTimeTotal = 5,
					ProjectTimePercentage = 75,
					VacationTimePercentage = 12.5,
					BenchTimePercentage = 12.5
				}
			};

			utilizationReportsServiceMock.Setup(u => u.GetUtilizationReportAsync(It.IsAny<string>(), 
																				 It.IsAny<DateTime>(), 
																				 It.IsAny<DateTime>()))
																	.ReturnsAsync(fakeUtilizationReport);

			// Act
			await timerTriggerFunction.RunAsync(myInfo);

			// Assert
			emailServiceMock.Verify(e => e.GenerateEmail(fakeUtilizationReport[0]), Times.Exactly(2));
			emailServiceMock.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
		}
	}
}
