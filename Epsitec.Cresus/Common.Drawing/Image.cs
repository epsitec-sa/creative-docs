namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Image permet de représenter une image de type bitmap.
	/// </summary>
	public class Image
	{
		public Image()
		{
			this.size   = new Size ();
			this.origin = new Point ();
		}
		
		public Size						Size
		{
			get { return this.size; }
		}
		
		public Point					Origin			//	0 < origin < size: l'origine est dans l'image
		{
			get { return this.origin; }
		}
		
		public bool						IsEmpty
		{
			get { return this.Size.IsEmpty; }
		}
		
		
		public static readonly Image	Empty;
		
		
		
		private Size					size;
		private Point					origin;
	}
}
