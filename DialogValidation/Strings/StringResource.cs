namespace CustomControls;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/// <summary>
/// Represents a list of string resources loaded from a files.
/// </summary>
internal class StringResource
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="StringResource"/> class.
    /// </summary>
    /// <param name="filePath">Path to the file to read.</param>
    /// <param name="resourceID">Identifier of the resources.</param>
    public StringResource(string filePath, uint resourceID)
    {
        FilePath = filePath;
        ResourceID = resourceID;
    }

    /// <summary>
    /// Loads the string resources.
    /// </summary>
    public void Load()
    {
        IntPtr hMod = LoadFile();
        if (hMod != IntPtr.Zero)
        {
            _ = LoadStringValues(hMod);
            FreeHandles(hMod);
        }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the path to the file resources are loaded from.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the identifier used to find and load resources in the file.
    /// </summary>
    public uint ResourceID { get; }

    /// <summary>
    /// Gets the loaded string resources.
    /// </summary>
    public IList<string> AsStrings { get; } = new List<string>();
    #endregion

    #region Implementation
    /// <summary>
    /// Load the file containing the resources in memory.
    /// </summary>
    /// <returns>Handle to the loaded file.</returns>
    protected virtual IntPtr LoadFile()
    {
        IntPtr hMod = NativeMethods.LoadLibraryEx(FilePath, IntPtr.Zero, NativeMethods.LOAD_LIBRARY_AS_DATAFILE);
        return hMod;
    }

    /// <summary>
    /// Load the string resources and fill the AsStrings property.
    /// </summary>
    /// <param name="hMod">Handle of the resource to load.</param>
    /// <returns>True if the resource has been loaded successfully; Otherwise, false.</returns>
    protected virtual bool LoadStringValues(IntPtr hMod)
    {
        IntPtr hResDir = NativeMethods.FindResource(hMod, (IntPtr)ResourceID, (IntPtr)NativeMethods.RT_STRING);
        if (hResDir == IntPtr.Zero)
            return false;

        uint size = NativeMethods.SizeofResource(hMod, hResDir);
        IntPtr pt = NativeMethods.LoadResource(hMod, hResDir);

        byte[] bPtr = new byte[size];
        Marshal.Copy(pt, bPtr, 0, (int)size);

        List<string> values = new();

        int offset = 0;
        while (offset + 2 < size)
        {
            ushort length = BitConverter.ToUInt16(bPtr, offset);
            offset += 2;

            string value = string.Empty;
            for (int j = 0; j < length && offset + 2 < size; j++)
            {
                value += BitConverter.ToChar(bPtr, offset);
                offset += 2;
            }

            value = value.Replace('&', '_');
            values.Add(value);
        }

        AsStrings.Clear();
        for (int i = 0; i < values.Count; i++)
            AsStrings.Add(values[i]);

        return true;
    }

    /// <summary>
    /// Frees loaded handles from memory.
    /// </summary>
    /// <param name="hMod">Handle to free.</param>
    protected virtual void FreeHandles(IntPtr hMod)
    {
        _ = NativeMethods.FreeLibrary(hMod);
    }
    #endregion
}
