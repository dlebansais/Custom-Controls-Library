﻿using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CustomControls
{
    /// <summary>
    ///     Represents an object that can extract string and icon resources from an assembly specified by name.
    /// </summary>
    public class CommandResourceReference
    {
        #region Init
        /// <summary>
        /// The default <see cref="CommandResourceReference"/>.
        /// </summary>
        public static CommandResourceReference Default { get; } = new CommandResourceReference();

        /// <summary>
        ///     Fills the cache when called for the first times. Does nothing on subsequent calls.
        /// </summary>
        private void Initialize()
        {
            if (InitResourceSource == typeof(object))
            {
                AssemblyName InitAssemblyName = new AssemblyName(AssemblyName);
                InitResourceAssembly = Assembly.Load(InitAssemblyName);

                string[] ResourceNames = InitResourceAssembly.GetManifestResourceNames();

                foreach (string ResourceName in ResourceNames)
                    if (ResourceName.EndsWith(ResourceExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        InitResourceSource = InitResourceAssembly.GetType(ResourceName.Substring(0, ResourceName.Length - ResourceExtension.Length));
                        if (InitResourceSource != null)
                            break;
                    }

                InitResourceManager = new ResourceManager(InitResourceSource);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets or sets the name of the assembly.
        /// </summary>
        public string AssemblyName { get; set; } = string.Empty;
        /// <summary>
        ///     Gets or sets the extension of resource files to find within the assembly.
        /// </summary>
        public string ResourceExtension { get; set; } = string.Empty;
        /// <summary>
        ///     Gets or sets the path to icon resources within the assembly.
        /// </summary>
        public string IconPath { get; set; } = string.Empty;
        #endregion

        #region Client Interface
        /// <summary>
        ///     Gets a localized string from the assembly by its resource name.
        /// </summary>
        public string GetString(string name)
        {
            Initialize();
            return InitResourceManager.GetString(name, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Gets a localized image source from the assembly by its resource name.
        /// </summary>
        public ImageSource GetImageSource(string name)
        {
            Initialize();

            string AssemblyName = InitResourceAssembly.GetName().Name;
            string UriPath = "pack://application:,,,/" + AssemblyName + ";component/" + IconPath + name;

            try
            {
                BitmapImage ImageResource = new BitmapImage(new Uri(UriPath));
                return ImageResource;
            }
            catch (IOException)
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        ///     Cached location of the assembly.
        /// </summary>
        private Assembly InitResourceAssembly = Assembly.GetExecutingAssembly();
        /// <summary>
        ///     Cached location of a type used to find resources in the assembly.
        /// </summary>
        private Type InitResourceSource = typeof(object);
        /// <summary>
        ///     Cached location of a <see cref="ResourceManager"/> used to find strings in the assembly.
        /// </summary>
        private ResourceManager InitResourceManager = new ResourceManager(typeof(object));
        #endregion
    }
}
