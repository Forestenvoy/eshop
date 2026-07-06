using Microsoft.OpenApi;
using System.Reflection;

namespace eshop.application.Configurations
{
    public static class ConfigureSwagger
    {
        private const string Version = "v1";
        private const string BearerScheme = "Bearer";

        public static IServiceCollection AddCustomSwaggerGen(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                // 啟用 [SwaggerResponse] 等 Annotations
                options.EnableAnnotations();

                // 後台／前台 兩份文件
                options.SwaggerDoc(SwaggerConst.Admin, new OpenApiInfo
                {
                    Version = Version,
                    Title = "eshop 後台管理 API",
                    Description = "請參考 `ResponseCode` 列舉了解回傳代碼意義",
                });
                options.SwaggerDoc(SwaggerConst.Front, new OpenApiInfo
                {
                    Version = Version,
                    Title = "eshop 前台購物 API",
                    Description = "請參考 `ResponseCode` 列舉了解回傳代碼意義",
                });

                // JWT 安全性定義
                options.AddSecurityDefinition(BearerScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(BearerScheme, document)] = []
                });

                // XML 註解
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }).AddSwaggerGenNewtonsoftSupport();

            return services;
        }
    }

    public static class SwaggerConst
    {
        public const string Admin = "admin";
        public const string Front = "front";
    }
}
