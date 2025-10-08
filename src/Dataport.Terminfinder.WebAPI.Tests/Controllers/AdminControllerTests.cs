namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class AdminControllerTests
{
    private static readonly Guid ExpectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
    private static readonly Guid ExpectedAdminId = new("0EB748E2-32CF-49DE-8A63-14685AC943FF");
    private static readonly Guid ExpectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly AppointmentStatusType ExpectedAppointmentStatusType = AppointmentStatusType.Started;
    private static readonly string ExpectedPassword = "P@$$w0rd";
    private static readonly string ExpectedInvalidGuidString = "invalid";

    [TestMethod]
    public void GetAppointment_Okay()
    {
        var appointment = CreateDefaultAppointment();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.Get(ExpectedCustomerId.ToString(), ExpectedAdminId.ToString());
        var result = httpResult as OkObjectResult;
        var appointmentResult = result?.Value as Appointment;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(appointment.CreatorName, appointmentResult.CreatorName);
        Assert.AreEqual(appointment.Subject, appointmentResult.Subject);
        Assert.AreEqual(appointment.Description, appointmentResult.Description);
        Assert.AreEqual(appointment.Place, appointmentResult.Place);
        Assert.AreEqual(appointment.AppointmentStatus, appointmentResult.AppointmentStatus);
        Assert.AreEqual(appointment.AdminId, appointmentResult.AdminId);
        Assert.AreEqual(appointment.AppointmentId, appointmentResult.AppointmentId);
    }

    [TestMethod]
    public void GetAppointment_AdminIdIsEmpty()
    {
        var appointment = CreateDefaultAppointment();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);

        var sut = CreateSut(mockBusinessLayer.Object);
        
        Assert.ThrowsException<BadRequestException>(() => sut.Get(ExpectedCustomerId.ToString(), string.Empty));
    }

    [TestMethod]
    public void GetAppointment_NotFound()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        Assert.ThrowsException<NotFoundException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), Guid.NewGuid().ToString()));
    }

    [TestMethod]
    public void GetAppointment_verificationFailed_Unauthorized()
    {
        var appointment = CreateDefaultAppointment();

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPasswordByAdminId(ExpectedCustomerId, ExpectedAdminId, ExpectedPassword))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        Assert.ThrowsException<UnauthorizedException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), ExpectedAdminId.ToString()));
    }

    [TestMethod]
    public void GetAppointment_verificationSuccessful_okay()
    {
        var appointment = CreateDefaultAppointment();

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPasswordByAdminId(ExpectedCustomerId, ExpectedAdminId, ExpectedPassword))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var httpResult = sut.Get(ExpectedCustomerId.ToString(), ExpectedAdminId.ToString());
        var result = httpResult as OkObjectResult;
        var appointmentResult = result?.Value as Appointment;

        // Assert
        Assert.AreEqual(ExpectedAppointmentId.ToString(), appointmentResult?.AppointmentId.ToString());
    }

    [TestMethod]
    public void GetProtection_appointmentExistsAndAppointmentIsProtectedByPassword_Okay()
    {
        var appointment = CreateDefaultAppointment();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.GetProtection(ExpectedCustomerId.ToString(), ExpectedAdminId.ToString());
        var result = httpResult as OkObjectResult;
        var appointmentProtectionResult = result?.Value as AppointmentProtectionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentProtectionResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(ExpectedAppointmentId.ToString(), appointmentProtectionResult.AppointmentId.ToString());
        Assert.IsTrue(appointmentProtectionResult.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetProtection_appointmentExistsAndAppointmentIsNotProtectedByPassword_Okay()
    {
        var appointment = CreateDefaultAppointment();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.GetProtection(ExpectedCustomerId.ToString(), ExpectedAdminId.ToString());
        var result = httpResult as OkObjectResult;
        var appointmentProtectionResult = result?.Value as AppointmentProtectionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentProtectionResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(ExpectedAppointmentId.ToString(), appointmentProtectionResult.AppointmentId.ToString());
        Assert.IsFalse(appointmentProtectionResult.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetProtection_GuidsAreInvalid_ThrowsException()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        Assert.ThrowsException<BadRequestException>(() =>
            sut.GetProtection(ExpectedInvalidGuidString, ExpectedAdminId.ToString()));
        Assert.ThrowsException<BadRequestException>(() =>
            sut.GetProtection(ExpectedCustomerId.ToString(), ExpectedInvalidGuidString));
    }

    [TestMethod]
    public void SetStatus_appointmentStatusTypeStarted_Okay()
    {
        var appointment = CreateDefaultAppointment();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(false);
        mockBusinessLayer
            .Setup(m => m.SetAppointmentStatusType(ExpectedCustomerId, ExpectedAdminId,
                It.IsAny<AppointmentStatusType>()))
            .Returns(appointment);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.SetStatus(ExpectedCustomerId.ToString(), ExpectedAdminId.ToString(),
            ExpectedAppointmentStatusType.ToString());
        var result = httpResult as OkObjectResult;
        var appointmentResult = result?.Value as Appointment;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(ExpectedAppointmentId.ToString(), appointmentResult.AppointmentId.ToString());
        Assert.AreEqual(ExpectedAppointmentStatusType, appointmentResult.AppointmentStatus);
    }

    [TestMethod]
    public void SetStatus_AppointmentIsNull_ThrowsException()
    {
        var appointment = CreateDefaultAppointment();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(false);
        mockBusinessLayer
            .Setup(m => m.SetAppointmentStatusType(ExpectedCustomerId, ExpectedAdminId,
                It.IsAny<AppointmentStatusType>()))
            .Returns((Appointment)null);

        var sut = CreateSut(mockBusinessLayer.Object);

        Assert.ThrowsException<ConflictException>(() => sut.SetStatus(ExpectedCustomerId.ToString(),
            ExpectedAdminId.ToString(), ExpectedAppointmentStatusType.ToString()));
    }

    [TestMethod]
    public void SetStatus_GuidsInvalid_ThrowsException()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        Assert.ThrowsException<BadRequestException>(() =>
            sut.SetStatus(ExpectedInvalidGuidString, ExpectedInvalidGuidString, string.Empty));
    }

    [TestMethod]
    public void GetPasswordVerification_verificationSuccessful_True()
    {
        var appointment = CreateDefaultAppointment();

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPasswordByAdminId(ExpectedCustomerId, ExpectedAdminId, ExpectedPassword))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var httpResult = sut.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedAdminId.ToString());
        var result = httpResult as OkObjectResult;
        var verificationResult = result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.IsTrue(verificationResult?.IsPasswordValid);
        Assert.IsTrue(verificationResult?.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetPasswordVerification_verificationSuccessful_False()
    {
        var appointment = CreateDefaultAppointment();

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPasswordByAdminId(ExpectedCustomerId, ExpectedAdminId, ExpectedPassword))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var httpResult = sut.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedAdminId.ToString());
        var result = httpResult as OkObjectResult;
        var verificationResult = result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.IsFalse(verificationResult?.IsPasswordValid);
        Assert.IsTrue(verificationResult?.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetPasswordVerification_AppointmentNotProtected_True()
    {
        var appointment = CreateDefaultAppointment();

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(appointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId))
            .Returns(false);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPasswordByAdminId(ExpectedCustomerId, ExpectedAdminId, ExpectedPassword))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var httpResult = sut.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedAdminId.ToString());
        var result = httpResult as OkObjectResult;
        var verificationResult = result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.IsFalse(verificationResult?.IsPasswordValid);
        Assert.IsFalse(verificationResult?.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetPasswordVerification_GuidsInvalid_ThrowsException()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        Assert.ThrowsException<BadRequestException>(() =>
            sut.GetPasswordVerification(ExpectedInvalidGuidString, ExpectedInvalidGuidString));
    }

    private static AdminController CreateSut(
        IAppointmentBusinessLayer appointmentBusinessLayer = null,
        IRequestContext requestContext = null)
    {
        var appointmentBusinessLayerToUse = appointmentBusinessLayer ?? new Mock<IAppointmentBusinessLayer>().Object;
        var requestContextToUse = requestContext ?? new Mock<IRequestContext>().Object;
        var mockLogger = new Mock<ILogger<AdminController>>();
        var mockLocalizer = new Mock<IStringLocalizer<AdminController>>();

        return new AdminController(
            appointmentBusinessLayerToUse,
            requestContextToUse,
            mockLogger.Object,
            mockLocalizer.Object);
    }

    private static Appointment CreateDefaultAppointment()
    {
        return new Appointment
        {
            AppointmentId = ExpectedAppointmentId,
            AdminId = ExpectedAdminId,
            CreatorName = "Tom",
            CustomerId = ExpectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = ExpectedAppointmentStatusType
        };
    }
}