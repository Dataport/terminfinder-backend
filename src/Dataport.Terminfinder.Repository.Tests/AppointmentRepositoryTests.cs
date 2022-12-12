namespace Dataport.Terminfinder.Repository.Tests;

[TestClass]
[ExcludeFromCodeCoverage]
public class AppointmentRepositoryTests
{
    private ILogger<AppointmentRepository> _logger;

    [TestInitialize]
    public void Inilialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<AppointmentRepository>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<AppointmentRepository>>();
    }

    [TestMethod]
    public void GetAppointment_StatusStarted_True()
    {

        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string creatorname = "userXY";
        string description = "des";
        string place = "berlin";
        string subject = "new appointment";
        var status = AppointmentStatusType.Started;

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                CustomerId = customerId,
                CreatorName = creatorname,
                Description = description,
                Place = place,
                Subject = subject,
                AppointmentStatus = status
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        IQueryable<Voting> votings = new List<Voting>().AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockVotingSet = new Mock<DbSet<Voting>>();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.Votings).Returns(mockVotingSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Appointment appointment = repository.GetAppointment(customerId, appointmentId);

        // Assert
        Assert.IsNotNull(appointment);
        Assert.AreEqual(appointment.Description, description);
        Assert.AreEqual(appointment.Place, place);
        Assert.AreEqual(appointment.Subject, subject);
        Assert.AreEqual(appointment.CreatorName, creatorname);
        Assert.AreEqual(appointment.AppointmentStatus, status);
    }

    [TestMethod]
    public void GetAppointment_StatusPaused_True()
    {

        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string creatorname = "userXY";
        string description = "des";
        string place = "berlin";
        string subject = "new appointment";
        var status = AppointmentStatusType.Paused;

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                CustomerId = customerId,
                CreatorName = creatorname,
                Description = description,
                Place = place,
                Subject = subject,
                AppointmentStatus = status
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        IQueryable<Voting> votings = new List<Voting>().AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockVotingSet = new Mock<DbSet<Voting>>();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.Votings).Returns(mockVotingSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Appointment appointment = repository.GetAppointment(customerId, appointmentId);

        // Assert
        Assert.IsNotNull(appointment);
        Assert.AreEqual(appointment.Description, description);
        Assert.AreEqual(appointment.Place, place);
        Assert.AreEqual(appointment.Subject, subject);
        Assert.AreEqual(appointment.CreatorName, creatorname);
        Assert.AreEqual(appointment.AppointmentStatus, status);
    }

    [TestMethod]
    public void GetAppointment_StatusDelete_NotFound()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string creatorname = "userXY";
        string description = "des";
        string place = "berlin";
        string subject = "new appointment";
        var status = AppointmentStatusType.Deleted;

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                CustomerId = customerId,
                CreatorName = creatorname,
                Description = description,
                Place = place,
                Subject = subject,
                AppointmentStatus = status
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        IQueryable<Voting> votings = new List<Voting>().AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockVotingSet = new Mock<DbSet<Voting>>();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.Votings).Returns(mockVotingSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Appointment appointment = repository.GetAppointment(customerId, appointmentId);

        // Assert
        Assert.IsNull(appointment);
    }

    [TestMethod]
    public void GetAppointment_NotFound()
    {

        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string creatorname = "userXY";
        string description = "des";
        string place = "berlin";
        string subject = "new appointment";
        var status = AppointmentStatusType.Started;

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                CustomerId = customerId,
                CreatorName = creatorname,
                Description = description,
                Place = place,
                Subject = subject,
                AppointmentStatus = status
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        IQueryable<Voting> votings = new List<Voting>().AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockVotingSet = new Mock<DbSet<Voting>>();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.Votings).Returns(mockVotingSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Appointment appointment =
            repository.GetAppointment(customerId, new("719A2E53-D753-4C50-8C48-88592EF3F0E1"));

        // Assert
        Assert.IsNull(appointment);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_StatusStarted_Okay()
    {

        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string creatorname = "userXY";
        string description = "des";
        string place = "berlin";
        string subject = "new appointment";
        var status = AppointmentStatusType.Started;

        IQueryable<Appointment> appointments = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                AdminId = adminId,
                CustomerId = customerId,
                CreatorName = creatorname,
                Description = description,
                Place = place,
                Subject = subject,
                AppointmentStatus = status
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(appointments.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(appointments.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(appointments.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(appointments.GetEnumerator());

        IQueryable<Voting> votings = new List<Voting>().AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockVotingSet = new Mock<DbSet<Voting>>();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.Votings).Returns(mockVotingSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Appointment appointment = repository.GetAppointmentByAdminId(customerId, adminId);

        // Assert
        Assert.IsNotNull(appointment);
        Assert.AreEqual(appointment.Description, description);
        Assert.AreEqual(appointment.Place, place);
        Assert.AreEqual(appointment.Subject, subject);
        Assert.AreEqual(appointment.CreatorName, creatorname);
        Assert.AreEqual(appointment.AppointmentStatus, status);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_StatusPaused_Okay()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string creatorname = "userXY";
        string description = "des";
        string place = "berlin";
        string subject = "new appointment";
        var status = AppointmentStatusType.Paused;

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                AdminId = adminId,
                CustomerId = customerId,
                CreatorName = creatorname,
                Description = description,
                Place = place,
                Subject = subject,
                AppointmentStatus = status
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        IQueryable<Voting> votings = new List<Voting>().AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockVotingSet = new Mock<DbSet<Voting>>();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.Votings).Returns(mockVotingSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Appointment appointment = repository.GetAppointmentByAdminId(customerId, adminId);

        // Assert
        Assert.IsNotNull(appointment);
        Assert.AreEqual(appointment.Description, description);
        Assert.AreEqual(appointment.Place, place);
        Assert.AreEqual(appointment.Subject, subject);
        Assert.AreEqual(appointment.CreatorName, creatorname);
        Assert.AreEqual(appointment.AppointmentStatus, status);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_StatusDeleted_NotFound()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string creatorname = "userXY";
        string description = "des";
        string place = "berlin";
        string subject = "new appointment";
        var status = AppointmentStatusType.Deleted;

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                AdminId = adminId,
                CustomerId = customerId,
                CreatorName = creatorname,
                Description = description,
                Place = place,
                Subject = subject,
                AppointmentStatus = status
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        IQueryable<Voting> votings = new List<Voting>().AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockVotingSet = new Mock<DbSet<Voting>>();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.Votings).Returns(mockVotingSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Appointment appointment = repository.GetAppointmentByAdminId(customerId, adminId);

        // Assert
        Assert.IsNull(appointment);
    }

    [TestMethod]
    public void GetAppointmentByAdminId_NotFound()
    {

        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string creatorname = "userXY";
        string description = "des";
        string place = "berlin";
        string subject = "new appointment";
        var status = AppointmentStatusType.Started;

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                AdminId = adminId,
                CustomerId = customerId,
                CreatorName = creatorname,
                Description = description,
                Place = place,
                Subject = subject,
                AppointmentStatus = status
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        IQueryable<Voting> votings = new List<Voting>().AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockVotingSet = new Mock<DbSet<Voting>>();
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockVotingSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.Votings).Returns(mockVotingSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Appointment appointment =
            repository.GetAppointmentByAdminId(customerId, new("719A2E53-D753-4C50-8C48-88592EF3F0E1"));

        // Assert
        Assert.IsNull(appointment);
    }

    [TestMethod]
    public void ExistsAppointment_appointmentExists_StatusStarted_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists = repository.ExistsAppointment(new(customerId.ToString()),
            new(appointmentId.ToString()));

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_appointmentExists_StatusPaused_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Paused
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists = repository.ExistsAppointment(new(customerId.ToString()),
            new(appointmentId.ToString()));

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_appointmentExists_StatusDeleted_false()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Deleted
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists = repository.ExistsAppointment(new(customerId.ToString()),
            new(appointmentId.ToString()));

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_appointmentNotExists_false()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists = repository.ExistsAppointment(new(customerId.ToString()),
            new(), new(adminId.ToString()));

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AdminIdAreEqual_appointmentExists_StatusStarted_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists = repository.ExistsAppointment(new(customerId.ToString()),
            new(appointmentId.ToString()), new(adminId.ToString()));

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AdminIdAreNotEqual()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists = repository.ExistsAppointment(new(customerId.ToString()),
            new(appointmentId.ToString()), new());

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AdminIdAreNotEqual_appointmentExists_StatusDeleted_false()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Deleted
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists = repository.ExistsAppointment(new(customerId.ToString()),
            new(appointmentId.ToString()), new(adminId.ToString()));

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointment_AdminIdAreNotEqual_appointmentNotExists_false()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists = repository.ExistsAppointment(new(customerId.ToString()),
            new(), new(adminId.ToString()));

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointmentIsStarted_appointmentExistsAndStatusIsStarted_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExistsAndIsStarted =
            repository.ExistsAppointmentIsStarted(new(customerId.ToString()),
                new(appointmentId.ToString()));

        // Assert
        Assert.IsTrue(appointmentExistsAndIsStarted);
    }

    [TestMethod]
    public void ExistsAppointmentIsStarted_appointmentExistsAndStatusIsStarted_false()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Paused
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExistsAndIsStarted =
            repository.ExistsAppointmentIsStarted(new(customerId.ToString()),
                new(appointmentId.ToString()));

        // Assert
        Assert.IsFalse(appointmentExistsAndIsStarted);
    }

    [TestMethod]
    public void ExistsAppointmentByAdminId_appointmentExists_StatusStarted_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists =
            repository.ExistsAppointmentByAdminId(new(customerId.ToString()),
                new(adminId.ToString()));

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointmentByAdminId_appointmentExists_StatusPaused_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Paused
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists =
            repository.ExistsAppointmentByAdminId(new(customerId.ToString()),
                new(adminId.ToString()));

        // Assert
        Assert.IsTrue(appointmentExists);
    }

    [TestMethod]
    public void ExistsAppointmentByAdminId_appointmentExists_StatusDeleted_false()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Deleted
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool appointmentExists =
            repository.ExistsAppointmentByAdminId(new(customerId.ToString()),
                new(adminId.ToString()));

        // Assert
        Assert.IsFalse(appointmentExists);
    }

    [TestMethod]
    public void GetAppointmentPassword_appointmentExistsAndHasPassword_returnsTheAppointmentPassword()
    {
        const string password = "P4$$w0rd";
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started,
                Password = password

            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        string appointmentPassword = repository.GetAppointmentPassword(
            new(customerId.ToString()), new(appointmentId.ToString()));

        // Assert
        Assert.AreEqual(password, appointmentPassword);
    }

    [TestMethod]
    public void GetAppointmentPassword_appointmentNotExists_throwException()
    {
        const string password = "P4$$w0rd";
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid notExistingAppointmentId = new("FFF2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started,
                Password = password

            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        try
        {
            using AppointmentRepository repository = new(mockContext.Object, _logger);
            repository.GetAppointmentPassword(new(customerId.ToString()),
                new(notExistingAppointmentId.ToString()));
            Assert.Fail("An exception should be thrown");
        }
        catch (InvalidOperationException)
        {
        }
    }

    [TestMethod]
    public void GetAppointmentPasswordByAdmin_appointmentExistsAndHasPassword_returnsTheAppointmentPassword()
    {
        const string password = "P4$$w0rd";
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started,
                Password = password

            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        string appointmentPassword = repository.GetAppointmentPasswordByAdmin(
            new(customerId.ToString()), new(adminId.ToString()));

        // Assert
        Assert.AreEqual(password, appointmentPassword);
    }

    [TestMethod]
    public void GetAppointmentPasswordByAdmin_appointmentNotExists_throwException()
    {
        const string password = "P4$$w0rd";
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid notExistingAdminId = new("FFF2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started,
                Password = password

            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        try
        {
            repository.GetAppointmentPasswordByAdmin(new(customerId.ToString()),
                new(notExistingAdminId.ToString()));
            Assert.Fail("An exception should be thrown");
        }
        catch (InvalidOperationException)
        {
        }
    }

    [TestMethod]
    public void GetAppointmentStatusTypeByAdmin_AppointmentStatusStarted_True()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        string statusIdentifier = repository.GetAppointmentStatusTypeByAdmin(customerId, adminId);
        // Assert
        Assert.AreEqual(statusIdentifier, AppointmentStatusType.Started.ToString());
    }

    [TestMethod]
    public void GetAppointmentStatusTypeByAdmin_AppointmentStatusPaused_True()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Paused
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        string statusIdentifier = repository.GetAppointmentStatusTypeByAdmin(customerId, adminId);
        // Assert
        Assert.AreEqual(statusIdentifier, AppointmentStatusType.Paused.ToString());
    }

    [TestMethod]
    public void GetAppointmentStatusTypeByAdmin_AppointmentStatusDeleted_False()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> customers = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = new(appointmentId.ToString()),
                AdminId = new(adminId.ToString()),
                CustomerId = new(customerId.ToString()),
                AppointmentStatus = AppointmentStatusType.Deleted
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        try
        {
            // act fetch
            using AppointmentRepository repository = new(mockContext.Object, _logger);
            repository.GetAppointmentStatusTypeByAdmin(customerId, adminId);
            Assert.Fail("An exception should be thrown");
        }
        catch (InvalidOperationException)
        {
        }
    }

    [TestMethod]
    public void GetAppointmentAdminId_Guid()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> appointments = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                AdminId = adminId,
                CustomerId = customerId,
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(appointments.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(appointments.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(appointments.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(appointments.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Guid returnValueAsAdminId = repository.GetAppointmentAdminId(customerId, appointmentId);
        // Assert
        Assert.AreEqual(returnValueAsAdminId, adminId);
    }

    [TestMethod]
    public void GetAppointmentAdminId_CustomerIdIsEmpty_GuidEmpty()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = Guid.Empty;

        IQueryable<Appointment> appointments = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                AdminId = adminId,
                CustomerId = customerId,
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(appointments.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(appointments.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(appointments.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(appointments.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Guid returnValueAsAdminId = repository.GetAppointmentAdminId(customerId, appointmentId);
        // Assert
        Assert.AreEqual(returnValueAsAdminId, Guid.Empty);
    }

    [TestMethod]
    public void GetAppointmentAdminId_AppointmentIdIsEmpty_GuidEmpty()
    {
        Guid appointmentId = Guid.Empty;
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> appointments = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                AdminId = adminId,
                CustomerId = customerId,
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(appointments.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(appointments.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(appointments.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(appointments.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Guid returnValueAsAdminId = repository.GetAppointmentAdminId(customerId, appointmentId);
        // Assert
        Assert.AreEqual(returnValueAsAdminId, Guid.Empty);
    }

    [TestMethod]
    public void GetAppointmentAdminId_AppointmentIdNotFound_GuidEmpty()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid appointmentIdToSearch = new("21E50876-25D5-492A-AAA2-2A6666BD5D7B");
        Guid adminId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Appointment> appointments = new List<Appointment>
        {
            new Appointment
            {
                AppointmentId = appointmentId,
                AdminId = adminId,
                CustomerId = customerId,
                AppointmentStatus = AppointmentStatusType.Started
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Appointment>>();
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Provider).Returns(appointments.Provider);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.Expression).Returns(appointments.Expression);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.ElementType).Returns(appointments.ElementType);
        mockSet.As<IQueryable<Appointment>>().Setup(m => m.GetEnumerator()).Returns(appointments.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Appointments).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        Guid returnValueAsAdminId = repository.GetAppointmentAdminId(customerId, appointmentIdToSearch);
        // Assert
        Assert.AreEqual(returnValueAsAdminId, Guid.Empty);
    }

    [TestMethod]
    public void GetParticipants_Participants()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid participantId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Participant> participants = new List<Participant>
        {
            new Participant
            {
                AppointmentId = appointmentId,
                ParticipantId = participantId,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Participant>>();
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participants.Provider);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participants.ElementType);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(participants.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Participants).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        ICollection<Participant> allParticipants = repository.GetParticipants(customerId, appointmentId);
        // Assert
        Assert.AreNotEqual(allParticipants, null);
        Assert.AreEqual(allParticipants?.Count, participants.Count());
        Assert.AreEqual(allParticipants?.First().ParticipantId, participantId);
    }

    [TestMethod]
    public void GetParticipants_AppointmentNotFound_EmptyCollection()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid appointmentIdToSearch = new("21E50876-25D5-492A-AAA2-2A6666BD5D7B");
        Guid participantId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Participant> participants = new List<Participant>
        {
            new Participant
            {
                AppointmentId = appointmentId,
                ParticipantId = participantId,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Participant>>();
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participants.Provider);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participants.ElementType);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(participants.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Participants).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        ICollection<Participant> allParticipants =
            repository.GetParticipants(customerId, appointmentIdToSearch);
        // Assert
        Assert.AreNotEqual(null, allParticipants);
        Assert.AreEqual(0, allParticipants?.Count);
    }

    [TestMethod]
    public void GetParticipants_AppointmentIdIsNull_Null()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid appointmentIdToSearch = Guid.Empty;
        Guid participantId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Participant> participants = new List<Participant>
        {
            new Participant
            {
                AppointmentId = appointmentId,
                ParticipantId = participantId,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Participant>>();
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participants.Provider);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participants.ElementType);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(participants.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Participants).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        ICollection<Participant> allParticipants =
            repository.GetParticipants(customerId, appointmentIdToSearch);
        // Assert
        Assert.AreEqual(null, allParticipants);
    }

    [TestMethod]
    public void GetParticipants_CustomerIdIsNull_Null()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid customerIdToSearch = Guid.Empty;
        Guid participantId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Participant> participants = new List<Participant>
        {
            new Participant
            {
                AppointmentId = appointmentId,
                ParticipantId = participantId,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Participant>>();
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participants.Provider);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participants.ElementType);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(participants.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Participants).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        ICollection<Participant> allParticipants =
            repository.GetParticipants(customerIdToSearch, appointmentId);
        // Assert
        Assert.AreEqual(null, allParticipants);
    }

    [TestMethod]
    public void ExistsParticipant_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid participantId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Participant> participants = new List<Participant>
        {
            new Participant
            {
                AppointmentId = appointmentId,
                ParticipantId = participantId,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Participant>>();
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participants.Provider);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participants.ElementType);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(participants.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Participants).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool isExistsParticipant = repository.ExistsParticipant(customerId, appointmentId, participantId);
        // Assert
        Assert.AreEqual(true, isExistsParticipant);
    }

    [TestMethod]
    public void ExistsParticipant_ParticipantNotFound_false()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid participantIdToSearch = new("21E50876-25D5-492A-AAA2-2A6666BD5D7B");
        Guid participantId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Participant> participants = new List<Participant>
        {
            new Participant
            {
                AppointmentId = appointmentId,
                ParticipantId = participantId,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Participant>>();
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participants.Provider);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participants.ElementType);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(participants.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Participants).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool isExistsParticipant =
            repository.ExistsParticipant(customerId, appointmentId, participantIdToSearch);
        // Assert
        Assert.AreEqual(false, isExistsParticipant);
    }

    [TestMethod]
    public void ExistsSuggestedDate_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid suggestedDateId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<SuggestedDate> suggestedDates = new List<SuggestedDate>
        {
            new SuggestedDate
            {
                AppointmentId = appointmentId,
                SuggestedDateId = suggestedDateId,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<SuggestedDate>>();
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.Provider).Returns(suggestedDates.Provider);
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.Expression).Returns(suggestedDates.Expression);
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.ElementType).Returns(suggestedDates.ElementType);
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.GetEnumerator()).Returns(suggestedDates.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.SuggestedDates).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool isExistsSuggestedDate = repository.ExistsSuggestedDate(customerId, appointmentId, suggestedDateId);
        // Assert
        Assert.AreEqual(true, isExistsSuggestedDate);
    }

    [TestMethod]
    public void ExistsSuggestedDate_NotFound_false()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid suggestedDateId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid suggestedDateIdToSearch = new("21E50876-25D5-492A-AAA2-2A6666BD5D7B");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<SuggestedDate> suggestedDates = new List<SuggestedDate>
        {
            new SuggestedDate
            {
                AppointmentId = appointmentId,
                SuggestedDateId = suggestedDateId,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<SuggestedDate>>();
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.Provider).Returns(suggestedDates.Provider);
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.Expression).Returns(suggestedDates.Expression);
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.ElementType).Returns(suggestedDates.ElementType);
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.GetEnumerator()).Returns(suggestedDates.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.SuggestedDates).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool isExistsSuggestedDate =
            repository.ExistsSuggestedDate(customerId, appointmentId, suggestedDateIdToSearch);
        // Assert
        Assert.AreEqual(false, isExistsSuggestedDate);
    }

    [TestMethod]
    public void ExistsVoting_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid suggestedDateId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid participantId = new("A365D57E-D78E-46AF-B127-8067A18F6B81");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid votingId = new("B198FD02-2C48-4932-AC34-A6878C65DC36");

        IQueryable<Voting> votings = new List<Voting>
        {
            new Voting
            {
                AppointmentId = appointmentId,
                SuggestedDateId = suggestedDateId,
                CustomerId = customerId,
                ParticipantId = participantId,
                VotingId = votingId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Voting>>();
        mockSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Votings).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool isExistsVoting =
            repository.ExistsVoting(customerId, appointmentId, participantId, votingId, suggestedDateId);
        // Assert
        Assert.AreEqual(true, isExistsVoting);
    }

    [TestMethod]
    public void ExistsVoting_NotFound_false()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid appointmentIdToSearch = new("FEBE03C9-27E5-4CBF-A285-FA13BD8522B1");
        Guid suggestedDateId = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid participantId = new("A365D57E-D78E-46AF-B127-8067A18F6B81");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid votingId = new("B198FD02-2C48-4932-AC34-A6878C65DC36");

        IQueryable<Voting> votings = new List<Voting>
        {
            new Voting
            {
                AppointmentId = appointmentId,
                SuggestedDateId = suggestedDateId,
                CustomerId = customerId,
                ParticipantId = participantId,
                VotingId = votingId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Voting>>();
        mockSet.As<IQueryable<Voting>>().Setup(m => m.Provider).Returns(votings.Provider);
        mockSet.As<IQueryable<Voting>>().Setup(m => m.Expression).Returns(votings.Expression);
        mockSet.As<IQueryable<Voting>>().Setup(m => m.ElementType).Returns(votings.ElementType);
        mockSet.As<IQueryable<Voting>>().Setup(m => m.GetEnumerator()).Returns(votings.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Votings).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        bool isExistsVoting = repository.ExistsVoting(customerId, appointmentIdToSearch, participantId,
            votingId, suggestedDateId);
        // Assert
        Assert.AreEqual(false, isExistsVoting);
    }

    [TestMethod]
    public void GetNumberOfSuggestedDates_TwoSuggestedDates_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid suggestedDateId1 = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid suggestedDateId2 = new("26688D0A-196B-408C-AB9B-8911652A6A0F");

        IQueryable<SuggestedDate> suggestedDates = new List<SuggestedDate>
        {
            new SuggestedDate
            {
                AppointmentId = appointmentId,
                SuggestedDateId = suggestedDateId1,
                CustomerId = customerId
            },
            new SuggestedDate
            {
                AppointmentId = appointmentId,
                SuggestedDateId = suggestedDateId2,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<SuggestedDate>>();
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.Provider).Returns(suggestedDates.Provider);
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.Expression).Returns(suggestedDates.Expression);
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.ElementType).Returns(suggestedDates.ElementType);
        mockSet.As<IQueryable<SuggestedDate>>().Setup(m => m.GetEnumerator()).Returns(suggestedDates.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.SuggestedDates).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        int numberOfSuggestedDates = repository.GetNumberOfSuggestedDates(customerId, appointmentId);
        // Assert
        Assert.AreEqual(numberOfSuggestedDates, suggestedDates.Count());
    }

    [TestMethod]
    public void GetNumberOfParticipants_TwoParticipants_true()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");
        Guid participantId1 = new("021CC2B6-9C84-4925-A927-36CC03BCC138");
        Guid participantId2 = new("21E50876-25D5-492A-AAA2-2A6666BD5D7B");
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");

        IQueryable<Participant> participants = new List<Participant>
        {
            new Participant
            {
                AppointmentId = appointmentId,
                ParticipantId = participantId1,
                CustomerId = customerId
            },
            new Participant
            {
                AppointmentId = appointmentId,
                ParticipantId = participantId2,
                CustomerId = customerId
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Participant>>();
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Provider).Returns(participants.Provider);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.Expression).Returns(participants.Expression);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.ElementType).Returns(participants.ElementType);
        mockSet.As<IQueryable<Participant>>().Setup(m => m.GetEnumerator()).Returns(participants.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Participants).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using AppointmentRepository repository = new(mockContext.Object, _logger);
        int numberOfParticipants = repository.GetNumberOfParticipants(customerId, appointmentId);
        // Assert
        Assert.AreEqual(numberOfParticipants, participants.Count());
    }
}