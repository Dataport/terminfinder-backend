using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Dataport.Terminfinder.Repository.Tests")]

namespace Dataport.Terminfinder.Repository;

/// <summary>
/// Repository for CRUD operations for appointments
/// </summary>
public class AppointmentRepository : RepositoryBase, IAppointmentRepository
{
    private readonly ILogger<AppointmentRepository> _logger;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException">logger</exception>
    public AppointmentRepository(DataContext ctx,
        ILogger<AppointmentRepository> logger)
        : base(ctx)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #region appointment

    /// <inheritdoc />
    public bool ExistsAppointment(Guid customerId, Guid appointmentId)
    {
        _logger.LogDebug(
            "Enter {NameofExistsAppointment}({NameofCustomerId}={CustomerId}, {NameofAppointmentId}={AppointmentId})",
            nameof(ExistsAppointment), nameof(customerId), customerId, nameof(appointmentId), appointmentId);

        if (customerId == Guid.Empty || appointmentId == Guid.Empty)
        {
            return false;
        }

        var selectedAppointmentId = (from a in Context.Appointments
            where (a.CustomerId == customerId
                   && a.AppointmentId == appointmentId
                   && (a.StatusIdentifier == nameof(AppointmentStatusType.Started)
                       || a.StatusIdentifier == nameof(AppointmentStatusType.Paused)))
            select a.AppointmentId).SingleOrDefault();

        return (selectedAppointmentId == appointmentId);
    }

    /// <inheritdoc />
    public bool ExistsAppointment(Guid customerId, Guid appointmentId, Guid adminId)
    {
        _logger.LogDebug(
            "Enter {NameofExistsAppointment}({NameofCustomerId}={CustomerId}, {NameofAppointmentId}={AppointmentId}, {NameofAdminId}={AdminId})",
            nameof(ExistsAppointment), nameof(customerId), customerId, nameof(appointmentId), appointmentId,
            nameof(adminId), adminId);

        if (customerId == Guid.Empty || appointmentId == Guid.Empty || adminId == Guid.Empty)
        {
            return false;
        }

        var selectedAppointmentId = (from a in Context.Appointments
            where (a.CustomerId == customerId
                   && a.AppointmentId == appointmentId
                   && a.AdminId == adminId
                   && (a.StatusIdentifier == nameof(AppointmentStatusType.Started)
                       || a.StatusIdentifier == nameof(AppointmentStatusType.Paused)))
            select a.AppointmentId).SingleOrDefault();

        return (selectedAppointmentId == appointmentId);
    }

    /// <inheritdoc />
    public bool ExistsAppointmentIsStarted(Guid customerId, Guid appointmentId)
    {
        _logger.LogDebug("Enter {NameofExistsAppointmentIsStarted}", nameof(ExistsAppointmentIsStarted));

        if (customerId == Guid.Empty || appointmentId == Guid.Empty)
        {
            return false;
        }

        var selectedAppointmentId = (from a in Context.Appointments
            where (a.CustomerId == customerId
                   && a.AppointmentId == appointmentId
                   && a.StatusIdentifier == nameof(AppointmentStatusType.Started))
            select a.AppointmentId).SingleOrDefault();

        return (selectedAppointmentId == appointmentId);
    }

    /// <inheritdoc />
    public bool ExistsAppointmentByAdminId(Guid customerId, Guid adminId)
    {
        _logger.LogDebug("Enter {NameofExistsAppointmentByAdminId}", nameof(ExistsAppointmentByAdminId));

        if (customerId == Guid.Empty || adminId == Guid.Empty)
        {
            return false;
        }

        var selectedAdminId = (from a in Context.Appointments
            where (a.CustomerId == customerId
                   && a.AdminId == adminId
                   && (a.StatusIdentifier == nameof(AppointmentStatusType.Started)
                       || a.StatusIdentifier == nameof(AppointmentStatusType.Paused)))
            select a.AdminId).SingleOrDefault();

        return (selectedAdminId == adminId);
    }

    /// <inheritdoc />
    public Appointment GetAppointment(Guid customerId, Guid appointmentId)
    {
        _logger.LogDebug($"Enter {nameof(GetAppointment)}");

        // if no parameter, the result is an empty class
        if (customerId == Guid.Empty || appointmentId == Guid.Empty)
        {
            return null;
        }

        var appointment = (from a in Context.Appointments
                .Include(s => s.SuggestedDates)
                .ThenInclude(v => v.Votings)
                .Include(u => u.Participants)
                .ThenInclude(v => v.Votings)
            where (a.AppointmentId == appointmentId
                   && a.CustomerId == customerId
                   && (a.StatusIdentifier == nameof(AppointmentStatusType.Started)
                       || a.StatusIdentifier == nameof(AppointmentStatusType.Paused)))
            select a).FirstOrDefault();

        return appointment;
    }

    /// <inheritdoc />
    public Appointment GetAppointmentByAdminId(Guid customerId, Guid adminId)
    {
        _logger.LogDebug($"Enter {nameof(GetAppointmentByAdminId)}");

        // if no parameter, the result is an empty class
        if (customerId == Guid.Empty || adminId == Guid.Empty)
        {
            return null;
        }

        var appointment = Context.Appointments
            .Include(a => a.SuggestedDates)
            .ThenInclude(sg => sg.Votings)
            .FirstOrDefault(a =>
                a.AdminId == adminId
                && a.CustomerId == customerId
                && (a.StatusIdentifier == nameof(AppointmentStatusType.Started)
                    || a.StatusIdentifier == nameof(AppointmentStatusType.Paused)));

        return appointment;
    }

    /// <inheritdoc />
    public string GetAppointmentPassword(Guid customerId, Guid appointmentId)
    {
        _logger.LogDebug($"Enter {nameof(GetAppointmentPassword)}");

        if (!ExistsAppointment(customerId, appointmentId))
        {
            throw new InvalidOperationException(
                $"The appointment with customer id {customerId} and appointment id '{appointmentId}' does not exist");
        }

        var password = (from a in Context.Appointments
            where (a.AppointmentId == appointmentId
                   && a.CustomerId == customerId
                   && (a.StatusIdentifier == nameof(AppointmentStatusType.Started)
                       || a.StatusIdentifier == nameof(AppointmentStatusType.Paused)))
            select a.Password).FirstOrDefault();

        return password;
    }

    /// <inheritdoc />
    public string GetAppointmentPasswordByAdmin(Guid customerId, Guid adminId)
    {
        _logger.LogDebug($"Enter {nameof(GetAppointmentPasswordByAdmin)}");

        if (!ExistsAppointmentByAdminId(customerId, adminId))
        {
            throw new InvalidOperationException(
                $"The appointment with customer id {customerId} and admin id '{adminId}' does not exist");
        }

        var password = (from a in Context.Appointments
            where (a.AdminId == adminId
                   && a.CustomerId == customerId
                   && (a.StatusIdentifier == nameof(AppointmentStatusType.Started)
                       || a.StatusIdentifier == nameof(AppointmentStatusType.Paused)))
            select a.Password).FirstOrDefault();

        return password;
    }

    /// <inheritdoc />
    public string GetAppointmentStatusTypeByAdmin(Guid customerId, Guid adminId)
    {
        _logger.LogDebug($"Enter {nameof(GetAppointmentStatusTypeByAdmin)}");

        if (!ExistsAppointmentByAdminId(customerId, adminId))
        {
            throw new InvalidOperationException(
                $"The appointment with customer id {customerId} and admin id '{adminId}' does not exist");
        }

        var statusIdentifier = (from a in Context.Appointments
            where (a.AdminId == adminId
                   && a.CustomerId == customerId)
            select a.StatusIdentifier).First();

        return statusIdentifier;
    }

    /// <inheritdoc />
    public void SetAppointmentStatusTypeByAdmin(Guid customerId, Guid adminId, string statusIdentifier)
    {
        _logger.LogDebug($"Enter {nameof(SetAppointmentStatusTypeByAdmin)}");

        var appointment = GetAppointmentByAdminId(customerId, adminId);
        if (appointment == null)
        {
            throw new InvalidOperationException(
                $"The appointment with customer id {customerId} and admin id '{adminId}' does not exist");
        }
        
        appointment.StatusIdentifier = statusIdentifier;
        Update(Context.Appointments, appointment);
        Save();
    }

    /// <inheritdoc />
    public Guid GetAppointmentAdminId(Guid customerId, Guid appointmentId)
    {
        _logger.LogDebug($"Enter {nameof(GetAppointmentAdminId)}");

        // if no parameter, the result is an empty class
        if (customerId == Guid.Empty || appointmentId == Guid.Empty)
        {
            return Guid.Empty;
        }

        var adminId = (from a in Context.Appointments
            where (a.AppointmentId == appointmentId
                   && a.CustomerId == customerId
                   && (a.StatusIdentifier == nameof(AppointmentStatusType.Started)
                       || a.StatusIdentifier == nameof(AppointmentStatusType.Paused)))
            select a.AdminId).FirstOrDefault();

        return adminId;
    }

    /// <inheritdoc />
    public void AddAndUpdateAppointment(Appointment appointment)
    {
        _logger.LogDebug($"Enter {nameof(AddAndUpdateAppointment)}");

        if (appointment == null)
        {
            return;
        }

        using IDbContextTransaction transaction = BeginTransaction();
        if (appointment.AppointmentId == Guid.Empty)
        {
            New(Context.Appointments, appointment);
        }
        else
        {
            if (!appointment.SuggestedDates.IsNullOrEmpty())
            {
                foreach (SuggestedDate suggestedDate in appointment.SuggestedDates)
                {
                    if (suggestedDate.SuggestedDateId == Guid.Empty)
                    {
                        New(Context.SuggestedDates, suggestedDate);
                    }
                    else if (ExistsSuggestedDate(suggestedDate.CustomerId, suggestedDate.AppointmentId,
                                 suggestedDate.SuggestedDateId))
                    {
                        Update(Context.SuggestedDates, suggestedDate);
                    }
                }
            }

            if (!appointment.Participants.IsNullOrEmpty())
            {
                AddAndUpdateParticipantsWithoutExplicitTransaction(appointment.Participants);
            }

            Update(Context.Appointments, appointment);
        }

        Save();
        transaction.Commit();
    }

    #endregion appointment

    #region participant

    /// <inheritdoc />
    public ICollection<Participant> GetParticipants(Guid customerId, Guid appointmentId)
    {
        _logger.LogDebug($"Enter {nameof(GetParticipants)}");

        // if no parameter, the result is an empty class
        if (customerId == Guid.Empty || appointmentId == Guid.Empty)
        {
            _logger.LogDebug($"Leave {nameof(GetParticipants)}");
            return null;
        }

        var participants = (from a in Context.Participants
                .Include(p => p.Votings)
            where (a.AppointmentId == appointmentId)
            select a).ToList();

        return participants;
    }

    /// <inheritdoc />
    public bool ExistsParticipant(Guid customerId, Guid appointmentId, Guid participantId)
    {
        _logger.LogDebug($"Enter {nameof(ExistsParticipant)}");

        if (participantId == Guid.Empty)
        {
            return false;
        }

        var participantObject = from p in Context.Participants
            where p.ParticipantId == participantId
                  && p.CustomerId == customerId
                  && p.AppointmentId == appointmentId
            select p;

        return participantObject.FirstOrDefault() != null;
    }

    /// <inheritdoc />
    public void AddAndUpdateParticipants(ICollection<Participant> participants)
    {
        _logger.LogDebug($"Enter {nameof(AddAndUpdateParticipants)}");

        if (!participants.IsNullOrEmpty())
        {
            using IDbContextTransaction transaction = BeginTransaction();
            AddAndUpdateParticipantsWithoutExplicitTransaction(participants);
            Save();
            transaction.Commit();
        }
    }

    /// <summary>
    /// Add and updates participants with voting to context
    /// </summary>
    /// <param name="participants">ICollection of participants</param>
    private void AddAndUpdateParticipantsWithoutExplicitTransaction(ICollection<Participant> participants)
    {
        _logger.LogDebug($"Enter {nameof(AddAndUpdateParticipantsWithoutExplicitTransaction)}");

        if (participants == null)
        {
            return;
        }

        foreach (Participant participant in participants)
        {
            if (participant.ParticipantId == Guid.Empty)
            {
                New(Context.Participants, participant);
            }
            else
            {
                if (!ExistsParticipant(participant.CustomerId, participant.AppointmentId, participant.ParticipantId))
                {
                    continue;
                }

                if (participant.Votings != null)
                {
                    foreach (Voting voting in participant.Votings)
                    {
                        if (voting.VotingId == Guid.Empty)
                        {
                            New(Context.Votings, voting);
                        }
                        else if (ExistsVoting(voting.CustomerId, voting.AppointmentId, voting.ParticipantId,
                                     voting.VotingId, voting.SuggestedDateId))
                        {
                            Update(Context.Votings, voting);
                        }
                    }
                }

                Update(Context.Participants, participant);
            }
        }
    }

    /// <inheritdoc />
    public void DeleteParticipant(Participant participant)
    {
        _logger.LogDebug($"Enter {nameof(DeleteParticipant)}");

        if (participant != null && participant.ParticipantId != Guid.Empty)
        {
            using IDbContextTransaction transaction = BeginTransaction();
            Delete(Context.Participants, participant);
            Save();
            transaction.Commit();
        }
    }

    #endregion participant

    #region suggested dates

    /// <inheritdoc />
    public bool ExistsSuggestedDate(Guid customerId, Guid appointmentId, Guid suggestedDateId)
    {
        _logger.LogDebug($"Enter {nameof(ExistsSuggestedDate)}");

        if (suggestedDateId == Guid.Empty)
        {
            return false;
        }

        var checkId = from s in Context.SuggestedDates
            where s.SuggestedDateId == suggestedDateId
                  && s.CustomerId == customerId
                  && s.AppointmentId == appointmentId
            select s;

        return checkId.FirstOrDefault() != null;
    }

    /// <inheritdoc />
    public void DeleteSuggestedDate(SuggestedDate suggestedDate)
    {
        _logger.LogDebug($"Enter {nameof(DeleteSuggestedDate)}");

        if (suggestedDate != null
            && suggestedDate.SuggestedDateId != Guid.Empty)
        {
            using IDbContextTransaction transaction = BeginTransaction();
            Delete(Context.SuggestedDates, suggestedDate);
            Save();
            transaction.Commit();
        }
    }

    #endregion suggested dates

    #region votings

    /// <summary>
    /// Check, if the voting exists
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="appointmentId"></param>
    /// <param name="participantId"></param>
    /// <param name="votingId"></param>
    /// <param name="suggestedDateId"></param>
    /// <returns>true, if the voting exists otherwise false</returns>
    internal bool ExistsVoting(Guid customerId, Guid appointmentId, Guid participantId, Guid votingId,
        Guid suggestedDateId)
    {
        _logger.LogDebug($"Enter {nameof(ExistsVoting)}");

        if (participantId == Guid.Empty)
        {
            return false;
        }

        var votingObject = from v in Context.Votings
            where v.ParticipantId == participantId
                  && v.CustomerId == customerId
                  && v.AppointmentId == appointmentId
                  && v.SuggestedDateId == suggestedDateId
                  && v.VotingId == votingId
            select v;

        return votingObject.FirstOrDefault() != null;
    }

    #endregion votings

    #region get number of

    /// <inheritdoc />
    public int GetNumberOfSuggestedDates(Guid customerId, Guid appointmentId)
    {
        _logger.LogDebug($"Enter {nameof(GetNumberOfSuggestedDates)}");

        if (appointmentId == Guid.Empty || customerId == Guid.Empty)
        {
            return 0;
        }

        var count = Context.SuggestedDates.Count(s => s.AppointmentId == appointmentId
                                                      && s.CustomerId == customerId);
        return count;
    }

    /// <inheritdoc />
    public int GetNumberOfParticipants(Guid customerId, Guid appointmentId)
    {
        _logger.LogDebug($"Enter {nameof(GetNumberOfParticipants)}");

        if (appointmentId == Guid.Empty || customerId == Guid.Empty)
        {
            return 0;
        }

        var count = Context.Participants.Count(s => s.AppointmentId == appointmentId
                                                    && s.CustomerId == customerId);
        return count;
    }

    #endregion get number of
}