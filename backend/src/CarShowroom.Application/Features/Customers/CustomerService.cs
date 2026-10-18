using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Customers.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Features.Customers;

public class CustomerService(IApplicationDbContext context) : ICustomerService
{
    public async Task<CustomApiResponse<PagedListModel<CustomerDto>>> GetCustomersAsync(
        QueryParameters query, CancellationToken cancellationToken = default)
    {
        var source = context.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            source = source.Where(c => c.Name.ToLower().Contains(search)
                || c.NationalId.Contains(search)
                || c.Mobile.Contains(search)
                || c.Address.ToLower().Contains(search));
        }

        var paged = await source.ToPagedListAsync(
            c => new CustomerDto(c.Id, c.Name, c.NationalId, c.Mobile, c.Address, c.CreatedDate),
            query,
            nameof(Customer.Id),
            cancellationToken);

        return CustomApiResponse<PagedListModel<CustomerDto>>.Success(paged);
    }

    public async Task<CustomApiResponse<CustomerDto>> GetCustomerByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (customer is null)
        {
            return CustomApiResponse<CustomerDto>.NotFound(Msg.NotFound(Msg.Customer));
        }

        return CustomApiResponse<CustomerDto>.Success(new CustomerDto(
            customer.Id, customer.Name, customer.NationalId, customer.Mobile, customer.Address, customer.CreatedDate));
    }

    public async Task<CustomApiResponse<CustomerDto>> CreateCustomerAsync(
        CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var nationalId = request.NationalId.Trim();
        var exists = await context.Customers.AnyAsync(c => c.NationalId == nationalId, cancellationToken);
        if (exists)
        {
            return CustomApiResponse<CustomerDto>.Conflict(Msg.AlreadyExists(Msg.NationalId));
        }

        var customer = new Customer
        {
            Name = request.Name.Trim(),
            NationalId = nationalId,
            Mobile = request.Mobile.Trim(),
            Address = request.Address.Trim()
        };

        await context.Customers.AddAsync(customer, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var dto = new CustomerDto(customer.Id, customer.Name, customer.NationalId, customer.Mobile, customer.Address, customer.CreatedDate);
        return CustomApiResponse<CustomerDto>.Created(dto, Msg.Saved);
    }

    public async Task<CustomApiResponse<CustomerDto>> UpdateCustomerAsync(
        int id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (customer is null)
        {
            return CustomApiResponse<CustomerDto>.NotFound(Msg.NotFound(Msg.Customer));
        }

        var nationalId = request.NationalId.Trim();
        var duplicate = await context.Customers
            .AnyAsync(c => c.Id != id && c.NationalId == nationalId, cancellationToken);

        if (duplicate)
        {
            return CustomApiResponse<CustomerDto>.Conflict(Msg.AlreadyExists(Msg.NationalId));
        }

        customer.Name = request.Name.Trim();
        customer.NationalId = nationalId;
        customer.Mobile = request.Mobile.Trim();
        customer.Address = request.Address.Trim();

        await context.SaveChangesAsync(cancellationToken);

        var dto = new CustomerDto(customer.Id, customer.Name, customer.NationalId, customer.Mobile, customer.Address, customer.CreatedDate);
        return CustomApiResponse<CustomerDto>.Success(dto, Msg.Saved);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteCustomerAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (customer is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.Customer));
        }

        context.Customers.Remove(customer);
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }
}
