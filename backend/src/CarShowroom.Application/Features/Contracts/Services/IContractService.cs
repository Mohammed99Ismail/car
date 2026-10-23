using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Contracts.Dtos;

namespace CarShowroom.Application.Features.Contracts.Services;

public interface IContractService
{
    Task<CustomApiResponse<PagedListModel<ContractDto>>> GetContractsAsync(
        ContractQueryParameters query,
        CancellationToken cancellationToken = default);

    Task<CustomApiResponse<ContractDto>> GetContractByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<CustomApiResponse<ContractReportDto>> GetContractReportAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<CustomApiResponse<ContractDto>> CreateContractAsync(
        CreateContractRequest request,
        CancellationToken cancellationToken = default);

    Task<CustomApiResponse<ContractDto>> UpdateContractAsync(
        int id,
        UpdateContractRequest request,
        CancellationToken cancellationToken = default);

    Task<CustomApiResponse<Nothing>> DeleteContractAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<CustomApiResponse<ContractCalculationResultDto>> CalculateAsync(
        ContractCalculationRequest request,
        CancellationToken cancellationToken = default);

    Task<CustomApiResponse<ContractTermsConfigDto>> GetTermsConfigAsync(
        CancellationToken cancellationToken = default);

    Task<CustomApiResponse<ContractTermsConfigDto>> UpdateTermsConfigAsync(
        UpdateContractTermsConfigRequest request,
        CancellationToken cancellationToken = default);
}
