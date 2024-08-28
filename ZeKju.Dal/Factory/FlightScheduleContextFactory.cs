namespace ZeKju.Dal
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using ZeKju.Bll;
    public class FlightScheduleContextFactory : IDesignTimeDbContextFactory<FlightScheduleContext>
    {
        public FlightScheduleContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ZeKju_BK");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<FlightScheduleContext>();
            var connectionString = configuration.GetConnectionString("FlightScheduleDatabase");

            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion("8.0"));
            return new FlightScheduleContext(optionsBuilder.Options);
        }
    }
}
