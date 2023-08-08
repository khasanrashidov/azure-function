using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using UtilizationReports.FunctionApp.Models;
using UtilizationReports.FunctionApp.Services;

namespace AzureFunctionApp.UnitTests
{
	public class UtilizationReportsServiceUnitTests
	{
		#region Initialization

		private readonly Mock<ILogger<UtilizationReportsService>> _loggerMock;
		private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
		private readonly Mock<IEnvironmentVarService> _envVarMock;
		private readonly Mock<HttpMessageHandler> _msgHandler;

		public UtilizationReportsServiceUnitTests()
		{
			_loggerMock = new Mock<ILogger<UtilizationReportsService>>();
			_httpClientFactoryMock = new Mock<IHttpClientFactory>();
			_envVarMock = new Mock<IEnvironmentVarService>();
			_msgHandler = new Mock<HttpMessageHandler>();
		}

		#endregion

		#region Test

		[Fact]
		public async Task GetUtilizationReportAsync_WithValidResponseFromApi_ReturnsUtilizationReport()
		{
			// Arrange
			var accountName = "TestAccount";
			var dateFrom = DateTime.Now.AddDays(-7);
			var dateTo = DateTime.Now.AddDays(-1);

			var utilizationReport = new List<UtilizationReport>()
			{
				new UtilizationReport()
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
				}
			};

			var response = new HttpResponseMessage()
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent(JsonConvert.SerializeObject(utilizationReport))
			};

			_msgHandler.Protected()
					   .Setup<Task<HttpResponseMessage>>("SendAsync",
						ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
					   .ReturnsAsync(response);

			var httpClient = new HttpClient(_msgHandler.Object);

			_httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

			_envVarMock.Setup(x => x.GetEnvironmentVariable("UtilizationReportApiUrl")).Returns("https://test.example.com/reports");

			var sut = new UtilizationReportsService(_loggerMock.Object, _httpClientFactoryMock.Object, _envVarMock.Object);

			// Act
			var result = await sut.GetUtilizationReportAsync(accountName, dateFrom, dateTo);

			// Assert
			result.Should().BeOfType<List<UtilizationReport>>().And.NotBeNull();
			result.Count.Should().Be(1);
			result[0].AccountName.Should().Be(accountName);
		}

		#endregion
	}
}
