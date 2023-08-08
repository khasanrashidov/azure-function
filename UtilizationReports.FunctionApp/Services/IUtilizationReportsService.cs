using UtilizationReports.FunctionApp.Models;

namespace UtilizationReports.FunctionApp.Services
{
    public interface IUtilizationReportsService
    {
        public Task<List<UtilizationReport>> GetUtilizationReportAsync(string accountName, DateTime dateFrom, DateTime dateTo);
    }
}
