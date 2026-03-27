using AlexisWorks.Web.Models;

namespace AlexisWorks.Web.ViewModels;

public class DashboardViewModel
{
    public int TotalClients { get; set; }
    public int TotalBookings { get; set; }
    public int UnpaidBillings { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<Booking> RecentBookings { get; set; } = new();
    public List<Billing> UnpaidBills { get; set; } = new();
}
