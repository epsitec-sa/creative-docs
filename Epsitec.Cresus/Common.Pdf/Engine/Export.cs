//	Copyright � 2004-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Drawing.Platform;

using System;
using System.Collections.Generic;
using System.IO;

namespace Epsitec.Common.Pdf.Engine
{
	using CultureInfo = System.Globalization.CultureInfo;
	using Path = Epsitec.Common.Drawing.Path;

	/// <summary>
	/// La classe Export impl�mente la publication d'un document PDF.
	/// [*] = documentation PDF Reference, version 1.6, fifth edition, 1236 pages
	/// </summary>
	/// 
	public class Export
	{
		public Export(ExportPdfInfo info)
		{
			Epsitec.Common.Widgets.Widget.Initialize ();

			this.info = info;
			this.zoom = 1.0;
			this.pages = new List<int> ();
		}

		public void Dispose()
		{
		}

		public void SetZoom(double zoom)
		{
			this.zoom = zoom;
		}


		public void ExportToFile(string path, int pageCount, Action<Port, int> renderPage)
		{
			using (var stream = File.Open (path, FileMode.Create))
			{
				this.ExportToFile (stream, pageCount, renderPage);
			}
		}

		public void ExportToFile(Stream stream, int pageCount, Action<Port, int> renderPage)
		{
			//	Exporte le document dans un stream.
			//	Le renderer re�oit le port et le num�ro de la page � g�n�rer (1..n).
			System.Diagnostics.Debug.Assert (stream != null);
			System.Diagnostics.Debug.Assert (pageCount > 0);
			System.Diagnostics.Debug.Assert (renderPage != null);

			this.pageCount  = pageCount;
			this.renderPage = renderPage;

			this.pages.Clear ();
			this.pages.AddRange (this.Pages);

			if (this.pages.Count == 0)
			{
				throw new PdfExportException ("Document vide");
			}

			this.ExportPdf (stream);
		}

		private void ExportPdf(Stream stream)
		{
			this.ExportBegin ();

			using (var port = this.CreatePort ())
			{
				var writer = this.CreateWriter (stream);

				this.PreProcess (port);

				this.EmitHeaderOutlines (writer);
				this.EmitHeaderPages (writer);
				this.EmitPageObjects (writer);
				this.EmitPageObjectResources (writer, port);

				this.EmitComplexSurfaces (writer, port);
				this.EmitImageSurfaces (writer, port);
				this.EmitFonts (writer, port);

				this.EmitPageContents (writer, port);
				this.ExportEnd (writer);
			}
		}


		private void ExportBegin()
		{
			this.colorConversion  = this.info.ColorConversion;
			this.imageCompression = this.info.ImageCompression;
			this.jpegQuality      = this.info.JpegQuality;
			this.imageMinDpi      = this.info.ImageMinDpi;
			this.imageMaxDpi      = this.info.ImageMaxDpi;
		}

		private void ExportEnd(Writer writer)
		{
			writer.Flush ();
			writer.Finish ();
		}


		private Port CreatePort()
		{
			var port = new Port ()
			{
				TextToCurve = this.info.TextToCurve,
			};

			port.PushColorModifier (new ColorModifierCallback (this.FinalOutputColorModifier));

			return port;
		}
		private Writer CreateWriter(Stream stream)
		{
			Writer writer = new Writer (stream);

			//	Objet racine du document.
			writer.WriteObjectDef ("Root");
			writer.WriteString ("<< /Type /Catalog /Outlines ");
			writer.WriteObjectRef ("HeaderOutlines");
			writer.WriteString ("/Pages ");
			writer.WriteObjectRef ("HeaderPages");
			writer.WriteLine (">> endobj");

			this.documentTitle = this.info.Title;

			string defaultProducer  = Export.GetDefaultProducerInformation ();
			string defaultCopyright = Export.GetDefaultModuleCopyright ();

			//	Object info
			string titleName    = Export.GetEscapedString (this.documentTitle);
			string author       = Export.GetEscapedString (this.info.Author);
			string creator      = Export.GetEscapedString (this.info.Creator);
			string producer     = Export.GetEscapedString (this.info.Producer);
			string creationDate = Export.GetDateString (this.info.CreationDate);

			writer.WriteObjectDef ("Info");
			writer.WriteString (string.Concat ("<< /Title (", titleName, ") "));
			if (author   != null)  writer.WriteString (string.Concat ("/Author (",   author,   ") "));
			if (creator  != null)  writer.WriteString (string.Concat ("/Creator (",  creator,  ") "));
			if (producer != null)  writer.WriteString (string.Concat ("/Producer (", producer, ") "));
			writer.WriteString (string.Concat ("/CreationDate (D:", creationDate, ") "));
			writer.WriteString ("/Trapped /False ");
			writer.WriteLine (">> endobj");
			return writer;
		}


		private void PreProcess(Port port)
		{
			//	Il faut passer en revue tous les caract�res de tous les textes, pour
			//	pouvoir ensuite cr�er les polices pdf.
			//	Il faut �galement passer en revue toutes les surfaces complexes, en
			//	l'occurrence les surfaces transparentes.
			{
				port.IsPreProcess = true;

				foreach (var page in this.pages)
				{
					port.Reset ();
					port.CurrentPage = page;

					this.renderPage (port, page);  // pr�-prossessing de la page
				}

				port.IsPreProcess = false;
			}

			FontList.CreateFonts (port.FontHash, port.CharacterHash);
		}


		private void EmitHeaderOutlines(Writer writer)
		{
			//	Objet outlines.
			writer.WriteObjectDef ("HeaderOutlines");
			writer.WriteLine ("<< /Type /Outlines /Count 0 >> endobj");
		}

		private void EmitHeaderPages(Writer writer)
		{
			//	Objet donnant la liste des pages.
			writer.WriteObjectDef ("HeaderPages");
			writer.WriteString ("<< /Type /Pages /Kids [ ");
			foreach (var page in this.pages)
			{
				writer.WriteObjectRef (Export.GetPageName (page));
			}
			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "] /Count {0} >> endobj", this.pages.Count));
		}

		private void EmitPageContents(Writer writer, Port port)
		{
			PdfExportPageOffset offset = new PdfExportPageOffset (this.info);

			//	Un objet pour le contenu de chaque page.
			foreach (var page in this.pages)
			{
				this.EmitSinglePageContents (writer, port, offset.GetPageOffset (page), page);
			}
		}

		private void EmitPageObjects(Writer writer)
		{
			//	Un objet pour chaque page.
			foreach (var page in this.pages)
			{
				Rectangle trimBox = new Rectangle (Point.Zero, this.info.PageSize);

				trimBox.Scale (this.zoom);

				Rectangle bleedBox = trimBox;
				Rectangle mediaBox = trimBox;

				double markSizeX = 0.0;
				double markSizeY = 0.0;

				if (this.info.PrintCropMarks)  // traits de coupe ?
				{
					markSizeX = this.info.CropMarksLengthX + this.info.CropMarksOffsetX;
					markSizeY = this.info.CropMarksLengthY + this.info.CropMarksOffsetY;
				}

				double left, right, top, bottom;

				if (page%2 == 0)  // page paire
				{
					left   = this.info.BleedMargin + this.info.BleedEvenMargins.Left;
					right  = this.info.BleedMargin + this.info.BleedEvenMargins.Right;
					top    = this.info.BleedMargin + this.info.BleedEvenMargins.Top;
					bottom = this.info.BleedMargin + this.info.BleedEvenMargins.Bottom;
				}
				else
				{
					left   = this.info.BleedMargin + this.info.BleedOddMargins.Left;
					right  = this.info.BleedMargin + this.info.BleedOddMargins.Right;
					top    = this.info.BleedMargin + this.info.BleedOddMargins.Top;
					bottom = this.info.BleedMargin + this.info.BleedOddMargins.Bottom;
				}

				//	La bo�te de d�bord ne prend en compte que le d�bord effectivement
				//	demand� (qui peut �tre diff�rent entre pages de droite et pages
				//	de gauche).
				bleedBox.Inflate (left, right, top, bottom);

				left   = System.Math.Max (left,   markSizeX);
				right  = System.Math.Max (right,  markSizeX);
				top    = System.Math.Max (top,    markSizeY);
				bottom = System.Math.Max (bottom, markSizeY);

				//	La bo�te de m�dia doit �tre assez grande pour contenir � la fois
				//	le d�bord et les �ventuels traits de coupe.
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

				writer.WriteString (Port.StringBBox ("/MediaBox", mediaBox));
				writer.WriteString (Port.StringBBox ("/CropBox", mediaBox));
				writer.WriteString (Port.StringBBox ("/BleedBox", bleedBox));
				writer.WriteString (Port.StringBBox ("/TrimBox", trimBox));

				writer.WriteString ("/Resources ");
				writer.WriteObjectRef (Export.GetPageResourceName (page));
				writer.WriteString ("/Contents ");
				writer.WriteObjectRef (Export.GetPageContentName (page));
				writer.WriteLine (">> endobj");
			}
		}

		private void EmitPageObjectResources(Writer writer, Port port)
		{
			//	Un objet pour les ressources de chaque page.
			foreach (var page in this.pages)
			{
				writer.WriteObjectDef (Export.GetPageResourceName (page));
				writer.WriteString ("<< /ProcSet [/PDF /Text /ImageB /ImageC] ");

				int tcs = this.GetPageComplexSurfaceCount (port, page);
				if (tcs > 0 || port.ImageSurfaces.Count > 0)
				{
					writer.WriteString ("/ExtGState << ");
					writer.WriteString (Export.GetComplexSurfaceShortName (0, PdfComplexSurfaceType.ExtGState));
					writer.WriteObjectRef (Export.GetComplexSurfaceName (0, PdfComplexSurfaceType.ExtGState));
					for (int index=0; index<tcs; index++)
					{
						ComplexSurface cs = this.GetComplexSurface (port, page, index);
						System.Diagnostics.Debug.Assert (cs != null);
						if (cs.Type == ComplexSurfaceType.TransparencyRegular  ||
							cs.Type == ComplexSurfaceType.TransparencyGradient)
						{
							writer.WriteString (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ExtGState));
							writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ExtGState));
						}
						if (cs.Type == ComplexSurfaceType.TransparencyPattern)
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
						ComplexSurface cs = this.GetComplexSurface (port, page, index);
						System.Diagnostics.Debug.Assert (cs != null);
						if (cs.Type == ComplexSurfaceType.OpaqueGradient       ||
							cs.Type == ComplexSurfaceType.TransparencyGradient)
						{
							writer.WriteString (Export.GetComplexSurfaceShortName (cs.Id, PdfComplexSurfaceType.ShadingColor));
							writer.WriteObjectRef (Export.GetComplexSurfaceName (cs.Id, PdfComplexSurfaceType.ShadingColor));
						}
					}
					writer.WriteString (">> ");

					writer.WriteString ("/XObject << ");
					foreach (ImageSurface image in port.ImageSurfaces)
					{
						//?if (!image.Exists)
						//?{
						//?	continue;
						//?}

						writer.WriteString (Export.GetComplexSurfaceShortName (image.Id, PdfComplexSurfaceType.XObject));
						writer.WriteObjectRef (Export.GetComplexSurfaceName (image.Id, PdfComplexSurfaceType.XObject));
					}
					writer.WriteString (">> ");
				}

				if (!this.info.TextToCurve)
				{
					writer.WriteString ("/Font << ");
					foreach (System.Collections.DictionaryEntry dict in port.FontHash)
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

		private void EmitSinglePageContents(Writer writer, Port port, Point currentPageOffset, int page)
		{
			port.Reset ();
			port.CurrentPage = page;

			var gtBeforeZoom = this.SetupTransformForPageExport (port, currentPageOffset);
			this.renderPage (port, page);  // effectue le rendu de la page
			port.Transform = gtBeforeZoom;

			this.CropToBleedBox (port, page);  // efface ce qui d�passe de la BleedBox
			this.DrawCropMarks (port, page);  // traits de coupe

			var pdf = port.GetPDF ();
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
			gt = gt.Translate (currentPageOffset);  // translation si d�bord et/ou traits de coupe
			gt = gt.Scale (Export.mmToInches);  // unit� = 0.1mm
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
			if (this.colorConversion == ColorConversion.ToRgb)
			{
				color.ColorSpace = ColorSpace.Rgb;
			}
			else if (this.colorConversion == ColorConversion.ToCmyk)
			{
				color.ColorSpace = ColorSpace.Cmyk;
			}
			else if (this.colorConversion == ColorConversion.ToGray)
			{
				color.ColorSpace = ColorSpace.Gray;
			}

			return color;
		}


		private void CropToBleedBox(Port port, int page)
		{
			//	Dessine un masque avec une ouverture qui a exactement la taille
			//	du BleedBox (� savoir la taille de la page avec ses d�bords).
			Size pageSize = this.info.PageSize;

			double width  = pageSize.Width * this.zoom;
			double height = pageSize.Height * this.zoom;

			double left, right, top, bottom;

			if (page%2 == 0)  // page paire
			{
				left   = this.info.BleedMargin + info.BleedEvenMargins.Left;
				right  = this.info.BleedMargin + info.BleedEvenMargins.Right;
				top    = this.info.BleedMargin + info.BleedEvenMargins.Top;
				bottom = this.info.BleedMargin + info.BleedEvenMargins.Bottom;
			}
			else
			{
				left   = this.info.BleedMargin + info.BleedOddMargins.Left;
				right  = this.info.BleedMargin + info.BleedOddMargins.Right;
				top    = this.info.BleedMargin + info.BleedOddMargins.Top;
				bottom = this.info.BleedMargin + info.BleedOddMargins.Bottom;
			}

			using (Path path = new Path ())
			{
				const double clip=1000.0;

				path.MoveTo (0.0 - clip, 0.0 - clip);
				path.LineTo (width + clip, 0.0 - clip);
				path.LineTo (width + clip, height + clip);
				path.LineTo (0.0 - clip, height + clip);
				path.LineTo (0.0 - clip, 0.0 - clip);
				path.Close ();

				path.MoveTo (0.0 - left, 0.0 - bottom);
				path.LineTo (0.0 - left, height + top);
				path.LineTo (width + right, height + top);
				path.LineTo (width + right, 0.0 - bottom);
				path.LineTo (0.0 - left, 0.0 - bottom);
				path.Close ();

				port.RichColor = RichColor.FromCmyk (0.0, 0.0, 0.0, 0.0);  // masque blanc
				port.PaintSurface (path);
			}
		}

		private void DrawCropMarks(Port port, int page)
		{
			//	Dessine les traits de coupe.
			if (!this.info.PrintCropMarks)
			{
				return;
			}

			Size pageSize = this.info.PageSize;

			double width  = pageSize.Width  * this.zoom;
			double height = pageSize.Height * this.zoom;

			double offsetX = this.info.CropMarksOffsetX;
			double offsetY = this.info.CropMarksOffsetY;
			double lengthX = this.info.CropMarksLengthX;
			double lengthY = this.info.CropMarksLengthY;

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

				port.LineWidth = this.info.CropMarksWidth * 3;
				port.LineCap   = CapStyle.Butt;
				port.RichColor = RichColor.FromCmyk (0.0, 0.0, 0.0, 0.0);  // fond blanc derri�re les traits de coupe
				port.PaintOutline (path);

				port.LineWidth = this.info.CropMarksWidth;
				port.LineCap   = CapStyle.Butt;
				port.RichColor = RichColor.FromCmyk (1.0, 1.0, 1.0, 1.0);  // noir de rep�rage
				port.PaintOutline (path);
			}

			using (Path path = new Path ())
			{
				double x = lengthY;
				double y = 0.0-offsetY-lengthY/2.0;
				double size = System.Math.Min (20.0, lengthY * 0.6);
				string text = string.Format ("{0} : {1}", this.documentTitle, page);
				Font   font = Font.GetFont ("Arial", "Regular");

				double dx = font.GetTextAdvance (text) * size;

				path.AppendRoundedRectangle (x-x/2, y+size*font.Descender, dx+2*x/2, size, x/8);
				port.RichColor = RichColor.FromCmyk (0.0, 0.0, 0.0, 0.0);  // fond blanc
				port.PaintSurface (path);

				path.Clear ();
				path.Append (font, text, x, y, size);
				port.RichColor = RichColor.FromCmyk (1.0, 1.0, 1.0, 1.0);  // noir de rep�rage
				port.PaintSurface (path);
			}
		}


		#region ComplexSurface
		private int GetPageComplexSurfaceCount(Port port, int page)
		{
			//	Calcule le nombre de surfaces complexes dans une page.
			int total = 0;

			foreach (var cs in port.ComplexSurfaces)
			{
				if (cs.Page == page)
				{
					total++;
				}
			}

			return total;
		}

		private ComplexSurface GetComplexSurface(Port port, int page, int index)
		{
			//	Donne une surface complexe.
			int ip = 0;

			foreach (var cs in port.ComplexSurfaces)
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

		private void EmitComplexSurfaces(Writer writer, Port port)
		{
			//	Cr�e toutes les surfaces complexes.
			//	Cr�e le ExtGState num�ro 0, pour annuler une transparence.
			writer.WriteObjectDef (Export.GetComplexSurfaceName (0, PdfComplexSurfaceType.ExtGState));
			writer.WriteLine ("<< /CA 1 /ca 1 >> endobj");

			for (int i=0; i<port.ComplexSurfaces.Count; i++)
			{
				ComplexSurface cs = port.ComplexSurfaces[i];

				switch (cs.Type)
				{
					case ComplexSurfaceType.TransparencyRegular:
						this.CreateComplexSurfaceTransparencyRegular (writer, port, cs);
						break;
				}
			}
		}

		private void CreateComplexSurfaceTransparencyRegular(Writer writer, Port port, ComplexSurface cs)
		{
			//	Cr�e une surface transparente unie.
			double a = cs.Color.A;
			this.CreateComplexSurfaceAlpha (writer, a, cs.Id, PdfComplexSurfaceType.ExtGState);
		}

		private void CreateComplexSurfaceAlpha(Writer writer, double alpha, int id, PdfComplexSurfaceType type)
		{
			//	Cr�e un ExtGState pour une transparence unie.
			writer.WriteObjectDef (Export.GetComplexSurfaceName (id, type));
			using (Port port = new Port ())
			{
				port.PutCommand ("<< /CA ");  // voir [*] page 192
				port.PutValue (alpha, 3);
				port.PutCommand ("/ca ");
				port.PutValue (alpha, 3);
				port.PutCommand (">> endobj");
				writer.WriteString (port.GetPDF ());
			}
		}
		#endregion


		#region Images
		private void EmitImageSurfaces(Writer writer, Port port)
		{
			//	Cr�e toutes les images.
			Export.debugTotal = 0;

			foreach (ImageSurface image in port.ImageSurfaces)
			{
				bool isTransparent = image.NativeBitmap.IsTransparent;

                this.CreateImageSurface (writer, image, PdfComplexSurfaceType.XObject, PdfComplexSurfaceType.XObjectMask);

				if (isTransparent)
				{
					this.CreateImageSurface (writer, image, PdfComplexSurfaceType.XObjectMask, PdfComplexSurfaceType.None);
				}

				writer.Flush ();  // �crit d�j� tout ce qu'on peut sur disque, afin d'utiliser le moins possible de m�moire
			}
		}

		private void CreateImageSurface(Writer writer, ImageSurface image, PdfComplexSurfaceType baseType, PdfComplexSurfaceType maskType)
		{
			//	Cr�e une image.
			ImageCompression compression = this.GetCompressionMode (baseType);
			image = this.ProcessImageAndCreatePdfStream (image, baseType, compression);
			
			Export.EmitImageHeader (writer, baseType, image, compression, this.colorConversion);
			writer.WriteString (image.ImageStream.Code);

			if (maskType == PdfComplexSurfaceType.XObjectMask && image.NativeBitmap.IsTransparent)
			{
				writer.WriteString ("/SMask ");
				writer.WriteObjectRef (Export.GetComplexSurfaceName (image.Id, maskType));
			}

			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, " {0} >>", Port.StringLength (image.ImageStream.StreamLength)));
			writer.WriteLine ("stream");
			writer.WriteStream (image.ImageStream.Stream);
			writer.WriteLine ("endstream endobj");

			image.Dispose ();
		}

		private ImageSurface ProcessImageAndCreatePdfStream(ImageSurface image, PdfComplexSurfaceType baseType, ImageCompression compression)
		{
			image.ColorConversion  = this.colorConversion;
			image.JpegQuality      = this.jpegQuality;
			image.SurfaceType      = baseType;
			image.ImageCompression = compression;

			Export.ProcessImageAndCreatePdfStream (image);
			return image;
		}

		private static void ProcessImageAndCreatePdfStream(ImageSurface image)
		{
			PdfComplexSurfaceType baseType        = image.SurfaceType;
			ImageCompression      compression     = image.ImageCompression;
			ColorConversion       colorConversion = image.ColorConversion;

			NativeBitmap fi = image.NativeBitmap;
			double jpegQuality = image.JpegQuality;

			if (compression == ImageCompression.JPEG)  // compression JPEG ?
			{
				image.ImageStream = Export.EmitJpegImageSurface (baseType, fi, colorConversion, jpegQuality);
			}
			else	// compression ZIP ou aucune ?
			{
				image.ImageStream = Export.EmitLosslessImageSurface (baseType, image, compression, fi, colorConversion);
			}
		}

		private static void EmitImageHeader(Writer writer, PdfComplexSurfaceType baseType, ImageSurface image, ImageCompression compression, ColorConversion colorConversion)
		{
			//	G�n�ration de l'en-t�te.
			int dx = (int) image.Image.Width;
			int dy = (int) image.Image.Height;

			writer.WriteObjectDef (Export.GetComplexSurfaceName (image.Id, baseType));
			writer.WriteString ("<< /Subtype /Image ");

			if (baseType == PdfComplexSurfaceType.XObject)
			{
				if (colorConversion == ColorConversion.ToGray)
				{
					writer.WriteString ("/ColorSpace /DeviceGray ");
				}
				else if (colorConversion == ColorConversion.ToCmyk)
				{
					writer.WriteString ("/ColorSpace /DeviceCMYK ");
				}
				else
				{
					switch (image.NativeBitmap.ColorType)
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
							throw new System.InvalidOperationException (string.Format ("ColorType.{0} not recognized", image.NativeBitmap.ColorType));
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
			int dx = (int) image.Image.Width;
			int dy = (int) image.Image.Height;

			int bpp = 3;
			if (baseType == PdfComplexSurfaceType.XObject)
			{
				if (colorConversion == ColorConversion.ToGray)
				{
					bpp = 1;
				}
				else if (colorConversion == ColorConversion.ToRgb)
				{
					bpp = 3;
				}
				else if (colorConversion == ColorConversion.ToCmyk)
				{
					bpp = 4;
				}
				else
				{
					switch (fi.ColorType)
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
				byte[] zip = IO.DeflateCompressor.Compress (data, 9);  // 9 = compression forte mais lente
				data = zip;
				zip = null;
			}

			using (Port port = new Port ())
			{
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
		}

		private static PdfImageStream EmitJpegImageSurface(PdfComplexSurfaceType baseType, NativeBitmap fi, ColorConversion colorConversion, double jpegQuality)
		{
			bool isGray = false;

			if (colorConversion == ColorConversion.ToGray     ||
				baseType == PdfComplexSurfaceType.XObjectMask ||
				fi.ColorType == BitmapColorType.MinIsBlack    ||
				fi.ColorType == BitmapColorType.MinIsWhite    )
			{
				isGray = true;
			}

			byte[] jpeg;

			if (baseType == PdfComplexSurfaceType.XObjectMask)
			{
				var mask = fi.GetChannel (BitmapColorChannel.Alpha);
				jpeg = mask.SaveToMemory (Export.FilterQualityToMode (jpegQuality));
				mask.Dispose ();
			}
			else if (isGray)
			{
				var gray = fi.ConvertToGrayscale ();
				jpeg = gray.SaveToMemory (Export.FilterQualityToMode (jpegQuality));
				gray.Dispose ();
			}
			else
			{
				if (fi.ColorType == BitmapColorType.Rgb)
				{
					jpeg = fi.SaveToMemory (Export.FilterQualityToMode (jpegQuality));
				}
				else
				{
					var rgb = fi.ConvertToArgb32 ();
					var rgb24 = rgb.ConvertToRgb24 ();
					jpeg = rgb24.SaveToMemory (Export.FilterQualityToMode (jpegQuality));
					rgb24.Dispose ();
					rgb.Dispose ();
				}
			}

			System.Diagnostics.Debug.Assert (jpeg != null);

			using (Port port = new Port ())
			{
				port.PutASCII85 (jpeg);
				Export.debugTotal += jpeg.Length;
				port.PutEOL ();

				jpeg = null;

				return new PdfImageStream ("/Filter [/ASCII85Decode /DCTDecode] ", port.GetPDF ());  // voir [*] page 43
			}
		}

		private ImageCompression GetCompressionMode(PdfComplexSurfaceType baseType)
		{
			//	Ajuste le mode de compression possible.
			ImageCompression compression = this.imageCompression;

			if (baseType == PdfComplexSurfaceType.XObject &&
				this.colorConversion == ColorConversion.ToCmyk &&
				compression == ImageCompression.JPEG) // cmyk impossible en jpg !
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

			if (plan == null &&	channel == BitmapColorChannel.Alpha)
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
		#endregion


		#region Fonts
		private void EmitFonts(Writer writer, Port port)
		{
			if (this.info.TextToCurve)
			{
				return;
			}

			//	Cr�e toutes les fontes.
			foreach (System.Collections.DictionaryEntry dict in port.FontHash)
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
						{
							count = Export.charPerFont;
						}
					}

					this.CreateFontBase       (writer, font, fontPage, count);
					this.CreateFontEncoding   (writer, font, fontPage, count);
					this.CreateFontCharProcs  (writer, font, fontPage, count);
					this.CreateFontWidths     (writer, font, fontPage, count);
					this.CreateFontToUnicode  (writer, font, fontPage, count);
					this.CreateFontCharacters (writer, font, fontPage, count);
				}
			}
		}

		private void CreateFontBase(Writer writer, FontList font, int fontPage, int count)
		{
			//	Cr�e l'objet de base d'une fonte.
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
			//	Cr�e l'objet Encoding d'une fonte.
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
			//	Cr�e l'objet CharProcs d'une fonte.
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
			//	Cr�e l'objet "table des chasses" d'une fonte.
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
			//	Cr�e l'objet ToUnicode d'une fonte, qui permet de retrouver les codes
			//	des caract�res lors d'une copie depuis Acrobat dans le clipboard.
			//	Voir [*] pages 420 � 446.
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
			//	Cr�e tous les objets des caract�res d'une fonte.
			int firstChar = fontPage*Export.charPerFont;
			for (int i=0; i<count; i++)
			{
				CharacterList cl = font.GetCharacter (firstChar+i);
				this.CreateFontCharacter (writer, font, fontPage, cl);
			}
		}

		private void CreateFontCharacter(Writer writer, FontList font, int fontPage, CharacterList cl)
		{
			//	Cr�e l'objet d'un caract�re d'une fonte.
			writer.WriteObjectDef (Export.GetCharacterName (font.Id, fontPage, cl));

			Font drawingFont = font.DrawingFont;
			Drawing.Transform ft = drawingFont.SyntheticTransform;
			int glyph = cl.Glyph;
			var path = new Path ();
			path.Append (drawingFont, glyph, ft.XX, ft.XY, ft.YX, ft.YY, ft.TX, ft.TY);


			using (var port = new Port ()
			{
				ColorForce      = ColorForce.Nothing,  // pas de commande de couleur !
				DefaultDecimals = 4,
			})
			{

				//	Sans "wx wy llx lly urx ury d1", Acrobat 8 n'arrive pas � afficher les caract�res.
				//	Attention, si wx ne correspond pas � la valeur g�n�r�e par CreateFontWidths, Acrobat 8 plante !
				//	Acrobat 8 n'appr�cie pas du tout si "... d1" est remplac� par "wx wy d0" !
				Rectangle bounds = cl.Bounds;
				port.PutValue (cl.Width);       // wx
				port.PutValue (0);              // wy
				port.PutValue (bounds.Left);    // iix
				port.PutValue (bounds.Bottom);  // iiy
				port.PutValue (bounds.Right);   // urx
				port.PutValue (bounds.Top);     // ury
				port.PutCommand ("d1 ");        // voir [*] page 393

				port.PaintSurface (path);

				var pdf = port.GetPDF ();
				writer.WriteLine (string.Format (CultureInfo.InvariantCulture, "<< {0} >>", Port.StringLength (pdf.Length)));
				writer.WriteLine ("stream");
				writer.WriteString (pdf);
				writer.WriteLine ("endstream endobj");
			}
		}
		#endregion


		private IEnumerable<int> Pages
		{
			//	Retourne la liste des pages � imprimer.
			get
			{
				for (int page = 1; page <= this.pageCount; page++)
				{
					if (page >= this.info.PageFrom &&
						page <= this.info.PageTo)
					{
						yield return page;
					}
				}
			}
		}


		public static int GetFontPage(int code)
		{
			return code / Export.charPerFont;
		}

		public static string GetFontIndex(int code)
		{
			return (code % Export.charPerFont).ToString ("X2");
		}


		private static BitmapFileFormat FilterQualityToMode(double quality)
		{
			int n = (int) (quality*100);

			n = System.Math.Min (100, n);
			n = System.Math.Max (10, n);

			return new BitmapFileFormat ()
			{
				Type    = BitmapFileType.Jpeg,
				Quality = n
			};
		}


		//	Constante pour conversion dixi�mes de millim�tres -> 72�me de pouce
		private static readonly double			mmToInches = 0.1*72/25.4;
		private static readonly int				charPerFont = 256;
		
		private static long						debugTotal;

		private readonly ExportPdfInfo			info;
		private readonly List<int>				pages;

		private int								pageCount;
		private Action<Port, int>				renderPage;
		private ColorConversion					colorConversion;
		private ImageCompression				imageCompression;
		private double							jpegQuality;
		private double							imageMinDpi;
		private double							imageMaxDpi;
		private string							documentTitle;
		private double							zoom;
	}

}
