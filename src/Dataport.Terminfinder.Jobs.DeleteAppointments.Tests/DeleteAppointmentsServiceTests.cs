using Dataport.Terminfinder.Common.Jobs.Configuration;
using Dataport.Terminfinder.Common.Services;
using Dataport.Terminfinder.Jobs.DeleteAppointments;
using Dataport.Terminfinder.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class DeleteAppointmentsServiceTests
{
    private static readonly Guid ExpectedCustomerId = Guid.Parse("F60473C7-1302-4717-9887-4A5BC0D72CF4");
    private static readonly DateTime ExpectedNowUtc = new(2026, 06, 15, 12, 00, 00, DateTimeKind.Utc);
    private const int ExpectedDeleteExpiredAppointmentsAfterDays = 10;
    private static readonly Guid ExpectedAppointmentId1 = Guid.Parse("D4C06F9D-EB5B-49FD-8BAC-3F42D119ED36");

    private static readonly DateTime ExpectedDeleteDate =
        ExpectedNowUtc.Subtract(TimeSpan.FromDays(ExpectedDeleteExpiredAppointmentsAfterDays));


    [TestMethod]
    public void DeleteAppointments_CallExpectedMethods()
    {
        // TODO hier gehts weiter!
        var sut = CreateSut();
    }

    private static DeleteAppointmentsService CreateSut(IAppointmentRepository? mockAppointmentRepository = null)
    {
        var mockOptions = new Mock<IOptions<DeleteAppointmentsConfig>>();
        mockOptions
            .Setup(m => m.Value)
            .Returns(new DeleteAppointmentsConfig
            {
                CustomerId = ExpectedCustomerId,
                DeleteExpiredAppointmentsAfterDays = ExpectedDeleteExpiredAppointmentsAfterDays,
                IsEnabled = true
            });

        var mockDateTimeGeneratorService = new Mock<IDateTimeGeneratorService>();
        mockDateTimeGeneratorService
            .Setup(s => s.GetCurrentDateTime())
            .Returns(ExpectedNowUtc);

        var mockAppointmentRepositoryToUse =
            mockAppointmentRepository ?? CreateDefaultAppointmentRepository([ExpectedAppointmentId1]);

        var logger = new Mock<ILogger<DeleteAppointmentsService>>();

        return new DeleteAppointmentsService(
            mockOptions.Object,
            mockDateTimeGeneratorService.Object,
            mockAppointmentRepositoryToUse,
            logger.Object);
    }

    private static IAppointmentRepository CreateDefaultAppointmentRepository(List<Guid> guidsToReturn)
    {
        var mockAppointmentRepository = new Mock<IAppointmentRepository>();
        mockAppointmentRepository
            .Setup(ar => ar.GetAppointmentIdsToDelete(ExpectedCustomerId, ExpectedDeleteDate))
            .Returns(guidsToReturn);

        return mockAppointmentRepository.Object;
    }
}