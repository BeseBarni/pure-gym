using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PureGym.Application.Interfaces;
using PureGym.Domain.Entities;
using PureGym.SharedKernel.Events;

namespace PureGym.Application.Features.GymAccess.Events;

public sealed class GymEnteredEventConsumer : IConsumer<GymEnteredEvent>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<GymEnteredEventConsumer> _logger;

    public GymEnteredEventConsumer(IApplicationDbContext dbContext, ILogger<GymEnteredEventConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GymEnteredEvent> context)
    {
        var e = context.Message;

        var member = await _dbContext.Members
            .Include(m => m.Memberships)
            .FirstOrDefaultAsync(m => m.Id == e.MemberId, context.CancellationToken);

        if (member is null)
        {
            _logger.LogWarning("GymEnteredEvent: Member not found: {MemberId}", e.MemberId);
            return;
        }

        var log = GymAccessLog.Record(member, e.OccurredOnUtc);

        if (log.WasDenied())
        {
            _logger.LogWarning(
                "GymEnteredEvent but access computed denied. MemberId={MemberId}, Result={Result}",
                e.MemberId,
                log.Result
            );
            return;
        }

        _dbContext.GymAccessLogs.Add(log);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("GymEnteredEvent consumed, access log saved for member {MemberId}", e.MemberId);
    }
}
