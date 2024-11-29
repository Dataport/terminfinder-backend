using Dataport.Terminfinder.BusinessLayer.Security;
using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.BusinessLayer.Tests;

[TestClass]
[ExcludeFromCodeCoverage]
public class AppointmentBusinessLayerTests
{
    private ILogger<AppointmentBusinessLayer> _logger;
    private IBcryptWrapper _bcryptWrapper;

    private static readonly string ExpectedHashPassword =
        "$2b$10$bKHadGFqngTajUrRAozjxeS3r5Mz6.nQwOBjT.kUcBeIF7FFYVt2W";

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<AppointmentBusinessLayer>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<AppointmentBusinessLayer>>();
        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.HashPassword(It.IsAny<string>()))
            .Returns(ExpectedHashPassword);
        _bcryptWrapper = mockBcryptWrapper.Object;
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipantsAdd_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<Participant> participants = new List<Participant>();

        for (int i = 0; i < 20; i++)
        {
            Participant fakeParticipant = new()
            {
                Name = "Dummy"
            };

            participants.Add(fakeParticipant);
        }

        int fakeCountOfParticipantsInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfParticipants(expectedCustomerId, expectedAppointmentId, participants);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipantsAddAndModify_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<Participant> participants = new List<Participant>();

        for (int i = 0; i < 40; i++)
        {
            Participant fakeParticipant = new()
            {
                Name = "Dummy"
            };

            if (i % 2 == 1)
            {
                fakeParticipant.ParticipantId = Guid.NewGuid();
            }

            participants.Add(fakeParticipant);
        }

        int fakeCountOfParticipantsInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfParticipants(expectedCustomerId, expectedAppointmentId, participants);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipantsMaxElementsAddReached_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<Participant> participants = new List<Participant>();

        Participant fakeParticipant = new()
        {
            Name = "Dummy"
        };

        for (int i = 0; i < 100; i++)
        {
            participants.Add(fakeParticipant);
        }

        int fakeCountOfParticipantsInDatabase = 4900;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfParticipants(expectedCustomerId, expectedAppointmentId, participants);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipantsMaxElementsAddModifyReached_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<Participant> participants = new List<Participant>();

        for (int i = 0; i < 200; i++)
        {
            Participant fakeParticipant = new()
            {
                Name = "Dummy"
            };

            if (i % 2 == 1)
            {
                fakeParticipant.ParticipantId = Guid.NewGuid();
            }

            participants.Add(fakeParticipant);
        }

        int fakeCountOfParticipantsInDatabase = 4900;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfParticipants(expectedCustomerId, expectedAppointmentId, participants);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipantsAdd_False()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<Participant> participants = new List<Participant>();

        for (int i = 0; i < 101; i++)
        {
            Participant fakeParticipant = new()
            {
                Name = "Dummy"
            };

            participants.Add(fakeParticipant);
        }

        int fakeCountOfParticipantsInDatabase = 4900;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfParticipants(expectedCustomerId, expectedAppointmentId, participants);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfParticipantsAddModifyAndDelete_False()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<Participant> participants = new List<Participant>();

        for (int i = 0; i < 204; i++)
        {
            Participant fakeParticipant = new()
            {
                Name = "Dummy"
            };

            if (i % 2 == 1)
            {
                fakeParticipant.ParticipantId = Guid.NewGuid();
            }

            participants.Add(fakeParticipant);
        }

        int fakeCountOfParticipantsInDatabase = 4900;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfParticipants(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfParticipantsInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfParticipants(expectedCustomerId, expectedAppointmentId, participants);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDatesAdd_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        for (int i = 0; i < 20; i++)
        {
            SuggestedDate fakeSuggestedDate = new()
            {
                StartDate = DateTime.Now
            };

            suggestedDates.Add(fakeSuggestedDate);
        }

        int fakeCountOfSuggestedDatesInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfSuggestedDates(expectedCustomerId, expectedAppointmentId, suggestedDates);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDatesAddAndModify_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        for (int i = 0; i < 40; i++)
        {
            SuggestedDate fakeSuggestedDate = new()
            {
                StartDate = DateTime.Now
            };

            if (i % 2 == 1)
            {
                fakeSuggestedDate.SuggestedDateId = Guid.NewGuid();
            }

            suggestedDates.Add(fakeSuggestedDate);
        }

        int fakeCountOfSuggestedDatesInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfSuggestedDates(expectedCustomerId, expectedAppointmentId, suggestedDates);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDatesMaxElementsAddReached_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        for (int i = 0; i < 10; i++)
        {
            SuggestedDate fakeSuggestedDate = new()
            {
                StartDate = DateTime.Now
            };

            suggestedDates.Add(fakeSuggestedDate);
        }

        int fakeCountOfSuggestedDatesInDatabase = 90;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfSuggestedDates(expectedCustomerId, expectedAppointmentId, suggestedDates);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDatesMaxElementsAddAndModifyReached_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        for (int i = 0; i < 20; i++)
        {
            SuggestedDate fakeSuggestedDate = new()
            {
                StartDate = DateTime.Now
            };
            if (i % 2 == 1)
            {
                fakeSuggestedDate.SuggestedDateId = Guid.NewGuid();
            }

            suggestedDates.Add(fakeSuggestedDate);
        }

        int fakeCountOfSuggestedDatesInDatabase = 90;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfSuggestedDates(expectedCustomerId, expectedAppointmentId, suggestedDates);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDatesAdd_False()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        for (int i = 0; i < 15; i++)
        {
            SuggestedDate fakeSuggestedDate = new()
            {
                StartDate = DateTime.Now
            };

            suggestedDates.Add(fakeSuggestedDate);
        }

        int fakeCountOfSuggestedDatesInDatabase = 90;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfSuggestedDates(expectedCustomerId, expectedAppointmentId, suggestedDates);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void CheckMaxTotalCountOfSuggestedDatesAddAndModify_False()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        for (int i = 0; i < 22; i++)
        {
            SuggestedDate fakeSuggestedDate = new()
            {
                StartDate = DateTime.Now
            };
            if (i % 2 == 1)
            {
                fakeSuggestedDate.SuggestedDateId = Guid.NewGuid();
            }

            suggestedDates.Add(fakeSuggestedDate);
        }

        int fakeCountOfSuggestedDatesInDatabase = 90;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMaxTotalCountOfSuggestedDates(expectedCustomerId, expectedAppointmentId, suggestedDates);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void CheckMinTotalCountOfSuggestedDatesAdd_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        for (int i = 0; i < 20; i++)
        {
            SuggestedDate fakeSuggestedDate = new()
            {
                StartDate = DateTime.Now
            };

            suggestedDates.Add(fakeSuggestedDate);
        }

        int fakeCountOfSuggestedDatesInDatabase = 10;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMinTotalCountOfSuggestedDates(expectedCustomerId, expectedAppointmentId, suggestedDates);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void CheckMinTotalCountOfSuggestedDatesMinElementsAddNotReached_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        int fakeCountOfSuggestedDatesInDatabase = 0;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMinTotalCountOfSuggestedDates(expectedCustomerId, expectedAppointmentId, suggestedDates);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void CheckMinTotalCountOfSuggestedDatesDelete_False()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        for (int i = 0; i < 5; i++)
        {
            SuggestedDate fakeSuggestedDate = new()
            {
                StartDate = DateTime.Now,
                AppointmentId = expectedAppointmentId,
                CustomerId = expectedCustomerId,
                SuggestedDateId = Guid.NewGuid()
            };

            suggestedDates.Add(fakeSuggestedDate);
        }

        int fakeCountOfSuggestedDatesInDatabase = 5;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMinTotalCountOfSuggestedDatesWithToDeletedDates(expectedCustomerId,
                expectedAppointmentId, suggestedDates);

        Assert.AreEqual(false, result);
    }

    [TestMethod]
    public void CheckMinTotalCountOfSuggestedDatesDelete_True()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IList<SuggestedDate> suggestedDates = new List<SuggestedDate>();

        for (int i = 0; i < 4; i++)
        {
            SuggestedDate fakeSuggestedDate1 = new()
            {
                StartDate = DateTime.Now,
                AppointmentId = expectedAppointmentId,
                CustomerId = expectedCustomerId,
                SuggestedDateId = Guid.NewGuid()
            };

            suggestedDates.Add(fakeSuggestedDate1);
        }

        int fakeCountOfSuggestedDatesInDatabase = 5;

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockAppointmentRepo.Setup(bl => bl.GetNumberOfSuggestedDates(expectedCustomerId, expectedAppointmentId))
            .Returns(fakeCountOfSuggestedDatesInDatabase);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);
        bool result =
            businessLayer.CheckMinTotalCountOfSuggestedDatesWithToDeletedDates(expectedCustomerId,
                expectedAppointmentId, suggestedDates);

        Assert.AreEqual(true, result);
    }

    [TestMethod]
    public void SetAppointmentForeignKeys_Okay()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        Appointment fakeAppointment = new()
        {
            AppointmentId = expectedAppointmentId,
            CreatorName = "Tom",
            CustomerId = expectedCustomerId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started,
            SuggestedDates = new List<SuggestedDate>()
        };

        SuggestedDate fakeSuggestedDate1 = new()
        {
            StartDate = DateTime.Now
        };
        fakeAppointment.SuggestedDates.Add(fakeSuggestedDate1);

        SuggestedDate fakeSuggestedDate2 = new()
        {
            StartDate = DateTime.Now
        };
        fakeAppointment.SuggestedDates.Add(fakeSuggestedDate2);

        fakeSuggestedDate1.Votings = new List<Voting>() { new Voting() };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        businessLayer.SetAppointmentForeignKeys(fakeAppointment, expectedCustomerId);

        Assert.AreEqual(expectedCustomerId, fakeAppointment.CustomerId);
        foreach (SuggestedDate sd in fakeAppointment.SuggestedDates)
        {
            Assert.AreEqual(expectedCustomerId, sd.CustomerId);
            Assert.AreEqual(expectedAppointmentId, sd.AppointmentId);
        }
    }

    [TestMethod]
    public void SetParticipantsForeignKeys_Okay()
    {
        int numberOfElements = 2;

        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedParticipantId1 = new("CF1E5ABB-7D41-41AA-8DDC-0EB0319CD6B4");
        Guid expectedParticipantId2 = new("D1C2DB5B-3FD8-4D8B-9A64-0C90298897D0");

        List<Participant> fakeParticipants = new()
        {
            new Participant()
            {
                Name = "Joe",
                ParticipantId = expectedParticipantId1,
                Votings = new List<Voting>()
                {
                    new Voting
                    {
                        Status = VotingStatusType.Accepted
                    },
                    new Voting
                    {
                        Status = VotingStatusType.Accepted
                    }
                }
            },
            new Participant()
            {
                Name = "Cevin",
                ParticipantId = expectedParticipantId2,
                Votings = new List<Voting>()
                {
                    new Voting()
                    {
                        Status = VotingStatusType.Accepted
                    },
                    new Voting()
                    {
                        Status = VotingStatusType.Accepted
                    }
                }
            }

        };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        businessLayer.SetParticipantsForeignKeys(fakeParticipants, expectedCustomerId, expectedAppointmentId);

        foreach (Participant participant in fakeParticipants)
        {
            Assert.AreEqual(expectedCustomerId, participant.CustomerId);
            Assert.AreEqual(expectedAppointmentId, participant.AppointmentId);
            Guid expectedParticipantId = participant.ParticipantId;

            List<Voting> votings = participant.Votings.ToList();

            for (int i = 0; i < numberOfElements; i++)
            {
                Voting voting = votings[i];

                Assert.AreEqual(expectedCustomerId, voting.CustomerId);
                Assert.AreEqual(expectedAppointmentId, voting.AppointmentId);
                Assert.AreEqual(expectedParticipantId, voting.ParticipantId);
            }
        }
    }

    [TestMethod]
    public void GetAppointment_getExistingAppointment_appointmentPasswordNotSet()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = expectedAppointmentId,
                Password = "password"
            }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment result = businessLayer.GetAppointment(expectedCustomerId, expectedAppointmentId);
        Assert.IsNotNull(result);
        Assert.IsNull(result.Password);
    }

    [TestMethod]
    public void GetAppointment_NotExistingCustomerId_Null()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = expectedAppointmentId,
                Password = "password"
            }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(false);
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment result = businessLayer.GetAppointment(expectedCustomerId, expectedAppointmentId);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void IsAppointmentPasswordProtected_appointmentExistsAndHasAnyPassword_true()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentPassword(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns("Dummy");
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result = businessLayer.IsAppointmentPasswordProtected(expectedCustomerId, expectedAppointmentId);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_getExistingAppointment_appointmentPasswordNotSet()
    {
        Guid expectedAdminId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AdminId = expectedAdminId,
                Password = "password"
            }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment result = businessLayer.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId);
        Assert.IsNotNull(result);
        Assert.IsNull(result.Password);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_NotExistingCustomerId_Null()
    {
        Guid expectedAdminId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AdminId = expectedAdminId,
                Password = "password"
            }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(false);
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment result = businessLayer.GetAppointmentByAdminId(expectedCustomerId, expectedAdminId);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void IsAppointmentPasswordProtectedByAdminId_appointmentExistsAndHasAnyPassword_true()
    {
        Guid expectedAdminId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentPasswordByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns("Dummy");
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result = businessLayer.IsAppointmentPasswordProtectedByAdminId(expectedCustomerId, expectedAdminId);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void AddAppointment_createAppointment_appointmentPasswordOfResultNotSet()
    {
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = expectedAppointmentId,
                CustomerId = expectedCustomerId,
                Password = "password"
            }
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment result = businessLayer.AddAppointment(new Appointment
        {
            AppointmentId = expectedAppointmentId,
            CustomerId = expectedCustomerId,
            Password = "password"
        });
        Assert.IsNotNull(result);
        Assert.IsNull(result.Password);
    }

    [TestMethod]
    public void AddAppointment_createAppointment_hashPasswordInAppointmentAndStoreItInDb()
    {
        const string customerId = "BE1D657A-4D06-40DB-8443-D67BBB950EE7";
        const string appointmentId = "C1C2474B-488A-4ECF-94E8-47387BB715D5";

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = new(appointmentId),
                CustomerId = new(customerId),
                Password = ExpectedHashPassword
            }
        );
        mockAppointmentRepo.Setup(r => r.AddAndUpdateAppointment(It.IsAny<Appointment>()))
            .Callback<Appointment>(s => { Assert.AreEqual(ExpectedHashPassword, s.Password); });

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment result = businessLayer.AddAppointment(new Appointment
        {
            AppointmentId = new(appointmentId),
            CustomerId = new(customerId),
            Password = "password"
        });
        Assert.IsNotNull(result);
        mockAppointmentRepo.Verify(r => r.AddAndUpdateAppointment(It.IsAny<Appointment>()), Times.Once());
    }

    [TestMethod]
    public void UpdateAppointment_updateAppointment_appointmentPasswordOfResultNotSet()
    {
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = expectedAppointmentId,
                CustomerId = expectedCustomerId,
                Password = "password"
            }
        );
        mockAppointmentRepo.Setup(r => r.GetAppointmentAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Guid("00000000-488A-4ECF-94E8-47387BB715D5")
        );
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment result = businessLayer.UpdateAppointment(new Appointment
        {
            AppointmentId = expectedAppointmentId,
            CustomerId = expectedCustomerId,
            Password = "password"
        });
        Assert.IsNotNull(result);
        Assert.IsNull(result.Password);
    }

    [TestMethod]
    public void UpdateAppointment_updateAppointment_hashPasswordInAppointmentAndStoreItInDb()
    {
        const string customerId = "BE1D657A-4D06-40DB-8443-D67BBB950EE7";
        const string appointmentId = "C1C2474B-488A-4ECF-94E8-47387BB715D5";

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AppointmentId = new(appointmentId),
                CustomerId = new(customerId),
                Password = ExpectedHashPassword
            }
        );
        mockAppointmentRepo.Setup(r => r.AddAndUpdateAppointment(It.IsAny<Appointment>()))
            .Callback<Appointment>(s => { Assert.AreEqual(ExpectedHashPassword, s.Password); });

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(r => r.ExistsCustomer(It.IsAny<Guid>())).Returns(true);
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment result = businessLayer.UpdateAppointment(new Appointment
        {
            AppointmentId = new(appointmentId),
            CustomerId = new(customerId),
            Password = "password"
        });
        Assert.IsNotNull(result);
        mockAppointmentRepo.Verify(r => r.AddAndUpdateAppointment(It.IsAny<Appointment>()), Times.Once());
    }

    [TestMethod]
    public void VerifyAppointmentPassword_submittedPasswordCouldBeVerified_true()
    {
        const string expectedPassword = "password";
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentPassword(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(ExpectedHashPassword);
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(expectedPassword, ExpectedHashPassword)).Returns(true);
        _bcryptWrapper = mockBcryptWrapper.Object;

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            mockBcryptWrapper.Object, _logger);

        bool result =
            businessLayer.VerifyAppointmentPassword(expectedCustomerId, expectedAppointmentId, expectedPassword);
        Assert.IsTrue(result);
        mockBcryptWrapper.Verify(w => w.Verify(expectedPassword, ExpectedHashPassword), Times.Once);
    }

    [TestMethod]
    public void VerifyAppointmentPasswordByAdminId_submittedPasswordCouldBeVerified_true()
    {
        const string expectedPassword = "password";
        Guid expectedAdminId = new("FFF2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.GetAppointmentPasswordByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(ExpectedHashPassword);

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(expectedPassword, ExpectedHashPassword)).Returns(true);
        _bcryptWrapper = mockBcryptWrapper.Object;

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            mockBcryptWrapper.Object, _logger);

        bool result =
            businessLayer.VerifyAppointmentPasswordByAdminId(expectedCustomerId, expectedAdminId, expectedPassword);
        Assert.IsTrue(result);
        mockBcryptWrapper.Verify(w => w.Verify(expectedPassword, ExpectedHashPassword), Times.Once);
    }

    [TestMethod]
    public void ExistsAppointment_appointmentExists_true()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointment(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        var mockCustomerRepo = new Mock<ICustomerRepository>();

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result =
            businessLayer.ExistsAppointment(new(expectedCustomerId.ToString()), new(expectedAppointmentId.ToString()));
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistsAppointmentIsStarted_appointmentIsStarted_false()
    {
        Guid expectedAppointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentIsStarted(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(false);
        var mockCustomerRepo = new Mock<ICustomerRepository>();

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result = businessLayer.ExistsAppointmentIsStarted(new(expectedCustomerId.ToString()),
            new(expectedAppointmentId.ToString()));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ExistsAppointmentByAdminId_appointmentExists_true()
    {
        Guid expectedAdminId = new("FFF2474B-488A-4ECF-94E8-47387BB715D5");
        Guid expectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        var mockCustomerRepo = new Mock<ICustomerRepository>();

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result =
            businessLayer.ExistsAppointmentByAdminId(new(expectedCustomerId.ToString()),
                new(expectedAdminId.ToString()));
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ParticipantToDeleteAreValid_participantGuidsAreValid_true()
    {
        Guid fakeParticipantId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid fakeAppointmentId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid fakeCustomerId = new("6B2B0BCB-E0C5-4773-9104-C39BBEDF5888");

        Participant fakeParticipant = new()
        {
            ParticipantId = fakeParticipantId,
            CustomerId = fakeCustomerId,
            AppointmentId = fakeAppointmentId
        };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result = businessLayer.ParticipantToDeleteAreValid(fakeParticipant);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ParticipantToDeleteAreValid_participantGuidsAreNotValid_false()
    {
        Guid fakeAppointmentId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid fakeCustomerId = new("6B2B0BCB-E0C5-4773-9104-C39BBEDF5888");

        Participant fakeParticipant = new()
        {
            CustomerId = fakeCustomerId,
            AppointmentId = fakeAppointmentId
        };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result = businessLayer.ParticipantToDeleteAreValid(fakeParticipant);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ParticipantsAreValid_participantsAreValid_true()
    {
        Guid fakeParticipantId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid fakeAppointmentId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid fakeCustomerId = new("6B2B0BCB-E0C5-4773-9104-C39BBEDF5888");

        Participant fakeParticipant = new()
        {
            ParticipantId = fakeParticipantId,
            CustomerId = fakeCustomerId,
            AppointmentId = fakeAppointmentId
        };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result = businessLayer.ParticipantsAreValid(new List<Participant>() { fakeParticipant });
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ParticipantsAreValid_participantsAreNull_false()
    {
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result = businessLayer.ParticipantsAreValid(null);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ParticipantsAreValid_VotingsAreValid_true()
    {
        Guid fakeParticipantId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid fakeAppointmentId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid fakeCustomerId = new("6B2B0BCB-E0C5-4773-9104-C39BBEDF5888");
        Guid fakeVotingId = new("E0C6FD7D-814C-48E7-BAD3-C2932E494675");

        Participant fakeParticipant = new()
        {
            ParticipantId = fakeParticipantId,
            CustomerId = fakeCustomerId,
            AppointmentId = fakeAppointmentId,
            Votings = new List<Voting>()
            {
                new Voting()
                {
                    AppointmentId = fakeAppointmentId,
                    CustomerId = fakeCustomerId,
                    ParticipantId = fakeParticipantId,
                    VotingId = fakeVotingId,
                    StatusIdentifier = "Accepted"
                }
            }
        };

        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        bool result = businessLayer.ParticipantsAreValid(new List<Participant>() { fakeParticipant });
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Appointment_ChangeStatusType_StartedToPaused_Okay()
    {
        Guid fakeAdminId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid fakeCustomerId = new("6B2B0BCB-E0C5-4773-9104-C39BBEDF5888");
        string password = string.Empty;
        AppointmentStatusType oldStatusType = AppointmentStatusType.Started;
        AppointmentStatusType newStatusType = AppointmentStatusType.Paused;

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockAppointmentRepo.Setup(r => r.GetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(oldStatusType.ToString());
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AdminId = fakeAdminId,
                Password = "password"
            }
        );
        mockAppointmentRepo.Setup(r =>
            r.SetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()));

        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(password, ExpectedHashPassword)).Returns(true);
        _bcryptWrapper = mockBcryptWrapper.Object;

        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(r => r.IsAppointmentPasswordProtectedByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment appointment = businessLayer.SetAppointmentStatusType(fakeCustomerId, fakeAdminId, newStatusType);
        Assert.IsFalse(appointment == null);
        Assert.AreEqual(fakeAdminId, appointment.AdminId);
    }

    [TestMethod]
    public void Appointment_ChangeStatusType_PausedToStarted_Okay()
    {
        Guid fakeAdminId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid fakeCustomerId = new("6B2B0BCB-E0C5-4773-9104-C39BBEDF5888");
        string password = string.Empty;
        AppointmentStatusType oldStatusType = AppointmentStatusType.Paused;
        AppointmentStatusType newStatusType = AppointmentStatusType.Started;

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockAppointmentRepo.Setup(r => r.GetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(oldStatusType.ToString());
        mockAppointmentRepo.Setup(r =>
            r.SetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()));
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AdminId = fakeAdminId,
                Password = "password"
            }
        );

        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(password, ExpectedHashPassword)).Returns(true);
        _bcryptWrapper = mockBcryptWrapper.Object;

        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(r => r.IsAppointmentPasswordProtectedByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment appointment = businessLayer.SetAppointmentStatusType(fakeCustomerId, fakeAdminId, newStatusType);
        Assert.IsFalse(appointment == null);
        Assert.AreEqual(fakeAdminId, appointment.AdminId);
    }

    [TestMethod]
    public void Appointment_ChangeStatusType_DeletedToStarted_Exception()
    {
        Guid fakeAdminId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid fakeCustomerId = new("6B2B0BCB-E0C5-4773-9104-C39BBEDF5888");
        string password = string.Empty;
        AppointmentStatusType oldStatusType = AppointmentStatusType.Deleted;
        AppointmentStatusType newStatusType = AppointmentStatusType.Started;

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        var mockAppointmentRepo = new Mock<IAppointmentRepository>();
        mockAppointmentRepo.Setup(r => r.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
        mockAppointmentRepo.Setup(r => r.GetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Returns(oldStatusType.ToString());
        mockAppointmentRepo.Setup(r =>
            r.SetAppointmentStatusTypeByAdmin(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()));
        mockAppointmentRepo.Setup(r => r.GetAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(
            new Appointment
            {
                AdminId = fakeAdminId,
                Password = "password"
            }
        );

        var mockBcryptWrapper = new Mock<IBcryptWrapper>();
        mockBcryptWrapper.Setup(w => w.Verify(password, ExpectedHashPassword)).Returns(true);
        _bcryptWrapper = mockBcryptWrapper.Object;

        var mockAppointmentBusinessLayer = new Mock<IAppointmentBusinessLayer>();
        mockAppointmentBusinessLayer
            .Setup(r => r.IsAppointmentPasswordProtectedByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);

        var businessLayer = new AppointmentBusinessLayer(mockAppointmentRepo.Object, mockCustomerRepo.Object,
            _bcryptWrapper, _logger);

        Appointment appointmentObject =
            businessLayer.SetAppointmentStatusType(fakeCustomerId, fakeAdminId, newStatusType);
        Assert.IsTrue(appointmentObject == null);
        Assert.IsFalse(appointmentObject != null);
    }
}