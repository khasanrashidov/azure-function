namespace UtilizationReports.FunctionApp.Services
{
    public class EnvironmentVarService : IEnvironmentVarService
    {
        public string GetEnvironmentVariable(string envVar)
        {
            return Environment.GetEnvironmentVariable(envVar, EnvironmentVariableTarget.Process)!;
        }
    }
}
