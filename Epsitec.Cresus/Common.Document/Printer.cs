using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Printer implémente l'impression d'un document.
	/// </summary>
	public class Printer
	{
		public Printer(Document document)
		{
			this.document = document;
			this.drawingContext = new DrawingContext(this.document, null);
		}

		// Imprime le document selon les choix faits dans le dialogue.
		public void Print(Epsitec.Common.Dialogs.Print dp)
		{
			PrintEngine printEngine = new PrintEngine();
			printEngine.Initialise(this.document, this, this.drawingContext);
			dp.Document.Print(printEngine);
		}


		protected class PrintEngine : Printing.IPrintEngine
		{
			public void Initialise(Document document, Printer printer, DrawingContext drawingContext)
			{
				this.document = document;
				this.printer = printer;
				this.drawingContext = drawingContext;
				this.pageCounter = 0;
			}

			#region IPrintEngine Members
			public void PrepareNewPage(Epsitec.Common.Printing.PageSettings settings)
			{
				settings.Margins = new Margins(0, 0, 0, 0);
				//?settings.Landscape = (this.document.Size.Width > this.document.Size.Height);
			}
			
			public void StartingPrintJob()
			{
			}
			
			public void FinishingPrintJob()
			{
			}
			
			public Printing.PrintEngineStatus PrintPage(Printing.PrintPort port)
			{
				this.drawingContext.ContainerSize = this.document.Size;
				this.drawingContext.PreviewActive = true;

				bool collate = port.PageSettings.PrinterSettings.Collate;
				int copies = port.PageSettings.PrinterSettings.Copies;
				int total = port.PageSettings.PrinterSettings.ToPage-port.PageSettings.PrinterSettings.FromPage+1;
				int page = this.pageCounter;

				if ( collate )  // 1,2,3 - 1,2,3 ?
				{
					page %= total;
				}
				else	// 1,1 - 2,2 - 3,3 ?
				{
					page /= copies;
				}
				page += port.PageSettings.PrinterSettings.FromPage-1;

				//?this.printer.PrintSimplyGeometry(port, this.drawingContext, page);
				this.printer.PrintComplexGeometry(port, this.drawingContext, page, 300);
				//?this.printer.PrintDummyGeometry(port, this.drawingContext, page, 300);

				this.pageCounter ++;
				if ( this.pageCounter < total*copies )
				{
					return Printing.PrintEngineStatus.MorePages;
				}
				return Printing.PrintEngineStatus.FinishJob;
			}
			#endregion

			protected Document				document;
			protected Printer				printer;
			protected DrawingContext		drawingContext;
			protected int					pageCounter;
		}

		
		// Imprime la géométrie simple de tous les objets, possible lorsque les
		// objets n'utilisent ni les dégradés ni la transparence.
		protected void PrintSimplyGeometry(Printing.PrintPort port,
										   DrawingContext drawingContext,
										   int pageNumber)
		{
			System.Diagnostics.Debug.Assert(pageNumber >= 0);
			System.Diagnostics.Debug.Assert(pageNumber < this.document.GetObjects.Count);

			Size paperSize = new Size();
			if ( port.PageSettings.Landscape )  // paysage ?
			{
				paperSize.Width  = port.PageSettings.PaperSize.Height;
				paperSize.Height = port.PageSettings.PaperSize.Width;
			}
			else	// portrait ?
			{
				paperSize = port.PageSettings.PaperSize.Size;
			}

#if false
			double zoomH = paperSize.Width  / this.document.Size.Width;
			double zoomV = paperSize.Height / this.document.Size.Height;
			double zoom = System.Math.Min(zoomH, zoomV);
			port.ScaleTransform(zoom, zoom, 0, 0);
#else
			port.ScaleTransform(0.1, 0.1, 0, 0);
#endif
			
			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				Properties.ModColor modColor = layer.PropertyModColor;
				drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					obj.PrintGeometry(port, drawingContext);
				}
			}
		}

		// Imprime la géométrie complexe de tous les objets, en utilisant
		// un bitmap intermédiaire.
		protected void PrintComplexGeometry(Printing.PrintPort port,
											DrawingContext drawingContext,
											int pageNumber, double dpi)
		{
			System.Diagnostics.Debug.Assert(pageNumber >= 0);
			System.Diagnostics.Debug.Assert(pageNumber < this.document.GetObjects.Count);

			Drawing.Graphics gfx = new Graphics();
			int dx = (int) ((this.document.Size.Width/10)*(dpi/25.4));
			int dy = (int) ((this.document.Size.Height/10)*(dpi/25.4));
			gfx.SetPixmapSize(dx, dy);
			gfx.SolidRenderer.ClearARGB(1,1,1,1);
			gfx.Rasterizer.Gamma = 0.0;

			//?gfx.AddRectangle(100, 100, dx-200, dy-200);
			//?gfx.RenderSolid(Color.FromRGB(0,0,1));

#if false
			double zoomH = dx / this.document.Size.Width;
			double zoomV = dy / this.document.Size.Height;
			double zoom = System.Math.Min(zoomH, zoomV);
#else
			double zoom = dpi/25.4/10;
#endif
			gfx.TranslateTransform(0, dy);
			gfx.ScaleTransform(zoom, -zoom, 0, 0);

			//?gfx.AddRectangle(100, 100, 2100-200, 2970-200);
			//?gfx.RenderSolid(Color.FromRGB(0,1,0));

			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				Properties.ModColor modColor = layer.PropertyModColor;
				drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					obj.DrawGeometry(gfx, drawingContext);
				}
			}

#if true
			port.ScaleTransform(0.1, 0.1, 0, 0);
			Drawing.Image bitmap = Drawing.Bitmap.FromPixmap(gfx.Pixmap);
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.document.Size.Width, this.document.Size.Height);
			port.PaintImage(bitmap, rect);

			Path path = new Path();
			path.AppendRectangle(100, 100, 2100-200, 2970-200);
			port.Color = Color.FromRGB(1,0,0);
			port.PaintOutline(path);
#else
			Drawing.Image bitmap = Drawing.Bitmap.FromPixmap(gfx.Pixmap);
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.document.Size.Width, this.document.Size.Height);
			port.PaintImage(bitmap, 0, 0, 210, 297, 0, 0, bitmap.Width, bitmap.Height);
#endif
		}

		// Imprime la géométrie complexe de tous les objets, en utilisant
		// un bitmap intermédiaire.
		protected void PrintDummyGeometry(Printing.PrintPort port,
										  DrawingContext drawingContext,
										  int pageNumber, double dpi)
		{
			System.Diagnostics.Debug.Assert(pageNumber >= 0);
			System.Diagnostics.Debug.Assert(pageNumber < this.document.GetObjects.Count);

			Drawing.Graphics gfx = new Graphics();
			gfx.SetPixmapSize(2100, 2970);
			gfx.SolidRenderer.ClearARGB(1,1,1,1);

			gfx.AddRectangle(100, 100, 2100-200, 2970-200);
			gfx.RenderSolid(Color.FromRGB(0,1,0));

			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				Properties.ModColor modColor = layer.PropertyModColor;
				drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					obj.DrawGeometry(gfx, drawingContext);
				}
			}

			port.ScaleTransform(0.1, 0.1, 0, 0);
			Drawing.Image bitmap = Drawing.Bitmap.FromPixmap(gfx.Pixmap);
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.document.Size.Width, this.document.Size.Height);
			port.PaintImage(bitmap, rect);

#if true
			Path path = new Path();
			path.AppendRectangle(100, 100, 2100-200, 2970-200);
			port.Color = Color.FromRGB(1,0,0);
			port.PaintOutline(path);
#endif
		}


		protected Document					document;
		protected DrawingContext			drawingContext;
	}
}
