using eshop.application.Common;
using eshop.application.Configurations;
using eshop.application.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

ConfigureSerilog.Prepare("eshop.application", configuration);

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddControllers()
        .AddNewtonsoftJson(option =>
        {
            option.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            option.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
        })
        .ConfigureApiBehaviorOptions(option =>
        {
            option.SuppressModelStateInvalidFilter = false;
            option.SuppressMapClientErrors = true;
            option.InvalidModelStateResponseFactory = actionContext =>
            {
                var msg = actionContext.ModelState
                    .FirstOrDefault(o => o.Value?.ValidationState == ModelValidationState.Invalid)
                    .Key ?? string.Empty;

                return new BadRequestObjectResult(new { code = ResponseCode.BAD_PARAMS, msg });
            };
        });

    builder.Services.AddDatabase(configuration);

    builder.Services.AddRedis(configuration);

    builder.Services.AddJwtAuthenticationAndAuthorization(configuration, builder.Environment);

    var app = builder.Build();

    app.UseMiddleware<GlobalErrorHandler>();

    if (app.Environment.IsDevelopment())
    {

    }

    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly: {errorMessage}", ex.Message);
}
finally
{
    Log.CloseAndFlush();
}
