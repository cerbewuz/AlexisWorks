using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlexisWorks.Web.Models;

public class Billing
{
    public int Id { get; set; }

    // Foreign Key
    public int BookingId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountDue { get; set; }

    [Required]
    [MaxLength(20)]
    public string PaymentStatus { get; set; } = "Unpaid";

    public DateTime? PaymentDate { get; set; }

    // Navigation
    public Booking Booking { get; set; } = null!;
}
