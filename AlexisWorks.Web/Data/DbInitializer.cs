using AlexisWorks.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AlexisWorks.Web.Data;

public static class DbInitializer
{
    public static void Initialize(WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();

            if (context.Services.Any()) return;

            var services = new Service[]
            {
                new Service { Name = "Plumbing", HourlyRate = 500 },
                new Service { Name = "Electrical", HourlyRate = 600 },
                new Service { Name = "Masonry", HourlyRate = 450 },
                new Service { Name = "Carpentry Works", HourlyRate = 550 },
                new Service { Name = "Others", HourlyRate = 400 }
            };

            context.Services.AddRange(services);
            context.SaveChanges();
        }
    }
}
