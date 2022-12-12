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
        Assembly assembly = GetDefaultAssembly();
        return GetVersion(assembly);
    }

    /// <summary>
    /// Gets the File version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetVersion(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        Assembly assembly = Assembly.GetAssembly(type);
        return GetVersion(assembly);
    }

    /// <summary>
    /// Gets the File version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetVersion(Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        return fvi.FileVersion;
    }

    /// <summary>
    /// Gets the product version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetProductVersion()
    {
        Assembly assembly = GetDefaultAssembly();
        return GetProductVersion(assembly);
    }

    /// <summary>
    /// Gets the product version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetProductVersion(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        Assembly assembly = Assembly.GetAssembly(type);
        return GetProductVersion(assembly);
    }

    /// <summary>
    /// Gets the product version of the assembly
    /// </summary>
    /// <returns></returns>
    public static string GetProductVersion(Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        return fvi.ProductVersion;
    }

    private static Assembly GetDefaultAssembly()
    {
        return Assembly.GetEntryAssembly();
    }
}