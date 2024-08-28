namespace ZeKju.Bll
{
    using Microsoft.EntityFrameworkCore;
    using ZeKju.Dal.Enitites;
    public class FlightScheduleContext : DbContext
    {
        public FlightScheduleContext(DbContextOptions<FlightScheduleContext> options)
            : base(options)
        {
            Options = options;
        }
        public DbContextOptions<FlightScheduleContext> Options { get; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Route>().HasKey(r => r.RouteId);
            modelBuilder.Entity<Flight>().HasKey(f => f.FlightId);
            modelBuilder.Entity<Subscription>().HasKey(s => new { s.AgencyId, s.OriginCityId, s.DestinationCityId });

            modelBuilder.Entity<Route>()
                .HasMany(r => r.Flights)
                .WithOne(f => f.Route)
                .HasForeignKey(f => f.RouteId);
        }
    }
}
