using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.BusinessObject.Enum;

namespace Dataport.Terminfinder.Repository;

/// <inheritdoc cref="ILegacyCustomerRepository" />
public class LegacyCustomerRepository : RepositoryBase, ILegacyCustomerRepository
{
    private readonly ILogger<LegacyCustomerRepository> _logger;

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="logger"></param>
    public LegacyCustomerRepository(DataContext ctx, ILogger<LegacyCustomerRepository> logger)
        : base(ctx)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public LegacyCustomer GetLegacyCustomer(Guid customerId)
    {
        _logger.LogDebug($"Enter {nameof(GetLegacyCustomer)}");

        return Context.LegacyCustomers.FirstOrDefault(c => c.CustomerId == customerId);
    }

    /// <inheritdoc />
    public bool ExistsLegacyCustomer(Guid customerId)
    {
        _logger.LogDebug($"Enter {nameof(ExistsLegacyCustomer)}");

        if (customerId == Guid.Empty)
        {
            return false;
        }

        return Context.LegacyCustomers.Any(c =>
            c.CustomerId == customerId
            && c.Status == nameof(AppointmentStatusType.Started));
    }
}