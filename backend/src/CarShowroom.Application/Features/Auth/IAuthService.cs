using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Auth.Dtos;

namespace CarShowroom.Application.Features.Auth;

public interface IAuthService
{
    Task<CustomApiResponse<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> ForgotPasswordAsync(ForgotPasswordRequest request, string clientOrigin, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CurrentUserDto>> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
