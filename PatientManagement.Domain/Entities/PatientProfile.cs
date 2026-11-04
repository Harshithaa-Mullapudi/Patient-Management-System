namespace PatientManagement.Domain.Entities;

public class PatientProfile
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactNumber { get; set; } = string.Empty;
    public Patient Patient { get; set; } = null!;
}