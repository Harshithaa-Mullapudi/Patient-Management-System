using PatientManagement.Domain.Enums;

namespace PatientManagement.Application.DTOs;

public class AppointmentResponseDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}