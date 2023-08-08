using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using UtilizationReports.FunctionApp.Models;

namespace UtilizationReports.FunctionApp.Services
{
    public class UtilizationReportsService : IUtilizationReportsService
    {
        private readonly ILogger<UtilizationReportsService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEnvironmentVarService _envVar;

        public UtilizationReportsService(ILogger<UtilizationReportsService> logger, IHttpClientFactory httpClientFactory, IEnvironmentVarService envVar)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _envVar = envVar;
        }

        public async Task<List<UtilizationReport>> GetUtilizationReportAsync(string accountName, DateTime dateFrom, DateTime dateTo)
        {
            UriBuilder uriBuilder = new(_envVar.GetEnvironmentVariable("UtilizationReportApiUrl"));
            var query = new StringBuilder();
            query.Append($"accountName={accountName}");
            query.Append($"&dateFrom={dateFrom.ToString("yyyy-MM-dd")}");
            query.Append($"&dateTo={dateTo.ToString("yyyy-MM-dd")}");
            uriBuilder.Query = query.ToString();

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync(uriBuilder.Uri);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                List<UtilizationReport>? utilizationReport = JsonConvert.DeserializeObject<List<UtilizationReport>>(content);
                _logger.LogInformation("Successfully fetched utilization report for Account: {AccountName}", utilizationReport![0].AccountName);
                return utilizationReport!;
            }
            else
            {
                _logger.LogError("Error getting utilization report. Status code: {StatusCode}", response.StatusCode);
                Exception exception = new($"Error getting utilization report. Status code: {response.StatusCode}");
                throw exception;
            }
        }
    }
}
