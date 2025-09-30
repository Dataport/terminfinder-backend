using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dataport.Terminfinder.BusinessLayer.Tests;

[TestClass]
public class BusinessLayerBaseTests
{
    private static readonly Guid ExpectedCustomerId = Guid.Parse("335A4C22-CEA0-4B81-864E-DC278F5D37D5");

    [TestMethod]
    public void ExistsCustomer_CustomerDoesExist_ReturnsTrue()
    {
        var customers = new List<Customer> { new() { CustomerId = ExpectedCustomerId } };
        var customersQueryable = customers.AsQueryable();

        var mockCustomerSet = new Mock<DbSet<Customer>>();
        mockCustomerSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customersQueryable.Provider);
        mockCustomerSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customersQueryable.Expression);
        mockCustomerSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customersQueryable.ElementType);
        using var enumeratorCustomers = customersQueryable.GetEnumerator();
        mockCustomerSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(enumeratorCustomers);

        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(x => x.ExistsCustomer(It.IsAny<Guid>())).Returns(true);

        var sut = CreateSut(mockCustomerRepo.Object);

        var result = sut.ExistsCustomer(ExpectedCustomerId);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ExistsCustomer_CustomerDoesNotExist_ReturnsFalse()
    {
        var mockCustomerRepo = new Mock<ICustomerRepository>();
        mockCustomerRepo.Setup(x => x.ExistsCustomer(It.IsAny<Guid>())).Returns(false);

        var sut = CreateSut(mockCustomerRepo.Object);

        var result = sut.ExistsCustomer(ExpectedCustomerId);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ExistsCustomer_CustomerIsEmptyGuid_ReturnsFalse()
    {
        var sut = CreateSut();

        var result = sut.ExistsCustomer(Guid.Empty);
        Assert.IsFalse(result);
    }

    private static BusinessLayerBase CreateSut(ICustomerRepository customerRepository = null)
    {
        var logger = new Mock<ILogger<BusinessLayerBase>>();
        var customerRepositoryToUse = customerRepository ?? new Mock<ICustomerRepository>().Object;

        return new BusinessLayerBase(logger.Object, customerRepositoryToUse);
    }
}