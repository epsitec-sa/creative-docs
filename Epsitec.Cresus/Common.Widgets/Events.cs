namespace Epsitec.Common.Widgets
{
	using Epsitec.Common.Drawing;
	
	/// <summary>
	/// La classe PaintEventArgs décrit les arguments pour une opération de dessin, ce qui
	/// comprend un contexte graphique et un rectangle de clipping.
	/// </summary>
	public class PaintEventArgs : System.IDisposable
	{
		public PaintEventArgs(Graphics graphics, System.Drawing.RectangleF clip_rect)
		{
			this.graphics  = graphics;
			this.clip_rect = clip_rect;
		}

		~ PaintEventArgs()
		{
			this.Dispose (false);
		}
		
		public Graphics						Graphics
		{
			get { return this.graphics; }
		}
		
		public System.Drawing.RectangleF	ClipRectangle
		{
			get { return this.clip_rect; }
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
			if (disposing && (this.graphics != null))
			{
				this.graphics.Dispose ();
				this.graphics = null;
			}
		}
		
		
		private Graphics					graphics;
		private System.Drawing.RectangleF	clip_rect;
	}
}
