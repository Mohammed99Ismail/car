using System.Reflection;
using CarShowroom.Application.Features.Auth;
using CarShowroom.Application.Features.Branches;
using CarShowroom.Application.Features.Cars;
using CarShowroom.Application.Features.Customers;
using CarShowroom.Application.Features.Lookups;
using CarShowroom.Application.Features.Roles;
using CarShowroom.Application.Features.Users;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CarShowroom.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // HybridCache (.NET 9+)
#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddHybridCache();
#pragma warning restore EXTEXP0018

        // Feature Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILookupService, LookupService>();
        services.AddScoped<IBranchService, BranchService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICarService, CarService>();

        return services;
    }
}
