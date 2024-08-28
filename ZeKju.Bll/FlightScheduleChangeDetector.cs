namespace ZeKju.Bll
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Concurrent;
    using ZeKju.Bll.Interface;
    using ZeKju.Models;
    public class FlightScheduleChangeDetector : IFlightScheduleChangeDetector
    {
        private readonly FlightScheduleContext _context;

        public FlightScheduleChangeDetector(FlightScheduleContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<FlightResult>> DetectChanges(DateTime startDate, DateTime endDate, int agencyId)
        {
            var results = new ConcurrentBag<FlightResult>();
            try
            {
                var subscriptions = await _context.Subscriptions
              .Where(s => s.AgencyId == agencyId)
              .ToListAsync()
              .ConfigureAwait(false);

                var routeIds = await (
                     from r in _context.Routes
                     join s in _context.Subscriptions on new { r.OriginCityId, r.DestinationCityId }
                                                         equals new { s.OriginCityId, s.DestinationCityId }
                     where s.AgencyId == agencyId
                     select r.RouteId
                 ).ToListAsync().ConfigureAwait(false);

                var flightsInRange = await _context.Flights
                    .Where(f => routeIds.Contains(f.RouteId) && f.DepartureTime.Date >= startDate && f.DepartureTime.Date <= endDate)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var tasks = flightsInRange.Select(async flight =>
                {
                    using (var context = new FlightScheduleContext(_context.Options))
                    {
                        var relatedFlights = await context.Flights
                            .Where(f => f.RouteId == flight.RouteId && f.AirlineId == flight.AirlineId)
                            .ToListAsync()
                            .ConfigureAwait(false);

                        var previousFlight = relatedFlights
                            .FirstOrDefault(f => Math.Abs((f.DepartureTime - flight.DepartureTime.AddDays(-7)).TotalMinutes) <= 30);

                        if (previousFlight == null)
                        {
                            var route = await context.Routes.FirstAsync(r => r.RouteId == flight.RouteId).ConfigureAwait(false);
                            results.Add(new FlightResult
                            {
                                FlightId = flight.FlightId,
                                OriginCityId = route.OriginCityId,
                                DestinationCityId = route.DestinationCityId,
                                DepartureTime = flight.DepartureTime,
                                ArrivalTime = flight.ArrivalTime,
                                AirlineId = flight.AirlineId,
                                Status = "New"
                            });
                        }

                        var futureFlight = relatedFlights
                            .FirstOrDefault(f => Math.Abs((f.DepartureTime - flight.DepartureTime.AddDays(7)).TotalMinutes) <= 30);

                        if (futureFlight == null)
                        {
                            var route = await context.Routes.FirstAsync(r => r.RouteId == flight.RouteId).ConfigureAwait(false);
                            results.Add(new FlightResult
                            {
                                FlightId = flight.FlightId,
                                OriginCityId = route.OriginCityId,
                                DestinationCityId = route.DestinationCityId,
                                DepartureTime = flight.DepartureTime,
                                ArrivalTime = flight.ArrivalTime,
                                AirlineId = flight.AirlineId,
                                Status = "Discontinued"
                            });
                        }
                    }
                });

                await Task.WhenAll(tasks).ConfigureAwait(false);
                return results;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while execution DetectChanges: " + ex.Message, ex);
            }
        }
    }
}
