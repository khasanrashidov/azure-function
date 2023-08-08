using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using UtilizationReports.FunctionApp.Services;

namespace UtilizationReports.FunctionApp.Functions
{
	public class TimerTriggerFunction
	{
		private readonly ILogger<TimerTriggerFunction> _logger;
		private readonly IUtilizationReportsService _utilizationReportsService;
		private readonly IEmailService _emailService;
		private readonly IEnvironmentVarService _envVar;

		public TimerTriggerFunction(ILogger<TimerTriggerFunction> logger,
									IUtilizationReportsService utilizationReportsService,
									IEmailService emailService,
									IEnvironmentVarService envVar)
		{
			_logger = logger;
			_utilizationReportsService = utilizationReportsService;
			_emailService = emailService;
			_envVar = envVar;
		}

		[Function("TimerTriggerFunction")]
		public async Task RunAsync([TimerTrigger("0 0 18 * * 5")] MyInfo myTimer) // every Friday at 6pm
		{
			_logger.LogInformation("C# Timer trigger function executed at: {time}", DateTime.Now);

			var accounts = _envVar.GetEnvironmentVariable("Accounts").Split(';').ToList();
			var emails = _envVar.GetEnvironmentVariable("ReceiverEmails").Split(';').ToList();

			var semaphore = new SemaphoreSlim(int.Parse(_envVar.GetEnvironmentVariable("NumberOfConcurrentTasks")));

			var tasks = new List<Task>();
			foreach (var account in accounts)
			{
				var utilizationReport = await _utilizationReportsService.GetUtilizationReportAsync(account,
											DateTime.Now.AddDays(int.Parse(_envVar.GetEnvironmentVariable("DateFrom"))),
											DateTime.Now.AddDays(int.Parse(_envVar.GetEnvironmentVariable("DateTo"))));

				foreach (var report in utilizationReport)
				{
					await semaphore.WaitAsync();

					var (subject, body) = _emailService.GenerateEmail(report);
					var task = _emailService.SendEmailAsync(emails[accounts.IndexOf(account)], subject, body)
											.ContinueWith(t => semaphore.Release());

					tasks.Add(task);
				}
			}
			await Task.WhenAll(tasks);
		}
	}

	public class MyInfo
	{
		public MyScheduleStatus? ScheduleStatus { get; set; }

		public bool IsPastDue { get; set; }
	}

	public class MyScheduleStatus
	{
		public DateTime Last { get; set; }

		public DateTime Next { get; set; }

		public DateTime LastUpdated { get; set; }
	}
}
