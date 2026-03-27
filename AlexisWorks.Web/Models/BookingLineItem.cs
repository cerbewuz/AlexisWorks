using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlexisWorks.Web.Models;

public class BookingLineItem
{
    public int Id { get; set; }

    // Foreign Keys
    public int BookingId { get; set; }
    public int ServiceId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HoursRendered { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal LineTotal { get; set; }

    // Navigation
    public Booking Booking { get; set; } = null!;
    public Service Service { get; set; } = null!;
}
