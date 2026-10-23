using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Contracts.Dtos;
using CarShowroom.Application.Features.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

[Route("api/contracts")]
public class ContractsController(IContractService contractService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.Contracts.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<PagedListModel<ContractDto>>>> GetContracts(
        [FromQuery] ContractQueryParameters query,
        CancellationToken cancellationToken)
    {
        var result = await contractService.GetContractsAsync(query, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Contracts.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<ContractDto>>> GetContractById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await contractService.GetContractByIdAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}/report")]
    [PermissionAuthorize(PermissionKeysConstant.Contracts.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<ContractReportDto>>> GetContractReport(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await contractService.GetContractReportAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionKeysConstant.Contracts.CREATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<ContractDto>>> CreateContract(
        [FromBody] CreateContractRequest request,
        CancellationToken cancellationToken)
    {
        var result = await contractService.CreateContractAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Contracts.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<ContractDto>>> UpdateContract(
        int id,
        [FromBody] UpdateContractRequest request,
        CancellationToken cancellationToken)
    {
        var result = await contractService.UpdateContractAsync(id, request, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Contracts.DELETE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteContract(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await contractService.DeleteContractAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost("calculate")]
    [PermissionAuthorize(PermissionKeysConstant.Contracts.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<ContractCalculationResultDto>>> Calculate(
        [FromBody] ContractCalculationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await contractService.CalculateAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("terms-config")]
    [PermissionAuthorize(PermissionKeysConstant.Contracts.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<ContractTermsConfigDto>>> GetTermsConfig(
        CancellationToken cancellationToken)
    {
        var result = await contractService.GetTermsConfigAsync(cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("terms-config")]
    [PermissionAuthorize(PermissionKeysConstant.Contracts.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<ContractTermsConfigDto>>> UpdateTermsConfig(
        [FromBody] UpdateContractTermsConfigRequest request,
        CancellationToken cancellationToken)
    {
        var result = await contractService.UpdateTermsConfigAsync(request, cancellationToken);
        return CustomResult(result);
    }
}
