using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace CustomControls
{
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
        /// Flag to indicate a cursor resource.
        /// </summary>
        public const uint RT_CURSOR = 1;

        /// <summary>
        /// Flag to indicate a cursor group resource.
        /// </summary>
        public const uint RT_GROUP_CURSOR = RT_CURSOR + 11;

        public class FileHeader
        {
            public short Reserved;
            public short Type;
            public short ImageCount;
        }

        public class FileRecord
        {
            public byte bWidth;
            public byte bHeight;
            public byte bColorCount;
            public byte bReserved;
            public ushort nID;
            public byte[] Data = Array.Empty<byte>();
            public ushort HotspotX;
            public ushort HotspotY;
        }

        /// <summary>
        /// Loads a DLL in memory
        /// </summary>
        /// <parameters>
        /// <param name="lpFileName">Path to the file to load.</param>
        /// <param name="hFile">This parameter is not used.</param>
        /// <param name="dwFlags">If LOAD_LIBRARY_AS_DATAFILE is specified, loads the file as data rather than executable code.</param>
        /// </parameters>
        /// <returns>
        /// A handle to the loaded file.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibraryEx([MarshalAs(UnmanagedType.LPWStr)]string lpFileName, IntPtr hFile, UInt32 dwFlags);

        /// <summary>
        /// Looks for a resource by its identifier in a file.
        /// </summary>
        /// <parameters>
        /// <param name="hModule">Handle of the loaded file.</param>
        /// <param name="lpID">Identifier of the resource to find.</param>
        /// <param name="lpType">Type of the resource.</param>
        /// </parameters>
        /// <returns>
        /// A handle to the resource.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpID, IntPtr lpType);

        /// <summary>
        /// Loads a resource in memory
        /// </summary>
        /// <parameters>
        /// <param name="hModule">Handle of the loaded file.</param>
        /// <param name="hResInfo">Handle to the resource to load.</param>
        /// </parameters>
        /// <returns>
        /// A handle to the block loaded in memory.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        /// <summary>
        /// Gets the size of a resource
        /// </summary>
        /// <parameters>
        /// <param name="hModule">Handle of the loaded file.</param>
        /// <param name="hResInfo">Handle to the resource to load.</param>
        /// </parameters>
        /// <returns>
        /// The size of the resource, in bytes.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        /// <summary>
        /// Frees a loaded DLL from memory.
        /// </summary>
        /// <parameters>
        /// <param name="hMod">Handle of the library to free.</param>
        /// </parameters>
        /// <returns>
        /// The returned value can be ignored.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hMod);
    }

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
        /// <parameters>
        /// <param name="FilePath">Path to the file to read.</param>
        /// <param name="ResourceID">Identifier of the resources.</param>
        /// </parameters>
        public CursorResource(string FilePath, uint ResourceID)
            : this(FilePath, ResourceID, DefaultPreferredSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorResource"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="filePath">Path to the file to read.</param>
        /// <param name="resourceID">Identifier of the resources.</param>
        /// <param name="preferredSize">Width and height of the preferred size, in pixels.</param>
        /// </parameters>
        public CursorResource(string filePath, uint resourceID, int preferredSize)
        {
            FilePath = filePath;
            ResourceID = resourceID;
            PreferredSize = preferredSize;
        }

        /// <summary>
        /// Loads the cursor resources.
        /// </summary>
        public virtual void Load()
        {
            IntPtr hMod = LoadFile();
            if (hMod != IntPtr.Zero)
            {
                NativeMethods.FileHeader Header;
                List<NativeMethods.FileRecord> RecordList;
                if (LoadDirectory(hMod, out Header, out RecordList))
                {
                    foreach (NativeMethods.FileRecord Record in RecordList)
                        if (!LoadData(hMod, Record))
                            return;

                    ConvertToCursor(Header, RecordList);
                }

                FreeHandles(hMod);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Path to the file resources are loaded from.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Identifier used to find and load resources in the file.
        /// </summary>
        public uint ResourceID { get; private set; }

        /// <summary>
        /// Width and height of the preferred size, in pixels.
        /// </summary>
        public int PreferredSize { get; private set; }

        /// <summary>
        /// Loaded cursor resources.
        /// </summary>
        public Cursor AsCursor { get; private set; } = Cursors.None;
        #endregion

        #region Implementation
        /// <summary>
        /// Load the file containing the resources in memory.
        /// </summary>
        protected virtual IntPtr LoadFile()
        {
            IntPtr hMod = NativeMethods.LoadLibraryEx(FilePath, IntPtr.Zero, NativeMethods.LOAD_LIBRARY_AS_DATAFILE);
            return hMod;
        }

        /// <summary>
        /// Load the cursor directory with images.
        /// </summary>
        protected virtual bool LoadDirectory(IntPtr hMod, out NativeMethods.FileHeader Header, out List<NativeMethods.FileRecord> RecordList)
        {
            IntPtr hResDir = NativeMethods.FindResource(hMod, (IntPtr)ResourceID, (IntPtr)NativeMethods.RT_GROUP_CURSOR);
            if (hResDir == IntPtr.Zero)
                throw new ArgumentOutOfRangeException(nameof(hMod));

            uint size = NativeMethods.SizeofResource(hMod, hResDir);
            if (size < 6)
                throw new ArgumentOutOfRangeException(nameof(hMod));

            IntPtr pt = NativeMethods.LoadResource(hMod, hResDir);

            byte[] bPtr = new byte[size];
            Marshal.Copy(pt, bPtr, 0, (int)size);

            Header = new NativeMethods.FileHeader
            {
                Reserved = BitConverter.ToInt16(bPtr, 0),
                Type = BitConverter.ToInt16(bPtr, 2),
                ImageCount = BitConverter.ToInt16(bPtr, 4)
            };

            RecordList = new List<NativeMethods.FileRecord>();
            List<NativeMethods.FileRecord> TempRecordList = new List<NativeMethods.FileRecord>();
            bool IsGoodSizedImageAdded = false;

            for (short i = 0; i < Header.ImageCount; i++)
            {
                int Offset = (i * 14) + 6;

                NativeMethods.FileRecord Record = new NativeMethods.FileRecord();
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
                    RecordList.Add(Record);
                }
            }

            if (!IsGoodSizedImageAdded)
                RecordList.AddRange(TempRecordList);

            return true;
        }

        protected virtual bool LoadData(IntPtr hMod, NativeMethods.FileRecord Record)
        {
            IntPtr hRes = NativeMethods.FindResource(hMod, (IntPtr)Record.nID, (IntPtr)NativeMethods.RT_CURSOR);
            if (hRes == IntPtr.Zero)
                return false;

            uint size = NativeMethods.SizeofResource(hMod, hRes);
            IntPtr pt = NativeMethods.LoadResource(hMod, hRes);

            Record.Data = new byte[size];
            Marshal.Copy(pt, Record.Data, 0, (int)size);

            Record.HotspotX = BitConverter.ToUInt16(Record.Data, 0);
            Record.HotspotY = BitConverter.ToUInt16(Record.Data, 2);

            return true;
        }

        protected virtual void ConvertToCursor(NativeMethods.FileHeader Header, List<NativeMethods.FileRecord> RecordList)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] FieldData;

                FieldData = BitConverter.GetBytes(Header.Reserved);
                ms.Write(FieldData, 0, FieldData.Length);
                FieldData = BitConverter.GetBytes(Header.Type);
                ms.Write(FieldData, 0, FieldData.Length);

                short ImageCount = (short)RecordList.Count;
                FieldData = BitConverter.GetBytes(ImageCount);
                ms.Write(FieldData, 0, FieldData.Length);

                int DataOffset = 6 + (RecordList.Count * 16);

                foreach (NativeMethods.FileRecord Record in RecordList)
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

                foreach (NativeMethods.FileRecord Record in RecordList)
                    ms.Write(Record.Data, 4, Record.Data.Length - 4);

                ms.Seek(0, SeekOrigin.Begin);
                AsCursor = new Cursor(ms);
            }
        }

        /// <summary>
        /// Frees loaded handles from memory.
        /// </summary>
        protected virtual void FreeHandles(IntPtr hMod)
        {
            NativeMethods.FreeLibrary(hMod);
        }
        #endregion
    }
}
