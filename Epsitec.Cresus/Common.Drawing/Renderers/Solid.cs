namespace Epsitec.Common.Drawing.Renderers
{
	public abstract class Solid : IRenderer, System.IDisposable
	{
		protected Solid()
		{
		}
		
		~Solid()
		{
			this.Dispose (false);
		}
		
		
		public virtual Pixmap				Pixmap
		{
			get { return null; }
			set { }
		}
		
		public virtual System.IntPtr		Handle
		{
			get { return System.IntPtr.Zero; }
		}
		
		public Color						Color
		{
			set { this.SetColor (value); }
		}
		
		
		public void Clear(Color color)
		{
			this.Clear (color.R, color.G, color.B, color.A);
		}
		
		public void Clear(double r, double g, double b)
		{
			this.Clear (r, g, b, 1);
		}
		
		public virtual void Clear(double r, double g, double b, double a)
		{
		}
		
		
		public void SetColor(Color color)
		{
			if (color.IsEmpty)
			{
					this.SetColor (0, 0, 0, 0);
			}
			else
			{
				this.SetColor (color.R, color.G, color.B, color.A);
			}
		}
		
		public void SetColor(double r, double g, double b)
		{
			this.SetColor (r, g, b, 1);
		}
		
		public virtual void SetColor(double r, double g, double b, double a)
		{
		}
		
		
		public abstract void SetAlphaMask(Pixmap pixmap, MaskComponent component);
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
		}
		
	}
}
