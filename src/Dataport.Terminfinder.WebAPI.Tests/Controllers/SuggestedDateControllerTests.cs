namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
[ExcludeFromCodeCoverage]
public class SuggestedDateControllerTests
{
    private ILogger<SuggestedDateController> _logger;
    private IStringLocalizer<SuggestedDateController> _localizer;
    private IRequestContext _requestContext;

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<SuggestedDateController>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<SuggestedDateController>>();

        // fake localizer
        var mockLocalize = new Mock<IStringLocalizer<SuggestedDateController>>();
        _localizer = mockLocalize.Object;
        _localizer = Mock.Of<IStringLocalizer<SuggestedDateController>>();

        // fake request context
        _requestContext = Mock.Of<IRequestContext>();
    }

    [TestMethod]
    public void DeleteSuggestedDates_Okay()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedSuggestedDateId = new ("9054E979-C40C-4FA3-B36A-D85803143F5D");

        SuggestedDate fakeSuggestedDate = new ()
        {
            AppointmentId = expectedAppointmentId,
            CustomerId = expectedCustomerId,
            SuggestedDateId = expectedSuggestedDateId
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteSuggestedDate(fakeSuggestedDate));
        mockBusinessLayer.Setup(m =>
                m.ExistsSuggestedDate(expectedCustomerId, expectedAppointmentId, expectedSuggestedDateId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.SuggestedDateToDeleteAreValid(It.IsAny<SuggestedDate>()))
            .Returns(true);

        var controller = new SuggestedDateController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.Delete(expectedCustomerId.ToString(), expectedAppointmentId.ToString(),
            expectedSuggestedDateId.ToString());
        OkResult result = httpResult as OkResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void DeleteSuggestedDates_SuggestedDates_NoInput()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedSuggestedDateId = Guid.Empty;

        SuggestedDate fakeSuggestedDate = new ();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteSuggestedDate(fakeSuggestedDate));
        mockBusinessLayer.Setup(m =>
                m.ExistsSuggestedDate(expectedCustomerId, expectedAppointmentId, expectedSuggestedDateId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.SuggestedDateToDeleteAreValid(It.IsAny<SuggestedDate>()))
            .Returns(false);

        var controller = new SuggestedDateController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Delete(expectedCustomerId.ToString(), expectedAppointmentId.ToString(),
                expectedSuggestedDateId.ToString());
            Assert.Fail("An Exception should be thrown");
        }
        catch (BadRequestException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.NoInput, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void DeleteSuggestedDates_SuggestedDates_NotFound()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedSuggestedDateId = new ("7D0BB25C-214E-42CF-8BE3-89910733B763");

        SuggestedDate fakeSuggestedDate = new ();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteSuggestedDate(fakeSuggestedDate));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.ExistsSuggestedDate(expectedCustomerId, expectedAppointmentId, expectedSuggestedDateId))
            .Returns(false);
        mockBusinessLayer.Setup(m => m.SuggestedDateToDeleteAreValid(It.IsAny<SuggestedDate>()))
            .Returns(true);

        var controller = new SuggestedDateController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Delete(expectedCustomerId.ToString(), expectedAppointmentId.ToString(),
                expectedSuggestedDateId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (NotFoundException uex)
        {
            Assert.AreEqual(ErrorType.SuggestedDateNotFound, uex.ErrorCode);
        }
    }

    [TestMethod]
    public void DeleteSuggestedDates_AppointmentIsPasswordProtectedNoPasswordSubmitted_Unauthorized()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedSuggestedDateId = new ("FFFD657A-4D06-40DB-8443-D67BBB950EE7");

        SuggestedDate fakeSuggestedDate = new();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteSuggestedDate(fakeSuggestedDate));
        mockBusinessLayer.Setup(m =>
                m.ExistsSuggestedDate(expectedCustomerId, expectedAppointmentId, expectedSuggestedDateId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.SuggestedDateToDeleteAreValid(It.IsAny<SuggestedDate>()))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);

        var controller = new SuggestedDateController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Delete(expectedCustomerId.ToString(), expectedAppointmentId.ToString(),
                expectedSuggestedDateId.ToString());
            Assert.Fail("An Exception should be thrown");
        }
        catch (UnauthorizedException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.PasswordRequired, ex.ErrorCode);
        }
    }
}
