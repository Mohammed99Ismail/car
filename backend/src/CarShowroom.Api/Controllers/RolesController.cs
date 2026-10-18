using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Roles;
using CarShowroom.Application.Features.Roles.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

public class RolesController(IRoleService roleService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.Roles.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<PagedListModel<RoleSummaryDto>>>> GetRoles(
        [FromQuery] QueryParameters query, CancellationToken cancellationToken)
    {
        var result = await roleService.GetRolesAsync(query, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Roles.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<RoleDetailDto>>> GetRoleById(
        int id, CancellationToken cancellationToken)
    {
        var result = await roleService.GetRoleByIdAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionKeysConstant.Roles.CREATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<RoleDetailDto>>> CreateRole(
        [FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await roleService.CreateRoleAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Roles.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<RoleDetailDto>>> UpdateRole(
        int id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await roleService.UpdateRoleAsync(id, request, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Roles.DELETE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteRole(
        int id, CancellationToken cancellationToken)
    {
        var result = await roleService.DeleteRoleAsync(id, cancellationToken);
        return CustomResult(result);
    }
}
