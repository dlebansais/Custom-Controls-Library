using System;
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
        ///     Cached location of the assembly.
        /// </summary>
        private Assembly InitResourceAssembly;
        /// <summary>
        ///     Cached location of a type used to find resources in the assembly.
        /// </summary>
        private Type InitResourceSource;
        /// <summary>
        ///     Cached location of a <see cref="ResourceManager"/> used to find strings in the assembly.
        /// </summary>
        private ResourceManager InitResourceManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandResourceReference"/> class.
        /// </summary>
        public CommandResourceReference()
        {
            InitResourceAssembly = null;
            InitResourceSource = null;
            InitResourceManager = null;
        }

        /// <summary>
        ///     Fills the cache when called for the first times. Does nothing on subsequent calls.
        /// </summary>
        private void Initialize()
        {
            if (InitResourceAssembly == null)
            {
                AssemblyName InitAssemblyName = new AssemblyName(AssemblyName);
                InitResourceAssembly = Assembly.Load(InitAssemblyName);
            }

            if (InitResourceSource == null && InitResourceAssembly != null)
            {
                string[] ResourceNames = InitResourceAssembly.GetManifestResourceNames();

                foreach (string ResourceName in ResourceNames)
                    if (ResourceName.EndsWith(ResourceExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        InitResourceSource = InitResourceAssembly.GetType(ResourceName.Substring(0, ResourceName.Length - ResourceExtension.Length));
                        if (InitResourceSource != null)
                            break;
                    }
            }

            if (InitResourceManager == null && InitResourceSource != null)
                InitResourceManager = new ResourceManager(InitResourceSource);
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets or sets the name of the assembly.
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        ///     Gets or sets the extension of resource files to find within the assembly.
        /// </summary>
        public string ResourceExtension { get; set; }
        /// <summary>
        ///     Gets or sets the path to icon resources within the assembly.
        /// </summary>
        public string IconPath { get; set; }
        #endregion

        #region Client Interface
        /// <summary>
        ///     Gets a localized string from the assembly by its resource name.
        /// </summary>
        public string GetString(string name)
        {
            if (name == null)
                return null;

            Initialize();

            if (InitResourceManager != null)
                return InitResourceManager.GetString(name, CultureInfo.CurrentCulture);
            else
                return null;
        }

        /// <summary>
        ///     Gets a localized image source from the assembly by its resource name.
        /// </summary>
        public ImageSource GetImageSource(string name)
        {
            Initialize();

            if (name == null)
                return null;

            if (InitResourceAssembly == null)
                return null;

            string AssemblyName = InitResourceAssembly.GetName().Name;
            string UriPath = "pack://application:,,,/" + AssemblyName + ";component/" + IconPath + name;

            try
            {
                BitmapImage ImageResource = new BitmapImage(new Uri(UriPath));
                return ImageResource;
            }
            catch (IOException)
            {
                return null;
            }
        }
        #endregion
    }
}
