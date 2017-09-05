using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CustomControls
{
    /// <summary>
    ///     Contains unmanaged methods to read resources from a file.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        ///     Flag to indicate the file should be loaded as data and not as executable code.
        /// </summary>
        public const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        /// <summary>
        ///     Flag to indicate a string resource.
        /// </summary>
        public const uint RT_STRING = 6;

        /// <summary>
        ///     Loads a DLL in memory
        /// </summary>
        /// <parameters>
        /// <param name="lpFileName">Path to the file to load.</param>
        /// <param name="hFile">This parameter is not used.</param>
        /// <param name="dwFlags">If LOAD_LIBRARY_AS_DATAFILE is specified, loads the file as data rather than executable code.</param>
        /// </parameters>
        /// <returns>
        ///     A handle to the loaded file.
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadLibraryEx([MarshalAs(UnmanagedType.LPWStr)]string lpFileName, IntPtr hFile, UInt32 dwFlags);

        /// <summary>
        ///     Looks for a resource by its identifier in a file.
        /// </summary>
        /// <parameters>
        /// <param name="hModule">Handle of the loaded file.</param>
        /// <param name="lpID">Identifier of the resource to find.</param>
        /// <param name="lpType">Type of the resource.</param>
        /// </parameters>
        /// <returns>
        ///     A handle to the resource.
        /// </returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpID, IntPtr lpType);

        /// <summary>
        ///     Loads a resource in memory
        /// </summary>
        /// <parameters>
        /// <param name="hModule">Handle of the loaded file.</param>
        /// <param name="hResInfo">Handle to the resource to load.</param>
        /// </parameters>
        /// <returns>
        ///     A handle to the block loaded in memory.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        /// <summary>
        ///     Gets the size of a resource
        /// </summary>
        /// <parameters>
        /// <param name="hModule">Handle of the loaded file.</param>
        /// <param name="hResInfo">Handle to the resource to load.</param>
        /// </parameters>
        /// <returns>
        ///     The size of the resource, in bytes.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        /// <summary>
        ///     Frees a loaded DLL from memory.
        /// </summary>
        /// <parameters>
        /// <param name="hMod">Handle of the library to free.</param>
        /// </parameters>
        /// <returns>
        ///     The returned value can be ignored.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hMod);
    }

    /// <summary>
    ///     Represents a list of string resources loaded from a files.
    /// </summary>
    internal class StringResource
    {
        #region Init
        /// <summary>
        ///     Initializes a new instance of the <see cref="StringResource"/> class.
        /// </summary>
        /// <parameters>
        /// <param name="FilePath">Path to the file to read.</param>
        /// <param name="ResourceID">Identifier of the resources.</param>
        /// </parameters>
        public StringResource(string FilePath, uint ResourceID)
        {
            this.FilePath = FilePath;
            this.ResourceID = ResourceID;

            AsStrings = new List<string>();
        }

        /// <summary>
        ///     Loads the string resources.
        /// </summary>
        public void Load()
        {
            IntPtr hMod = LoadFile();
            if (hMod != IntPtr.Zero)
            {
                LoadStringValues(hMod);
                FreeHandles(hMod);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Path to the file resources are loaded from.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        ///     Identifier used to find and load resources in the file.
        /// </summary>
        public uint ResourceID { get; private set; }

        /// <summary>
        ///     Loaded string resources.
        /// </summary>
        public IList<string> AsStrings { get; private set; }
        #endregion

        #region Implementation
        /// <summary>
        ///     Load the file containing the resources in memory.
        /// </summary>
        protected virtual IntPtr LoadFile()
        {
            IntPtr hMod = NativeMethods.LoadLibraryEx(FilePath, IntPtr.Zero, NativeMethods.LOAD_LIBRARY_AS_DATAFILE);
            return hMod;
        }

        /// <summary>
        ///     Load the string resources and fill the AsStrings property.
        /// </summary>
        protected virtual bool LoadStringValues(IntPtr hMod)
        {
            IntPtr hResDir = NativeMethods.FindResource(hMod, (IntPtr)ResourceID, (IntPtr)NativeMethods.RT_STRING);
            if (hResDir == IntPtr.Zero)
                return false;

            uint size = NativeMethods.SizeofResource(hMod, hResDir);
            IntPtr pt = NativeMethods.LoadResource(hMod, hResDir);

            byte[] bPtr = new byte[size];
            Marshal.Copy(pt, bPtr, 0, (int)size);

            List<string> Values = new List<string>();

            int Offset = 0;
            while (Offset + 2 < size)
            {
                ushort Length = BitConverter.ToUInt16(bPtr, Offset);
                Offset += 2;

                string Value = "";
                for (int j = 0; j < Length && Offset + 2 < size; j++)
                {
                    Value += BitConverter.ToChar(bPtr, Offset);
                    Offset += 2;
                }

                Value = Value.Replace('&', '_');
                Values.Add(Value);
            }

            AsStrings.Clear();
            for (int i = 0; i < Values.Count; i++)
                AsStrings.Add(Values[i]);

            return true;
        }

        /// <summary>
        ///     Frees loaded handles from memory.
        /// </summary>
        protected virtual void FreeHandles(IntPtr hMod)
        {
            NativeMethods.FreeLibrary(hMod);
        }
        #endregion
    }
}
