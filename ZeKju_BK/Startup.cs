namespace ZeKju_BK
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ZeKju.Bll;
    using ZeKju.Bll.Interface;
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FlightScheduleContext>(options =>
                options.UseMySql(configuration.GetConnectionString("FlightScheduleDatabase"), new MySqlServerVersion("8.0")));
            services.AddSingleton<IFlightScheduleChangeDetector, FlightScheduleChangeDetector>();
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<FlightScheduleContext>();
                context.Database.Migrate();
            }
        }
    }
}
