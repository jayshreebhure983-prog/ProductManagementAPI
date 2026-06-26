namespace ProductManagement.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(string userName);
}
