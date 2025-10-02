namespace Dataport.Terminfinder.Repository.Tests.Utils;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<AppConfig> AppConfig { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<SuggestedDate> SuggestedDates { get; set; } = null!;
    public DbSet<Voting> Votings { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;
}