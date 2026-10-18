using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Customers.Dtos;

namespace CarShowroom.Application.Features.Customers;

public interface ICustomerService
{
    Task<CustomApiResponse<PagedListModel<CustomerDto>>> GetCustomersAsync(QueryParameters query, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CustomerDto>> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CustomerDto>> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<CustomerDto>> UpdateCustomerAsync(int id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<CustomApiResponse<Nothing>> DeleteCustomerAsync(int id, CancellationToken cancellationToken = default);
}
