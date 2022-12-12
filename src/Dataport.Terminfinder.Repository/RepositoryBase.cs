using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dataport.Terminfinder.Repository;

/// <summary>
/// Baseclass for Entity
/// </summary>
public abstract class RepositoryBase : IRepositoryBase, IDisposable
{
    /// <summary>
    /// tracking on/off
    /// </summary>
    protected bool Tracking;

    /// <summary>
    /// DBContext
    /// </summary>
    protected DataContext Context;

    /// <inheritdoc />
    public IDbContextTransaction BeginTransaction()
    {
        return Context.Database.BeginTransaction();
    }

    /// <inheritdoc />
    public int Save()
    {
        return Context.SaveChanges();
    }

    /// <summary>
    /// IDispose
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Context.Dispose();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="context"></param>
    /// <param name="tracking"></param>
    /// <exception cref="ArgumentNullException">DataContext</exception>
    protected RepositoryBase(DataContext context, bool tracking)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        SetTracking(tracking);
    }

    /// <summary>
    /// constructor
    /// </summary>
    /// <param name="context"></param>
    protected RepositoryBase(DataContext context)
        : this(context, false)
    {
    }

    /// <inheritdoc />
    public void SetTracking(bool tracking)
    {
        Tracking = tracking;
        Context.SetTracking(tracking);
    }

    /// <inheritdoc />
    public void Delete<T>(DbSet<T> dbSet, T businessObject) where T : class
    {
        dbSet.Attach(businessObject);
        dbSet.Remove(businessObject);
    }

    /// <inheritdoc />
    public T Update<T>(DbSet<T> dbSet, T businessObject) where T : class
    {
        dbSet.Attach(businessObject);
        Context.Entry(businessObject).State = EntityState.Modified;
        return businessObject;
    }

    /// <inheritdoc />
    public T New<T>(DbSet<T> dbSet, T businessObject) where T : class
    {
        dbSet.Add(businessObject);
        return businessObject;
    }
}