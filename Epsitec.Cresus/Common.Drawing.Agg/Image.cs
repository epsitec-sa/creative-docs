//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Image permet de représenter une image de type bitmap.
	/// </summary>
	public abstract class Image : System.IDisposable
	{
		public Image()
		{
			this.size   = Size.Empty;
			this.origin = new Point ();
			this.dpi_x  = 96;
			this.dpi_y  = 96;
			
			this.unique_id = System.Threading.Interlocked.Increment (ref Image.unique_id_seed);
		}
		
		
		public abstract void DefineZoom(double zoom);
		public abstract void DefineColor(Drawing.Color color);
		public abstract void DefineAdorner(object adorner);
		
		public virtual void MergeTransform(Transform transform)
		{
			//	Fusionne la transformation spécifiée avec la transformation propre à l'image
			//	(changement d'échelle pour que la taille logique soit respectée).
		}
		
		public virtual Image GetImageForPaintStyle(GlyphPaintStyle style)
		{
			if (style == GlyphPaintStyle.Normal)
			{
				return this;
			}
			
			return null;
		}
		
		
		public virtual Size						Size
		{
			get { return this.size; }
		}
		
		public double							Width
		{
			get { return this.Size.Width; }
		}
		
		public double							Height
		{
			get { return this.Size.Height; }
		}
		
		public virtual Point					Origin
		{
			//	0 < origin < size: l'origine est dans l'image
			
			get { return this.origin; }
		}
		
		
		public virtual bool						IsOriginDefined
		{
			get { return this.is_origin_defined; }
		}
		
		public bool								IsEmpty
		{
			get { return this.Size.IsEmpty; }
		}
		
		
		public abstract Bitmap					BitmapImage
		{
			get;
		}
		
		public long								UniqueId
		{
			get { return this.unique_id; }
		}
		
		public double							DpiX
		{
			get
			{
				return this.dpi_x;
			}
		}
		
		public double							DpiY
		{
			get
			{
				return this.dpi_y;
			}
		}
		
		
		public static readonly Image			Empty;
		
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
		
		
		public virtual bool IsPaintStyleDefined(GlyphPaintStyle style)
		{
			return this.GetImageForPaintStyle (style) != null;
		}
		
		public virtual void RemoveFromCache()
		{
		}
		
		
		internal bool							is_origin_defined;
		internal double							dpi_x;
		internal double							dpi_y;
		
		protected Size							size;
		protected Point							origin;
		protected long							unique_id;
		
		private static long						unique_id_seed = 1;
	}
}
