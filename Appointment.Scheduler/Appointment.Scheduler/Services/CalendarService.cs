using Appointment.Scheduler.Models;
using AppointmentScheduler.Models;

namespace AppointmentScheduler.Services;

public class CalendarService
{
    private readonly AppointmentContext _context;
    private readonly ILogger<CalendarService> _logger;

    public CalendarService(AppointmentContext context, ILogger<CalendarService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DateTime>> GetAvailableSlotsAsync(DateTime date, int slotDurationMinutes = 30)
    {
        try
        {
            var dayOfWeek = date.DayOfWeek;
            var availabilitySlots = await _context.AvailabilitySlots
                .Where(s => s.DayOfWeek == dayOfWeek && s.IsActive)
                .ToListAsync();

            var bookedAppointments = await _context.Appointments
                .Where(a => a.StartTime.Date == date && a.Status != AppointmentStatus.Cancelled)
                .ToListAsync();

            var availableSlots = new List<DateTime>();

            foreach (var slot in availabilitySlots)
            {
                var slotStart = date.Date.Add(slot.StartTime);
                var slotEnd = date.Date.Add(slot.EndTime);

                while (slotStart < slotEnd)
                {
                    var slotEndTime = slotStart.AddMinutes(slot.SlotDurationMinutes);

                    // Check if slot is available (not booked)
                    if (!bookedAppointments.Any(a =>
                        (a.StartTime < slotEndTime && a.EndTime > slotStart)))
                    {
                        availableSlots.Add(slotStart);
                    }

                    slotStart = slotEndTime;
                }
            }

            return availableSlots;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting available slots: {ex.Message}");
            return new List<DateTime>();
        }
    }

    public async Task<bool> IsTimeSlotAvailableAsync(DateTime startTime, DateTime endTime)
    {
        var conflicts = await _context.Appointments
            .Where(a => a.StartTime < endTime && a.EndTime > startTime && a.Status != AppointmentStatus.Cancelled)
            .AnyAsync();

        return !conflicts;
    }
}