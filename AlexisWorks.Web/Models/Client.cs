using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlexisWorks.Web.Models;

public class Client
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(50)]
    [Display(Name = "Middle Name")]
    public string MiddleName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(10)]
    public string? Suffix { get; set; }

    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    // UI-only granular address fields (Not mapped to DB columns)
    [NotMapped]
    [Required]
    [MaxLength(150)]
    [Display(Name = "Street/Village/Barangay")]
    public string StreetVillageBarangay { get; set; } = string.Empty;

    [NotMapped]
    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [NotMapped]
    [Required]
    [MaxLength(100)]
    public string Province { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [RegularExpression(@"^\+?63\d{10}$|^9\d{9}$", ErrorMessage = "Phone number must be 10 digits starting with 9 (e.g., 9123456789) or start with +63.")]
    public string Phone { get; set; } = string.Empty;

    // Helper properties
    [NotMapped]
    public string Name => $"{FirstName} {MiddleName} {LastName} {Suffix}".Trim().Replace("  ", " ");

    // Navigation
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
