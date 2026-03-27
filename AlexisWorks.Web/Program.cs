using Microsoft.EntityFrameworkCore;
using AlexisWorks.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// ----- Services -----

// Entity Framework Core + SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MVC with Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ----- Middleware Pipeline -----

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

DbInitializer.Initialize(app);

app.Run();
