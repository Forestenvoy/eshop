using eshop.application.Services;

namespace eshop.application.Configurations
{
    /// <summary>
    /// 一般服務註冊
    /// </summary>
    public static class ConfigureCoreService
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddSingleton<TokenService>();
            services.AddScoped<AdminService>();
            services.AddScoped<RoleService>();
            services.AddScoped<ProductService>();
            services.AddScoped<FileService>();

            return services;
        }
    }
}
