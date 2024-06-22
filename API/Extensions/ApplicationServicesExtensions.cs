﻿using API.Data;
using API.Interface;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration){
        services.AddControllers();
        services.AddDbContext<DataContext>(options =>
        options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}
