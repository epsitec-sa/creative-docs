using Epsitec.Common.Pictogram.Data;
using Epsitec.Common.Pictogram.Widgets;

namespace Epsitec.Common.Pictogram
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
			if ( Engine.current == null )
			{
				Engine.current = new Engine();
			}
		}
		
		#region ICanvasEngine Members
		public void GetSizeAndOrigin(byte[] data, out Drawing.Size size, out Drawing.Point origin)
		{
			size   = new Drawing.Size(20, 20);
			origin = new Drawing.Point(0, 0);

			using ( System.IO.MemoryStream stream = new System.IO.MemoryStream(data) )
			{
				IconObjects icon = new IconObjects();
				
				if ( icon.Read(stream) )
				{
					size   = icon.Size;
					origin = icon.Origin;
				}
			}
		}

		public void Paint(Drawing.Graphics graphics, Drawing.Size size, byte[] data, bool disabled, Drawing.Color color, object adorner_object)
		{
			using ( System.IO.MemoryStream stream = new System.IO.MemoryStream(data) )
			{
				IconObjects icon = new IconObjects();
				IconContext context = new IconContext();
				
				Epsitec.Common.Widgets.IAdorner adorner = adorner_object as Epsitec.Common.Widgets.IAdorner;
				
				if ( icon.Read(stream) )
				{
					context.Adorner = adorner;
					context.UniqueColor = color;
					context.ScaleX = size.Width / icon.Size.Width;
					context.ScaleY = size.Height / icon.Size.Height;
					graphics.ScaleTransform(context.ScaleX, context.ScaleY, 0, 0);
					
					icon.DrawGeometry(graphics, context, color, adorner_object);
				}
			}
		}
		#endregion
		
		protected static Engine			current;
	}
}
