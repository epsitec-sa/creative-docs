namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Image permet de représenter une image de type bitmap.
	/// </summary>
	public abstract class Image
	{
		public Image()
		{
			this.size   = new Size ();
			this.origin = new Point ();
			
			this.unique_id = System.Threading.Interlocked.Increment (ref Image.unique_id_seed);
		}
		
		
		public Size						Size
		{
			get { return this.size; }
		}
		
		public double					Width
		{
			get { return this.size.Width; }
		}
		
		public double					Height
		{
			get { return this.size.Height; }
		}
		
		public Point					Origin			//	0 < origin < size: l'origine est dans l'image
		{
			get { return this.origin; }
		}
		
		public bool						IsOriginDefined
		{
			get { return this.is_origin_defined; }
		}
		
		public bool						IsEmpty
		{
			get { return this.Size.IsEmpty; }
		}
		
		public abstract Bitmap			BitmapImage
		{
			get;
		}
		
		public long						UniqueId
		{
			get { return this.unique_id; }
		}
		
		
		public static readonly Image	Empty;
		
		
		protected bool					is_origin_defined;
		protected Size					size;
		protected Point					origin;
		protected long					unique_id;
		
		private static long				unique_id_seed;
	}
}
