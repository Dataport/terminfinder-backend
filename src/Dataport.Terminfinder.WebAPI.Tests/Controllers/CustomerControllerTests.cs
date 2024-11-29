namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
[ExcludeFromCodeCoverage]
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
            CustomerId = new ("BE1D657A-4D06-40DB-8443-D67BBB950EE7"),
            CustomerName = "S-H",
            Status = AppointmentStatusType.Started.ToString()
        };

        var mock = new Mock<ICustomerBusinessLayer>();
        mock.Setup(m => m.GetCustomer(fakeCustomer.CustomerId))
            .Returns(fakeCustomer);
        var controller = new CustomerController(mock.Object, _requestContext, _logger, _localizer);

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
}