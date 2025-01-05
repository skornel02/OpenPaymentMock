
using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Model.Enums;
using OpenPaymentMock.Server.Interfaces;
using OpenPaymentMock.Server.Persistance;

namespace OpenPaymentMock.Server.BackgroundServices;

public class CallbackBackgroundService : BackgroundService
{
    private readonly ILogger<CallbackBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CallbackBackgroundService(
        ILogger<CallbackBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CallbackBackgroundService is starting.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("CallbackBackgroundService is working.");

            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var paymentService = scope.ServiceProvider.GetRequiredService<ICallbackService>();

            var now = DateTimeOffset.Now;
            var timeout = now.AddMinutes(-1);

            var activeCallbacks = await context.Callbacks
                .Include(_ => _.PaymentSituation)
                .Where(_ => _.Status == PaymentCallbackStatus.Pending)
                .Where(_ => _.LatestResponseAt == null || _.LatestResponseAt.Value.CompareTo(timeout) < 0)
                .ToListAsync(cancellationToken);

            foreach (var callback in activeCallbacks)
            {
                try
                {
                    var (success, error) = await paymentService.SendCallbackToServiceAsync(callback.PaymentSituation, cancellationToken);

                    if (success)
                    {
                        callback.Status = PaymentCallbackStatus.Success;
                        callback.LatestResponse = null;
                        callback.LatestResponseAt = DateTimeOffset.Now;
                    }
                    else 
                    {
                        if (++callback.RetryCount >= 5)
                        {
                            callback.Status = PaymentCallbackStatus.Failure;
                        }

                        callback.LatestResponse = error;
                        callback.LatestResponseAt = DateTimeOffset.Now;
                    }
                } 
                catch (Exception ex)
                {
                    callback.LatestResponse = ex.Message;
                    callback.LatestResponseAt = DateTimeOffset.Now;
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            await Task.Delay(10000, cancellationToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("CallbackBackgroundService is stopping.");

        return base.StopAsync(cancellationToken);
    }
}
