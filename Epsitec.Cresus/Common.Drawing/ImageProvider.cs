namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe ImageProvider permet d'obtenir des images à partir de leur
	/// nom. Cette implémentation naïve ne connaît pour l'instant que le protocole
	/// "file:".
	/// </summary>
	public class ImageProvider : IImageProvider
	{
		private ImageProvider()
		{
		}
		
		public static ImageProvider		Default
		{
			get { return ImageProvider.default_provider; }
		}
		
		
		public Image GetImage(string name)
		{
			if ((name == null) ||
				(name.Length < 1))
			{
				return null;
			}
			
			if (this.images.ContainsKey (name))
			{
				return this.images[name] as Image;
			}
			
			if (name.StartsWith ("file:"))
			{
				Image image = Bitmap.FromFile (name.Remove (0, 5));
				
				if (image != null)
				{
					this.images[name] = image;
				}
				
				return image;
			}
			
			return null;
		}
		
		
		protected System.Collections.Hashtable	images = new System.Collections.Hashtable ();
		protected static ImageProvider			default_provider = new ImageProvider ();
	}
}
