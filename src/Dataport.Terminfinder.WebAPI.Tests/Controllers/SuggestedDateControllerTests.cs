namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class SuggestedDateControllerTests
{
    private static readonly Guid ExpectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
    private static readonly Guid ExpectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly Guid ExpectedSuggestedDateId = new("9054E979-C40C-4FA3-B36A-D85803143F5D");

    private static readonly SuggestedDate ExpectedNewSuggestedDate = new();

    private static readonly SuggestedDate ExpectedSuggestedDate = new()
    {
        AppointmentId = ExpectedAppointmentId,
        CustomerId = ExpectedCustomerId,
        SuggestedDateId = ExpectedSuggestedDateId
    };

    [TestMethod]
    public void DeleteSuggestedDates_Okay()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteSuggestedDate(ExpectedSuggestedDate));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.SuggestedDateToDeleteAreValid(It.IsAny<SuggestedDate>())).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsSuggestedDate(ExpectedCustomerId, ExpectedAppointmentId, ExpectedSuggestedDateId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.Delete(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString(),
            ExpectedSuggestedDateId.ToString());
        var result = httpResult as OkResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [TestMethod]
    public void DeleteSuggestedDates_SuggestedDates_NoInput()
    {
        var expectedSuggestedDateId = Guid.Empty;

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteSuggestedDate(ExpectedNewSuggestedDate));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.SuggestedDateToDeleteAreValid(It.IsAny<SuggestedDate>())).Returns(false);
        mockBusinessLayer
            .Setup(m => m.ExistsSuggestedDate(ExpectedCustomerId, ExpectedAppointmentId, expectedSuggestedDateId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<BadRequestException>(() => sut.Delete(ExpectedCustomerId.ToString(),
            ExpectedAppointmentId.ToString(), expectedSuggestedDateId.ToString()));
        Assert.AreEqual(ErrorType.NoInput, exception.ErrorCode);
    }

    [TestMethod]
    public void DeleteSuggestedDates_SuggestedDates_NotFound()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteSuggestedDate(ExpectedNewSuggestedDate));
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.SuggestedDateToDeleteAreValid(It.IsAny<SuggestedDate>())).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsSuggestedDate(ExpectedCustomerId, ExpectedAppointmentId, ExpectedSuggestedDateId))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<NotFoundException>(() => sut.Delete(ExpectedCustomerId.ToString(),
            ExpectedAppointmentId.ToString(), ExpectedSuggestedDateId.ToString()));
        Assert.AreEqual(ErrorType.SuggestedDateNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void DeleteSuggestedDates_AppointmentIsPasswordProtectedNoPasswordSubmitted_Unauthorized()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.DeleteSuggestedDate(ExpectedNewSuggestedDate));
        mockBusinessLayer
            .Setup(m => m.ExistsSuggestedDate(ExpectedCustomerId, ExpectedAppointmentId, ExpectedSuggestedDateId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer.Setup(m => m.SuggestedDateToDeleteAreValid(It.IsAny<SuggestedDate>())).Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedException>(() => sut.Delete(ExpectedCustomerId.ToString(),
            ExpectedAppointmentId.ToString(), ExpectedSuggestedDateId.ToString()));
        Assert.AreEqual(ErrorType.PasswordRequired, exception.ErrorCode);
    }

    [TestMethod]
    public void DeleteSuggestedDates_GuidsAreInvalid_ThrowsException()
    {
        var invalidGuidString = "invalid";

        var sut = CreateSut();

        var exceptionCustomerId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Delete(invalidGuidString, ExpectedAppointmentId.ToString(), ExpectedSuggestedDateId.ToString()));
        Assert.AreEqual(ErrorType.WrongInputOrNotAllowed, exceptionCustomerId.ErrorCode);
        var exceptionAppointmentId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Delete(ExpectedCustomerId.ToString(), invalidGuidString, ExpectedSuggestedDateId.ToString()));
        Assert.AreEqual(ErrorType.WrongInputOrNotAllowed, exceptionAppointmentId.ErrorCode);
        var exceptionSuggestedDateId = Assert.ThrowsException<BadRequestException>(() =>
            sut.Delete(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString(), invalidGuidString));
        Assert.AreEqual(ErrorType.WrongInputOrNotAllowed, exceptionSuggestedDateId.ErrorCode);
    }

    private static SuggestedDateController CreateSut(IAppointmentBusinessLayer appointmentBusinessLayer = null)
    {
        var appointmentBusinessLayerToUse = appointmentBusinessLayer ?? new Mock<IAppointmentBusinessLayer>().Object;
        var mockRequestContext = new Mock<IRequestContext>();
        var mockLogger = new Mock<ILogger<SuggestedDateController>>();
        var mockLocalizer = new Mock<IStringLocalizer<SuggestedDateController>>();

        return new SuggestedDateController(
            appointmentBusinessLayerToUse,
            mockRequestContext.Object,
            mockLogger.Object,
            mockLocalizer.Object);
    }
}