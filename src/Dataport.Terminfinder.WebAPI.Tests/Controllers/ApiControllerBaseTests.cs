namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class ApiControllerBaseTests
{
    private static readonly Guid ExpectedCustomerId = Guid.Parse("06DC0487-DE35-4ABE-8165-5238C81A5A9C");
    private static readonly Guid ExpectedAppointmentId = Guid.Parse("359666CA-3498-4C94-A3D4-82321B21CAFB");
    private static readonly Guid ExpectedAdminId = Guid.Parse("3D3FEAB0-7766-434E-8398-5E0AC40DF9FC");

    [TestMethod]
    public void BuildAdditionalErrorMessageFromModelState_ModelStateIsValid_ThrowsException()
    {
        var expectedExceptionMessage = "ModelState contains no errors and is valid";
        
        var sut = CreateSut();
        sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        sut.ModelState.Clear();

        var exception = Assert.ThrowsException<ArgumentException>(() => 
            sut.PublicBuildAdditionalErrorMessageFromModelState());
        Assert.AreEqual(expectedExceptionMessage, exception.Message);
    }

    [TestMethod]
    public void ValidateCustomerRequest_CustomerIdIsInvalid_ThrowsException()
    {
        var invalidCustomerId = Guid.Empty;
        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();

        var sut = CreateSut();
        
        var exception = Assert.ThrowsException<NotFoundException>(() =>
            sut.PublicValidateCustomerRequest(invalidCustomerId, mockAppointmentBusinessLayer.Object));
        Assert.AreEqual(ErrorType.CustomerIdNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void ValidateCustomerRequest_AppointmentBusinessLayerIsNull_ThrowsException()
    {
        var sut = CreateSut();

        var exception = Assert.ThrowsException<NotFoundException>(() => 
            sut.PublicValidateCustomerRequest(ExpectedCustomerId, null));
        Assert.AreEqual(ErrorType.CustomerIdNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void ValidateCustomerRequest_CustomerWithIdDoesNotExist_ThrowsException()
    {
        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer.Setup(x => x.ExistsCustomer(It.IsAny<Guid>())).Returns(false);

        var sut = CreateSut();
        
        var exception = Assert.ThrowsException<NotFoundException>(() =>
            sut.PublicValidateCustomerRequest(ExpectedCustomerId, mockAppointmentBusinessLayer.Object));
        Assert.AreEqual(ErrorType.CustomerIdNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void ValidateAppointmentRequest_AppointmentIdIsInvalid_ThrowsException()
    {
        var invalidAppointmentId = Guid.Empty;
        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer.Setup(x => x.ExistsCustomer(It.IsAny<Guid>())).Returns(true);

        var sut = CreateSut();
        
        var exception = Assert.ThrowsException<NotFoundException>(() =>
            sut.PublicValidateAppointmentRequest(ExpectedCustomerId, invalidAppointmentId,
                mockAppointmentBusinessLayer.Object));
        Assert.AreEqual(ErrorType.AppointmentIdNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void ValidateAppointmentRequest_AdminIdIsInvalid_ThrowsException()
    {
        var invalidAdminId = Guid.Empty;
        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(x => x.ExistsCustomer(It.IsAny<Guid>()))
            .Returns(true);
        mockAppointmentBusinessLayer
            .Setup(x => x.ExistsAppointment(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(false);

        var sut = CreateSut();
        
        var exception = Assert.ThrowsException<NotFoundException>(() =>
            sut.PublicValidateAppointmentRequest(ExpectedCustomerId, ExpectedAppointmentId, invalidAdminId,
                mockAppointmentBusinessLayer.Object));
        Assert.AreEqual(ErrorType.AppointmentIdNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void ValidateAppointmentRequest_AppointmentDoesNotExist_ThrowsException()
    {
        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(x => x.ExistsCustomer(It.IsAny<Guid>()))
            .Returns(true);
        mockAppointmentBusinessLayer
            .Setup(x => x.ExistsAppointment(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(false);

        var sut = CreateSut();
        
        var exception = Assert.ThrowsException<NotFoundException>(() =>
            sut.PublicValidateAppointmentRequest(ExpectedCustomerId, ExpectedAppointmentId, ExpectedAdminId,
                mockAppointmentBusinessLayer.Object));
        Assert.AreEqual(ErrorType.AppointmentNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void IsAppointmentPasswordValid_PasswordIsNull_ReturnsFalse()
    {
        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(x => x.IsAppointmentPasswordProtected(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(true);
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(x => x.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = null });

        var sut = CreateSut(mockRequestContext.Object);
        var result = sut.PublicIsAppointmentPasswordValid(ExpectedCustomerId, ExpectedAppointmentId,
            mockAppointmentBusinessLayer.Object);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsAppointmentPasswordValidByAdminId_PasswordIsNull_ReturnsFalse()
    {
        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(x => x.IsAppointmentPasswordProtectedByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(true);
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(x => x.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = null });

        var sut = CreateSut(mockRequestContext.Object);
        var result = sut.PublicIsAppointmentPasswordValidByAdminId(ExpectedCustomerId, ExpectedAdminId,
            mockAppointmentBusinessLayer.Object);

        Assert.IsFalse(result);
    }

    private static UtApiControllerBase CreateSut(IRequestContext requestContext = null)
    {
        var requestContextToUse = requestContext ?? new Mock<IRequestContext>().Object;
        var mockLogger = new Mock<ILogger<UtApiControllerBase>>();
        var mockStringLocalizer = new Mock<IStringLocalizer<UtApiControllerBase>>();

        return new UtApiControllerBase(requestContextToUse, mockLogger.Object, mockStringLocalizer.Object);
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public class UtApiControllerBase(IRequestContext requestContext, ILogger logger, IStringLocalizer localizer)
        : ApiControllerBase(requestContext, logger, localizer)
    {
        public string PublicBuildAdditionalErrorMessageFromModelState()
        {
            return BuildAdditionalErrorMessageFromModelState();
        }

        public void PublicValidateCustomerRequest(
            Guid customerIdFromRequest,
            IAppointmentBusinessLayer appointmentBusinessLayer)
        {
            ValidateCustomerRequest(customerIdFromRequest, appointmentBusinessLayer);
        }

        public void PublicValidateAppointmentRequest(
            Guid customerIdFromRequest,
            Guid appointmentId,
            IAppointmentBusinessLayer appointmentBusinessLayer)
        {
            ValidateAppointmentRequest(customerIdFromRequest, appointmentId, appointmentBusinessLayer);
        }

        public void PublicValidateAppointmentRequest(
            Guid customerIdFromRequest,
            Guid appointmentId,
            Guid adminId,
            IAppointmentBusinessLayer appointmentBusinessLayer)
        {
            ValidateAppointmentRequest(customerIdFromRequest, appointmentId, adminId, appointmentBusinessLayer);
        }

        public bool PublicIsAppointmentPasswordValid(
            Guid customerId,
            Guid appointmentId,
            IAppointmentBusinessLayer appointmentBusinessLayer)
        {
            return IsAppointmentPasswordValid(customerId, appointmentId, appointmentBusinessLayer);
        }

        public bool PublicIsAppointmentPasswordValidByAdminId(
            Guid customerId,
            Guid adminId,
            IAppointmentBusinessLayer appointmentBusinessLayer)
        {
            return IsAppointmentPasswordValidByAdminId(customerId, adminId, appointmentBusinessLayer);
        }
    }
}