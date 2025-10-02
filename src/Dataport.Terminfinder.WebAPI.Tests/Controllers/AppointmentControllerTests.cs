namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class AppointmentControllerTests
{
    private ILogger<AppointmentController> _logger;
    private IStringLocalizer<AppointmentController> _localizer;
    private IRequestContext _requestContext;

    private static readonly Guid ExpectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
    private static readonly Guid ExpectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly Guid ExpectedAdminId = new("FFFD657A-4D06-40DB-8443-D67BBB950EE7");
    private const string ExpectedPassword = "P@$$w0rd";

    private static readonly Appointment FakeAppointment = new()
    {
        AppointmentId = ExpectedAppointmentId,
        AdminId = ExpectedAdminId,
        CreatorName = "Tom",
        CustomerId = ExpectedCustomerId,
        Subject = "new",
        Description = "whats new",
        Place = "Hamburg",
        AppointmentStatus = AppointmentStatusType.Started
    };

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        _logger = Mock.Of<ILogger<AppointmentController>>();

        // fake localizer
        _localizer = Mock.Of<IStringLocalizer<AppointmentController>>();

        // fake request context
        _requestContext = Mock.Of<IRequestContext>();
    }

    [TestMethod]
    public void GetAppointment_Okay()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        Appointment appointment = result?.Value as Appointment;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointment);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(appointment.CreatorName, FakeAppointment.CreatorName);
        Assert.AreEqual(appointment.Subject, FakeAppointment.Subject);
        Assert.AreEqual(appointment.Description, FakeAppointment.Description);
        Assert.AreEqual(appointment.Place, FakeAppointment.Place);
        Assert.AreEqual(appointment.AppointmentStatus, FakeAppointment.AppointmentStatus);
        Assert.AreEqual(Guid.Empty, FakeAppointment.AdminId);
    }

    [TestMethod]
    public void GetAppointment_AppointmentId_NotFound()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(false);
        mockBusinessLayer.Setup(b => b.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId));

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Get(ExpectedCustomerId.ToString(), "ECFE296D-220B-4BF3-9488-20F81DECA40A");
            Assert.Fail("An Exception should be thrown");
        }
        catch (NotFoundException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.AppointmentNotFound, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void GetAppointment_AppointmentIdIsEmpty()
    {
        IList<Participant> fakeListOfParticipants = new List<Participant>();
        IList<SuggestedDate> fakeListOfSuggestedDates = new List<SuggestedDate>();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(b =>
            b.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, ExpectedAppointmentId, fakeListOfParticipants));
        mockBusinessLayer.Setup(b =>
            b.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId,
                fakeListOfSuggestedDates));
        mockBusinessLayer.Setup(b => b.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Get(ExpectedCustomerId.ToString(), string.Empty);
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
    public void GetAppointment_noUserCredentialSubmittedButTheyAreRequired_Unauthorized()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
            Assert.Fail("An Exception should be thrown");
        }
        catch (UnauthorizedException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.PasswordRequired, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void GetAppointment_verificationFailed_Unauthorized()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = ExpectedPassword
        });
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        try
        {
            controller.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
            Assert.Fail("An Exception should be thrown");
        }
        catch (UnauthorizedException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.AuthorizationFailed, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void GetAppointment_verificationFailedPasswordCanNotBeDecoded_BadRequest()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(() =>
            throw new DecodingBasicAuthenticationValueFailedException("Dummy"));
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        try
        {
            controller.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
            Assert.Fail("An Exception should be thrown");
        }
        catch (BadRequestException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.DecodingPasswordFailed, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void GetAppointment_verificationSuccessful_okay()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = ExpectedPassword
        });
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        IActionResult httpResult = controller.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        Appointment appointment = result?.Value as Appointment;

        // Assert
        Assert.AreEqual(ExpectedAppointmentId.ToString(), appointment?.AppointmentId.ToString());
    }

    [TestMethod]
    public void AddAppointment_Okay()
    {
        Guid expectedSuggestedDateId1 = new("5FE9C00C-A59C-4985-A2BB-53D179C2B52C");

        Appointment fakeAppointment = new()
        {
            AppointmentId = Guid.Empty,
            CreatorName = "Tom",
            CustomerId = ExpectedCustomerId,
            AdminId = Guid.Empty,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        SuggestedDate fakeSuggestedDate1 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = ExpectedCustomerId,
            SuggestedDateId = Guid.Empty,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        SuggestedDate fakeSuggestedDate2 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = ExpectedCustomerId,
            SuggestedDateId = Guid.Empty,
            StartDate = DateTime.Now.AddDays(2),
            StartTime = DateTimeOffset.Now.AddHours(1),
            EndDate = DateTime.Now.AddDays(2),
            EndTime = DateTimeOffset.Now.AddHours(2)
        };
        fakeAppointment.SuggestedDates = new List<SuggestedDate>
        {
            fakeSuggestedDate1,
            fakeSuggestedDate2
        };

        Appointment fakeAppointmentReturn = new()
        {
            AppointmentId = ExpectedAppointmentId,
            CreatorName = "Tom",
            CustomerId = ExpectedCustomerId,
            AdminId = ExpectedAdminId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        SuggestedDate fakeSuggestedDateReturn1 = new()
        {
            AppointmentId = fakeAppointmentReturn.AppointmentId,
            CustomerId = ExpectedCustomerId,
            SuggestedDateId = expectedSuggestedDateId1,
            StartDate = new DateTime(2018, 12, 12),
            EndDate = new DateTime(2018, 12, 12).AddDays(1)
        };

        SuggestedDate fakeSuggestedDateReturn2 = new()
        {
            AppointmentId = fakeAppointmentReturn.AppointmentId,
            CustomerId = ExpectedCustomerId,
            SuggestedDateId = new("76AAC930-EC94-4B78-8F5B-B108E1A53860"),
            StartDate = new DateTime(2018, 12, 14),
            StartTime = new DateTimeOffset(2018, 12, 14, 20, 05, 0, new TimeSpan(1, 0, 0)),
            EndDate = new DateTime(2018, 12, 14),
            EndTime = new DateTimeOffset(2018, 12, 14, 21, 05, 0, new TimeSpan(1, 0, 0))
        };
        fakeAppointmentReturn.SuggestedDates = new List<SuggestedDate>
        {
            fakeSuggestedDateReturn1,
            fakeSuggestedDateReturn2
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId.ToString()))
            .Returns(true);
        mockBusinessLayer.Setup(bl => bl.CheckMaxTotalCountOfParticipants(ExpectedCustomerId,
            fakeAppointment.AppointmentId, fakeAppointment.Participants)).Returns(true);
        mockBusinessLayer.Setup(bl => bl.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId,
            fakeAppointment.AppointmentId, fakeAppointment.SuggestedDates)).Returns(true);
        mockBusinessLayer.Setup(bl => bl.CheckMinTotalCountOfSuggestedDates(ExpectedCustomerId,
            fakeAppointment.AppointmentId, fakeAppointment.SuggestedDates)).Returns(true);
        mockBusinessLayer.Setup(bl => bl.AddAppointment(fakeAppointment)).Returns(fakeAppointment);
        mockBusinessLayer.Setup(bl => bl.SaveAppointment())
            .Returns(0);

        // see https://github.com/aspnet/Mvc/issues/3586
        var objectValidator = new Mock<IObjectModelValidator>();
        objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
            It.IsAny<ValidationStateDictionary>(),
            It.IsAny<string>(),
            It.IsAny<Object>()));

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer)
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
        IActionResult httpResult = controller.Post(fakeAppointment, ExpectedCustomerId.ToString());
        CreatedResult result = httpResult as CreatedResult;
        Appointment appointment = result?.Value as Appointment;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointment);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(appointment.CreatorName, fakeAppointmentReturn.CreatorName);
        Assert.AreEqual(appointment.Subject, fakeAppointmentReturn.Subject);
        Assert.AreEqual(appointment.Description, fakeAppointmentReturn.Description);
        Assert.AreEqual(appointment.Place, fakeAppointmentReturn.Place);
        Assert.AreEqual(appointment.AppointmentStatus, fakeAppointmentReturn.AppointmentStatus);
        Assert.AreEqual(ExpectedAdminId, fakeAppointmentReturn.AdminId);
        Assert.AreEqual(ExpectedAppointmentId, fakeAppointmentReturn.AppointmentId);
        foreach (SuggestedDate sd in fakeAppointmentReturn.SuggestedDates)
        {
            if (sd.StartDate == new DateTime(2018, 12, 12))
            {
                Assert.AreEqual(expectedSuggestedDateId1, sd.SuggestedDateId);
                Assert.AreEqual(ExpectedCustomerId, sd.CustomerId);
                Assert.AreEqual(ExpectedAppointmentId, sd.AppointmentId);
                break;
            }
        }
    }

    [TestMethod]
    public void AddAppointment_WithAppointmentId_FailedNotFound()
    {
        Appointment fakeAppointment = new()
        {
            AppointmentId = ExpectedAppointmentId,
            CreatorName = "Tom",
            CustomerId = ExpectedCustomerId,
            AdminId = Guid.Empty,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        SuggestedDate fakeSuggestedDate1 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = ExpectedCustomerId,
            SuggestedDateId = Guid.Empty,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        fakeAppointment.SuggestedDates = new List<SuggestedDate>
        {
            fakeSuggestedDate1
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId.ToString()))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Scheme = "http";
        controller.ControllerContext.HttpContext.Request.Host = new HostString("localhost", 50018);

        // Act
        try
        {
            controller.Post(fakeAppointment, ExpectedCustomerId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (BadRequestException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.WrongInputOrNotAllowed, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void AddAppointment_WithAdminId_FailedNotFound()
    {
        Appointment fakeAppointment = new()
        {
            AppointmentId = Guid.Empty,
            CreatorName = "Tom",
            CustomerId = ExpectedCustomerId,
            AdminId = ExpectedAdminId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        SuggestedDate fakeSuggestedDate1 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = ExpectedCustomerId,
            SuggestedDateId = Guid.Empty,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        fakeAppointment.SuggestedDates = new List<SuggestedDate>
        {
            fakeSuggestedDate1
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId.ToString()))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Scheme = "http";
        controller.ControllerContext.HttpContext.Request.Host = new HostString("localhost", 50018);

        // Act
        try
        {
            controller.Post(fakeAppointment, ExpectedCustomerId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (BadRequestException ex)
        {
            // Assert
            Assert.AreEqual(ErrorType.WrongInputOrNotAllowed, ex.ErrorCode);
        }
    }

    [TestMethod]
    public void AddAppointment_verificationFailed_Unauthorized()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = ExpectedPassword
        });
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId, ExpectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        try
        {
            controller.Put(new Appointment
            {
                AppointmentId = ExpectedAppointmentId,
                AdminId = ExpectedAdminId
            }, ExpectedCustomerId.ToString());
            Assert.Fail("An exception should be thrown");
        }
        catch (UnauthorizedException unex)
        {
            // Assert
            Assert.AreEqual(ErrorType.AuthorizationFailed, unex.ErrorCode);
        }
    }

    [TestMethod]
    public void GetProtection_appointmentExistsAndAppointmentIsProtectedByPassword_Okay()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult =
            controller.GetProtection(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        AppointmentProtectionResult appointmentProtectionResult = result?.Value as AppointmentProtectionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentProtectionResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(ExpectedAppointmentId.ToString(),
            appointmentProtectionResult.AppointmentId.ToString());
        Assert.AreEqual(true, appointmentProtectionResult.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetProtection_appointmentExistsAndAppointmentIsNotProtectedByPassword_Okay()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult =
            controller.GetProtection(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        AppointmentProtectionResult appointmentProtectionResult = result?.Value as AppointmentProtectionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentProtectionResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(ExpectedAppointmentId.ToString(),
            appointmentProtectionResult.AppointmentId.ToString());
        Assert.AreEqual(false, appointmentProtectionResult.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetPasswordVerification_verificationSuccessful_True()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = ExpectedPassword
        });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        IActionResult httpResult =
            controller.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
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
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = ExpectedPassword
        });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        IActionResult httpResult =
            controller.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
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
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(new UserCredential
        {
            Password = ExpectedPassword
        });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(FakeAppointment);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(false);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        IActionResult httpResult =
            controller.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        AppointmentPasswordVerificationResult verificationResult =
            result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.AreEqual(false, verificationResult?.IsPasswordValid);
        Assert.AreEqual(false, verificationResult?.IsProtectedByPassword);
    }
}