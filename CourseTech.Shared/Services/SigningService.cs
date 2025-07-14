using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CourseTech.Shared.Services
{
    public static class SigningService
    {
        public static SecurityKey GetSymmetricSecurityKey(string secret)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        }
    }
}