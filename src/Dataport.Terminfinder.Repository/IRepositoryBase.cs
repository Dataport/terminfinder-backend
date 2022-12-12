using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dataport.Terminfinder.Repository;

/// <summary>
/// Interface for the base repository
/// </summary>
public interface IRepositoryBase
{
    /// <summary>
    /// save data context
    /// </summary>
    /// <returns>result of SaveChanges()</returns>
    int Save();

    /// <summary>
    /// Set tracking behavior
    /// </summary>
    void SetTracking(bool tracking);

    /// <summary>
    /// BeginTransaction
    /// </summary>
    /// <returns>transaction context</returns>
    IDbContextTransaction BeginTransaction();

    /// <summary>
    /// Update BusinessObject
    /// </summary>
    /// <typeparam name="T">class</typeparam>
    /// <param name="dbSet"></param>
    /// <param name="businessObject"></param>
    /// <returns>businessobject</returns>
    T Update<T>(DbSet<T> dbSet, T businessObject) where T : class;

    /// <summary>
    /// Add BusinessObject
    /// </summary>
    /// <typeparam name="T">class</typeparam>
    /// <param name="dbSet"></param>
    /// <param name="businessObject"></param>
    /// <returns>businessobject</returns>
    T New<T>(DbSet<T> dbSet, T businessObject) where T : class;

    /// <summary>
    /// Delete BusinessObject
    /// </summary>
    /// <typeparam name="T">class</typeparam>
    /// <param name="dbSet"></param>
    /// <param name="businessObject"></param>
    void Delete<T>(DbSet<T> dbSet, T businessObject) where T : class;
}