namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class AppointmentControllerTests
{
    private static readonly Guid ExpectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
    private static readonly Guid ExpectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly Guid ExpectedAdminId = new("FFFD657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly Guid ExpectedSuggestedDateId1 = new("FFFD657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly Guid ExpectedSuggestedDateId2 = new("76AAC930-EC94-4B78-8F5B-B108E1A53860");
    private static readonly DateTime ExpectedStartDate1 = new(2025, 12, 12, 0, 0, 0, DateTimeKind.Local);
    private static readonly DateTime ExpectedEndDate1 = ExpectedStartDate1.AddDays(1);
    private static readonly DateTime ExpectedStartDate2 = new(2025, 12, 14, 0, 0, 0, DateTimeKind.Local);
    private static readonly DateTime ExpectedEndDate2 = ExpectedStartDate2;
    private static readonly DateTimeOffset ExpectedStartTime = new(2025, 12, 14, 20, 5, 0, new TimeSpan(1, 0, 0));
    private static readonly DateTimeOffset ExpectedEndTime = ExpectedStartTime.AddHours(1);
    private const string ExpectedPassword = "P@$$w0rd";
    private const string ExpectedInvalidGuidString = "invalid";

    private static readonly Appointment ExpectedAppointment = new()
    {
        AppointmentId = ExpectedAppointmentId,
        AdminId = ExpectedAdminId,
        CustomerId = ExpectedCustomerId,
        CreatorName = "Tom",
        Subject = "new",
        Description = "whats new",
        Place = "Hamburg",
        AppointmentStatus = AppointmentStatusType.Started
    };

    [TestMethod]
    public void GetAppointment_Okay()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        var result = httpResult as OkObjectResult;
        var appointment = result?.Value as Appointment;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointment);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(ExpectedAppointment.CreatorName, appointment.CreatorName);
        Assert.AreEqual(ExpectedAppointment.Subject, appointment.Subject);
        Assert.AreEqual(ExpectedAppointment.Description, appointment.Description);
        Assert.AreEqual(ExpectedAppointment.Place, appointment.Place);
        Assert.AreEqual(ExpectedAppointment.AppointmentStatus, appointment.AppointmentStatus);
        Assert.AreEqual(ExpectedAppointment.AdminId, Guid.Empty);
    }

    [TestMethod]
    public void GetAppointment_AppointmentId_NotFound()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(false);
        mockBusinessLayer.Setup(b => b.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId));

        var sut = CreateSut(mockBusinessLayer.Object);

        var exception = Assert.ThrowsException<NotFoundException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), Guid.NewGuid().ToString()));
        Assert.AreEqual(ErrorType.AppointmentNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void GetAppointment_AppointmentIdIsEmpty()
    {
        var participants = new List<Participant>();
        var suggestedDates = new List<SuggestedDate>();

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer
            .Setup(b => b.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, ExpectedAppointmentId, participants));
        mockBusinessLayer
            .Setup(b => b.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId,
                suggestedDates));
        mockBusinessLayer
            .Setup(b => b.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), string.Empty));
        Assert.AreEqual(ErrorType.AppointmentIdNotValid, exception.ErrorCode);
    }

    [TestMethod]
    public void GetAppointment_noUserCredentialSubmittedButTheyAreRequired_Unauthorized()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.PasswordRequired, exception.ErrorCode);
    }

    [TestMethod]
    public void GetAppointment_verificationFailed_Unauthorized()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.AuthorizationFailed, exception.ErrorCode);
    }

    [TestMethod]
    public void GetAppointment_verificationFailedPasswordCanNotBeDecoded_BadRequest()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(() => throw new DecodingBasicAuthenticationValueFailedException("Dummy"));

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.DecodingPasswordFailed, exception.ErrorCode);
    }

    [TestMethod]
    public void GetAppointment_verificationSuccessful_okay()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var httpResult = sut.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        var result = httpResult as OkObjectResult;
        var appointment = result?.Value as Appointment;

        // Assert
        Assert.AreEqual(ExpectedAppointmentId.ToString(), appointment?.AppointmentId.ToString());
    }

    [TestMethod]
    public void GetAppointment_GuidsAreInvalid_ThrowsException()
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
    public void GetAppointment_AppointmentIsNull_ThrowsException()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(x => x.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer.Setup(x => x.ExistsAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer.Setup(x => x.GetAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns((Appointment)null);

        var sut = CreateSut(mockBusinessLayer.Object);

        var exception = Assert.ThrowsException<NotFoundException>(() =>
            sut.Get(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.AppointmentNotFound, exception.ErrorCode);
    }

    [TestMethod]
    public void AddAppointment_Okay()
    {
        var appointmentToAdd = CreateDefaultAppointment();
        appointmentToAdd.SuggestedDates.Add(new SuggestedDate
        {
            SuggestedDateId = Guid.Empty,
            AppointmentId = Guid.Empty,
            CustomerId = ExpectedCustomerId,
            StartDate = ExpectedStartDate2,
            StartTime = ExpectedStartTime,
            EndDate = ExpectedEndDate2,
            EndTime = ExpectedEndTime
        });

        var expectedAppointment = ExpectedAppointment;
        expectedAppointment.SuggestedDates = new List<SuggestedDate>
        {
            new()
            {
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                SuggestedDateId = ExpectedSuggestedDateId1,
                StartDate = ExpectedStartDate1,
                EndDate = ExpectedEndDate1
            },
            new()
            {
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                SuggestedDateId = ExpectedSuggestedDateId2,
                StartDate = ExpectedStartDate2,
                StartTime = ExpectedStartTime,
                EndDate = ExpectedEndDate2,
                EndTime = ExpectedEndTime
            }
        };

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId.ToString())).Returns(true);
        mockBusinessLayer
            .Setup(bl => bl.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, appointmentToAdd.AppointmentId,
                appointmentToAdd.Participants))
            .Returns(true);
        mockBusinessLayer
            .Setup(bl => bl.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId, appointmentToAdd.AppointmentId,
                appointmentToAdd.SuggestedDates))
            .Returns(true);
        mockBusinessLayer
            .Setup(bl => bl.CheckMinTotalCountOfSuggestedDates(ExpectedCustomerId, appointmentToAdd.AppointmentId,
                appointmentToAdd.SuggestedDates))
            .Returns(true);
        mockBusinessLayer.Setup(bl => bl.AddAppointment(appointmentToAdd)).Returns(expectedAppointment);
        mockBusinessLayer.Setup(bl => bl.SaveAppointment()).Returns(0);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.Post(appointmentToAdd, appointmentToAdd.CustomerId.ToString());
        var result = httpResult as CreatedResult;
        var appointmentResult = result?.Value as Appointment;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(appointmentResult);
        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(ExpectedAppointmentId, appointmentResult.AppointmentId);
    }

    [TestMethod]
    public void AddAppointment_WithAppointmentId_FailedNotFound()
    {
        var appointment = CreateDefaultAppointment();
        appointment.AppointmentId = ExpectedAppointmentId;

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId.ToString())).Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<BadRequestException>(() => 
            sut.Post(appointment, ExpectedCustomerId.ToString()));
        Assert.AreEqual(ErrorType.IdsMustBeEmpty, exception.ErrorCode);
    }

    [TestMethod]
    public void AddAppointment_WithAdminId_FailedNotFound()
    {
        var appointment = CreateDefaultAppointment();
        appointment.AdminId = ExpectedAdminId;

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(ExpectedCustomerId.ToString())).Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Post(appointment, ExpectedCustomerId.ToString()));
        Assert.AreEqual(ErrorType.IdsMustBeEmpty, exception.ErrorCode);
    }

    [TestMethod]
    public void AddAppointment_ParticipantCountExceedsLimit_ThrowsException()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer
            .Setup(bl =>
                bl.CheckMaxTotalCountOfParticipants(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<ICollection<Participant>>()))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Post(new Appointment(), ExpectedCustomerId.ToString()));
        Assert.AreEqual(ErrorType.MaximumElementsOfParticipantsAreExceeded, exception.ErrorCode);
    }

    [TestMethod]
    public void AddAppointment_SuggestedDateCountExceedsLimit_ThrowsException()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer
            .Setup(bl =>
                bl.CheckMaxTotalCountOfParticipants(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<ICollection<Participant>>()))
            .Returns(true);
        mockBusinessLayer
            .Setup(bl =>
                bl.CheckMaxTotalCountOfSuggestedDates(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<ICollection<SuggestedDate>>()))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Post(new Appointment(), ExpectedCustomerId.ToString()));
        Assert.AreEqual(ErrorType.MaximumElementsOfSuggestedDatesAreExceeded, exception.ErrorCode);
    }

    [TestMethod]
    public void AddAppointment_SuggestedDateCountSubceedsLimit_ThrowsException()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(bl => bl.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        mockBusinessLayer
            .Setup(bl =>
                bl.CheckMaxTotalCountOfParticipants(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<ICollection<Participant>>()))
            .Returns(true);
        mockBusinessLayer
            .Setup(bl =>
                bl.CheckMaxTotalCountOfSuggestedDates(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<ICollection<SuggestedDate>>()))
            .Returns(true);
        mockBusinessLayer
            .Setup(bl =>
                bl.CheckMinTotalCountOfSuggestedDates(It.IsAny<Guid>(), It.IsAny<Guid>(),
                    It.IsAny<ICollection<SuggestedDate>>()))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        var exception = Assert.ThrowsException<BadRequestException>(() =>
            sut.Post(new Appointment(), ExpectedCustomerId.ToString()));
        Assert.AreEqual(ErrorType.MinimumElementsOfSuggestedDatesAreNotExceeded, exception.ErrorCode);
    }

    [TestMethod]
    public void AddAppointment_verificationFailed_Unauthorized()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId, ExpectedAdminId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var exception = Assert.ThrowsException<UnauthorizedException>(() =>
            sut.Put(new Appointment
            {
                AppointmentId = ExpectedAppointmentId,
                AdminId = ExpectedAdminId
            }, ExpectedCustomerId.ToString()));
        Assert.AreEqual(ErrorType.AuthorizationFailed, exception.ErrorCode);
    }

    [TestMethod]
    public void UpdateAppointment_CustomerIdIsInvalid_ThrowsException()
    {
        var appointment = new Appointment
        {
            AppointmentId = ExpectedAppointmentId,
            AdminId = ExpectedAdminId
        };

        var mockRequestContext = new Mock<IRequestContext>();
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        var exception = Assert.ThrowsException<BadRequestException>(() => 
            sut.Put(appointment, ExpectedInvalidGuidString));
        Assert.AreEqual(ErrorType.CustomerIdNotValid, exception.ErrorCode);
    }

    [TestMethod]
    public void GetProtection_appointmentExistsAndAppointmentIsProtectedByPassword_Okay()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.GetProtection(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
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
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object);

        // Act
        var httpResult = sut.GetProtection(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
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
        var sut = CreateSut();

        var exceptionCustomerId = Assert.ThrowsException<BadRequestException>(() =>
            sut.GetProtection(ExpectedInvalidGuidString, ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.CustomerIdNotValid, exceptionCustomerId.ErrorCode);
        var exceptionAppointmentId = Assert.ThrowsException<BadRequestException>(() =>
            sut.GetProtection(ExpectedCustomerId.ToString(), ExpectedInvalidGuidString));
        Assert.AreEqual(ErrorType.AppointmentIdNotValid, exceptionAppointmentId.ErrorCode);
    }

    [TestMethod]
    public void GetPasswordVerification_verificationSuccessful_True()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext
            .Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(true);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var httpResult = sut.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        var result = httpResult as OkObjectResult;
        var verificationResult = result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.IsTrue(verificationResult?.IsPasswordValid);
        Assert.IsTrue(verificationResult?.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetPasswordVerification_verificationSuccessful_False()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(true);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var httpResult = sut.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        var result = httpResult as OkObjectResult;
        var verificationResult = result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.IsFalse(verificationResult?.IsPasswordValid);
        Assert.IsTrue(verificationResult?.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetPasswordVerification_AppointmentNotProtected_True()
    {
        var mockRequestContext = new Mock<IRequestContext>();
        mockRequestContext.Setup(c => c.GetDecodedBasicAuthCredentials())
            .Returns(new UserCredential { Password = ExpectedPassword });

        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockBusinessLayer.Setup(m => m.ExistsCustomer(ExpectedCustomerId)).Returns(true);
        mockBusinessLayer.Setup(m => m.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId)).Returns(true);
        mockBusinessLayer
            .Setup(m => m.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(ExpectedAppointment);
        mockBusinessLayer
            .Setup(m => m.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(false);
        mockBusinessLayer
            .Setup(m => m.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword))
            .Returns(false);

        var sut = CreateSut(mockBusinessLayer.Object, mockRequestContext.Object);

        // Act
        var httpResult = sut.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedAppointmentId.ToString());
        var result = httpResult as OkObjectResult;
        var verificationResult = result?.Value as AppointmentPasswordVerificationResult;

        // Assert
        Assert.IsFalse(verificationResult?.IsPasswordValid);
        Assert.IsFalse(verificationResult?.IsProtectedByPassword);
    }

    [TestMethod]
    public void GetPasswordVerification_GuidsAreInvalid_ThrowsException()
    {
        var mockBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        var sut = CreateSut(mockBusinessLayer.Object);

        var exceptionCustomerId = Assert.ThrowsException<BadRequestException>(() =>
            sut.GetPasswordVerification(ExpectedInvalidGuidString, ExpectedAppointmentId.ToString()));
        Assert.AreEqual(ErrorType.CustomerIdNotValid, exceptionCustomerId.ErrorCode);
        var exceptionAppointmentId = Assert.ThrowsException<BadRequestException>(() =>
            sut.GetPasswordVerification(ExpectedCustomerId.ToString(), ExpectedInvalidGuidString));
        Assert.AreEqual(ErrorType.AppointmentIdNotValid, exceptionAppointmentId.ErrorCode);
    }

    private static AppointmentController CreateSut(
        IAppointmentBusinessLayer appointmentBusinessLayer = null,
        IRequestContext requestContext = null)
    {
        var appointmentBusinessLayerToUse = appointmentBusinessLayer ?? new Mock<IAppointmentBusinessLayer>().Object;
        var requestContextToUse = requestContext ?? new Mock<IRequestContext>().Object;
        var mockLogger = new Mock<ILogger<AppointmentController>>();
        var mockLocalizer = new Mock<IStringLocalizer<AppointmentController>>();

        // see https://github.com/aspnet/Mvc/issues/3586
        var objectValidator = new Mock<IObjectModelValidator>();
        objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
            It.IsAny<ValidationStateDictionary>(),
            It.IsAny<string>(),
            It.IsAny<Object>()));

        return new AppointmentController(
            appointmentBusinessLayerToUse,
            requestContextToUse,
            mockLogger.Object,
            mockLocalizer.Object)
        {
            ObjectValidator = objectValidator.Object,
            ControllerContext =
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

    private static Appointment CreateDefaultAppointment()
    {
        return new Appointment
        {
            AppointmentId = Guid.Empty,
            AdminId = Guid.Empty,
            CustomerId = ExpectedCustomerId,
            CreatorName = "Tom",
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started,
            SuggestedDates = new List<SuggestedDate>
            {
                new()
                {
                    AppointmentId = Guid.Empty,
                    CustomerId = ExpectedCustomerId,
                    SuggestedDateId = Guid.Empty,
                    StartDate = ExpectedStartDate1,
                    EndDate = ExpectedEndDate1
                }
            }
        };
    }
}