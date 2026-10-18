using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Roles;
using CarShowroom.Application.Features.Roles.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

public class PermissionsController(IRoleService roleService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.Roles.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<IReadOnlyList<ModulePermissionsDto>>>> GetAllPermissions(
        CancellationToken cancellationToken)
    {
        var result = await roleService.GetAllPermissionsAsync(cancellationToken);
        return CustomResult(result);
    }
}
