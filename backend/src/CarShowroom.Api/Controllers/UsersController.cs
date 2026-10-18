using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Users;
using CarShowroom.Application.Features.Users.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

public class UsersController(IUserService userService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.Users.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<PagedListModel<UserSummaryDto>>>> GetUsers(
        [FromQuery] QueryParameters query, CancellationToken cancellationToken)
    {
        var result = await userService.GetUsersAsync(query, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Users.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<UserDetailDto>>> GetUserById(
        int id, CancellationToken cancellationToken)
    {
        var result = await userService.GetUserByIdAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionKeysConstant.Users.CREATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<UserDetailDto>>> CreateUser(
        [FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await userService.CreateUserAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}/roles")]
    [PermissionAuthorize(PermissionKeysConstant.Users.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> UpdateUserRoles(
        int id, [FromBody] UpdateUserRolesRequest request, CancellationToken cancellationToken)
    {
        var result = await userService.UpdateUserRolesAsync(id, request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}/status")]
    [PermissionAuthorize(PermissionKeysConstant.Users.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> ToggleUserStatus(
        int id, [FromBody] ToggleUserStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await userService.ToggleUserStatusAsync(id, request, cancellationToken);
        return CustomResult(result);
    }
}
