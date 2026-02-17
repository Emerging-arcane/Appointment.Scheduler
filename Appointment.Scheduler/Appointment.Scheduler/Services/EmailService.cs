using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;

namespace AppointmentScheduler.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAppointmentConfirmationAsync(string recipientEmail, string clientName, DateTime appointmentTime, string subject)
    {
        try
        {
            var smtpServer = _configuration["EmailService:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailService:SmtpPort"] ?? "587");
            var senderEmail = _configuration["EmailService:SenderEmail"];
            var senderPassword = _configuration["EmailService:SenderPassword"];

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(senderEmail));
            email.To.Add(MailboxAddress.Parse(recipientEmail));
            email.Subject = $"Appointment Confirmation - {subject}";

            var htmlBody = $@"
                <html>
                    <body>
                        <h2>Appointment Confirmation</h2>
                        <p>Dear {clientName},</p>
                        <p>Your appointment has been scheduled successfully.</p>
                        <p><strong>Appointment Details:</strong></p>
                        <ul>
                            <li><strong>Subject:</strong> {subject}</li>
                            <li><strong>Date & Time:</strong> {appointmentTime:F}</li>
                        </ul>
                        <p>Thank you for scheduling with us!</p>
                        <p>Best regards,<br/>Appointment Scheduler Team</p>
                    </body>
                </html>";

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlBody
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderEmail, senderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation($"Email sent successfully to {recipientEmail}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email: {ex.Message}");
            throw;
        }
    }
}