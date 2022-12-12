using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;

namespace Dataport.Terminfinder.Repository;

/// <inheritdoc cref="ICustomerRepository" />
public class CustomerRepository : RepositoryBase, ICustomerRepository
{
    private readonly ILogger<CustomerRepository> _logger;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="logger"></param>
    public CustomerRepository(DataContext ctx, ILogger<CustomerRepository> logger)
        : base(ctx)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Customer GetCustomer(Guid customerId)
    {
        _logger.LogDebug($"Enter {nameof(GetCustomer)}");

        if (customerId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(customerId));
        }

        Customer customer = (from c in Context.Customers
            where (c.CustomerId == customerId)
            select c).SingleOrDefault();

        return customer;
    }

    /// <inheritdoc />
    public bool ExistsCustomer(Guid customerId)
    {
        _logger.LogDebug($"Enter {nameof(ExistsCustomer)}");

        if (customerId == Guid.Empty)
        {
            return false;
        }

        var selectedCustomerId = (from c in Context.Customers
            where (c.CustomerId == customerId
                   && c.Status == AppointmentStatusType.Started.ToString())
            select c.CustomerId).SingleOrDefault();

        return (selectedCustomerId == customerId);
    }
}