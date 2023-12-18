using Example.Api.Application.Behaviors;
using Example.Api.Endpoints;
using Example.Api.Infrastructure.Extensions;
using Example.Domain.Repositories;
using Example.Infrastructure;
using Example.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var assemblies = AppDomain.CurrentDomain.GetAssemblies();

builder.Services
    .AddDbContext<UserDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgresql"));
        options.EnableDetailedErrors(builder.Environment.IsDevelopment());
        options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    })
    .AddScoped<IUserRepository, UserRepository>();
    
builder.Services
    .AddMediatR(configuration =>
    {
        configuration.RegisterServicesFromAssemblies(assemblies);
        configuration.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));
        configuration.AddOpenBehavior(typeof(RequestValidatorBehavior<,>));
    })
    .AddValidatorsFromAssemblies(assemblies, ServiceLifetime.Scoped, null, true);

builder.Host
    .UseSerilog((context, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandler();
app.UserUserEndpoints();

await app.MigrateDbContext<UserDbContext>();

await app.RunAsync();
