using Dataport.Terminfinder.BusinessLayer.Security;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.Repository;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dataport.Terminfinder.BusinessLayer.Tests;

[TestClass]
public class AppointmentBusinessLayerTests
{
    private static readonly Guid ExpectedAppointmentId = Guid.Parse("C1C2474B-488A-4ECF-94E8-47387BB715D5");
    private static readonly Guid ExpectedAdminId = Guid.Parse("021CC2B6-9C84-4925-A927-36CC03BCC138");
    private static readonly Guid ExpectedCustomerId = Guid.Parse("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly Guid ExpectedParticipantId1 = Guid.Parse("09F659EF-CBBE-4B8F-BF0F-73BC3C942C0A");
    private static readonly Guid ExpectedParticipantId2 = Guid.Parse("D1C2DB5B-3FD8-4D8B-9A64-0C90298897D0");
    private static readonly Guid ExpectedVotingId = Guid.Parse("B198FD02-2C48-4932-AC34-A6878C65DC36");
    private static readonly VotingStatusType ExpectedVotingStatus = VotingStatusType.Accepted;
    private static readonly string ExpectedPassword = "P4$$w0rd";
    private static readonly string ExpectedHashPassword =
        "$2b$10$bKHadGFqngTajUrRAozjxeS3r5Mz6.nQwOBjT.kUcBeIF7FFYVt2W";

    #region CheckMaxTotalCountOfParticipants

    [TestMethod]
    public void CheckMaxTotalCountOfParticipants_Add_Okay()
    {
        var participants = CreateParticipants(20);
        var fakeCountOfParticipantsInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, ExpectedAppointmentId, participants);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipants_AddAndModify_Okay()
    {
        var participants = CreateParticipantsWithId(40, 2);
        var fakeCountOfParticipantsInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, ExpectedAppointmentId, participants);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipants_MaxElementsAddReached_Okay()
    {
        var participants = CreateParticipants(100);
        var fakeCountOfParticipantsInDatabase = 4900;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, ExpectedAppointmentId, participants);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipants_MaxElementsAddModifyReached_Okay()
    {
        var participants = CreateParticipantsWithId(200, 2);
        var fakeCountOfParticipantsInDatabase = 4900;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, ExpectedAppointmentId, participants);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipants_Add_False()
    {
        var participants = CreateParticipants(101);
        var fakeCountOfParticipantsInDatabase = 4900;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, ExpectedAppointmentId, participants);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipants_AddModifyAndDelete_False()
    {
        var participants = CreateParticipantsWithId(204, 2);
        var fakeCountOfParticipantsInDatabase = 4900;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfParticipants(ExpectedCustomerId, ExpectedAppointmentId, participants);

        Assert.IsFalse(result);
    }

    #endregion

    #region CheckMaxTotalCountOfSuggestedDates

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDates_Add_Okay()
    {
        var suggestedDates = CreateSuggestedDates(20);
        var fakeCountOfSuggestedDatesInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId, suggestedDates);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDates_AddAndModify_Okay()
    {
        var suggestedDates = CreateSuggestedDatesWithId(40, 2);
        var fakeCountOfSuggestedDatesInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId, suggestedDates);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDatesMaxElements_AddReached_Okay()
    {
        var suggestedDates = CreateSuggestedDates(10);
        var fakeCountOfSuggestedDatesInDatabase = 90;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId, suggestedDates);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDates_MaxElements_AddAndModifyReached_Okay()
    {
        var suggestedDates = CreateSuggestedDatesWithId(20, 2);
        var fakeCountOfSuggestedDatesInDatabase = 90;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId, suggestedDates);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDates_Add_False()
    {
        var suggestedDates = CreateSuggestedDates(15);
        var fakeCountOfSuggestedDatesInDatabase = 90;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId, suggestedDates);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDates_AddAndModify_False()
    {
        var suggestedDates = CreateSuggestedDatesWithId(22, 2);
        var fakeCountOfSuggestedDatesInDatabase = 90;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMaxTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId, suggestedDates);

        Assert.IsFalse(result);
    }

    #endregion

    #region CheckMinTotalCountOfSuggestedDates

    [TestMethod]
    public void CheckMinTotalCountOfSuggestedDates_Add_Okay()
    {
        var suggestedDates = CreateSuggestedDates(20);
        var fakeCountOfSuggestedDatesInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMinTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId, suggestedDates);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void CheckMinTotalCountOfSuggestedDates_MinElementsAddNotReached_Okay()
    {
        var suggestedDates = new List<SuggestedDate>();
        var fakeCountOfSuggestedDatesInDatabase = 0;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMinTotalCountOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId, suggestedDates);

        Assert.IsFalse(result);
    }

    #endregion

    #region CheckMinTotalCountOfSuggestedDatesWithToDeletedDates

    [TestMethod]
    public void CheckMinTotalCountOfSuggestedDatesWithToDeletedDates_False()
    {
        var suggestedDates = new List<SuggestedDate>();

        for (var i = 0; i < 5; i++)
        {
            SuggestedDate fakeSuggestedDate = new()
            {
                StartDate = DateTime.Now,
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                SuggestedDateId = Guid.NewGuid()
            };

            suggestedDates.Add(fakeSuggestedDate);
        }

        var fakeCountOfSuggestedDatesInDatabase = 5;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMinTotalCountOfSuggestedDatesWithToDeletedDates(ExpectedCustomerId,
            ExpectedAppointmentId, suggestedDates);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void CheckMinTotalCountOfSuggestedDatesWithToDeletedDates_True()
    {
        var suggestedDates = new List<SuggestedDate>();

        for (var i = 0; i < 4; i++)
        {
            SuggestedDate fakeSuggestedDate1 = new()
            {
                StartDate = DateTime.Now,
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                SuggestedDateId = Guid.NewGuid()
            };

            suggestedDates.Add(fakeSuggestedDate1);
        }

        var fakeCountOfSuggestedDatesInDatabase = 5;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(ExpectedCustomerId, ExpectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);
        var result = sut.CheckMinTotalCountOfSuggestedDatesWithToDeletedDates(ExpectedCustomerId,
            ExpectedAppointmentId, suggestedDates);

        Assert.IsTrue(result);
    }

    #endregion

    #region SetAppointmentForeignKeys

    [TestMethod]
    public void SetAppointmentForeignKeys_Okay()
    {
        Appointment fakeAppointment = new()
        {
            AppointmentId = ExpectedAppointmentId,
            CreatorName = "Tom",
            CustomerId = ExpectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started,
            SuggestedDates = new List<SuggestedDate>()
        };

        SuggestedDate fakeSuggestedDate1 = new() { StartDate = DateTime.Now };
        fakeAppointment.SuggestedDates.Add(fakeSuggestedDate1);

        SuggestedDate fakeSuggestedDate2 = new() { StartDate = DateTime.Now };
        fakeAppointment.SuggestedDates.Add(fakeSuggestedDate2);

        fakeSuggestedDate1.Votings = new List<Voting> { new() };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        sut.SetAppointmentForeignKeys(fakeAppointment, ExpectedCustomerId);

        Assert.AreEqual(ExpectedCustomerId, fakeAppointment.CustomerId);
        foreach (var sd in fakeAppointment.SuggestedDates)
        {
            Assert.AreEqual(ExpectedCustomerId, sd.CustomerId);
            Assert.AreEqual(ExpectedAppointmentId, sd.AppointmentId);
        }
    }

    #endregion

    #region SetParticipantsForeignKeys

    [TestMethod]
    public void SetParticipantsForeignKeys_Okay()
    {
        var numberOfElements = 2;

        List<Participant> fakeParticipants =
        [
            new()
            {
                Name = "Joe",
                ParticipantId = ExpectedParticipantId1,
                Votings = new List<Voting>
                {
                    new() { Status = VotingStatusType.Accepted }, new() { Status = VotingStatusType.Accepted }
                }
            },

            new()
            {
                Name = "Cevin",
                ParticipantId = ExpectedParticipantId2,
                Votings = new List<Voting>
                {
                    new() { Status = VotingStatusType.Accepted }, new() { Status = VotingStatusType.Accepted }
                }
            }
        ];

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        sut.SetParticipantsForeignKeys(fakeParticipants, ExpectedCustomerId, ExpectedAppointmentId);

        foreach (var participant in fakeParticipants)
        {
            Assert.AreEqual(ExpectedCustomerId, participant.CustomerId);
            Assert.AreEqual(ExpectedAppointmentId, participant.AppointmentId);
            var expectedParticipantId = participant.ParticipantId;

            var votings = participant.Votings.ToList();

            for (var i = 0; i < numberOfElements; i++)
            {
                var voting = votings[i];

                Assert.AreEqual(ExpectedCustomerId, voting.CustomerId);
                Assert.AreEqual(ExpectedAppointmentId, voting.AppointmentId);
                Assert.AreEqual(expectedParticipantId, voting.ParticipantId);
            }
        }
    }

    #endregion

    #region GetAppointment

    [TestMethod]
    public void GetAppointment_getExistingAppointment_appointmentPasswordNotSet()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment { AppointmentId = ExpectedAppointmentId, Password = ExpectedPassword }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId);
        Assert.IsNotNull(result);
        Assert.IsNull(result.Password);
    }

    [TestMethod]
    public void GetAppointment_NotExistingCustomerId_Null()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment { AppointmentId = ExpectedAppointmentId, Password = ExpectedPassword }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(false);
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.GetAppointment(ExpectedCustomerId, ExpectedAppointmentId);
        Assert.IsNull(result);
    }

    #endregion

    #region IsAppointmentPasswordProtected

    [TestMethod]
    public void IsAppointmentPasswordProtected_appointmentExistsAndHasAnyPassword_true()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentPassword(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns("Dummy");
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.IsAppointmentPasswordProtected(ExpectedCustomerId, ExpectedAppointmentId);
        Assert.IsTrue(result);
    }

    #endregion

    #region GetAppointmentByAdminId

    [TestMethod]
    public void GetAppointmentByAdminId_getExistingAppointment_appointmentPasswordNotSet()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment { AdminId = ExpectedAdminId, Password = ExpectedPassword }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId);
        Assert.IsNotNull(result);
        Assert.IsNull(result.Password);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_NotExistingCustomerId_Null()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment { AdminId = ExpectedAdminId, Password = ExpectedPassword }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(false);
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.GetAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId);
        Assert.IsNull(result);
    }

    #endregion

    #region IsAppointmentPasswordProtectedByAdminId

    [TestMethod]
    public void IsAppointmentPasswordProtectedByAdminId_appointmentExistsAndHasAnyPassword_true()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentPasswordByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns("Dummy");
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.IsAppointmentPasswordProtectedByAdminId(ExpectedCustomerId, ExpectedAdminId);
        Assert.IsTrue(result);
    }

    #endregion

    #region AddAppointment

    [TestMethod]
    public void AddAppointment_createAppointment_appointmentPasswordOfResultNotSet()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = ExpectedAppointmentId, CustomerId = ExpectedCustomerId, Password = ExpectedPassword
            }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.AddAppointment(new Appointment
        {
            AppointmentId = ExpectedAppointmentId, CustomerId = ExpectedCustomerId, Password = ExpectedPassword
        });
        Assert.IsNotNull(result);
        Assert.IsNull(result.Password);
    }

    [TestMethod]
    public void AddAppointment_createAppointment_hashPasswordInAppointmentAndStoreItInDb()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                Password = ExpectedHashPassword
            }
        );
        mockAppointmentRepo.Setup(r => r.AddAndUpdateAppointment(It.IsAny<Appointment>()))
            .Callback<Appointment>(s => { Assert.AreEqual(ExpectedHashPassword, s.Password); });

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.AddAppointment(new Appointment
        {
            AppointmentId = ExpectedAppointmentId, CustomerId = ExpectedCustomerId, Password = ExpectedPassword
        });
        Assert.IsNotNull(result);
        mockAppointmentRepo.Verify(r => r.AddAndUpdateAppointment(It.IsAny<Appointment>()), Times.Once());
    }

    #endregion

    #region UpdateAppointment

    [TestMethod]
    public void UpdateAppointment_updateAppointment_appointmentPasswordOfResultNotSet()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = ExpectedAppointmentId, CustomerId = ExpectedCustomerId, Password = ExpectedPassword
            }
        );
        mockAppointmentRepo.Setup(r => r.GetAppointmentAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Guid("00000000-488A-4ECF-94E8-47387BB715D5")
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.UpdateAppointment(new Appointment
        {
            AppointmentId = ExpectedAppointmentId, CustomerId = ExpectedCustomerId, Password = ExpectedPassword
        });
        Assert.IsNotNull(result);
        Assert.IsNull(result.Password);
    }

    [TestMethod]
    public void UpdateAppointment_updateAppointment_hashPasswordInAppointmentAndStoreItInDb()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = ExpectedAppointmentId,
                CustomerId = ExpectedCustomerId,
                Password = ExpectedHashPassword
            }
        );
        mockAppointmentRepo.Setup(r => r.AddAndUpdateAppointment(It.IsAny<Appointment>()))
            .Callback<Appointment>(s => { Assert.AreEqual(ExpectedHashPassword, s.Password); });

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.UpdateAppointment(new Appointment
        {
            AppointmentId = ExpectedAppointmentId, CustomerId = ExpectedCustomerId, Password = ExpectedPassword
        });
        Assert.IsNotNull(result);
        mockAppointmentRepo.Verify(r => r.AddAndUpdateAppointment(It.IsAny<Appointment>()), Times.Once());
    }

    #endregion

    #region VerifyAppointmentPassword

    [TestMethod]
    public void VerifyAppointmentPassword_submittedPasswordCouldBeVerified_true()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentPassword(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(ExpectedHashPassword);
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(ExpectedPassword, ExpectedHashPassword)).Returns(true);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object, mockBcryptWrapper.Object);

        var result = sut.VerifyAppointmentPassword(ExpectedCustomerId, ExpectedAppointmentId, ExpectedPassword);
        Assert.IsTrue(result);
        mockBcryptWrapper.Verify(w => w.Verify(ExpectedPassword, ExpectedHashPassword), Times.Once);
    }

    #endregion

    #region VerifyAppointmentPasswordByAdminId

    [TestMethod]
    public void VerifyAppointmentPasswordByAdminId_submittedPasswordCouldBeVerified_true()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentPasswordByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(ExpectedHashPassword);

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(ExpectedPassword, ExpectedHashPassword)).Returns(true);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object, mockBcryptWrapper.Object);

        var result = sut.VerifyAppointmentPasswordByAdminId(ExpectedCustomerId, ExpectedAdminId, ExpectedPassword);
        Assert.IsTrue(result);
        mockBcryptWrapper.Verify(w => w.Verify(ExpectedPassword, ExpectedHashPassword), Times.Once);
    }

    #endregion

    #region ExistsAppointment

    [TestMethod]
    public void ExistsAppointment_appointmentExists_true()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        var mockCustomerRepo = new Mock<ICustomerRepository>();

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.ExistsAppointment(ExpectedCustomerId, ExpectedAppointmentId);
        Assert.IsTrue(result);
    }

    #endregion

    #region ExistsAppointmentIsStarted

    [TestMethod]
    public void ExistsAppointmentIsStarted_appointmentIsStarted_false()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentIsStarted(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(false);
        var mockCustomerRepo = new Mock<ICustomerRepository>();

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.ExistsAppointmentIsStarted(ExpectedCustomerId, ExpectedAppointmentId);
        Assert.IsFalse(result);
    }

    #endregion

    #region ExistsAppointmentByAdminId

    [TestMethod]
    public void ExistsAppointmentByAdminId_appointmentExists_true()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        var mockCustomerRepo = new Mock<ICustomerRepository>();

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.ExistsAppointmentByAdminId(ExpectedCustomerId, ExpectedAdminId);
        Assert.IsTrue(result);
    }

    #endregion

    #region ParticipantToDeleteAreValid

    [TestMethod]
    public void ParticipantToDeleteAreValid_participantGuidsAreValid_true()
    {
        Participant fakeParticipant = new()
        {
            ParticipantId = ExpectedParticipantId1,
            CustomerId = ExpectedCustomerId,
            AppointmentId = ExpectedAppointmentId
        };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.ParticipantToDeleteAreValid(fakeParticipant);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ParticipantToDeleteAreValid_participantGuidsAreNotValid_false()
    {
        Participant fakeParticipant = new() { CustomerId = ExpectedCustomerId, AppointmentId = ExpectedAppointmentId };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.ParticipantToDeleteAreValid(fakeParticipant);
        Assert.IsFalse(result);
    }

    #endregion

    #region ParticipantsAreValid

    [TestMethod]
    public void ParticipantsAreValid_participantsAreValid_true()
    {
        Participant fakeParticipant = new()
        {
            ParticipantId = ExpectedParticipantId1,
            CustomerId = ExpectedCustomerId,
            AppointmentId = ExpectedAppointmentId
        };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.ParticipantsAreValid(new List<Participant> { fakeParticipant });
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ParticipantsAreValid_participantsAreNull_false()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.ParticipantsAreValid(null);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ParticipantsAreValid_VotingsAreValid_true()
    {
        Participant fakeParticipant = new()
        {
            ParticipantId = ExpectedParticipantId1,
            CustomerId = ExpectedCustomerId,
            AppointmentId = ExpectedAppointmentId,
            Votings = new List<Voting>
            {
                new()
                {
                    AppointmentId = ExpectedAppointmentId,
                    CustomerId = ExpectedCustomerId,
                    ParticipantId = ExpectedParticipantId1,
                    VotingId = ExpectedVotingId,
                    StatusIdentifier = ExpectedVotingStatus.ToString()
                }
            }
        };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object);

        var result = sut.ParticipantsAreValid(new List<Participant> { fakeParticipant });
        Assert.IsTrue(result);
    }

    #endregion

    #region SetAppointmentStatusType

    [TestMethod]
    public void SetAppointmentStatusType_ChangeStatusType_StartedToPaused_Okay()
    {
        var password = string.Empty;
        var oldStatusType = AppointmentStatusType.Started;
        var newStatusType = AppointmentStatusType.Paused;

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockAppointmentRepo.Setup(r => r.GetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(oldStatusType.ToString());
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment { AdminId = ExpectedAdminId, Password = ExpectedPassword }
        );
        mockAppointmentRepo.Setup(r =>
            r.SetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()));

        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(password, ExpectedHashPassword)).Returns(true);

        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(r => r.IsAppointmentPasswordProtectedByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object, mockBcryptWrapper.Object);

        var appointment = sut.SetAppointmentStatusType(ExpectedCustomerId, ExpectedAdminId, newStatusType);
        Assert.IsFalse(appointment == null);
        Assert.AreEqual(ExpectedAdminId, appointment.AdminId);
    }

    [TestMethod]
    public void SetAppointmentStatusType_ChangeStatusType_PausedToStarted_Okay()
    {
        var password = string.Empty;
        var oldStatusType = AppointmentStatusType.Paused;
        var newStatusType = AppointmentStatusType.Started;

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockAppointmentRepo.Setup(r => r.GetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(oldStatusType.ToString());
        mockAppointmentRepo.Setup(r =>
            r.SetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()));
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment { AdminId = ExpectedAdminId, Password = ExpectedPassword }
        );

        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(password, ExpectedHashPassword)).Returns(true);

        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(r => r.IsAppointmentPasswordProtectedByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object, mockBcryptWrapper.Object);

        var appointment = sut.SetAppointmentStatusType(ExpectedCustomerId, ExpectedAdminId, newStatusType);
        Assert.IsFalse(appointment == null);
        Assert.AreEqual(ExpectedAdminId, appointment.AdminId);
    }

    [TestMethod]
    public void SetAppointmentStatusType_ChangeStatusType_DeletedToStarted_Exception()
    {
        var password = string.Empty;
        var oldStatusType = AppointmentStatusType.Deleted;
        var newStatusType = AppointmentStatusType.Started;

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockAppointmentRepo.Setup(r => r.GetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(oldStatusType.ToString());
        mockAppointmentRepo.Setup(r =>
            r.SetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()));
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment { AdminId = ExpectedAdminId, Password = ExpectedPassword }
        );

        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(password, ExpectedHashPassword)).Returns(true);

        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(r => r.IsAppointmentPasswordProtectedByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);

        var sut = CreateSut(mockAppointmentRepo.Object, mockCustomerRepo.Object, mockBcryptWrapper.Object);

        var appointmentObject = sut.SetAppointmentStatusType(ExpectedCustomerId, ExpectedAdminId, newStatusType);
        Assert.IsTrue(appointmentObject == null);
        Assert.IsFalse(appointmentObject != null);
    }

    #endregion

    private static AppointmentBusinessLayer CreateSut(
        [CanBeNull] IAppointmentRepository appointmentRepository = null,
        [CanBeNull] ICustomerRepository customerRepository = null,
        [CanBeNull] IBcryptWrapper bcryptWrapper = null)
    {
        var appointmentRepositoryToUse = appointmentRepository ?? new Mock<IAppointmentRepository>().Object;
        var customerRepositoryToUse = customerRepository ?? new Mock<ICustomerRepository>().Object;
        var mockBcryptWrapper = bcryptWrapper ?? CreateDefaultBcryptWrapper();
        var logger = new Mock<ILogger<AppointmentBusinessLayer>>();

        return new AppointmentBusinessLayer(
            appointmentRepositoryToUse,
            customerRepositoryToUse,
            mockBcryptWrapper,
            logger.Object);
    }

    private static IBcryptWrapper CreateDefaultBcryptWrapper()
    {
        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper
            .Setup(w => w.HashPassword(It.IsAny<string>()))
            .Returns(ExpectedHashPassword);

        return mockBcryptWrapper.Object;
    }

    private static List<Participant> CreateParticipants(int total)
    {
        var participant = new Participant { Name = "Dummy" };
        var participants = new List<Participant>();

        for (var i = 0; i < total; i++)
        {
            participants.Add(participant);
        }

        return participants;
    }

    private static List<Participant> CreateParticipantsWithId(int total, int modulo)
    {
        var participants = new List<Participant>();

        for (var i = 0; i < total; i++)
        {
            participants.Add(
                new Participant { ParticipantId = i % modulo == 1 ? Guid.NewGuid() : Guid.Empty, Name = "Dummy" }
            );
        }

        return participants;
    }

    private static List<SuggestedDate> CreateSuggestedDates(int total)
    {
        var suggestedDate = new SuggestedDate { StartDate = DateTime.Now };
        var suggestedDates = new List<SuggestedDate>();

        for (var i = 0; i < total; i++)
        {
            suggestedDates.Add(suggestedDate);
        }

        return suggestedDates;
    }

    private static List<SuggestedDate> CreateSuggestedDatesWithId(int total, int modulo)
    {
        var suggestedDates = new List<SuggestedDate>();

        for (var i = 0; i < total; i++)
        {
            suggestedDates.Add(
                new SuggestedDate
                {
                    SuggestedDateId = i % modulo == 1 ? Guid.NewGuid() : Guid.Empty, StartDate = DateTime.Now
                }
            );
        }

        return suggestedDates;
    }
}