using CarShowroom.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CarShowroom.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();
    private static readonly object DummyUser = new();

    public string Hash(string password)
    {
        return _hasher.HashPassword(DummyUser, password);
    }

    public bool Verify(string hash, string password)
    {
        if (string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(password))
            return false;

        var result = _hasher.VerifyHashedPassword(DummyUser, hash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
