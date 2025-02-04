namespace VaccineChildren.Application.DTOs.Response;

public class RevenueDataRes
{
    public int Year { get; set; }
    public double TotalRevenue { get; set; }
    public List<MonthlyRevenue> MonthlyRevenueList { get; set; } = new List<MonthlyRevenue>();
    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public double Amount { get; set; }
    }
}