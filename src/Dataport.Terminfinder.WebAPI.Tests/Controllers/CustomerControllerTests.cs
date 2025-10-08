namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class CustomerControllerTests
{
    private static readonly Guid ExpectedCustomerId = new("BE1D657A-4D06-40DB-8443-D67BBB950EE7");
    private static readonly string ExpectedCustomerName = "S-H";
    private static readonly string ExpectedStatus = nameof(AppointmentStatusType.Started);

    private static readonly Customer ExpectedCustomer = new()
    {
        CustomerId = ExpectedCustomerId,
        CustomerName = ExpectedCustomerName,
        Status = ExpectedStatus
    };

    [TestMethod]
    public void GetCustomer_Okay()
    {
        var mockCustomerBusinessLayer = new Mock<ICustomerBusinessLayer>();
        mockCustomerBusinessLayer.Setup(m => m.GetCustomer(ExpectedCustomerId)).Returns(ExpectedCustomer);
        var sut = CreateSut(mockCustomerBusinessLayer.Object);

        // Act
        var httpResult = sut.GetCustomer(ExpectedCustomerId.ToString());
        var result = httpResult as OkObjectResult;
        var customerResult = result?.Value as Customer;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(customerResult);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(ExpectedCustomerId, customerResult.CustomerId);
        Assert.AreEqual(ExpectedCustomerName, customerResult.CustomerName);
        Assert.AreEqual(ExpectedStatus, customerResult.Status);
    }

    [TestMethod]
    public void GetCustomer_CustomerIdIsEmptyString_BadRequest()
    {
        var sut = CreateSut();

        // Act
        var result = sut.GetCustomer(string.Empty);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void GetCustomer_CustomerIdIsInvalid_BadRequest()
    {
        var invalidGuidString = "invalid";
        var sut = CreateSut();

        // Act
        var result = sut.GetCustomer(invalidGuidString);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void GetCustomer_CustomerIsInvalid_NotFound()
    {
        var customer = ExpectedCustomer;
        customer.Status = nameof(AppointmentStatusType.Undefined);

        var mockCustomerBusinessLayer = new Mock<ICustomerBusinessLayer>();
        mockCustomerBusinessLayer.Setup(m => m.GetCustomer(ExpectedCustomerId)).Returns(customer);
        var sut = CreateSut(mockCustomerBusinessLayer.Object);

        // Act
        var result = sut.GetCustomer(ExpectedCustomerId.ToString());
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    private static CustomerController CreateSut(ICustomerBusinessLayer customerBusinessLayer = null)
    {
        var customerBusinessLayerToUse = customerBusinessLayer ?? new Mock<ICustomerBusinessLayer>().Object;
        var mockRequestContext = new Mock<IRequestContext>();
        var mockLogger = new Mock<ILogger<CustomerController>>();
        var mockLocalizer = new Mock<IStringLocalizer<CustomerController>>();

        return new CustomerController(
            customerBusinessLayerToUse,
            mockRequestContext.Object,
            mockLogger.Object,
            mockLocalizer.Object);
    }
}