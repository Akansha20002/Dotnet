namespace CourseTech.Core.DTOs.Authentication
{
    public record TokenDTO
    (
        string AccessToken,
        DateTime AcccesTokenExpiresAt,
        string RefreshToken,
        DateTime RefreshTokenExpiresAt
    );
}