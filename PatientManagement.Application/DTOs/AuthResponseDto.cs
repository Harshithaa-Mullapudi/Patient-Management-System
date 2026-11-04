namespace PatientManagement.Application.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public DateTime ExpiresAtUtc { get; set; }

    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;
}