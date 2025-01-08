using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Server.Interfaces;
using OpenPaymentMock.Server.Persistance;
using OpenPaymentMock.Server.StateMachines;

namespace OpenPaymentMock.Server.BackgroundServices;

public class PaymentSituationTimeoutBackgroundService : BackgroundService
{
    private readonly ILogger<PaymentSituationTimeoutBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PaymentSituationTimeoutBackgroundService(
        ILogger<PaymentSituationTimeoutBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment situation timeout service is starting.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var paymentService = scope.ServiceProvider.GetRequiredService<ICallbackService>();

            var now = DateTimeOffset.UtcNow;

            var timedOutPaymentSituations = await context.PaymentSituations
                .Where(_ => _.Status == Model.Enums.PaymentSituationStatus.Created || _.Status == Model.Enums.PaymentSituationStatus.Processing)
                .Where(_ => _.TimeoutAt.CompareTo(now) < 0)
                .ToListAsync(cancellationToken);

            foreach (var paymentSituation in timedOutPaymentSituations)
            {
                paymentSituation.GetStateMachine().Fire(PaymentSituationTriggers.Cancel);
            }

            await context.SaveChangesAsync(cancellationToken);

            await Task.Delay(1000, cancellationToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Payment situation timeout service is stopping.");

        return base.StopAsync(cancellationToken);
    }
}
