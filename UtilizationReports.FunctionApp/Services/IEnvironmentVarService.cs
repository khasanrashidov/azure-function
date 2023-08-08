namespace UtilizationReports.FunctionApp.Services
{
    public interface IEnvironmentVarService
    {
        public string GetEnvironmentVariable(string envVar);
    }
}
