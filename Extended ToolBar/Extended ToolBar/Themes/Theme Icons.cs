﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CustomControls
{
    /// <summary>
    /// Gets image sources and icons from resources in an assembly.
    /// </summary>
    public static class ThemeIcons
    {
        /// <summary>
        /// Gets an image sources from resources in an assembly.
        /// </summary>
        /// <param name="resourceAssembly">The assembly where to find the image source.</param>
        /// <param name="key">The key used to locate the resource in the assembly.</param>
        public static ImageSource GetImageSource(Assembly resourceAssembly, string key)
        {
            return GetImageSource(resourceAssembly, "/Resources/Icons", key);
        }

        /// <summary>
        /// Gets an image sources from resources in an assembly.
        /// </summary>
        /// <param name="resourceAssembly">The assembly where to find the image source.</param>
        /// <param name="iconPath">The path to resources in the assembly.</param>
        /// <param name="key">The key used to locate the resource in the assembly.</param>
        public static ImageSource GetImageSource(Assembly resourceAssembly, string iconPath, string key)
        {
            if (resourceAssembly == null)
                throw new ArgumentNullException(nameof(resourceAssembly));

            string AssemblyName = resourceAssembly.GetName().Name;
            string UriPath = "pack://application:,,,/" + AssemblyName + ";component" + iconPath + "/" + key + ".png";

            try
            {
                BitmapImage ImageResource = new BitmapImage(new Uri(UriPath));
                return ImageResource;
            }
            catch (IOException e)
            {
                Debug.Print(e.Message);
                Debug.Print("Icon Path: " + UriPath);
                throw new ArgumentOutOfRangeException(nameof(key));
            }
        }

        /// <summary>
        /// Gets an icon from resources in an assembly.
        /// </summary>
        /// <param name="resourceAssembly">The assembly where to find the icon.</param>
        /// <param name="key">The key used to locate the resource in the assembly.</param>
        public static Image GetIcon(Assembly resourceAssembly, string key)
        {
            return GetIcon(resourceAssembly, "/Resources/Icons", key);
        }

        /// <summary>
        /// Gets an icon from resources in an assembly.
        /// </summary>
        /// <param name="resourceAssembly">The assembly where to find the icon.</param>
        /// <param name="iconPath">The path to resources in the assembly.</param>
        /// <param name="key">The key used to locate the resource in the assembly.</param>
        public static Image GetIcon(Assembly resourceAssembly, string iconPath, string key)
        {
            try
            {
                ImageSource ImageResource = GetImageSource(resourceAssembly, iconPath, key);
                return new Image { Source = ImageResource, Width = 16.0, Height = 16.0 };
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.Message);
                throw new ArgumentOutOfRangeException(nameof(key));
            }
        }
    }
}
