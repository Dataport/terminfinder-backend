using Microsoft.Data.Sqlite;

namespace Dataport.Terminfinder.Repository.Tests;

[TestClass]
public class StatisticRepositoryTests
{
    private static readonly Guid ExpectedCustomerId = Guid.Parse("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly DateOnly ExpectedYearMonth = new(2026, 9, 1);
    private const int ExpectedInitialAppointmentCount = 2;
    private const int ExpectedInitialParticipantCount = 4;
    private const int ExpectedInitialVotingCount = 2;
    private const int ExpectedIncrementedAppointmentCount = ExpectedInitialAppointmentCount + 1;
    private const int ExpectedIncrementedParticipantCount = ExpectedInitialParticipantCount + 1;
    private const int ExpectedIncrementedVotingCount = ExpectedInitialVotingCount + 5;

    [TestMethod]
    public void IncrementAppointmentStatisticCount_WhenStatisticIsMissing_CreatesStatisticWithExpectedCount()
    {
        using var context = CreateContext();
        var sut = CreateSut(context);

        sut.IncrementAppointmentStatisticCount(ExpectedCustomerId);

        var statistic = context.AppointmentStatistics.Single();
        Assert.AreEqual(ExpectedCustomerId, statistic.CustomerId);
        Assert.AreEqual(ExpectedYearMonth, statistic.YearMonth);
        Assert.AreEqual(1, statistic.Count);
    }

    [TestMethod]
    public void IncrementAppointmentStatisticCount_WhenStatisticExists_IncreasesCountByOne()
    {
        using var context = CreateContext();
        AddAppointmentStatistic(context, ExpectedInitialAppointmentCount);
        var sut = CreateSut(context);

        sut.IncrementAppointmentStatisticCount(ExpectedCustomerId);

        var statistic = context.AppointmentStatistics.Single();
        Assert.AreEqual(ExpectedIncrementedAppointmentCount, statistic.Count);
    }

    [TestMethod]
    public void IncrementParticipantStatisticCount_WhenStatisticIsMissing_CreatesStatisticWithExpectedCount()
    {
        using var context = CreateContext();
        var sut = CreateSut(context);

        sut.IncrementParticipantStatisticCount(ExpectedCustomerId);

        var statistic = context.ParticipantStatistics.Single();
        Assert.AreEqual(ExpectedCustomerId, statistic.CustomerId);
        Assert.AreEqual(ExpectedYearMonth, statistic.YearMonth);
        Assert.AreEqual(1, statistic.Count);
    }

    [TestMethod]
    public void IncrementParticipantStatisticCount_WhenStatisticExists_IncreasesCountByOne()
    {
        using var context = CreateContext();
        AddParticipantStatistic(context, ExpectedInitialParticipantCount);
        var sut = CreateSut(context);

        sut.IncrementParticipantStatisticCount(ExpectedCustomerId);

        var statistic = context.ParticipantStatistics.Single();
        Assert.AreEqual(ExpectedIncrementedParticipantCount, statistic.Count);
    }

    [TestMethod]
    public void IncrementVotingStatisticCount_WhenStatisticIsMissing_CreatesStatisticWithExpectedCount()
    {
        using var context = CreateContext();
        var sut = CreateSut(context);

        sut.IncrementVotingStatisticCount(ExpectedCustomerId, 3);

        var statistic = context.VotingStatistics.Single();
        Assert.AreEqual(ExpectedCustomerId, statistic.CustomerId);
        Assert.AreEqual(ExpectedYearMonth, statistic.YearMonth);
        Assert.AreEqual(3, statistic.Count);
    }

    [TestMethod]
    public void IncrementVotingStatisticCount_WhenStatisticExists_IncreasesCountByProvidedValue()
    {
        using var context = CreateContext();
        AddVotingStatistic(context, ExpectedInitialVotingCount);
        var sut = CreateSut(context);

        sut.IncrementVotingStatisticCount(ExpectedCustomerId, 5);

        var statistic = context.VotingStatistics.Single();
        Assert.AreEqual(ExpectedIncrementedVotingCount, statistic.Count);
    }

    [TestMethod]
    public void IncrementVotingStatisticCount_WhenCountIsNotPositive_DoesNotCreateAnyExpectedStatistic()
    {
        using var context = CreateContext();
        var sut = CreateSut(context);

        sut.IncrementVotingStatisticCount(ExpectedCustomerId, 0);
        sut.IncrementVotingStatisticCount(ExpectedCustomerId, -1);

        Assert.AreEqual(0, context.VotingStatistics.Count());
    }

    private static StatisticRepository CreateSut(DataContext context)
    {
        return new StatisticRepository(context, Mock.Of<ILogger<StatisticRepository>>());
    }

    private static void AddAppointmentStatistic(DataContext context, int count)
    {
        context.AppointmentStatistics.Add(
            new AppointmentStatistic { CustomerId = ExpectedCustomerId, YearMonth = ExpectedYearMonth, Count = count }
        );
        context.SaveChanges();
        context.ChangeTracker.Clear();
    }

    private static void AddParticipantStatistic(DataContext context, int count)
    {
        context.ParticipantStatistics.Add(
            new ParticipantStatistic { CustomerId = ExpectedCustomerId, YearMonth = ExpectedYearMonth, Count = count }
        );
        context.SaveChanges();
        context.ChangeTracker.Clear();
    }

    private static void AddVotingStatistic(DataContext context, int count)
    {
        context.VotingStatistics.Add(
            new VotingStatistic { CustomerId = ExpectedCustomerId, YearMonth = ExpectedYearMonth, Count = count }
        );
        context.SaveChanges();
        context.ChangeTracker.Clear();
    }

    private static DataContext CreateContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .Options;

        var context = new DataContext(options, LoggerFactory.Create(_ => { }));
        context.Database.EnsureCreated();
        return context;
    }
}