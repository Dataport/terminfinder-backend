using Dataport.Terminfinder.Repository;
using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.BusinessLayer;

/// <inheritdoc cref="ICustomerBusinessLayer" />
public class CustomerBusinessLayer : BusinessLayerBase, ICustomerBusinessLayer
{
    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="customerRepository">Customer repository</param>
    /// <param name="logger">Logger</param>
    public CustomerBusinessLayer(ICustomerRepository customerRepository,
        ILogger<AppConfigBusinessLayer> logger)
        : base(logger, customerRepository)
    {
        Logger.LogDebug($"Enter {nameof(CustomerBusinessLayer)}");

        Logger.LogDebug($"Leave {nameof(CustomerBusinessLayer)}");
    }

    /// <inheritdoc />
    public Customer GetCustomer(Guid customerId)
    {
        Logger.LogDebug($"Enter {nameof(GetCustomer)}");

        return CustomerRepository.GetCustomer(customerId);
    }

    /// <inheritdoc />
    public new bool ExistsCustomer(Guid customerId)
    {
        Logger.LogDebug($"Enter {nameof(ExistsCustomer)}");

        return base.ExistsCustomer(customerId);
    }
}