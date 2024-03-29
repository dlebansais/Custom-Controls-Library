﻿namespace CustomControls;

using System;
using System.Runtime.InteropServices;

/// <summary>
/// Contains unmanaged methods to read resources from a file.
/// </summary>
internal static class NativeMethods
{
    /// <summary>
    /// Flag to indicate the file should be loaded as data and not as executable code.
    /// </summary>
    public const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

    /// <summary>
    /// Flag to indicate a string resource.
    /// </summary>
    public const uint RT_STRING = 6;

    /// <summary>
    /// Loads a DLL in memory.
    /// </summary>
    /// <param name="lpFileName">Path to the file to load.</param>
    /// <param name="hFile">This parameter is not used.</param>
    /// <param name="dwFlags">If LOAD_LIBRARY_AS_DATAFILE is specified, loads the file as data rather than executable code.</param>
    /// <returns>
    /// A handle to the loaded file.
    /// </returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern IntPtr LoadLibraryEx([MarshalAs(UnmanagedType.LPWStr)]string lpFileName, IntPtr hFile, uint dwFlags);

    /// <summary>
    /// Looks for a resource by its identifier in a file.
    /// </summary>
    /// <param name="hModule">Handle of the loaded file.</param>
    /// <param name="lpID">Identifier of the resource to find.</param>
    /// <param name="lpType">Type of the resource.</param>
    /// <returns>
    /// A handle to the resource.
    /// </returns>
    [DllImport("kernel32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpID, IntPtr lpType);

    /// <summary>
    /// Loads a resource in memory.
    /// </summary>
    /// <param name="hModule">Handle of the loaded file.</param>
    /// <param name="hResInfo">Handle to the resource to load.</param>
    /// <returns>
    /// A handle to the block loaded in memory.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

    /// <summary>
    /// Gets the size of a resource.
    /// </summary>
    /// <param name="hModule">Handle of the loaded file.</param>
    /// <param name="hResInfo">Handle to the resource to load.</param>
    /// <returns>
    /// The size of the resource, in bytes.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

    /// <summary>
    /// Frees a loaded DLL from memory.
    /// </summary>
    /// <param name="hMod">Handle of the library to free.</param>
    /// <returns>
    /// The returned value can be ignored.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeLibrary(IntPtr hMod);
}
