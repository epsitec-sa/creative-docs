//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -


using Epsitec.Common.Drawing;

using Epsitec.Common.Support;

using System.Linq;


namespace Epsitec.Cresus.Core.Server.CoreServer
{
	
	
	/// <summary>
	/// Used to write every available *.icon into their PNG equivalent so that they are available
	/// through the web server.
	/// It also creates a CSS file to be able to call the icons in the JS code.
	/// Uncomment the "define" to regenerate all icons at server startup.
	/// It takes time !
	/// </summary>
	internal sealed class IconManager
	{


		private IconManager(string rootfolder)
		{
			this.cssFilename = string.Concat (rootfolder, IconManager.baseCssFilename);
			this.imagesFilename = string.Concat (rootfolder, IconManager.baseImagesFilename);
		}


		public static void BuildIcons(string rootfolder)
		{
			new IconManager (rootfolder).BuildIcons ();
		}


		public static string GetCSSClassName(string iconUri, IconSize size)
		{
			if (iconUri == null)
			{
				return null;
			}

			var iconRes = Misc.GetResourceIconUri (iconUri);
			var iconName = iconRes.Substring (9);

			return string.Format (IconManager.cssClassName, iconName.Replace ('.', '-').ToLower (), size);
		}


		private void BuildIcons()
		{
			Epsitec.Common.Document.Engine.Initialize ();

			System.IO.File.Delete (this.cssFilename);

			var list = ImageProvider.Default.GetImageNames ("manifest", null);

			list.ToList ().ForEach (CreateIcons);
		}


		/// <summary>
		/// Create an image using a brick
		/// </summary>
		/// <param name="brick">Brick to use</param>
		/// <returns>Key/value pair with the icon name and the filename</returns>
		private void CreateIcons(string iconUri)
		{
			if (iconUri == null)
			{
				return;
			}

			// Get the ressource from the icon name
			var iconRes = Misc.GetResourceIconUri (iconUri);
			var iconName = iconRes.Substring (9); // remove "manifest:"
			var icon = ImageProvider.Default.GetImage (iconRes, Resources.DefaultManager) as Canvas;

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
			string path = GetImageAbsoluteFilePath (iconName, IconSize.ThirtyTwo);
			System.IO.File.WriteAllBytes (path, bytes);

			// Add it to the CSS
			var relativePath = GetImageRelativeFilePath (iconName, IconSize.ThirtyTwo);
			AddToCSS (iconUri, relativePath, IconSize.ThirtyTwo);


			// Save in 16
			icon.DefineZoom (0.5);
			bitmap = icon.BitmapImage;

			// Save the image
			bytes = bitmap.Save (ImageFormat.Png);
			path = GetImageAbsoluteFilePath (iconName, IconSize.Sixteen);
			System.IO.File.WriteAllBytes (path, bytes);

			// Add it to the CSS
			relativePath = GetImageRelativeFilePath (iconName, IconSize.Sixteen);
			AddToCSS (iconUri, relativePath, IconSize.Sixteen);
		}


		/// <summary>
		/// Create CSS content to be able to call an icon from the HTML code
		/// </summary>
		/// <param name="iconName">Name of the icon (ressource)</param>
		/// <param name="path">Icon filename</param>
		/// <returns>CSS classname</returns>
		private void AddToCSS(string iconUri, string path, IconSize size)
		{
			var cssClassname = IconManager.GetCSSClassName (iconUri, size);
			string imgPath = string.Concat ("../", path);
			var css = string.Format (IconManager.cssClass, cssClassname, imgPath);

			System.IO.File.AppendAllText (this.cssFilename, css);
		}


		/// <summary>
		/// Get the filename of an image
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private string GetImageAbsoluteFilePath(string name, IconSize size)
		{
			var path = string.Format (this.imagesFilename, name.Replace ('.', '/'), size);
			IconManager.EnsureDirectoryStructureExists (path);
			return path;
		}


		private string GetImageRelativeFilePath(string name, IconSize size)
		{
			var path = string.Format (IconManager.baseImagesFilename, name.Replace ('.', '/'), size);
			return path;
		}


		/// <summary>
		/// checks if the folder exists, otherwise creates it
		/// </summary>
		/// <param name="path"></param>
		private static void EnsureDirectoryStructureExists(string path)
		{
			var dir  = System.IO.Path.GetDirectoryName (path);

			if (System.IO.Directory.Exists (dir) == false)
			{
				System.IO.Directory.CreateDirectory (dir);
			}
		}


		private readonly string cssFilename;
		private readonly string imagesFilename;

		private readonly static string baseCssFilename = "css/icons.css";
		private readonly static string baseImagesFilename = "images/{0}{1:d}.png";
		private readonly static string cssClassName = "{0}{1:d}";
		private readonly static string cssClass = ".{0} {{ background-image: url({1}) !important; }} \n";
	}


}