using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlexisWorks.Web.Models;

public class Service
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HourlyRate { get; set; }

    // Navigation
    public ICollection<Tool> Tools { get; set; } = new List<Tool>();
    public ICollection<BookingLineItem> BookingLineItems { get; set; } = new List<BookingLineItem>();
}
