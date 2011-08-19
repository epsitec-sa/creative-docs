
// Uncomment to regenerate all icons
//#define BUILD_ICONS

using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Server
{
	internal class IconsBuilder
	{

		public static void BuildIcons(string rootfolder)
		{
#if BUILD_ICONS

			var builder = new IconsBuilder (rootfolder);
			builder.Run ();
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

		private IconsBuilder(string rootfolder)
		{
			this.cssFilename = string.Concat (rootfolder, IconsBuilder.baseCssFilename);
			this.imagesFilename = string.Concat (rootfolder, IconsBuilder.baseImagesFilename);
		}

		private void Run()
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
			var cssClassname = IconsBuilder.GetCSSClassName (iconUri, size);
			string imgPath = string.Concat ("../", path);
			var css = string.Format (IconsBuilder.cssClass, cssClassname, imgPath);

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
			IconsBuilder.EnsureDirectoryStructureExists (path);
			return path;
		}

		private string GetImageRelativeFilePath(string name, IconSize size)
		{
			var path = string.Format (IconsBuilder.baseImagesFilename, name.Replace ('.', '/'), size);
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

	internal enum IconSize
	{
		Sixteen = 16,
		ThirtyTwo = 32
	}
}
