using CourseTech.Core.Models.Authentication;
using CourseTech.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CourseTech.Repository.Repositories
{
    public class AppUserRefreshTokenRepository(AppDbContext context) : IAppUserRefreshTokenRepository
    {
        public async Task<AppUserRefreshToken?> GetAsync(Expression<Func<AppUserRefreshToken, bool>> predicate)
        {
            return await context.AppUserRefreshToken.SingleOrDefaultAsync(predicate);
        }

        public async Task AddRefreshTokenAsync(AppUserRefreshToken refreshToken)
        {
            await context.AppUserRefreshToken.AddAsync(refreshToken);
        }

        public async Task RemoveRefreshTokenAsync(string token)
        {
            var refreshToken = await context.AppUserRefreshToken
                .FirstOrDefaultAsync(x => x.Token == token);

            if (refreshToken != null)
            {
                context.AppUserRefreshToken.Remove(refreshToken);
            }
        }
    }
}