namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
[ExcludeFromCodeCoverage]
public class VotingControllerTests
{
    private ILogger<VotingController> _logger;
    private IStringLocalizer<VotingController> _localizer;
    private IRequestContext _requestContext;

    [TestInitialize]
    public void Inilialize()
    {
        // fake logger
        _logger = Mock.Of<ILogger<VotingController>>();

        // fake localizer
        _localizer = Mock.Of<IStringLocalizer<VotingController>>();

        // fake request context
        _requestContext = Mock.Of<IRequestContext>();
    }

    [TestMethod]
    public void Get_AppointmentIsPasswordProtectedAndNoPasswordSubmitted_Unauthorized()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);

        var controller = new VotingController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Get(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (UnauthorizedException unex)
        {
            // Assert
            Assert.AreEqual(ErrorType.PasswordRequired, unex.ErrorCode);
        }
    }

    [TestMethod]
    public void Put_AppointmentIsPasswordProtectedAndNoPasswordSubmitted_Unauthorized()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentIsStarted(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);

        var controller = new VotingController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Put(new Participant[] { null }, expectedCustomerId.ToString(), expectedAppointmentId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (UnauthorizedException unex)
        {
            // Assert
            Assert.AreEqual(ErrorType.PasswordRequired, unex.ErrorCode);
        }
    }

    [TestMethod]
    public void Put_AppointmentStatusIsPaused_NotFound()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentIsStarted(expectedCustomerId, expectedAppointmentId))
            .Returns(false);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(false);

        var controller = new VotingController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Put(new Participant[] { null }, expectedCustomerId.ToString(), expectedAppointmentId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (NotFoundException unex)
        {
            // Assert
            Assert.AreEqual(ErrorType.AppointmentNotFound, unex.ErrorCode);
        }
    }

    [TestMethod]
    public void Put_AddVoting_True()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid fakeSuggestedDateId = new ("1EC377A8-35C7-442F-97E6-DE6BD09A661B");

        Voting fakeVoting = new ()
        {
            VotingId = Guid.Empty,
            CustomerId = expectedCustomerId,
            AppointmentId = expectedAppointmentId,
            ParticipantId = Guid.Empty,
            SuggestedDateId = fakeSuggestedDateId,
            Status = VotingStatusType.Accepted
        };

        Participant[] participants = new Participant[1];
        Participant fakeParticipant = new()
        {
            AppointmentId = expectedAppointmentId,
            CustomerId = expectedCustomerId,
            ParticipantId = Guid.Empty,
            Name = "Joe",
            Votings = new List<Voting>()
        };
        fakeParticipant.Votings.Add(fakeVoting);
        participants[0] = fakeParticipant;


        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentIsStarted(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(false);
        mockBusinessLayer.Setup(m => m.ParticipantsAreValid(participants))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.CheckMaxTotalCountOfParticipants(expectedCustomerId, expectedAppointmentId, participants))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.AddAndUpdateParticipiants(expectedCustomerId, expectedAppointmentId, participants))
            .Returns(participants);
        mockBusinessLayer.Setup(m =>
            m.SetParticipantsForeignKeys(participants, expectedCustomerId, expectedAppointmentId));

        // see https://github.com/aspnet/Mvc/issues/3586
        var objectValidator = new Mock<IObjectModelValidator>();
        objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
            It.IsAny<ValidationStateDictionary>(),
            It.IsAny<string>(),
            It.IsAny<Object>()));

        var controller = new VotingController(mockBusinessLayer.Object, _requestContext, _logger, _localizer)
            {
                ObjectValidator = objectValidator.Object,
                ControllerContext =
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        controller.ControllerContext.HttpContext.Request.Scheme = "http";
        controller.ControllerContext.HttpContext.Request.Host = new HostString("localhost", 50018);

        // Act
        IActionResult httpResult =
            controller.Put(participants, expectedCustomerId.ToString(), expectedAppointmentId.ToString());
        CreatedResult result = httpResult as CreatedResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(201, result.StatusCode);
    }
}