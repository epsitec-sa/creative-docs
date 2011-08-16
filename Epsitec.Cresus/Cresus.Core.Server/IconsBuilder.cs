
// Uncomment to regenerate all icons
//#define BUILD_ICONS

using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Linq;

namespace Epsitec.Cresus.Core.Server
{
	internal static class IconsBuilder
	{

		public static void BuildIcons()
		{
#if BUILD_ICONS

			Epsitec.Common.Document.Engine.Initialize ();

			System.IO.File.Delete (IconsBuilder.cssFilename);

			var list = ImageProvider.Default.GetImageNames ("manifest", null);

			list.ToList ().ForEach (IconsBuilder.CreateIcons);
#endif
		}

		public static string GetCSSClassName(string iconUri, IconSize size)
		{
			if (iconUri == null)
			{
				return null;
			}

			var iconRes = Misc.GetResourceIconUri (iconUri);
			var iconName = iconRes.Substring (9);

			return string.Format (IconsBuilder.cssClassName, iconName.Replace ('.', '-').ToLower (), size);
		}
		/// <summary>
		/// Create an image using a brick
		/// </summary>
		/// <param name="brick">Brick to use</param>
		/// <returns>Key/value pair with the icon name and the filename</returns>
		private static void CreateIcons(string iconUri)
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
			string path = IconsBuilder.GetImageFilePath (iconName, IconSize.ThirtyTwo);
			System.IO.File.WriteAllBytes (path, bytes);

			// Add it to the CSS
			IconsBuilder.AddToCSS (iconUri, path, IconSize.ThirtyTwo);


			// Save in 16
			icon.DefineZoom (0.5);
			bitmap = icon.BitmapImage;

			// Save the image
			bytes = bitmap.Save (ImageFormat.Png);
			path = IconsBuilder.GetImageFilePath (iconName, IconSize.Sixteen);
			System.IO.File.WriteAllBytes (path, bytes);

			// Add it to the CSS
			IconsBuilder.AddToCSS (iconUri, path, IconSize.Sixteen);
		}

		/// <summary>
		/// Create CSS content to be able to call an icon from the HTML code
		/// </summary>
		/// <param name="iconName">Name of the icon (ressource)</param>
		/// <param name="path">Icon filename</param>
		/// <returns>CSS classname</returns>
		private static void AddToCSS(string iconUri, string path, IconSize size)
		{
			var cssClassname = IconsBuilder.GetCSSClassName (iconUri, size);
			string imgPath = path.Replace ("web", "..");
			var css = string.Format (IconsBuilder.cssClass, cssClassname, imgPath);

			System.IO.File.AppendAllText (IconsBuilder.cssFilename, css);
		}
		/// <summary>
		/// Get the filename of an image
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetImageFilePath(string name, IconSize size)
		{
			var path = string.Format (IconsBuilder.imagesFilename, name.Replace ('.', '/'), size);
			IconsBuilder.EnsureDirectoryStructureExists (path);
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


		// Generated files filenames
		private readonly static string cssFilename = "web/css/icons.css";
		private readonly static string imagesFilename = "web/images/{0}{1:d}.png";
		private readonly static string cssClassName = "{0}{1:d}";
		private readonly static string cssClass = ".{0} {{ background-image: url({1}) !important; }} \n";
	}

	public enum IconSize
	{
		Sixteen = 16,
		ThirtyTwo = 32
	}
}
