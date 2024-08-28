namespace ZeKju.Bll.Interface
{
    using ZeKju.Models;
    public interface IFlightScheduleChangeDetector
    {
        Task<IEnumerable<FlightResult>> DetectChanges(DateTime startDate, DateTime endDate, int agencyId);
    }
}
