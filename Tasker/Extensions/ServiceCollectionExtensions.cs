using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Tasker.Data;
using Tasker.Services;
using Tasker.Services.Interfaces;
using Tasker.Validators;

namespace Tasker.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidAudience = config["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
                    };
                });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddAppSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tasker API", Version = "v1" });

                c.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Enter: Bearer {your JWT token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "bearerAuth",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
            });

            return services;
        }

        public static IServiceCollection AddAppValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<TaskItemDtoValidator>();

            return services;
        }
    }
}
