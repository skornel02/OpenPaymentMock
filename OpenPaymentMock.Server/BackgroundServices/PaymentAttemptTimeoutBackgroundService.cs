using Microsoft.EntityFrameworkCore;
using OpenPaymentMock.Server.Interfaces;
using OpenPaymentMock.Server.Persistance;
using OpenPaymentMock.Server.StateMachines;

namespace OpenPaymentMock.Server.BackgroundServices;

public class PaymentAttemptTimeoutBackgroundService : BackgroundService
{
    private readonly ILogger<PaymentAttemptTimeoutBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PaymentAttemptTimeoutBackgroundService(
        ILogger<PaymentAttemptTimeoutBackgroundService> logger,
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

            var timedOutPaymentAttempts = await context.PaymentAttempts
                .Where(_ => _.Status == Model.Enums.PaymentAttemptStatus.Started || _.Status == Model.Enums.PaymentAttemptStatus.BankVerificationRequired)
                .Where(_ => _.TimeoutAt.CompareTo(now) < 0)
                .ToListAsync(cancellationToken);

            foreach (var paymentAttempt in timedOutPaymentAttempts)
            {
                paymentAttempt.GetStateMachine(out var _).Fire(PaymentAttemptTrigger.Timeout);
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
