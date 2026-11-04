using System.ComponentModel.DataAnnotations;

namespace PatientManagement.Application.DTOs;

public class RegisterRequestDto
{
    [Required]
    [Length(2, 50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Length(2, 50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}