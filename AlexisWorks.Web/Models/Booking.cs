using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlexisWorks.Web.Models;

public class Booking
{
    public int Id { get; set; }

    // Foreign Key
    public int ClientId { get; set; }

    [Required]
    public DateTime ScheduleDate { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    // Navigation
    public Client Client { get; set; } = null!;
    public ICollection<BookingLineItem> LineItems { get; set; } = new List<BookingLineItem>();
    public Billing? Billing { get; set; }
}
