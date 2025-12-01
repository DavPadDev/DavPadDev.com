using System.ComponentModel.DataAnnotations;

namespace DavPadDev.Models;

public class ContactFormModel
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(60, ErrorMessage = "Name is too long.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Message is required.")]
    [MinLength(5, ErrorMessage = "Message must be at least 5 characters.")]
    [StringLength(1000, ErrorMessage = "Message is too long.")]
    public string Message { get; set; } = "";
}