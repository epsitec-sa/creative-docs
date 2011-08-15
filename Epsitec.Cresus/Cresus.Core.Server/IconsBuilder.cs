
//#define BUILD_ICONS

using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Core.Server
{
	internal static class IconsBuilder
	{

		public static void BuildIcons()
		{

#if BUILD_ICONS

			Epsitec.Common.Document.Engine.Initialize ();

			System.IO.File.Delete (IconsBuilder.cssFilename);

			IconsBuilder.CreateIcon ("manifest:Epsitec.Cresus.Core.Images.Data.Affair.icon");
			IconsBuilder.CreateIcon ("manifest:Epsitec.Cresus.Core.Images.Data.Customer.icon");
			IconsBuilder.CreateIcon ("manifest:Epsitec.Cresus.Core.Images.Data.Document.icon");
			IconsBuilder.CreateIcon ("manifest:Epsitec.Cresus.Core.Images.Data.MailContact.icon");
			IconsBuilder.CreateIcon ("manifest:Epsitec.Cresus.Core.Images.Data.TelecomContact.icon");
			IconsBuilder.CreateIcon ("manifest:Epsitec.Cresus.Core.Images.Data.UriContact.icon");
			IconsBuilder.CreateIcon ("manifest:Epsitec.Cresus.Core.Images.Data.NaturalPerson.icon");
			IconsBuilder.CreateIcon ("manifest:Epsitec.Cresus.Core.Images.Data.LegalPerson.icon");
#endif

		}

		public static string GetCSSName(string iconUri)
		{
			if (iconUri == null)
			{
				return null;
			}

			var iconRes = Misc.GetResourceIconUri (iconUri);
			var iconName = iconRes.Substring (9);

			return iconName.Replace ('.', '-').ToLower ();
		}
		/// <summary>
		/// Create an image using a brick
		/// </summary>
		/// <param name="brick">Brick to use</param>
		/// <returns>Key/value pair with the icon name and the filename</returns>
		private static void CreateIcon(string iconUri)
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
			icon.DefineZoom (0.5);

			var bitmap = icon.BitmapImage;

			// Save the image
			var bytes = bitmap.Save (ImageFormat.Png);
			string path = IconsBuilder.GetImageFilePath (iconName);
			System.IO.File.WriteAllBytes (path, bytes);

			// Add it to the CSS
			IconsBuilder.CreateCssFromIcon (iconUri, path);
		}

		/// <summary>
		/// Create CSS content to be able to call an icon from the HTML code
		/// </summary>
		/// <param name="iconName">Name of the icon (ressource)</param>
		/// <param name="path">Icon filename</param>
		/// <returns>CSS classname</returns>
		private static void CreateCssFromIcon(string iconUri, string path)
		{
			var cssClassname = IconsBuilder.GetCSSName (iconUri);
			string imgPath = path.Replace ("web", "..");
			var css = string.Format (IconsBuilder.cssClass, cssClassname, imgPath);

			System.IO.File.AppendAllText (IconsBuilder.cssFilename, css);
		}
		/// <summary>
		/// Get the filename of an image
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetImageFilePath(string name)
		{
			var path = string.Format (IconsBuilder.imagesFilename, name.Replace ('.', '/'));
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
		private readonly static string imagesFilename = "web/images/{0}.png";
		private readonly static string cssClass = ".{0} {{ background-image: url({1}) !important; }} \n";
	}
}
