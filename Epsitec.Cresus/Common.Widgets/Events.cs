namespace Epsitec.Common.Widgets
{
	using Epsitec.Common.Drawing;
	
	public delegate void PaintEventHandler(object sender, PaintEventArgs e);
	public delegate void MessageEventHandler(object sender, MessageEventArgs e);
	
	
	/// <summary>
	/// La classe PaintEventArgs décrit les arguments pour une opération de dessin, ce qui
	/// comprend un contexte graphique et un rectangle de clipping.
	/// </summary>
	public class PaintEventArgs : System.EventArgs, System.IDisposable
	{
		public PaintEventArgs(Graphics graphics, System.Drawing.RectangleF clip_rect)
		{
			this.graphics  = graphics;
			this.clip_rect = clip_rect;
		}
		
		
		public Graphics						Graphics
		{
			get { return this.graphics; }
		}
		
		public System.Drawing.RectangleF	ClipRectangle
		{
			get { return this.clip_rect; }
		}
		
		public bool							Suppress
		{
			get { return this.suppress; }
			set { this.suppress = value; }
		}
		
		
		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		#endregion
		
		~ PaintEventArgs()
		{
			this.Dispose (false);
		}
		
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
		private bool						suppress;
	}
	
	
	/// <summary>
	/// La classe MessageEventArgs décrit un événement produit par l'utilisateur.
	/// </summary>
	public class MessageEventArgs : System.EventArgs
	{
		public MessageEventArgs(Message message, System.Drawing.PointF point)
		{
			this.message = message;
			this.point   = point;
		}
		
		
		public Message						Message
		{
			get { return this.message; }
		}
		
		public System.Drawing.PointF		Point
		{
			get { return this.point; }
		}
		
		
		private Message						message;
		private System.Drawing.PointF		point;
	}
}
