//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Image permet de représenter une image de type bitmap.
	/// </summary>
	public abstract class Image : System.IDisposable
	{
		public Image()
			: this (0L)
		{
		}

		public Image(long uniqueId)
		{
			this.size   = Size.Empty;
			this.origin = new Point ();
			this.dpiX  = 96;
			this.dpiY  = 96;
			
			this.uniqueId = uniqueId == 0 ? System.Threading.Interlocked.Increment (ref Image.uniqueIdSeed) : uniqueId;
		}
		
		
		public abstract void DefineZoom(double zoom);
		public abstract void DefineColor(Drawing.Color color);
		public abstract void DefineAdorner(object adorner);
		
		public virtual Image GetImageForPaintStyle(GlyphPaintStyle style)
		{
			if (style == GlyphPaintStyle.Normal)
			{
				return this;
			}
			
			return null;
		}


		public string							Id
		{
			//	L'identificateur permet de retrouver l'image lors de la désérialisation du port graphique XmlPort.
			get;
			set;
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
			get { return this.isOriginDefined; }
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
			get { return this.uniqueId; }
		}
		
		public double							DpiX
		{
			get
			{
				return this.dpiX;
			}
		}
		
		public double							DpiY
		{
			get
			{
				return this.dpiY;
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

		public void AssigneUniqueId(long uniqueId)
		{
			if (uniqueId != 0)
			{
				this.uniqueId = uniqueId;
			}
		}
		
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
		
		
		internal bool							isOriginDefined;
		internal double							dpiX;
		internal double							dpiY;
		
		protected Size							size;
		protected Point							origin;
		private long							uniqueId;
		
		private static long						uniqueIdSeed = 1;
	}
}
