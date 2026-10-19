using System.Security.Claims;

namespace ClinicAPI.Shared
{
    public class GlobalService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ClaimsPrincipal CurrentUser => _httpContextAccessor.HttpContext!.User;
        public TimeZoneInfo egyptZone = TimeZoneInfo.FindSystemTimeZoneById(OperatingSystem.IsLinux() ? "Africa/Cairo" : "Egypt Standard Time");
        public DateTime utcStartOfDay;
        public DateTime utcEndOfDay;

        public GlobalService(IHttpContextAccessor httpContextAccessor)
        {
            this.CalculateTodayDate();
            _httpContextAccessor = httpContextAccessor;
        }

        public void CalculateTodayDate()
        {
            DateTime nowInEgypt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptZone);

            DateTime egyptStartOfDay = new DateTime(nowInEgypt.Year, nowInEgypt.Month, nowInEgypt.Day, 0, 0, 0);
            DateTime egyptEndOfDay = egyptStartOfDay.AddDays(1).AddTicks(-1);

            utcStartOfDay = TimeZoneInfo.ConvertTimeToUtc(egyptStartOfDay, egyptZone);
            utcEndOfDay = TimeZoneInfo.ConvertTimeToUtc(egyptEndOfDay, egyptZone);
        }

        public DateTime GetUtcStartOfDay(DateTime date)
        {
            var offset = ExtractOffset(date);
            var dto = new DateTimeOffset(date, offset);

            var startLocal = new DateTimeOffset(dto.Year, dto.Month, dto.Day, 0, 0, 0, offset);

            return startLocal.UtcDateTime;
        }

        public DateTime GetUtcEndOfDay(DateTime date)
        {
             var offset = ExtractOffset(date);
            var dto = new DateTimeOffset(date, offset);
            var endLocal = new DateTimeOffset(dto.Year, dto.Month, dto.Day, 23, 59, 59, 999, offset);

            return endLocal.UtcDateTime;
        }

         private TimeSpan ExtractOffset(DateTime date)
        {
            return date.Kind switch
            {
                DateTimeKind.Utc => TimeSpan.Zero,

                DateTimeKind.Local => TimeZoneInfo.Local.GetUtcOffset(date),

                DateTimeKind.Unspecified => TimeZoneInfo.Local.GetUtcOffset(DateTime.SpecifyKind(date, DateTimeKind.Local)),

                _ => TimeSpan.Zero
            };
        }

        public DateTime GetEgyptDate(DateTime date)
        {
            DateTime dateTimeInEgypt = TimeZoneInfo.ConvertTimeFromUtc(date, egyptZone);
            return dateTimeInEgypt;
        }
    }
}
