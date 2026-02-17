using Appointment.Scheduler.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AppointmentScheduler.Data;

public class AppointmentContext : DbContext
{
    public AppointmentContext(DbContextOptions<AppointmentContext> options) : base(options) { }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Appointment entity
        modelBuilder.Entity<Appointment>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Appointment>()
            .Property(a => a.Status)
            .HasConversion<string>();

        // Configure AvailabilitySlot entity
        modelBuilder.Entity<AvailabilitySlot>()
            .HasKey(a => a.Id);

        // Seed default availability (9 AM to 5 PM, Monday to Friday)
        var availability = new List<AvailabilitySlot>();
        for (int day = 1; day <= 5; day++)
        {
            availability.Add(new AvailabilitySlot
            {
                Id = day,
                DayOfWeek = (DayOfWeek)day,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                SlotDurationMinutes = 30,
                IsActive = true
            });
        }

        modelBuilder.Entity<AvailabilitySlot>().HasData(availability);
    }
}