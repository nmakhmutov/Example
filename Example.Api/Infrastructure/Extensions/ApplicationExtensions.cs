using Example.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Example.Api.Infrastructure.Extensions;

internal static class ApplicationExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) =>
        app.UseExceptionHandler(options =>
            options.Run(context =>
            {
                if (context.Features.Get<IExceptionHandlerPathFeature>() is not { } ex)
                    return Results.Problem()
                        .ExecuteAsync(context);

                var factory = context.RequestServices.GetRequiredService<ILoggerFactory>();
                var logger = factory.CreateLogger("GlobalExceptionHandler");

                logger.LogError(ex.Error, "Error in: {Endpoint}", ex.Endpoint);

                var problem = ex.Error switch
                {
                    ValidationException error => new ValidationProblemDetails(
                        error.Errors
                            .GroupBy(failure => failure.PropertyName)
                            .ToDictionary(
                                x => x.Key,
                                failures => failures.Select(failure => failure.ErrorMessage)
                                    .ToArray()
                            )
                    )
                    {
                        Status = 400,
                        Detail = error.Message
                    },

                    UserException error => new ProblemDetails
                    {
                        Status = 400,
                        Title = "User error",
                        Detail = error.Message
                    },

                    _ => new ProblemDetails
                    {
                        Status = 500,
                        Title = "Unhandled error",
                        Detail = ex.Error.Message
                    }
                };

                return Results.Problem(problem)
                    .ExecuteAsync(context);
            })
        );

    public static Task<IApplicationBuilder> MigrateDbContext<TContext>(this IApplicationBuilder app)
        where TContext : DbContext =>
        app.MigrateDbContext<TContext>((_, _) => Task.CompletedTask);

    public static async Task<IApplicationBuilder> MigrateDbContext<TContext>(
        this IApplicationBuilder app, Func<TContext, IServiceProvider, Task> seeder)
        where TContext : DbContext
    {
        var name = typeof(TContext).Name;

        await using var scope = app.ApplicationServices.CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        logger.LogInformation("Migrating database associated with context {DbContext}...", name);

        try
        {
            await context.Database.MigrateAsync();

            await seeder(context, scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database used on context {DbContext}", name);
            throw;
        }

        logger.LogInformation("Migrating database associated with context {DbContext} completed", name);

        return app;
    }
}
