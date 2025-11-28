using System.Net;
using System.Text.Json;
using api.Models;
using api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace api;

public class Contact
{
    private readonly ILogger<Contact> _logger;
    private readonly EmailService _emailService;

    public Contact(ILogger<Contact> logger, EmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    [Function("Contact")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData req)
    {
        _logger.LogInformation("Contact endpoint hit.");

        ContactRequest? request;

        try
        {
            request = await JsonSerializer.DeserializeAsync<ContactRequest>(
                req.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to deserialize request.");
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new
            {
                success = false,
                error = "Invalid JSON."
            });
        }

        if (request is null)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new
            {
                success = false,
                error = "Request body is empty."
            });
        }

        // Basic validation
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new
            {
                success = false,
                error = "Name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new
            {
                success = false,
                error = "A valid email is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Message) || request.Message.Length < 5)
        {
            return await CreateJsonResponse(req, HttpStatusCode.BadRequest, new
            {
                success = false,
                error = "Message is too short."
            });
        }

        // Send email via SendGrid
        var sent = await _emailService.SendContactEmailAsync(
            request.Name,
            request.Email,
            request.Message);

        if (!sent)
        {
            return await CreateJsonResponse(req, HttpStatusCode.InternalServerError, new
            {
                success = false,
                error = "Message received, but email sending failed."
            });
        }

        return await CreateJsonResponse(req, HttpStatusCode.OK, new
        {
            success = true,
            error = (string?)null
        });
    }

    private static async Task<HttpResponseData> CreateJsonResponse<T>(
        HttpRequestData req,
        HttpStatusCode statusCode,
        T payload)
    {
        var res = req.CreateResponse(statusCode);
        await res.WriteAsJsonAsync(payload);
        return res;
    }
}
