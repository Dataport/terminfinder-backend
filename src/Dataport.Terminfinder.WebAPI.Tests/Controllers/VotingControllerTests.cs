namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class VotingControllerTests
{
    private static readonly Guid ExpectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
    private static readonly Guid ExpectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly Guid ExpectedSuggestedDateId = new("1EC377A8-35C7-442F-97E6-DE6BD09A661B");
    private static readonly string ExpectedInvalidGuidString = "invalid";

    [TestMethod]
    public void Get_AppointmentIsPasswordProtectedAndNoPasswordSubmitted_Unauthorized()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.PasswordRequired, exception.ErrorCode);
    }

    [TestMethod]
    public void Get_GuidsAreInvalid_ThrowsException()
    {
        var sut = CreateSut();

        var exceptionCustomerId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Get(ExpectedInvalidGuidString, ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.CustomerIdNotValid, exceptionCustomerId.ErrorCode);
        var exceptionAppointmentId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), ExpectedInvalidGuidString));
        Assert.AreEqual(ErrorType.AppointmentIdNotValid, exceptionAppointmentId.ErrorCode);
    }

    [TestMethod]
    public void Get_ParticipantsIsNull_ThrowsException()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetParticipants(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns((List<Participant>)null);

        var sut = CreateSut(mockBusinessLayer.Object);

        var exception = Assert.ThrowsException<NotFoundException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.AppointmentNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void Put_AppointmentIsPasswordProtectedAndNoPasswordSubmitted_Unauthorized()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
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
            sut.Put([null], ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.PasswordRequired, exception.ErrorCode);
    }

    [TestMethod]
    public void Put_AppointmentStatusIsPaused_NotFound()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(false);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<NotFoundException>(() =>
            sut.Put([null], ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.AppointmentNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void Put_AddVoting_True()
    {
        var participants = new Participant[]
        {
            new()
            {
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                ParticipantId = Guid.Empty,
                Name = "Joe",
                Votings = new List<Voting>
                {
                    new()
                    {
                        VotingId = Guid.Empty,
                        CustomerId = ExpectedCustomerId,
                        AppointmentId = ExpectedAppointmentId,
                        ParticipantId = Guid.Empty,
                        SuggestedDateId = ExpectedSuggestedDateId,
                        Status = VotingStatusType.Accepted
                    }
                }
            }
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantsAreValid(participants)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(false);
        mockBusinessLayer
            .Setup(m => m.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, ExpectedAppointmentId, participants))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.AddAndUpdateParticipants(ExpectedCustomerId, ExpectedAppointmentId, participants))
            .Returns(participants);
        mockBusinessLayer
            .Setup(m => m.SetParticipantsForeignKeys(participants, ExpectedCustomerId, ExpectedAppointmentId));

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.Put(participants, ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        var result = httpResult as CreatedResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
    }

    [TestMethod]
    public void Put_ParticipantsIsNull_ThrowsException()
    {
        var sut = CreateSut();

        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Put(null, ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.NoInput, exception.ErrorCode);
    }

    [TestMethod]
    public void Put_GuidsAreInvalid_ThrowsException()
    {
        var participants = new Participant[] { new() };

        var sut = CreateSut();

        var exceptionCustomerId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Put(participants, ExpectedInvalidGuidString, ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.CustomerIdNotValid, exceptionCustomerId.ErrorCode);
        var exceptionAppointmentId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Put(participants, ExpectedCustomerId.ToString(), ExpectedInvalidGuidString));
        Assert.AreEqual(ErrorType.AppointmentIdNotValid, exceptionAppointmentId.ErrorCode);
    }

    [TestMethod]
    public void Put_ParticipantsAreInvalid_ThrowsException()
    {
        var participants = new Participant[] { new() };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(x => x.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer.Setup(x => x.ExistsAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer.Setup(x => x.ParticipantsAreValid(It.IsAny<ICollection<Participant>>())).Returns(false);
        mockBusinessLayer.Setup(x => x.ExistsAppointmentIsStarted(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);
        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Put(participants, ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.ParticipantNotValid, exception.ErrorCode);
    }

    [TestMethod]
    public void Put_ModelIsInvalid_ThrowsException()
    {
        var participants = new Participant[]
        {
            new()
            {
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                ParticipantId = Guid.Empty,
                Name = "Joe",
                Votings = new List<Voting>()
            }
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(x => x.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer.Setup(x => x.ExistsAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer.Setup(x => x.ParticipantsAreValid(It.IsAny<ICollection<Participant>>())).Returns(true);
        mockBusinessLayer.Setup(x => x.ExistsAppointmentIsStarted(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);

        var objectModelValidator = new Mock<IObjectModelValidator>();
        objectModelValidator
            .Setup(o => o.Validate(
                It.IsAny<ActionContext>(),
                It.IsAny<ValidationStateDictionary>(),
                It.IsAny<string>(),
                It.IsAny<Object>()))
            .Callback<ActionContext, ValidationStateDictionary, string, object>((ac, _, prefix, _) =>
            {
                ac.ModelState.AddModelError($"{prefix}", "Validation Error");
            });

        var sut = CreateSut(mockBusinessLayer.Object, objectModelValidator.Object);
        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Put(participants, ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.AppointmentNotValid, exception.ErrorCode);
    }

    [TestMethod]
    public void Put_ParticipantCountExceedsLimit_ThrowsException()
    {
        var participants = new Participant[]
        {
            new()
            {
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                ParticipantId = Guid.Empty,
                Name = "Joe",
                Votings = new List<Voting>()
            }
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(x => x.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer.Setup(x => x.ExistsAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer.Setup(x => x.ParticipantsAreValid(It.IsAny<ICollection<Participant>>())).Returns(true);
        mockBusinessLayer.Setup(x => x.ExistsAppointmentIsStarted(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer
            .Setup(x => x.CheckMaxTotalCountOfParticipants(It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.IsAny<ICollection<Participant>>()))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Put(participants, ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.MaximumElementsOfParticipantsAreExceeded, exception.ErrorCode);
    }

    private static VotingController CreateSut(
        IAppointmentBusinessLayer appointmentBusinessLayer = null,
        IObjectModelValidator objectModelValidator = null)
    {
        var appointmentBusinessLayerToUse = appointmentBusinessLayer ?? new Mock<IAppointmentBusinessLayer>().Object;
        var mockRequestContext = new Mock<IRequestContext>();
        var mockLogger = new Mock<ILogger<VotingController>>();
        var mockLocalizer = new Mock<IStringLocalizer<VotingController>>();

        var objectModelValidatorToUse = objectModelValidator ?? CreateDefaultObjectModelValidator();

        return new VotingController(
            appointmentBusinessLayerToUse,
            mockRequestContext.Object,
            mockLogger.Object,
            mockLocalizer.Object)
        {
            ObjectValidator = objectModelValidatorToUse,
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
                {
                    Request =
                    {
                        Scheme = "http",
                        Host = new HostString("localhost", 50018)
                    }
                }
            }
        };
    }

    private static IObjectModelValidator CreateDefaultObjectModelValidator()
    {
        var mockObjectModelValidator = new Mock<IObjectModelValidator>();
        mockObjectModelValidator.Setup(o => o.Validate(
            It.IsAny<ActionContext>(),
            It.IsAny<ValidationStateDictionary>(),
            It.IsAny<string>(),
            It.IsAny<Object>()));

        return mockObjectModelValidator.Object;
    }
}