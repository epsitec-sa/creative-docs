//	Copyright © 2012-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;

using System.Globalization;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Core
{
	/// <summary>
	/// This class is used to convert every ICON file that might be used in the application into
	/// their equivalent PNG file. It creates a CSS file with classes that allow for easy access by
	/// the javascript client.
	/// Moreover, it provides the server with methods that lets it references those classes in the
	/// layout engine.
	/// </summary>
	internal sealed class IconManager
	{
		private IconManager(string rootfolder)
		{
            this.cssClassNames = new HashSet<string> ();
			this.cssFileName = string.Concat (rootfolder, IconManager.baseCssFileName);
			this.cssLines = new ConcurrentQueue<string> ();
			this.imagesFileNamePattern = string.Concat (rootfolder, IconManager.baseImagesFileNamePattern);
		}


		public static void BuildIcons(string rootfolder)
		{
            if (IconManager.instance == null)
            {
                var manager = new IconManager (rootfolder);
                manager.BuildIcons ();

                IconManager.instance = manager;
            }
		}


		public static string GetCssClassName(string iconUri, IconSize size, System.Type entityType = null)
		{
			if (string.IsNullOrEmpty (iconUri))
			{
				return null;
			}

			var iconResource = Misc.IconProvider.GetResourceIconUri (iconUri, entityType);
			var cssClassName = IconManager.GetCssClassName (iconResource, size);

            if ((IconManager.instance != null) &&
                (IconManager.instance.cssClassNames.Contains (cssClassName) == false))
            {
                var images = "-images-";
                var imagesPos = cssClassName.IndexOf (images);
                if (imagesPos > 0)
                {
                    var match = cssClassName.Substring (imagesPos);
                    var found = IconManager.instance.cssClassNames.FirstOrDefault (x => x.EndsWith (match));

                    if (found != null)
                    {
                        //  Silently subsitute image found in another namespace...
                        return found;
                    }
                }


                System.Diagnostics.Trace.WriteLine (string.Format ("Cannot resolve CSS class {0}", cssClassName));
            }

            return cssClassName;
		}


		private static string GetCssClassName(string iconResource, IconSize size)
		{
			var iconName = iconResource.Substring (9).Replace ('.', '-').ToLower (CultureInfo.InvariantCulture);

			return string.Format (CultureInfo.InvariantCulture, IconManager.cssClassNamePattern, iconName, size);
		}


		private void BuildIcons()
		{
			Epsitec.Common.Document.Engine.Initialize ();

            System.IO.File.Delete (this.cssFileName);

			var iconUris = ImageProvider.Instance.GetImageNames ("manifest", null);

			//	I'd love to Parallel.ForEach this piece of code, however the icon generation is
			//	not thread safe:

			foreach (var uri in iconUris)
			{
				this.CreateIcon (uri);
			}
			
			var cssLines = this.cssLines.ToArray ().OrderBy (x => x);

            System.IO.File.AppendAllLines (this.cssFileName, cssLines);
		}


		private void CreateIcon(string iconUri)
		{
			if (iconUri == null)
			{
				return;
			}

			// Get the ressource from the icon name
			var iconResource = Misc.IconProvider.GetResourceIconUri (iconUri);
			var iconName = iconResource.Substring (9); // remove "manifest:"
			var icon = ImageProvider.Instance.GetImage (iconResource, Resources.DefaultManager) as Canvas;

			if (icon == null)
			{
				return;
			}

			Canvas.IconKey key = new Canvas.IconKey ();

			key.Size.Width  = 32;
			key.Size.Height = 32;
			icon = icon.GetImageForIconKey (key) as Canvas;

			// Save in 32
			var bitmap = icon.BitmapImage;

			// Save the image
			var bytes = bitmap.Save (ImageFormat.Png);
			var path = this.GetImageAbsoluteFilePath (iconName, IconSize.ThirtyTwo);
			System.IO.File.WriteAllBytes (path, bytes);

			// Add it to the CSS
			var relativePath = this.GetImageRelativeFilePath (iconName, IconSize.ThirtyTwo);
			this.AddToCSS (iconUri, relativePath, IconSize.ThirtyTwo);

			
			
			key = new Canvas.IconKey ();

			key.Size.Width  = 16;
			key.Size.Height = 16;
			
			var icon16 = icon.GetImageForIconKey (key) as Canvas;

			if ((icon16 == null) ||
				(icon16.Width != 16))
			{
				// Generate 16x16 base on scaled down 32x32
				icon.DefineZoom (0.5);
				bitmap = icon.BitmapImage;
			}
			else
			{
				bitmap = icon16.BitmapImage;
			}

			// Save the image
			bytes = bitmap.Save (ImageFormat.Png);
			path = this.GetImageAbsoluteFilePath (iconName, IconSize.Sixteen);
			System.IO.File.WriteAllBytes (path, bytes);

			// Add it to the CSS
			relativePath = this.GetImageRelativeFilePath (iconName, IconSize.Sixteen);
			this.AddToCSS (iconUri, relativePath, IconSize.Sixteen);
		}


		private void AddToCSS(string iconUri, string path, IconSize size)
		{
			var cssClassName = IconManager.GetCssClassName (iconUri, size);
			var imagePath = string.Concat ("../", path);
			var css = string.Format (CultureInfo.InvariantCulture, IconManager.cssClassBodyPattern, cssClassName, imagePath);

			this.cssLines.Enqueue (css);
            this.cssClassNames.Add (cssClassName);
		}


		private string GetImageAbsoluteFilePath(string name, IconSize size)
		{
			var path = string.Format (CultureInfo.InvariantCulture, this.imagesFileNamePattern, name.Replace ('.', '-').ToLowerInvariant (), size);

			IconManager.EnsureDirectoryStructureExists (path);

			return path;
		}


		private string GetImageRelativeFilePath(string name, IconSize size)
		{
			return string.Format (CultureInfo.InvariantCulture, IconManager.baseImagesFileNamePattern, name.Replace ('.', '-').ToLowerInvariant (), size);
		}


		private static void EnsureDirectoryStructureExists(string path)
		{
			var directory = System.IO.Path.GetDirectoryName (path);

			if (System.IO.Directory.Exists (directory) == false)
			{
                System.IO.Directory.CreateDirectory (directory);
			}
		}



        private static IconManager instance;

		private readonly string cssFileName;
		private readonly string imagesFileNamePattern;

		private readonly ConcurrentQueue<string> cssLines;
        private readonly HashSet<string> cssClassNames;

		private readonly static string baseCssFileName = "css/icons.css";
		private readonly static string baseImagesFileNamePattern = "images/{0}{1:d}.png";
		private readonly static string cssClassNamePattern = "{0}{1:d}";
		private readonly static string cssClassBodyPattern = ".{0} {{ background-image: url({1}) !important; }} \n";


	}


}
