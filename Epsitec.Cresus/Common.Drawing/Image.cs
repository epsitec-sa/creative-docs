namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Image permet de repr�senter une image de type bitmap.
	/// </summary>
	public abstract class Image : System.IDisposable
	{
		public Image()
		{
			this.size   = Size.Empty;
			this.origin = new Point ();
			
			this.unique_id = System.Threading.Interlocked.Increment (ref Image.unique_id_seed);
		}
		
		
		public abstract void DefineZoom(double zoom);
		
		public virtual void MergeTransform(Transform transform)
		{
			//	Fusionne la transformation sp�cifi�e avec la transformation propre � l'image
			//	(changement d'�chelle pour que la taille logique soit respect�e).
		}
		
		public virtual Image GetDisabled()
		{
			return null;
		}
		
		public virtual Size				Size
		{
			get { return this.size; }
		}
		
		public double					Width
		{
			get { return this.Size.Width; }
		}
		
		public double					Height
		{
			get { return this.Size.Height; }
		}
		
		public virtual Point			Origin
		{
			//	0 < origin < size: l'origine est dans l'image
			
			get { return this.origin; }
		}
		
		public virtual bool				IsOriginDefined
		{
			get { return this.is_origin_defined; }
		}
		
		public bool						IsEmpty
		{
			get { return this.Size.IsEmpty; }
		}
		
		public virtual bool				IsDisabledDefined
		{
			get { return false; }
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
		
		~Image()
		{
			this.Dispose (false);
		}
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
		}
		
		
		public virtual void RemoveFromCache()
		{
		}
		
		
		internal bool					is_origin_defined;
		
		protected Size					size;
		protected Point					origin;
		protected long					unique_id;
		
		private static long				unique_id_seed;
	}
}
