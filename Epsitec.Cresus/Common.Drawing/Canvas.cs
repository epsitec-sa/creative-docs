namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Canvas permet de représenter une image vectorielle.
	/// </summary>
	public class Canvas : Image
	{
		static Canvas()
		{
			Bitmap.CanvasFactory = new Canvas.Factory ();
		}
		
		
		internal Canvas(byte[] data)
		{
			this.data = new byte[data.Length];
			data.CopyTo (this.data, 0);
			
			this.effects = new EffectTable ();
			
			this.effects[GlyphPaintStyle.Normal]   = new Canvas (this, GlyphPaintStyle.Normal);
			this.effects[GlyphPaintStyle.Disabled] = new Canvas (this, GlyphPaintStyle.Disabled);
			this.effects[GlyphPaintStyle.Selected] = new Canvas (this, GlyphPaintStyle.Selected);
			this.effects[GlyphPaintStyle.Entered]  = new Canvas (this, GlyphPaintStyle.Entered);
			this.effects[GlyphPaintStyle.Shadow]   = new Canvas (this, GlyphPaintStyle.Shadow);
			
			Canvas.global_icon_cache.Add (this);
		}
		
		protected Canvas(Canvas original, GlyphPaintStyle style)
		{
			//	Version "normal", "disabled" ou "selected" du même dessin; on partage
			//	les données avec le modèle original...
			
			this.data        = original.data;
			this.effects     = original.effects;
			this.paint_style = style;
		}
		
		
		public override void DefineZoom(double zoom)
		{
			if (this.zoom != zoom)
			{
				this.zoom = zoom;
				this.InvalidateCache ();
			}
		}
		
		public override void DefineColor(Drawing.Color color)
		{
			if (this.color != color)
			{
				this.color = color;
				this.InvalidateCache ();
			}
		}
		
		public override void DefineAdorner(object adorner)
		{
			if (this.adorner != adorner)
			{
				this.adorner = adorner;
				this.InvalidateCache ();
			}
		}
		
		public override Image GetImageForPaintStyle(GlyphPaintStyle style)
		{
			return this.effects[style];
		}
		
		
		public static void RegisterEngine(ICanvasEngine engine)
		{
			int n = Canvas.engines.Length;
			ICanvasEngine[] engines = new ICanvasEngine[n+1];
			Canvas.engines.CopyTo (engines, 0);
			engines[n] = engine;
			Canvas.engines = engines;
		}
		
		public static ICanvasEngine FindEngine(byte[] data)
		{
			for (int i = 0; i < Canvas.engines.Length; i++)
			{
				ICanvasEngine engine = Canvas.engines[i];
				
				if (engine.IsDataCompatible (data))
				{
					return engine;
				}
			}
			
			return null;
		}
		
		
		public override Bitmap			BitmapImage
		{
			get
			{
				this.ValidateCache ();
				return this.cache;
			}
		}
		
		public override Size			Size
		{
			get
			{
				this.ValidateGeometry ();
				return base.Size;
			}
		}

		public override Point			Origin
		{
			get
			{
				this.ValidateGeometry ();
				return base.Origin;
			}
		}

		public override bool			IsOriginDefined
		{
			get { return true; }
		}
		
		public GlyphPaintStyle			PaintStyle
		{
			get { return this.paint_style; }
		}
		
		
		protected void ValidateCache()
		{
			if (this.cache == null)
			{
				int dx = (int) (this.Width * this.zoom);
				int dy = (int) (this.Height * this.zoom);
				
				Size size = new Size (dx, dy);
				
				ICanvasEngine engine = Canvas.FindEngine (this.data);
				
				System.Diagnostics.Debug.Assert (engine != null);
				
				using (Graphics graphics = new Graphics ())
				{
					graphics.SetPixmapSize (dx, dy);
					Drawing.Pixmap pixmap = graphics.Pixmap;
					pixmap.Clear ();
					
					engine.Paint (graphics, size, this.data, this.paint_style, this.color, this.adorner);
					
					int width, height, stride;
					System.Drawing.Imaging.PixelFormat format;
					System.IntPtr scan0;
					
					pixmap.GetMemoryLayout (out width, out height, out stride, out format, out scan0);
					
					System.Diagnostics.Debug.Assert (width == dx);
					System.Diagnostics.Debug.Assert (height == dy);
					

					System.Drawing.Bitmap                bitmap = new System.Drawing.Bitmap (dx, dy, format);
					System.Drawing.Rectangle             bbox   = new System.Drawing.Rectangle (0, 0, dx, dy);
					System.Drawing.Imaging.ImageLockMode mode   = System.Drawing.Imaging.ImageLockMode.WriteOnly;
					System.Drawing.Imaging.BitmapData    data   = bitmap.LockBits (bbox, mode, format);
					
					try
					{
						unsafe
						{
							int   num = stride / 4;
							uint* src = (uint*) scan0.ToPointer ();
							uint* buf = (uint*) data.Scan0.ToPointer () + dy * num;
						
							for (int line = 0; line < dy; line++)
							{
								buf -= num;
							
								uint* dst = buf;
							
								for (int i = 0; i < num; i++)
								{
									*dst++ = *src++;
								}
							}
						}
					}
					finally
					{
						bitmap.UnlockBits (data);
					}
					
					this.cache = Bitmap.FromNativeBitmap (bitmap, this.Origin, this.Size).BitmapImage;
				}
			}
		}
		
		protected void ValidateGeometry()
		{
			if (this.is_geom_ok == false)
			{
				ICanvasEngine engine = Canvas.FindEngine (this.data);
				
				System.Diagnostics.Debug.Assert (engine != null);
				
				engine.GetSizeAndOrigin (this.data, out this.size, out this.origin);
				
				this.is_geom_ok = true;
			}
		}
		
		protected void InvalidateCache()
		{
			if (this.cache != null)
			{
				this.cache.Dispose ();
				this.cache = null;
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			System.Diagnostics.Debug.Assert (this.is_disposed == false);
			
			this.is_disposed = true;
			
			if (disposing)
			{
				Canvas.global_icon_cache.Remove (this);
				this.InvalidateCache ();
			}
			
			base.Dispose (disposing);
		}

		public override void RemoveFromCache()
		{
			System.Diagnostics.Debug.WriteLine ("Removed image from cache.");
			
			Canvas.global_icon_cache.Remove (this);
		}
		
		
		protected class EffectTable : System.Collections.IDictionary
		{
			public EffectTable()
			{
				this.hash = new System.Collections.Hashtable ();
			}
			
			public Canvas				this[GlyphPaintStyle key]
			{
				get { return this.hash[key] as Canvas; }
				set { this.hash[key] = value; }
			}
			
			
			#region IDictionary Members
			public bool					IsReadOnly
			{
				get { return this.hash.IsReadOnly; }
			}
			
			object						System.Collections.IDictionary.this[object key]
			{
				get { return this.hash[key]; }
				set { this.hash[key] = value; }
			}
			
			public System.Collections.ICollection Keys
			{
				get { return this.hash.Keys; }
			}

			public System.Collections.ICollection Values
			{
				get { return this.hash.Values; }
			}

			public bool					IsFixedSize
			{
				get { return this.hash.IsFixedSize; }
			}
			
			
			public System.Collections.IDictionaryEnumerator GetEnumerator()
			{
				return this.hash.GetEnumerator ();
			}

			
			public bool Contains(object key)
			{
				return this.hash.Contains (key);
			}
			
			public void Add(object key, object value)
			{
				this.hash.Add (key, value);
			}
			
			public void Remove(object key)
			{
				this.hash.Remove (key);
			}

			public void Clear()
			{
				this.hash.Clear ();
			}
			#endregion

			#region ICollection Members
			public bool					IsSynchronized
			{
				get { return this.hash.IsSynchronized; }
			}
			
			public int					Count
			{
				get { return this.hash.Count; }
			}
			
			public object				SyncRoot
			{
				get { return this.hash.SyncRoot; }
			}
			
			
			public void CopyTo(System.Array array, int index)
			{
				this.CopyTo (array, index);
			}
			#endregion

			#region IEnumerable Members
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.hash.GetEnumerator ();
			}
			#endregion
			
			protected System.Collections.Hashtable hash;
		}
		
		protected class Factory : ICanvasFactory
		{
			#region ICanvasFactory Members
			public Image CreateCanvas(byte[] data)
			{
				return new Canvas (data);
			}
			#endregion
		}

		
		protected bool							is_disposed;
		protected bool							is_geom_ok;
		
		protected GlyphPaintStyle				paint_style = GlyphPaintStyle.Invalid;
		protected EffectTable					effects;
		protected byte[]						data;
		protected double						zoom = 1.0;
		protected Drawing.Color					color = Drawing.Color.Empty;
		protected object						adorner = null;
		protected Bitmap						cache;
		
		static ICanvasEngine[]					engines = new ICanvasEngine[0];
		static System.Collections.ArrayList		global_icon_cache = new System.Collections.ArrayList ();
	}
}
