using Dataport.Terminfinder.Common.Jobs.Configuration;
using Dataport.Terminfinder.WebAPI.Services.Configuration;

namespace Dataport.Terminfinder.WebAPI.Tests.Services.Configuration;

[TestClass]
public class DeleteAppointmentsConfigValidatorTests
{
    private static readonly Guid ExpectedCustomerId = Guid.Parse("682799BD-6FFA-4F1C-9842-7C101176D9DA");
    private const int ExpectedDeleteExpiredAppointmentsAfterDays = 7;

    [TestMethod]
    public void Validate_ValidDeleteAppointmentsConfig_ResultIsValid()
    {
        var config = CreateValidConfig();

        var sut = CreateSut();

        var result = sut.Validate(DeleteAppointmentsConfig.SettingsKey, config);
        ConfigValidatorTestUtils.CheckValidValidationResult(result);
    }

    [TestMethod]
    public void Validate_CustomerIdIsEmpty_ResultIsInvalid()
    {
        var config = CreateValidConfig();
        config.CustomerId = Guid.Empty;
        var expectedFailureMessage = $"Value '{config.CustomerId}' in property '{DeleteAppointmentsConfig.SettingsKey}." +
                                     $"{nameof(DeleteAppointmentsConfig.CustomerId)}' is an empty Guid.";

        var sut = CreateSut();

        var result = sut.Validate(DeleteAppointmentsConfig.SettingsKey, config);
        ConfigValidatorTestUtils.CheckInvalidValidationResult(result, expectedFailureMessage);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(366)]
    public void Validate_DeleteExpiredAppointmentsAfterDaysIsInvalid_ResultIsInvalid(int deleteDays)
    {
        var config = CreateValidConfig();
        config.DeleteExpiredAppointmentsAfterDays = deleteDays;
        var expectedFailureMessage =
            $"Value '{deleteDays}' in property '{DeleteAppointmentsConfig.SettingsKey}.{nameof(DeleteAppointmentsConfig.DeleteExpiredAppointmentsAfterDays)}' " +
            $"has to be between 0 and 365.";

        var sut = CreateSut();

        var result = sut.Validate(DeleteAppointmentsConfig.SettingsKey, config);
        ConfigValidatorTestUtils.CheckInvalidValidationResult(result, expectedFailureMessage);
    }

    private static DeleteAppointmentsConfigValidator CreateSut()
    {
        var logger = new Mock<ILogger<DeleteAppointmentsConfigValidator>>();
        return new DeleteAppointmentsConfigValidator(logger.Object);
    }

    private static DeleteAppointmentsConfig CreateValidConfig()
    {
        return new DeleteAppointmentsConfig
        {
            IsEnabled = true,
            CustomerId = ExpectedCustomerId,
            DeleteExpiredAppointmentsAfterDays = ExpectedDeleteExpiredAppointmentsAfterDays
        };
    }
}