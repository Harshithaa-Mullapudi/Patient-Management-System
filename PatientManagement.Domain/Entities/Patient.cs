using PatientManagement.Domain.Enums;

namespace PatientManagement.Domain.Entities;

public class Patient
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; private set; }
    public Gender Gender { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public PatientProfile PatientProfile { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public void UpdateContactInformation(string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new ArgumentException(
                "Email and Phone Number can't be null, empty or whitespace.");
        }
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public void UpdateAge(int age)
    {
        if (age < 0 || age > 120)
        {
            throw new ArgumentOutOfRangeException(
                nameof(age),
                "Age must be between 0 and 120.");
        }

        Age = age;
    }

    public Patient(string firstName, string lastName, int age, Gender gender, string email, string phoneNumber, DateTime dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdateAge(age);
        Gender = gender;
        UpdateContactInformation(email, phoneNumber);
        DateOfBirth = dateOfBirth;
        CreatedAt = DateTime.UtcNow;
    }
}