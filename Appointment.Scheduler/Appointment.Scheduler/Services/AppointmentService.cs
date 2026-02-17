using AppointmentScheduler.Models;
using AppointmentScheduler.Data;
using Microsoft.EntityFrameworkCore;

namespace AppointmentScheduler.Services;

public class AppointmentService
{
    private readonly AppointmentContext _context;
    private readonly EmailService _emailService;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(AppointmentContext context, EmailService emailService, ILogger<AppointmentService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
    {
        try
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Send confirmation email
            await _emailService.SendAppointmentConfirmationAsync(
                appointment.ClientEmail,
                appointment.ClientName,
                appointment.StartTime,
                appointment.Subject);

            return appointment;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating appointment: {ex.Message}");
            throw;
        }
    }

    public async Task<List<Appointment>> GetAppointmentsAsync()
    {
        return await _context.Appointments.ToListAsync();
    }

    public async Task<Appointment?> GetAppointmentAsync(int id)
    {
        return await _context.Appointments.FindAsync(id);
    }

    public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
    {
        try
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating appointment: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> CancelAppointmentAsync(int id)
    {
        try
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Cancelled;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error cancelling appointment: {ex.Message}");
            return false;
        }
    }
}