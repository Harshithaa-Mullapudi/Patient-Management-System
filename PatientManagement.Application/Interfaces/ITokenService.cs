using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}