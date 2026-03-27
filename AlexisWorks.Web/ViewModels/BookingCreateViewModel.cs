using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AlexisWorks.Web.ViewModels;

public class BookingCreateViewModel
{
    [Required(ErrorMessage = "Please select a client.")]
    [Display(Name = "Client")]
    public int ClientId { get; set; }

    [Required(ErrorMessage = "Please select a date and time.")]
    [Display(Name = "Schedule Date & Time")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime ScheduleDate { get; set; } = DateTime.Now;

    public List<BookingLineItemViewModel> LineItems { get; set; } = new()
    {
        new BookingLineItemViewModel()
    };

    // Dropdown data
    public List<SelectListItem> Clients { get; set; } = new();
    public List<SelectListItem> Services { get; set; } = new();
}

public class BookingLineItemViewModel
{
    [Required(ErrorMessage = "Please select a service.")]
    [Display(Name = "Service")]
    public int ServiceId { get; set; }

    [Required]
    [Range(0.25, 100, ErrorMessage = "Hours must be between 0.25 and 100.")]
    [Display(Name = "Hours Rendered")]
    public decimal HoursRendered { get; set; } = 1;
}
