namespace Dataport.Terminfinder.Repository.Tests;

[TestClass]
[ExcludeFromCodeCoverage]
public class CustomerRepositoryTests
{
    private ILogger<CustomerRepository> _logger;

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<CustomerRepository>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<CustomerRepository>>();
    }

    [TestMethod]
    public void GetCustomer_Okay()
    {
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string customerName = "bla";
        string status = AppointmentStatusType.Started.ToString();

        // https://medium.com/@metse/entity-framework-core-unit-testing-3c412a0a997c

        IQueryable<Customer> customers = new List<Customer>
        {
            new Customer()
            {
                CustomerId = customerId,
                CustomerName = customerName,
                Status = status
            },
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Customer>>();
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Customers).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using CustomerRepository repository = new(mockContext.Object, _logger);
        Customer customer = repository.GetCustomer(customerId);

        // Assert
        Assert.IsNotNull(customer);
        Assert.AreEqual(customer.CustomerId, customerId);
        Assert.AreEqual(customer.CustomerName, customerName);
        Assert.AreEqual(customer.Status, status);
    }

    [TestMethod]
    public void GetCustomer_GuidNotExists_Null()
    {
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        Guid fakeCustomerId = new("27A55CAB-9628-4F52-909E-8B35B155CEEC");
        string customerName = "bla";
        string status = AppointmentStatusType.Started.ToString().ToLower();

        // https://medium.com/@metse/entity-framework-core-unit-testing-3c412a0a997c

        IQueryable<Customer> customers = new List<Customer>
        {
            new Customer()
            {
                CustomerId = customerId,
                CustomerName = customerName,
                Status = status
            },
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Customer>>();
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Customers).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using CustomerRepository repository = new(mockContext.Object, _logger);
        Customer customer = repository.GetCustomer(fakeCustomerId);

        // Assert
        Assert.IsNull(customer);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void GetCustomer_GuidIsEmpty_ArgumentNullException()
    {
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string customerName = "bla";
        string status = AppointmentStatusType.Started.ToString().ToLower();

        // https://medium.com/@metse/entity-framework-core-unit-testing-3c412a0a997c

        IQueryable<Customer> customers = new List<Customer>
        {
            new Customer()
            {
                CustomerId = customerId,
                CustomerName = customerName,
                Status = status
            },
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Customer>>();
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Customers).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        using CustomerRepository repository = new(mockContext.Object, _logger);
        repository.GetCustomer(Guid.Empty);
        Assert.Fail("An Exception should be thrown");
    }

    [TestMethod]
    public void ExistsCustomer_true()
    {
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string customerName = "bla";
        string status = AppointmentStatusType.Started.ToString();

        // https://medium.com/@metse/entity-framework-core-unit-testing-3c412a0a997c

        IQueryable<Customer> customers = new List<Customer>
        {
            new Customer()
            {
                CustomerId = customerId,
                CustomerName = customerName,
                Status = status
            },
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Customer>>();
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Customers).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using CustomerRepository repository = new(mockContext.Object, _logger);
        bool isExistsCustomer = repository.ExistsCustomer(customerId);

        // Assert
        Assert.AreEqual(true, isExistsCustomer);
    }

    [TestMethod]
    public void ExistsCustomer_GuidIsEmpty_false()
    {
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string customerName = "bla";
        string status = AppointmentStatusType.Started.ToString();

        // https://medium.com/@metse/entity-framework-core-unit-testing-3c412a0a997c

        IQueryable<Customer> customers = new List<Customer>
        {
            new Customer()
            {
                CustomerId = customerId,
                CustomerName = customerName,
                Status = status
            },
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Customer>>();
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Customers).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using CustomerRepository repository = new(mockContext.Object, _logger);
        bool isExistsCustomer = repository.ExistsCustomer(Guid.Empty);

        // Assert
        Assert.AreEqual(false, isExistsCustomer);
    }

    [TestMethod]
    public void ExistsCustomer_StatusNotStarted_false()
    {
        Guid customerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
        string customerName = "bla";
        string status = AppointmentStatusType.Paused.ToString();

        // https://medium.com/@metse/entity-framework-core-unit-testing-3c412a0a997c

        IQueryable<Customer> customers = new List<Customer>
        {
            new Customer()
            {
                CustomerId = customerId,
                CustomerName = customerName,
                Status = status
            },
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<Customer>>();
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
        mockSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Customers).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        using CustomerRepository repository = new(mockContext.Object, _logger);
        bool isExistsCustomer = repository.ExistsCustomer(customerId);

        // Assert
        Assert.AreEqual(false, isExistsCustomer);
    }

}