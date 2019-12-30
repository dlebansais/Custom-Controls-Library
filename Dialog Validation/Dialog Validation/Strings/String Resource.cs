using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CustomControls
{
    /// <summary>
    /// Represents a list of string resources loaded from a files.
    /// </summary>
    internal class StringResource
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="StringResource"/> class.
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
        /// Loads the string resources.
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
        /// Path to the file resources are loaded from.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Identifier used to find and load resources in the file.
        /// </summary>
        public uint ResourceID { get; private set; }

        /// <summary>
        /// Loaded string resources.
        /// </summary>
        public IList<string> AsStrings { get; private set; }
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
        /// Load the string resources and fill the AsStrings property.
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
        /// Frees loaded handles from memory.
        /// </summary>
        protected virtual void FreeHandles(IntPtr hMod)
        {
            NativeMethods.FreeLibrary(hMod);
        }
        #endregion
    }
}
