using AlexisWorks.Web.Models;

namespace AlexisWorks.Web.ViewModels;

public class BillingListViewModel
{
    public List<Billing> Billings { get; set; } = new();
    public string? StatusFilter { get; set; }
    public int? ClientIdFilter { get; set; }
    public List<Client> Clients { get; set; } = new();
}
