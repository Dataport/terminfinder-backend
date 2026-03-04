namespace Dataport.Terminfinder.Common.Jobs;

/// <summary>
/// Job Interface
/// </summary>
public interface IJob
{
    /// <summary>
    /// Execute Job
    /// </summary>
    /// <returns></returns>
    Task ExecuteAsync();
}