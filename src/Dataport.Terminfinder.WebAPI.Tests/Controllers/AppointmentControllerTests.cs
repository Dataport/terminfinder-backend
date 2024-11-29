namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
[ExcludeFromCodeCoverage]
public class AppointmentControllerTests
{
    private ILogger<AppointmentController> _logger;
    private IStringLocalizer<AppointmentController> _localizer;
    private IRequestContext _requestContext;

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
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = Guid.NewGuid(),
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.Get(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
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
        Assert.AreEqual(Guid.Empty, fakeAppointment.AdminId);
    }

    [TestMethod]
    public void GetAppointment_AppointmentId_NotFound()
    {
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(false);
        mockBusinessLayer.Setup(b => b.GetAppointment(expectedCustomerId, expectedAppointmentId));

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Get(expectedCustomerId.ToString(), "ECFE296D-220B-4BF3-9488-20F81DECA40A");
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
        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        IList<Participant> fakeListOfParticipants = new List<Participant>();
        IList<SuggestedDate> fakeListOfSuggestedDates = new List<SuggestedDate>();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(b =>
            b.CheckMaxTotalCountOfParticipants(expectedCustomerId, expectedAppointmentId, fakeListOfParticipants));
        mockBusinessLayer.Setup(b =>
            b.CheckMaxTotalCountOfSuggestedDates(expectedCustomerId, expectedAppointmentId,
                fakeListOfSuggestedDates));
        mockBusinessLayer.Setup(b => b.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

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
    public void GetAppointment_noUserCredentialSubmittedButTheyAreRequired_Unauthorized()
    {
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

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        try
        {
            controller.Get(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
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
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(expectedCustomerId, expectedAppointmentId, expectedPassword))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        try
        {
            controller.Get(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
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
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials()).Returns(() =>
            throw new DecodingBasicAuthenticationValueFailedException("Dummy"));
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        try
        {
            controller.Get(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
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
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(expectedCustomerId, expectedAppointmentId, expectedPassword))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        IActionResult httpResult = controller.Get(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        Appointment appointment = result?.Value as Appointment;

        // Assert
        Assert.AreEqual(new Guid(expectedAppointmentIdString).ToString(), appointment?.AppointmentId.ToString());
    }

    [TestMethod]
    public void AddAppointment_Okay()
    {
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAdminId = new ("8FD9B537-2BF4-4931-A86B-F805715BAF8C");
        Guid expectedAppointmentId = new ("3C1178B4-7115-46DA-AABA-F388ADAEF76E");
        Guid expectedSuggestedDateId1 = new ("5FE9C00C-A59C-4985-A2BB-53D179C2B52C");

        Appointment fakeAppointment = new()
        {
            AppointmentId = Guid.Empty,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            AdminId = Guid.Empty,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        SuggestedDate fakeSuggestedDate1 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = expectedCustomerId,
            SuggestedDateId = Guid.Empty,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        SuggestedDate fakeSuggestedDate2 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = expectedCustomerId,
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
            AppointmentId = expectedAppointmentId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            AdminId = expectedAdminId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        SuggestedDate fakeSuggestedDateReturn1 = new()
        {
            AppointmentId = fakeAppointmentReturn.AppointmentId,
            CustomerId = expectedCustomerId,
            SuggestedDateId = expectedSuggestedDateId1,
            StartDate = new DateTime(2018, 12, 12),
            EndDate = new DateTime(2018, 12, 12).AddDays(1)
        };

        SuggestedDate fakeSuggestedDateReturn2 = new ()
        {
            AppointmentId = fakeAppointmentReturn.AppointmentId,
            CustomerId = expectedCustomerId,
            SuggestedDateId = new ("76AAC930-EC94-4B78-8F5B-B108E1A53860"),
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
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(expectedCustomerId.ToString()))
            .Returns(true);
        mockBusinessLayer.Setup(bl => bl.CheckMaxTotalCountOfParticipants(expectedCustomerId,
            fakeAppointment.AppointmentId, fakeAppointment.Participants)).Returns(true);
        mockBusinessLayer.Setup(bl => bl.CheckMaxTotalCountOfSuggestedDates(expectedCustomerId,
            fakeAppointment.AppointmentId, fakeAppointment.SuggestedDates)).Returns(true);
        mockBusinessLayer.Setup(bl => bl.CheckMinTotalCountOfSuggestedDates(expectedCustomerId,
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
        IActionResult httpResult = controller.Post(fakeAppointment, expectedCustomerId.ToString());
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
        Assert.AreEqual(expectedAdminId, fakeAppointmentReturn.AdminId);
        Assert.AreEqual(expectedAppointmentId, fakeAppointmentReturn.AppointmentId);
        foreach (SuggestedDate sd in fakeAppointmentReturn.SuggestedDates)
        {
            if (sd.StartDate == new DateTime(2018, 12, 12))
            {
                Assert.AreEqual(expectedSuggestedDateId1, sd.SuggestedDateId);
                Assert.AreEqual(expectedCustomerId, sd.CustomerId);
                Assert.AreEqual(expectedAppointmentId, sd.AppointmentId);
                break;
            }
        }
    }

    [TestMethod]
    public void AddAppointment_WithAppointmentId_FailedNotFound()
    {
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid appointmentId = new ("3C1178B4-7115-46DA-AABA-F388ADAEF76E");

        Appointment fakeAppointment = new()
        {
            AppointmentId = appointmentId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            AdminId = Guid.Empty,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        SuggestedDate fakeSuggestedDate1 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = expectedCustomerId,
            SuggestedDateId = Guid.Empty,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        fakeAppointment.SuggestedDates = new List<SuggestedDate>
        {
            fakeSuggestedDate1
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(expectedCustomerId.ToString()))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Scheme = "http";
        controller.ControllerContext.HttpContext.Request.Host = new HostString("localhost", 50018);

        // Act
        try
        {
            controller.Post(fakeAppointment, expectedCustomerId.ToString());
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
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid adminId = new ("3C1178B4-7115-46DA-AABA-F388ADAEF76E");

        Appointment fakeAppointment = new()
        {
            AppointmentId = Guid.Empty,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            AdminId = adminId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        SuggestedDate fakeSuggestedDate1 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = expectedCustomerId,
            SuggestedDateId = Guid.Empty,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(1)
        };

        fakeAppointment.SuggestedDates = new List<SuggestedDate>
        {
            fakeSuggestedDate1
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(expectedCustomerId.ToString()))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.ControllerContext.HttpContext.Request.Scheme = "http";
        controller.ControllerContext.HttpContext.Request.Host = new HostString("localhost", 50018);

        // Act
        try
        {
            controller.Post(fakeAppointment, expectedCustomerId.ToString());
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
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId, expectedAdminId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(expectedCustomerId, expectedAppointmentId, expectedPassword))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        try
        {
            controller.Put(new Appointment
            {
                AppointmentId = expectedAppointmentId,
                AdminId = expectedAdminId
            }, expectedCustomerId.ToString());
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
        const string appointmentIdAsString = "C1C2474B-488A-4ECF-94E8-47387BB715D5";
        Guid expectedAppointmentId = new (appointmentIdAsString);
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = Guid.NewGuid(),
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult =
            controller.GetProtection(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
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
        Guid expectedAppointmentId = new (appointmentIdAsString);
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            AdminId = Guid.NewGuid(),
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.ExistsCustomer(expectedCustomerId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult =
            controller.GetProtection(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
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
    public void GetPasswordVerification_verificationSuccessful_True()
    {
        const string expectedPassword = "P@$$w0rd";

        Guid expectedAppointmentId = new ("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
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
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(expectedCustomerId, expectedAppointmentId, expectedPassword))
            .Returns(true);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        IActionResult httpResult =
            controller.GetPasswordVerification(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
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

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
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
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(expectedCustomerId, expectedAppointmentId, expectedPassword))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        IActionResult httpResult =
            controller.GetPasswordVerification(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
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

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
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
        mockBusinessLayer.Setup(m => m.ExistsAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(true);
        mockBusinessLayer.Setup(m => m.GetAppointment(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeAppointment);
        mockBusinessLayer.Setup(m => m.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId))
            .Returns(false);
        mockBusinessLayer.Setup(m =>
                m.VerifyAppointmentPassword(expectedCustomerId, expectedAppointmentId, expectedPassword))
            .Returns(false);

        var controller = new AppointmentController(mockBusinessLayer.Object, mockRequestContext.Object, _logger,
            _localizer);

        // Act
        IActionResult httpResult =
            controller.GetPasswordVerification(expectedCustomerId.ToString(), expectedAppointmentId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        AppointmentPasswordVerificationResult verificationResult =
            result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.AreEqual(false, verificationResult?.IsPasswordValid);
        Assert.AreEqual(false, verificationResult?.IsProtectedByPassword);
    }
}