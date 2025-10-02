using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.Console.DeleteAppointments.Tool.Tests;

[TestClass]
public class DeleteAppointmentsTests
{
    private ILogger<DeleteAppointmentsService> _logger;
    private IDateTimeGeneratorService _dateTimeGeneratorServiceFake;

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<DeleteAppointmentsService>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<DeleteAppointmentsService>>();

        _dateTimeGeneratorServiceFake = new DateTimeGeneratorServiceFake();
    }

    [TestMethod]
    public void DeleteExpiredAppointments_ConnectionStringIsEmpty_ApplicationException()
    {
        string connectionString = string.Empty;
        int deleteExpiredAppointmentsAfterDays = 7;
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        string expectedErrorMessage = $"The connectionstring are not defined.";

        var mockRepository = new Mock<IRepository>();
        //mockRepository.Setup(m => m.ExistsAppointmentByAdminId(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(false);

        var deleteExpiredAppointments = new DeleteAppointmentsService(mockRepository.Object, _logger);

        // Act
        try
        {
            deleteExpiredAppointments.DeleteExpiredAppointments(connectionString, customerId,
                deleteExpiredAppointmentsAfterDays, _dateTimeGeneratorServiceFake);
            Assert.Fail("An Exception should be thrown");
        }
        catch (ApplicationException ex)
        {
            // Assert
            Assert.AreEqual(expectedErrorMessage, ex.Message);
        }
        catch (Exception)
        {
            // Assert
            Assert.Fail("An ApplicationException should be thrown");
        }
    }

    [TestMethod]
    public void DeleteExpiredAppointments_deleteExpiredAppointmentsAfterDaysGreaterThanZeroApplicationException()
    {
        string connectionString = "connectionstring";
        int deleteExpiredAppointmentsAfterDays = 0;
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        string expectedErrorMessage =
            $"The configurationvalue of deleteExpiredAppointmentsAfterDays has to be greater than zero.";

        var mockRepository = new Mock<IRepository>();

        var deleteExpiredAppointments = new DeleteAppointmentsService(mockRepository.Object, _logger);

        // Act
        try
        {
            deleteExpiredAppointments.DeleteExpiredAppointments(connectionString, customerId,
                deleteExpiredAppointmentsAfterDays, _dateTimeGeneratorServiceFake);
            Assert.Fail("An Exception should be thrown");
        }
        catch (ApplicationException ex)
        {
            // Assert
            Assert.AreEqual(expectedErrorMessage, ex.Message);
        }
        catch (Exception)
        {
            // Assert
            Assert.Fail("An ApplicationException should be thrown");
        }
    }

    [TestMethod]
    public void DeleteExpiredAppointments_customerIdIsEmpty_ApplicationException()
    {
        string connectionString = "connectionstring";
        int deleteExpiredAppointmentsAfterDays = 7;
        Guid customerId = Guid.Empty;

        string expectedErrorMessage = $"The customerId is empty.";

        var mockRepository = new Mock<IRepository>();

        var deleteExpiredAppointments = new DeleteAppointmentsService(mockRepository.Object, _logger);

        // Act
        try
        {
            deleteExpiredAppointments.DeleteExpiredAppointments(connectionString, customerId,
                deleteExpiredAppointmentsAfterDays, _dateTimeGeneratorServiceFake);
            Assert.Fail("An Exception should be thrown");
        }
        catch (ApplicationException ex)
        {
            // Assert
            Assert.AreEqual(expectedErrorMessage, ex.Message);
        }
        catch (Exception)
        {
            // Assert
            Assert.Fail("An ApplicationException should be thrown");
        }
    }

    [TestMethod]
    public void DeleteExpiredAppointments_NoAppointmentsToDelete_true()
    {
        string connectionString = "connectionstring";
        int deleteExpiredAppointmentsAfterDays = 7;
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        List<Appointment> fakeAppointmentsToDelete = new();

        var mockRepository = new Mock<IRepository>();
        mockRepository.Setup(m =>
                m.GetListOfAppointmentsToDelete(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>()))
            .Returns(fakeAppointmentsToDelete);
        mockRepository.Setup(m =>
            m.DeleteAppointments(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<List<Appointment>>()));

        var deleteExpiredAppointments = new DeleteAppointmentsService(mockRepository.Object, _logger);

        // Act
        deleteExpiredAppointments.DeleteExpiredAppointments(connectionString, customerId,
            deleteExpiredAppointmentsAfterDays, _dateTimeGeneratorServiceFake);

        // Assert
        mockRepository.Verify(
            x => x.DeleteAppointments(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<List<Appointment>>()),
            Times.Never);
    }

    [TestMethod]
    public void DeleteExpiredAppointments_AppointmentsToDelete_true()
    {
        string connectionString = "connectionstring";
        int deleteExpiredAppointmentsAfterDays = 7;
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        List<Appointment> fakeAppointmentsToDelete = new();
        Appointment fakeAppointment = new()
        {
            AppointmentId = Guid.NewGuid(),
            CustomerId = customerId
        };
        fakeAppointmentsToDelete.Add(fakeAppointment);

        var mockRepository = new Mock<IRepository>();
        mockRepository.Setup(m =>
                m.GetListOfAppointmentsToDelete(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>()))
            .Returns(fakeAppointmentsToDelete);
        mockRepository.Setup(m =>
            m.DeleteAppointments(It.IsAny<string>(), It.IsAny<Guid>(), fakeAppointmentsToDelete));

        var deleteExpiredAppointments = new DeleteAppointmentsService(mockRepository.Object, _logger);
        deleteExpiredAppointments.DeleteExpiredAppointments(connectionString, customerId,
            deleteExpiredAppointmentsAfterDays, _dateTimeGeneratorServiceFake);
        mockRepository.Verify(
            x => x.GetListOfAppointmentsToDelete(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>()),
            Times.Once);
        mockRepository.Verify(
            x => x.DeleteAppointments(connectionString, It.Is<Guid>(y => y.ToString() == customerId.ToString()),
                fakeAppointmentsToDelete), Times.Once);

    }
}

[ExcludeFromCodeCoverage]
class DateTimeGeneratorServiceFake : IDateTimeGeneratorService
{
    public DateTime GetCurrentDateTime()
    {
        return new DateTime(2020, 02, 20, 12, 30, 00);
    }
}