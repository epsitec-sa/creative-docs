using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ExportPDF implémente la publication d'un document PDF.
	/// </summary>
	/// 
	public class ExportPDF
	{
		public ExportPDF(Document document)
		{
			this.document = document;
		}

		// Exporte le document dans un fichier.
		public string Export(string filename)
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

			if ( System.IO.File.Exists(filename) )
			{
				System.IO.File.Delete(filename);
			}

			this.FileOpen(filename);

			this.WriteLine("%PDF-1.4");

			this.WriteObj(1);
			this.WriteLine("<< /Type /Catalog /Outlines 2 0 R /Pages 5 0 R >> endobj");

			this.WriteObj(2);
			this.WriteLine("<< /Type /Outlines /Count 0 >> endobj");

			this.WriteObj(3);
			this.port = new PDFPort();
			this.port.SetPageSize(this.document.Size);
			this.WriteLine(this.port.GetPDF());

			this.WriteObj(4);
			this.WriteLine("<< /ProcSet [/PDF /Text] >> endobj");

			this.streamObjRef = 6;
			this.WriteObj(5);
			this.WriteString("<< /Type /Pages /Kids [ ");
			for ( int page=from ; page<=to ; page++ )
			{
				this.WriteString(string.Format("{0} 0 R ", this.streamObjRef++));
			}
			this.WriteLine(string.Format("] /Count {0} >> endobj", total));

			this.streamObjRef -= total;
			for ( int page=from ; page<=to ; page++ )
			{
				this.WriteObj(this.streamObjRef);
				this.WriteLine(string.Format("<< /Type /Page /Parent 5 0 R /MediaBox 3 0 R /Resources 4 0 R /Contents {0} 0 R >> endobj", this.streamObjRef+total));
				this.streamObjRef ++;
			}

			for ( int page=from ; page<=to ; page++ )
			{
				this.port = new PDFPort();
				
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
				this.WriteObj(this.streamObjRef++);
				this.WriteLine(string.Format("<< /Length {0} >>", pdf.Length));
				this.WriteLine("stream");
				this.WriteString(pdf);
				this.WriteLine("endstream endobj");
			}

			int startXref = this.streamOffset;
			this.WriteLine(string.Format("xref 0 {0}", (this.streamObjList.Count+1).ToString()));
			this.WriteLine("0000000000 65535 f");
			for ( int i=0 ; i<this.streamObjList.Count ; i++ )
			{
				int offset = (int)this.streamObjList[i];
				this.WriteLine(string.Format("{0} 00000 n", offset.ToString("D10")));
			}
			this.WriteLine(string.Format("trailer << /Size {0} /Root 1 0 R >>", (this.streamObjList.Count+1).ToString()));
			this.WriteLine("startxref");
			this.WriteLine(string.Format("{0}", startXref));
			this.WriteLine("%%EOF");

			this.FileClose();
			return "";  // ok
		}


		#region File
		// Ouvre le fichier PDF.
		protected void FileOpen(string filename)
		{
			this.streamIO = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew);
			this.streamOffset = 0;
			this.streamObjRef = 0;
			this.streamObjList = new System.Collections.ArrayList();
		}

		// Ecrit une définition d'objet sous la forme "n 0 obj".
		protected void WriteObj(int obj)
		{
			this.streamObjList.Add(this.streamOffset);
			string text = string.Format("{0} 0 obj ", obj.ToString());
			this.WriteString(text);
		}

		// Ecrit une string suivie d'une fin de ligne.
		protected void WriteLine(string line)
		{
			line += "\r\n";
			this.WriteString(line);
		}

		// Ecrit juste une string telle quelle.
		protected void WriteString(string text)
		{
			System.Text.Encoding e = System.Text.Encoding.GetEncoding(1252);
			byte[] array = e.GetBytes(text);
			this.streamIO.Write(array, 0, array.Length);
			this.streamOffset += array.Length;
		}

		// Ferme le fichier PDF.
		protected void FileClose()
		{
			this.streamIO.Close();
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
		protected System.IO.FileStream			streamIO;
		protected int							streamOffset;
		protected int							streamObjRef;
		protected System.Collections.ArrayList	streamObjList;
		protected PDFPort						port;
	}
}
