namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Input;

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

        /// <summary>
        /// Header of a resource file.
        /// </summary>
        public class FileHeader
        {
#pragma warning disable SA1600 // Elements should be documented
            public short Reserved { get; set; }
            public short Type { get; set; }
            public short ImageCount { get; set; }
#pragma warning restore SA1600 // Elements should be documented
        }

        /// <summary>
        /// Record in a resource file.
        /// </summary>
        public class FileRecord
        {
#pragma warning disable SA1600 // Elements should be documented
            public byte bWidth { get; set; }
            public byte bHeight { get; set; }
            public byte bColorCount { get; set; }
            public byte bReserved { get; set; }
            public ushort nID { get; set; }
            public byte[] Data { get; set; } = Array.Empty<byte>();
            public ushort HotspotX { get; set; }
            public ushort HotspotY { get; set; }
#pragma warning restore SA1600 // Elements should be documented
        }

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
        public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        /// <summary>
        /// Frees a loaded DLL from memory.
        /// </summary>
        /// <param name="hMod">Handle of the library to free.</param>
        /// <returns>
        /// The returned value can be ignored.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hMod);
    }
}
