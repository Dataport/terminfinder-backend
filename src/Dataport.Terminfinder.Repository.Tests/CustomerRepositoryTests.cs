using Dataport.Terminfinder.Repository.Tests.Utils;
using JetBrains.Annotations;

namespace Dataport.Terminfinder.Repository.Tests;

[TestClass]
public class CustomerRepositoryTests
{
    private static readonly Guid ExpectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly string ExpectedCustomerName = "Customer Name";
    private static readonly string ExpectedAppointmentStatusType = nameof(AppointmentStatusType.Started);

    [TestMethod]
    public void GetCustomer_Okay()
    {
        var sut = CreateSut();

        var customer = sut.GetCustomer(ExpectedCustomerId);

        // Assert
        Assert.IsNotNull(customer);
        Assert.AreEqual(ExpectedCustomerId, customer.CustomerId);
        Assert.AreEqual(ExpectedCustomerName, customer.CustomerName);
        Assert.AreEqual(ExpectedAppointmentStatusType, customer.Status);
    }

    [TestMethod]
    public void GetCustomer_GuidNotExists_Null()
    {
        var customerId = new Guid("27A55CAB-9628-4F52-909E-8B35B155CEEC");

        // act fetch
        var sut = CreateSut();
        var customer = sut.GetCustomer(customerId);

        // Assert
        Assert.IsNull(customer);
    }

    [TestMethod]
    public void GetCustomer_GuidIsEmpty_ArgumentNullException()
    {
        var expectedExceptionMessage = "Value cannot be null. (Parameter 'customerId')";

        var sut = CreateSut();

        var exception = Assert.ThrowsException<ArgumentNullException>(() => sut.GetCustomer(Guid.Empty));
        Assert.AreEqual(expectedExceptionMessage, exception.Message);
    }

    [TestMethod]
    public void ExistsCustomer_true()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsCustomer = sut.ExistsCustomer(ExpectedCustomerId);

        // Assert
        Assert.IsTrue(isExistsCustomer);
    }

    [TestMethod]
    public void ExistsCustomer_GuidIsEmpty_false()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsCustomer = sut.ExistsCustomer(Guid.Empty);

        // Assert
        Assert.IsFalse(isExistsCustomer);
    }

    [TestMethod]
    public void ExistsCustomer_StatusNotStarted_false()
    {
        var customers = GetValidCustomers();
        customers[0].Status = nameof(AppointmentStatusType.Paused);
        var mockCustomersSet = DbSetMockFactory.CreateMockDbSet(customers);

        // act fetch
        var sut = CreateSut(mockCustomersSet);
        var isExistsCustomer = sut.ExistsCustomer(ExpectedCustomerId);

        // Assert
        Assert.IsFalse(isExistsCustomer);
    }

    private static CustomerRepository CreateSut([CanBeNull] Mock<DbSet<Customer>> mockCustomerSet = null)
    {
        // https://medium.com/@metse/entity-framework-core-unit-testing-3c412a0a997c
        var mockCustomerSetToUse = mockCustomerSet ?? DbSetMockFactory.CreateMockDbSet(GetValidCustomers());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.Customers).Returns(mockCustomerSetToUse.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));
        mockContext.Setup(c => c.SaveChanges()).Verifiable();

        var logger = new Mock<ILogger<CustomerRepository>>();

        return new CustomerRepository(mockContext.Object, logger.Object);
    }

    private static List<Customer> GetValidCustomers()
    {
        return
        [
            new Customer
            {
                CustomerId = ExpectedCustomerId,
                CustomerName = ExpectedCustomerName,
                Status = ExpectedAppointmentStatusType
            }
        ];
    }
}