using Microsoft.EntityFrameworkCore;
using AlexisWorks.Web.Models;

namespace AlexisWorks.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Tool> Tools => Set<Tool>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingLineItem> BookingLineItems => Set<BookingLineItem>();
    public DbSet<Billing> Billings => Set<Billing>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // Tool -> Service (many-to-one)
        modelBuilder.Entity<Tool>(entity =>
        {
            entity.HasOne(t => t.Service)
                  .WithMany(s => s.Tools)
                  .HasForeignKey(t => t.ServiceId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Booking -> Client (many-to-one)
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasOne(b => b.Client)
                  .WithMany(c => c.Bookings)
                  .HasForeignKey(b => b.ClientId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(b => b.ScheduleDate);
        });

        // BookingLineItem -> Booking & Service
        modelBuilder.Entity<BookingLineItem>(entity =>
        {
            entity.HasOne(li => li.Booking)
                  .WithMany(b => b.LineItems)
                  .HasForeignKey(li => li.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(li => li.Service)
                  .WithMany(s => s.BookingLineItems)
                  .HasForeignKey(li => li.ServiceId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Billing -> Booking (one-to-one)
        modelBuilder.Entity<Billing>(entity =>
        {
            entity.HasOne(bi => bi.Booking)
                  .WithOne(b => b.Billing)
                  .HasForeignKey<Billing>(bi => bi.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(bi => bi.PaymentStatus);
        });
    }
}
