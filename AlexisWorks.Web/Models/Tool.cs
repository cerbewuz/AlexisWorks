using System.ComponentModel.DataAnnotations;

namespace AlexisWorks.Web.Models;

public class Tool
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Foreign Key
    public int ServiceId { get; set; }

    // Navigation
    public Service Service { get; set; } = null!;
}
