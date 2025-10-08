namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class CustomerControllerTests
{
    private ILogger<CustomerController> _logger;
    private IStringLocalizer<CustomerController> _localizer;
    private IRequestContext _requestContext;

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<CustomerController>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<CustomerController>>();

        // fake localizer
        var mockLocalize = new Mock<IStringLocalizer<CustomerController>>();
        _localizer = mockLocalize.Object;
        _localizer = Mock.Of<IStringLocalizer<CustomerController>>();

        // fake request context
        _requestContext = Mock.Of<IRequestContext>();
    }

    [TestMethod]
    public void GetCustomer_Okay()
    {
        Customer fakeCustomer = new()
        {
            CustomerId = new Guid("BE1D657A-4D06-40DB-8443-D67BBB950EE7"),
            CustomerName = "S-H",
            Status = nameof(AppointmentStatusType.Started)
        };

        var mockCustomerBusinessLayer = new Mock<ICustomerBusinessLayer>();
        mockCustomerBusinessLayer.Setup(m => m.GetCustomer(fakeCustomer.CustomerId))
            .Returns(fakeCustomer);
        var controller = new CustomerController(mockCustomerBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.GetCustomer(fakeCustomer.CustomerId.ToString());
        OkObjectResult result = httpResult as OkObjectResult;
        Customer customer = result?.Value as Customer;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(customer);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(customer.CustomerId, fakeCustomer.CustomerId);
        Assert.AreEqual(customer.CustomerName, fakeCustomer.CustomerName);
        Assert.AreEqual(customer.Status, fakeCustomer.Status);
    }
    
    [TestMethod]
    public void GetCustomer_CustomerIdIsEmptyString_BadRequest()
    {
        var mockCustomerBusinessLayer = new Mock<ICustomerBusinessLayer>();
        var controller = new CustomerController(mockCustomerBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        var result = controller.GetCustomer(string.Empty);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
    
    [TestMethod]
    public void GetCustomer_CustomerIdIsInvalid_BadRequest()
    {
        var invalidGuidString = "invalid";
        var mockCustomerBusinessLayer = new Mock<ICustomerBusinessLayer>();
        var controller = new CustomerController(mockCustomerBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        var result = controller.GetCustomer(invalidGuidString);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
    
    [TestMethod]
    public void GetCustomer_CustomerIsInvalid_NotFound()
    {
        Customer customer = new()
        {
            CustomerId = new Guid("BE1D657A-4D06-40DB-8443-D67BBB950EE7"),
            CustomerName = "S-H",
            Status = nameof(AppointmentStatusType.Undefined)
        };

        var mockCustomerBusinessLayer = new Mock<ICustomerBusinessLayer>();
        mockCustomerBusinessLayer.Setup(m => m.GetCustomer(customer.CustomerId)).Returns(customer);
        var controller = new CustomerController(mockCustomerBusinessLayer.Object, _requestContext, _logger, _localizer);

        // Act
        var result = controller.GetCustomer(customer.CustomerId.ToString());
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }
}