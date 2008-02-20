//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	public delegate bool DynamicImagePaintCallback(Drawing.Graphics graphics, Drawing.Size size, string argument, Drawing.GlyphPaintStyle style, Drawing.Color color, object adorner);
	
	/// <summary>
	/// La classe DynamicImage permet de représenter une image construite à la
	/// volée (on the fly). Le dessin lui-même se fait dans un call-back fourni
	/// par l'utilisateur.
	/// Cette classe est utilisée par le protocole "dyn:xyz/abc" (cf ImageProvider,
	/// il sait retrouver le DynamicImage correspondant à partir de la paire nom
	/// "xyz" et argument "abc").
	/// </summary>
	public sealed class DynamicImage : Image
	{
		public DynamicImage(Drawing.Size size, DynamicImagePaintCallback callback) : this ()
		{
			this.DefineSize (size);
			this.DefinePaintCallback (callback);
		}
		
		public DynamicImage(Drawing.Size size, DynamicImagePaintCallback callback, bool enableCache) : this (size, callback)
		{
			this.isCacheEnabled = enableCache;
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
		
		
		public void ClearCache(string argument)
		{
			if (this.model != null)
			{
				this.model.ClearCache (argument);
			}
			else
			{
				if (this.variants != null)
				{
					foreach (System.Collections.DictionaryEntry entry in this.variants)
					{
						Key          key   = entry.Key as Key;
						DynamicImage image = entry.Value as DynamicImage;
						
						if ((argument == null) ||
							(key.Argument == argument))
						{
							image.ClearCache ();
						}
					}
				}
			}
		}
		
		
		#region Public Override Properties
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
				this.ValidateGeometry();
				return base.Size;
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
		
		#endregion
		
		#region Private Properties
		private double							Zoom
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
		
		private object							Adorner
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
		
		private Drawing.Color					Color
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
		
		private DynamicImagePaintCallback		PaintCallback
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
		
		
		private string							Argument
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

		private GlyphPaintStyle					PaintStyle
		{
			get
			{
				return this.paintStyle;
			}
		}
		
		#endregion
		
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
		
		
		#region Public Override Methods
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

		#endregion

		public override Image GetImageForPaintStyle(GlyphPaintStyle style)
		{
			//	Retourne l'image qui correspond au style de peinture de glyphe
			//	désiré. On réalise un clonage rapide. Afin d'éviter de devoir
			//	copier trop d'information, on chaîne le clone avec son modèle.
			
			return this.GetImageForKey (new Key (style, this.Width, this.Height, this.Argument));
		}

		public override bool IsPaintStyleDefined(GlyphPaintStyle style)
		{
			return this.PaintCallback (null, Size.Zero, this.Argument, style, Color.Empty, null);
		}
		
		public DynamicImage GetImageForArgument(string argument)
		{
			//	Retourne une instance de DynamicImage réglée pour correspondre
			//	à l'argument spécifié. Le modèle initial contient une table avec
			//	les instances déjà créées, ce qui évite que l'on n'instancie des
			//	nouvelles copies à tour de bras.
			
			return this.GetImageForKey (new Key (this.PaintStyle, this.Width, this.Height, argument));
		}
		
		public DynamicImage GetImageForSize(double width, double height)
		{
			//	Retourne une instance de DynamicImage réglée pour correspondre
			//	à la taille sépcifiée.
			
			return this.GetImageForKey (new Key (this.PaintStyle, width, height, this.Argument));
		}
		
		
		#region Private Constructors
		private DynamicImage()
		{
			this.variants = new System.Collections.Hashtable ();
		}
		private DynamicImage(DynamicImage model, Key key) : this (model, key.PaintStyle, new Drawing.Size (key.Width, key.Height), key.Argument)
		{
		}
		private DynamicImage(DynamicImage model, GlyphPaintStyle style, Drawing.Size size, string argument)
		{
			this.model      = model;
			this.paintStyle = style;
			this.size       = size;
			this.argument   = argument;
		}
		#endregion
		
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

		
		private DynamicImage GetImageForKey(Key key)
		{
			if (this.model == null)
			{
				//	Nous travaillons avec le modèle initial, seul à contenir
				//	une table des variantes :
				
				DynamicImage image = this.variants[key] as DynamicImage;
				
				if (image == null)
				{
					//	Crée un clone qui reprend tous les réglages du modèle
					//	initial, sauf les arguments qui lui sont spécifiques.
					
					//	Chacun a son propre cache.
					
					image = new DynamicImage (this, key);
					this.variants[key] = image;
				}
				
				return image;
			}
			else
			{
				return this.model.GetImageForKey (key);
			}
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

					if (callback (graphics, size, this.Argument, this.paintStyle, this.Color, this.Adorner))
					{
						int width, height, stride;
						System.Drawing.Imaging.PixelFormat format;
						System.IntPtr scan0;

						pixmap.GetMemoryLayout (out width, out height, out stride, out format, out scan0);
						System.Diagnostics.Debug.Assert (width == dx);
						System.Diagnostics.Debug.Assert (height == dy);

						System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap (dx, dy, format);
						System.Drawing.Rectangle bbox   = new System.Drawing.Rectangle (0, 0, dx, dy);
						System.Drawing.Imaging.ImageLockMode mode   = System.Drawing.Imaging.ImageLockMode.WriteOnly;
						System.Drawing.Imaging.BitmapData data   = bitmap.LockBits (bbox, mode, format);

						try
						{
							unsafe
							{
								int num = stride / 4;
								uint* src = (uint*) scan0.ToPointer ();
								uint* buf = (uint*) data.Scan0.ToPointer () + dy * num;

								for (int line = 0; line < dy; line++)
								{
									buf -= num;

									uint* dst = buf;

									for (int i=0; i<num; i++)
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

						this.cache = Bitmap.FromNativeBitmap (bitmap, this.Origin, size).BitmapImage;
					}
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
		
		#region Private Key Class
		private class Key
		{
			public Key(GlyphPaintStyle paintStyle, double width, double height, string argument)
			{
				this.paintStyle = paintStyle;
				this.width      = width;
				this.height     = height;
				this.argument   = argument;
			}
			
			
			public GlyphPaintStyle				PaintStyle
			{
				get
				{
					return this.paintStyle;
				}
			}
			public double						Width
			{
				get
				{
					return this.width;
				}
			}
			public double						Height
			{
				get
				{
					return this.height;
				}
			}
			public string						Argument
			{
				get
				{
					return this.argument;
				}
			}
			
			public override bool Equals(object obj)
			{
				Key key = obj as Key;
				
				if (key == null)
				{
					return false;
				}
				
				return (this.paintStyle == key.paintStyle)
					&& (this.width == key.width) 
					&& (this.height == key.height)
					&& (this.argument == key.argument);
			}
			
			public override int GetHashCode()
			{
				return this.paintStyle.GetHashCode ()
					 ^ this.width.GetHashCode ()
					 ^ this.height.GetHashCode ()
					 ^ this.argument.GetHashCode ();
			}

			
			GlyphPaintStyle						paintStyle;
			double								width;
			double								height;
			string								argument;
		}
		#endregion
		
		private bool							isDisposed;
		private bool							isCacheEnabled = true;
		
		private DynamicImagePaintCallback		callback;
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
