using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UtilizationReports.FunctionApp.Middlewares;
using UtilizationReports.FunctionApp.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder => // builder is IFunctionsWorkerApplicationBuilder
    {
        builder.UseCustomExceptionHandler();
    })
    .ConfigureServices(services =>
    {
        // Register services (DI)
        services.AddLogging();
        services.AddHttpClient();
        services.AddScoped<IUtilizationReportsService, UtilizationReportsService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEnvironmentVarService, EnvironmentVarService>();
    })
    .Build();

host.Run();
