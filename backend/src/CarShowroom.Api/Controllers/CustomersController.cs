using CarShowroom.Api.Auth;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Common.Security;
using CarShowroom.Application.Features.Customers;
using CarShowroom.Application.Features.Customers.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CarShowroom.Api.Controllers;

public class CustomersController(ICustomerService customerService) : BaseApiController
{
    [HttpGet]
    [PermissionAuthorize(PermissionKeysConstant.Customers.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<PagedListModel<CustomerDto>>>> GetCustomers(
        [FromQuery] QueryParameters query, CancellationToken cancellationToken)
    {
        var result = await customerService.GetCustomersAsync(query, cancellationToken);
        return CustomResult(result);
    }

    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Customers.VIEW_KEY)]
    public async Task<ActionResult<CustomApiResponse<CustomerDto>>> GetCustomerById(
        int id, CancellationToken cancellationToken)
    {
        var result = await customerService.GetCustomerByIdAsync(id, cancellationToken);
        return CustomResult(result);
    }

    [HttpPost]
    [PermissionAuthorize(PermissionKeysConstant.Customers.CREATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CustomerDto>>> CreateCustomer(
        [FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await customerService.CreateCustomerAsync(request, cancellationToken);
        return CustomResult(result);
    }

    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Customers.UPDATE_KEY)]
    public async Task<ActionResult<CustomApiResponse<CustomerDto>>> UpdateCustomer(
        int id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await customerService.UpdateCustomerAsync(id, request, cancellationToken);
        return CustomResult(result);
    }

    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionKeysConstant.Customers.DELETE_KEY)]
    public async Task<ActionResult<CustomApiResponse<Nothing>>> DeleteCustomer(
        int id, CancellationToken cancellationToken)
    {
        var result = await customerService.DeleteCustomerAsync(id, cancellationToken);
        return CustomResult(result);
    }
}
