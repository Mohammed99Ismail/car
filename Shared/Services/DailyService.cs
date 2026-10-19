using ClinicAPI.Features.Notification.DTO;
using ClinicAPI.Features.Notifications;
using ClinicAPI.Features.Notifications.Constants;
using ClinicAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace ClinicAPI.Shared.Services;

public class DailyService
{
    private readonly ILogger<DailyService> _logger;
    private readonly ClinicContext _context;
    private readonly GlobalService _globalService;
    private readonly INotificationsService _notification;

    public DailyService(ILogger<DailyService> logger, ClinicContext context, GlobalService globalService, INotificationsService notification)
    {
        _logger = logger;
        _context = context;
        _globalService = globalService;
        _notification = notification;
    }

    public async Task RunDailyService()
    {
        try
        {
            _logger.LogInformation("Starting daily service process");

            await CheckExpiredMaterial();

            _logger.LogInformation("Daily service process completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during running daily service");
        }
    }

    private async Task CheckExpiredMaterial()
    {
        var today = _globalService.GetEgyptDate(DateTime.UtcNow);

        var materialsHasExpiredDate = await _context.Materials.Where(x => x.ExpiryDate != null && x.ExpiryDate != DateTime.MinValue && x.ExpiryDate != DateTime.MaxValue)
            .Include(x => x.MaterialItem).ToListAsync();

        foreach (var material in materialsHasExpiredDate)
        {
            var expiry = material.ExpiryDate.GetValueOrDefault();

            if (expiry > today && expiry <= today.AddMonths(1))
            {
                var notification = new NotificationDto
                {
                    // NotificationType = NotificationTypeEnum.MaterialExpiration,
                    Title = "قرب إنتهاء مخزون",
                    Body = $"نفيدك بقرب إنتهاء صلاحية {material.MaterialItem?.NameAr} حيث أنه متبقي شهر علي الإنتهاء"
                };

                await _notification.SendNotification(notification, material.BranchId);
            }
        }
    }

}
