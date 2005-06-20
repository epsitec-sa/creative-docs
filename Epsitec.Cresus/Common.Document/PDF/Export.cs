using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe Export implémente la publication d'un document PDF.
	/// </summary>
	/// 
	public class Export
	{
		public Export(Document document)
		{
			this.document = document;
		}

		// Exporte le document dans un fichier.
		public string FileExport(string filename)
		{
			// Crée le DrawingContext utilisé pour l'exportation.
			DrawingContext drawingContext;
			drawingContext = new DrawingContext(this.document, null);
			drawingContext.ContainerSize = this.document.Size;
			drawingContext.PreviewActive = true;

			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;

			Settings.PrintRange range = info.PageRange;
			int from = 1;
			int to   = this.document.Modifier.PrintableTotalPages();

			if ( range == Settings.PrintRange.FromTo )
			{
				from = info.PageFrom;
				to   = info.PageTo;
			}
			else if ( range == Settings.PrintRange.Current )
			{
				int cp = this.document.Modifier.ActiveViewer.DrawingContext.CurrentPage;
				Objects.Page page = this.document.GetObjects[cp] as Objects.Page;
				if ( page.MasterType == Objects.MasterType.Slave )
				{
					from = page.Rank+1;
					to   = from;
				}
				else
				{
					from = cp;
					to   = cp;
				}
			}

			if ( from > to )
			{
				Misc.Swap(ref from, ref to);
			}
			int total = to-from+1;

			this.SearchPatterns(from, to);
			int totalPattern = this.patterns.Count;

			// Crée et ouvre le fichier.
			if ( System.IO.File.Exists(filename) )
			{
				System.IO.File.Delete(filename);
			}

			Writer writer = new Writer(filename);

			// Objet racine du document.
			writer.WriteObjectDef("Root");
			writer.WriteString("<< /Type /Catalog /Outlines ");
			writer.WriteObjectRef("HeaderOutlines");
			writer.WriteString("/Pages ");
			writer.WriteObjectRef("HeaderPages");
			writer.WriteLine(">> endobj");

			// Objet outlines.
			writer.WriteObjectDef("HeaderOutlines");
			writer.WriteLine("<< /Type /Outlines /Count 0 >> endobj");

			// Objet décrivant le format de la page.
			writer.WriteObjectDef("HeaderFormat");
			this.port = new Port(null);
			this.port.SetPageSize(this.document.Size);
			writer.WriteLine(this.port.GetPDF());

			// Objet donnant la liste des pages.
			writer.WriteObjectDef("HeaderPages");
			writer.WriteString("<< /Type /Pages /Kids [ ");
			for ( int page=from ; page<=to ; page++ )
			{
				writer.WriteObjectRef(Export.NamePage(page));
			}
			writer.WriteLine(string.Format("] /Count {0} >> endobj", total));

			// Un objet pour chaque page.
			for ( int page=from ; page<=to ; page++ )
			{
				writer.WriteObjectDef(Export.NamePage(page));
				writer.WriteString("<< /Type /Page /Parent ");
				writer.WriteObjectRef("HeaderPages");
				writer.WriteString("/MediaBox ");
				writer.WriteObjectRef("HeaderFormat");
				writer.WriteString("/Resources ");
				writer.WriteObjectRef(Export.NameResources(page));
				writer.WriteString("/Contents ");
				writer.WriteObjectRef(Export.NameContent(page));
				writer.WriteLine(">> endobj");
			}

			// Un objet pour les ressources de chaque page.
			for ( int page=from ; page<=to ; page++ )
			{
				writer.WriteObjectDef(Export.NameResources(page));
				writer.WriteString("<< /ProcSet [/PDF /Text] ");

				int tp = this.TotalPattern(page);
				if ( tp > 0 )
				{
					writer.WriteString("/Pattern << ");
					for ( int index=0 ; index<tp ; index++ )
					{
						Pattern pattern = this.GetPattern(page, index);
						System.Diagnostics.Debug.Assert(pattern != null);
						writer.WriteString(Export.ShortNamePattern(pattern.Id));
						writer.WriteObjectRef(Export.NamePattern(pattern.Id));
					}
					writer.WriteString(">> ");
				}

				writer.WriteLine("/ColorSpace << /Cs [/Pattern /DeviceRGB] >> >> endobj");
			}

			// Un objet pour chaque pattern.
			this.CreatePattern(writer);

			// Un objet pour le contenu de chaque page.
			for ( int page=from ; page<=to ; page++ )
			{
				this.port = new Port(this.patterns);
				
				System.Collections.ArrayList layers = this.ComputeLayers(page-1);
				foreach ( Objects.Layer layer in layers )
				{
					Properties.ModColor modColor = layer.PropertyModColor;
					drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);
					drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);

					foreach ( Objects.Abstract obj in this.document.Deep(layer) )
					{
						if ( obj.IsHide )  continue;  // objet caché ?

						obj.ExportPDF(this.port, drawingContext);
					}
				}

				string pdf = this.port.GetPDF();
				writer.WriteObjectDef(Export.NameContent(page));
				writer.WriteLine(string.Format("<< /Length {0} >>", pdf.Length));
				writer.WriteLine("stream");
				writer.WriteString(pdf);
				writer.WriteLine("endstream endobj");
			}

			writer.Flush();
			return "";  // ok
		}

		protected static string NamePage(int page)
		{
			return string.Format("HeaderPage{0}", page);
		}

		protected static string NameResources(int page)
		{
			return string.Format("HeaderResources{0}", page);
		}

		protected static string NameContent(int page)
		{
			return string.Format("HeaderContent{0}", page);
		}

		protected static string NamePattern(int pattern)
		{
			return string.Format("HeaderPattern{0}", pattern);
		}

		public static string ShortNamePattern(int pattern)
		{
			return string.Format("/P{0} ", pattern);
		}


		#region Pattern
		// Cherche tous les patterns dans toutes les pages.
		protected void SearchPatterns(int from, int to)
		{
			this.patterns = new System.Collections.ArrayList();

			int id = 1;
			for ( int page=from ; page<=to ; page++ )
			{
				System.Collections.ArrayList layers = this.ComputeLayers(page-1);
				foreach ( Objects.Layer layer in layers )
				{
					foreach ( Objects.Abstract obj in this.document.Deep(layer) )
					{
						if ( obj.IsHide )  continue;  // objet caché ?

						System.Collections.ArrayList list = obj.GetPattensPDF();
						int total = list.Count;
						for ( int rank=0 ; rank<total ; rank++ )
						{
							Pattern pattern = new Pattern();
							pattern.Page = page;
							pattern.Object = obj;
							pattern.Property = list[rank] as Properties.Abstract;
							pattern.Rank = rank;
							pattern.Id = id++;

							this.patterns.Add(pattern);
						}
					}
				}
			}
		}

		// Calcule le nombre de patterns dans une page.
		protected int TotalPattern(int page)
		{
			int total = 0;
			for ( int i=0 ; i<this.patterns.Count ; i++ )
			{
				Pattern pattern = this.patterns[i] as Pattern;
				if ( pattern.Page == page )  total++;
			}
			return total;
		}

		// Donne un pattern.
		protected Pattern GetPattern(int page, int index)
		{
			int ip = 0;
			for ( int i=0 ; i<this.patterns.Count ; i++ )
			{
				Pattern pattern = this.patterns[i] as Pattern;
				if ( pattern.Page == page )
				{
					if ( ip == index )  return pattern;
					ip ++;
				}
			}
			return null;
		}

		// Crée tous les patterns pour une page donnée.
		protected void CreatePattern(Writer writer)
		{
			for ( int i=0 ; i<this.patterns.Count ; i++ )
			{
				Pattern pattern = this.patterns[i] as Pattern;
				Objects.Abstract obj = pattern.Object;

				this.port = new Port(null);
				Size size = obj.CreatePatternPDF(pattern.Rank, this.port);

				string pdf = this.port.GetPDF();

				// /PatternType 1  % Tiling pattern
				// /PaintType 2    % Uncolored
				// /TilingType 1   % Constant spacing
				writer.WriteObjectDef(Export.NamePattern(pattern.Id));
				writer.WriteLine(string.Format("<< /Type /Pattern /PatternType 1 /PaintType 2 /TilingType 1 /BBox [0 0 {0} {1}] /XStep {0} /YStep {1}/Resources << >> /Length {2} >>", Port.StringDistance(size.Width), Port.StringDistance(size.Height), pdf.Length));
				writer.WriteLine("stream");
				writer.WriteString(pdf);
				writer.WriteLine("endstream endobj");
			}
		}
		#endregion


		// Calcule la liste des calques, y compris ceux des pages maîtres.
		// Les calques cachés à l'impression ne sont pas mis dans la liste.
		protected System.Collections.ArrayList ComputeLayers(int pageNumber)
		{
			System.Collections.ArrayList layers = new System.Collections.ArrayList();
			System.Collections.ArrayList masterList = new System.Collections.ArrayList();
			this.document.Modifier.ComputeMasterPageList(masterList, pageNumber);

			// Mets d'abord les premiers calques de toutes les pages maîtres.
			foreach ( Objects.Page master in masterList )
			{
				int frontier = master.MasterFirstFrontLayer;
				for ( int i=0 ; i<frontier ; i++ )
				{
					Objects.Layer layer = master.Objects[i] as Objects.Layer;
					if ( layer.Print == Objects.LayerPrint.Hide )  continue;
					layers.Add(layer);
				}
			}

			// Mets ensuite tous les calques de la page.
			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;
				layers.Add(layer);
			}

			// Mets finalement les derniers calques de toutes les pages maîtres.
			foreach ( Objects.Page master in masterList )
			{
				int frontier = master.MasterFirstFrontLayer;
				int total = master.Objects.Count;
				for ( int i=frontier ; i<total ; i++ )
				{
					Objects.Layer layer = master.Objects[i] as Objects.Layer;
					if ( layer.Print == Objects.LayerPrint.Hide )  continue;
					layers.Add(layer);
				}
			}

			return layers;
		}


		protected Document						document;
		protected System.Collections.ArrayList	patterns;
		protected Port							port;
	}
}
