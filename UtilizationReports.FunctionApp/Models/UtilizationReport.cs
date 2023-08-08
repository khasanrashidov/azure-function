namespace UtilizationReports.FunctionApp.Models
{
    public record class UtilizationReport
    {
        public string AccountName { get; set; } = null!;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public double TotalSpentTime { get; set; }
        public double ProjectTimeTotal { get; set; }
        public double VacationTimeTotal { get; set; }
        public double BenchTimeTotal { get; set; }
        public double ProjectTimePercentage { get; set; }
        public double VacationTimePercentage { get; set; }
        public double BenchTimePercentage { get; set; }
    }
}
