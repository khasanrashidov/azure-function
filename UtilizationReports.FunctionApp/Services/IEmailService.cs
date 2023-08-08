using UtilizationReports.FunctionApp.Models;

namespace UtilizationReports.FunctionApp.Services
{
    public interface IEmailService
    {
        public (string subject, string body) GenerateEmail(UtilizationReport utilizationReport);
        public Task SendEmailAsync(string toAddress, string subject, string body);
    }
}
