using Dataport.Terminfinder.Repository.Tests.Utils;
using JetBrains.Annotations;

namespace Dataport.Terminfinder.Repository.Tests;

[TestClass]
public class LegacyCustomerRepositoryTests
{
    private static readonly Guid ExpectedLegacyCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly string ExpectedHostAddress = "expectedHostAddress";
    private static readonly string ExpectedStatusType = nameof(AppointmentStatusType.Started);

    [TestMethod]
    public void GetLegacyCustomer_Okay()
    {
        var sut = CreateSut();

        var customer = sut.GetLegacyCustomer(ExpectedLegacyCustomerId);

        // Assert
        Assert.IsNotNull(customer);
        Assert.AreEqual(ExpectedLegacyCustomerId, customer.CustomerId);
        Assert.AreEqual(ExpectedHostAddress, customer.HostAddress);
        Assert.AreEqual(ExpectedStatusType, customer.Status);
    }

    [TestMethod]
    public void GetLegacyCustomer_GuidNotExists_Null()
    {
        var customerId = Guid.NewGuid();

        // act fetch
        var sut = CreateSut();
        var customer = sut.GetLegacyCustomer(customerId);

        // Assert
        Assert.IsNull(customer);
    }

    [TestMethod]
    public void GetLegacyCustomer_GuidIsEmpty_ArgumentNullException()
    {
        var expectedExceptionMessage = "Value cannot be null. (Parameter 'customerId')";

        var sut = CreateSut();

        var exception = Assert.ThrowsException<ArgumentNullException>(() => sut.GetLegacyCustomer(Guid.Empty));
        Assert.AreEqual(expectedExceptionMessage, exception.Message);
    }

    [TestMethod]
    public void ExistsLegacyCustomer_true()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsLegacyCustomer = sut.ExistsLegacyCustomer(ExpectedLegacyCustomerId);

        // Assert
        Assert.IsTrue(isExistsLegacyCustomer);
    }

    [TestMethod]
    public void ExistsLegacyCustomer_GuidIsEmpty_false()
    {
        // act fetch
        var sut = CreateSut();
        var isExistsLegacyCustomer = sut.ExistsLegacyCustomer(Guid.Empty);

        // Assert
        Assert.IsFalse(isExistsLegacyCustomer);
    }

    [TestMethod]
    public void ExistsLegacyCustomer_StatusNotStarted_false()
    {
        var customers = GetValidLegacyCustomers();
        customers[0].Status = nameof(AppointmentStatusType.Paused);
        var mockCustomersSet = DbSetMockFactory.CreateMockDbSet(customers);

        // act fetch
        var sut = CreateSut(mockCustomersSet);
        var isExistsLegacyCustomer = sut.ExistsLegacyCustomer(ExpectedLegacyCustomerId);

        // Assert
        Assert.IsFalse(isExistsLegacyCustomer);
    }

    private static LegacyCustomerRepository CreateSut([CanBeNull] Mock<DbSet<LegacyCustomer>> mockCustomerSet = null)
    {
        var mockCustomerSetToUse = mockCustomerSet ?? DbSetMockFactory.CreateMockDbSet(GetValidLegacyCustomers());

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.LegacyCustomers).Returns(mockCustomerSetToUse.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));
        mockContext.Setup(c => c.SaveChanges()).Verifiable();

        var logger = new Mock<ILogger<LegacyCustomerRepository>>();

        return new LegacyCustomerRepository(mockContext.Object, logger.Object);
    }

    private static List<LegacyCustomer> GetValidLegacyCustomers()
    {
        return
        [
            new LegacyCustomer
            {
                CustomerId = ExpectedLegacyCustomerId,
                HostAddress = ExpectedHostAddress,
                Status = ExpectedStatusType
            }
        ];
    }
}