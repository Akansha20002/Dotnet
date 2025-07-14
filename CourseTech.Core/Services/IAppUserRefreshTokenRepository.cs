using CourseTech.Core.Models.Authentication;
using System.Linq.Expressions;

namespace CourseTech.Core.Services
{
    public interface IAppUserRefreshTokenRepository
    {
        Task<AppUserRefreshToken?> GetAsync(Expression<Func<AppUserRefreshToken, bool>> predicate);
        Task AddRefreshTokenAsync(AppUserRefreshToken refreshToken);
        Task RemoveRefreshTokenAsync(string token);
    }
}