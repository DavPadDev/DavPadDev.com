namespace api.Models;

public class ContactRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Message { get; set; }
    public string? Honeypot { get; set; }
}
