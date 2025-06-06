﻿namespace CustomControls;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Contracts;

/// <summary>
/// Represents a list of string resources loaded from a files.
/// </summary>
internal class CursorResource
{
    #region Constants
    /// <summary>
    /// Default width and height when no preference is provided in the constructor.
    /// </summary>
    public const int DefaultPreferredSize = 32;
    #endregion

    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="CursorResource"/> class.
    /// </summary>
    /// <param name="filePath">Path to the file to read.</param>
    /// <param name="resourceID">Identifier of the resources.</param>
    public CursorResource(string filePath, uint resourceID)
        : this(filePath, resourceID, DefaultPreferredSize)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CursorResource"/> class.
    /// </summary>
    /// <param name="filePath">Path to the file to read.</param>
    /// <param name="resourceID">Identifier of the resources.</param>
    /// <param name="preferredSize">Width and height of the preferred size, in pixels.</param>
    public CursorResource(string filePath, uint resourceID, int preferredSize)
    {
        FilePath = filePath;
        ResourceID = resourceID;
        PreferredSize = preferredSize;
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
    /// Gets the width and height of the preferred size, in pixels.
    /// </summary>
    public int PreferredSize { get; }
    #endregion

    #region Client Interface
    /// <summary>
    /// Loads the cursor resources.
    /// </summary>
    public virtual Cursor? Load()
    {
        IntPtr hMod = LoadFile();
        if (hMod == IntPtr.Zero)
            return null;

        NativeMethods.FileHeader Header;
        List<NativeMethods.FileRecord> RecordList;
        if (!LoadDirectory(hMod, out Header, out RecordList))
        {
            FreeHandle(hMod);
            return null;
        }

        bool IsLoaded = true;
        foreach (NativeMethods.FileRecord Record in RecordList)
            if (!LoadData(hMod, Record))
            {
                IsLoaded = false;
                break;
            }

        if (!IsLoaded)
        {
            FreeHandle(hMod);
            return null;
        }

        Cursor Result = ConvertToCursor(Header, RecordList);

        FreeHandle(hMod);

        return Result;
    }
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
    /// Load the cursor directory with images.
    /// </summary>
    /// <param name="hMod">Handle of the resource file.</param>
    /// <param name="header">Header of the resource file.</param>
    /// <param name="recordList">List of record.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    protected virtual bool LoadDirectory(IntPtr hMod, out NativeMethods.FileHeader header, out List<NativeMethods.FileRecord> recordList)
    {
        IntPtr hResDir = NativeMethods.FindResource(hMod, (IntPtr)ResourceID, (IntPtr)NativeMethods.RT_GROUP_CURSOR);
        if (hResDir == IntPtr.Zero)
        {
            Contract.Unused(out header);
            Contract.Unused(out recordList);
            return false;
        }

        uint size = NativeMethods.SizeofResource(hMod, hResDir);
        if (size < 6)
        {
            Contract.Unused(out header);
            Contract.Unused(out recordList);
            return false;
        }

        IntPtr pt = NativeMethods.LoadResource(hMod, hResDir);

        byte[] bPtr = new byte[size];
        Marshal.Copy(pt, bPtr, 0, (int)size);

        header = new NativeMethods.FileHeader
        {
            Reserved = BitConverter.ToInt16(bPtr, 0),
            Type = BitConverter.ToInt16(bPtr, 2),
            ImageCount = BitConverter.ToInt16(bPtr, 4),
        };

        recordList = new List<NativeMethods.FileRecord>();
        List<NativeMethods.FileRecord> TempRecordList = new();
        bool IsGoodSizedImageAdded = false;

        for (short i = 0; i < header.ImageCount; i++)
        {
            int Offset = (i * 14) + 6;

            NativeMethods.FileRecord Record = new();
            Record.bWidth = bPtr[Offset + 0];
            Record.bHeight = Record.bWidth;
            short wBitCount = BitConverter.ToInt16(bPtr, Offset + 6);
            Record.bColorCount = (byte)Math.Pow(2, wBitCount);
            Record.bReserved = 0;
            Record.nID = BitConverter.ToUInt16(bPtr, Offset + 12);

            TempRecordList.Add(Record);

            if (Record.bWidth == PreferredSize)
            {
                IsGoodSizedImageAdded = true;
                recordList.Add(Record);
            }
        }

        if (!IsGoodSizedImageAdded)
            recordList.AddRange(TempRecordList);

        return true;
    }

    /// <summary>
    /// Loads data from a resource file.
    /// </summary>
    /// <param name="hMod">Handle to the file.</param>
    /// <param name="record">Record in the file.</param>
    /// <returns>True if successful; otherwise, false.</returns>
    protected virtual bool LoadData(IntPtr hMod, NativeMethods.FileRecord record)
    {
#if NETFRAMEWORK
        IntPtr hRes = NativeMethods.FindResource(hMod, (IntPtr)record.nID, (IntPtr)NativeMethods.RT_CURSOR);
#elif NET7_0_OR_GREATER
        IntPtr hRes = NativeMethods.FindResource(hMod, record.nID, (IntPtr)NativeMethods.RT_CURSOR);
#else
        IntPtr hRes = NativeMethods.FindResource(hMod, (IntPtr)(int)record.nID, (IntPtr)NativeMethods.RT_CURSOR);
#endif
        if (hRes == IntPtr.Zero)
            return false;

        uint size = NativeMethods.SizeofResource(hMod, hRes);
        IntPtr pt = NativeMethods.LoadResource(hMod, hRes);

        record.Data = new byte[size];
        Marshal.Copy(pt, record.Data, 0, (int)size);

        record.HotspotX = BitConverter.ToUInt16(record.Data, 0);
        record.HotspotY = BitConverter.ToUInt16(record.Data, 2);

        return true;
    }

    /// <summary>
    /// Converts a record to a cursor.
    /// </summary>
    /// <param name="header">Header of the file.</param>
    /// <param name="recordList">List of records.</param>
    protected virtual Cursor ConvertToCursor(NativeMethods.FileHeader header, List<NativeMethods.FileRecord> recordList)
    {
        using MemoryStream ms = new();
        byte[] FieldData;

        FieldData = BitConverter.GetBytes(header.Reserved);
        ms.Write(FieldData, 0, FieldData.Length);
        FieldData = BitConverter.GetBytes(header.Type);
        ms.Write(FieldData, 0, FieldData.Length);

        short ImageCount = (short)recordList.Count;
        FieldData = BitConverter.GetBytes(ImageCount);
        ms.Write(FieldData, 0, FieldData.Length);

        int DataOffset = 6 + (recordList.Count * 16);

        foreach (NativeMethods.FileRecord Record in recordList)
        {
            ms.WriteByte(Record.bWidth);
            ms.WriteByte(Record.bHeight);
            ms.WriteByte(Record.bColorCount);
            ms.WriteByte(Record.bReserved);
            FieldData = BitConverter.GetBytes(Record.HotspotX);
            ms.Write(FieldData, 0, FieldData.Length);
            FieldData = BitConverter.GetBytes(Record.HotspotY);
            ms.Write(FieldData, 0, FieldData.Length);

            int DataLength = Record.Data.Length - 4;
            FieldData = BitConverter.GetBytes(DataLength);
            ms.Write(FieldData, 0, FieldData.Length);
            FieldData = BitConverter.GetBytes(DataOffset);
            ms.Write(FieldData, 0, FieldData.Length);

            DataOffset += DataLength;
        }

        foreach (NativeMethods.FileRecord Record in recordList)
            ms.Write(Record.Data, 4, Record.Data.Length - 4);

        _ = ms.Seek(0, SeekOrigin.Begin);

        return new Cursor(ms);
    }

    /// <summary>
    /// Frees loaded handle from memory.
    /// </summary>
    /// <param name="hMod">Handle to free.</param>
    protected virtual void FreeHandle(IntPtr hMod) => _ = NativeMethods.FreeLibrary(hMod);
    #endregion
}
