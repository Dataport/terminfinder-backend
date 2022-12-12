using Microsoft.EntityFrameworkCore;
using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.Repository;

/// <summary>
/// Context for Entity Framework
/// </summary>
public class DataContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// EF-context
    /// </summary>
    public DataContext()
    { }

    /// <summary>
    /// EF-Context
    /// </summary>
    /// <param name="options"></param>
    /// <param name="loggerFactory"></param>
    public DataContext(DbContextOptions<DataContext> options, ILoggerFactory loggerFactory)
        : base(options)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseLoggerFactory(_loggerFactory);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppConfig>()
            .HasKey(c => c.Key)
            .HasName("appconfig_pkey");

        modelBuilder.Entity<Customer>(customer =>
        {
            customer.Property(p => p.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            customer
                    .HasKey(c => c.CustomerId)
                    .HasName("customer_pkey");
        });

        modelBuilder.Entity<Appointment>(appointment =>
        {
            appointment.Property(p => p.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            appointment
                .HasKey(c => c.AppointmentId)
                .HasName("appointment_pkey");

            appointment
                .HasIndex(x => x.AdminId)
                .HasDatabaseName("appointment_adminid_ix")
                .IsUnique();

            appointment.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .HasConstraintName("appointment_customerid_fkey")
                .OnDelete(DeleteBehavior.Restrict);

            appointment.HasIndex(x => x.CustomerId)
                .HasDatabaseName("appointment_customerid_ix");
        });

        modelBuilder.Entity<Participant>(participant =>
        {
            participant.Property(p => p.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            participant
                .HasKey(c => c.ParticipantId)
                .HasName("participant_pkey");

            participant
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .HasConstraintName("participant_customerid_fkey")
                .OnDelete(DeleteBehavior.Restrict);

            participant.HasOne(x => x.Appointment)
                .WithMany(a => a.Participants)
                .HasForeignKey(c => c.AppointmentId)
                .HasConstraintName("participant_appointmentid_fkey")
                .OnDelete(DeleteBehavior.Cascade);

            participant.HasIndex(x => x.AppointmentId)
                .HasDatabaseName("participant_appointmentid_ix");

            participant.HasIndex(x => x.CustomerId)
                .HasDatabaseName("participant_customerid_ix");
        });

        modelBuilder.Entity<SuggestedDate>(suggestedDate =>
        {
            suggestedDate.Property(p => p.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            suggestedDate
                .HasKey(c => c.SuggestedDateId)
                .HasName("suggesteddate_pkey");

            suggestedDate.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .HasConstraintName("suggesteddate_customerid_fkey")
                .OnDelete(DeleteBehavior.Restrict);

            suggestedDate.HasOne(x => x.Appointment)
                .WithMany(s => s.SuggestedDates)
                .HasForeignKey(x => x.AppointmentId)
                .HasConstraintName("suggesteddate_appointmentid_fkey")
                .OnDelete(DeleteBehavior.Cascade);

            suggestedDate.HasIndex(x => x.StartDate)
                .HasDatabaseName("suggesteddate_startdate_ix");

            suggestedDate.HasIndex(x => x.EndDate)
                .HasDatabaseName("suggesteddate_enddate_ix");

            suggestedDate.HasIndex(x => x.AppointmentId)
                .HasDatabaseName("suggesteddate_appointmentid_ix");

            suggestedDate.HasIndex(x => x.CustomerId)
                .HasDatabaseName("suggestedDate_customerid_ix");
        });

        modelBuilder.Entity<Voting>(voting =>
        {
            voting.Property(p => p.CreationDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            voting
                .HasKey(c => c.VotingId)
                .HasName("voting_pkey");

            voting.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .HasConstraintName("voting_customerid_fkey")
                .OnDelete(DeleteBehavior.Restrict);


            voting.HasOne(x => x.Appointment)
                .WithMany()
                .HasForeignKey(x => x.AppointmentId)
                .HasConstraintName("voting_appointmentid_fkey")
                .OnDelete(DeleteBehavior.Cascade);

            voting.HasOne(x => x.Participant)
                .WithMany(v => v.Votings)
                .HasForeignKey(x => x.ParticipantId)
                .HasConstraintName("voting_participantid_fkey")
                .OnDelete(DeleteBehavior.Cascade);

            voting.HasOne(x => x.SuggestedDate)
                .WithMany(v => v.Votings)
                .HasForeignKey(x => x.SuggestedDateId)
                .HasConstraintName("voting_suggesteddateid_fkey")
                .OnDelete(DeleteBehavior.Cascade);

            voting.HasIndex(x => x.AppointmentId)
                .HasDatabaseName("voting_appointmentid_ix");

            voting.HasIndex(x => x.ParticipantId)
                .HasDatabaseName("voting_participantid_ix");

            voting.HasIndex(x => x.CustomerId)
                .HasDatabaseName("voting_customerid_ix");

            voting.HasIndex(x => x.SuggestedDateId)
                .HasDatabaseName("voting_suggesteddateid_ix");
        });
    }

    /// <summary>
    /// Save all changes
    /// </summary>
    /// <returns></returns>
    public override int SaveChanges()
    {
        ChangeTracker.DetectChanges();
        return base.SaveChanges();
    }

    /// <summary>
    /// Set tracking behavior
    /// </summary>
    /// <param name="tracking"></param>
    public virtual void SetTracking(bool tracking)
    {
        ChangeTracker.QueryTrackingBehavior = tracking
            ? QueryTrackingBehavior.TrackAll
            : QueryTrackingBehavior.NoTracking;
    }

    /// <summary>
    /// Information about the application
    /// </summary>
    public virtual DbSet<AppConfig> AppConfig { get; set; }

    /// <summary>
    /// Customers
    /// </summary>
    public virtual DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// Appointments
    /// </summary>
    public virtual DbSet<Appointment> Appointments { get; set; }

    /// <summary>
    /// Suggested Date
    /// </summary>
    public virtual DbSet<SuggestedDate> SuggestedDates { get; set; }

    /// <summary>
    /// Votings
    /// </summary>
    public virtual DbSet<Voting> Votings { get; set; }

    /// <summary>
    /// Participants
    /// </summary>
    public virtual DbSet<Participant> Participants { get; set; }
}