using Epsitec.Common.Pictogram.Widgets;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe IconPrinter implémente l'impression d'un document.
	/// </summary>
	public class IconPrinter
	{
		public IconPrinter()
		{
		}

		public IconObjects IconObjects
		{
			set { this.iconObjects = value; }
		}

		public void Print(Dialogs.Print dp, IconContext iconContext)
		{
			PrintEngine printEngine = new PrintEngine();
			printEngine.IconPrinter = this;
			printEngine.IconContext = iconContext;
			dp.Document.Print(printEngine);
		}


		protected class PrintEngine : Printing.IPrintEngine
		{
			public IconPrinter IconPrinter
			{
				set { this.iconPrinter = value; }
			}

			public IconContext IconContext
			{
				set { this.iconContext = value; }
			}

			#region IPrintEngine Members
			public void PrepareNewPage(Epsitec.Common.Printing.PageSettings settings)
			{
				settings.Margins = new Drawing.Margins(0, 0, 0, 0);
				this.paperSize = settings.PaperSize;
			}
			
			public void StartingPrintJob()
			{
			}
			
			public void FinishingPrintJob()
			{
			}
			
			public Printing.PrintEngineStatus PrintPage(Printing.PrintPort port)
			{
				double zoom = this.paperSize.Width / this.iconPrinter.iconObjects.Size.Width;
#if true
				port.ScaleTransform(zoom, zoom, 0, 0);
#else
				port.TranslateTransform(5, 5);
#endif
				this.iconPrinter.PrintGeometry(port, this.iconContext, 0, true);
				return Printing.PrintEngineStatus.FinishJob;
			}
			#endregion

			protected IconPrinter			iconPrinter;
			protected IconContext			iconContext;
			protected Printing.PaperSize	paperSize;
		}

		
		// Imprime la géométrie de tous les objets.
		protected void PrintGeometry(Printing.PrintPort port,
									 IconContext iconContext,
									 int pageNumber,
									 bool showAllLayers)
		{
			if ( this.iconObjects.Objects.Count == 0 )  return;
			ObjectPattern pattern = this.iconObjects.Objects[0] as ObjectPattern;
			if ( pattern.Objects.Count == 0 )  return;
			ObjectPage page = pattern.Objects[pageNumber] as ObjectPage;
			this.PrintGeometry(page.Objects, port, iconContext, showAllLayers, !showAllLayers);
		}

		protected void PrintGeometry(System.Collections.ArrayList objects,
									 Printing.PrintPort port,
									 IconContext iconContext,
									 bool showAllLayers,
									 bool dimmed)
		{
			System.Collections.ArrayList root = this.iconObjects.CurrentGroup;
			if ( objects == root )  dimmed = false;
			iconContext.IsDimmed = dimmed;

			int total = objects.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				AbstractObject obj = objects[index] as AbstractObject;
				ObjectLayer layer = obj as ObjectLayer;

				if ( obj.Objects != null && obj.Objects.Count > 0 )
				{
					if ( layer != null )
					{

						PropertyModColor modColor = layer.PropertyModColor(0);
						iconContext.modifyColor = new IconContext.ModifyColor(modColor.ModifyColor);

						if ( layer.Actif || showAllLayers )
						{
							this.PrintGeometry(obj.Objects, port, iconContext, showAllLayers, dimmed);
						}
						else
						{
							if ( layer.Type != LayerType.Hide )
							{
								bool newDimmed = dimmed;
								if ( layer.Type == LayerType.Show )  newDimmed = false;
								this.PrintGeometry(obj.Objects, port, iconContext, showAllLayers, newDimmed);
							}
						}
					}
					else
					{
						this.PrintGeometry(obj.Objects, port, iconContext, showAllLayers, dimmed);
					}
				}

				if ( obj is ObjectGroup )
				{
					if ( objects != root )  continue;
				}

				iconContext.IsDimmed = dimmed;
				obj.PrintGeometry(port, iconContext, this.iconObjects);
			}
		}


		protected IconObjects				iconObjects;
	}
}
