using PatientManagement.Domain.Enums;

namespace PatientManagement.Domain.Entities;

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
    public DateTime CreatedAt { get; set; }
    public Patient Patient { get; set; } = null!;

    public void CompleteAppointment()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException(
                "Only scheduled appointments can be completed.");
        }

        Status = AppointmentStatus.Completed;
    }

    public void CancelAppointment()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException(
                "Only scheduled appointments can be cancelled.");
        }

        Status = AppointmentStatus.Cancelled;
    } 
}