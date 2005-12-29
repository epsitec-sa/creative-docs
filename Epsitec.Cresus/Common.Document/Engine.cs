using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Engine.
	/// </summary>
	public class Engine : ICanvasEngine
	{
		private Engine()
		{
			Res.Initialise(typeof(Engine), "Document");
			Drawing.Canvas.RegisterEngine(this);
		}
		
		public static void Initialise()
		{
			if ( Engine.current == null )
			{
				Engine.current = new Engine();
			}
		}
		
		#region ICanvasEngine Members
		public bool IsDataCompatible(byte[] data)
		{
			if ( ( data[0] == (byte) '<' ) &&
				 ( data[1] == (byte) '?' ) &&
				 ( data[2] == (byte) 'i' ) &&
				 ( data[3] == (byte) 'c' ) &&
				 ( data[4] == (byte) 'o' ) &&
				 ( data[5] == (byte) 'n' ) &&
				 ( data[6] == (byte) '?' ) &&
				 ( data[7] == (byte) '>' ) )
			{
				return true;
			}
			
			return false;
		}
		
		public void GetSizeAndOrigin(byte[] data, out Size size, out Point origin)
		{
			size   = new Size(20, 20);
			origin = new Point(0, 0);

			using ( System.IO.MemoryStream stream = new System.IO.MemoryStream(data) )
			{
				Document doc = new Document(DocumentType.Pictogram, DocumentMode.ReadOnly, InstallType.Full, DebugMode.Release, null, null);
				
				if ( doc.Read(stream, "") == "" )
				{
					size   = doc.DocumentSize;
					origin = doc.HotSpot;
				}
			}
		}
		
		public void Paint(Graphics graphics, Size size, byte[] data, GlyphPaintStyle style, Color color, object adornerObject)
		{
			using ( System.IO.MemoryStream stream = new System.IO.MemoryStream(data) )
			{
				Document doc = new Document(DocumentType.Pictogram, DocumentMode.ReadOnly, InstallType.Full, DebugMode.Release, null, null);
				DrawingContext context = new DrawingContext(doc, null);
				
				if ( doc.Read(stream, "") == "" )
				{
					this.adorner = adornerObject as Common.Widgets.IAdorner;
					this.glyphPaintStyle = style;
					this.uniqueColor = color;
					graphics.PushColorModifier(new ColorModifierCallback(this.ColorModifier));

					context.ContainerSize = size;
					context.PreviewActive = true;
					context.LayerDrawingMode = LayerDrawingMode.ShowInactive;
					context.RootStackPush(0);  // premi�re page
					context.RootStackPush(0);  // premier calque

					Point scale = context.Scale;
					graphics.ScaleTransform(scale.X, scale.Y, 0, 0);
					
					doc.Paint(graphics, context, Rectangle.Infinite);

					graphics.PopColorModifier();
				}
			}
		}

		protected void ColorModifier(ref RichColor color)
		{
			//	Adapte une couleur.
			if ( this.adorner != null )
			{
				Color basic = color.Basic;
				this.adorner.AdaptPictogramColor(ref basic, this.glyphPaintStyle, this.uniqueColor);
				color.Basic = basic;
			}
		}
		#endregion
		
		protected static Engine					current;

		protected Common.Widgets.IAdorner		adorner;
		protected GlyphPaintStyle				glyphPaintStyle;
		protected Color							uniqueColor;
	}
}
