namespace CarShowroom.Application.Features.Auth.Dtos;

public record RegisterRequest(string FullName, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record RefreshTokenRequest(string RefreshToken);
public record ForgotPasswordRequest(string Email);
public record ResetPasswordRequest(string Token, string NewPassword);

public record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt,
    string RefreshToken,
    CurrentUserDto User);

public record CurrentUserDto(
    int Id,
    string FullName,
    string Email,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
