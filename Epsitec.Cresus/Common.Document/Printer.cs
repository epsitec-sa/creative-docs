using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;

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
		}

		// Imprime le document selon les choix faits dans le dialogue Window (dp)
		// ainsi que dans le dialogue des réglages (PrintInfo).
		public void Print(Epsitec.Common.Dialogs.Print dp)
		{
			PrintEngine printEngine = new PrintEngine();
			printEngine.Initialise(this, dp);
			dp.Document.Print(printEngine);
		}


		protected class PrintEngine : Printing.IPrintEngine
		{
			public void Initialise(Printer printer, Epsitec.Common.Dialogs.Print dp)
			{
				this.printer = printer;
				this.document = printer.document;

				// Crée le DrawingContext utilisé pour l'impression.
				this.drawingContext = new DrawingContext(this.document, null);
				this.drawingContext.ContainerSize = this.document.Size;
				this.drawingContext.PreviewActive = true;

				// Reprend ici tous les choix effectués dans le dialogue Window
				// de l'impression. Même s'il semble possible de les atteindre
				// plus tard avec port.PageSettings.PrinterSettings, cela ne
				// fonctionne pas.
				this.fromPage = dp.Document.PrinterSettings.FromPage;
				this.toPage   = dp.Document.PrinterSettings.ToPage;
				this.copies   = dp.Document.PrinterSettings.Copies;
				this.collate  = dp.Document.PrinterSettings.Collate;
				this.totalPages = this.toPage-this.fromPage+1;
				this.pageCounter = 0;
			}

			#region IPrintEngine Members
			public void PrepareNewPage(Printing.PageSettings settings)
			{
				settings.Margins = new Margins(0, 0, 0, 0);

				if ( this.printer.PrintInfo.AutoLandscape )
				{
					settings.Landscape = (this.document.Size.Width > this.document.Size.Height);
				}
			}
			
			public void StartingPrintJob()
			{
			}
			
			public void FinishingPrintJob()
			{
			}
			
			public Printing.PrintEngineStatus PrintPage(Printing.PrintPort port)
			{
				int page = this.pageCounter;

				if ( this.collate )  // 1,2,3 - 1,2,3 ?
				{
					page %= this.totalPages;
				}
				else	// 1,1 - 2,2 - 3,3 ?
				{
					page /= this.copies;
				}
				page += this.fromPage-1;
				this.printer.PrintGeometry(port, this.drawingContext, page);
				this.pageCounter ++;

				if ( this.pageCounter < this.totalPages*this.copies )
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
			protected int					fromPage;
			protected int					toPage;
			protected int					totalPages;
			protected int					copies;
			protected bool					collate;
		}


		// Imprime la géométrie de tous les objets.
		protected void PrintGeometry(Printing.PrintPort port,
									 DrawingContext drawingContext,
									 int pageNumber)
		{
			System.Diagnostics.Debug.Assert(pageNumber >= 0);
			System.Diagnostics.Debug.Assert(pageNumber < this.document.GetObjects.Count);

			if ( this.PrintInfo.ForceSimply )
			{
				this.PrintSimplyGeometry(port, drawingContext, pageNumber);
			}
			else if ( this.PrintInfo.ForceComplex )
			{
				this.PrintComplexGeometry(port, drawingContext, pageNumber);
			}
			else
			{
				if ( this.IsComplexPrinting(pageNumber) )
				{
					this.PrintComplexGeometry(port, drawingContext, pageNumber);
				}
				else
				{
					this.PrintSimplyGeometry(port, drawingContext, pageNumber);
				}
			}
		}

		// Indique si une impression complexe est nécessaire.
		protected bool IsComplexPrinting(int pageNumber)
		{
			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;

				foreach ( Objects.Abstract obj in this.document.Deep(layer) )
				{
					if ( obj.IsHide )  continue;  // objet caché ?
					if ( obj.IsComplexPrinting )  return true;
				}
			}
			return false;
		}

		// Imprime la géométrie simple de tous les objets, possible lorsque les
		// objets n'utilisent ni les dégradés ni la transparence.
		protected void PrintSimplyGeometry(Printing.PrintPort port,
										   DrawingContext drawingContext,
										   int pageNumber)
		{
			double zoom = 1.0;
			if ( this.PrintInfo.AutoZoom )
			{
				Size paperSize = port.PageSettings.PaperSize.Size;
				double zoomH = paperSize.Width  / this.document.Size.Width;
				double zoomV = paperSize.Height / this.document.Size.Height;
				zoom = System.Math.Min(zoomH, zoomV);
			}
			else
			{
				zoom = 0.1*this.PrintInfo.Zoom;
			}
			port.ScaleTransform(zoom, zoom, 0, 0);
			
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
											int pageNumber)
		{
			double dpi = this.PrintInfo.Dpi;

			Graphics gfx = new Graphics();
			int dx = (int) ((this.document.Size.Width/10.0)*(dpi/25.4));
			int dy = (int) ((this.document.Size.Height/10.0)*(dpi/25.4));
			gfx.SetPixmapSize(dx, dy);
			gfx.SolidRenderer.ClearARGB(1,1,1,1);
			gfx.Rasterizer.Gamma = this.PrintInfo.Gamma;

			double zoom = 1.0;
			if ( this.PrintInfo.AutoZoom )
			{
				double zoomH = dx / this.document.Size.Width;
				double zoomV = dy / this.document.Size.Height;
				zoom = System.Math.Min(zoomH, zoomV);
			}
			else
			{
				zoom = (dpi/25.4/10.0)*this.PrintInfo.Zoom;
			}
			gfx.TranslateTransform(0, dy);
			gfx.ScaleTransform(zoom, -zoom, 0, 0);

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
			Image bitmap = Bitmap.FromPixmap(gfx.Pixmap);
			Rectangle rect = new Rectangle(0, 0, this.document.Size.Width, this.document.Size.Height);
			port.PaintImage(bitmap, rect);

			//?Path path = new Path();
			//?path.AppendRectangle(100, 100, 2100-200, 2970-200);
			//?port.Color = Color.FromRGB(1,0,0);
			//?port.PaintOutline(path);
#else
			Image bitmap = Bitmap.FromPixmap(gfx.Pixmap);
			port.PaintImage(bitmap, 0, 0, 210, 297, 0, 0, bitmap.Width, bitmap.Height);
#endif
		}


		// Donne les réglages de l'impression.
		protected Settings.PrintInfo PrintInfo
		{
			get
			{
				return this.document.Settings.PrintInfo;
			}
		}


		protected Document					document;
	}
}
