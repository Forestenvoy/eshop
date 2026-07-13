using Coravel;
using eshop.application.Tasks;

namespace eshop.application.Configurations
{
    /// <summary>
    /// 背景排程 相關配置
    /// </summary>
    public static class ConfigureBackgroundTask
    {
        /// <summary>
        /// 註冊背景排程
        /// </summary>
        public static IServiceCollection AddBackgroundTasks(this IServiceCollection services)
        {
            services.AddScheduler();

            services.AddScoped<RefreshProductCacheTask>();

            return services;
        }

        /// <summary>
        /// 設置背景排程排程表
        /// </summary>
        public static IApplicationBuilder UseBackgroundTaskScheduler(this IApplicationBuilder app)
        {
            app.ApplicationServices.UseScheduler(scheduler =>
            {
                scheduler.Schedule<RefreshProductCacheTask>()
                    .EveryTenMinutes()
                    .PreventOverlapping(nameof(RefreshProductCacheTask));
            })
            .LogScheduledTaskProgress();

            return app;
        }
    }
}
