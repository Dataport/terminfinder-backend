using JetBrains.Annotations;

namespace Dataport.Terminfinder.Repository.Tests;

[TestClass]
[ExcludeFromCodeCoverage]
public class AppointmentRepositoryTests
{
    private static readonly Guid ExpectedAppointmentId = Guid.Parse("C1C2474B-488A-4ECF-94E8-47387BB715D5");
    private static readonly Guid ExpectedAdminId = Guid.Parse("021CC2B6-9C84-4925-A927-36CC03BCC138");
    private static readonly Guid ExpectedCustomerId = Guid.Parse("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly Guid ExpectedParticipantId = Guid.Parse("09F659EF-CBBE-4B8F-BF0F-73BC3C942C0A");
    private static readonly Guid ExpectedSuggestedDateId = Guid.Parse("CA97E983-093B-4D1A-A118-AC7CC8388106");
    private static readonly Guid ExpectedVotingId = Guid.Parse("B198FD02-2C48-4932-AC34-A6878C65DC36");
    private static readonly string ExpectedCreatorName = "userXY";
    private static readonly string ExpectedDescription = "des";
    private static readonly string ExpectedPlace = "berlin";
    private static readonly string ExpectedSubject = "new appointment";
    private static readonly string ExpectedPassword = "P4$$w0rd";
    private static readonly AppointmentStatusType ExpectedStatus = AppointmentStatusType.Started;

    #region GetAppointment

    [TestMethod]
    public void GetAppointment_StatusStarted_True()
    {
        // act fetch
        var sut = CreateSut();
        var appointment = sut.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.IsNotNull(appointment);
        Assert.AreEqual(ExpectedCreatorName, appointment.CreatorName);
        Assert.AreEqual(ExpectedDescription, appointment.Description);
        Assert.AreEqual(ExpectedPlace, appointment.Place);
        Assert.AreEqual(ExpectedSubject, appointment.Subject);
        Assert.AreEqual(ExpectedStatus, appointment.AppointmentStatus);
    }

    [TestMethod]
    public void GetAppointment_StatusPaused_True()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Paused;

        // act fetch
        var sut = CreateSut(appointments);
        var appointment = sut.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.IsNotNull(appointment);
        Assert.AreEqual(AppointmentStatusType.Paused, appointment.AppointmentStatus);
    }

    [TestMethod]
    public void GetAppointment_StatusDelete_NotFound()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Deleted;

        // act fetch
        var sut = CreateSut(appointments);
        var appointment = sut.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.IsNull(appointment);
    }

    [TestMethod]
    public void GetAppointment_IdDoesNotExist_NotFound()
    {
        // act fetch
        var sut = CreateSut();
        var appointment = sut.GetAppointment(ExpectedCustomerId, Guid.Parse("719A2E53-D753-4C50-8C48-88592EF3F0E1"));

        // Assert
        Assert.IsNull(appointment);
    }

    #endregion

    #region GetAppointmentByAdminId

    [TestMethod]
    public void GetAppointmentByAdminId_StatusStarted_Okay()
    {
        // act fetch
        var sut = CreateSut();
        var appointment = sut.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId);

        // Assert
        Assert.IsNotNull(appointment);
        Assert.AreEqual(ExpectedCreatorName, appointment.CreatorName);
        Assert.AreEqual(ExpectedDescription, appointment.Description);
        Assert.AreEqual(ExpectedPlace, appointment.Place);
        Assert.AreEqual(ExpectedSubject, appointment.Subject);
        Assert.AreEqual(ExpectedStatus, appointment.AppointmentStatus);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_StatusPaused_Okay()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Paused;

        // act fetch
        var sut = CreateSut(appointments);
        var appointment = sut.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId);

        // Assert
        Assert.IsNotNull(appointment);
        Assert.AreEqual(AppointmentStatusType.Paused, appointment.AppointmentStatus);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_StatusDeleted_NotFound()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Deleted;

        // act fetch
        var sut = CreateSut(appointments);
        var appointment = sut.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId);

        // Assert
        Assert.IsNull(appointment);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_IdDoesNotExist_NotFound()
    {
        // act fetch
        var sut = CreateSut();
        var appointment =
            sut.GetAppointmentByAdminId(ExpectedCustomerId, Guid.Parse("719A2E53-D753-4C50-8C48-88592EF3F0E1"));

        // Assert
        Assert.IsNull(appointment);
    }

    #endregion

    #region ExistsAppointment

    [TestMethod]
    public void ExistsAppointment_AppointmentExists_StatusStarted_true()
    {
        // act fetch
        var sut = CreateSut();
        var appointmentExists = sut.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AppointmentExists_StatusPaused_true()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Paused;

        // act fetch
        var sut = CreateSut(appointments);
        var appointmentExists = sut.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AppointmentExists_StatusDeleted_false()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Deleted;

        // act fetch
        var sut = CreateSut(appointments);
        var appointmentExists = sut.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AppointmentDoesNotExists_false()
    {
        // act fetch
        var sut = CreateSut();
        var appointmentExists = sut.ExistsAppointment(ExpectedCustomerId, Guid.Empty, ExpectedAdminId);

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AdminIdAreEqual_AppointmentExists_StatusStarted_true()
    {
        // act fetch
        var sut = CreateSut();
        var appointmentExists = sut.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId, ExpectedAdminId);

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AdminIdAreNotEqual()
    {
        // act fetch
        var sut = CreateSut();
        var appointmentExists = sut.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId, Guid.Empty);

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AdminIdAreEqual_AppointmentExists_StatusDeleted_false()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Deleted;

        // act fetch
        var sut = CreateSut(appointments);
        var appointmentExists = sut.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId, ExpectedAdminId);

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    #endregion

    #region ExistsAppointmentIsStarted

    [TestMethod]
    public void ExistsAppointmentIsStarted_AppointmentExistsAndStatusIsStarted_true()
    {
        // act fetch
        var sut = CreateSut();
        var appointmentExistsAndIsStarted = sut.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.IsTrue(appointmentExistsAndIsStarted);
    }

    [TestMethod]
    public void ExistsAppointmentIsStarted_AppointmentExistsAndStatusIsStarted_false()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Paused;

        // act fetch
        var sut = CreateSut(appointments);
        var appointmentExistsAndIsStarted = sut.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.IsFalse(appointmentExistsAndIsStarted);
    }

    #endregion

    #region ExistsAppointmentByAdminId

    [TestMethod]
    public void ExistsAppointmentByAdminId_AppointmentExists_StatusStarted_true()
    {
        // act fetch
        var sut = CreateSut();
        var appointmentExists = sut.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId);

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointmentByAdminId_AppointmentExists_StatusPaused_true()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Paused;

        // act fetch
        var sut = CreateSut(appointments);
        var appointmentExists = sut.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId);

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointmentByAdminId_AppointmentExists_StatusDeleted_false()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Deleted;

        // act fetch
        var sut = CreateSut(appointments);
        var appointmentExists = sut.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId);

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    #endregion

    #region GetAppointmentPassword

    [TestMethod]
    public void GetAppointmentPassword_AppointmentExistsAndHasPassword_ReturnsTheAppointmentPassword()
    {
        // act fetch
        var sut = CreateSut();
        var appointmentPassword = sut.GetAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.AreEqual(ExpectedPassword, appointmentPassword);
    }

    [TestMethod]
    public void GetAppointmentPassword_AppointmentDoesNotExists_ThrowException()
    {
        // act fetch
        var sut = CreateSut();
        Assert.ThrowsException<InvalidOperationException>(() =>
            sut.GetAppointmentPassword(ExpectedCustomerId, Guid.Parse("FFF2474B-488A-4ECF-94E8-47387BB715D5")));
    }

    #endregion

    #region GetAppointmentPasswordByAdmin

    [TestMethod]
    public void GetAppointmentPasswordByAdmin_AppointmentExistsAndHasPassword_ReturnsTheAppointmentPassword()
    {
        // act fetch
        var sut = CreateSut();
        var appointmentPassword = sut.GetAppointmentPasswordByAdmin(ExpectedCustomerId, ExpectedAdminId);

        // Assert
        Assert.AreEqual(ExpectedPassword, appointmentPassword);
    }

    [TestMethod]
    public void GetAppointmentPasswordByAdmin_AppointmentDoesNotExists_ThrowException()
    {
        // act fetch
        var sut = CreateSut();
        Assert.ThrowsException<InvalidOperationException>(() =>
            sut.GetAppointmentPasswordByAdmin(ExpectedCustomerId, Guid.Parse("B84FFAC3-C03C-4A09-B584-70E2BDE4DC73")));
    }

    #endregion

    #region GetAppointmentStatusTypeByAdmin

    [TestMethod]
    public void GetAppointmentStatusTypeByAdmin_AppointmentStatusStarted_True()
    {
        // act fetch
        var sut = CreateSut();
        var statusIdentifier = sut.GetAppointmentStatusTypeByAdmin(ExpectedCustomerId, ExpectedAdminId);

        // Assert
        Assert.AreEqual(ExpectedStatus.ToString(), statusIdentifier);
    }

    [TestMethod]
    public void GetAppointmentStatusTypeByAdmin_AppointmentStatusPaused_True()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Paused;

        // act fetch
        var sut = CreateSut(appointments);
        var statusIdentifier = sut.GetAppointmentStatusTypeByAdmin(ExpectedCustomerId, ExpectedAdminId);

        // Assert
        Assert.AreEqual(nameof(AppointmentStatusType.Paused), statusIdentifier);
    }

    [TestMethod]
    public void GetAppointmentStatusTypeByAdmin_AppointmentStatusDeleted_False()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentStatus = AppointmentStatusType.Deleted;

        // act fetch
        var sut = CreateSut(appointments);
        Assert.ThrowsException<InvalidOperationException>(() =>
            sut.GetAppointmentStatusTypeByAdmin(ExpectedCustomerId, ExpectedAdminId));
    }

    #endregion

    #region GetAppointmentAdminId

    [TestMethod]
    public void GetAppointmentAdminId_Guid()
    {
        // act fetch
        var sut = CreateSut();
        var returnValueAsAdminId = sut.GetAppointmentAdminId(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.AreEqual(ExpectedAdminId, returnValueAsAdminId);
    }

    [TestMethod]
    public void GetAppointmentAdminId_CustomerIdIsEmpty_GuidEmpty()
    {
        var appointments = GetValidAppointments();
        appointments[0].CustomerId = Guid.Empty;

        // act fetch
        var sut = CreateSut(appointments);
        var returnValueAsAdminId = sut.GetAppointmentAdminId(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.AreEqual(Guid.Empty, returnValueAsAdminId);
    }

    [TestMethod]
    public void GetAppointmentAdminId_AppointmentIdIsEmpty_GuidEmpty()
    {
        var appointments = GetValidAppointments();
        appointments[0].AppointmentId = Guid.Empty;

        // act fetch
        var sut = CreateSut(appointments);
        var returnValueAsAdminId = sut.GetAppointmentAdminId(ExpectedCustomerId, Guid.Empty);

        // Assert
        Assert.AreEqual(Guid.Empty, returnValueAsAdminId);
    }

    [TestMethod]
    public void GetAppointmentAdminId_AppointmentIdNotFound_GuidEmpty()
    {
        // act fetch
        var sut = CreateSut();
        var returnValueAsAdminId =
            sut.GetAppointmentAdminId(ExpectedCustomerId, Guid.Parse("21E50876-25D5-492A-AAA2-2A6666BD5D7B"));

        // Assert
        Assert.AreEqual(Guid.Empty, returnValueAsAdminId);
    }

    #endregion

    #region GetParticipants

    [TestMethod]
    public void GetParticipants_Participants()
    {
        // act fetch
        var sut = CreateSut();
        var allParticipants = sut.GetParticipants(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.IsNotNull(allParticipants);
        Assert.AreEqual(GetValidParticipants().Count, allParticipants.Count);
        Assert.AreEqual(ExpectedParticipantId, allParticipants.First().ParticipantId);
    }

    [TestMethod]
    public void GetParticipants_AppointmentNotFound_EmptyCollection()
    {
        // act fetch
        var sut = CreateSut();
        var allParticipants =
            sut.GetParticipants(ExpectedCustomerId, Guid.Parse("21E50876-25D5-492A-AAA2-2A6666BD5D7B"));

        // Assert
        Assert.IsNotNull(allParticipants);
        Assert.AreEqual(0, allParticipants?.Count);
    }

    [TestMethod]
    public void GetParticipants_AppointmentIdIsEmpty_Null()
    {
        // act fetch
        var sut = CreateSut();
        var allParticipants = sut.GetParticipants(ExpectedCustomerId, Guid.Empty);

        // Assert
        Assert.IsNull(allParticipants);
    }

    [TestMethod]
    public void GetParticipants_CustomerIdIsNull_Null()
    {
        // act fetch
        var sut = CreateSut();
        var allParticipants =
            sut.GetParticipants(Guid.Empty, ExpectedAppointmentId);
        
        // Assert
        Assert.IsNull(allParticipants);
    }

    #endregion

    #region ExistsParticipant

    [TestMethod]
    public void ExistsParticipant_true()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsParticipant = sut.ExistsParticipant(ExpectedCustomerId, ExpectedAppointmentId, ExpectedParticipantId);
        
        // Assert
        Assert.IsTrue(isExistsParticipant);
    }

    [TestMethod]
    public void ExistsParticipant_ParticipantNotFound_false()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsParticipant =
            sut.ExistsParticipant(ExpectedCustomerId, ExpectedAppointmentId, Guid.Parse("21E50876-25D5-492A-AAA2-2A6666BD5D7B"));
        
        // Assert
        Assert.IsFalse(isExistsParticipant);
    }

    #endregion

    #region ExistsSuggestedDate

    [TestMethod]
    public void ExistsSuggestedDate_true()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsSuggestedDate =
            sut.ExistsSuggestedDate(ExpectedCustomerId, ExpectedAppointmentId, ExpectedSuggestedDateId);

        // Assert
        Assert.IsTrue(isExistsSuggestedDate);
    }

    [TestMethod]
    public void ExistsSuggestedDate_NotFound_false()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsSuggestedDate = sut.ExistsSuggestedDate(ExpectedCustomerId, ExpectedAppointmentId,
            Guid.Parse("21E50876-25D5-492A-AAA2-2A6666BD5D7B"));

        // Assert
        Assert.IsFalse(isExistsSuggestedDate);
    }

    #endregion

    #region ExistsVoting

    [TestMethod]
    public void ExistsVoting_true()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsVoting = sut.ExistsVoting(ExpectedCustomerId, ExpectedAppointmentId, ExpectedParticipantId,
            ExpectedVotingId, ExpectedSuggestedDateId);
        
        // Assert
        Assert.IsTrue(isExistsVoting);
    }

    [TestMethod]
    public void ExistsVoting_NotFound_false()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsVoting = sut.ExistsVoting(ExpectedCustomerId, Guid.Parse("FEBE03C9-27E5-4CBF-A285-FA13BD8522B1"),
            ExpectedParticipantId, ExpectedVotingId, ExpectedSuggestedDateId);
        
        // Assert
        Assert.IsFalse(isExistsVoting);
    }

    #endregion

    #region GetNumberOfSuggestedDates

    [TestMethod]
    public void GetNumberOfSuggestedDates_TwoSuggestedDates_true()
    {
        var suggestedDates = new List<SuggestedDate>
        {
            new()
            {
                AppointmentId = ExpectedAppointmentId,
                SuggestedDateId = ExpectedSuggestedDateId,
                CustomerId = ExpectedCustomerId
            },
            new()
            {
                AppointmentId = ExpectedAppointmentId,
                SuggestedDateId = Guid.Parse("26688D0A-196B-408C-AB9B-8911652A6A0F"),
                CustomerId = ExpectedCustomerId
            }
        };

        // act fetch
        var sut = CreateSut(suggestedDates: suggestedDates);
        var numberOfSuggestedDates = sut.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId);

        // Assert
        Assert.AreEqual(suggestedDates.Count, numberOfSuggestedDates);
    }

    #endregion

    #region GetNumberOfParticipants

    [TestMethod]
    public void GetNumberOfParticipants_TwoParticipants_true()
    {
        var participants = new List<Participant>
        {
            new()
            {
                AppointmentId = ExpectedAppointmentId,
                ParticipantId = ExpectedParticipantId,
                CustomerId = ExpectedCustomerId
            },
            new()
            {
                AppointmentId = ExpectedAppointmentId,
                ParticipantId = Guid.Parse("21E50876-25D5-492A-AAA2-2A6666BD5D7B"),
                CustomerId = ExpectedCustomerId
            }
        };

        // act fetch
        var sut = CreateSut(participants: participants);
        var numberOfParticipants = sut.GetNumberOfParticipants(ExpectedCustomerId, ExpectedAppointmentId);
        
        // Assert
        Assert.AreEqual(participants.Count, numberOfParticipants);
    }

    #endregion

    private static AppointmentRepository CreateSut(
        [CanBeNull] List<Appointment> appointments = null,
        [CanBeNull] List<Voting> votings = null,
        [CanBeNull] List<Participant> participants = null,
        [CanBeNull] List<SuggestedDate> suggestedDates = null)
    {
        var appointmentsQueryable = (appointments ?? GetValidAppointments()).AsQueryable();
        var votingsQueryable = (votings ?? GetValidVotings()).AsQueryable();
        var participantsQueryable = (participants ?? GetValidParticipants()).AsQueryable();
        var suggestedDatesQueryable = (suggestedDates ?? GetValidSuggestedDates()).AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockAppointmentSet = new Mock<DbSet<Appointment>>();
        mockAppointmentSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(appointmentsQueryable.Provider);
        mockAppointmentSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(appointmentsQueryable.Expression);
        mockAppointmentSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(appointmentsQueryable.ElementType);
        using var enumeratorAppointments = appointmentsQueryable.GetEnumerator();
        mockAppointmentSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(enumeratorAppointments);

        var mockVotingSet = new Mock<DbSet<Voting>>();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votingsQueryable.Provider);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votingsQueryable.Expression);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votingsQueryable.ElementType);
        using var enumeratorVotings = votingsQueryable.GetEnumerator();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(enumeratorVotings);

        var mockParticipantSet = new Mock<DbSet<Participant>>();
        mockParticipantSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participantsQueryable.Provider);
        mockParticipantSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participantsQueryable.Expression);
        mockParticipantSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participantsQueryable.ElementType);
        using var enumeratorParticipants = participantsQueryable.GetEnumerator();
        mockParticipantSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(enumeratorParticipants);
        
        var mockSuggestedDateSet = new Mock<DbSet<SuggestedDate>>();
        mockSuggestedDateSet.As<IQueryable<SuggestedDate>>().Setup(m => m.Provider).Returns(suggestedDatesQueryable.Provider);
        mockSuggestedDateSet.As<IQueryable<SuggestedDate>>().Setup(m => m.Expression).Returns(suggestedDatesQueryable.Expression);
        mockSuggestedDateSet.As<IQueryable<SuggestedDate>>().Setup(m => m.ElementType).Returns(suggestedDatesQueryable.ElementType);
        using var enumeratorSuggestedDates = suggestedDatesQueryable.GetEnumerator();
        mockSuggestedDateSet.As<IQueryable<SuggestedDate>>().Setup(m => m.GetEnumerator()).Returns(enumeratorSuggestedDates);

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockAppointmentSet.Object);
        mockContext.Setup(c => c.Votings).Returns(mockVotingSet.Object);
        mockContext.Setup(c => c.Participants).Returns(mockParticipantSet.Object);
        mockContext.Setup(c => c.SuggestedDates).Returns(mockSuggestedDateSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        var logger = new Mock<ILogger<AppointmentRepository>>();

        return new AppointmentRepository(mockContext.Object, logger.Object);
    }

    private static List<Appointment> GetValidAppointments()
    {
        return
        [
            new Appointment
            {
                AppointmentId = ExpectedAppointmentId,
                AdminId = ExpectedAdminId,
                CustomerId = ExpectedCustomerId,
                CreatorName = ExpectedCreatorName,
                Description = ExpectedDescription,
                Place = ExpectedPlace,
                Subject = ExpectedSubject,
                Password = ExpectedPassword,
                AppointmentStatus = ExpectedStatus
            }
        ];
    }

    private static List<Voting> GetValidVotings()
    {
        return
        [
            new Voting
            {
                AppointmentId = ExpectedAppointmentId,
                SuggestedDateId = ExpectedSuggestedDateId,
                CustomerId = ExpectedCustomerId,
                ParticipantId = ExpectedParticipantId,
                VotingId = ExpectedVotingId
            }
        ];
    }
    

    private static List<Participant> GetValidParticipants()
    {
        return
        [
            new Participant
            {
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                ParticipantId = ExpectedParticipantId
            }
        ];
    }
    
    private static List<SuggestedDate> GetValidSuggestedDates()
    {
        return
        [
            new SuggestedDate
            {
                AppointmentId = ExpectedAppointmentId,
                SuggestedDateId = ExpectedSuggestedDateId,
                CustomerId = ExpectedCustomerId
            }
        ];
    }
}