using CarShowroom.Application.Features.Contracts.Dtos;

namespace CarShowroom.Application.Features.Contracts.Services;

public interface IContractCalculationService
{
    (decimal BookValue, decimal AccumulatedDepreciation, decimal ProductiveAge) CalculateCarBookValue(
        decimal purchasePrice,
        DateTime purchaseDate,
        DateTime? evaluationDate = null);

    ContractCalculationResultDto Calculate(
        ContractCalculationRequest request,
        decimal defaultInsurance = 4.00m,
        decimal defaultMaintenance = 5.00m,
        decimal defaultReplacement = 1.00m,
        decimal defaultAdminFees = 1.00m,
        decimal defaultDepreciation = 25.00m,
        decimal defaultVat = 15.00m);
}
