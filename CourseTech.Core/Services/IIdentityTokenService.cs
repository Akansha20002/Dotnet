using CourseTech.Core.DTOs.Authentication;
using CourseTech.Core.Models;

namespace CourseTech.Core.Services
{
    public interface IIdentityTokenService
    {
        TokenDTO CreateToken(AppUser user);
    }
}