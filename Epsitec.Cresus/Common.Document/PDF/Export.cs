using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	public enum TypeComplexSurface
	{
		None            = -1,
		ExtGState       = 0,
		ExtGStateP1     = 1,
		ExtGStateP2     = 2,
		ExtGStateSmooth = 3,
		ShadingColor    = 4,
		ShadingGray     = 5,
		XObject         = 6,
		XObjectSmooth   = 7,
		XObjectMask     = 7,
	}

	public enum TypeFont
	{
		Base      = 0,
		Encoding  = 1,
		CharProcs = 2,
		Widths    = 3,
		ToUnicode = 4,
	}

	public enum ColorConversion
	{
		None   = 0,
		ToRgb  = 1,
		ToCmyk = 2,
		ToGray = 3,
	}

	public enum ImageCompression
	{
		None = 0,
		ZIP  = 1,
		JPEG = 2,
	}

	/// <summary>
	/// La classe Export implémente la publication d'un document PDF.
	/// [*] = documentation PDF Reference, version 1.6, fifth edition, 1236 pages
	/// </summary>
	/// 
	public class Export
	{
		protected enum TypeFunction
		{
			SampledColor = 0,
			SampledAlpha = 1,
			Color1       = 2,	// red or cyan or gray
			Color2       = 3,	// green or magenta
			Color3       = 4,	// blue or yellow
			Color4       = 5,	// black
			Alpha        = 6,
		}

		public Export(Document document)
		{
			this.document = document;
		}

		public void Dispose()
		{
		}

		public string FileExport(string filename)
		{
			//	Exporte le document dans un fichier.
			Settings.ExportPDFInfo info = this.document.Settings.ExportPDFInfo;
			this.colorConversion = info.ColorConversion;
			this.imageCompression = info.ImageCompression;
			this.jpegQuality = info.JpegQuality;
			this.imageMinDpi = info.ImageMinDpi;
			this.imageMaxDpi = info.ImageMaxDpi;

			this.complexSurfaces = new System.Collections.ArrayList();
			this.imageSurfaces   = new System.Collections.ArrayList();
			this.characterList   = new System.Collections.Hashtable();
			this.fontList        = new System.Collections.Hashtable();

			Port port = new Port(this.complexSurfaces, this.imageSurfaces, info.TextCurve ? null : this.fontList);
			port.PushColorModifier(new ColorModifierCallback(this.FinalOutputColorModifier));

			//	Crée le DrawingContext utilisé pour l'exportation.
			DrawingContext drawingContext;
			drawingContext = new DrawingContext(this.document, null);
			drawingContext.ContainerSize = this.document.DocumentSize;
			drawingContext.PreviewActive = true;

			int max  = this.document.Modifier.PrintableTotalPages();
			int from = 1;
			int to   = max;
			bool justeOneMaster = false;

			Settings.PrintRange range = info.PageRange;

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
					justeOneMaster = true;
				}
			}

			from = System.Math.Min(from, max);
			to   = System.Math.Min(to,   max);

			if ( from > to )
			{
				Misc.Swap(ref from, ref to);
			}
			int total = to-from+1;

			//	Calcule la liste des pages à imprimer.
			this.pageList = new System.Collections.ArrayList();

			if ( justeOneMaster )
			{
				this.pageList.Add(from);
			}
			else
			{
				for ( int page=from ; page<=to ; page++ )
				{
					int rank = this.document.Modifier.PrintablePageRank(page-1);
					if ( rank != -1 )
					{
						this.pageList.Add(rank);
					}
				}
			}

			this.FoundComplexSurfaces(port, drawingContext);

			//	Crée et ouvre le fichier.
			Writer writer;
			try
			{
				if ( System.IO.File.Exists(filename) )
				{
					System.IO.File.Delete(filename);
				}

				writer = new Writer(filename);
			}
			catch ( System.Exception e )
			{
				return e.Message;
			}

			//	Objet racine du document.
			writer.WriteObjectDef("Root");
			writer.WriteString("<< /Type /Catalog /Outlines ");
			writer.WriteObjectRef("HeaderOutlines");
			writer.WriteString("/Pages ");
			writer.WriteObjectRef("HeaderPages");
			writer.WriteLine(">> endobj");

			//	Objet outlines.
			writer.WriteObjectDef("HeaderOutlines");
			writer.WriteLine("<< /Type /Outlines /Count 0 >> endobj");

			//	Objet décrivant le format de la page.
			Point pageOffset = new Point(0.0, 0.0);

			if ( info.Debord > 0.0 )
			{
				pageOffset.X += info.Debord;
				pageOffset.Y += info.Debord;
			}

			if ( info.Target )  // traits de coupe ?
			{
				pageOffset.X += info.TargetLength;
				pageOffset.Y += info.TargetLength;
			}

			//	Objet donnant la liste des pages.
			writer.WriteObjectDef("HeaderPages");
			writer.WriteString("<< /Type /Pages /Kids [ ");
			foreach ( int page in this.pageList )
			{
				writer.WriteObjectRef(Export.NamePage(page));
			}
			writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "] /Count {0} >> endobj", total));

			//	Un objet pour chaque page.
			foreach ( int page in this.pageList )
			{
				Size pageSize = this.document.GetPageSize(page);

				if ( info.Debord > 0.0 )
				{
					pageSize.Width  += info.Debord*2.0;
					pageSize.Height += info.Debord*2.0;
				}

				if ( info.Target )  // traits de coupe ?
				{
					pageSize.Width  += info.TargetLength*2.0;
					pageSize.Height += info.TargetLength*2.0;
				}

				writer.WriteObjectDef(Export.NamePage(page));
				writer.WriteString("<< /Type /Page /Parent ");
				writer.WriteObjectRef("HeaderPages");
				writer.WriteString("/MediaBox [0 0 ");
				writer.WriteString(Port.StringValue(pageSize.Width*Export.mm2in));
				writer.WriteString(" ");
				writer.WriteString(Port.StringValue(pageSize.Height*Export.mm2in));
				writer.WriteString("] /Resources ");
				writer.WriteObjectRef(Export.NameResources(page));
				writer.WriteString("/Contents ");
				writer.WriteObjectRef(Export.NameContent(page));
				writer.WriteLine(">> endobj");
			}

			//	Un objet pour les ressources de chaque page.
			foreach ( int page in this.pageList )
			{
				writer.WriteObjectDef(Export.NameResources(page));
				writer.WriteString("<< /ProcSet [/PDF /Text /ImageB /ImageC] ");

				int tcs = this.TotalComplexSurface(page);
				if ( tcs > 0 || this.imageSurfaces.Count > 0 )
				{
					writer.WriteString("/ExtGState << ");
					writer.WriteString(Export.ShortNameComplexSurface(0, TypeComplexSurface.ExtGState));
					writer.WriteObjectRef(Export.NameComplexSurface(0, TypeComplexSurface.ExtGState));
					for ( int index=0 ; index<tcs ; index++ )
					{
						ComplexSurface cs = this.GetComplexSurface(page, index);
						System.Diagnostics.Debug.Assert(cs != null);
						if ( cs.Type == Type.TransparencyRegular  ||
							 cs.Type == Type.TransparencyGradient ||
							 cs.IsSmooth                          )
						{
							writer.WriteString(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.ExtGState));
							writer.WriteObjectRef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.ExtGState));
						}
						if ( cs.Type == Type.TransparencyPattern  )
						{
							writer.WriteString(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.ExtGStateP1));
							writer.WriteObjectRef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.ExtGStateP1));

							writer.WriteString(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.ExtGStateP2));
							writer.WriteObjectRef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.ExtGStateP2));
						}
					}
					writer.WriteString(">> ");

					writer.WriteString("/Shading << ");
					for ( int index=0 ; index<tcs ; index++ )
					{
						ComplexSurface cs = this.GetComplexSurface(page, index);
						System.Diagnostics.Debug.Assert(cs != null);
						if ( cs.Type == Type.OpaqueGradient       ||
							 cs.Type == Type.TransparencyGradient )
						{
							writer.WriteString(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.ShadingColor));
							writer.WriteObjectRef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.ShadingColor));
						}
					}
					writer.WriteString(">> ");

					writer.WriteString("/XObject << ");
					for ( int index=0 ; index<tcs ; index++ )
					{
						ComplexSurface cs = this.GetComplexSurface(page, index);
						System.Diagnostics.Debug.Assert(cs != null);
						if ( cs.Type == Type.OpaquePattern       ||
							 cs.Type == Type.TransparencyPattern )
						{
							writer.WriteString(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.XObject));
							writer.WriteObjectRef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.XObject));
						}
					}
					foreach ( ImageSurface image in this.imageSurfaces )
					{
						if ( image.DrawingImage == null )  continue;
						writer.WriteString(Export.ShortNameComplexSurface(image.Id, TypeComplexSurface.XObject));
						writer.WriteObjectRef(Export.NameComplexSurface(image.Id, TypeComplexSurface.XObject));
					}
					writer.WriteString(">> ");
				}

				if ( !info.TextCurve )
				{
					writer.WriteString("/Font << ");
					foreach ( System.Collections.DictionaryEntry dict in this.fontList )
					{
						FontList font = dict.Key as FontList;
						int totalPages = (font.CharacterCount+Export.charPerFont-1)/Export.charPerFont;
						for ( int fontPage=0 ; fontPage<totalPages ; fontPage++ )
						{
							writer.WriteString(Export.ShortNameFont(font.Id, fontPage));
							writer.WriteObjectRef(Export.NameFont(font.Id, fontPage, TypeFont.Base));
						}
					}
					writer.WriteString(">> ");
				}

				//?writer.WriteLine("/ColorSpace << /Cs [/Pattern /DeviceRGB] >> ");
				writer.WriteLine(">> endobj");
			}

			//	Crée les objets de définition.
			this.CreateComplexSurface(writer, port, drawingContext);
			this.CreateImageSurface(writer, port);

			if ( !info.TextCurve )
			{
				this.CreateFont(writer);
			}

			//	Un objet pour le contenu de chaque page.
			foreach ( int page in this.pageList )
			{
				port.Reset();

				//	Matrice de transformation globale:
				Transform gt = port.Transform;
				gt.Translate(pageOffset);  // translation si débord
				gt.Scale(Export.mm2in);  // unité = 0.1mm
				port.Transform = gt;

				System.Collections.ArrayList layers = this.ComputeLayers(page);
				foreach ( Objects.Layer layer in layers )
				{
					Properties.ModColor modColor = layer.PropertyModColor;
					port.PushColorModifier(new ColorModifierCallback(modColor.ModifyColor));
					drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
					port.PushColorModifier(new ColorModifierCallback(drawingContext.DimmedColor));

					foreach ( Objects.Abstract obj in this.document.Deep(layer) )
					{
						if ( obj.IsHide )  continue;  // objet caché ?

						obj.DrawGeometry(port, drawingContext);
					}

					port.PopColorModifier();
					port.PopColorModifier();
				}

				this.DrawTarget(port, info, page);  // traits de coupe

				string pdf = port.GetPDF();
				writer.WriteObjectDef(Export.NameContent(page));
				writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "<< {0} >>", Port.StringLength(pdf.Length)));
				writer.WriteLine("stream");
				writer.WriteString(pdf);
				writer.WriteLine("endstream endobj");
			}

			writer.Flush();
			this.FlushComplexSurface();
			this.FlushImageSurface();
			this.FlushFont();

			return "";  // ok
		}

		protected static string NamePage(int page)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "HeaderPage{0}", page);
		}

		protected static string NameResources(int page)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "HeaderResources{0}", page);
		}

		protected static string NameContent(int page)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "HeaderContent{0}", page);
		}

		protected static string NameComplexSurface(int id, TypeComplexSurface type)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "HeaderComplexSurface{0}", id*10+(int)type);
		}

		public static string ShortNameComplexSurface(int id, TypeComplexSurface type)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "/S{0} ", id*10+(int)type);
		}

		protected static string NameFont(int id, int fontPage, TypeFont type)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "HeaderFont{0}", id*100 + fontPage*10 + (int)type);
		}

		public static string ShortNameFont(int id, int fontPage)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "/F{0} ", id*10 + fontPage);
		}

		protected static string NameCharacter(int id, int fontPage, CharacterList cl)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "HeaderCharacter{0}_{1}_{2}_{3}", id, fontPage, cl.Glyph, cl.Unicode);
		}

		protected static string ShortNameCharacter(int id, int fontPage, CharacterList cl)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "/C{0}_{1}_{2}_{3} ", id, fontPage, cl.Glyph, cl.Unicode);
		}

		protected static string NameFunction(int id, TypeFunction type)
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "HeaderFunction{0}", id*10+(int)type);
		}


		protected void FinalOutputColorModifier(ref RichColor color)
		{
			//	Modification finale d'une couleur en fonction du mode de sortie.
			if ( this.colorConversion == PDF.ColorConversion.ToRgb )
			{
				color.ColorSpace = ColorSpace.Rgb;
			}
			else if ( this.colorConversion == PDF.ColorConversion.ToCmyk )
			{
				color.ColorSpace = ColorSpace.Cmyk;
			}
			else if ( this.colorConversion == PDF.ColorConversion.ToGray )
			{
				color.ColorSpace = ColorSpace.Gray;
			}
		}


		protected void DrawTarget(Port port, Settings.ExportPDFInfo info, int page)
		{
			//	Dessine les traits de coupe.
			if ( !info.Target )  return;

			Size pageSize = this.document.GetPageSize(page);
			double width  = pageSize.Width;
			double height = pageSize.Height;
			double debord = info.Debord;
			double length = info.TargetLength;

			Path path = new Path();

			//	Traits horizontaux.
			path.MoveTo(0.0-debord, 0.0);
			path.LineTo(0.0-debord-length, 0.0);

			path.MoveTo(0.0-debord, height);
			path.LineTo(0.0-debord-length, height);

			path.MoveTo(width+debord, 0.0);
			path.LineTo(width+debord+length, 0.0);

			path.MoveTo(width+debord, height);
			path.LineTo(width+debord+length, height);

			//	Traits verticaux.
			path.MoveTo(0.0, 0.0-debord);
			path.LineTo(0.0, 0.0-debord-length);

			path.MoveTo(width, 0.0-debord);
			path.LineTo(width, 0.0-debord-length);

			path.MoveTo(0.0, height+debord);
			path.LineTo(0.0, height+debord+length);

			path.MoveTo(width, height+debord);
			path.LineTo(width, height+debord+length);

			port.LineWidth = info.TargetWidth;
			port.RichColor = RichColor.FromCmyk(1.0, 1.0, 1.0, 1.0);  // noir de repérage
			port.PaintOutline(path);
		}


		#region ComplexSurface
		protected void FoundComplexSurfaces(Port port, DrawingContext drawingContext)
		{
			//	Trouve toutes les surfaces complexes dans toutes les pages.
			int id = 1;
			foreach ( int page in this.pageList )
			{
				System.Collections.ArrayList layers = this.ComputeLayers(page);
				foreach ( Objects.Layer layer in layers )
				{
					Properties.ModColor modColor = layer.PropertyModColor;
					port.PushColorModifier(new ColorModifierCallback(modColor.ModifyColor));
					drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
					port.PushColorModifier(new ColorModifierCallback(drawingContext.DimmedColor));

					foreach ( Objects.Abstract obj in this.document.Deep(layer) )
					{
						if ( obj.IsHide )  continue;  // objet caché ?

						System.Collections.ArrayList list = obj.GetComplexSurfacesPDF(port);
						int total = list.Count;
						for ( int rank=0 ; rank<total ; rank++ )
						{
							if ( list[rank] is Properties.Gradient )
							{
								Properties.Gradient gradient = list[rank] as Properties.Gradient;
								Properties.Line stroke = null;

								if ( gradient.Type == Properties.Type.LineColor )
								{
									stroke = obj.PropertyLineMode;
								}

								ComplexSurface cs = new ComplexSurface(page, port, layer, obj, gradient, stroke, rank, id++);
								this.complexSurfaces.Add(cs);
							}

							if ( list[rank] is Properties.Font )
							{
								Properties.Font font = list[rank] as Properties.Font;

								ComplexSurface cs = new ComplexSurface(page, port, layer, obj, font, null, rank, id++);
								this.complexSurfaces.Add(cs);
							}
						}

						if ( obj is Objects.Image )
						{
							Objects.Image objImage = obj as Objects.Image;
							Properties.Image propImage = obj.PropertyImage;
							System.Diagnostics.Debug.Assert(propImage != null);
							string filename = propImage.Filename;
							ImageFilter filter = propImage.Filter ? new Drawing.ImageFilter (Drawing.ImageFilteringMode.Bilinear) : new Drawing.ImageFilter (Drawing.ImageFilteringMode.None);
							Margins crop = propImage.CropMargins;
							Size size = objImage.ImageBitmapSize;

							ImageSurface image = ImageSurface.Search(this.imageSurfaces, filename, size, crop, filter);
							if ( image == null )
							{
								ImageCache.Item item = this.document.ImageCache.Get(filename);
								System.Diagnostics.Debug.Assert(item != null);
								image = new ImageSurface(item, size, crop, filter, id++);
								this.imageSurfaces.Add(image);
							}
						}

						obj.FillOneCharList(port, drawingContext, this.characterList);
					}

					port.PopColorModifier();
					port.PopColorModifier();
				}
			}

			FontList.CreateFonts(this.fontList, this.characterList);
		}

		protected int TotalComplexSurface(int page)
		{
			//	Calcule le nombre de surfaces complexes dans une page.
			int total = 0;
			for ( int i=0 ; i<this.complexSurfaces.Count ; i++ )
			{
				ComplexSurface cs = this.complexSurfaces[i] as ComplexSurface;
				if ( cs.Page == page )  total++;
			}
			return total;
		}

		protected ComplexSurface GetComplexSurface(int page, int index)
		{
			//	Donne une surface complexe.
			int ip = 0;
			for ( int i=0 ; i<this.complexSurfaces.Count ; i++ )
			{
				ComplexSurface cs = this.complexSurfaces[i] as ComplexSurface;
				if ( cs.Page == page )
				{
					if ( ip == index )  return cs;
					ip ++;
				}
			}
			return null;
		}

		protected void CreateComplexSurface(Writer writer, Port port, DrawingContext drawingContext)
		{
			//	Crée toutes les surfaces complexes.
			//	Crée le ExtGState numéro 0, pour annuler une transparence.
			writer.WriteObjectDef(Export.NameComplexSurface(0, TypeComplexSurface.ExtGState));
			writer.WriteLine("<< /CA 1 /ca 1 >> endobj");

			for ( int i=0 ; i<this.complexSurfaces.Count ; i++ )
			{
				ComplexSurface cs = this.complexSurfaces[i] as ComplexSurface;

				Objects.Layer layer = cs.Layer;
				Properties.ModColor modColor = layer.PropertyModColor;
				port.PushColorModifier(new ColorModifierCallback(modColor.ModifyColor));
				drawingContext.IsDimmed = (layer.Print == Objects.LayerPrint.Dimmed);
				port.PushColorModifier(new ColorModifierCallback(drawingContext.DimmedColor));

				cs.Object.SurfaceAnchor.LineUse = cs.Fill.IsStrokingGradient;
				this.MatrixComplexSurfaceGradient(cs);

				switch ( cs.Type )
				{
					case PDF.Type.OpaqueRegular:
						this.CreateComplexSurfaceTransparencyGradientOrSmooth(writer, port, drawingContext, cs);
						break;

					case PDF.Type.TransparencyRegular:
						this.CreateComplexSurfaceTransparencyRegular(writer, port, drawingContext, cs);
						break;

					case PDF.Type.OpaqueGradient:
						this.CreateComplexSurfaceOpaqueGradient(writer, port, drawingContext, cs);
						break;

					case PDF.Type.TransparencyGradient:
						this.CreateComplexSurfaceTransparencyGradientOrSmooth(writer, port, drawingContext, cs);
						break;

					case PDF.Type.OpaquePattern:
						this.CreateComplexSurfaceOpaquePattern(writer, port, drawingContext, cs);
						break;

					case PDF.Type.TransparencyPattern:
						this.CreateComplexSurfaceTransparencyPattern(writer, port, drawingContext, cs);
						this.CreateComplexSurfaceOpaquePattern(writer, port, drawingContext, cs);
						break;
				}

				port.PopColorModifier();
				port.PopColorModifier();
			}
		}

		protected void CreateComplexSurfaceTransparencyRegular(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface transparente unie.
			if ( cs.IsSmooth )
			{
				this.CreateComplexSurfaceTransparencyGradientOrSmooth(writer, port, drawingContext, cs);
			}
			else
			{
				double a = 1.0;
				if ( cs.Fill is Properties.Gradient )
				{
					Properties.Gradient gradient = cs.Fill as Properties.Gradient;
					a = port.GetFinalColor(gradient.Color1.Basic).A;
				}
				if ( cs.Fill is Properties.Font )
				{
					Properties.Font font = cs.Fill as Properties.Font;
					a = port.GetFinalColor(font.FontColor.Basic).A;
				}

				this.CreateComplexSurfaceAlpha(writer, a, cs.Id, TypeComplexSurface.ExtGState);
			}
		}

		protected void CreateComplexSurfaceTransparencyPattern(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée deux surfaces transparentes unies pour un motif.
			double a1 = 1.0;
			double a2 = 1.0;
			if ( cs.Fill is Properties.Gradient )
			{
				Properties.Gradient gradient = cs.Fill as Properties.Gradient;
				a1 = port.GetFinalColor(gradient.Color1.Basic).A;
				a2 = port.GetFinalColor(gradient.Color2.Basic).A;
			}
			if ( cs.Fill is Properties.Font )
			{
				Properties.Font font = cs.Fill as Properties.Font;
				a1 = port.GetFinalColor(font.FontColor.Basic).A;
				a2 = a1;
			}

			this.CreateComplexSurfaceAlpha(writer, a1, cs.Id, TypeComplexSurface.ExtGStateP1);
			this.CreateComplexSurfaceAlpha(writer, a2, cs.Id, TypeComplexSurface.ExtGStateP2);
		}

		protected void CreateComplexSurfaceAlpha(Writer writer, double alpha, int id, TypeComplexSurface type)
		{
			//	Crée un ExtGState pour une transparence unie.
			writer.WriteObjectDef(Export.NameComplexSurface(id, type));
			Port port = new Port();
			port.PutCommand("<< /CA ");  // voir [*] page 192
			port.PutValue(alpha, 3);
			port.PutCommand("/ca ");
			port.PutValue(alpha, 3);
			port.PutCommand(">> endobj");
			writer.WriteLine(port.GetPDF());
		}

		public static void SurfaceInflate(ComplexSurface cs, ref Rectangle bbox)
		{
			//	Engraisse une bbox en fonction d'une surface floue.
			if ( cs.Fill is Properties.Gradient )
			{
				Properties.Gradient surface = cs.Fill as Properties.Gradient;
				Properties.Line     stroke  = cs.Stroke;

				double width = 0.0;

				if ( stroke != null && surface != null )
				{
					width += surface.InflateBoundingBoxWidth();
					width += stroke.InflateBoundingBoxWidth();
					width *= stroke.InflateBoundingBoxFactor();
				}
				else if ( surface != null )
				{
					width += surface.InflateBoundingBoxWidth();
				}

				bbox.Inflate(width);
			}
		}

		protected void CreateComplexSurfaceTransparencyGradientOrSmooth(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface dégradée transparente et/ou floue.
			Objects.Abstract obj = cs.Object;
			SurfaceAnchor sa = obj.SurfaceAnchor;

			//	ExtGState.
			writer.WriteObjectDef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.ExtGState));
			writer.WriteString("<< /SMask << /S /Luminosity /G ");  // voir [*] page 521
			writer.WriteObjectRef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.XObject));
			writer.WriteLine(">> >> endobj");

			//	XObject.
			Rectangle bbox = sa.BoundingBox;
			Export.SurfaceInflate(cs, ref bbox);

			writer.WriteObjectDef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.XObject));
			writer.WriteString("<< /Subtype /Form /FormType 1 ");    // voir [*] page 328
			writer.WriteString(Port.StringBBox(bbox));

			writer.WriteString("/Resources << ");

			if ( cs.IsSmooth )
			{
				writer.WriteString("/ExtGState << ");
				writer.WriteString(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.ExtGStateSmooth));
				writer.WriteObjectRef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.ExtGStateSmooth));
				writer.WriteString(">> ");
			}

			writer.WriteString("/Shading << ");
			writer.WriteString(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.ShadingGray));
			writer.WriteObjectRef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.ShadingGray));
			writer.WriteString(">> ");

			writer.WriteString(">> ");
			writer.WriteString("/Group << /S /Transparency /CS /DeviceGray >> ");  // voir [*] page 525

			Path path = new Path();
			path.AppendRectangle(bbox);
			port.Reset();
			if ( cs.IsSmooth )
			{
				port.PutCommand("q ");
				port.PutCommand(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.ExtGStateSmooth));
				port.PutCommand("gs ");
				port.PutPath(path);
				port.PutCommand("W n ");
				port.PutTransform(cs.Matrix);
				port.PutCommand(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.ShadingGray));
				port.PutCommand("sh Q ");  // shading, voir [*] page 273
			}
			else
			{
				port.PutPath(path);
				port.PutCommand("q W n ");
				port.PutTransform(cs.Matrix);
				port.PutCommand(Export.ShortNameComplexSurface(cs.Id, TypeComplexSurface.ShadingGray));
				port.PutCommand("sh Q ");  // shading, voir [*] page 273
			}
			port.PutEOL();

			string pdf = port.GetPDF();
			writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} >>", Port.StringLength(pdf.Length)));
			writer.WriteLine("stream");
			writer.WriteString(pdf);
			writer.WriteLine("endstream endobj");

			this.CreateComplexSurfaceGradient(writer, port, drawingContext, cs, 0);

			if ( cs.Type == PDF.Type.OpaqueGradient       ||
				 cs.Type == PDF.Type.TransparencyGradient )
			{
				int nbColors = this.ComplexSurfaceGradientNbColors(port, drawingContext, cs);
				this.CreateComplexSurfaceGradient(writer, port, drawingContext, cs, nbColors);
			}

			if ( cs.IsSmooth )
			{
				this.CreateComplexSurfaceSmooth(writer, port, drawingContext, cs);
			}
		}

		protected void CreateComplexSurfaceSmooth(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface floue.
			Objects.Abstract obj = cs.Object;
			SurfaceAnchor sa = obj.SurfaceAnchor;

			Properties.Gradient surface = cs.Fill as Properties.Gradient;
			Properties.Line     stroke  = cs.Stroke;

			//	ExtGStateSmooth.
			writer.WriteObjectDef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.ExtGStateSmooth));
			writer.WriteString("<< /SMask << /S /Luminosity /G ");  // voir [*] page 521
			writer.WriteObjectRef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.XObjectSmooth));
			writer.WriteLine(">> >> endobj");

			//	XObjectSmooth.
			Rectangle bbox = sa.BoundingBox;
			Export.SurfaceInflate(cs, ref bbox);

			writer.WriteObjectDef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.XObjectSmooth));
			writer.WriteString("<< /Subtype /Form /FormType 1 ");    // voir [*] page 328
			writer.WriteString(Port.StringBBox(bbox));
			writer.WriteString("/Group << /S /Transparency /CS /DeviceGray >> ");  // voir [*] page 525

			double    width = 0.0;
			CapStyle  cap   = CapStyle.Round;
			JoinStyle join  = JoinStyle.Round;
			double    limit = 5.0;

			if ( stroke != null )
			{
				width = stroke.Width;
				cap   = stroke.Cap;
				join  = stroke.Join;
				limit = stroke.Limit;
			}

			port.Reset();
			port.ColorForce = ColorForce.Gray;
			port.LineCap = cap;
			port.LineJoin = join;
			port.LineMiterLimit = limit;

			Shape[] shapes = obj.ShapesBuild(port, drawingContext, false);

			int step = (int)(surface.Smooth*drawingContext.ScaleX*2);
			if ( step > 20 )  step = 20;
			if ( step <  2 )  step =  2;
			foreach ( Shape shape in shapes )
			{
				if ( shape.PropertySurface != surface )  continue;

				for ( int i=0 ; i<step ; i++ )
				{
					double intensity = (i+1.0)/step;
					double w = surface.Smooth-i*surface.Smooth/step;
					port.RichColor = RichColor.FromGray(intensity);
					port.LineWidth = width + w*2.0;
					port.PaintOutline(shape.Path);
				}

				if ( stroke == null )
				{
					port.RichColor = RichColor.FromGray(1.0);
					port.PaintSurface(shape.Path);
				}
			}

			string pdf = port.GetPDF();
			writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} >>", Port.StringLength(pdf.Length)));
			writer.WriteLine("stream");
			writer.WriteString(pdf);
			writer.WriteLine("endstream endobj");
		}

		protected void CreateComplexSurfaceOpaqueGradient(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface dégradée transparente.
			if ( cs.IsSmooth )
			{
				this.CreateComplexSurfaceTransparencyGradientOrSmooth(writer, port, drawingContext, cs);
			}
			else
			{
				int nbColors = this.ComplexSurfaceGradientNbColors(port, drawingContext, cs);
				this.CreateComplexSurfaceGradient(writer, port, drawingContext, cs, nbColors);
			}
		}

		protected void MatrixComplexSurfaceGradient(ComplexSurface cs)
		{
			//	Calcule la matrice de transformation pour une surface dégradée.
			Properties.Gradient gradient = cs.Fill as Properties.Gradient;
			if ( gradient == null )  return;

			Objects.Abstract obj = cs.Object;
			SurfaceAnchor sa = obj.SurfaceAnchor;
			Rectangle t = cs.Object.BoundingBox;  // update bounding box !
			Point center = sa.ToAbs(new Point(gradient.Cx, gradient.Cy));

			if ( gradient.FillType == Properties.GradientFillType.Linear )
			{
				double sx = sa.Width *gradient.Sx;
				double sy = sa.Height*gradient.Sy;
				double angle = Point.ComputeAngleDeg(sx, sy)-90.0;
				double length = System.Math.Sqrt(sx*sx + sy*sy);

				cs.Matrix.RotateDeg(obj.Direction+angle);
				cs.Matrix.Scale(length, length);
				cs.Matrix.Translate(center);
			}

			if ( gradient.FillType == Properties.GradientFillType.Circle )
			{
				double dx = System.Math.Abs(sa.Width *gradient.Sx);
				double dy = System.Math.Abs(sa.Height*gradient.Sy);

				cs.Matrix.RotateDeg(obj.Direction);
				cs.Matrix.Scale(dx, dy);
				cs.Matrix.Translate(center);
			}

			if ( gradient.FillType == Properties.GradientFillType.Diamond ||
				 gradient.FillType == Properties.GradientFillType.Conic   )
			{
				double dx = System.Math.Abs(sa.Width *gradient.Sx);
				double dy = System.Math.Abs(sa.Height*gradient.Sy);

				cs.Matrix.Scale(dx, dy);
				cs.Matrix.Translate(center);
				cs.Matrix.RotateDeg(sa.Direction+gradient.Angle, center);
			}
		}

		protected void CreateComplexSurfaceGradient(Writer writer,
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
			port.Reset();

			int totalColors = System.Math.Max(nbColors, 1);

			TypeComplexSurface tcs = (nbColors == 0) ? TypeComplexSurface.ShadingGray : TypeComplexSurface.ShadingColor;

			if ( gradient.FillType == Properties.GradientFillType.None )
			{
				writer.WriteObjectDef(Export.NameComplexSurface(cs.Id, tcs));

				//	Axial shading, voir [*] page 279
				port.PutCommand("<< /ShadingType 2 /Coords [0 -1 0 1] ");

				if ( nbColors <= 1 )  port.PutCommand("/ColorSpace /DeviceGray ");
				if ( nbColors == 3 )  port.PutCommand("/ColorSpace /DeviceRGB ");
				if ( nbColors == 4 )  port.PutCommand("/ColorSpace /DeviceCMYK ");

				port.PutCommand("/Function << /FunctionType 2 /Domain [0 1] /C0 [");
				RichColor c1 = port.GetFinalColor(gradient.Color1);
				RichColor c2 = port.GetFinalColor(gradient.Color2);
				if ( nbColors == 1 )  // gray ?
				{
					c1.ColorSpace = ColorSpace.Gray;
					c2.ColorSpace = ColorSpace.Gray;
				}
				if ( nbColors == 3 )  // rgb ?
				{
					c1.ColorSpace = ColorSpace.Rgb;
					c2.ColorSpace = ColorSpace.Rgb;
				}
				if ( nbColors == 4 )  // cmyk ?
				{
					c1.ColorSpace = ColorSpace.Cmyk;
					c2.ColorSpace = ColorSpace.Cmyk;
				}
				if ( nbColors == 0 )  port.PutValue(c1.A, 3);
				else                  port.PutColor(c1);
				port.PutCommand("] /C1 [");
				if ( nbColors == 0 )  port.PutValue(c1.A, 3);  // c1 to c1 (flat) !
				else                  port.PutColor(c1);
				port.PutCommand("] /N 1 >> ");

				port.PutCommand("/Extend [true true] >> endobj");
				writer.WriteLine(port.GetPDF());
			}

			if ( gradient.FillType == Properties.GradientFillType.Linear ||
				 gradient.FillType == Properties.GradientFillType.Circle )
			{
				if ( gradient.Middle != 0.0 || gradient.Repeat != 1 )
				{
					Export.FunctionColorSampled(writer, port, drawingContext, cs, nbColors);
				}

				writer.WriteObjectDef(Export.NameComplexSurface(cs.Id, tcs));

				if ( gradient.FillType == Properties.GradientFillType.Linear )
				{
					//	Axial shading, voir [*] page 279
					port.PutCommand("<< /ShadingType 2 /Coords [0 -1 0 1] ");
				}

				if ( gradient.FillType == Properties.GradientFillType.Circle )
				{
					//	Radial shading, voir [*] page 280
					port.PutCommand("<< /ShadingType 3 /Coords [0 0 0 0 0 1] ");
				}

				if ( nbColors <= 1 )  port.PutCommand("/ColorSpace /DeviceGray ");
				if ( nbColors == 3 )  port.PutCommand("/ColorSpace /DeviceRGB ");
				if ( nbColors == 4 )  port.PutCommand("/ColorSpace /DeviceCMYK ");

				if ( gradient.Middle == 0.0 && gradient.Repeat == 1 )
				{
					//	Exponential interpolation function, voir [*] page 146
					port.PutCommand("/Function << /FunctionType 2 /Domain [0 1] /C0 [");
					RichColor c1 = port.GetFinalColor(gradient.Color1);
					RichColor c2 = port.GetFinalColor(gradient.Color2);
					if ( nbColors == 1 )  // gray ?
					{
						c1.ColorSpace = ColorSpace.Gray;
						c2.ColorSpace = ColorSpace.Gray;
					}
					if ( nbColors == 3 )  // rgb ?
					{
						c1.ColorSpace = ColorSpace.Rgb;
						c2.ColorSpace = ColorSpace.Rgb;
					}
					if ( nbColors == 4 )  // cmyk ?
					{
						c1.ColorSpace = ColorSpace.Cmyk;
						c2.ColorSpace = ColorSpace.Cmyk;
					}
					if ( nbColors == 0 )  port.PutValue(c1.A, 3);
					else                  port.PutColor(c1);
					port.PutCommand("] /C1 [");
					if ( nbColors == 0 )  port.PutValue(c2.A, 3);
					else                  port.PutColor(c2);
					port.PutCommand("] /N 1 >> ");
				}
				else
				{
					port.PutCommand("/Function ");
					writer.WriteString(port.GetPDF());
					writer.WriteObjectRef(Export.NameFunction(cs.Id, (nbColors == 0) ? TypeFunction.SampledAlpha : TypeFunction.SampledColor));
					port.Reset();
				}

				port.PutCommand("/Extend [true true] >> endobj");
				writer.WriteLine(port.GetPDF());
			}

			if ( gradient.FillType == Properties.GradientFillType.Diamond ||
				 gradient.FillType == Properties.GradientFillType.Conic   )
			{
				//	Calcule le domaine (pfff...).
				double w = sa.Width;
				double h = sa.Height;
				Point p1 = new Point((0.0-gradient.Cx)*w, (0.0-gradient.Cy)*h);
				Point p2 = new Point((1.0-gradient.Cx)*w, (0.0-gradient.Cy)*h);
				Point p3 = new Point((0.0-gradient.Cx)*w, (1.0-gradient.Cy)*h);
				Point p4 = new Point((1.0-gradient.Cx)*w, (1.0-gradient.Cy)*h);
				p1 = Transform.RotatePointDeg(-gradient.Angle, p1);
				p2 = Transform.RotatePointDeg(-gradient.Angle, p2);
				p3 = Transform.RotatePointDeg(-gradient.Angle, p3);
				p4 = Transform.RotatePointDeg(-gradient.Angle, p4);
				double domainMinX = System.Math.Min(System.Math.Min(p1.X, p2.X), System.Math.Min(p3.X, p4.X));
				double domainMaxX = System.Math.Max(System.Math.Max(p1.X, p2.X), System.Math.Max(p3.X, p4.X));
				double domainMinY = System.Math.Min(System.Math.Min(p1.Y, p2.Y), System.Math.Min(p3.Y, p4.Y));
				double domainMaxY = System.Math.Max(System.Math.Max(p1.Y, p2.Y), System.Math.Max(p3.Y, p4.Y));
				domainMinX /= w*System.Math.Abs(gradient.Sx);
				domainMaxX /= w*System.Math.Abs(gradient.Sx);
				domainMinY /= h*System.Math.Abs(gradient.Sy);
				domainMaxY /= h*System.Math.Abs(gradient.Sy);
				Rectangle bbox = new Rectangle(0.0, 0.0, w, h);
				Export.SurfaceInflate(cs, ref bbox);
				double fx = bbox.Width /w;
				double fy = bbox.Height/h;
				double f = System.Math.Max(fx, fy);
				domainMinX *= f;
				domainMaxX *= f;
				domainMinY *= f;
				domainMaxY *= f;

				port.PutCommand("/Domain [");
				port.PutValue(domainMinX, 3);
				port.PutValue(domainMaxX, 3);
				port.PutValue(domainMinY, 3);
				port.PutValue(domainMaxY, 3);
				port.PutCommand("]");
				string domain = port.GetPDF();
				port.Reset();

				writer.WriteObjectDef(Export.NameComplexSurface(cs.Id, tcs));

				//	Function-based shading, voir [*] page 178
				port.PutCommand("<< /ShadingType 1 ");
				if ( nbColors <= 1 )  port.PutCommand("/ColorSpace /DeviceGray ");
				if ( nbColors == 3 )  port.PutCommand("/ColorSpace /DeviceRGB ");
				if ( nbColors == 4 )  port.PutCommand("/ColorSpace /DeviceCMYK ");
				port.PutCommand(domain);
				port.PutCommand(" /Function [");
				writer.WriteString(port.GetPDF());
				if ( nbColors == 0 )  // alpha ?
				{
					writer.WriteObjectRef(Export.NameFunction(cs.Id, TypeFunction.Alpha));
				}
				if ( nbColors == 1 )  // gray ?
				{
					writer.WriteObjectRef(Export.NameFunction(cs.Id, TypeFunction.Color1));
				}
				if ( nbColors == 3 )  // rgb ?
				{
					writer.WriteObjectRef(Export.NameFunction(cs.Id, TypeFunction.Color1));
					writer.WriteObjectRef(Export.NameFunction(cs.Id, TypeFunction.Color2));
					writer.WriteObjectRef(Export.NameFunction(cs.Id, TypeFunction.Color3));
				}
				if ( nbColors == 4 )  // cmyk ?
				{
					writer.WriteObjectRef(Export.NameFunction(cs.Id, TypeFunction.Color1));
					writer.WriteObjectRef(Export.NameFunction(cs.Id, TypeFunction.Color2));
					writer.WriteObjectRef(Export.NameFunction(cs.Id, TypeFunction.Color3));
					writer.WriteObjectRef(Export.NameFunction(cs.Id, TypeFunction.Color4));
				}
				writer.WriteLine("] >> endobj");

				//	Génère les fonctions pour les couleurs.
				string[] fonctions = new string[totalColors];
				RichColor c1 = port.GetFinalColor(gradient.Color1);
				RichColor c2 = port.GetFinalColor(gradient.Color2);
				if ( gradient.FillType == Properties.GradientFillType.Diamond )
				{
					if ( nbColors == 0 )  // alpha ?
					{
						fonctions[0] = Export.FunctionGradientDiamond(gradient, c1.A, c2.A);
					}
					if ( nbColors == 1 )  // gray ?
					{
						fonctions[0] = Export.FunctionGradientDiamond(gradient, c1.Gray, c2.Gray);
					}
					if ( nbColors == 3 )  // rgb ?
					{
						fonctions[0] = Export.FunctionGradientDiamond(gradient, c1.R, c2.R);
						fonctions[1] = Export.FunctionGradientDiamond(gradient, c1.G, c2.G);
						fonctions[2] = Export.FunctionGradientDiamond(gradient, c1.B, c2.B);
					}
					if ( nbColors == 4 )  // cmyk ?
					{
						fonctions[0] = Export.FunctionGradientDiamond(gradient, c1.C, c2.C);
						fonctions[1] = Export.FunctionGradientDiamond(gradient, c1.M, c2.M);
						fonctions[2] = Export.FunctionGradientDiamond(gradient, c1.Y, c2.Y);
						fonctions[3] = Export.FunctionGradientDiamond(gradient, c1.K, c2.K);
					}
				}
				if ( gradient.FillType == Properties.GradientFillType.Conic )
				{
					if ( nbColors == 0 )  // alpha ?
					{
						fonctions[0] = Export.FunctionGradientConic(gradient, c1.A, c2.A);
					}
					if ( nbColors == 1 )  // gray ?
					{
						fonctions[0] = Export.FunctionGradientConic(gradient, c1.Gray, c2.Gray);
					}
					if ( nbColors == 3 )  // rgb ?
					{
						fonctions[0] = Export.FunctionGradientConic(gradient, c1.R, c2.R);
						fonctions[1] = Export.FunctionGradientConic(gradient, c1.G, c2.G);
						fonctions[2] = Export.FunctionGradientConic(gradient, c1.B, c2.B);
					}
					if ( nbColors == 4 )  // cmyk ?
					{
						fonctions[0] = Export.FunctionGradientConic(gradient, c1.C, c2.C);
						fonctions[1] = Export.FunctionGradientConic(gradient, c1.M, c2.M);
						fonctions[2] = Export.FunctionGradientConic(gradient, c1.Y, c2.Y);
						fonctions[3] = Export.FunctionGradientConic(gradient, c1.K, c2.K);
					}
				}

				for ( int i=0 ; i<totalColors ; i++ )
				{
					TypeFunction ft = TypeFunction.Alpha;
					if ( nbColors != 0 )  // gray, rgb ou cmyk ?
					{
						switch ( i )
						{
							case 0:  ft = TypeFunction.Color1;  break;
							case 1:  ft = TypeFunction.Color2;  break;
							case 2:  ft = TypeFunction.Color3;  break;
							case 3:  ft = TypeFunction.Color4;  break;
						}
					}
					writer.WriteObjectDef(Export.NameFunction(cs.Id, ft));
					//	PostScript calculator function, voir [*] page 148
					writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "<< /FunctionType 4 /Range [0 1] {0} {1} >>", domain, Port.StringLength(fonctions[i].Length)));
					writer.WriteLine("stream");
					writer.WriteLine(fonctions[i]);
					writer.WriteLine("endstream endobj");
				}
			}
		}

		protected static void FunctionColorSampled(Writer writer,
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
			RichColor c1 = port.GetFinalColor(gradient.Color1);
			RichColor c2 = port.GetFinalColor(gradient.Color2);

			int totalColors = System.Math.Max(nbColors, 1);

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			for ( int i=0 ; i<256 ; i++ )
			{
				double factor = gradient.GetProgressColorFactor(i/255.0);

				if ( nbColors == 0 )  // alpha ?
				{
					int a = (int) ((c1.A + (c2.A-c1.A)*factor) * 255.0);
					builder.Append(a.ToString("X2"));
				}

				if ( nbColors == 1 )  // gray ?
				{
					int a = (int) ((c1.Gray + (c2.Gray-c1.Gray)*factor) * 255.0);
					builder.Append(a.ToString("X2"));
				}

				if ( nbColors == 3 )  // rgb ?
				{
					int r = (int) ((c1.R + (c2.R-c1.R)*factor) * 255.0);
					int g = (int) ((c1.G + (c2.G-c1.G)*factor) * 255.0);
					int b = (int) ((c1.B + (c2.B-c1.B)*factor) * 255.0);
					builder.Append(r.ToString("X2"));
					builder.Append(g.ToString("X2"));
					builder.Append(b.ToString("X2"));
				}

				if ( nbColors == 4 )  // cmyk ?
				{
					int c = (int) ((c1.C + (c2.C-c1.C)*factor) * 255.0);
					int m = (int) ((c1.M + (c2.M-c1.M)*factor) * 255.0);
					int y = (int) ((c1.Y + (c2.Y-c1.Y)*factor) * 255.0);
					int k = (int) ((c1.K + (c2.K-c1.K)*factor) * 255.0);
					builder.Append(c.ToString("X2"));
					builder.Append(m.ToString("X2"));
					builder.Append(y.ToString("X2"));
					builder.Append(k.ToString("X2"));
				}
			}
			string table = builder.ToString();

			builder = new System.Text.StringBuilder();
			for ( int i=0 ; i<totalColors ; i++ )
			{
				builder.Append("0 1 ");
			}
			string range = builder.ToString();

			writer.WriteObjectDef(Export.NameFunction(cs.Id, (nbColors == 0) ? TypeFunction.SampledAlpha : TypeFunction.SampledColor));
			writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "<< /FunctionType 0 /Domain [0 1] /Range [{0}] /Size [256] /BitsPerSample 8 /Filter /ASCIIHexDecode {1} >>", range, Port.StringLength(table.Length)));
			writer.WriteLine("stream");
			writer.WriteLine(table);
			writer.WriteLine("endstream endobj");
		}

		protected static string FunctionGradientDiamond(Properties.Gradient gradient,
														double colorStart,
														double colorEnd)
		{
			//	Retourne la fonction permettant de calculer une couleur fondamentale
			//	d'un dégradé en diamant. En entrés, la pile contient x et y [-1..1].
			//	En sortie, elle contient la couleur [0..1]. Avec une répétition de 1,
			//	la fonction est la suivante:
			//	f(c) = max(|x|,|y|)
			//	PostScript calculator function, voir [*] page 148
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("{ ");

			if ( colorStart == colorEnd )
			{
				builder.Append("pop pop ");
				builder.Append(Port.StringValue(colorStart, 3));
			}
			else
			{
				builder.Append("abs exch abs 2 copy lt { exch } if pop ");
				if ( gradient.Repeat > 1 )
				{
					builder.Append("dup 1 gt { pop 1 } if ");
					builder.Append(Port.StringValue(gradient.Repeat, 3));
					builder.Append(" mul dup dup truncate sub exch cvi 2 mod 1 eq { 1 exch sub } if ");
				}

				Export.FunctionMiddleAdjust(builder, gradient);
				Export.FunctionColorAdjust(builder, colorStart, colorEnd);
			}

			builder.Append("}");
			return builder.ToString();
		}

		protected static string FunctionGradientConic(Properties.Gradient gradient,
													  double colorStart,
													  double colorEnd)
		{
			//	Retourne la fonction permettant de calculer une couleur fondamentale
			//	d'un dégradé conique. En entrés, la pile contient x et y [-1..1].
			//	En sortie, elle contient la couleur [0..1]. Avec une répétition de 1,
			//	la fonction est la suivante:
			//	f(c) = (360-atan(x,y))/360
			//	PostScript calculator function, voir [*] page 148
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("{ ");

			if ( colorStart == colorEnd )
			{
				builder.Append("pop pop ");
				builder.Append(Port.StringValue(colorStart, 3));
			}
			else
			{
				if ( gradient.Sx < 0.0 )  // miroir X ?
				{
					builder.Append("exch neg exch ");  // [-1..1] -> [1..-1]
				}

				if ( gradient.Sy < 0.0 )  // miroir Y ?
				{
					builder.Append("neg ");  // [-1..1] -> [1..-1]
				}

				builder.Append("atan 360 exch sub ");

				if ( gradient.Repeat == 1 )
				{
					builder.Append("360 div ");
				}
				else
				{
					builder.Append(Port.StringValue(360.0/gradient.Repeat, 3));
					builder.Append(" div dup exch dup truncate sub exch cvi 2 mod 1 eq { 1 exch sub } if ");
				}

				Export.FunctionMiddleAdjust(builder, gradient);
				Export.FunctionColorAdjust(builder, colorStart, colorEnd);
			}

			builder.Append("}");
			return builder.ToString();
		}

		protected static void FunctionMiddleAdjust(System.Text.StringBuilder builder, Properties.Gradient gradient)
		{
			//	Ajoute une modification du nombre [0..1] sur la pile pour tenir compte
			//	du point milieu (voir Properties.Gradient.GetProgressColorFactor).
			//	Si M>0:  P=1-(1-P)^(1+M)
			//	Si M<0:  P=P^(1-M)
			if ( gradient.Middle > 0.0 )
			{
				builder.Append("1 exch sub ");
				builder.Append(Port.StringValue(1.0+gradient.Middle, 3));
				builder.Append(" exp 1 exch sub ");
			}

			if ( gradient.Middle < 0.0 )
			{
				builder.Append(Port.StringValue(1.0-gradient.Middle, 3));
				builder.Append(" exp ");
			}
		}

		protected static void FunctionColorAdjust(System.Text.StringBuilder builder, double colorStart, double colorEnd)
		{
			//	Ajoute une modification du nombre [0..1] sur la pile pour tenir compte
			//	de la couleur.
			if ( colorEnd-colorStart != 1.0 )
			{
				builder.Append(Port.StringValue(colorEnd-colorStart, 3));
				builder.Append(" mul ");
			}
			if ( colorStart != 0.0 )
			{
				builder.Append(Port.StringValue(colorStart, 3));
				builder.Append(" add ");
			}
		}

		protected void CreateComplexSurfaceOpaquePattern(Writer writer, Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Crée une surface opaque contenant un motif.
			Properties.Gradient surface = cs.Fill as Properties.Gradient;
			Objects.Abstract obj = cs.Object;
			SurfaceAnchor sa = obj.SurfaceAnchor;
			Rectangle bbox = cs.Object.BoundingBox;  // update bounding box !

			port.Reset();

			Path path = new Path();
			path.AppendRectangle(bbox);

			Drawer.RenderMotif(port, drawingContext, obj, path, sa, surface);

			writer.WriteObjectDef(Export.NameComplexSurface(cs.Id, TypeComplexSurface.XObject));
			writer.WriteString("<< /Subtype /Form ");  // voir [*] page 328
			writer.WriteString(Port.StringBBox(bbox));

			string pdf = port.GetPDF();
			writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} >>", Port.StringLength(pdf.Length)));
			writer.WriteLine("stream");
			writer.WriteString(pdf);
			writer.WriteLine("endstream endobj");
		}

		protected int ComplexSurfaceGradientNbColors(Port port, DrawingContext drawingContext, ComplexSurface cs)
		{
			//	Détermine le nombre de couleurs (1, 3 ou 4) à utiliser pour une surface
			//	complexe. C'est la première couleur qui fait foi. La deuxième sera
			//	convertie dans l'espace de couleur de la première, au besoin.
			Properties.Gradient gradient = cs.Fill as Properties.Gradient;
			RichColor color = port.GetFinalColor(gradient.Color1);

			if ( color.ColorSpace == ColorSpace.Gray )
			{
				color = port.GetFinalColor(gradient.Color2);
			}

			if ( color.ColorSpace == ColorSpace.Cmyk )  return 4;
			if ( color.ColorSpace == ColorSpace.Gray )  return 1;
			return 3;
		}

		protected void FlushComplexSurface()
		{
			//	Libère toutes les surfaces complexes.
			foreach ( ComplexSurface cs in this.complexSurfaces )
			{
				cs.Dispose();
			}
			this.complexSurfaces.Clear();
			this.complexSurfaces = null;
		}
		#endregion


		#region Images
		protected void CreateImageSurface(Writer writer, Port port)
		{
			//	Crée toutes les images.
			foreach ( ImageSurface image in this.imageSurfaces )
			{
				if ( image.DrawingImage == null )  continue;

				if ( this.CreateImageSurface(writer, port, image, TypeComplexSurface.XObject, TypeComplexSurface.XObjectMask) )
				{
					this.CreateImageSurface(writer, port, image, TypeComplexSurface.XObjectMask, TypeComplexSurface.None);
				}
			}
		}

		protected bool CreateImageSurface(Writer writer, Port port, ImageSurface image,
										  TypeComplexSurface baseType, TypeComplexSurface maskType)
		{
			//	Crée une image.
			//	Création d'une instance de Magick.Image à partir du nom de fichier.
			byte[] imageData = image.Cache.Data;
			
			Magick.Blob blob = new Magick.Blob();
			blob.Update(imageData);
			
			Magick.Image magick = new Magick.Image(blob);
			
			blob.Dispose();
			blob = null;
			imageData = null;

			int dx, dy;

			Margins crop = image.Crop;
			if (crop != Margins.Zero)  // recadrage nécessaire ?
			{
				dx = magick.Width;
				dy = magick.Height;
				dx -= (int) crop.Left;
				dx -= (int) crop.Right;
				dy -= (int) crop.Bottom;
				dy -= (int) crop.Top;

				byte[] buffer = new byte[dx*dy*4];

				//	Attention, avec Magick, y=0 est en haut, d'où crop.Top !
				magick.ModifyBegin();
				magick.GetRawPixels(buffer, (int) crop.Left, (int) crop.Top, dx, dy);
				magick.ModifyEnd();

				Magick.Image cropped = new Magick.Image(dx, dy);
				cropped.ModifyBegin();
				cropped.SetRawPixels(buffer, 0, 0, dx, dy);
				cropped.ModifyEnd();

				magick.Dispose();
				magick = cropped;  // remplace par l'instance recadrée
			}

			bool useMask = false;

			//	Mise à l'échelle éventuelle de l'image selon les choix de l'utilisateur.
			//	Une image sans filtrage n'est jamais mise à l'échelle !
			double currentDpiX = magick.Width *254.0/image.Size.Width;
			double currentDpiY = magick.Height*254.0/image.Size.Height;

			double finalDpiX = currentDpiX;
			double finalDpiY = currentDpiY;

			if ( this.imageMinDpi != 0.0 && image.Filter.Active )
			{
				finalDpiX = System.Math.Max(finalDpiX, this.imageMinDpi);
				finalDpiY = System.Math.Max(finalDpiY, this.imageMinDpi);
			}

			if ( this.imageMaxDpi != 0.0 && image.Filter.Active )
			{
				finalDpiX = System.Math.Min(finalDpiX, this.imageMaxDpi);
				finalDpiY = System.Math.Min(finalDpiY, this.imageMaxDpi);
			}

			dx = magick.Width;
			dy = magick.Height;
			bool resizeRequired = false;

			if ( currentDpiX != finalDpiX || currentDpiY != finalDpiY )
			{
				dx = (int) ((dx+0.5)*finalDpiX/currentDpiX);
				dy = (int) ((dy+0.5)*finalDpiY/currentDpiY);
				resizeRequired = true;
			}

			//	Choix du mode de compression possible.
			ImageCompression compression = this.imageCompression;

			if ( baseType == TypeComplexSurface.XObject &&
				 this.colorConversion == PDF.ColorConversion.ToCmyk &&
				 compression == ImageCompression.JPEG )  // cmyk impossible en jpg !
			{
				compression = ImageCompression.ZIP;  // utilise la compression sans pertes
			}

			//	Génération de l'en-tête.
			writer.WriteObjectDef(Export.NameComplexSurface(image.Id, baseType));
			writer.WriteString("<< /Subtype /Image ");

			if ( baseType == TypeComplexSurface.XObject )
			{
				if ( this.colorConversion == PDF.ColorConversion.ToGray )
				{
					writer.WriteString("/ColorSpace /DeviceGray ");
				}
				else if ( this.colorConversion == PDF.ColorConversion.ToCmyk )
				{
					writer.WriteString("/ColorSpace /DeviceCMYK ");
				}
				else
				{
					switch ( magick.ColorSpace )
					{
						case Magick.ColorSpace.Gray:
							writer.WriteString("/ColorSpace /DeviceGray ");
							break;
						
						case Magick.ColorSpace.Rgb:
							writer.WriteString("/ColorSpace /DeviceRGB ");
							break;
						
						case Magick.ColorSpace.Cmyk:
							if ( compression == ImageCompression.JPEG )
							{
								writer.WriteString("/ColorSpace /DeviceRGB ");
							}
							else
							{
								writer.WriteString("/ColorSpace /DeviceCMYK ");
							}
							break;
						
						default:
							throw new System.InvalidOperationException();
					}
				}
			}
			if ( baseType == TypeComplexSurface.XObjectMask )
			{
				writer.WriteString("/ColorSpace /DeviceGray ");
			}

			writer.WriteString("/BitsPerComponent 8 /Width ");  // voir [*] page 310
			writer.WriteString(Port.StringValue(dx, 0));
			writer.WriteString(" /Height ");
			writer.WriteString(Port.StringValue(dy, 0));
			writer.WriteString(" ");

			if ( image.Filter.Active )
			{
				writer.WriteString("/Interpolate true ");
			}

			if ( magick.ImageType == Magick.ImageType.PaletteMatte         ||
				 magick.ImageType == Magick.ImageType.TrueColorMatte       ||
				 magick.ImageType == Magick.ImageType.ColorSeparationMatte ||
				 magick.ImageType == Magick.ImageType.GrayscaleMatte       )
			{
				useMask = true;
			}
			
			if ( compression == ImageCompression.JPEG )  // compression JPEG ?
			{
				writer.WriteString("/Filter [/ASCII85Decode /DCTDecode] ");  // voir [*] page 43

				bool isGray = false;
				if ( this.colorConversion == PDF.ColorConversion.ToGray )	isGray = true;
				if ( baseType == TypeComplexSurface.XObjectMask )			isGray = true;
				if ( magick.ImageType == Magick.ImageType.Grayscale )		isGray = true;
				if ( magick.ImageType == Magick.ImageType.GrayscaleMatte )  isGray = true;
				
				Magick.Image copy = new Magick.Image(magick);
				
				if ( baseType == TypeComplexSurface.XObjectMask )
				{
					copy.SelectChannel(Magick.Channel.Alpha);
					
					copy.Depth = 8;
					copy.ImageType = Magick.ImageType.TrueColor;
					copy.ColorSpace = Magick.ColorSpace.Rgb;

					int width  = copy.Width;
					int height = copy.Height;
					byte[] buffer = new byte[width*height*4];

					copy.ModifyBegin();
					copy.GetRawPixels(buffer, 0, 0, width, height);
					
					for ( int i=0 ; i<buffer.Length ; i++ )
					{
						buffer[i] = (byte) (0xff - buffer[i]);
					}
					
					copy.SetRawPixels(buffer, 0, 0, width, height);
					copy.ModifyEnd();
				}
				
				copy.CompressionType = Magick.Compression.Jpeg;
				copy.ColorSpace      = isGray ? Magick.ColorSpace.Gray : Magick.ColorSpace.Rgb;
				copy.ImageType       = isGray ? Magick.ImageType.Grayscale : Magick.ImageType.TrueColor;
				copy.MagickFormat    = "JPEG";
				copy.Quality         = (int) (this.jpegQuality*100.0);
				
				if ( resizeRequired )
				{
					copy.Zoom(dx, dy, Magick.FilterType.Cubic);
				}

				blob = new Magick.Blob();
				copy.SaveToBlob(blob);
				byte[] jpeg = blob.GetData();
				blob.Dispose();
				blob = null;

				port.Reset();
				port.PutASCII85(jpeg);
				port.PutEOL();
			}
			else	// compression ZIP ou aucune ?
			{
				if ( compression == ImageCompression.ZIP )  // compression ZIP ?
				{
					writer.WriteString("/Filter [/ASCII85Decode /FlateDecode] ");  // voir [*] page 43
				}
				else
				{
					writer.WriteString("/Filter /ASCII85Decode ");  // voir [*] page 43
				}

				magick.CompressionType = Magick.Compression.No;

				int bpp = 3;
				if ( baseType == TypeComplexSurface.XObject )
				{
					if ( this.colorConversion == PDF.ColorConversion.ToGray )
					{
						bpp = 1;
					}
					else if ( this.colorConversion == PDF.ColorConversion.ToRgb )
					{
						bpp = 3;
					}
					else if ( this.colorConversion == PDF.ColorConversion.ToCmyk )
					{
						bpp = 4;
					}
					else
					{
						if ( magick.ColorSpace == Magick.ColorSpace.Gray )  bpp = 1;
						if ( magick.ColorSpace == Magick.ColorSpace.Rgb  )  bpp = 3;
						if ( magick.ColorSpace == Magick.ColorSpace.Cmyk )  bpp = 4;
					}
				}
				else
				{
					bpp = -1;
				}

				byte[] data = null;

				if ( bpp == -1 )  // alpha ?
				{
					byte[] bufferAlpha = this.CreateImageSurfaceChannel(magick, dx, dy, Magick.Channel.Alpha);

					data = new byte[dx*dy];
					for ( int i=0 ; i<dx*dy ; i++ )
					{
						data[i] = (byte) (0xff - bufferAlpha[i*4+0]);
					}
				}

				if ( bpp == 1 )
				{
					magick.ImageType = Magick.ImageType.Grayscale;
					
					byte[] bufferGray = this.CreateImageSurfaceChannel(magick, dx, dy, Magick.Channel.Gray);

					data = new byte[dx*dy];
					for ( int i=0 ; i<dx*dy ; i++ )
					{
						data[i] = bufferGray[i*4+0];
					}
				}

				if ( bpp == 3 )
				{
					magick.ImageType = Magick.ImageType.TrueColor;
					
					byte[] bufferRed   = this.CreateImageSurfaceChannel(magick, dx, dy, Magick.Channel.Red);
					byte[] bufferGreen = this.CreateImageSurfaceChannel(magick, dx, dy, Magick.Channel.Green);
					byte[] bufferBlue  = this.CreateImageSurfaceChannel(magick, dx, dy, Magick.Channel.Blue);

					data = new byte[dx*dy*3];
					for ( int i=0 ; i<dx*dy ; i++ )
					{
						data[i*3+0] = bufferRed[i*4+0];
						data[i*3+1] = bufferGreen[i*4+0];
						data[i*3+2] = bufferBlue[i*4+0];
					}
				}

				if ( bpp == 4 )
				{
					magick.ImageType = Magick.ImageType.ColorSeparation;
					
					byte[] bufferCyan    = this.CreateImageSurfaceChannel(magick, dx, dy, Magick.Channel.Cyan);
					byte[] bufferMagenta = this.CreateImageSurfaceChannel(magick, dx, dy, Magick.Channel.Magenta);
					byte[] bufferYellow  = this.CreateImageSurfaceChannel(magick, dx, dy, Magick.Channel.Yellow);
					byte[] bufferBlack   = this.CreateImageSurfaceChannel(magick, dx, dy, Magick.Channel.Black);

					data = new byte[dx*dy*4];
					for ( int i=0 ; i<dx*dy ; i++ )
					{
						data[i*4+0] = bufferCyan[i*4+0];
						data[i*4+1] = bufferMagenta[i*4+0];
						data[i*4+2] = bufferYellow[i*4+0];
						data[i*4+3] = bufferBlack[i*4+0];
					}
				}

				if ( compression == ImageCompression.ZIP )  // compression ZIP ?
				{
					byte[] zip = Common.IO.DeflateCompressor.Compress(data, 9);  // 9 = compression forte mais lente
					data = zip;
					zip = null;
				}

				port.Reset();
				port.PutASCII85(data);
				port.PutEOL();
			}

			if ( maskType == TypeComplexSurface.XObjectMask && useMask )
			{
				writer.WriteString("/SMask ");
				writer.WriteObjectRef(Export.NameComplexSurface(image.Id, maskType));
			}

			string pdf = port.GetPDF();
			writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, " {0} >>", Port.StringLength(pdf.Length)));
			writer.WriteLine("stream");
			writer.WriteString(pdf);
			writer.WriteLine("endstream endobj");

			return useMask;
		}

		protected byte[] CreateImageSurfaceChannel(Magick.Image magick, int dx, int dy, Magick.Channel channel)
		{
			Magick.Image copy = new Magick.Image(magick);
			copy.SelectChannel(channel);
			
			copy.Depth      = 8;
			copy.ImageType  = Magick.ImageType.TrueColor;
			copy.ColorSpace = Magick.ColorSpace.Rgb;

			if ( dx != magick.Width || dy != magick.Height )
			{
				copy.Zoom(dx, dy, Magick.FilterType.Cubic);
			}

			byte[] buffer = new byte[dx*dy*4];

			copy.ModifyBegin();
			copy.GetRawPixels(buffer, 0, 0, dx, dy);
			copy.ModifyEnd();

			return buffer;
		}

		protected void FlushImageSurface()
		{
			//	Libère toutes les images.
			foreach ( ImageSurface image in this.imageSurfaces )
			{
				image.Dispose();
			}
			this.imageSurfaces.Clear();
			this.imageSurfaces = null;
		}
		#endregion


		#region Fonts
		protected void CreateFont(Writer writer)
		{
			//	Crée toutes les fontes.
			foreach ( System.Collections.DictionaryEntry dict in this.fontList )
			{
				FontList font = dict.Key as FontList;

				int totalPages = (font.CharacterCount+Export.charPerFont-1)/Export.charPerFont;
				System.Diagnostics.Debug.Assert(totalPages <= 10);  // voir Export.NameFont
				for ( int fontPage=0 ; fontPage<totalPages ; fontPage++ )
				{
					int count = Export.charPerFont;
					if ( fontPage == totalPages-1 )
					{
						count = font.CharacterCount%Export.charPerFont;
						if ( count == 0 )  count = Export.charPerFont;
					}

					this.CreateFontBase(writer, font, fontPage, count);
					this.CreateFontEncoding(writer, font, fontPage, count);
					this.CreateFontCharProcs(writer, font, fontPage, count);
					this.CreateFontWidths(writer, font, fontPage, count);
					this.CreateFontToUnicode(writer, font, fontPage, count);
					this.CreateFontCharacters(writer, font, fontPage, count);
				}
			}
		}

		protected void CreateFontBase(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet de base d'une fonte.
			Rectangle bbox = font.CharacterBBox;

			writer.WriteObjectDef(Export.NameFont(font.Id, fontPage, TypeFont.Base));
			writer.WriteString("<< /Type /Font /Subtype /Type3 ");  // voir [*] page 394
			writer.WriteString(string.Format(System.Globalization.CultureInfo.InvariantCulture, "/FirstChar 0 /LastChar {0} ", count-1));
			writer.WriteString(Port.StringBBox("/FontBBox", bbox));
			writer.WriteString("/FontMatrix [1 0 0 1 0 0] ");

			writer.WriteString("/Encoding ");
			writer.WriteObjectRef(Export.NameFont(font.Id, fontPage, TypeFont.Encoding));

			writer.WriteString("/CharProcs ");
			writer.WriteObjectRef(Export.NameFont(font.Id, fontPage, TypeFont.CharProcs));

			writer.WriteString("/Widths ");
			writer.WriteObjectRef(Export.NameFont(font.Id, fontPage, TypeFont.Widths));

			writer.WriteString("/ToUnicode ");
			writer.WriteObjectRef(Export.NameFont(font.Id, fontPage, TypeFont.ToUnicode));

			writer.WriteLine(">> endobj");
		}
		
		protected void CreateFontEncoding(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet Encoding d'une fonte.
			writer.WriteObjectDef(Export.NameFont(font.Id, fontPage, TypeFont.Encoding));

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("<< /Differences [ 0 ");
			
			int firstChar = fontPage*Export.charPerFont;
			for ( int i=0 ; i<count ; i++ )
			{
				CharacterList cl = font.GetCharacter(firstChar+i);
				builder.Append(Export.ShortNameCharacter(font.Id, fontPage, cl));
			}

			builder.Append("] >> endobj");
			writer.WriteLine(builder.ToString());
		}
		
		protected void CreateFontCharProcs(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet CharProcs d'une fonte.
			writer.WriteObjectDef(Export.NameFont(font.Id, fontPage, TypeFont.CharProcs));
			writer.WriteString("<< ");

			int firstChar = fontPage*Export.charPerFont;
			for ( int i=0 ; i<count ; i++ )
			{
				CharacterList cl = font.GetCharacter(firstChar+i);
				writer.WriteString(Export.ShortNameCharacter(font.Id, fontPage, cl));
				writer.WriteObjectRef(Export.NameCharacter(font.Id, fontPage, cl));
			}

			writer.WriteLine(">> endobj");
		}
		
		protected void CreateFontWidths(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet "table des chasses" d'une fonte.
			writer.WriteObjectDef(Export.NameFont(font.Id, fontPage, TypeFont.Widths));

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("[");

			int firstChar = fontPage*Export.charPerFont;
			for ( int i=0 ; i<count ; i++ )
			{
				CharacterList cl = font.GetCharacter(firstChar+i);
				double advance = cl.Width;
				builder.Append(Port.StringValue(advance, 4));
				builder.Append(" ");
			}
			builder.Append("] endobj");
			writer.WriteLine(builder.ToString());
		}
		
		protected void CreateFontToUnicode(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée l'objet ToUnicode d'une fonte, qui permet de retrouver les codes
			//	des caractères lors d'une copie depuis Acrobat dans le clipboard.
			//	Voir [*] pages 420 à 446.
			writer.WriteObjectDef(Export.NameFont(font.Id, fontPage, TypeFont.ToUnicode));

			string fontName = font.DrawingFont.FaceName;
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append("/CIDInit /ProcSet findresource begin ");
			builder.Append("12 dict begin begincmap ");
			builder.Append("/CIDSystemInfo << /Registry (Epsitec) /Ordering (Identity-H) /Supplement 0 >> def ");
			builder.Append("/CMapName /Epsitec def ");
			builder.Append("1 begincodespacerange <00> <FF> endcodespacerange ");

			int firstChar = fontPage*Export.charPerFont;
			builder.Append(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} beginbfrange ", count));
			for ( int i=0 ; i<count ; i++ )
			{
				CharacterList cl = font.GetCharacter(firstChar+i);
				string hex1 = i.ToString("X2");
				string hex2 = "";
				if ( cl.Unicodes == null )
				{
					hex2 = (cl.Unicode).ToString("X4");
				}
				else
				{
					for ( int u=0 ; u<cl.Unicodes.Length ; u++ )
					{
						hex2 += (cl.Unicodes[u]).ToString("X4");
					}
				}
				builder.Append(string.Format(System.Globalization.CultureInfo.InvariantCulture, "<{0}> <{0}> <{1}> ", hex1, hex2));
			}
			builder.Append("endbfrange ");

			builder.Append("endcmap CMapName currentdict /CMap defineresource pop end end\r\n");

			writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "<< {0} >>", Port.StringLength(builder.Length)));
			writer.WriteLine("stream");
			writer.WriteString(builder.ToString());
			writer.WriteLine("endstream endobj");
		}
		
		protected void CreateFontCharacters(Writer writer, FontList font, int fontPage, int count)
		{
			//	Crée tous les objets des caractères d'une fonte.
			int firstChar = fontPage*Export.charPerFont;
			for ( int i=0 ; i<count ; i++ )
			{
				CharacterList cl = font.GetCharacter(firstChar+i);
				this.CreateFontCharacter(writer, font, fontPage, cl);
			}
		}
		
		protected void CreateFontCharacter(Writer writer, FontList font, int fontPage, CharacterList cl)
		{
			//	Crée l'objet d'un caractère d'une fonte.
			writer.WriteObjectDef(Export.NameCharacter(font.Id, fontPage, cl));

			Font drawingFont = font.DrawingFont;
			Drawing.Transform ft = drawingFont.SyntheticTransform;
			int glyph = cl.Glyph;
			Path path = new Path();
			path.Append(drawingFont, glyph, ft.XX, ft.XY, ft.YX, ft.YY, ft.TX, ft.TY);


			Port port = new Port();
			port.ColorForce = ColorForce.Nothing;  // pas de commande de couleur !
			port.DefaultDecimals = 4;
			port.PaintSurface(path);

			string pdf = port.GetPDF();
			writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "<< {0} >>", Port.StringLength(pdf.Length)));
			writer.WriteLine("stream");
			writer.WriteString(pdf);
			writer.WriteLine("endstream endobj");
		}
		
		protected void FlushFont()
		{
			//	Libère toutes les fontes.
			this.characterList = null;
			this.fontList = null;
		}
		#endregion

		
		protected System.Collections.ArrayList ComputeLayers(int pageNumber)
		{
			//	Calcule la liste des calques, y compris ceux des pages maîtres.
			//	Les calques cachés à l'impression ne sont pas mis dans la liste.
			System.Collections.ArrayList layers = new System.Collections.ArrayList();
			System.Collections.ArrayList masterList = new System.Collections.ArrayList();
			this.document.Modifier.ComputeMasterPageList(masterList, pageNumber);

			//	Mets d'abord les premiers calques de toutes les pages maîtres.
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

			//	Mets ensuite tous les calques de la page.
			Objects.Abstract page = this.document.GetObjects[pageNumber] as Objects.Abstract;
			foreach ( Objects.Layer layer in this.document.Flat(page) )
			{
				if ( layer.Print == Objects.LayerPrint.Hide )  continue;
				layers.Add(layer);
			}

			//	Mets finalement les derniers calques de toutes les pages maîtres.
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


		//	Constante pour conversion dixièmes de millimètres -> 72ème de pouce
		protected static readonly double		mm2in = 7.2/25.4;
		public static readonly int				charPerFont = 256;

		protected Document						document;
		protected System.Collections.ArrayList	complexSurfaces;
		protected System.Collections.ArrayList	imageSurfaces;
		protected System.Collections.Hashtable	characterList;
		protected System.Collections.Hashtable	fontList;
		protected ColorConversion				colorConversion;
		protected ImageCompression				imageCompression;
		protected double						jpegQuality;
		protected double						imageMinDpi;
		protected double						imageMaxDpi;
		protected System.Collections.ArrayList	pageList;
	}
}
