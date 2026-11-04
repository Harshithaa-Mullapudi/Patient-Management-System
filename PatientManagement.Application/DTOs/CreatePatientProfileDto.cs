using System.ComponentModel.DataAnnotations;

namespace PatientManagement.Application.DTOs;

public class CreatePatientProfileDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be greater than 0.")]
    public int PatientId { get; set; }

    [Required]
    [Length(5, 200)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [Length(2, 100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [Length(2, 100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [Length(4, 10)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [Length(2, 100)]
    public string EmergencyContactName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string EmergencyContactNumber { get; set; } = string.Empty;
}
