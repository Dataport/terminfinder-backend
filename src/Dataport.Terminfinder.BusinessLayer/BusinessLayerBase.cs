using Dataport.Terminfinder.Repository;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Dataport.Terminfinder.BusinessLayer.Tests")]
namespace Dataport.Terminfinder.BusinessLayer;

/// <summary>
/// Base class for all business layer
/// </summary>
public class BusinessLayerBase
{
    /// <summary>
    /// Logger
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Customer repository
    /// </summary>
    protected ICustomerRepository CustomerRepository { get; }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="customerRepository"></param>
    protected internal BusinessLayerBase(ILogger logger, ICustomerRepository customerRepository)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        CustomerRepository = customerRepository;
    }

    /// <summary>
    /// Check, if the customer is valid and started
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns>true, if the customer is valid</returns>
    public bool ExistsCustomer(Guid customerId)
    {
        Logger.LogDebug($"Enter {nameof(ExistsCustomer)}");

        return customerId != Guid.Empty 
            && CustomerRepository.ExistsCustomer(customerId);
    }
}