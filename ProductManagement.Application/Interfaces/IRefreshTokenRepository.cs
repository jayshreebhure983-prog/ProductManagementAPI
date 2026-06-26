using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);

    Task<RefreshToken?> GetByTokenAsync(string token);

    void Delete(RefreshToken refreshToken);
}
