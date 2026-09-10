using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.Repository;

/// <inheritdoc cref="IStatisticRepository" />
public class StatisticRepository
    : RepositoryBase, IStatisticRepository
{
    private readonly ILogger<StatisticRepository> _logger;
    private readonly DataContext _ctx;

    /// <inheritdoc cref="IStatisticRepository" />
    public StatisticRepository(DataContext ctx, ILogger<StatisticRepository> logger) : base(ctx)
    {
        _ctx = ctx;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public void IncrementAppointmentStatisticCount(Guid customerId)
    {
        var yearMonth = GetCurrentYearMonth();
        _logger.LogDebug(
            "Incrementing appointment statistic count for customer {CustomerId} and yearMonth {YearMonth}",
            customerId,
            yearMonth
        );

        using var transaction = BeginTransaction();

        var appointmentStatistic = _ctx.AppointmentStatistics
            .FirstOrDefault(s => s.CustomerId == customerId && s.YearMonth == yearMonth);

        if (appointmentStatistic == null)
        {
            _logger.LogDebug(
                "No appointment statistic found for customer {CustomerId} and yearMonth {YearMonth}, creating new statistic",
                customerId,
                yearMonth
            );

            New(Context.AppointmentStatistics, new AppointmentStatistic
            {
                CustomerId = customerId, YearMonth = yearMonth, Count = 1
            });
        }
        else
        {
            appointmentStatistic.Count++;
            Update(Context.AppointmentStatistics, appointmentStatistic);
        }

        Save();
        transaction.Commit();
    }

    /// <inheritdoc />
    public void IncrementParticipantStatisticCount(Guid customerId)
    {
        var yearMonth = GetCurrentYearMonth();
        _logger.LogDebug(
            "Incrementing participant statistic count for customer {CustomerId} and yearMonth {YearMonth}",
            customerId,
            yearMonth
        );

        using var transaction = BeginTransaction();

        var participantStatistic = _ctx.ParticipantStatistics
            .FirstOrDefault(s => s.CustomerId == customerId && s.YearMonth == yearMonth);

        if (participantStatistic == null)
        {
            _logger.LogDebug(
                "No participant statistic found for customer {CustomerId} and yearMonth {YearMonth}, creating new statistic",
                customerId,
                yearMonth
            );

            New(Context.ParticipantStatistics, new ParticipantStatistic
            {
                CustomerId = customerId, YearMonth = yearMonth, Count = 1
            });
        }
        else
        {
            participantStatistic.Count++;
            Update(Context.ParticipantStatistics, participantStatistic);
        }

        Save();
        transaction.Commit();
    }

    /// <inheritdoc />
    public void IncrementVotingStatisticCount(Guid customerId, int count = 1)
    {
        if (count <= 0)
        {
            return;
        }

        var yearMonth = GetCurrentYearMonth();
        _logger.LogDebug(
            "Incrementing voting statistic count for customer {CustomerId} and yearMonth {YearMonth}",
            customerId,
            yearMonth
        );

        using var transaction = BeginTransaction();

        var votingStatistic = _ctx.VotingStatistics
            .FirstOrDefault(s => s.CustomerId == customerId && s.YearMonth == yearMonth);

        if (votingStatistic == null)
        {
            _logger.LogDebug(
                "No voting statistic found for customer {CustomerId} and yearMonth {YearMonth}, creating new statistic",
                customerId,
                yearMonth
            );

            New(Context.VotingStatistics, new VotingStatistic
            {
                CustomerId = customerId, YearMonth = yearMonth, Count = count
            });
        }
        else
        {
            votingStatistic.Count += count;
            Update(Context.VotingStatistics, votingStatistic);
        }

        Save();
        transaction.Commit();
    }

    private static DateOnly GetCurrentYearMonth()
    {
        var now = DateTime.UtcNow;
        return new DateOnly(now.Year, now.Month, 1);
    }
}