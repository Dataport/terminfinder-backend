namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class ParticipantControllerTests
{
    private static readonly Guid ExpectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
    private static readonly Guid ExpectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly Guid ExpectedParticipantId = new("9054E979-C40C-4FA3-B36A-D85803143F5D");
    private const string ExpectedInvalidGuidString = "invalid";

    [TestMethod]
    public void DeleteParticipants_Okay()
    {
        var participant = CreateDefaultParticipant();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(participant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>())).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsParticipant(ExpectedCustomerId, ExpectedAppointmentId, ExpectedParticipantId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.Delete(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString(),
            ExpectedParticipantId.ToString());
        var result = httpResult as OkResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void DeleteParticipants_Participants_NoInput()
    {
        var expectedParticipantId = Guid.Empty;
        var participant = CreateDefaultParticipant();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(participant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>())).Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Delete(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString(),
                expectedParticipantId.ToString()));
        Assert.AreEqual(ErrorType.NoInput, exception.ErrorCode);
    }

    [TestMethod]
    public void DeleteParticipants_Participants_NotFound()
    {
        var participant = CreateDefaultParticipant();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(participant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>())).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsParticipant(ExpectedCustomerId, ExpectedAppointmentId, ExpectedParticipantId))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<NotFoundException>(() => 
            sut.Delete(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString(), 
                ExpectedParticipantId.ToString()));
        Assert.AreEqual(ErrorType.ParticipantNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void DeleteParticipants_AppointmentIsPaused_NotFound()
    {
        var participant = CreateDefaultParticipant();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(participant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>())).Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(false);
        mockBusinessLayer
            .Setup(m => m.ExistsParticipant(ExpectedCustomerId, ExpectedAppointmentId, ExpectedParticipantId))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<NotFoundException>(() => 
            sut.Delete(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString(),
                ExpectedParticipantId.ToString()));
        Assert.AreEqual(ErrorType.AppointmentNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void DeleteParticipants_AppointmentIsPasswordProtectedNoPasswordSubmitted_Unauthorized()
    {
        var participant = CreateDefaultParticipant();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(participant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>())).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedException>(() =>
            sut.Delete(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString(), 
                ExpectedParticipantId.ToString()));
        Assert.AreEqual(ErrorType.PasswordRequired, exception.ErrorCode);
    }

    [TestMethod]
    public void DeleteParticipants_GuidsAreInvalid_ThrowsException()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        var sut = CreateSut(mockBusinessLayer.Object);

        var exceptionCustomerId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Delete(ExpectedInvalidGuidString, ExpectedAppointmentId.ToString(), ExpectedParticipantId.ToString()));
        Assert.AreEqual(ErrorType.WrongInputOrNotAllowed, exceptionCustomerId.ErrorCode);
        var exceptionAppointmentId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Delete(ExpectedCustomerId.ToString(), ExpectedInvalidGuidString, ExpectedParticipantId.ToString()));
        Assert.AreEqual(ErrorType.WrongInputOrNotAllowed, exceptionAppointmentId.ErrorCode);
        var exceptionParticipantId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Delete(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString(), ExpectedInvalidGuidString));
        Assert.AreEqual(ErrorType.WrongInputOrNotAllowed, exceptionParticipantId.ErrorCode);
    }

    [TestMethod]
    public void DeleteParticipants_ParticipantsAreInvalid_ThrowsException()
    {
        var participant = CreateDefaultParticipant();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(participant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>())).Returns(false);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        var exception = Assert.ThrowsException<BadRequestException>(() => 
            sut.Delete(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString(), 
                ExpectedParticipantId.ToString()));
        Assert.AreEqual(ErrorType.ParticipantNotValid, exception.ErrorCode);
    }

    private static ParticipantController CreateSut(IAppointmentBusinessLayer appointmentBusinessLayer = null)
    {
        var appointmentBusinessLayerToUse = appointmentBusinessLayer ?? new Mock<IAppointmentBusinessLayer>().Object;
        var mockRequestContext = new Mock<IRequestContext>();
        var mockLogger = new Mock<ILogger<ParticipantController>>();
        var mockLocalizer = new Mock<IStringLocalizer<ParticipantController>>();

        return new ParticipantController(
            appointmentBusinessLayerToUse,
            mockRequestContext.Object,
            mockLogger.Object,
            mockLocalizer.Object);
    }

    private static Participant CreateDefaultParticipant()
    {
        return new Participant
        {
            AppointmentId = ExpectedAppointmentId,
            CustomerId = ExpectedCustomerId,
            ParticipantId = ExpectedParticipantId
        };
    }
}