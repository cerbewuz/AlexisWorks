using AlexisWorks.Web.Models;

namespace AlexisWorks.Web.ViewModels;

public class ReportsViewModel
{
    public List<Billing> Billings { get; set; } = new();
    public int? ClientIdFilter { get; set; }
    public List<Client> Clients { get; set; } = new();
    public decimal TotalPaid { get; set; }
    public decimal TotalUnpaid { get; set; }
}
