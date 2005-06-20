using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Drawer contient le dessinateur universel.
	/// </summary>
	public class Drawer
	{
		public Drawer()
		{
		}

		// Dessine un chemin.
		public void DrawStroke(IPaintPort port, DrawingContext drawingContext, Shape shape, Properties.Line propLine, Properties.Gradient propColor)
		{
			if ( !propLine.IsVisible() || !propColor.IsVisible() )  return;

			if ( port is Graphics )
			{
				this.DrawStroke(port as Graphics, drawingContext, shape, propLine, propColor);
				return;
			}

			if ( port is Printing.PrintPort )
			{
				this.DrawStroke(port as Printing.PrintPort, drawingContext, shape, propLine, propColor);
				return;
			}

			if ( port is PDF.Port )
			{
				this.DrawStroke(port as PDF.Port, drawingContext, shape, propLine, propColor);
				return;
			}
		}

		// Dessine une surface.
		public void DrawSurface(IPaintPort port, DrawingContext drawingContext, Shape shape, Properties.Gradient propColor)
		{
			if ( !propColor.IsVisible() )  return;

			if ( port is Graphics )
			{
				this.DrawSurface(port as Graphics, drawingContext, shape, propColor);
				return;
			}

			if ( port is Printing.PrintPort )
			{
				this.DrawSurface(port as Printing.PrintPort, drawingContext, shape, propColor);
				return;
			}

			if ( port is PDF.Port )
			{
				this.DrawSurface(port as PDF.Port, drawingContext, shape, propColor);
				return;
			}
		}


		// Dessine un chemin à l'écran ou dans un bitmap.
		protected void DrawStroke(Graphics port, DrawingContext drawingContext, Shape shape, Properties.Line propLine, Properties.Gradient propColor)
		{
		}

		// Dessine un chemin sur l'imprimante.
		protected void DrawStroke(Printing.PrintPort port, DrawingContext drawingContext, Shape shape, Properties.Line propLine, Properties.Gradient propColor)
		{
		}

		// Dessine un chemin dans un fichier PDF.
		protected void DrawStroke(PDF.Port port, DrawingContext drawingContext, Shape shape, Properties.Line propLine, Properties.Gradient propColor)
		{
		}


		// Dessine une surface à l'écran ou dans un bitmap.
		protected void DrawSurface(Graphics port, DrawingContext drawingContext, Shape shape, Properties.Gradient propColor)
		{
		}

		// Dessine une surface sur l'imprimante.
		protected void DrawSurface(Printing.PrintPort port, DrawingContext drawingContext, Shape shape, Properties.Gradient propColor)
		{
			port.Color = propColor.Color1;
			port.PaintSurface(shape.Path);
		}

		// Dessine une surface dans un fichier PDF.
		protected void DrawSurface(PDF.Port port, DrawingContext drawingContext, Shape shape, Properties.Gradient propColor)
		{
		}
	}
}
