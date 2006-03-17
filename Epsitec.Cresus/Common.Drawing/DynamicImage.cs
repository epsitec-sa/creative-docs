//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	public delegate void DynamicImagePaintCallback(Drawing.Graphics graphics, Drawing.Size size, string argument, Drawing.GlyphPaintStyle style, Drawing.Color color, object adorner);
	
	/// <summary>
	/// La classe DynamicImage permet de représenter une image construite à la
	/// volée (on the fly).
	/// </summary>
	public class DynamicImage : Image
	{
		public DynamicImage()
		{
			this.effects = new System.Collections.Hashtable ();
			this.variants = new System.Collections.Hashtable ();
			
			this.effects[GlyphPaintStyle.Normal]   = new DynamicImage(this, GlyphPaintStyle.Normal);
			this.effects[GlyphPaintStyle.Disabled] = new DynamicImage(this, GlyphPaintStyle.Disabled);
			this.effects[GlyphPaintStyle.Selected] = new DynamicImage(this, GlyphPaintStyle.Selected);
			this.effects[GlyphPaintStyle.Entered]  = new DynamicImage(this, GlyphPaintStyle.Entered);
			this.effects[GlyphPaintStyle.Shadow]   = new DynamicImage(this, GlyphPaintStyle.Shadow);
		}
		
		public DynamicImage(Drawing.Size size, DynamicImagePaintCallback callback) : this ()
		{
			this.DefineSize (size);
			this.DefinePaintCallback (callback);
		}
		
		
		public override Bitmap					BitmapImage
		{
			get
			{
				if (this.IsCacheEnabled)
				{
				}
				else
				{
					this.ClearCache ();
				}
				
				this.ValidateCache ();
				
				return this.cache;
			}
		}
		
		public override Size					Size
		{
			get
			{
				if (this.model == null)
				{
					this.ValidateGeometry();
					return base.Size;
				}
				else
				{
					return this.model.Size;
				}
			}
		}

		public override Point					Origin
		{
			get
			{
				if (this.model == null)
				{
					this.ValidateGeometry();
					return base.Origin;
				}
				else
				{
					return this.model.Origin;
				}
			}
		}

		public override bool					IsOriginDefined
		{
			get
			{
				return true;
			}
		}
		
		public double							Zoom
		{
			get
			{
				if (this.model == null)
				{
					return this.zoom;
				}
				else
				{
					return this.model.Zoom;
				}
			}
		}
		
		public object							Adorner
		{
			get
			{
				if (this.model == null)
				{
					return this.adorner;
				}
				else
				{
					return this.model.Adorner;
				}
			}
		}
		
		public Drawing.Color					Color
		{
			get
			{
				if (this.model == null)
				{
					return this.color;
				}
				else
				{
					return this.model.Color;
				}
			}
		}
		
		public DynamicImagePaintCallback		PaintCallback
		{
			get
			{
				if (this.model == null)
				{
					return this.callback;
				}
				else
				{
					return this.model.PaintCallback;
				}
			}
		}
		
		public bool								IsCacheEnabled
		{
			get
			{
				if (this.model == null)
				{
					return this.isCacheEnabled;
				}
				else
				{
					return this.model.IsCacheEnabled;
				}
			}
			set
			{
				if (this.model == null)
				{
					this.isCacheEnabled = value;
				}
				else
				{
					this.model.IsCacheEnabled = value;
				}
			}
		}
		
		
		public string							Argument
		{
			get
			{
				if ((this.argument == null) &&
					(this.model != null))
				{
					return this.model.Argument;
				}
				
				return this.argument;
			}
		}

		public GlyphPaintStyle					PaintStyle
		{
			get
			{
				return this.paintStyle;
			}
		}
		
		
		public void DefineSize(Drawing.Size size)
		{
			if (this.model == null)
			{
				if (this.size != size)
				{
					this.size = size;
					this.InvalidateCache ();
				}
			}
			else
			{
				this.model.DefineSize (size);
			}
		}
		
		public void DefineOrigin(Drawing.Point origin)
		{
			if (this.model == null)
			{
				if (this.origin != origin)
				{
					this.origin = origin;
					this.InvalidateCache ();
				}
			}
			else
			{
				this.model.DefineOrigin (origin);
			}
		}
		
		public void DefinePaintCallback(DynamicImagePaintCallback callback)
		{
			if (this.model == null)
			{
				this.callback = callback;
				this.InvalidateCache ();
			}
			else
			{
				this.model.DefinePaintCallback (callback);
			}
	}
		
		
		public override void DefineZoom(double zoom)
		{
			if (this.model == null)
			{
				if (this.zoom != zoom)
				{
					this.zoom = zoom;
					this.InvalidateCache ();
				}
			}
			else
			{
				this.model.DefineZoom (zoom);
			}
		}
		
		public override void DefineColor(Drawing.Color color)
		{
			if (this.model == null)
			{
				if (this.color != color)
				{
					this.color = color;
					this.InvalidateCache ();
				}
			}
			else
			{
				this.model.DefineColor (color);
			}
		}
		
		public override void DefineAdorner(object adorner)
		{
			if (this.model == null)
			{
				if (this.adorner != adorner)
				{
					this.adorner = adorner;
					this.InvalidateCache ();
				}
			}
			else
			{
				this.model.DefineAdorner (adorner);
			}
		}


		public override Image GetImageForPaintStyle(GlyphPaintStyle style)
		{
			System.Diagnostics.Debug.Assert (this.model != null);
			
			if (this.effects != null)
			{
				return this.effects[style] as DynamicImage;
			}
			else
			{
				return this.model.GetImageForPaintStyle (style);
			}
		}
		
		
		public DynamicImage GetImageForArgument(string argument)
		{
			if (this.model == null)
			{
				DynamicImage image = this.variants[argument] as DynamicImage;
				
				if (image == null)
				{
					image = new DynamicImage ();
					image.model = this;
					image.argument = argument;
					
					this.variants[argument] = image;
				}
				
				return image;
			}
			else
			{
				return this.model.GetImageForArgument (argument);
			}
		}
		
		private DynamicImage(DynamicImage model, GlyphPaintStyle style)
		{
			this.model = model;
			this.paintStyle = style;
		}
		
		
		private void ValidateCache()
		{
			//	Construit l'image bitmap...
			
			DynamicImagePaintCallback callback = this.PaintCallback;
			
			if ((this.cache == null) &&
				(callback != null))
			{
				Size size = this.Size;
				
				int dx = (int) (size.Width * this.Zoom);
				int dy = (int) (size.Height * this.Zoom);
				
				size = new Size (dx, dy);
				
				using (Drawing.Graphics graphics = new Drawing.Graphics ())
				{
					graphics.SetPixmapSize (dx, dy);
					Drawing.Pixmap pixmap = graphics.Pixmap;
					pixmap.Clear ();
					
					callback (graphics, size, this.Argument, this.paintStyle, this.Color, this.Adorner);
					
					int width, height, stride;
					System.Drawing.Imaging.PixelFormat format;
					System.IntPtr scan0;
					
					pixmap.GetMemoryLayout(out width, out height, out stride, out format, out scan0);
					System.Diagnostics.Debug.Assert(width == dx);
					System.Diagnostics.Debug.Assert(height == dy);

					System.Drawing.Bitmap                bitmap = new System.Drawing.Bitmap (dx, dy, format);
					System.Drawing.Rectangle             bbox   = new System.Drawing.Rectangle (0, 0, dx, dy);
					System.Drawing.Imaging.ImageLockMode mode   = System.Drawing.Imaging.ImageLockMode.WriteOnly;
					System.Drawing.Imaging.BitmapData    data   = bitmap.LockBits (bbox, mode, format);
					
					try
					{
						unsafe
						{
							int   num = stride / 4;
							uint* src = (uint*) scan0.ToPointer();
							uint* buf = (uint*) data.Scan0.ToPointer() + dy * num;
						
							for (int line = 0; line < dy; line++)
							{
								buf -= num;
							
								uint* dst = buf;
							
								for ( int i=0 ; i<num ; i++ )
								{
									*dst++ = *src++;
								}
							}
						}
					}
					finally
					{
						bitmap.UnlockBits(data);
					}
					
					this.cache = Bitmap.FromNativeBitmap(bitmap, this.Origin, size).BitmapImage;
				}
			}
		}
		
		private void ValidateGeometry()
		{
		}
		
		private void InvalidateCache()
		{
			System.Diagnostics.Debug.Assert (this.model == null);

			this.ClearCache ();
		}
		
		private void ClearCache()
		{
			if (this.effects != null)
			{
				foreach (DynamicImage image in this.effects.Values)
				{
					image.ClearCache ();
				}
			}
			if (this.variants != null)
			{
				foreach (DynamicImage image in this.variants.Values)
				{
					image.ClearCache ();
				}
			}
			
			if (this.cache != null)
			{
				this.cache.Dispose ();
				this.cache = null;
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.Assert (this.isDisposed == false);
			
			this.isDisposed = true;
			
			if (disposing)
			{
				if (this.model == null)
				{
					this.InvalidateCache ();
				}
			}
			
			base.Dispose(disposing);
		}

		
		private bool							isDisposed;
		private bool							isCacheEnabled;
		
		private DynamicImagePaintCallback		callback;
		private System.Collections.Hashtable	effects;
		
		//	       modèle
		//	          |
		//	   +------+------+
		//	   |      |      |
		//	 arg=X  arg=Y  arg=Z
		//	          |
		//	   +------+------+
		//	   |      |      |
		//	divers GlyphPaintStyle
		
		private DynamicImage					model;
		
		private System.Collections.Hashtable	variants;
		private GlyphPaintStyle					paintStyle = GlyphPaintStyle.Invalid;
		private double							zoom       = 1.0;
		private Drawing.Color					color      = Drawing.Color.Empty;
		private object							adorner;
		private Bitmap							cache;
		private string							argument;
	}
}
