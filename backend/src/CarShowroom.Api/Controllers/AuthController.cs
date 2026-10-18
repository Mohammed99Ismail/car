using CarShowroom.Api.Controllers;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Auth;
using CarShowroom.Application.Features.Auth.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<CustomApiResponse<AuthResponse>>> Register(
        [FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<CustomApiResponse<AuthResponse>>> Login(
        [FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<CustomApiResponse<AuthResponse>>> RefreshToken(
        [FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> Logout(
        [FromBody] RefreshTokenRequest? request, CancellationToken cancellationToken)
    {
        var result = await authService.LogoutAsync(request?.RefreshToken, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> ForgotPassword(
        [FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var origin = Request.Headers.Origin.FirstOrDefault()
            ?? Request.Headers.Referer.FirstOrDefault()
            ?? $"{Request.Scheme}://{Request.Host}";

        var result = await authService.ForgotPasswordAsync(request, origin, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> ResetPassword(
        [FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ResetPasswordAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CustomApiResponse<CurrentUserDto>>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await authService.GetCurrentUserAsync(cancellationToken);
        return CustomResult(result);
    }
}
