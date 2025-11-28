using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace api.Services;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly string _apiKey;
    private readonly string _toEmail;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;

        _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? "";
        _toEmail = Environment.GetEnvironmentVariable("CONTACT_TO_EMAIL") ?? "";
        _fromEmail = Environment.GetEnvironmentVariable("CONTACT_FROM_EMAIL") ?? _toEmail;
        _fromName = Environment.GetEnvironmentVariable("CONTACT_FROM_NAME") ?? "DavPadDev Contact Form";
    }

    public async Task<bool> SendContactEmailAsync(string name, string email, string message)
    {
        if (string.IsNullOrWhiteSpace(_apiKey) || string.IsNullOrWhiteSpace(_toEmail))
        {
            _logger.LogError("SendGrid API key or CONTACT_TO_EMAIL is missing.");
            return false;
        }

        var client = new SendGridClient(_apiKey);

        var from = new EmailAddress(_fromEmail, _fromName);
        var to = new EmailAddress(_toEmail);

        var subject = $"New contact form message from {name}";
        var plainTextContent = $"Name: {name}\nEmail: {email}\n\nMessage:\n{message}";

        var encodedMessage = WebUtility.HtmlEncode(message).Replace("\n", "<br/>");
        var htmlContent = $@"
            <p><strong>Name:</strong> {name}</p>
            <p><strong>Email:</strong> {email}</p>
            <p><strong>Message:</strong></p>
            <p>{encodedMessage}</p>";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        // makes Reply button go back to whoever filled the form
        msg.ReplyTo = new EmailAddress(email, name);

        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation("SendGrid response status: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Body.ReadAsStringAsync();
            _logger.LogError("SendGrid error body: {Body}", body);
        }

        return response.IsSuccessStatusCode;
    }
}
