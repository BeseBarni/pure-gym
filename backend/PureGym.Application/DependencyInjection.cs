using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PureGym.Application.Features.Common;
using PureGym.Application.Models;
using PureGym.Application.Validators;
using PureGym.Domain.Entities;

namespace PureGym.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGeneratedSettings(configuration);
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(MembershipValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(assembly);

        services.AddListQueryHandler<Member>();
        services.AddListQueryHandler<Membership>();
        services.AddListQueryHandler<MembershipType>();
        services.AddListQueryHandler<GymAccessLog>();

        
        return services;
    }

    private static IServiceCollection AddListQueryHandler<TEntity>(this IServiceCollection services)
        where TEntity : class
    {
        services.AddTransient<
            IRequestHandler<ListQuery<TEntity>.Query, Result<ListQuery<TEntity>.Response>>,
            ListQuery<TEntity>.Handler>();

        return services;
    }
}
