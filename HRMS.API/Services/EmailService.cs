using HRMS.API.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HRMS.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration configuration;

    public EmailService(
        IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string body)
    {
        var email = new MimeMessage();

        email.From.Add(
            new MailboxAddress(
                configuration["EmailSettings:SenderName"],
                configuration["EmailSettings:SenderEmail"]));

        email.To.Add(
            MailboxAddress.Parse(toEmail));

        email.Subject = subject;

        email.Body = new TextPart("html")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            configuration["EmailSettings:SmtpServer"],
            int.Parse(configuration["EmailSettings:Port"]!),
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            configuration["EmailSettings:Username"],
            configuration["EmailSettings:Password"]);

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }
}