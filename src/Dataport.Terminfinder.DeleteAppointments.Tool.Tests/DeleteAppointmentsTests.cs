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
        var connectionString = string.Empty;
        var deleteExpiredAppointmentsAfterDays = 7;
        var customerId = new Guid("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var expectedErrorMessage = $"The connectionstring are not defined.";

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
        var connectionString = "connectionstring";
        var deleteExpiredAppointmentsAfterDays = 0;
        var customerId = new Guid("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var expectedErrorMessage =
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
        var connectionString = "connectionstring";
        var deleteExpiredAppointmentsAfterDays = 7;
        var customerId = Guid.Empty;

        var expectedErrorMessage = $"The customerId is empty.";

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
        var connectionString = "connectionstring";
        var deleteExpiredAppointmentsAfterDays = 7;
        var customerId = new Guid("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var fakeAppointmentsToDelete = new List<Appointment>();

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
        var connectionString = "connectionstring";
        var deleteExpiredAppointmentsAfterDays = 7;
        var customerId = new Guid("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        var fakeAppointmentsToDelete = new List<Appointment>();
        var fakeAppointment = new Appointment
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