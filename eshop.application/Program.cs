using eshop.application.Common.Models;
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

DapperBootstrap.Init();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    builder.Services.AddControllers()
        .AddNewtonsoftJson(option =>
        {
            option.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        })
        .ConfigureApiBehaviorOptions(option =>
        {
            // 保留 ModelState 自動驗證
            option.SuppressModelStateInvalidFilter = false;

            // 關閉 ASP.NET Core 預設 ProblemDetails 格式
            option.SuppressMapClientErrors = true;

            // 自訂 Model Validation Error Response
            option.InvalidModelStateResponseFactory = actionContext =>
            {
                var errors = actionContext.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => x.Key)
                    .ToList();

                var message = errors.Count > 0
                    ? string.Join(", ", errors)
                    : ResponseMessage.INVALID_PARAMETERS;

                return new OkObjectResult(ApiResponse.Fail(ResponseCode.INVALID_PARAMS, message));
            };
        });

    builder.Services.AddDatabase(configuration);

    builder.Services.AddRedis(configuration);

    builder.Services.AddJwtAuthenticationAndAuthorization(configuration, builder.Environment);

    builder.Services.AddCustomSwaggerGen();

    builder.Services.AddCoreServices();

    var app = builder.Build();

    app.UseCors("CorsPolicy");

    // 啟用靜態文件存取
    app.UseStaticFiles();

    app.UseMiddleware<GlobalErrorHandler>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{SwaggerConst.Admin}/swagger.json", "後台管理 API");
            options.SwaggerEndpoint($"/swagger/{SwaggerConst.Front}/swagger.json", "前台購物 API");
        });
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
