using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Interfaces;

public interface IRefreshTokenService
{
    Task<LoginResponse> LoginAsync(string userName);

    Task<LoginResponse> RefreshTokenAsync(string refreshToken);
}
