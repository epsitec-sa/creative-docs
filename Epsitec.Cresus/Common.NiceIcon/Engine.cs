namespace Epsitec.Common.NiceIcon
{
	/// <summary>
	/// Engine.
	/// </summary>
	public class Engine : Drawing.ICanvasEngine
	{
		private Engine()
		{
			Drawing.Canvas.Engine = this;
		}
		
		public static void Initialise()
		{
			if (Engine.current == null)
			{
				Engine.current = new Engine ();
			}
		}
		
		#region ICanvasEngine Members
		public void GetSizeAndOrigin(byte[] data, out Drawing.Size size, out Drawing.Point origin)
		{
			size   = new Drawing.Size (20, 20);
			origin = new Drawing.Point (0, 0);
		}

		public void Paint(Drawing.Graphics graphics, Drawing.Size size, byte[] data)
		{
			using (System.IO.MemoryStream stream = new System.IO.MemoryStream(data))
			{
				IconObjects icon = new IconObjects();
				IconContext context = new IconContext();
				
				if ( icon.Read(stream) )
				{
					context.IsEnable = true;
					context.ScaleX = size.Width / 100;
					context.ScaleY = size.Height / 100;
					
					graphics.ScaleTransform(context.ScaleX, context.ScaleY, 0.5, 2);
					
					icon.DrawGeometry(graphics, context);
				}
			}
		}
		#endregion
		
		protected static Engine			current;
	}
}
