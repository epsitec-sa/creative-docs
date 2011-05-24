using Epsitec.Common.Drawing;
using System.Collections.Generic;

namespace Epsitec.Common.Document.PDF
{
	using CultureInfo=System.Globalization.CultureInfo;
	using Epsitec.Common.Drawing.Platform;
	/// <summary>
	/// La classe Export implémente la publication d'un document PDF.
	/// [*] = documentation PDF Reference, version 1.6, fifth edition, 1236 pages
	/// </summary>
	/// 
	public class Export
	{
		public Export(Document document)
		{
			this.document = document;
			this.zoom = 1.0;
		}

		public void Dispose()
		{
		}

		public void SetZoom(double zoom)
		{
			this.zoom = zoom;
		}



		public string ExportToFile(string path, Common.Dialogs.IWorkInProgressReport report)
		{
			//	Exporte le document dans un fichier.
			try
			{
				this.report = report;
				this.ExportPdf (path);
				this.DisplayPdf (path);
			}
			catch (PdfExportException ex)
			{
				return ex.Message;
			}
			finally
			{
				this.ClearComplexSurfaces ();
				this.ClearImageSurfaces ();
				this.ClearFonts ();

				this.report = null;
			}

			return "";
		}

		private void ExportPdf(string path)
		{
			var modifier = this.document.Modifier;
			
			this.report.DefineProgress (0, Res.Strings.Export.PDF.Progress.Init);

			this.ExportBegin ();

			var port           = this.CreatePort ();
			var drawingContext = this.CreateDrawingContext ();
			var printableRange = this.GetPrintablePageRange (modifier);
			var printablePages = printableRange.GetPrintablePageList (modifier);

			using (var writer = this.CreateWriter (path))
			{
				this.PreProcessComplexSurfaces (port, drawingContext, printablePages);

				this.EmitHeaderOutlines (writer);
				this.EmitHeaderPages (writer, printableRange, printablePages);
				this.EmitPageObjects (writer, modifier, printablePages);
				this.EmitPageObjectResources (writer, printablePages);

				this.EmitComplexSurfaces (writer, port, drawingContext);
				this.EmitImageSurfaces (writer, port);
				this.EmitFonts (writer);

				this.EmitPageContents (writer, port, drawingContext, modifier, printablePages);
				this.ExportEnd (writer);
			}
		}

		private void DisplayPdf(string path)
		{
			if (this.document.Settings.ExportPDFInfo.Execute)
			{
				System.Diagnostics.Process.Start (path);
			}
		}


		private void ExportBegin()
		{
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;

			this.colorConversion  = info.ColorConversion;
			this.imageCompression = info.ImageCompression;
			this.jpegQuality      = info.JpegQuality;
			this.imageMinDpi      = info.ImageMinDpi;
			this.imageMaxDpi      = info.ImageMaxDpi;
			this.complexSurfaces  = new List<ComplexSurface> ();
			this.imageSurfaces    = new List<ImageSurface> ();
			this.characterHash    = new CharacterHash ();
			this.fontHash         = new FontHash ();
		}

		private void ExportEnd(Writer writer)
		{
			writer.Flush ();
			writer.Finish ();
		}


		private Port CreatePort()
		{
			Port port = new Port (this.complexSurfaces, this.imageSurfaces, this.GetFontHash ());
			port.PushColorModifier (new ColorModifierCallback (this.FinalOutputColorModifier));
			return port;
		}
		private Writer CreateWriter(string path)
		{
			Writer writer;

			try
			{
				if (System.IO.File.Exists (path))
				{
					System.IO.File.Delete (path);
				}

				writer = new Writer (path);
			}
			catch (System.Exception e)
			{
				throw new PdfExportException (e.Message);
			}

			//	Objet racine du document.
			writer.WriteObjectDef ("Root");
			writer.WriteString ("<< /Type /Catalog /Outlines ");
			writer.WriteObjectRef ("HeaderOutlines");
			writer.WriteString ("/Pages ");
			writer.WriteObjectRef ("HeaderPages");
			writer.WriteLine (">> endobj");

			this.documentTitle = Support.Globals.Properties.GetProperty<string> ("PDF:Title") ?? System.IO.Path.GetFileName (path);

			string defaultProducer  = Export.GetDefaultProducerInformation ();
			string defaultCopyright = Export.GetDefaultModuleCopyright ();

			//	Object info
			string titleName    = Export.GetEscapedString (this.documentTitle);
			string author       = Export.GetEscapedString (Support.Globals.Properties.GetProperty<string> ("PDF:Author"));
			string creator      = Export.GetEscapedString (Support.Globals.Properties.GetProperty<string> ("PDF:Creator"));
			string producer     = Export.GetEscapedString (Support.Globals.Properties.GetProperty<string> ("PDF:Producer") ?? string.Format ("{0}, {1}", defaultProducer, defaultCopyright));
			string creationDate = Export.GetDateString (Support.Globals.Properties.GetProperty<System.DateTime> ("PDF:CreationDate", System.DateTime.Now));

			writer.WriteObjectDef ("Info");
			writer.WriteString (string.Concat ("<< /Title (", titleName, ") "));
			if (author != null)
				writer.WriteString (string.Concat ("/Author (", author, ") "));
			if (creator != null)
				writer.WriteString (string.Concat ("/Creator (", creator, ") "));
			if (producer != null)
				writer.WriteString (string.Concat ("/Producer (", producer, ") "));
			writer.WriteString (string.Concat ("/CreationDate (D:", creationDate, ") "));
			writer.WriteString ("/Trapped /False ");
			writer.WriteLine (">> endobj");
			return writer;
		}

		private DrawingContext CreateDrawingContext()
		{
			//	Crée le DrawingContext utilisé pour l'exportation.

			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;

			DrawingContext drawingContext;
			drawingContext = new DrawingContext (this.document, null);
			drawingContext.ContainerSize = this.document.DocumentSize;
			drawingContext.PreviewActive = true;
			drawingContext.TextShowControlCharacters = false;
			drawingContext.VisibleHandles = false;
			drawingContext.SetImageNameFilter (0, info.GetImageNameFilter (0));  // filtre A
			drawingContext.SetImageNameFilter (1, info.GetImageNameFilter (1));  // filtre B
			return drawingContext;
		}

		
		private FontHash GetFontHash()
		{
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;
			FontHash fontHash = info.TextCurve ? null : this.fontHash;
			return fontHash;
		}

		
		private void EmitHeaderOutlines(Writer writer)
		{
			//	Objet outlines.
			writer.WriteObjectDef ("HeaderOutlines");
			writer.WriteLine ("<< /Type /Outlines /Count 0 >> endobj");
		}

		private void EmitHeaderPages(Writer writer, PdfExportPageRange pageRange, IEnumerable<int> pageList)
		{
			//	Objet donnant la liste des pages.
			writer.WriteObjectDef ("HeaderPages");
			writer.WriteString ("<< /Type /Pages /Kids [ ");
			foreach (int page in pageList)
			{
				writer.WriteObjectRef (Export.GetPageName (page));
			}
			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "] /Count {0} >> endobj", pageRange.Total));
		}

		private void EmitPageContents(Writer writer, Port port, DrawingContext drawingContext, Modifier modifier, IEnumerable<int> pageList)
		{
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;
			PdfExportPageOffset offset = new PdfExportPageOffset (info);

			//	Un objet pour le contenu de chaque page.
			foreach (int page in pageList)
			{
				this.report.DefineProgress (0, string.Format (Res.Strings.Export.PDF.Progress.Content, page+1));

				if (this.report.Canceled)
				{
					throw new PdfExportException (Res.Strings.Export.PDF.Progress.Canceled);
				}

				int rank = modifier.PageLocalRank (page);

				this.EmitSinglePageContents (writer, port, drawingContext, offset.GetPageOffset (rank), page);
			}
		}

		private void EmitPageObjects(Writer writer, Modifier modifier, IEnumerable<int> pageList)
		{
			//	Un objet pour chaque page.
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;

			foreach (int page in pageList)
			{
				Rectangle trimBox = new Rectangle (Point.Zero, this.document.GetPageSize (page));

				trimBox.Scale (this.zoom);

				Rectangle bleedBox = trimBox;
				Rectangle mediaBox = trimBox;

				double markSizeX = 0.0;
				double markSizeY = 0.0;

				if (info.PrintCropMarks)  // traits de coupe ?
				{
					markSizeX = info.CropMarksLengthX + info.CropMarksOffsetX;
					markSizeY = info.CropMarksLengthY + info.CropMarksOffsetY;
				}

				double left, right, top, bottom;
				int rank = modifier.PageLocalRank (page);

				if (rank%2 == 1)  // page paire
				{
					left   = info.BleedMargin + info.BleedEvenMargins.Left;
					right  = info.BleedMargin + info.BleedEvenMargins.Right;
					top    = info.BleedMargin + info.BleedEvenMargins.Top;
					bottom = info.BleedMargin + info.BleedEvenMargins.Bottom;
				}
				else
				{
					left   = info.BleedMargin + info.BleedOddMargins.Left;
					right  = info.BleedMargin + info.BleedOddMargins.Right;
					top    = info.BleedMargin + info.BleedOddMargins.Top;
					bottom = info.BleedMargin + info.BleedOddMargins.Bottom;
				}

				//	La boîte de débord ne prend en compte que le débord effectivement
				//	demandé (qui peut être différent entre pages de droite et pages
				//	de gauche).
				bleedBox.Inflate (left, right, top, bottom);

				left   = System.Math.Max (left, markSizeX);
				right  = System.Math.Max (right, markSizeX);
				top    = System.Math.Max (top, markSizeY);
				bottom = System.Math.Max (bottom, markSizeY);

				//	La boîte de média doit être assez grande pour contenir à la fois
				//	le débord et les éventuels traits de coupe.
				mediaBox.Inflate (left, right, top, bottom);

				Point offset = -mediaBox.BottomLeft;
				trimBox.Offset (offset);
				bleedBox.Offset (offset);
				mediaBox.Offset (offset);
				trimBox.Scale (Export.mmToInches);
				bleedBox.Scale (Export.mmToInches);
				mediaBox.Scale (Export.mmToInches);

				writer.WriteObjectDef (Export.GetPageName (page));
				writer.WriteString ("<< /Type /Page /Parent ");
				writer.WriteObjectRef ("HeaderPages");

				//	Définit les boîtes en utilisant soit les valeurs déterminées par CrDoc,
				//	soit des modes forcés par l'appelant externe (ça permet par exemple de
				//	ne pas inclure un MediaBox ou de faire en sorte que le BleedBox ait la
				//	taille du média d'impression).
				switch (Support.Globals.Properties.GetProperty<string> ("PDF:MediaBoxDefinition", "MediaBox"))
				{
					case "MediaBox":
						writer.WriteString (Port.StringBBox ("/MediaBox", mediaBox));
						break;
					case "None":
						break;
				}

				switch (Support.Globals.Properties.GetProperty<string> ("PDF:CropBoxDefinition", "CropBox"))
				{
					case "CropBox":
						writer.WriteString (Port.StringBBox ("/CropBox", mediaBox));
						break;
					case "None":
						break;
				}
				switch (Support.Globals.Properties.GetProperty<string> ("PDF:BleedBoxDefinition", "BleedBox"))
				{
					case "BleedBox":
						writer.WriteString (Port.StringBBox ("/BleedBox", bleedBox));
						break;
					case "MediaBox":
						writer.WriteString (Port.StringBBox ("/MediaBox", mediaBox));
						break;
					case "None":
						break;
				}
				switch (Support.Globals.Properties.GetProperty<string> ("PDF:TrimBoxDefinition", "TrimBox"))
				{
					case "BleedBox":
						writer.WriteString (Port.StringBBox ("/BleedBox", bleedBox));
						break;
					case "MediaBox":
						writer.WriteString (Port.StringBBox ("/MediaBox", mediaBox));
						break;
					case "TrimBox":
						writer.WriteString (Port.StringBBox ("/TrimBox", trimBox));
						break;
					case "None":
						break;
				}

				writer.WriteString ("/Resources ");
				writer.WriteObjectRef (Export.GetPageResourceName (page));
				writer.WriteString ("/Contents ");
				writer.WriteObjectRef (Export.GetPageContentName (page));
				writer.WriteLine (">> endobj");
			}
		}

		private void EmitPageObjectResources(Writer writer, IEnumerable<int> pageList)
		{
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;

			//	Un objet pour les ressources de chaque page.
			foreach (int page in pageList)
			{
				this.report.DefineProgress (0, string.Format (Res.Strings.Export.PDF.Progress.Ressource, page+1));
				if (this.report.Canceled)
				{
					throw new PdfExportException (Res.Strings.Export.PDF.Progress.Canceled);
				}

				writer.WriteObjectDef (Export.GetPageResourceName (page));
				writer.WriteString ("<< /ProcSet [/PDF /Text /ImageB /ImageC] ");

				int tcs = this.GetPageComplexSurfaceCount (page);
				if (tcs > 0 || this.imageSurfaces.Count > 0)
				{
					writer.WriteString ("/ExtGState << ");
					writer.WriteString (Export.GetComplexSurfaceShortName (0, PdfComplexSurfaceType.ExtGState));
					writer.WriteObjectRef (Export.GetComplexSurfaceName (0, PdfComplexSurfaceType.ExtGState));
					for (int index=0; index<tcs; index++)
					{
						ComplexSurface cs = this.GetComplexSurface (page, index);
						System.Diagnostics.Debug.Assert (cs != null);
						if (cs.Type == Type.TransparencyRegular  ||
									 cs.Type == Type.TransparencyGradient ||
									 cs.IsSmooth)
						{
							writer.WriteString (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ExtGState));
							writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ExtGState));
						}
						if (cs.Type == Type.TransparencyPattern)
						{
							writer.WriteString (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ExtGStateP1));
							writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ExtGStateP1));

							writer.WriteString (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ExtGStateP2));
							writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ExtGStateP2));
						}
					}
					writer.WriteString (">> ");

					writer.WriteString ("/Shading << ");
					for (int index=0; index<tcs; index++)
					{
						ComplexSurface cs = this.GetComplexSurface (page, index);
						System.Diagnostics.Debug.Assert (cs != null);
						if (cs.Type == Type.OpaqueGradient       ||
									 cs.Type == Type.TransparencyGradient)
						{
							writer.WriteString (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ShadingColor));
							writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ShadingColor));
						}
					}
					writer.WriteString (">> ");

					writer.WriteString ("/XObject << ");
					for (int index=0; index<tcs; index++)
					{
						ComplexSurface cs = this.GetComplexSurface (page, index);
						System.Diagnostics.Debug.Assert (cs != null);
						if (cs.Type == Type.OpaquePattern       ||
									 cs.Type == Type.TransparencyPattern)
						{
							writer.WriteString (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.XObject));
							writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.XObject));
						}
					}
					foreach (ImageSurface image in this.imageSurfaces)
					{
						if (!image.Exists)
							continue;
						writer.WriteString (Export.GetComplexSurfaceShortName (image.Id, PdfComplexSurfaceType.XObject));
						writer.WriteObjectRef (Export.GetComplexSurfaceName (image.Id, PdfComplexSurfaceType.XObject));
					}
					writer.WriteString (">> ");
				}

				if (!info.TextCurve)
				{
					writer.WriteString ("/Font << ");
					foreach (System.Collections.DictionaryEntry dict in this.fontHash)
					{
						FontList font = dict.Key as FontList;
						int totalPages = (font.CharacterCount+Export.charPerFont-1)/Export.charPerFont;
						for (int fontPage=0; fontPage<totalPages; fontPage++)
						{
							writer.WriteString (Export.GetFontShortName (font.Id, fontPage));
							writer.WriteObjectRef (Export.GetFontName (font.Id, fontPage, PdfFontType.Base));
						}
					}
					writer.WriteString (">> ");
				}

				//?writer.WriteLine("/ColorSpace << /Cs [/Pattern /DeviceRGB] >> ");
				writer.WriteLine (">> endobj");
			}
		}

		private PdfExportPageRange GetPrintablePageRange(Modifier modifier)
		{
			PdfExportPageRange pageRange = new PdfExportPageRange (modifier.PrintableTotalPages ());
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;

			Settings.PrintRange range = info.PageRange;

			if (range == Settings.PrintRange.FromTo)
			{
				pageRange.From = info.PageFrom;
				pageRange.To   = info.PageTo;
			}
			else if (range == Settings.PrintRange.Current)
			{
				int cp = modifier.ActiveViewer.DrawingContext.CurrentPage;
				Objects.Page page = this.document.DocumentObjects[cp] as Objects.Page;
				if (page.MasterType == Objects.MasterType.Slave)
				{
					pageRange.From = page.Rank+1;
					pageRange.To   = pageRange.From;
				}
				else
				{
					pageRange.From = cp;
					pageRange.To   = cp;
					pageRange.JustOneMaster = true;
				}
			}

			pageRange.Constrain ();
			return pageRange;
		}

		private void EmitSinglePageContents(Writer writer, Port port, DrawingContext drawingContext, Point currentPageOffset, int page)
		{
			port.Reset ();

			var gtBeforeZoom = this.SetupTransformForPageExport (port, currentPageOffset);
			var layers = this.GetPrintableLayers (page);

			foreach (Objects.Layer layer in layers)
			{
				Properties.ModColor modColor = layer.PropertyModColor;
				port.PushColorModifier (new ColorModifierCallback (modColor.ModifyColor));
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
				port.PushColorModifier (new ColorModifierCallback (drawingContext.DimmedColor));

				foreach (Objects.Abstract obj in this.document.Deep (layer))
				{
					if (obj.IsHide)
					{
						continue;  // objet caché ?
					}

					obj.DrawGeometry (port, drawingContext);
				}

				port.PopColorModifier ();
				port.PopColorModifier ();
			}

			port.Transform = gtBeforeZoom;

			this.CropToBleedBox (port, page);  // efface ce qui dépasse de la BleedBox
			this.DrawCropMarks (port, page);  // traits de coupe

			string pdf = port.GetPDF ();
			writer.WriteObjectDef (Export.GetPageContentName (page));
			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "<< {0} >>", Port.StringLength (pdf.Length)));
			writer.WriteLine ("stream");
			writer.WriteString (pdf);
			writer.WriteLine ("endstream endobj");
		}

		private Transform SetupTransformForPageExport(Port port, Point currentPageOffset)
		{
			//	Matrice de transformation globale:
			Transform gt = port.Transform;
			gt = gt.Translate (currentPageOffset);  // translation si débord et/ou traits de coupe
			gt = gt.Scale (Export.mmToInches);  // unité = 0.1mm
			Transform gtBeforeZoom = gt;
			gt = gt.Scale (this.zoom, this.zoom, gt.TX, gt.TY);
			port.Transform = gt;
			return gtBeforeZoom;
		}


		private static string GetDefaultModuleCopyright()
		{
			foreach (System.Reflection.AssemblyCopyrightAttribute attribute in typeof (Export).Assembly.GetCustomAttributes (typeof (System.Reflection.AssemblyCopyrightAttribute), false))
			{
				return attribute.Copyright;
			}

			return "-";
		}

		private static string GetDefaultProducerInformation()
		{
			string moduleName = typeof (Export).Assembly.ManifestModule.Name;

			foreach (System.Diagnostics.ProcessModule module in System.Diagnostics.Process.GetCurrentProcess ().Modules)
			{
				if (string.Compare (module.ModuleName, moduleName, System.StringComparison.OrdinalIgnoreCase) == 0)
				{
					return string.Format ("{0} {1}", moduleName, module.FileVersionInfo.FileVersion);
				}
			}

			return moduleName;
		}

		private static string GetDateString(System.DateTime dateTime)
		{
			System.DateTime utc = dateTime.ToUniversalTime ();
			return string.Format (CultureInfo.InvariantCulture, "{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}Z", utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second);
		}

		private static string GetEscapedString(string name)
		{
			if (name == null)
			{
				return null;
			}

			name = name.Replace (@"\", @"\\");
			name = name.Replace (@"(", @"\(");
			name = name.Replace (@")", @"\)");

			return name;
		}

		private static string GetPageName(int page)
		{
			return string.Format (CultureInfo.InvariantCulture, "HeaderPage{0}", page);
		}

		private static string GetPageResourceName(int page)
		{
			return string.Format (CultureInfo.InvariantCulture, "HeaderResources{0}", page);
		}

		private static string GetPageContentName(int page)
		{
			return string.Format (CultureInfo.InvariantCulture, "HeaderContent{0}", page);
		}

		private static string GetComplexSurfaceName(int id, PdfComplexSurfaceType type)
		{
			return string.Format (CultureInfo.InvariantCulture, "HeaderComplexSurface{0}", id*10+(int) type);
		}

		public static string GetComplexSurfaceShortName(int id, PdfComplexSurfaceType type)
		{
			return string.Format (CultureInfo.InvariantCulture, "/S{0} ", id*10+(int) type);
		}

		private static string GetFontName(int id, int fontPage, PdfFontType type)
		{
			return string.Format (CultureInfo.InvariantCulture, "HeaderFont{0}", id*100 + fontPage*10 + (int) type);
		}

		public static string GetFontShortName(int id, int fontPage)
		{
			return string.Format (CultureInfo.InvariantCulture, "/F{0} ", id*10 + fontPage);
		}

		private static string GetCharacterName(int id, int fontPage, CharacterList cl)
		{
			return string.Format (CultureInfo.InvariantCulture, "HeaderCharacter{0}_{1}_{2}_{3}", id, fontPage, cl.Glyph, cl.Unicode);
		}

		private static string GetCharacterShortName(int id, int fontPage, CharacterList cl)
		{
			return string.Format (CultureInfo.InvariantCulture, "/C{0}_{1}_{2}_{3} ", id, fontPage, cl.Glyph, cl.Unicode);
		}

		private static string GetFunctionName(int id, PdfFunctionType type)
		{
			return string.Format (CultureInfo.InvariantCulture, "HeaderFunction{0}", id*10+(int) type);
		}


		private RichColor FinalOutputColorModifier(RichColor color)
		{
			//	Modification finale d'une couleur en fonction du mode de sortie.
			if (this.colorConversion == PDF.ColorConversion.ToRgb)
			{
				color.ColorSpace = ColorSpace.Rgb;
			}
			else if (this.colorConversion == PDF.ColorConversion.ToCmyk)
			{
				color.ColorSpace = ColorSpace.Cmyk;
			}
			else if (this.colorConversion == PDF.ColorConversion.ToGray)
			{
				color.ColorSpace = ColorSpace.Gray;
			}

			return color;
		}


		private void CropToBleedBox(Port port, int page)
		{
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;
			//	Dessine un masque avec une ouverture qui a exactement la taille
			//	du BleedBox (à savoir la taille de la page avec ses débords).
			Size pageSize = this.document.GetPageSize (page);

			double width  = pageSize.Width * this.zoom;
			double height = pageSize.Height * this.zoom;

			double left, right, top, bottom;
			int rank = this.document.Modifier.PageLocalRank (page);

			if (rank%2 == 1)  // page paire
			{
				left   = info.BleedMargin + info.BleedEvenMargins.Left;
				right  = info.BleedMargin + info.BleedEvenMargins.Right;
				top    = info.BleedMargin + info.BleedEvenMargins.Top;
				bottom = info.BleedMargin + info.BleedEvenMargins.Bottom;
			}
			else
			{
				left   = info.BleedMargin + info.BleedOddMargins.Left;
				right  = info.BleedMargin + info.BleedOddMargins.Right;
				top    = info.BleedMargin + info.BleedOddMargins.Top;
				bottom = info.BleedMargin + info.BleedOddMargins.Bottom;
			}

			if (Support.Globals.Properties.IsPropertyDefined ("PDF:DisableClipToBleedBox"))
			{
				//	Pas de clipping...
			}
			else
			{
				using (Path path = new Path ())
				{
					double clip = 1000.0;

					path.MoveTo (0.0-clip, 0.0-clip);
					path.LineTo (width+clip, 0.0-clip);
					path.LineTo (width+clip, height+clip);
					path.LineTo (0.0-clip, height+clip);
					path.LineTo (0.0-clip, 0.0-clip);
					path.Close ();

					path.MoveTo (0.0-left, 0.0-bottom);
					path.LineTo (0.0-left, height+top);
					path.LineTo (width+right, height+top);
					path.LineTo (width+right, 0.0-bottom);
					path.LineTo (0.0-left, 0.0-bottom);
					path.Close ();

					port.RichColor = RichColor.FromCmyk (0.0, 0.0, 0.0, 0.0);  // masque blanc
					port.PaintSurface (path);
				}
			}
		}

		private void DrawCropMarks(Port port, int page)
		{
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;
			//	Dessine les traits de coupe.
			if (!info.PrintCropMarks)
				return;

			Size pageSize = this.document.GetPageSize (page);

			double width  = pageSize.Width * this.zoom;
			double height = pageSize.Height * this.zoom;
			double offsetX = info.CropMarksOffsetX;
			double offsetY = info.CropMarksOffsetY;
			double lengthX = info.CropMarksLengthX;
			double lengthY = info.CropMarksLengthY;

			using (Path path = new Path ())
			{
				//	Traits horizontaux.
				path.MoveTo (0.0-offsetX, 0.0);
				path.LineTo (0.0-offsetX-lengthX, 0.0);

				path.MoveTo (0.0-offsetX, height);
				path.LineTo (0.0-offsetX-lengthX, height);

				path.MoveTo (width+offsetX, 0.0);
				path.LineTo (width+offsetX+lengthX, 0.0);

				path.MoveTo (width+offsetX, height);
				path.LineTo (width+offsetX+lengthX, height);

				//	Traits verticaux.
				path.MoveTo (0.0, 0.0-offsetY);
				path.LineTo (0.0, 0.0-offsetY-lengthY);

				path.MoveTo (width, 0.0-offsetY);
				path.LineTo (width, 0.0-offsetY-lengthY);

				path.MoveTo (0.0, height+offsetY);
				path.LineTo (0.0, height+offsetY+lengthY);

				path.MoveTo (width, height+offsetY);
				path.LineTo (width, height+offsetY+lengthY);

				port.LineWidth = info.CropMarksWidth * 3;
				port.LineCap   = CapStyle.Butt;
				port.RichColor = RichColor.FromCmyk (0.0, 0.0, 0.0, 0.0);  // fond blanc derrière les traits de coupe
				port.PaintOutline (path);

				port.LineWidth = info.CropMarksWidth;
				port.LineCap   = CapStyle.Butt;
				port.RichColor = RichColor.FromCmyk (1.0, 1.0, 1.0, 1.0);  // noir de repérage
				port.PaintOutline (path);
			}

			using (Path path = new Path ())
			{
				int rank = this.document.Modifier.PageLocalRank (page);
				double x = lengthY;
				double y = 0.0-offsetY-lengthY/2.0;
				double size = System.Math.Min (20.0, lengthY * 0.6);
				string text = string.Format ("{0} : {1}", this.documentTitle, rank+1);
				Font   font = Font.GetFont ("Arial", "Regular");

				double dx = font.GetTextAdvance (text) * size;

				path.AppendRoundedRectangle (x-x/2, y+size*font.Descender, dx+2*x/2, size, x/8);
				port.RichColor = RichColor.FromCmyk (0.0, 0.0, 0.0, 0.0);  // fond blanc
				port.PaintSurface (path);

				path.Clear ();
				path.Append (font, text, x, y, size);
				port.RichColor = RichColor.FromCmyk (1.0, 1.0, 1.0, 1.0);  // noir de repérage
				port.PaintSurface (path);
			}
		}


		#region ComplexSurface
		private void PreProcessComplexSurfaces(Port port, DrawingContext drawingContext, IEnumerable<int> pageList)
		{
			//	Trouve toutes les surfaces complexes dans toutes les pages.
			port.Reset ();

			//	Il faut utiliser la bonne échelle à cause du filtre des images.
			Transform gt = port.Transform;
			gt = gt.Scale (Export.mmToInches);  // unité = 0.1mm
			port.Transform = gt;

			int id = 1;
			foreach (int page in pageList)
			{
				var layers = this.GetPrintableLayers (page);
				foreach (Objects.Layer layer in layers)
				{
					Properties.ModColor modColor = layer.PropertyModColor;
					port.PushColorModifier (new ColorModifierCallback (modColor.ModifyColor));
					drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
					port.PushColorModifier (new ColorModifierCallback (drawingContext.DimmedColor));

					int objIndex = 1;
					foreach (Objects.Abstract obj in this.document.Deep (layer))
					{
						if (obj.IsHide)
						{
							continue;  // objet caché ?
						}

						this.report.DefineProgress (0, string.Format (Res.Strings.Export.PDF.Progress.Surface, page+1, objIndex++));

						if (this.report.Canceled)
						{
							throw new PdfExportException (Res.Strings.Export.PDF.Progress.Canceled);
						}

						this.PreProcessSurfaceOfObjectProperties (port, page, layer, obj, ref id);
						this.PreProcessSurfaceOfImageObjects (port, drawingContext, obj, ref id);
						this.PreProcessCharactersOfTextObjects (port, drawingContext, obj);
					}

					port.PopColorModifier ();
					port.PopColorModifier ();
				}
			}

			FontList.CreateFonts (this.fontHash, this.characterHash);
		}

		private void PreProcessSurfaceOfObjectProperties(Port port, int page, Objects.Layer layer, Objects.Abstract obj, ref int id)
		{
			var list = obj.GetPdfComplexSurfaces (port);

			for (int rank = 0; rank < list.Count; rank++)
			{
				Properties.Gradient gradient = list[rank] as Properties.Gradient;
				Properties.Font     font     = list[rank] as Properties.Font;

				if (gradient != null)
				{
					Properties.Line stroke = (gradient.Type == Properties.Type.LineColor) ? obj.PropertyLineMode : null;

					this.complexSurfaces.Add (new ComplexSurface (page, port, layer, obj, gradient, stroke, rank, id++));
				}

				if (font != null)
				{
					this.complexSurfaces.Add (new ComplexSurface (page, port, layer, obj, font, null, rank, id++));
				}
			}
		}
		
		private void PreProcessSurfaceOfImageObjects(Port port, DrawingContext drawingContext, Objects.Abstract obj, ref int id)
		{
			Objects.Image    objImage  = obj as Objects.Image;
			Properties.Image propImage = obj.PropertyImage;

			if ((objImage != null) && (propImage != null))
			{
				string          path     = propImage.FileName;
				System.DateTime filedate = propImage.FileDate;
				Margins         crop     = propImage.CropMargins;
				ImageFilter     filter   = objImage.GetFilter (port, drawingContext);
				Size            size     = objImage.ImageBitmapSize;

				if (ImageSurface.Search (this.imageSurfaces, path, size, crop, filter) == null)
				{
					ImageCache.Item item = this.document.ImageCache.Find (path, filedate);

					if (item != null)
					{
						this.imageSurfaces.Add (new ImageSurface (item, size, crop, filter, id++));
					}
				}
			}
		}
		
		private void PreProcessCharactersOfTextObjects(Port port, DrawingContext drawingContext, Objects.Abstract obj)
		{
			obj.FillOneCharList (port, drawingContext, this.characterHash);
		}
		
		private int GetPageComplexSurfaceCount(int page)
		{
			//	Calcule le nombre de surfaces complexes dans une page.
			
			int total = 0;
			
			foreach (var cs in this.complexSurfaces)
			{
				if (cs.Page == page)
				{
					total++;
				}
			}
			
			return total;
		}

		private ComplexSurface GetComplexSurface(int page, int index)
		{
			//	Donne une surface complexe.
			int ip = 0;
			
			foreach (var cs in this.complexSurfaces)
			{
				if (cs.Page == page)
				{
					if (ip++ == index)
					{
						return cs;
					}
				}
			}
			
			return null;
		}

		private void EmitComplexSurfaces(Writer writer, Port port, DrawingContext drawingContext)
		{
			//	Crée toutes les surfaces complexes.
			//	Crée le ExtGState numéro 0, pour annuler une transparence.
			writer.WriteObjectDef (Export.GetComplexSurfaceName (0, PdfComplexSurfaceType.ExtGState));
			writer.WriteLine ("<< /CA 1 /ca 1 >> endobj");

			for (int i=0; i<this.complexSurfaces.Count; i++)
			{
				ComplexSurface cs = this.complexSurfaces[i] as ComplexSurface;

				Objects.Layer layer = cs.Layer;
				Properties.ModColor modColor = layer.PropertyModColor;
				port.PushColorModifier (new ColorModifierCallback (modColor.ModifyColor));
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
				port.PushColorModifier (new ColorModifierCallback (drawingContext.DimmedColor));

				cs.Object.SurfaceAnchor.LineUse = cs.Fill.IsStrokingGradient;
				this.MatrixComplexSurfaceGradient (cs);

				switch (cs.Type)
				{
					case PDF.Type.OpaqueRegular:
						this.CreateComplexSurfaceTransparencyGradientOrSmooth (writer, port, drawingContext, cs);
						break;

					case PDF.Type.TransparencyRegular:
						this.CreateComplexSurfaceTransparencyRegular (writer, port, drawingContext, cs);
						break;

					case PDF.Type.OpaqueGradient:
						this.CreateComplexSurfaceOpaqueGradient (writer, port, drawingContext, cs);
						break;

					case PDF.Type.TransparencyGradient:
						this.CreateComplexSurfaceTransparencyGradientOrSmooth (writer, port, drawingContext, cs);
						break;

					case PDF.Type.OpaquePattern:
						this.CreateComplexSurfaceOpaquePattern (writer, port, drawingContext, cs);
						break;

					case PDF.Type.TransparencyPattern:
						this.CreateComplexSurfaceTransparencyPattern (writer, port, drawingContext, cs);
						this.CreateComplexSurfaceOpaquePattern (writer, port, drawingContext, cs);
						break;
				}

				port.PopColorModifier ();
				port.PopColorModifier ();
			}
		}

		private void CreateComplexSurfaceTransparencyRegular(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface transparente unie.
			if (cs.IsSmooth)
			{
				this.CreateComplexSurfaceTransparencyGradientOrSmooth (writer, port, drawingContext, cs);
			}
			else
			{
				double a = 1.0;
				if (cs.Fill is Properties.Gradient)
				{
					Properties.Gradient gradient = cs.Fill as Properties.Gradient;
					a = port.GetFinalColor (gradient.Color1.Basic).A;
				}
				if (cs.Fill is Properties.Font)
				{
					Properties.Font font = cs.Fill as Properties.Font;
					a = port.GetFinalColor (font.FontColor.Basic).A;
				}

				this.CreateComplexSurfaceAlpha (writer, a, cs.Id, PdfComplexSurfaceType.ExtGState);
			}
		}

		private void CreateComplexSurfaceTransparencyPattern(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée deux surfaces transparentes unies pour un motif.
			double a1 = 1.0;
			double a2 = 1.0;
			if (cs.Fill is Properties.Gradient)
			{
				Properties.Gradient gradient = cs.Fill as Properties.Gradient;
				a1 = port.GetFinalColor (gradient.Color1.Basic).A;
				a2 = port.GetFinalColor (gradient.Color2.Basic).A;
			}
			if (cs.Fill is Properties.Font)
			{
				Properties.Font font = cs.Fill as Properties.Font;
				a1 = port.GetFinalColor (font.FontColor.Basic).A;
				a2 = a1;
			}

			this.CreateComplexSurfaceAlpha (writer, a1, cs.Id, PdfComplexSurfaceType.ExtGStateP1);
			this.CreateComplexSurfaceAlpha (writer, a2, cs.Id, PdfComplexSurfaceType.ExtGStateP2);
		}

		private void CreateComplexSurfaceAlpha(Writer writer, double alpha, int id, PdfComplexSurfaceType type)
		{
			//	Crée un ExtGState pour une transparence unie.
			writer.WriteObjectDef (Export.GetComplexSurfaceName (id, type));
			Port port = new Port ();
			port.PutCommand ("<< /CA ");  // voir [*] page 192
			port.PutValue (alpha, 3);
			port.PutCommand ("/ca ");
			port.PutValue (alpha, 3);
			port.PutCommand (">> endobj");
			writer.WriteLine (port.GetPDF ());
		}

		public static void SurfaceInflate(ComplexSurface cs, ref Rectangle bbox)
		{
			//	Engraisse une bbox en fonction d'une surface floue.
			if (cs.Fill is Properties.Gradient)
			{
				Properties.Gradient surface = cs.Fill as Properties.Gradient;
				Properties.Line     stroke  = cs.Stroke;

				double width = 0.0;

				if (stroke != null && surface != null)
				{
					width += surface.InflateBoundingBoxWidth ();
					width += stroke.InflateBoundingBoxWidth ();
					width *= stroke.InflateBoundingBoxFactor ();
				}
				else if (surface != null)
				{
					width += surface.InflateBoundingBoxWidth ();
				}

				bbox.Inflate (width);
			}
		}

		private void CreateComplexSurfaceTransparencyGradientOrSmooth(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface dégradée transparente et/ou floue.
			Objects.Abstract obj = cs.Object;
			SurfaceAnchor sa = obj.SurfaceAnchor;

			//	ExtGState.
			writer.WriteObjectDef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ExtGState));
			writer.WriteString ("<< /SMask << /S /Luminosity /G ");  // voir [*] page 521
			writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.XObject));
			writer.WriteLine (">> >> endobj");

			//	XObject.
			Rectangle bbox = sa.BoundingBox;
			Export.SurfaceInflate (cs, ref bbox);

			writer.WriteObjectDef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.XObject));
			writer.WriteString ("<< /Subtype /Form /FormType 1 ");    // voir [*] page 328
			writer.WriteString (Port.StringBBox (bbox));

			writer.WriteString ("/Resources << ");

			if (cs.IsSmooth)
			{
				writer.WriteString ("/ExtGState << ");
				writer.WriteString (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ExtGStateSmooth));
				writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ExtGStateSmooth));
				writer.WriteString (">> ");
			}

			writer.WriteString ("/Shading << ");
			writer.WriteString (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ShadingGray));
			writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ShadingGray));
			writer.WriteString (">> ");

			writer.WriteString (">> ");
			writer.WriteString ("/Group << /S /Transparency /CS /DeviceGray >> ");  // voir [*] page 525

			Path path = new Path ();
			path.AppendRectangle (bbox);
			port.Reset ();
			if (cs.IsSmooth)
			{
				port.PutCommand ("q ");
				port.PutCommand (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ExtGStateSmooth));
				port.PutCommand ("gs ");
				port.PutPath (path);
				port.PutCommand ("W n ");
				port.PutTransform (cs.Matrix);
				port.PutCommand (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ShadingGray));
				port.PutCommand ("sh Q ");  // shading, voir [*] page 273
			}
			else
			{
				port.PutPath (path);
				port.PutCommand ("q W n ");
				port.PutTransform (cs.Matrix);
				port.PutCommand (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ShadingGray));
				port.PutCommand ("sh Q ");  // shading, voir [*] page 273
			}
			port.PutEOL ();

			string pdf = port.GetPDF ();
			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "{0} >>", Port.StringLength (pdf.Length)));
			writer.WriteLine ("stream");
			writer.WriteString (pdf);
			writer.WriteLine ("endstream endobj");

			this.CreateComplexSurfaceGradient (writer, port, drawingContext, cs, 0);

			if (cs.Type == PDF.Type.OpaqueGradient       ||
				 cs.Type == PDF.Type.TransparencyGradient)
			{
				int nbColors = this.ComplexSurfaceGradientNbColors (port, drawingContext, cs);
				this.CreateComplexSurfaceGradient (writer, port, drawingContext, cs, nbColors);
			}

			if (cs.IsSmooth)
			{
				this.CreateComplexSurfaceSmooth (writer, port, drawingContext, cs);
			}
		}

		private void CreateComplexSurfaceSmooth(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface floue.
			Objects.Abstract obj = cs.Object;
			SurfaceAnchor sa = obj.SurfaceAnchor;

			Properties.Gradient surface = cs.Fill as Properties.Gradient;
			Properties.Line     stroke  = cs.Stroke;

			//	ExtGStateSmooth.
			writer.WriteObjectDef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ExtGStateSmooth));
			writer.WriteString ("<< /SMask << /S /Luminosity /G ");  // voir [*] page 521
			writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.XObjectSmooth));
			writer.WriteLine (">> >> endobj");

			//	XObjectSmooth.
			Rectangle bbox = sa.BoundingBox;
			Export.SurfaceInflate (cs, ref bbox);

			writer.WriteObjectDef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.XObjectSmooth));
			writer.WriteString ("<< /Subtype /Form /FormType 1 ");    // voir [*] page 328
			writer.WriteString (Port.StringBBox (bbox));
			writer.WriteString ("/Group << /S /Transparency /CS /DeviceGray >> ");  // voir [*] page 525

			double    width = 0.0;
			CapStyle  cap   = CapStyle.Round;
			JoinStyle join  = JoinStyle.Round;
			double    limit = 5.0;

			if (stroke != null)
			{
				width = stroke.Width;
				cap   = stroke.Cap;
				join  = stroke.Join;
				limit = stroke.Limit;
			}

			port.Reset ();
			port.ColorForce = ColorForce.Gray;
			port.LineCap = cap;
			port.LineJoin = join;
			port.LineMiterLimit = limit;

			Shape[] shapes = obj.ShapesBuild (port, drawingContext, false);

			int step = (int) (surface.Smooth*drawingContext.ScaleX*2);
			if (step > 20)
				step = 20;
			if (step <  2)
				step =  2;
			foreach (Shape shape in shapes)
			{
				if (shape.PropertySurface != surface)
					continue;

				for (int i=0; i<step; i++)
				{
					double intensity = (i+1.0)/step;
					double w = surface.Smooth-i*surface.Smooth/step;
					port.RichColor = RichColor.FromGray (intensity);
					port.LineWidth = width + w*2.0;
					port.PaintOutline (shape.Path);
				}

				if (stroke == null)
				{
					port.RichColor = RichColor.FromGray (1.0);
					port.PaintSurface (shape.Path);
				}
			}

			string pdf = port.GetPDF ();
			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "{0} >>", Port.StringLength (pdf.Length)));
			writer.WriteLine ("stream");
			writer.WriteString (pdf);
			writer.WriteLine ("endstream endobj");
		}

		private void CreateComplexSurfaceOpaqueGradient(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface dégradée transparente.
			if (cs.IsSmooth)
			{
				this.CreateComplexSurfaceTransparencyGradientOrSmooth (writer, port, drawingContext, cs);
			}
			else
			{
				int nbColors = this.ComplexSurfaceGradientNbColors (port, drawingContext, cs);
				this.CreateComplexSurfaceGradient (writer, port, drawingContext, cs, nbColors);
			}
		}

		private void MatrixComplexSurfaceGradient(ComplexSurface cs)
		{
			//	Calcule la matrice de transformation pour une surface dégradée.
			Properties.Gradient gradient = cs.Fill as Properties.Gradient;
			if (gradient == null)
				return;

			Objects.Abstract obj = cs.Object;
			SurfaceAnchor sa = obj.SurfaceAnchor;
			Rectangle t = cs.Object.BoundingBox;  // update bounding box !
			Point center = sa.ToAbs (new Point (gradient.Cx, gradient.Cy));

			if (gradient.FillType == Properties.GradientFillType.Linear)
			{
				double sx = sa.Width *gradient.Sx;
				double sy = sa.Height*gradient.Sy;
				double angle = Point.ComputeAngleDeg (sx, sy)-90.0;
				double length = System.Math.Sqrt (sx*sx + sy*sy);

				cs.Matrix = cs.Matrix.RotateDeg (obj.Direction+angle);
				cs.Matrix = cs.Matrix.Scale (length, length);
				cs.Matrix = cs.Matrix.Translate (center);
			}

			if (gradient.FillType == Properties.GradientFillType.Circle)
			{
				double dx = System.Math.Abs (sa.Width *gradient.Sx);
				double dy = System.Math.Abs (sa.Height*gradient.Sy);

				cs.Matrix = cs.Matrix.RotateDeg (obj.Direction);
				cs.Matrix = cs.Matrix.Scale (dx, dy);
				cs.Matrix = cs.Matrix.Translate (center);
			}

			if (gradient.FillType == Properties.GradientFillType.Diamond ||
				 gradient.FillType == Properties.GradientFillType.Conic)
			{
				double dx = System.Math.Abs (sa.Width *gradient.Sx);
				double dy = System.Math.Abs (sa.Height*gradient.Sy);

				cs.Matrix = cs.Matrix.Scale (dx, dy);
				cs.Matrix = cs.Matrix.Translate (center);
				cs.Matrix = cs.Matrix.RotateDeg (sa.Direction+gradient.Angle, center);
			}
		}

		private void CreateComplexSurfaceGradient(Writer writer,
													Port port,
													DrawingContext drawingContext,
													ComplexSurface cs,
													int nbColors)
		{
			//	Crée une surface dégradée.
			//	Si nbColors == 0  ->  canal alpha
			//	Si nbColors == 1  ->  espace Gray
			//	Si nbColors == 3  ->  espace RGB
			//	Si nbColors == 4  ->  espace CMYK
			Properties.Gradient gradient = cs.Fill as Properties.Gradient;
			Objects.Abstract obj = cs.Object;
			SurfaceAnchor sa = obj.SurfaceAnchor;
			Rectangle t = cs.Object.BoundingBox;  // update bounding box !
			port.Reset ();

			int totalColors = System.Math.Max (nbColors, 1);

			PdfComplexSurfaceType tcs = (nbColors == 0) ? PdfComplexSurfaceType.ShadingGray : PdfComplexSurfaceType.ShadingColor;

			if (gradient.FillType == Properties.GradientFillType.None)
			{
				writer.WriteObjectDef (Export.GetComplexSurfaceName (cs.Id, tcs));

				//	Axial shading, voir [*] page 279
				port.PutCommand ("<< /ShadingType 2 /Coords [0 -1 0 1] ");

				if (nbColors <= 1)
					port.PutCommand ("/ColorSpace /DeviceGray ");
				if (nbColors == 3)
					port.PutCommand ("/ColorSpace /DeviceRGB ");
				if (nbColors == 4)
					port.PutCommand ("/ColorSpace /DeviceCMYK ");

				port.PutCommand ("/Function << /FunctionType 2 /Domain [0 1] /C0 [");
				RichColor c1 = port.GetFinalColor (gradient.Color1);
				RichColor c2 = port.GetFinalColor (gradient.Color2);
				if (nbColors == 1)  // gray ?
				{
					c1.ColorSpace = ColorSpace.Gray;
					c2.ColorSpace = ColorSpace.Gray;
				}
				if (nbColors == 3)  // rgb ?
				{
					c1.ColorSpace = ColorSpace.Rgb;
					c2.ColorSpace = ColorSpace.Rgb;
				}
				if (nbColors == 4)  // cmyk ?
				{
					c1.ColorSpace = ColorSpace.Cmyk;
					c2.ColorSpace = ColorSpace.Cmyk;
				}
				if (nbColors == 0)
					port.PutValue (c1.A, 3);
				else
					port.PutColor (c1);
				port.PutCommand ("] /C1 [");
				if (nbColors == 0)
					port.PutValue (c1.A, 3);  // c1 to c1 (flat) !
				else
					port.PutColor (c1);
				port.PutCommand ("] /N 1 >> ");

				port.PutCommand ("/Extend [true true] >> endobj");
				writer.WriteLine (port.GetPDF ());
			}

			if (gradient.FillType == Properties.GradientFillType.Linear ||
				 gradient.FillType == Properties.GradientFillType.Circle)
			{
				if (gradient.Middle != 0.0 || gradient.Repeat != 1)
				{
					Export.FunctionColorSampled (writer, port, drawingContext, cs, nbColors);
				}

				writer.WriteObjectDef (Export.GetComplexSurfaceName (cs.Id, tcs));

				if (gradient.FillType == Properties.GradientFillType.Linear)
				{
					//	Axial shading, voir [*] page 279
					port.PutCommand ("<< /ShadingType 2 /Coords [0 -1 0 1] ");
				}

				if (gradient.FillType == Properties.GradientFillType.Circle)
				{
					//	Radial shading, voir [*] page 280
					port.PutCommand ("<< /ShadingType 3 /Coords [0 0 0 0 0 1] ");
				}

				if (nbColors <= 1)
					port.PutCommand ("/ColorSpace /DeviceGray ");
				if (nbColors == 3)
					port.PutCommand ("/ColorSpace /DeviceRGB ");
				if (nbColors == 4)
					port.PutCommand ("/ColorSpace /DeviceCMYK ");

				if (gradient.Middle == 0.0 && gradient.Repeat == 1)
				{
					//	Exponential interpolation function, voir [*] page 146
					port.PutCommand ("/Function << /FunctionType 2 /Domain [0 1] /C0 [");
					RichColor c1 = port.GetFinalColor (gradient.Color1);
					RichColor c2 = port.GetFinalColor (gradient.Color2);
					if (nbColors == 1)  // gray ?
					{
						c1.ColorSpace = ColorSpace.Gray;
						c2.ColorSpace = ColorSpace.Gray;
					}
					if (nbColors == 3)  // rgb ?
					{
						c1.ColorSpace = ColorSpace.Rgb;
						c2.ColorSpace = ColorSpace.Rgb;
					}
					if (nbColors == 4)  // cmyk ?
					{
						c1.ColorSpace = ColorSpace.Cmyk;
						c2.ColorSpace = ColorSpace.Cmyk;
					}
					if (nbColors == 0)
						port.PutValue (c1.A, 3);
					else
						port.PutColor (c1);
					port.PutCommand ("] /C1 [");
					if (nbColors == 0)
						port.PutValue (c2.A, 3);
					else
						port.PutColor (c2);
					port.PutCommand ("] /N 1 >> ");
				}
				else
				{
					port.PutCommand ("/Function ");
					writer.WriteString (port.GetPDF ());
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, (nbColors == 0) ? PdfFunctionType.SampledAlpha : PdfFunctionType.SampledColor));
					port.Reset ();
				}

				port.PutCommand ("/Extend [true true] >> endobj");
				writer.WriteLine (port.GetPDF ());
			}

			if (gradient.FillType == Properties.GradientFillType.Diamond ||
				 gradient.FillType == Properties.GradientFillType.Conic)
			{
				//	Calcule le domaine (pfff...).
				double w = sa.Width;
				double h = sa.Height;
				Point p1 = new Point ((0.0-gradient.Cx)*w, (0.0-gradient.Cy)*h);
				Point p2 = new Point ((1.0-gradient.Cx)*w, (0.0-gradient.Cy)*h);
				Point p3 = new Point ((0.0-gradient.Cx)*w, (1.0-gradient.Cy)*h);
				Point p4 = new Point ((1.0-gradient.Cx)*w, (1.0-gradient.Cy)*h);
				p1 = Transform.RotatePointDeg (-gradient.Angle, p1);
				p2 = Transform.RotatePointDeg (-gradient.Angle, p2);
				p3 = Transform.RotatePointDeg (-gradient.Angle, p3);
				p4 = Transform.RotatePointDeg (-gradient.Angle, p4);
				double domainMinX = System.Math.Min (System.Math.Min (p1.X, p2.X), System.Math.Min (p3.X, p4.X));
				double domainMaxX = System.Math.Max (System.Math.Max (p1.X, p2.X), System.Math.Max (p3.X, p4.X));
				double domainMinY = System.Math.Min (System.Math.Min (p1.Y, p2.Y), System.Math.Min (p3.Y, p4.Y));
				double domainMaxY = System.Math.Max (System.Math.Max (p1.Y, p2.Y), System.Math.Max (p3.Y, p4.Y));
				domainMinX /= w*System.Math.Abs (gradient.Sx);
				domainMaxX /= w*System.Math.Abs (gradient.Sx);
				domainMinY /= h*System.Math.Abs (gradient.Sy);
				domainMaxY /= h*System.Math.Abs (gradient.Sy);
				Rectangle bbox = new Rectangle (0.0, 0.0, w, h);
				Export.SurfaceInflate (cs, ref bbox);
				double fx = bbox.Width /w;
				double fy = bbox.Height/h;
				double f = System.Math.Max (fx, fy);
				domainMinX *= f;
				domainMaxX *= f;
				domainMinY *= f;
				domainMaxY *= f;

				port.PutCommand ("/Domain [");
				port.PutValue (domainMinX, 3);
				port.PutValue (domainMaxX, 3);
				port.PutValue (domainMinY, 3);
				port.PutValue (domainMaxY, 3);
				port.PutCommand ("]");
				string domain = port.GetPDF ();
				port.Reset ();

				writer.WriteObjectDef (Export.GetComplexSurfaceName (cs.Id, tcs));

				//	Function-based shading, voir [*] page 178
				port.PutCommand ("<< /ShadingType 1 ");
				if (nbColors <= 1)
					port.PutCommand ("/ColorSpace /DeviceGray ");
				if (nbColors == 3)
					port.PutCommand ("/ColorSpace /DeviceRGB ");
				if (nbColors == 4)
					port.PutCommand ("/ColorSpace /DeviceCMYK ");
				port.PutCommand (domain);
				port.PutCommand (" /Function [");
				writer.WriteString (port.GetPDF ());
				if (nbColors == 0)  // alpha ?
				{
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, PdfFunctionType.Alpha));
				}
				if (nbColors == 1)  // gray ?
				{
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, PdfFunctionType.Color1));
				}
				if (nbColors == 3)  // rgb ?
				{
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, PdfFunctionType.Color1));
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, PdfFunctionType.Color2));
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, PdfFunctionType.Color3));
				}
				if (nbColors == 4)  // cmyk ?
				{
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, PdfFunctionType.Color1));
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, PdfFunctionType.Color2));
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, PdfFunctionType.Color3));
					writer.WriteObjectRef (Export.GetFunctionName (cs.Id, PdfFunctionType.Color4));
				}
				writer.WriteLine ("] >> endobj");

				//	Génère les fonctions pour les couleurs.
				string[] fonctions = new string[totalColors];
				RichColor c1 = port.GetFinalColor (gradient.Color1);
				RichColor c2 = port.GetFinalColor (gradient.Color2);
				if (gradient.FillType == Properties.GradientFillType.Diamond)
				{
					if (nbColors == 0)  // alpha ?
					{
						fonctions[0] = Export.FunctionGradientDiamond (gradient, c1.A, c2.A);
					}
					if (nbColors == 1)  // gray ?
					{
						fonctions[0] = Export.FunctionGradientDiamond (gradient, c1.Gray, c2.Gray);
					}
					if (nbColors == 3)  // rgb ?
					{
						fonctions[0] = Export.FunctionGradientDiamond (gradient, c1.R, c2.R);
						fonctions[1] = Export.FunctionGradientDiamond (gradient, c1.G, c2.G);
						fonctions[2] = Export.FunctionGradientDiamond (gradient, c1.B, c2.B);
					}
					if (nbColors == 4)  // cmyk ?
					{
						fonctions[0] = Export.FunctionGradientDiamond (gradient, c1.C, c2.C);
						fonctions[1] = Export.FunctionGradientDiamond (gradient, c1.M, c2.M);
						fonctions[2] = Export.FunctionGradientDiamond (gradient, c1.Y, c2.Y);
						fonctions[3] = Export.FunctionGradientDiamond (gradient, c1.K, c2.K);
					}
				}
				if (gradient.FillType == Properties.GradientFillType.Conic)
				{
					if (nbColors == 0)  // alpha ?
					{
						fonctions[0] = Export.FunctionGradientConic (gradient, c1.A, c2.A);
					}
					if (nbColors == 1)  // gray ?
					{
						fonctions[0] = Export.FunctionGradientConic (gradient, c1.Gray, c2.Gray);
					}
					if (nbColors == 3)  // rgb ?
					{
						fonctions[0] = Export.FunctionGradientConic (gradient, c1.R, c2.R);
						fonctions[1] = Export.FunctionGradientConic (gradient, c1.G, c2.G);
						fonctions[2] = Export.FunctionGradientConic (gradient, c1.B, c2.B);
					}
					if (nbColors == 4)  // cmyk ?
					{
						fonctions[0] = Export.FunctionGradientConic (gradient, c1.C, c2.C);
						fonctions[1] = Export.FunctionGradientConic (gradient, c1.M, c2.M);
						fonctions[2] = Export.FunctionGradientConic (gradient, c1.Y, c2.Y);
						fonctions[3] = Export.FunctionGradientConic (gradient, c1.K, c2.K);
					}
				}

				for (int i=0; i<totalColors; i++)
				{
					PdfFunctionType ft = PdfFunctionType.Alpha;
					if (nbColors != 0)  // gray, rgb ou cmyk ?
					{
						switch (i)
						{
							case 0:
								ft = PdfFunctionType.Color1;
								break;
							case 1:
								ft = PdfFunctionType.Color2;
								break;
							case 2:
								ft = PdfFunctionType.Color3;
								break;
							case 3:
								ft = PdfFunctionType.Color4;
								break;
						}
					}
					writer.WriteObjectDef (Export.GetFunctionName (cs.Id, ft));
					//	PostScript calculator function, voir [*] page 148
					writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "<< /FunctionType 4 /Range [0 1] {0} {1} >>", domain, Port.StringLength (fonctions[i].Length)));
					writer.WriteLine ("stream");
					writer.WriteLine (fonctions[i]);
					writer.WriteLine ("endstream endobj");
				}
			}
		}

		private static void FunctionColorSampled(Writer writer,
												   Port port,
												   DrawingContext drawingContext,
												   ComplexSurface cs,
												   int nbColors)
		{
			//	Génère la fonction de calcul des couleurs intermédiaires, sur la base
			//	d'une table de 3x256 valeurs 8 bits.
			//	Si nbColors == 0  ->  canal alpha
			//	Si nbColors == 1  ->  espace Gray
			//	Si nbColors == 3  ->  espace RGB
			//	Si nbColors == 4  ->  espace CMYK
			//	Sampled function, voir [*] page 142
			Properties.Gradient gradient = cs.Fill as Properties.Gradient;
			RichColor c1 = port.GetFinalColor (gradient.Color1);
			RichColor c2 = port.GetFinalColor (gradient.Color2);

			int totalColors = System.Math.Max (nbColors, 1);

			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			for (int i=0; i<256; i++)
			{
				double factor = gradient.GetProgressColorFactor (i/255.0);

				if (nbColors == 0)  // alpha ?
				{
					int a = (int) ((c1.A + (c2.A-c1.A)*factor) * 255.0);
					builder.Append (a.ToString ("X2"));
				}

				if (nbColors == 1)  // gray ?
				{
					int a = (int) ((c1.Gray + (c2.Gray-c1.Gray)*factor) * 255.0);
					builder.Append (a.ToString ("X2"));
				}

				if (nbColors == 3)  // rgb ?
				{
					int r = (int) ((c1.R + (c2.R-c1.R)*factor) * 255.0);
					int g = (int) ((c1.G + (c2.G-c1.G)*factor) * 255.0);
					int b = (int) ((c1.B + (c2.B-c1.B)*factor) * 255.0);
					builder.Append (r.ToString ("X2"));
					builder.Append (g.ToString ("X2"));
					builder.Append (b.ToString ("X2"));
				}

				if (nbColors == 4)  // cmyk ?
				{
					int c = (int) ((c1.C + (c2.C-c1.C)*factor) * 255.0);
					int m = (int) ((c1.M + (c2.M-c1.M)*factor) * 255.0);
					int y = (int) ((c1.Y + (c2.Y-c1.Y)*factor) * 255.0);
					int k = (int) ((c1.K + (c2.K-c1.K)*factor) * 255.0);
					builder.Append (c.ToString ("X2"));
					builder.Append (m.ToString ("X2"));
					builder.Append (y.ToString ("X2"));
					builder.Append (k.ToString ("X2"));
				}
			}
			string table = builder.ToString ();

			builder = new System.Text.StringBuilder ();
			for (int i=0; i<totalColors; i++)
			{
				builder.Append ("0 1 ");
			}
			string range = builder.ToString ();

			writer.WriteObjectDef (Export.GetFunctionName (cs.Id, (nbColors == 0) ? PdfFunctionType.SampledAlpha : PdfFunctionType.SampledColor));
			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "<< /FunctionType 0 /Domain [0 1] /Range [{0}] /Size [256] /BitsPerSample 8 /Filter /ASCIIHexDecode {1} >>", range, Port.StringLength (table.Length)));
			writer.WriteLine ("stream");
			writer.WriteLine (table);
			writer.WriteLine ("endstream endobj");
		}

		private static string FunctionGradientDiamond(Properties.Gradient gradient,
														double colorStart,
														double colorEnd)
		{
			//	Retourne la fonction permettant de calculer une couleur fondamentale
			//	d'un dégradé en diamant. En entrés, la pile contient x et y [-1..1].
			//	En sortie, elle contient la couleur [0..1]. Avec une répétition de 1,
			//	la fonction est la suivante:
			//	f(c) = max(|x|,|y|)
			//	PostScript calculator function, voir [*] page 148
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("{ ");

			if (colorStart == colorEnd)
			{
				builder.Append ("pop pop ");
				builder.Append (Port.StringValue (colorStart, 3));
			}
			else
			{
				builder.Append ("abs exch abs 2 copy lt { exch } if pop ");
				if (gradient.Repeat > 1)
				{
					builder.Append ("dup 1 gt { pop 1 } if ");
					builder.Append (Port.StringValue (gradient.Repeat, 3));
					builder.Append (" mul dup dup truncate sub exch cvi 2 mod 1 eq { 1 exch sub } if ");
				}

				Export.FunctionMiddleAdjust (builder, gradient);
				Export.FunctionColorAdjust (builder, colorStart, colorEnd);
			}

			builder.Append ("}");
			return builder.ToString ();
		}

		private static string FunctionGradientConic(Properties.Gradient gradient,
													  double colorStart,
													  double colorEnd)
		{
			//	Retourne la fonction permettant de calculer une couleur fondamentale
			//	d'un dégradé conique. En entrés, la pile contient x et y [-1..1].
			//	En sortie, elle contient la couleur [0..1]. Avec une répétition de 1,
			//	la fonction est la suivante:
			//	f(c) = (360-atan(x,y))/360
			//	PostScript calculator function, voir [*] page 148
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("{ ");

			if (colorStart == colorEnd)
			{
				builder.Append ("pop pop ");
				builder.Append (Port.StringValue (colorStart, 3));
			}
			else
			{
				if (gradient.Sx < 0.0)  // miroir X ?
				{
					builder.Append ("exch neg exch ");  // [-1..1] -> [1..-1]
				}

				if (gradient.Sy < 0.0)  // miroir Y ?
				{
					builder.Append ("neg ");  // [-1..1] -> [1..-1]
				}

				builder.Append ("atan 360 exch sub ");

				if (gradient.Repeat == 1)
				{
					builder.Append ("360 div ");
				}
				else
				{
					builder.Append (Port.StringValue (360.0/gradient.Repeat, 3));
					builder.Append (" div dup exch dup truncate sub exch cvi 2 mod 1 eq { 1 exch sub } if ");
				}

				Export.FunctionMiddleAdjust (builder, gradient);
				Export.FunctionColorAdjust (builder, colorStart, colorEnd);
			}

			builder.Append ("}");
			return builder.ToString ();
		}

		private static void FunctionMiddleAdjust(System.Text.StringBuilder builder, Properties.Gradient gradient)
		{
			//	Ajoute une modification du nombre [0..1] sur la pile pour tenir compte
			//	du point milieu (voir Properties.Gradient.GetProgressColorFactor).
			//	Si M>0:  P=1-(1-P)^(1+M)
			//	Si M<0:  P=P^(1-M)
			if (gradient.Middle > 0.0)
			{
				builder.Append ("1 exch sub ");
				builder.Append (Port.StringValue (1.0+gradient.Middle, 3));
				builder.Append (" exp 1 exch sub ");
			}

			if (gradient.Middle < 0.0)
			{
				builder.Append (Port.StringValue (1.0-gradient.Middle, 3));
				builder.Append (" exp ");
			}
		}

		private static void FunctionColorAdjust(System.Text.StringBuilder builder, double colorStart, double colorEnd)
		{
			//	Ajoute une modification du nombre [0..1] sur la pile pour tenir compte
			//	de la couleur.
			if (colorEnd-colorStart != 1.0)
			{
				builder.Append (Port.StringValue (colorEnd-colorStart, 3));
				builder.Append (" mul ");
			}
			if (colorStart != 0.0)
			{
				builder.Append (Port.StringValue (colorStart, 3));
				builder.Append (" add ");
			}
		}

		private void CreateComplexSurfaceOpaquePattern(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface opaque contenant un motif.
			Properties.Gradient surface = cs.Fill as Properties.Gradient;
			Objects.Abstract obj = cs.Object;
			SurfaceAnchor sa = obj.SurfaceAnchor;
			Rectangle bbox = cs.Object.BoundingBox;  // update bounding box !

			port.Reset ();

			Path path = new Path ();
			path.AppendRectangle (bbox);

			Drawer.RenderMotif (port, drawingContext, obj, path, sa, surface);

			writer.WriteObjectDef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.XObject));
			writer.WriteString ("<< /Subtype /Form ");  // voir [*] page 328
			writer.WriteString (Port.StringBBox (bbox));

			string pdf = port.GetPDF ();
			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "{0} >>", Port.StringLength (pdf.Length)));
			writer.WriteLine ("stream");
			writer.WriteString (pdf);
			writer.WriteLine ("endstream endobj");
		}

		private int ComplexSurfaceGradientNbColors(Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Détermine le nombre de couleurs (1, 3 ou 4) à utiliser pour une surface
			//	complexe. C'est la première couleur qui fait foi. La deuxième sera
			//	convertie dans l'espace de couleur de la première, au besoin.
			Properties.Gradient gradient = cs.Fill as Properties.Gradient;
			RichColor color = port.GetFinalColor (gradient.Color1);

			if (color.ColorSpace == ColorSpace.Gray)
			{
				color = port.GetFinalColor (gradient.Color2);
			}

			if (color.ColorSpace == ColorSpace.Cmyk)
				return 4;
			if (color.ColorSpace == ColorSpace.Gray)
				return 1;
			return 3;
		}

		private void ClearComplexSurfaces()
		{
			//	Libère toutes les surfaces complexes.
			if (this.complexSurfaces != null)
			{
				foreach (ComplexSurface cs in this.complexSurfaces)
				{
					cs.Dispose ();
				}

				this.complexSurfaces.Clear ();
				this.complexSurfaces = null;
			}
		}
		#endregion


		#region Images
		private void EmitImageSurfaces(Writer writer, Port port)
		{
			//	Crée toutes les images.
			Export.debugTotal = 0;

			foreach (ImageSurface image in this.imageSurfaces)
			{
				this.report.DefineProgress (0, string.Format (Res.Strings.Export.PDF.Progress.Image, image.Id));

				if (this.report.Canceled)
				{
					throw new PdfExportException (Res.Strings.Export.PDF.Progress.Canceled);
				}

				if (this.CreateImageSurface (writer, image, PdfComplexSurfaceType.XObject, PdfComplexSurfaceType.XObjectMask))
				{
					this.CreateImageSurface (writer, image, PdfComplexSurfaceType.XObjectMask, PdfComplexSurfaceType.None);
				}

				writer.Flush ();  // écrit déjà tout ce qu'on peut sur disque, afin d'utiliser le moins possible de mémoire
			}
		}

		class PdfImageStream
		{
			public PdfImageStream(string code, string stream)
			{
				this.code = code;
				this.stream = stream;
			}

			public string Code
			{
				get
				{
					return this.code;
				}
			}

			public string Stream
			{
				get
				{
					return this.stream;
				}
			}


			private readonly string code;
			private readonly string stream;
		}

		private bool CreateImageSurface(Writer writer, ImageSurface image,
										PdfComplexSurfaceType baseType, PdfComplexSurfaceType maskType)
		{
			ImageCompression compression = this.GetCompressionMode (baseType);

			//	Crée une image.

			image.MinDpi = this.imageMinDpi;
			image.MaxDpi = this.imageMaxDpi;

			PdfImageStream pdf = this.ProcessImageAndCreatePdfStream (image, baseType, compression);
			
			Export.EmitImageHeader (writer, baseType, image, compression, this.colorConversion);

			writer.WriteString (pdf.Code);

			if (maskType == PdfComplexSurfaceType.XObjectMask && image.IsTransparent)
			{
				writer.WriteString ("/SMask ");
				writer.WriteObjectRef (Export.GetComplexSurfaceName (image.Id, maskType));
			}

			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, " {0} >>", Port.StringLength (pdf.Stream.Length)));
			writer.WriteLine ("stream");
			writer.WriteHugeString (pdf.Stream);
			writer.WriteLine ("endstream endobj");

			return image.IsTransparent;
		}

		private static NativeBitmap PrepareImage(ImageSurface image)
		{
			double imageMinDpi = image.MinDpi;
			double imageMaxDpi = image.MaxDpi;

			NativeBitmap fi;

			int dx;
			int dy;

			fi = Export.LoadImage (image);
			fi = Export.CropImage (image, fi);
			fi = Export.ResizeImage (image, fi, imageMinDpi, imageMaxDpi, out dx, out dy);

			image.ColorType = fi.ColorType;
			image.IsTransparent = fi.IsTransparent;

			image.DX = dx;
			image.DY = dy;

			return fi;
		}

		private PdfImageStream ProcessImageAndCreatePdfStream(ImageSurface image, PdfComplexSurfaceType baseType, ImageCompression compression)
		{
			ColorConversion colorConversion = this.colorConversion;
			double jpegQuality = this.jpegQuality;

			using (NativeBitmap fi = Export.PrepareImage (image))
			{
				if (compression == ImageCompression.JPEG)  // compression JPEG ?
				{
					return Export.EmitJpegImageSurface (baseType, fi, colorConversion, jpegQuality);
				}
				else	// compression ZIP ou aucune ?
				{
					return Export.EmitLosslessImageSurface (baseType, image, compression, fi, colorConversion);
				}
			}
		}

		private static void EmitImageHeader(Writer writer, PdfComplexSurfaceType baseType, ImageSurface image, ImageCompression compression, ColorConversion colorConversion)
		{
			int dx = image.DX;
			int dy = image.DY;

			//	Génération de l'en-tête.
			writer.WriteObjectDef (Export.GetComplexSurfaceName (image.Id, baseType));
			writer.WriteString ("<< /Subtype /Image ");

			if (baseType == PdfComplexSurfaceType.XObject)
			{
				if (colorConversion == PDF.ColorConversion.ToGray)
				{
					writer.WriteString ("/ColorSpace /DeviceGray ");
				}
				else if (colorConversion == PDF.ColorConversion.ToCmyk)
				{
					writer.WriteString ("/ColorSpace /DeviceCMYK ");
				}
				else
				{
					switch (image.ColorType)
					{
						case BitmapColorType.MinIsBlack:
						case BitmapColorType.MinIsWhite:
							writer.WriteString ("/ColorSpace /DeviceGray ");
							break;

						case BitmapColorType.Rgb:
						case BitmapColorType.RgbAlpha:
						case BitmapColorType.Palette:
							writer.WriteString ("/ColorSpace /DeviceRGB ");
							break;

						case BitmapColorType.Cmyk:
							if (compression == ImageCompression.JPEG)
							{
								writer.WriteString ("/ColorSpace /DeviceRGB ");
							}
							else
							{
								writer.WriteString ("/ColorSpace /DeviceCMYK ");
							}
							break;

						default:
							throw new System.InvalidOperationException ();
					}
				}
			}
			if (baseType == PdfComplexSurfaceType.XObjectMask)
			{
				writer.WriteString ("/ColorSpace /DeviceGray ");
			}

			writer.WriteString ("/BitsPerComponent 8 /Width ");  // voir [*] page 310
			writer.WriteString (Port.StringValue (dx, 0));
			writer.WriteString (" /Height ");
			writer.WriteString (Port.StringValue (dy, 0));
			writer.WriteString (" ");

			if (image.Filter.Active)
			{
				writer.WriteString ("/Interpolate true ");
			}
		}

		private static PdfImageStream EmitLosslessImageSurface(PdfComplexSurfaceType baseType, ImageSurface image, ImageCompression compression, NativeBitmap fi, ColorConversion colorConversion)
		{
			int dx = image.DX;
			int dy = image.DY;

			int bpp = 3;
			if (baseType == PdfComplexSurfaceType.XObject)
			{
				if (colorConversion == PDF.ColorConversion.ToGray)
				{
					bpp = 1;
				}
				else if (colorConversion == PDF.ColorConversion.ToRgb)
				{
					bpp = 3;
				}
				else if (colorConversion == PDF.ColorConversion.ToCmyk)
				{
					bpp = 4;
				}
				else
				{
					switch (image.ColorType)
					{
						case BitmapColorType.MinIsBlack:
						case BitmapColorType.MinIsWhite:
							bpp = 1;
							break;

						case BitmapColorType.Rgb:
						case BitmapColorType.RgbAlpha:
						case BitmapColorType.Palette:
							bpp = 3;
							break;

						case BitmapColorType.Cmyk:
							bpp = 4;
							break;

						default:
							throw new System.InvalidOperationException ();
					}
				}
			}
			else
			{
				bpp = -1;
			}

			byte[] data = null;

			if (bpp == -1)  // alpha ?
			{
				data = Export.CreateImageSurfaceChannel (fi, BitmapColorChannel.Alpha, image.Filter);
			}

			if (bpp == 1)
			{
				data = Export.CreateImageSurfaceChannel (fi, BitmapColorChannel.Grayscale, image.Filter);
			}

			if (bpp == 3)
			{
				byte[] bufferRed   = Export.CreateImageSurfaceChannel (fi, BitmapColorChannel.Red, image.Filter);
				byte[] bufferGreen = Export.CreateImageSurfaceChannel (fi, BitmapColorChannel.Green, image.Filter);
				byte[] bufferBlue  = Export.CreateImageSurfaceChannel (fi, BitmapColorChannel.Blue, image.Filter);

				data = new byte[dx*dy*3];
				for (int i=0; i<dx*dy; i++)
				{
					data[i*3+0] = bufferRed[i];
					data[i*3+1] = bufferGreen[i];
					data[i*3+2] = bufferBlue[i];
				}
			}

			if (bpp == 4)
			{
				byte[] bufferCyan    = Export.CreateImageSurfaceChannel (fi, BitmapColorChannel.Cyan, image.Filter);
				byte[] bufferMagenta = Export.CreateImageSurfaceChannel (fi, BitmapColorChannel.Magenta, image.Filter);
				byte[] bufferYellow  = Export.CreateImageSurfaceChannel (fi, BitmapColorChannel.Yellow, image.Filter);
				byte[] bufferBlack   = Export.CreateImageSurfaceChannel (fi, BitmapColorChannel.Black, image.Filter);

				data = new byte[dx*dy*4];
				for (int i=0; i<dx*dy; i++)
				{
					data[i*4+0] = bufferCyan[i];
					data[i*4+1] = bufferMagenta[i];
					data[i*4+2] = bufferYellow[i];
					data[i*4+3] = bufferBlack[i];
				}
			}

			if (compression == ImageCompression.ZIP)  // compression ZIP ?
			{
				byte[] zip = Common.IO.DeflateCompressor.Compress (data, 9);  // 9 = compression forte mais lente
				data = zip;
				zip = null;
			}

			Port port = new Port ();
			port.Reset ();
			port.PutASCII85 (data);
			Export.debugTotal += data.Length;
			port.PutEOL ();

			data = null;

			if (compression == ImageCompression.ZIP)  // compression ZIP ?
			{
				return new PdfImageStream ("/Filter [/ASCII85Decode /FlateDecode] ", port.GetPDF ());  // voir [*] page 43
			}
			else
			{
				return new PdfImageStream ("/Filter /ASCII85Decode ", port.GetPDF ());  // voir [*] page 43
			}
		}

		private static PdfImageStream EmitJpegImageSurface(PdfComplexSurfaceType baseType, NativeBitmap fi, ColorConversion colorConversion, double jpegQuality)
		{
			bool isGray = false;
			if (colorConversion == PDF.ColorConversion.ToGray)
				isGray = true;
			if (baseType == PdfComplexSurfaceType.XObjectMask)
				isGray = true;
			if (fi.ColorType == BitmapColorType.MinIsBlack)
				isGray = true;
			if (fi.ColorType == BitmapColorType.MinIsWhite)
				isGray = true;

			byte[] jpeg;

			if (baseType == PdfComplexSurfaceType.XObjectMask)
			{
				var mask = fi.GetChannel (BitmapColorChannel.Alpha);
				jpeg = mask.SaveToMemory (Properties.Image.FilterQualityToMode (jpegQuality));
				mask.Dispose ();
			}
			else if (isGray)
			{
				var gray = fi.ConvertToGrayscale ();
				jpeg = gray.SaveToMemory (Properties.Image.FilterQualityToMode (jpegQuality));
				gray.Dispose ();
			}
			else
			{
				if (fi.ColorType == BitmapColorType.Rgb)
				{
					jpeg = fi.SaveToMemory (Properties.Image.FilterQualityToMode (jpegQuality));
				}
				else
				{
					var rgb = fi.ConvertToArgb32 ();
					var rgb24 = rgb.ConvertToRgb24 ();
					jpeg = rgb24.SaveToMemory (Properties.Image.FilterQualityToMode (jpegQuality));
					rgb24.Dispose ();
					rgb.Dispose ();
				}
			}

			System.Diagnostics.Debug.Assert (jpeg != null);

			Port port = new Port ();
			port.PutASCII85 (jpeg);
			Export.debugTotal += jpeg.Length;
			port.PutEOL ();

			jpeg = null;

			return new PdfImageStream ("/Filter [/ASCII85Decode /DCTDecode] ", port.GetPDF ());  // voir [*] page 43
		}

		private static NativeBitmap LoadImage(ImageSurface image)
		{
			return NativeBitmap.Load (image.FilePath);
		}

		private static NativeBitmap CropImage(ImageSurface image, NativeBitmap fi)
		{
			Margins crop = image.Crop;
			if (crop != Margins.Zero)  // recadrage nécessaire ?
			{
				var cropped = fi.Crop ((int) crop.Left, (int) crop.Top, fi.Width-(int) (crop.Left+crop.Right), fi.Height-(int) (crop.Top+crop.Bottom));
				fi.Dispose ();
				fi = cropped;
			}

			return fi;
		}

		private static NativeBitmap ResizeImage(ImageSurface image, NativeBitmap fi, double imageMinDpi, double imageMaxDpi, out int dx, out int dy)
		{
			dx = fi.Width;
			dy = fi.Height;

			//	Mise à l'échelle éventuelle de l'image selon les choix de l'utilisateur.
			//	Une image sans filtrage n'est jamais mise à l'échelle !
			double currentDpiX = dx*254.0/image.ImageSize.Width;
			double currentDpiY = dy*254.0/image.ImageSize.Height;

			double finalDpiX = currentDpiX;
			double finalDpiY = currentDpiY;

			if (imageMinDpi != 0.0)
			{
				finalDpiX = System.Math.Max (finalDpiX, imageMinDpi);
				finalDpiY = System.Math.Max (finalDpiY, imageMinDpi);
			}

			if (imageMaxDpi != 0.0)
			{
				finalDpiX = System.Math.Min (finalDpiX, imageMaxDpi);
				finalDpiY = System.Math.Min (finalDpiY, imageMaxDpi);
			}

			bool resizeRequired = false;

			if (currentDpiX != finalDpiX || currentDpiY != finalDpiY)
			{
				dx = (int) System.Math.Ceiling ((dx+0.5)*finalDpiX/currentDpiX);
				dy = (int) System.Math.Ceiling ((dy+0.5)*finalDpiY/currentDpiY);

				if (dx < 1)
				{
					dx = 1;
				}
				if (dy < 1)
				{
					dy = 1;
				}

				resizeRequired = true;
			}

			if (resizeRequired)
			{
				//	TODO: take into account the value of 'image.Filter'

				var resized = fi.Rescale (dx, dy);
				fi.Dispose ();
				fi = resized;
			}

			return fi;
		}

		private ImageCompression GetCompressionMode(PdfComplexSurfaceType baseType)
		{
			//	Ajuste le mode de compression possible.
			ImageCompression compression = this.imageCompression;

			if ((baseType == PdfComplexSurfaceType.XObject) &&
				(this.colorConversion == PDF.ColorConversion.ToCmyk) &&
				(compression == ImageCompression.JPEG)) // cmyk impossible en jpg !
			{
				return ImageCompression.ZIP;  // utilise la compression sans pertes
			}
			else
			{
				return compression;
			}
		}

		private static byte[] CreateImageSurfaceChannel(NativeBitmap fi, BitmapColorChannel channel, ImageFilter filter)
		{
			var plan = fi.GetChannel (channel);
			bool invert = false;

			if ((plan == null) &&
				(channel == BitmapColorChannel.Alpha))
			{
				plan = fi.GetChannel (BitmapColorChannel.Red);
				invert = true;
			}

			byte[] data = plan.GetRawImageDataInCompactFormFor8BitImage ();

			if (invert)
			{
				for (int i = 0; i < data.Length; i++)
				{
					data[i] = (byte) (0xff ^ data[i]);
				}
			}

			return data;
		}

		private void ClearImageSurfaces()
		{
			if (this.imageSurfaces != null)
			{
				//	Libère toutes les images.
				foreach (ImageSurface image in this.imageSurfaces)
				{
					image.Dispose ();
				}

				this.imageSurfaces.Clear ();
				this.imageSurfaces = null;
			}
		}
		#endregion


		#region Fonts
		private void EmitFonts(Writer writer)
		{
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;

			if (info.TextCurve)
			{
				return;
			}
			//	Crée toutes les fontes.
			foreach (System.Collections.DictionaryEntry dict in this.fontHash)
			{
				FontList font = dict.Key as FontList;

				int totalPages = (font.CharacterCount+Export.charPerFont-1)/Export.charPerFont;
				System.Diagnostics.Debug.Assert (totalPages <= 10);  // voir Export.NameFont
				for (int fontPage=0; fontPage<totalPages; fontPage++)
				{
					int count = Export.charPerFont;
					if (fontPage == totalPages-1)
					{
						count = font.CharacterCount%Export.charPerFont;
						if (count == 0)
							count = Export.charPerFont;
					}

					this.CreateFontBase (writer, font, fontPage, count);
					this.CreateFontEncoding (writer, font, fontPage, count);
					this.CreateFontCharProcs (writer, font, fontPage, count);
					this.CreateFontWidths (writer, font, fontPage, count);
					this.CreateFontToUnicode (writer, font, fontPage, count);
					this.CreateFontCharacters (writer, font, fontPage, count);
				}
			}
		}

		private void CreateFontBase(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet de base d'une fonte.
			Rectangle bbox = font.CharacterBBox;

			writer.WriteObjectDef (Export.GetFontName (font.Id, fontPage, PdfFontType.Base));
			writer.WriteString ("<< /Type /Font /Subtype /Type3 ");  // voir [*] page 394
			writer.WriteString (string.Format (CultureInfo.InvariantCulture, "/FirstChar 0 /LastChar {0} ", count-1));
			writer.WriteString (Port.StringBBox ("/FontBBox", bbox));
			writer.WriteString ("/FontMatrix [1 0 0 1 0 0] ");

			writer.WriteString ("/Encoding ");
			writer.WriteObjectRef (Export.GetFontName (font.Id, fontPage, PdfFontType.Encoding));

			writer.WriteString ("/CharProcs ");
			writer.WriteObjectRef (Export.GetFontName (font.Id, fontPage, PdfFontType.CharProcs));

			writer.WriteString ("/Widths ");
			writer.WriteObjectRef (Export.GetFontName (font.Id, fontPage, PdfFontType.Widths));

			writer.WriteString ("/ToUnicode ");
			writer.WriteObjectRef (Export.GetFontName (font.Id, fontPage, PdfFontType.ToUnicode));

			writer.WriteLine (">> endobj");
		}

		private void CreateFontEncoding(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet Encoding d'une fonte.
			writer.WriteObjectDef (Export.GetFontName (font.Id, fontPage, PdfFontType.Encoding));

			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("<< /Differences [ 0 ");

			int firstChar = fontPage*Export.charPerFont;
			for (int i=0; i<count; i++)
			{
				CharacterList cl = font.GetCharacter (firstChar+i);
				builder.Append (Export.GetCharacterShortName (font.Id, fontPage, cl));
			}

			builder.Append ("] >> endobj");
			writer.WriteLine (builder.ToString ());
		}

		private void CreateFontCharProcs(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet CharProcs d'une fonte.
			writer.WriteObjectDef (Export.GetFontName (font.Id, fontPage, PdfFontType.CharProcs));
			writer.WriteString ("<< ");

			int firstChar = fontPage*Export.charPerFont;
			for (int i=0; i<count; i++)
			{
				CharacterList cl = font.GetCharacter (firstChar+i);
				writer.WriteString (Export.GetCharacterShortName (font.Id, fontPage, cl));
				writer.WriteObjectRef (Export.GetCharacterName (font.Id, fontPage, cl));
			}

			writer.WriteLine (">> endobj");
		}

		private void CreateFontWidths(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet "table des chasses" d'une fonte.
			writer.WriteObjectDef (Export.GetFontName (font.Id, fontPage, PdfFontType.Widths));

			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("[");

			int firstChar = fontPage*Export.charPerFont;
			for (int i=0; i<count; i++)
			{
				CharacterList cl = font.GetCharacter (firstChar+i);
				double advance = cl.Width;
				builder.Append (Port.StringValue (advance, 4));
				builder.Append (" ");
			}
			builder.Append ("] endobj");
			writer.WriteLine (builder.ToString ());
		}

		private void CreateFontToUnicode(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet ToUnicode d'une fonte, qui permet de retrouver les codes
			//	des caractères lors d'une copie depuis Acrobat dans le clipboard.
			//	Voir [*] pages 420 à 446.
			writer.WriteObjectDef (Export.GetFontName (font.Id, fontPage, PdfFontType.ToUnicode));

			string fontName = font.DrawingFont.FaceName;
			System.Text.StringBuilder builder = new System.Text.StringBuilder ();
			builder.Append ("/CIDInit /ProcSet findresource begin ");
			builder.Append ("12 dict begin begincmap ");
			builder.Append ("/CIDSystemInfo << /Registry (Epsitec) /Ordering (Identity-H) /Supplement 0 >> def ");
			builder.Append ("/CMapName /Epsitec def ");
			builder.Append ("1 begincodespacerange <00> <FF> endcodespacerange ");

			int firstChar = fontPage*Export.charPerFont;
			builder.Append (string.Format (CultureInfo.InvariantCulture, "{0} beginbfrange ", count));
			for (int i=0; i<count; i++)
			{
				CharacterList cl = font.GetCharacter (firstChar+i);
				string hex1 = i.ToString ("X2");
				string hex2 = "";
				if (cl.Unicodes == null)
				{
					hex2 = (cl.Unicode).ToString ("X4");
				}
				else
				{
					for (int u=0; u<cl.Unicodes.Length; u++)
					{
						hex2 += (cl.Unicodes[u]).ToString ("X4");
					}
				}
				builder.Append (string.Format (CultureInfo.InvariantCulture, "<{0}> <{0}> <{1}> ", hex1, hex2));
			}
			builder.Append ("endbfrange ");

			builder.Append ("endcmap CMapName currentdict /CMap defineresource pop end end\r\n");

			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "<< {0} >>", Port.StringLength (builder.Length)));
			writer.WriteLine ("stream");
			writer.WriteString (builder.ToString ());
			writer.WriteLine ("endstream endobj");
		}

		private void CreateFontCharacters(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée tous les objets des caractères d'une fonte.
			int firstChar = fontPage*Export.charPerFont;
			for (int i=0; i<count; i++)
			{
				CharacterList cl = font.GetCharacter (firstChar+i);
				this.CreateFontCharacter (writer, font, fontPage, cl);
			}
		}

		private void CreateFontCharacter(Writer writer, FontList font, int fontPage, CharacterList cl)
		{
			//	Crée l'objet d'un caractère d'une fonte.
			writer.WriteObjectDef (Export.GetCharacterName (font.Id, fontPage, cl));

			Font drawingFont = font.DrawingFont;
			Drawing.Transform ft = drawingFont.SyntheticTransform;
			int glyph = cl.Glyph;
			Path path = new Path ();
			path.Append (drawingFont, glyph, ft.XX, ft.XY, ft.YX, ft.YY, ft.TX, ft.TY);


			Port port = new Port ();
			port.ColorForce = ColorForce.Nothing;  // pas de commande de couleur !
			port.DefaultDecimals = 4;

			//	Sans "wx wy llx lly urx ury d1", Acrobat 8 n'arrive pas à afficher les caractères.
			//	Attention, si wx ne correspond pas à la valeur générée par CreateFontWidths, Acrobat 8 plante !
			//	Acrobat 8 n'apprécie pas du tout si "... d1" est remplacé par "wx wy d0" !
			Rectangle bounds = cl.Bounds;
			port.PutValue (cl.Width);       // wx
			port.PutValue (0);              // wy
			port.PutValue (bounds.Left);    // iix
			port.PutValue (bounds.Bottom);  // iiy
			port.PutValue (bounds.Right);   // urx
			port.PutValue (bounds.Top);     // ury
			port.PutCommand ("d1 ");        // voir [*] page 393

			port.PaintSurface (path);

			string pdf = port.GetPDF ();
			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "<< {0} >>", Port.StringLength (pdf.Length)));
			writer.WriteLine ("stream");
			writer.WriteString (pdf);
			writer.WriteLine ("endstream endobj");
		}

		private void ClearFonts()
		{
			//	Libère toutes les fontes.
			this.characterHash = null;
			this.fontHash = null;
		}
		#endregion


		private IEnumerable<Objects.Layer> GetPrintableLayers(int pageNumber)
		{
			//	Calcule la liste des calques, y compris ceux des pages maîtres.
			//	Les calques cachés à l'impression ne sont pas mis dans la liste.
			List<Objects.Layer> layers     = new List<Objects.Layer> ();
			List<Objects.Page>  masterList = this.document.Modifier.ComputeMasterPageList (pageNumber);

			//	Met d'abord les premiers calques de toutes les pages maîtres.
			foreach (Objects.Page master in masterList)
			{
				int frontier = master.MasterFirstFrontLayer;
				for (int i=0; i<frontier; i++)
				{
					Objects.Layer layer = master.Objects[i] as Objects.Layer;

					if (layer.Print != Objects.LayerPrint.Hide)
					{
						layers.Add (layer);
					}
				}
			}

			//	Met ensuite tous les calques de la page.
			Objects.Abstract page = this.document.DocumentObjects[pageNumber] as Objects.Abstract;
			foreach (Objects.Layer layer in this.document.Flat (page))
			{
				if (layer.Print != Objects.LayerPrint.Hide)
				{
					layers.Add (layer);
				}
			}

			//	Met finalement les derniers calques de toutes les pages maîtres.
			foreach (Objects.Page master in masterList)
			{
				int frontier = master.MasterFirstFrontLayer;
				int total    = master.Objects.Count;
				
				for (int i=frontier; i<total; i++)
				{
					Objects.Layer layer = master.Objects[i] as Objects.Layer;
					
					if (layer.Print != Objects.LayerPrint.Hide)
					{
						layers.Add (layer);
					}
				}
			}

			return layers;
		}

		public static int GetFontPage(int code)
		{
			return code/Export.charPerFont;
		}

		public static string GetFontIndex(int code)
		{
			return (code%Export.charPerFont).ToString ("X2");
		}


		//	Constante pour conversion dixièmes de millimètres -> 72ème de pouce
		private static readonly double			mmToInches = 0.1*72/25.4;
		private static readonly int				charPerFont = 256;
		
		private static long						debugTotal;

		private readonly Document				document;

		private Common.Dialogs.IWorkInProgressReport report;
		private List<ComplexSurface>			complexSurfaces;
		private List<ImageSurface>				imageSurfaces;
		private CharacterHash					characterHash;
		private FontHash						fontHash;
		private ColorConversion					colorConversion;
		private ImageCompression				imageCompression;
		private double							jpegQuality;
		private double							imageMinDpi;
		private double							imageMaxDpi;
		private string							documentTitle;
		private double							zoom;
	}
}
