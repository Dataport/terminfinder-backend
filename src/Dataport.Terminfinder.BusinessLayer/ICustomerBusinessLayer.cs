using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.BusinessLayer;

/// <summary>
/// Business methods for customer
/// </summary>
public interface ICustomerBusinessLayer
{
    /// <summary>
    /// Get customer information
    /// </summary>
    /// <param name="customerId">id of the customer</param>
    /// <returns>customer information or null if the customer does not exist</returns>
    [CanBeNull]
    Customer GetCustomer(Guid customerId);


    /// <summary>
    /// Check, if the customer is valid and started
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns>true, if the customer is valid</returns>
    bool ExistsCustomer(Guid customerId);
}