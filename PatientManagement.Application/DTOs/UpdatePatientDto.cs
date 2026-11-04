using PatientManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using PatientManagement.Application.Validation;

namespace PatientManagement.Application.DTOs;

public class UpdatePatientDto
{
    [Required]
    [Length(2, 50)]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    [Length(2, 50)]
    public string LastName { get; set; } = string.Empty;
    [Range(0, 120)]
    public int Age { get; set; }
    [EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    [NotFutureDate(ErrorMessage = "Date of birth cannot be in the future.")]
    public DateTime? DateOfBirth { get; set; }
}