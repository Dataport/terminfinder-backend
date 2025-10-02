namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class AdminControllerTests
{
    private ILogger<AdminController> _logger;
    private IStringLocalizer<AdminController> _localizer;
    private IRequestContext _requestContext;

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<AdminController>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<AdminController>>();

        // fake localizer
        var mockLocalize = new Mock<IStringLocalizer<AdminController>>();
        _localizer = mockLocalize.Object;
        _localizer = Mock.Of<IStringLocalizer<AdminController>>();

        // fake request context
        _requestContext = Mock.Of<IRequestContext>();
    }

    [TestMethod]
    public void GetAppointment_Okay()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedAdminId = new ("0EB748E2-32CF-49DE-8A63-14685AC943FF");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);

        var controller = new AdminController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.Get(expectedCustomerId.ToString(), expectedAdminId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        Appointment appointment = result?.Value as Appointment;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointment);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(appointment.CreatorName, fakeAppointment.CreatorName);
        Assert.AreEqual(appointment.Subject, fakeAppointment.Subject);
        Assert.AreEqual(appointment.Description, fakeAppointment.Description);
        Assert.AreEqual(appointment.Place, fakeAppointment.Place);
        Assert.AreEqual(appointment.AppointmentStatus, fakeAppointment.AppointmentStatus);
        Assert.AreEqual(appointment.AdminId, fakeAppointment.AdminId);
        Assert.AreEqual(appointment.AppointmentId, fakeAppointment.AppointmentId);
    }

    [TestMethod]
    public void GetAppointment_AdminIdIsEmpty()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedAdminId = new ("0EB748E2-32CF-49DE-8A63-14685AC943FF");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);

        var controller = new AdminController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Get(expectedCustomerId.ToString(), string.Empty);
            Assert.Fail("An Exception should be thrown");
        }
        catch (BadRequestException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.WrongInputOrNotAllowed, ex.ErrorCode);
        }
        catch (Exception)
        {
            Assert.Fail("Wrong Exception should be thrown");
        }
    }

    [TestMethod]
    public void GetAppointment_NotFound()
    {
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(false);

        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);

        var controller = new AdminController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Get(expectedCustomerId.ToString(), "3FA29CE7-8508-4543-AD1F-58AABDF10458");
            Assert.Fail("An Exception should be thrown");
        }
        catch (NotFoundException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.AppointmentNotFound, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void GetAppointment_verificationFailed_Unauthorized()
    {
        const string expectedPassword = "P@$$w0rd";

        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAdminId = new ("FFFD657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = expectedPassword
        });
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtectedByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPasswordByAdminId(expectedCustomerId, expectedAdminId, expectedPassword))
            .Returns(false);

        var controller = new AdminController(mockBusinessLayer.Object, mockRequestContext.Object, _logger, _localizer);

        // Act
        try
        {
            controller.Get(expectedCustomerId.ToString(), expectedAdminId.ToString());
            Assert.Fail("An Exception should be thrown");
        }
        catch (UnauthorizedException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.AuthorizationFailed, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void GetAppointment_verificationSuccessful_okay()
    {
        const string expectedPassword = "P@$$w0rd";

        const string expectedAppointmentIdString = "C1C2474B-488A-4ECF-94E8-47387BB715D5";
        Guid expectedAppointmentId = new (expectedAppointmentIdString);
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAdminId = new ("FFFD657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = expectedPassword
        });
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtectedByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPasswordByAdminId(expectedCustomerId, expectedAdminId, expectedPassword))
            .Returns(true);

        var controller = new AdminController(mockBusinessLayer.Object, mockRequestContext.Object, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.Get(expectedCustomerId.ToString(), expectedAdminId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        Appointment appointment = result?.Value as Appointment;

        // Assert
        Assert.AreEqual(new Guid(expectedAppointmentIdString).ToString(), appointment?.AppointmentId.ToString());
    }

    [TestMethod]
    public void GetProtection_appointmentExistsAndAppointmentIsProtectedByPassword_Okay()
    {
        const string appointmentIdAsString = "C1C2474B-488A-4ECF-94E8-47387BB715D5";
        const string adminIdAsString = "FFF2474B-488A-4ECF-94E8-47387BB715D5";
        Guid expectedAppointmentId = new (appointmentIdAsString);
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAdminId = new (adminIdAsString);

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtectedByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);

        var controller = new AdminController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.GetProtection(expectedCustomerId.ToString(), expectedAdminId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        AppointmentProtectionResult appointmentProtectionResult = result?.Value as AppointmentProtectionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentProtectionResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(new Guid(appointmentIdAsString).ToString(),
            appointmentProtectionResult.AppointmentId.ToString());
        Assert.AreEqual(true, appointmentProtectionResult.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetProtection_appointmentExistsAndAppointmentIsNotProtectedByPassword_Okay()
    {
        const string appointmentIdAsString = "C1C2474B-488A-4ECF-94E8-47387BB715D5";
        const string adminIdAsString = "FFF2474B-488A-4ECF-94E8-47387BB715D5";
        Guid expectedAppointmentId = new(appointmentIdAsString);
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAdminId = new(adminIdAsString);

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtectedByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(false);

        var controller = new AdminController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.GetProtection(expectedCustomerId.ToString(), expectedAdminId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        AppointmentProtectionResult appointmentProtectionResult = result?.Value as AppointmentProtectionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentProtectionResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(new Guid(appointmentIdAsString).ToString(),
            appointmentProtectionResult.AppointmentId.ToString());
        Assert.AreEqual(false, appointmentProtectionResult.IsProtectedByPassword);
    }

    [TestMethod]
    public void SetStatus_appointmentStatusTypeStarted_Okay()
    {
        const string appointmentIdAsString = "C1C2474B-488A-4ECF-94E8-47387BB715D5";
        const string adminIdAsString = "FFF2474B-488A-4ECF-94E8-47387BB715D5";
        Guid expectedAppointmentId = new (appointmentIdAsString);
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAdminId = new (adminIdAsString);
        AppointmentStatusType statusType = AppointmentStatusType.Started;

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = statusType
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtectedByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(false);
        mockBusinessLayer.Setup(m =>
                m.SetAppointmentStatusType(expectedCustomerId, expectedAdminId, It.IsAny<AppointmentStatusType>()))
            .Returns(fakeAppointment);

        var controller = new AdminController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.SetStatus(expectedCustomerId.ToString(), expectedAdminId.ToString(),
            statusType.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        Appointment appointmentResult = result?.Value as Appointment;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(new Guid(appointmentIdAsString).ToString(), appointmentResult.AppointmentId.ToString());
        Assert.AreEqual(true, appointmentResult.AppointmentStatus == statusType);
    }

    [TestMethod]
    public void GetPasswordVerification_verificationSuccessful_True()
    {
        const string expectedPassword = "P@$$w0rd";

        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAdminId = new ("FFF2474B-488A-4ECF-94E8-47387BB715D5");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = expectedPassword
        });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtectedByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPasswordByAdminId(expectedCustomerId, expectedAdminId, expectedPassword))
            .Returns(true);

        var controller = new AdminController(mockBusinessLayer.Object, mockRequestContext.Object, _logger, _localizer);

        // Act
        IActionResult httpResult =
            controller.GetPasswordVerification(expectedCustomerId.ToString(), expectedAdminId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        AppointmentPasswordVerificationResult verificationResult =
            result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.AreEqual(true, verificationResult?.IsPasswordValid);
        Assert.AreEqual(true, verificationResult?.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetPasswordVerification_verificationSuccessful_False()
    {
        const string expectedPassword = "P@$$w0rd";

        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAdminId = new ("FFF2474B-488A-4ECF-94E8-47387BB715D5");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = expectedPassword
        });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtectedByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPasswordByAdminId(expectedCustomerId, expectedAdminId, expectedPassword))
            .Returns(false);

        var controller = new AdminController(mockBusinessLayer.Object, mockRequestContext.Object, _logger, _localizer);

        // Act
        IActionResult httpResult =
            controller.GetPasswordVerification(expectedCustomerId.ToString(), expectedAdminId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        AppointmentPasswordVerificationResult verificationResult =
            result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.AreEqual(false, verificationResult?.IsPasswordValid);
        Assert.AreEqual(true, verificationResult?.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetPasswordVerification_AppointmentNotProtected_True()
    {
        const string expectedPassword = "P@$$w0rd";

        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAdminId = new ("FFF2474B-488A-4ECF-94E8-47387BB715D5");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = expectedAdminId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = expectedPassword
        });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtectedByAdminId(expectedCustomerId, expectedAdminId))
            .Returns(false);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPasswordByAdminId(expectedCustomerId, expectedAdminId, expectedPassword))
            .Returns(false);

        var controller = new AdminController(mockBusinessLayer.Object, mockRequestContext.Object, _logger, _localizer);

        // Act
        IActionResult httpResult =
            controller.GetPasswordVerification(expectedCustomerId.ToString(), expectedAdminId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        AppointmentPasswordVerificationResult verificationResult =
            result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.AreEqual(false, verificationResult?.IsPasswordValid);
        Assert.AreEqual(false, verificationResult?.IsProtectedByPassword);
    }
}
