using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Dataport.Terminfinder.Common;

/// <summary>
/// Assembly utilities
/// </summary>
[ExcludeFromCodeCoverage]
public static class AssemblyUtils
{
    /// <summary>
    /// Gets the File version of the assembly
    /// </summary>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public static string GetVersion()
    {
        var assembly = GetDefaultAssembly();
        return GetVersion(assembly);
    }

    /// <summary>
    /// Gets the File version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetVersion(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var assembly = Assembly.GetAssembly(type);
        return GetVersion(assembly);
    }

    /// <summary>
    /// Gets the File version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetVersion(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        return fvi.FileVersion;
    }

    /// <summary>
    /// Gets the product version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetProductVersion()
    {
        var assembly = GetDefaultAssembly();
        return GetProductVersion(assembly);
    }

    /// <summary>
    /// Gets the product version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetProductVersion(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var assembly = Assembly.GetAssembly(type);
        return GetProductVersion(assembly);
    }

    /// <summary>
    /// Gets the product version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetProductVersion(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        return fvi.ProductVersion;
    }

    private static Assembly GetDefaultAssembly()
    {
        return Assembly.GetEntryAssembly();
    }
}