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
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				this.SetColor (value);
			}
		}
		
		
		public void Clear(Color color)
		{
			if (color.IsValid)
			{
				this.ClearARGB (color.A, color.R, color.G, color.B);
			}
		}
		
		public void Clear(double r, double g, double b)
		{
			this.ClearARGB (1, r, g, b);
		}
		
		public virtual void ClearARGB(double a, double r, double g, double b)
		{
		}
		
		
		public void SetColor(Color color)
		{
			if (color.IsEmpty)
			{
				this.SetColorARGB (0, 0, 0, 0);
			}
			else
			{
				this.SetColorARGB (color.A, color.R, color.G, color.B);
			}
		}
		
		public void SetColor(double r, double g, double b)
		{
			this.SetColorARGB (1, r, g, b);
		}
		
		public virtual void SetColorARGB(double a, double r, double g, double b)
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
		
		
		protected Color						color;
	}
}
