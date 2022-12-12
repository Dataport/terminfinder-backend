namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
[ExcludeFromCodeCoverage]
public class ParticipantControllerTests
{
    private ILogger<ParticipantController> _logger;
    private IStringLocalizer<ParticipantController> _localizer;
    private IRequestContext _requestContext;

    [TestInitialize]
    public void Inilialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<ParticipantController>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<ParticipantController>>();

        // fake localizer
        var mockLocalize = new Mock<IStringLocalizer<ParticipantController>>();
        _localizer = mockLocalize.Object;
        _localizer = Mock.Of<IStringLocalizer<ParticipantController>>();

        // fake request context
        _requestContext = Mock.Of<IRequestContext>();
    }

    [TestMethod]
    public void DeleteParticipants_Okay()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid participantId = new ("9054E979-C40C-4FA3-B36A-D85803143F5D");

        Participant fakeParticipant = new ()
        {
            AppointmentId = expectedAppointmentId,
            CustomerId = expectedCustomerId,
            ParticipantId = participantId
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(fakeParticipant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentIsStarted(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsParticipant(expectedCustomerId, expectedAppointmentId, participantId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>()))
            .Returns(true);

        var controller = new ParticipantController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.Delete(expectedCustomerId.ToString(), expectedAppointmentId.ToString(),
            participantId.ToString());
        OkResult result = httpResult as OkResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void DeleteParticipants_Participants_NoInput()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid participantId = Guid.Empty;

        Participant fakeParticipant = new ()
        {
            AppointmentId = expectedAppointmentId,
            CustomerId = expectedCustomerId,
            ParticipantId = participantId
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(fakeParticipant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>()))
            .Returns(false);

        var controller = new ParticipantController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Delete(expectedCustomerId.ToString(), expectedAppointmentId.ToString(),
                participantId.ToString());
            Assert.Fail("An Exception should be thrown");
        }
        catch (BadRequestException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.NoInput, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void DeleteParticipants_Participants_NotFound()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid participantId = new ("7D0BB25C-214E-42CF-8BE3-89910733B763");

        Participant fakeParticipant = new ()
        {
            AppointmentId = expectedAppointmentId,
            CustomerId = expectedCustomerId,
            ParticipantId = participantId
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(fakeParticipant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentIsStarted(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsParticipant(expectedCustomerId, expectedAppointmentId, participantId))
            .Returns(false);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>()))
            .Returns(true);

        var controller = new ParticipantController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Delete(expectedCustomerId.ToString(), expectedAppointmentId.ToString(),
                participantId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (NotFoundException uex)
        {
            Assert.AreEqual(ErrorType.ParticipantNotFound, uex.ErrorCode);
        }
    }

    [TestMethod]
    public void DeleteParticipants_AppointmentIsPaused_NotFound()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid participantId = new ("7D0BB25C-214E-42CF-8BE3-89910733B763");

        Participant fakeParticipant = new ()
        {
            AppointmentId = expectedAppointmentId,
            CustomerId = expectedCustomerId,
            ParticipantId = participantId
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(fakeParticipant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentIsStarted(expectedCustomerId, expectedAppointmentId))
            .Returns(false);
        mockBusinessLayer.Setup(m => m.ExistsParticipant(expectedCustomerId, expectedAppointmentId, participantId))
            .Returns(false);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>()))
            .Returns(true);

        var controller = new ParticipantController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Delete(expectedCustomerId.ToString(), expectedAppointmentId.ToString(),
                participantId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (NotFoundException uex)
        {
            Assert.AreEqual(ErrorType.AppointmentNotFound, uex.ErrorCode);
        }
    }

    [TestMethod]
    public void DeleteParticipants_AppointmentIsPasswordProtectedNoPasswordSubmitted_Unauthorized()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid participantId = new ("7D0BB25C-214E-42CF-8BE3-89910733B763");

        Participant fakeParticipant = new ()
        {
            AppointmentId = expectedAppointmentId,
            CustomerId = expectedCustomerId,
            ParticipantId = participantId
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteParticipiant(fakeParticipant));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentIsStarted(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ParticipantToDeleteAreValid(It.IsAny<Participant>()))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);

        var controller = new ParticipantController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Delete(expectedCustomerId.ToString(), expectedAppointmentId.ToString(),
                participantId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (UnauthorizedException uex)
        {
            Assert.AreEqual(ErrorType.PasswordRequired, uex.ErrorCode);
        }
    }
}
