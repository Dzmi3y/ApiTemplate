namespace Core.Models
{
    public enum IdentityErrorCode
    {
        RefreshTokenNotExists,
        RefreshTokenExpired,
        RefreshTokenInvalidated,
        RefreshTokenUsed,
        NoAssociatedUser
    }

}
