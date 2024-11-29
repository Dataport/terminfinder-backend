using Dataport.Terminfinder.BusinessLayer.Security;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.Common.Extension;
using Dataport.Terminfinder.Repository;

namespace Dataport.Terminfinder.BusinessLayer;

/// <inheritdoc cref="IAppointmentBusinessLayer" />
public class AppointmentBusinessLayer : BusinessLayerBase, IAppointmentBusinessLayer
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IBcryptWrapper _bcryptWrapper;
    private static readonly int MaxCountOfElementsOfParticipants = 5000;
    private static readonly int MaxCountOfElementsOfSuggestedDates = 100;
    private static readonly int MinCountOfElementsOfSuggestedDates = 1;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="appointmentRepo">Appointment repository</param>
    /// <param name="customerRepo">Customer repository</param>
    /// <param name="bcryptWrapper">BCryptWrapper</param>
    /// <param name="logger">Logger</param>
    public AppointmentBusinessLayer(IAppointmentRepository appointmentRepo,
        ICustomerRepository customerRepo,
        IBcryptWrapper bcryptWrapper,
        ILogger<AppointmentBusinessLayer> logger)
        : base(logger, customerRepo)
    {
        Logger.LogDebug($"Enter {nameof(AppointmentBusinessLayer)}");

        _appointmentRepo = appointmentRepo;
        _bcryptWrapper = bcryptWrapper ?? throw new ArgumentNullException(nameof(bcryptWrapper));
    }

    #region CheckMaxTotalCount

    /// <inheritdoc />
    public bool CheckMaxTotalCountOfParticipants(Guid customerId, Guid appointmentId,
        ICollection<Participant> participants)
    {
        Logger.LogDebug($"Leave {nameof(CheckMaxTotalCountOfParticipants)}");

        int countOfElementsToAdd = 0;

        if (!participants.IsNullOrEmpty())
        {
            countOfElementsToAdd = (from p in participants
                where (p.ParticipantId == Guid.Empty)
                select p).Count();
        }

        int count = _appointmentRepo.GetNumberOfParticipants(customerId, appointmentId) + countOfElementsToAdd;

        return count <= MaxCountOfElementsOfParticipants;
    }

    /// <inheritdoc />
    public bool CheckMinTotalCountOfSuggestedDates(Guid customerId, Guid appointmentId,
        ICollection<SuggestedDate> suggestedDates)
    {
        Logger.LogDebug($"Leave {nameof(CheckMinTotalCountOfSuggestedDates)}");

        int countOfElementsToAdd = 0;

        if (!suggestedDates.IsNullOrEmpty())
        {
            countOfElementsToAdd = (from s in suggestedDates
                where (s.SuggestedDateId == Guid.Empty)
                select s).Count();
        }

        int count = _appointmentRepo.GetNumberOfSuggestedDates(customerId, appointmentId) + countOfElementsToAdd;

        return count >= MinCountOfElementsOfSuggestedDates;
    }

    /// <inheritdoc />
    public bool CheckMinTotalCountOfSuggestedDatesWithToDeletedDates(Guid customerId, Guid appointmentId,
        ICollection<SuggestedDate> suggestedDates)
    {
        Logger.LogDebug($"Leave {nameof(CheckMinTotalCountOfSuggestedDatesWithToDeletedDates)}");

        int countOfElementsToDelete = 0;

        if (!suggestedDates.IsNullOrEmpty())
        {
            countOfElementsToDelete = (from s in suggestedDates
                where (s.SuggestedDateId != Guid.Empty
                       && s.AppointmentId != Guid.Empty
                       && s.CustomerId != Guid.Empty)
                select s).Count();
        }

        int count = _appointmentRepo.GetNumberOfSuggestedDates(customerId, appointmentId) - countOfElementsToDelete;

        return (count >= MinCountOfElementsOfSuggestedDates);
    }

    /// <inheritdoc />
    public bool CheckMaxTotalCountOfSuggestedDates(Guid customerId, Guid appointmentId,
        ICollection<SuggestedDate> suggestedDates)
    {
        Logger.LogDebug($"Leave {nameof(CheckMaxTotalCountOfSuggestedDates)}");

        int countOfElementsToAdd = 0;

        if (!suggestedDates.IsNullOrEmpty())
        {
            countOfElementsToAdd = (from s in suggestedDates
                where (s.SuggestedDateId == Guid.Empty)
                select s).Count();
        }

        int count = _appointmentRepo.GetNumberOfSuggestedDates(customerId, appointmentId) + countOfElementsToAdd;

        return (count <= MaxCountOfElementsOfSuggestedDates);
    }

    #endregion CheckMaxTotalCount

    #region Appointment

    /// <inheritdoc />
    public Appointment GetAppointment(Guid customerId, Guid appointmentId)
    {
        Logger.LogDebug($"Enter {nameof(GetAppointment)}");

        if (!ExistsCustomer(customerId))
        {
            return null;
        }

        Appointment result = _appointmentRepo.GetAppointment(customerId, appointmentId);
        return RemovePasswordFromAppointment(result);
    }

    /// <inheritdoc />
    public bool IsAppointmentPasswordProtected(Guid customerId, Guid appointmentId)
    {
        Logger.LogDebug("Enter {NameofIsAppointmentPasswordProtected}(Guid), (Guid))",
            nameof(IsAppointmentPasswordProtected));

        string password = _appointmentRepo.GetAppointmentPassword(customerId, appointmentId);
        return IsPasswordSet(password);
    }

    /// <inheritdoc />
    public Appointment GetAppointmentByAdminId(Guid customerId, Guid adminId)
    {
        Logger.LogDebug($"Enter {nameof(GetAppointmentByAdminId)}");

        if (!ExistsCustomer(customerId))
        {
            return null;
        }

        Appointment result = _appointmentRepo.GetAppointmentByAdminId(customerId, adminId);
        return RemovePasswordFromAppointment(result);
    }

    /// <inheritdoc />
    public bool IsAppointmentPasswordProtectedByAdminId(Guid customerId, Guid adminId)
    {
        Logger.LogDebug(
            "Enter {NameofIsAppointmentPasswordProtectedByAdminId}(Guid), (Guid)",
            nameof(IsAppointmentPasswordProtectedByAdminId));

        string password = _appointmentRepo.GetAppointmentPasswordByAdmin(customerId, adminId);
        return IsPasswordSet(password);
    }

    [ContractAnnotation("password:null => false; password:notnull => true")]
    private static bool IsPasswordSet(string password)
    {
        return password != null;
    }

    /// <inheritdoc />
    public Appointment AddAppointment(Appointment appointment)
    {
        Logger.LogDebug($"Enter {nameof(AddAppointment)}");

        if (appointment != null && ExistsCustomer(appointment.CustomerId))
        {
            HashPasswordInAppointment(appointment);
            _appointmentRepo.AddAndUpdateAppointment(appointment);
            appointment = _appointmentRepo.GetAppointment(appointment.CustomerId, appointment.AppointmentId);
        }

        return RemovePasswordFromAppointment(appointment);
    }

    /// <inheritdoc />
    public Appointment UpdateAppointment(Appointment appointment, Guid adminId)
    {
        Logger.LogDebug("Enter {NameofUpdateAppointment} (adminId={AdminId})", nameof(UpdateAppointment), adminId);

        if (appointment == null || adminId == Guid.Empty)
        {
            return null;
        }

        return UpdateAppointmentAndRemovePasswordFromAppointment(appointment);
    }

    /// <inheritdoc />
    public Appointment UpdateAppointment(Appointment appointment)
    {
        Logger.LogDebug($"Enter {nameof(UpdateAppointment)}");

        if (appointment == null)
        {
            return null;
        }

        // read adminid from the database to set the value
        appointment.AdminId =
            _appointmentRepo.GetAppointmentAdminId(appointment.CustomerId, appointment.AppointmentId);
        return UpdateAppointmentAndRemovePasswordFromAppointment(appointment);
    }

    private Appointment UpdateAppointmentAndRemovePasswordFromAppointment([NotNull] Appointment appointment)
    {
        Logger.LogDebug($"Enter {nameof(UpdateAppointmentAndRemovePasswordFromAppointment)}");

        if (ExistsCustomer(appointment.CustomerId))
        {
            HashPasswordInAppointment(appointment);
            _appointmentRepo.AddAndUpdateAppointment(appointment);
            appointment = _appointmentRepo.GetAppointmentByAdminId(appointment.CustomerId, appointment.AdminId);
        }

        return RemovePasswordFromAppointment(appointment);
    }

    [CanBeNull]
    [ContractAnnotation("appointment:null => null; appointment:notnull => notnull")]
    private static Appointment RemovePasswordFromAppointment(Appointment appointment)
    {
        if (appointment != null)
        {
            appointment.Password = null;
        }

        return appointment;
    }

    private void HashPasswordInAppointment(Appointment appointment)
    {
        if (appointment?.Password != null)
        {
            appointment.Password = _bcryptWrapper.HashPassword(appointment.Password);
        }
    }

    /// <inheritdoc />
    public int SaveAppointment()
    {
        Logger.LogDebug($"Enter {nameof(SaveAppointment)}");
        return _appointmentRepo.Save();
    }

    #endregion Appointment

    #region Participant

    /// <inheritdoc />
    public bool ExistsParticipant(Guid customerId, Guid appointmentId, Guid participantId)
    {
        Logger.LogDebug($"Enter {nameof(ExistsParticipant)}");
        return _appointmentRepo.ExistsParticipant(customerId, appointmentId, participantId);
    }

    /// <inheritdoc />
    public ICollection<Participant> GetParticipants(Guid customerId, Guid appointmentId)
    {
        Logger.LogDebug($"Enter {nameof(GetParticipants)}");
        return _appointmentRepo.GetParticipants(customerId, appointmentId);
    }

    /// <inheritdoc />
    public ICollection<Participant> AddAndUpdateParticipiants(Guid customerId, Guid appointmentId,
        ICollection<Participant> participants)
    {
        Logger.LogDebug($"Enter {nameof(AddAndUpdateParticipiants)}");
        if (participants.IsNullOrEmpty())
        {
            return (participants);
        }

        _appointmentRepo.AddAndUpdateParticipants(participants);
        participants = _appointmentRepo.GetParticipants(customerId, appointmentId);
        return (participants);
    }

    /// <inheritdoc />
    public void DeleteParticipiant(Participant participant)
    {
        Logger.LogDebug($"Enter {nameof(DeleteParticipiant)}");
        _appointmentRepo.DeleteParticipant(participant);
    }

    /// <inheritdoc />
    public bool ParticipantsAreValid(ICollection<Participant> participants)
    {
        return !participants.IsNullOrEmpty()
             && participants.All(participant => participant.IsValid);
    }

    /// <inheritdoc />
    public bool ParticipantToDeleteAreValid(Participant participant)
    {
        return participant != null
               && participant.AppointmentId != Guid.Empty
               && participant.CustomerId != Guid.Empty
               && participant.ParticipantId != Guid.Empty;
    }

    #endregion Participant

    #region Suggested Dates

    /// <inheritdoc />
    public bool ExistsSuggestedDate(Guid customerId, Guid appointmentId, Guid suggestedDateId)
    {
        Logger.LogDebug($"Enter {nameof(ExistsSuggestedDate)}");
        return _appointmentRepo.ExistsSuggestedDate(customerId, appointmentId, suggestedDateId);
    }


    /// <inheritdoc />
    public void DeleteSuggestedDate(SuggestedDate suggestedDate)
    {
        Logger.LogDebug($"Enter {nameof(DeleteSuggestedDate)}");
        _appointmentRepo.DeleteSuggestedDate(suggestedDate);
    }

    /// <inheritdoc />
    public bool SuggestedDateToDeleteAreValid(SuggestedDate suggestedDate)
    {
        return suggestedDate != null
               && suggestedDate.AppointmentId != Guid.Empty
               && suggestedDate.CustomerId != Guid.Empty
               && suggestedDate.SuggestedDateId != Guid.Empty;
    }

    #endregion Suggested Dates

    #region Set foreign keys

    /// <inheritdoc />
    public void SetAppointmentForeignKeys(Appointment appointment, Guid customerId)
    {
        Logger.LogDebug($"Enter {nameof(SetAppointmentForeignKeys)}");

        appointment.CustomerId = customerId;

        if (!appointment.SuggestedDates.IsNullOrEmpty())
        {
            SetSuggestedDatesForeignKeys(appointment.SuggestedDates, customerId, appointment.AppointmentId);
        }

        if (!appointment.Participants.IsNullOrEmpty())
        {
            SetParticipantsForeignKeys(appointment.Participants, customerId, appointment.AppointmentId);
        }
    }

    /// <inheritdoc />
    public void SetParticipantsForeignKeys(ICollection<Participant> participants, Guid customerId,
        Guid appointmentId)
    {
        Logger.LogDebug($"Enter {nameof(SetParticipantsForeignKeys)}");

        if (participants.IsNullOrEmpty())
        {
            return;
        }

        foreach (Participant participant in participants)
        {
            participant.AppointmentId = appointmentId;
            participant.CustomerId = customerId;

            if (participant.Votings.IsNullOrEmpty())
            {
                continue;
            }

            foreach (Voting voting in participant.Votings)
            {
                voting.AppointmentId = appointmentId;
                voting.CustomerId = customerId;

                if (voting.ParticipantId == Guid.Empty)
                {
                    voting.ParticipantId = participant.ParticipantId;
                }
            }
        }
    }

    /// <inheritdoc />
    public void SetSuggestedDatesForeignKeys(ICollection<SuggestedDate> suggestedDates, Guid customerId,
        Guid appointmentId)
    {
        Logger.LogDebug($"Enter {nameof(SetSuggestedDatesForeignKeys)}");

        if (!suggestedDates.IsNullOrEmpty())
        {
            foreach (SuggestedDate suggestedDatesElem in suggestedDates)
            {
                suggestedDatesElem.AppointmentId = appointmentId;
                suggestedDatesElem.CustomerId = customerId;

                if (!suggestedDatesElem.Votings.IsNullOrEmpty())
                {
                    foreach (Voting voting in suggestedDatesElem.Votings)
                    {
                        voting.AppointmentId = appointmentId;
                        voting.CustomerId = customerId;

                        if (voting.SuggestedDateId == Guid.Empty)
                        {
                            voting.SuggestedDateId = suggestedDatesElem.SuggestedDateId;
                        }
                    }
                }
            }
        }
    }

    #endregion Set foreign keys


    /// <inheritdoc />
    public bool ExistsAppointment(Guid customerId, Guid appointmentId)
    {
        Logger.LogDebug(
            "Enter {NameofExistsAppointment}(customerId={CustomerId}, appointmentId={AppointmentId})",
            nameof(ExistsAppointment), customerId, appointmentId);

        if (appointmentId != Guid.Empty && customerId != Guid.Empty)
        {
            return _appointmentRepo.ExistsAppointment(customerId, appointmentId);
        }
        return false;
    }

    /// <inheritdoc />
    public bool ExistsAppointment(Guid customerId, Guid appointmentId, Guid adminId)
    {
        Logger.LogDebug(
            "Enter {NameofExistsAppointment}(customerId={CustomerId}, appointmentId={AppointmentId}, adminId={AdminId})",
            nameof(ExistsAppointment), customerId, appointmentId, adminId);

        if (appointmentId != Guid.Empty && adminId != Guid.Empty && customerId != Guid.Empty)
        {
            return _appointmentRepo.ExistsAppointment(customerId, appointmentId, adminId);
        }
        return false;
    }

    /// <inheritdoc />
    public bool ExistsAppointmentIsStarted(Guid customerId, Guid appointmentId)
    {
        Logger.LogDebug($"Enter {nameof(ExistsAppointmentIsStarted)}");

        if (appointmentId != Guid.Empty && customerId != Guid.Empty)
        {
            return _appointmentRepo.ExistsAppointmentIsStarted(customerId, appointmentId);
        }

        return false;
    }

    /// <inheritdoc />
    public bool ExistsAppointmentByAdminId(Guid customerId, Guid adminId)
    {
        Logger.LogDebug($"Enter {nameof(ExistsAppointmentByAdminId)}");

        if (adminId != Guid.Empty && customerId != Guid.Empty)
        {
            return _appointmentRepo.ExistsAppointmentByAdminId(customerId, adminId);
        }

        return false;
    }

    /// <inheritdoc />
    public new bool ExistsCustomer(Guid customerId)
    {
        Logger.LogDebug($"Enter {nameof(ExistsCustomer)}");

        return base.ExistsCustomer(customerId);
    }

    /// <inheritdoc />
    public bool ExistsCustomer(string customerId)
    {
        Logger.LogDebug($"Enter {nameof(ExistsCustomer)}");

        if (!Guid.TryParse(customerId, out Guid customerIdGuid))
        {
            return false;
        }

        return base.ExistsCustomer(customerIdGuid);
    }

    /// <inheritdoc />
    public bool VerifyAppointmentPassword(Guid customerId, Guid appointmentId, string password)
    {
        Logger.LogDebug(
            "Enter {NameofVerifyAppointmentPassword}(Guid), (Guid), (string))", nameof(VerifyAppointmentPassword));

        if (!IsAppointmentPasswordProtected(customerId, appointmentId))
        {
            throw new InvalidOperationException(
                $"The appointment with customer id '{customerId}' and appointment id '{appointmentId}' is not protected by a password");
        }

        string appointmentPassword = _appointmentRepo.GetAppointmentPassword(customerId, appointmentId);
        if (appointmentPassword == null)
        {
            throw new InvalidOperationException(
                $"The appointment with customer id '{customerId}' and appointment id '{appointmentId}' has no password");
        }

        return _bcryptWrapper.Verify(password, appointmentPassword);
    }

    /// <inheritdoc />
    public bool VerifyAppointmentPasswordByAdminId(Guid customerId, Guid adminId, string password)
    {
        Logger.LogDebug(
            "Enter {NameofVerifyAppointmentPasswordByAdminId}(Guid), (Guid), (string)",
            nameof(VerifyAppointmentPasswordByAdminId));

        if (!IsAppointmentPasswordProtectedByAdminId(customerId, adminId))
        {
            throw new InvalidOperationException(
                $"The appointment with customer id '{customerId}' and admin id '{adminId}' is not protected by a password");
        }

        string appointmentPassword = _appointmentRepo.GetAppointmentPasswordByAdmin(customerId, adminId);
        if (appointmentPassword == null)
        {
            throw new InvalidOperationException(
                $"The appointment with customer id '{customerId}' and admin id '{adminId}' has no password");
        }

        return _bcryptWrapper.Verify(password, appointmentPassword);
    }

    /// <inheritdoc />
    public Appointment SetAppointmentStatusType(Guid customerId, Guid adminId, AppointmentStatusType newStatusType)
    {
        Logger.LogDebug(
            "Enter {NameofSetAppointmentStatusType}(Guid), (Guid), (AppointmentStatusType)",
            nameof(SetAppointmentStatusType));

        if (!ExistsAppointmentByAdminId(customerId, adminId))
        {
            throw new InvalidOperationException(
                $"The appointment with customer id {customerId} and admin id '{adminId}' does not exist");
        }

        AppointmentStatusType oldStatusIdentifier = _appointmentRepo
            .GetAppointmentStatusTypeByAdmin(customerId, adminId).ToEnum<AppointmentStatusType>();
        if ((oldStatusIdentifier == AppointmentStatusType.Started &&
             newStatusType == AppointmentStatusType.Paused)
            || (oldStatusIdentifier == AppointmentStatusType.Paused &&
                newStatusType == AppointmentStatusType.Started))
        {
            _appointmentRepo.SetAppointmentStatusTypeByAdmin(customerId, adminId, newStatusType.ToString());
            Appointment result = _appointmentRepo.GetAppointmentByAdminId(customerId, adminId);
            return RemovePasswordFromAppointment(result);
        }

        return null;
    }
}