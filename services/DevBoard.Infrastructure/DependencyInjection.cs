using DevBoard.Application.Interfaces;
using DevBoard.Application.Interfaces.Repositories;
using DevBoard.Domain.Entities;
using DevBoard.Infrastructure.Jobs;
using DevBoard.Infrastructure.Persistence;
using DevBoard.Infrastructure.Repositories;
using DevBoard.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevBoard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();

        services.AddScoped<OverdueTaskScanner>();
        services.AddScoped<DailyDigestJob>();

        return services;
    }
}