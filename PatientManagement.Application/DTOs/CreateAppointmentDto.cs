using System.ComponentModel.DataAnnotations;

namespace PatientManagement.Application.DTOs;

public class CreateAppointmentDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Patient ID must be greater than 0.")]
    public int PatientId { get; set; }

    [Required]
    public DateTime? AppointmentDate { get; set; }

    [Required]
    [Length(2, 100)]
    public string DoctorName { get; set; } = string.Empty;

    [Required]
    [Length(5, 500)]
    public string Reason { get; set; } = string.Empty;
}