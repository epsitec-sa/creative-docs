namespace Epsitec.Common.Drawing
{
	public enum CapStyle
	{
		Butt, Square, Round
	}
	
	public enum JoinStyle
	{
		Miter, Round, Bevel
	}
	
	public enum FillMode
	{
		EvenOdd		= 1,
		NonZero		= 2
	}
	
	
	public abstract class Rasterizer : System.IDisposable
	{
		protected Rasterizer()
		{
		}
		
		~Rasterizer()
		{
			this.Dispose (false);
		}
		
		
		public FillMode					FillMode
		{
			get
			{
				return this.fill_mode;
			}
			set
			{
				if (this.fill_mode != value)
				{
					this.fill_mode = value;
					this.SyncFillMode ();
				}
			}
		}
		
		public double					Gamma
		{
			get
			{
				return this.gamma;
			}
			set
			{
				if (this.gamma != value)
				{
					this.gamma = value;
					this.SyncGamma ();
				}
			}
		}
		
		public Transform				Transform
		{
			get
			{
				return this.transform;
			}
			set
			{
				if (value == null)
				{
					throw new System.NullReferenceException ("Rasterizer.Transform");
				}
				
				if (this.transform != value)
				{
					this.transform = new Transform (value);
					this.SyncTransform ();
				}
			}
		}
		
		
		protected virtual void SyncFillMode()
		{
		}
		
		protected virtual void SyncGamma()
		{
		}
		
		protected virtual void SyncTransform()
		{
		}
		
		
		internal virtual void SetClipBox(double x1, double y1, double x2, double y2)
		{
		}
		
		internal virtual void ResetClipBox()
		{
		}
		
		
		public virtual void AddSurface(Path path)
		{
		}
		
		public void AddOutline(Path path)
		{
			this.AddOutline (path, 1);
		}
		
		public virtual void AddOutline(Path path, double width)
		{
		}
		
		public void AddOutline(Path path, double width, CapStyle cap, JoinStyle join)
		{
			this.AddOutline (path, width, cap, join, 4.0);
		}

		public virtual void AddOutline(Path path, double width, CapStyle cap, JoinStyle join, double miter_limit)
		{
		}
		
		public virtual void AddGlyph(Font font, int glyph, double x, double y, double scale)
		{
		}
		
		public virtual double AddText(Font font, string text, double x, double y, double scale)
		{
			return font.GetTextAdvance (text) * scale;
		}
		
		
		public virtual void Render(Renderers.Solid renderer)
		{
		}
		
		public virtual void Render(Renderers.Image renderer)
		{
		}
		
		public virtual void Render(Renderers.Gradient renderer)
		{
		}
		
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
		}
		
		
		
		protected FillMode						fill_mode = FillMode.NonZero;
		protected double						gamma = 1.0;
		
		protected Transform						transform = new Transform ();
	}
}
