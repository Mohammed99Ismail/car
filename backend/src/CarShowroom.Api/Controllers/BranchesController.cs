using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Branches;
using CarShowroom.Application.Features.Branches.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

public class BranchesController(IBranchService branchService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.Branches.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<PagedListModel<BranchDto>>>> GetBranches(
        [FromQuery] QueryParameters query, CancellationToken cancellationToken)
    {
        var result = await branchService.GetBranchesAsync(query, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Branches.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<BranchDto>>> GetBranchById(
        int id, CancellationToken cancellationToken)
    {
        var result = await branchService.GetBranchByIdAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionKeysConstant.Branches.CREATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<BranchDto>>> CreateBranch(
        [FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
    {
        var result = await branchService.CreateBranchAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Branches.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<BranchDto>>> UpdateBranch(
        int id, [FromBody] UpdateBranchRequest request, CancellationToken cancellationToken)
    {
        var result = await branchService.UpdateBranchAsync(id, request, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Branches.DELETE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteBranch(
        int id, CancellationToken cancellationToken)
    {
        var result = await branchService.DeleteBranchAsync(id, cancellationToken);
        return CustomResult(result);
    }
}
