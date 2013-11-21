//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;

using System.Globalization;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Threading.Tasks;


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
			this.cssFileName = string.Concat (rootfolder, IconManager.baseCssFileName);
			this.cssLines = new ConcurrentQueue<string> ();
			this.imagesFileNamePattern = string.Concat (rootfolder, IconManager.baseImagesFileNamePattern);
		}


		public static void BuildIcons(string rootfolder)
		{
			var manager = new IconManager (rootfolder);
			
			manager.BuildIcons ();
		}


		public static string GetCssClassName(string iconUri, IconSize size, System.Type entityType = null)
		{
			if (iconUri == null)
			{
				return null;
			}

			var iconResource = Misc.IconProvider.GetResourceIconUri (iconUri, entityType);

			return IconManager.GetCssClassName (iconResource, size);
		}


		private static string GetCssClassName(string iconResource, IconSize size)
		{
			var iconName = iconResource.Substring (9).Replace ('.', '-').ToLower (CultureInfo.InvariantCulture);

			return string.Format (CultureInfo.InvariantCulture, IconManager.cssClassNamePattern, iconName, size);
		}


		private void BuildIcons()
		{
			Epsitec.Common.Document.Engine.Initialize ();

			File.Delete (this.cssFileName);

			var iconUris = ImageProvider.Instance.GetImageNames ("manifest", null);

			//	I'd love to Parallel.ForEach this piece of code, however the icon generation is
			//	not thread safe:

			foreach (var uri in iconUris)
			{
				this.CreateIcon (uri);
			}
			
			var cssLines = this.cssLines.ToArray ().OrderBy (x => x);
			
			File.AppendAllLines (this.cssFileName, cssLines);
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
			File.WriteAllBytes (path, bytes);

			// Add it to the CSS
			var relativePath = this.GetImageRelativeFilePath (iconName, IconSize.ThirtyTwo);
			this.AddToCSS (iconUri, relativePath, IconSize.ThirtyTwo);

			// Save in 16
			icon.DefineZoom (0.5);
			bitmap = icon.BitmapImage;

			// Save the image
			bytes = bitmap.Save (ImageFormat.Png);
			path = this.GetImageAbsoluteFilePath (iconName, IconSize.Sixteen);
			File.WriteAllBytes (path, bytes);

			// Add it to the CSS
			relativePath = GetImageRelativeFilePath (iconName, IconSize.Sixteen);
			this.AddToCSS (iconUri, relativePath, IconSize.Sixteen);
		}


		private void AddToCSS(string iconUri, string path, IconSize size)
		{
			var cssClassname = IconManager.GetCssClassName (iconUri, size);
			var imagePath = string.Concat ("../", path);
			var css = string.Format (CultureInfo.InvariantCulture, IconManager.cssClassBodyPattern, cssClassname, imagePath);

			this.cssLines.Enqueue (css);
		}


		private string GetImageAbsoluteFilePath(string name, IconSize size)
		{
			var path = string.Format (CultureInfo.InvariantCulture, this.imagesFileNamePattern, name.Replace ('.', '/'), size);

			IconManager.EnsureDirectoryStructureExists (path);

			return path;
		}


		private string GetImageRelativeFilePath(string name, IconSize size)
		{
			return string.Format (CultureInfo.InvariantCulture, IconManager.baseImagesFileNamePattern, name.Replace ('.', '/'), size);
		}


		private static void EnsureDirectoryStructureExists(string path)
		{
			var directory = System.IO.Path.GetDirectoryName (path);

			if (Directory.Exists (directory) == false)
			{
				Directory.CreateDirectory (directory);
			}
		}


		private readonly string cssFileName;
		private readonly string imagesFileNamePattern;
		private readonly ConcurrentQueue<string> cssLines;

		private readonly static string baseCssFileName = "css/icons.css";
		private readonly static string baseImagesFileNamePattern = "images/{0}{1:d}.png";
		private readonly static string cssClassNamePattern = "{0}{1:d}";
		private readonly static string cssClassBodyPattern = ".{0} {{ background-image: url({1}) !important; }} \n";


	}


}
