//	Copyright © 2004-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System;
using Epsitec.Common.Drawing;
using Epsitec.Common.Drawing.Platform;

namespace Epsitec.Common.Pdf
{
	using CultureInfo=System.Globalization.CultureInfo;

	/// <summary>
	/// La classe Export implémente la publication d'un document PDF.
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


		public string ExportToFile(string path, int pageCount, Action<Port, int> renderer)
		{
			//	Exporte le document dans un fichier.
			//	Le renderer reçoit le port et le numéro de la page à générer (1..n).
			//	Retourne une éventuelle erreur, ou une chaîne vide.
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty(path));
			System.Diagnostics.Debug.Assert (pageCount > 0);
			System.Diagnostics.Debug.Assert (renderer != null);

			this.pageCount = pageCount;
			this.renderer = renderer;

			this.pages.Clear ();
			this.pages.AddRange (this.Pages);

			if (this.pages.Count == 0)
			{
				return "Empty document";
			}

			try
			{
				this.ExportPdf (path);
				this.DisplayPdf (path);
			}
			catch (PdfExportException ex)
			{
				return ex.Message;  // retourne l'erreur
			}
			finally
			{
				this.ClearImageSurfaces ();
				this.ClearFonts ();
			}

			return null;  // ok
		}

		private void ExportPdf(string path)
		{
			this.ExportBegin ();

			var port = this.CreatePort ();

			using (var writer = this.CreateWriter (path))
			{
				this.PreProcessTexts (port);

				this.EmitHeaderOutlines (writer);
				this.EmitHeaderPages (writer);
				this.EmitPageObjects (writer);
				this.EmitPageObjectResources (writer);

				this.EmitImageSurfaces (writer, port);
				this.EmitFonts (writer);

				this.EmitPageContents (writer, port);
				this.ExportEnd (writer);
			}
		}

		private void DisplayPdf(string path)
		{
			if (this.info.Execute)
			{
				//	Exécute le logiciel "Abobe Acrobat Reader" ou un autre, selon les réglages de Windows.
				System.Diagnostics.Process.Start (path);
			}
		}


		private void ExportBegin()
		{
			this.colorConversion  = this.info.ColorConversion;
			this.imageCompression = this.info.ImageCompression;
			this.jpegQuality      = this.info.JpegQuality;
			this.imageMinDpi      = this.info.ImageMinDpi;
			this.imageMaxDpi      = this.info.ImageMaxDpi;
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
			FontHash fontHash = this.info.TextToCurve ? null : this.fontHash;

			var port = new Port (fontHash, this.characterHash);
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
			string creationDate = Export.GetDateString    (Support.Globals.Properties.GetProperty<System.DateTime> ("PDF:CreationDate", System.DateTime.Now));

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


		private void PreProcessTexts(Port port)
		{
			if (this.info.TextToCurve)
			{
				return;
			}

			//	Il faut passer en revue tous les caractères de tous les textes, pour
			//	pouvoir ensuite créer les polices pdf.
			{
				port.IsPreProcessText = true;

				foreach (var page in this.pages)
				{
					this.renderer (port, page);  // pré-prossessing des textes de la page
				}

				port.IsPreProcessText = false;
			}

			FontList.CreateFonts (this.fontHash, this.characterHash);
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

				//	La boîte de débord ne prend en compte que le débord effectivement
				//	demandé (qui peut être différent entre pages de droite et pages
				//	de gauche).
				bleedBox.Inflate (left, right, top, bottom);

				left   = System.Math.Max (left,   markSizeX);
				right  = System.Math.Max (right,  markSizeX);
				top    = System.Math.Max (top,    markSizeY);
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

		private void EmitPageObjectResources(Writer writer)
		{
			//	Un objet pour les ressources de chaque page.
			foreach (var page in this.pages)
			{
				writer.WriteObjectDef (Export.GetPageResourceName (page));
				writer.WriteString ("<< /ProcSet [/PDF /Text /ImageB /ImageC] ");

				if (this.imageSurfaces.Count > 0)
				{
					writer.WriteString ("/ExtGState << ");
					writer.WriteString (Export.GetComplexSurfaceShortName (0, PdfComplexSurfaceType.ExtGState));
					writer.WriteObjectRef (Export.GetComplexSurfaceName (0, PdfComplexSurfaceType.ExtGState));
					writer.WriteString (">> ");

					writer.WriteString ("/XObject << ");
					foreach (ImageSurface image in this.imageSurfaces)
					{
						if (!image.Exists)
						{
							continue;
						}

						writer.WriteString (Export.GetComplexSurfaceShortName (image.Id, PdfComplexSurfaceType.XObject));
						writer.WriteObjectRef (Export.GetComplexSurfaceName (image.Id, PdfComplexSurfaceType.XObject));
					}
					writer.WriteString (">> ");
				}

				if (!this.info.TextToCurve)
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

		private void EmitSinglePageContents(Writer writer, Port port, Point currentPageOffset, int page)
		{
			port.Reset ();
			var gtBeforeZoom = this.SetupTransformForPageExport (port, currentPageOffset);
			this.renderer (port, page);  // effectue le rendu de la page
			port.Transform = gtBeforeZoom;

			this.CropToBleedBox (port, page);  // efface ce qui dépasse de la BleedBox
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
			//	du BleedBox (à savoir la taille de la page avec ses débords).
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

			if (Support.Globals.Properties.IsPropertyDefined ("PDF:DisableClipToBleedBox"))
			{
				//	Pas de clipping...
			}
			else
			{
				using (Path path = new Path ())
				{
					const double clip=1000.0;

					path.MoveTo (  0.0 - clip,    0.0 - clip);
					path.LineTo (width + clip,    0.0 - clip);
					path.LineTo (width + clip, height + clip);
					path.LineTo (  0.0 - clip, height + clip);
					path.LineTo (  0.0 - clip,    0.0 - clip);
					path.Close ();

					path.MoveTo (  0.0 - left,     0.0 - bottom);
					path.LineTo (  0.0 - left,  height + top);
					path.LineTo (width + right, height + top);
					path.LineTo (width + right,    0.0 - bottom);
					path.LineTo (  0.0 - left,     0.0 - bottom);
					path.Close ();

					port.RichColor = RichColor.FromCmyk (0.0, 0.0, 0.0, 0.0);  // masque blanc
					port.PaintSurface (path);
				}
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
				port.RichColor = RichColor.FromCmyk (0.0, 0.0, 0.0, 0.0);  // fond blanc derrière les traits de coupe
				port.PaintOutline (path);

				port.LineWidth = this.info.CropMarksWidth;
				port.LineCap   = CapStyle.Butt;
				port.RichColor = RichColor.FromCmyk (1.0, 1.0, 1.0, 1.0);  // noir de repérage
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
				port.RichColor = RichColor.FromCmyk (1.0, 1.0, 1.0, 1.0);  // noir de repérage
				port.PaintSurface (path);
			}
		}


		#region Images
		private void EmitImageSurfaces(Writer writer, Port port)
		{
			//	Crée toutes les images.
			Export.debugTotal = 0;

			foreach (ImageSurface image in this.imageSurfaces)
			{
				if (this.CreateImageSurface (writer, image, PdfComplexSurfaceType.XObject, PdfComplexSurfaceType.XObjectMask))
				{
					this.CreateImageSurface (writer, image, PdfComplexSurfaceType.XObjectMask, PdfComplexSurfaceType.None);
				}

				writer.Flush ();  // écrit déjà tout ce qu'on peut sur disque, afin d'utiliser le moins possible de mémoire
			}
		}

		private bool CreateImageSurface(Writer writer, ImageSurface image,
										PdfComplexSurfaceType baseType, PdfComplexSurfaceType maskType)
		{
			ImageCompression compression = this.GetCompressionMode (baseType);

			//	Crée une image.

			image.MinDpi = this.imageMinDpi;
			image.MaxDpi = this.imageMaxDpi;

			image = this.ProcessImageAndCreatePdfStream (image, baseType, compression);
			
			Export.EmitImageHeader (writer, baseType, image, compression, this.colorConversion);

			writer.WriteString (image.ImageStream.Code);

			if (maskType == PdfComplexSurfaceType.XObjectMask && image.IsTransparent)
			{
				writer.WriteString ("/SMask ");
				writer.WriteObjectRef (Export.GetComplexSurfaceName (image.Id, maskType));
			}

			writer.WriteLine (string.Format (CultureInfo.InvariantCulture, " {0} >>", Port.StringLength (image.ImageStream.StreamLength)));
			writer.WriteLine ("stream");
			writer.WriteStream (image.ImageStream.Stream);
			writer.WriteLine ("endstream endobj");

			bool transparent = image.IsTransparent;

			image.Dispose ();

			return transparent;
		}

		private static NativeBitmap PrepareImage(ImageSurface image)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("PrepareImage, image:\n{0}", image == null ? "<null>" : image.GetDebugInformation()));
			double imageMinDpi = image.MinDpi;
			double imageMaxDpi = image.MaxDpi;

			int dx = image.DX;
			int dy = image.DY;

			NativeBitmap fi = Export.LoadImage (image);

			System.Diagnostics.Debug.WriteLine (string.Format ("PrepareImage, before crop and resizing:\n{0}", fi == null ? "<null>" : fi.ToString()));
			fi = Export.CropImage (image, fi);
			System.Diagnostics.Debug.WriteLine (string.Format ("PrepareImage, After crop:\n{0}", fi == null ? "<null>" : fi.ToString()));

			// --------------- 06-02-2012 15:45 -----------------
			// Start of JFC modification : For debug purpose
			// --------------------------------------------------
			// Original Code :

			fi = Export.ResizeImage (image, fi, imageMinDpi, imageMaxDpi, out dx, out dy);
			// Modified Code :
//+			dx = fi.Width;
//+			dy = fi.Height;
			// --------------------------------------------------
			// End of JFC modification (06-févr.-2012 15:45)
			// --------------------------------------------------
			System.Diagnostics.Debug.WriteLine (string.Format ("PrepareImage: after crop & resize, Size={0}x{1}\n{2}", dx, dy, fi == null ? "<null>" : fi.ToString()));

			image.ColorType     = fi.ColorType;
			image.IsTransparent = fi.IsTransparent;
			image.DX            = dx;
			image.DY            = dy;

			return fi;
		}

		private ImageSurface ProcessImageAndCreatePdfStream(ImageSurface image, PdfComplexSurfaceType baseType, ImageCompression compression)
		{
			image.ColorConversion  = this.colorConversion;
			image.JpegQuality      = this.jpegQuality;
			image.SurfaceType      = baseType;
			image.ImageCompression = compression;

			System.Diagnostics.Debug.WriteLine (string.Format ("ProcessImageAndCreatePdfStream> SurfaceType={0}, ImageCompression={1}, ColorConversion={2}, Quality={3}", baseType, compression, this.colorConversion, this.jpegQuality));

#if true
			image = Export.LaunchProcessImageAndCreatePdfStream (image);

			var source = ImageSurface.Serialize (image);
			
			System.Diagnostics.Debug.WriteLine ("----------------------------------------------------------------------");
			System.Diagnostics.Debug.WriteLine (source);
			System.Diagnostics.Debug.WriteLine ("----------------------------------------------------------------------");
			
			return image;
#else
			Export.ExecuteProcessImageAndCreatePdfStream (image);
			return image;
#endif
		}

		private static ImageSurface LaunchProcessImageAndCreatePdfStream(ImageSurface image)
		{
			string path = System.IO.Path.GetTempFileName ();

			try
			{
				System.IO.File.WriteAllText (path, ImageSurface.Serialize (image), System.Text.Encoding.UTF8);

				const string program="Common.Document.ExportEngine.exe";

				string exe1 = System.IO.Path.Combine (Epsitec.Common.Support.Globals.Directories.Executable, program);
				string exe2 = System.IO.Path.Combine (Epsitec.Common.Support.Globals.Directories.InitialDirectory, program);
				string exe3 = System.IO.Path.Combine (System.IO.Path.GetDirectoryName (typeof (Export).Assembly.Location), program);

				string exe = System.IO.File.Exists (exe1) ? exe1 :
							 System.IO.File.Exists (exe2) ? exe2 :
							 exe3;

				string args = string.Concat (@"""", path, @"""");
				var process = System.Diagnostics.Process.Start (exe, args);

				process.WaitForExit ();

				return ImageSurface.Deserialize (System.IO.File.ReadAllText (path, System.Text.Encoding.UTF8));
			}
			finally
			{
				System.IO.File.Delete (path);
			}
		}

		public static void ExecuteProcessImageAndCreatePdfStream(ImageSurface image)
		{
			Export.InternalProcessImageAndCreatePdfStream (image);
		}


		private static void InternalProcessImageAndCreatePdfStream(ImageSurface image)
		{
			PdfComplexSurfaceType baseType        = image.SurfaceType;
			ImageCompression      compression     = image.ImageCompression;
			ColorConversion       colorConversion = image.ColorConversion;
			
			double jpegQuality = image.JpegQuality;

			System.Diagnostics.Debug.WriteLine (string.Format ("ProcessImageAndCreatePdfStream: SurfaceType={0}, ImageCompression={1}, ColorConversion={2}, Quality={3}", baseType, compression, colorConversion, jpegQuality));

			using (NativeBitmap fi = Export.PrepareImage (image))
			{
				if (compression == ImageCompression.JPEG)  // compression JPEG ?
				{
					image.ImageStream = Export.EmitJpegImageSurface (baseType, fi, colorConversion, jpegQuality);
				}
				else	// compression ZIP ou aucune ?
				{
					image.ImageStream = Export.EmitLosslessImageSurface (baseType, image, compression, fi, colorConversion);
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

			System.Diagnostics.Debug.WriteLine (string.Format ("EmitHeader: {0} XML={1}", baseType, image.GetDebugInformation ()));

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
							throw new System.InvalidOperationException (string.Format ("ColorType.{0} not recognized", image.ColorType));
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

			Port port = new Port (null, null);
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

			if ((colorConversion == ColorConversion.ToGray) ||
				(baseType == PdfComplexSurfaceType.XObjectMask) ||
				(fi.ColorType == BitmapColorType.MinIsBlack) ||
				(fi.ColorType == BitmapColorType.MinIsWhite))
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

			Port port = new Port (null, null);
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

		/// <summary>
		/// Rescales image to limit its printed resolution to what is sufficient for
		/// printing (usually 300 dpi)
		/// </summary>
		private static NativeBitmap ResizeImage(ImageSurface image, NativeBitmap fi, double imageMinDpi, double imageMaxDpi, out int dx, out int dy)
		{
			dx = fi.Width;
			dy = fi.Height;
			//	Mise à l'échelle éventuelle de l'image selon les choix de l'utilisateur.
			//	Une image sans filtrage n'est jamais mise à l'échelle !
			const double tenthMillimeterPerInch = 254;
			double currentDpiX = dx * tenthMillimeterPerInch / image.ImageSize.Width;      	// Image size is expressed in mm/10 !!!
			double currentDpiY = dy * tenthMillimeterPerInch / image.ImageSize.Height;
			System.Diagnostics.Debug.WriteLine("Current Dpi: {0} x {1}", currentDpiX, currentDpiY);
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
			System.Diagnostics.Debug.WriteLine("Final Dpi:   {0} x {1}", finalDpiX, finalDpiY);

			bool resizeRequired = false;

			// --------------- 07-02-2012 14:52 ------------------
			//+ Start of JFC modification: This is a workaround to compensate the fact that either image.ImageSize.Width or image.ImageSize.Height
			//+ 						   may be not correct (for example square source image inserted as a thin rectangle will have its height equal
			//+                            to its width)
			// --------------------------------------------------
			//  Original Code:
//+			if (currentDpiX != finalDpiX || currentDpiY != finalDpiY)
//+			{
//+				dx = (int) System.Math.Ceiling ((dx+0.5)*finalDpiX/currentDpiX);
//+				dy = (int) System.Math.Ceiling ((dy+0.5)*finalDpiY/currentDpiY);

//+				if (dx < 1)
//+				{
//+					dx = 1;
//+				}
//+				if (dy < 1)
//+				{
//+					dy = 1;
//+				}

//+				resizeRequired = true;
//+				System.Diagnostics.Debug.WriteLine("Dpi are different, resizeRequired: {0}", resizeRequired);
//+			}
			//  Modified Code:
			System.Diagnostics.Debug.WriteLine("Export.ResizeImage: USING TEMPORARY WORKAROUND !!!!!");
			var usedCurrentDpi = System.Math.Min(currentDpiX, currentDpiY);
			var usedFinalDpi   = (usedCurrentDpi == currentDpiX) ? finalDpiX : finalDpiY;
			if (usedCurrentDpi != usedFinalDpi)
			{
				dx = (int) System.Math.Ceiling ((dx + 0.5) * usedFinalDpi / usedCurrentDpi);
				dy = (int) System.Math.Ceiling ((dy + 0.5) * usedFinalDpi / usedCurrentDpi);

				dx = System.Math.Max(1, dx);
				dy = System.Math.Max(1, dy);

				resizeRequired = true;
				System.Diagnostics.Debug.WriteLine("Dpi are different, resize is required:, now, dx = {0}, dy = {1}", dx, dy);
			}
			// --------------------------------------------------
			// End of JFC modification (07-févr.-2012 14:52)
			// --------------------------------------------------

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
				(this.colorConversion == ColorConversion.ToCmyk) &&
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
			if (this.info.TextToCurve)
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
						{
							count = Export.charPerFont;
						}
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
			var path = new Path ();
			path.Append (drawingFont, glyph, ft.XX, ft.XY, ft.YX, ft.YY, ft.TX, ft.TY);


			var port = new Port (null, null)
			{
				ColorForce      = ColorForce.Nothing,  // pas de commande de couleur !
				DefaultDecimals = 4,
			};

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

			var pdf = port.GetPDF ();
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


		private IEnumerable<int> Pages
		{
			//	Retourne la liste des pages à imprimer.
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


		//	Constante pour conversion dixièmes de millimètres -> 72ème de pouce
		private static readonly double			mmToInches = 0.1*72/25.4;
		private static readonly int				charPerFont = 256;
		
		private static long						debugTotal;

		private readonly ExportPdfInfo			info;
		private readonly List<int>				pages;

		private int								pageCount;
		private Action<Port, int>				renderer;
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
