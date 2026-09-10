namespace Dataport.Terminfinder.Repository;

/// <summary>
/// Business methods for statistics
/// </summary>
public interface IStatisticRepository : IRepositoryBase
{
    /// <summary>
    /// Increment the count of the appointment statistic for the given customer for the current month.
    /// If no statistic exists, a new one will be created with count = 1.
    /// Appointments are always created as single requests, so no count parameter is needed.
    /// </summary>
    /// <param name="customerId"></param>
    void IncrementAppointmentStatisticCount(Guid customerId);

    /// <summary>
    /// Increment the count of the participant statistic for the given customer for the current month.
    /// If no statistic exists, a new one will be created with count = 1.
    /// Participants are always created as single requests, so no count parameter is needed.
    /// </summary>
    /// <param name="customerId"></param>
    void IncrementParticipantStatisticCount(Guid customerId);

    /// <summary>
    /// Increment the count of the voting statistic for the given customer for the current month.
    /// If no statistic exists, a new one will be created with count = 1.
    /// Votings can be created in bulk, so the count parameter allows to increment the statistic by more than 1.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="count"></param>
    void IncrementVotingStatisticCount(Guid customerId, int count = 1);
}