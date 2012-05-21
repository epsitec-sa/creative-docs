using Epsitec.Common.Drawing;

using Epsitec.Common.Support;

using Epsitec.Cresus.Core;

using System;

using System.Globalization;

using System.IO;


namespace Epsitec.Cresus.WebCore.Server.CoreServer
{
	
	
	/// <summary>
	/// Used to write every available *.icon into their PNG equivalent so that they are available
	/// through the web server.
	/// It also creates a CSS file to be able to call the icons in the JS code.
	/// </summary>
	internal sealed class IconManager
	{


		private IconManager(string rootfolder)
		{
			this.cssFileName = string.Concat (rootfolder, IconManager.baseCssFileName);
			this.imagesFileNamePattern = string.Concat (rootfolder, IconManager.baseImagesFileNamePattern);
		}


		public static void BuildIcons(string rootfolder)
		{
			new IconManager (rootfolder).BuildIcons ();
		}


		public static string GetCssClassName(Type entityType, string iconUri, IconSize size)
		{
			if (iconUri == null)
			{
				return null;
			}

			var iconResource = Misc.GetResourceIconUri (iconUri, entityType);

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

			var iconUris = ImageProvider.Default.GetImageNames ("manifest", null);

			foreach (var iconUri in iconUris)
			{
				this.CreateIcon (iconUri);
			}
		}


		/// <summary>
		/// Create an image using a brick
		/// </summary>
		/// <param name="brick">Brick to use</param>
		/// <returns>Key/value pair with the icon name and the filename</returns>
		private void CreateIcon(string iconUri)
		{
			if (iconUri == null)
			{
				return;
			}

			// Get the ressource from the icon name
			var iconResource = Misc.GetResourceIconUri (iconUri);
			var iconName = iconResource.Substring (9); // remove "manifest:"
			var icon = ImageProvider.Default.GetImage (iconResource, Resources.DefaultManager) as Canvas;

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


		/// <summary>
		/// Create CSS content to be able to call an icon from the HTML code
		/// </summary>
		/// <param name="iconName">Name of the icon (ressource)</param>
		/// <param name="path">Icon filename</param>
		/// <returns>CSS classname</returns>
		private void AddToCSS(string iconUri, string path, IconSize size)
		{
			var cssClassname = IconManager.GetCssClassName (iconUri, size);
			var imagePath = string.Concat ("../", path);
			var css = string.Format (CultureInfo.InvariantCulture, IconManager.cssClassBodyPattern, cssClassname, imagePath);

			File.AppendAllText (this.cssFileName, css);
		}


		/// <summary>
		/// Get the filename of an image
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
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


		/// <summary>
		/// checks if the folder exists, otherwise creates it
		/// </summary>
		/// <param name="path"></param>
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

		private readonly static string baseCssFileName = "css/icons.css";
		private readonly static string baseImagesFileNamePattern = "images/{0}{1:d}.png";
		private readonly static string cssClassNamePattern = "{0}{1:d}";
		private readonly static string cssClassBodyPattern = ".{0} {{ background-image: url({1}) !important; }} \n";


	}


}