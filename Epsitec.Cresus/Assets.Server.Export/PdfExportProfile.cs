//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;
using Epsitec.Cresus.Assets.Export.Helpers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour PdfExport.
	/// </summary>
	public class PdfExportProfile : AbstractExportProfile
	{
		public PdfExportProfile(PdfStyle style, Size pageSize, Margins pageMargins, Margins cellMargins,
			ExportFont font, double fontSize, bool automaticColumnWidths,
			string header, string footer, string indent, string watermark)
		{
			this.Style                 = style;
			this.PageSize              = pageSize;
			this.PageMargins           = pageMargins;
			this.CellMargins           = cellMargins;
			this.Font                  = font;
			this.FontSize              = fontSize;
			this.AutomaticColumnWidths = automaticColumnWidths;
			this.Header                = header;
			this.Footer                = footer;
			this.Indent                = indent;
			this.Watermark             = watermark;
		}

		public PdfExportProfile(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == X.Style)
					{
						this.Style = new PdfStyle (reader);
					}
					else if (reader.Name == X.Params)
					{
						this.PageSize.Width        = (double) reader.ReadDecimalAttribute (X.Attr.PageSize_Width);
						this.PageSize.Height       = (double) reader.ReadDecimalAttribute (X.Attr.PageSize_Height);

						this.PageMargins.Left      = (double) reader.ReadDecimalAttribute (X.Attr.PageMargins_Left);
						this.PageMargins.Right     = (double) reader.ReadDecimalAttribute (X.Attr.PageMargins_Right);
						this.PageMargins.Top       = (double) reader.ReadDecimalAttribute (X.Attr.PageMargins_Top);
						this.PageMargins.Bottom    = (double) reader.ReadDecimalAttribute (X.Attr.PageMargins_Bottom);

						this.CellMargins.Left      = (double) reader.ReadDecimalAttribute (X.Attr.CellMargins_Left);
						this.CellMargins.Right     = (double) reader.ReadDecimalAttribute (X.Attr.CellMargins_Right);
						this.CellMargins.Top       = (double) reader.ReadDecimalAttribute (X.Attr.CellMargins_Top);
						this.CellMargins.Bottom    = (double) reader.ReadDecimalAttribute (X.Attr.CellMargins_Bottom);

						this.Font                  = (ExportFont) reader.ReadTypeAttribute (X.Attr.Font, typeof (ExportFont));
						this.FontSize              = (double) reader.ReadDecimalAttribute (X.Attr.FontSize);
						this.AutomaticColumnWidths = reader.ReadBoolAttribute   (X.Attr.AutomaticColumnWidths);
						this.Header                = reader.ReadStringAttribute (X.Attr.Header);
						this.Footer                = reader.ReadStringAttribute (X.Attr.Footer);
						this.Indent                = reader.ReadStringAttribute (X.Attr.Indent);
						this.Watermark             = reader.ReadStringAttribute (X.Attr.Watermark);

						reader.Read ();
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

	
		public static PdfExportProfile Default =
			new PdfExportProfile (PdfStyle.Factory (PdfPredefinedStyle.Frameless), new Size (297.0, 210.0),
				new Margins (10.0), new Margins (1.0), ExportFont.Arial, 10.0, false,
				TagConverters.Compile ("Crésus <TIRET> <TITLE>"),
				TagConverters.Compile ("Epsitec SA"),
				TagConverters.Compile ("<BULLET><SPACE><SPACE><SPACE>"),
				TagConverters.Compile ("SPECIMEN"));

		public string							FinalHeader
		{
			get
			{
				return TagConverters.GetFinalText (this.Header);
			}
		}

		public string							FinalFooter
		{
			get
			{
				return TagConverters.GetFinalText (this.Footer);
			}
		}

		public string							FinalIndent
		{
			get
			{
				return TagConverters.GetFinalText (this.Indent);
			}
		}

		public string							FinalWatermark
		{
			get
			{
				return TagConverters.GetFinalText (this.Watermark);
			}
		}


		protected override void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			this.Style.Serialize (writer, X.Style);

			writer.WriteStartElement (X.Params);
			writer.WriteDecimalAttribute (X.Attr.PageSize_Width,        (decimal) this.PageSize.Width);
			writer.WriteDecimalAttribute (X.Attr.PageSize_Height,       (decimal) this.PageSize.Height);

			writer.WriteDecimalAttribute (X.Attr.PageMargins_Left,      (decimal) this.PageMargins.Left);
			writer.WriteDecimalAttribute (X.Attr.PageMargins_Right,     (decimal) this.PageMargins.Right);
			writer.WriteDecimalAttribute (X.Attr.PageMargins_Top,       (decimal) this.PageMargins.Top);
			writer.WriteDecimalAttribute (X.Attr.PageMargins_Bottom,    (decimal) this.PageMargins.Bottom);

			writer.WriteDecimalAttribute (X.Attr.CellMargins_Left,      (decimal) this.CellMargins.Left);
			writer.WriteDecimalAttribute (X.Attr.CellMargins_Right,     (decimal) this.CellMargins.Right);
			writer.WriteDecimalAttribute (X.Attr.CellMargins_Top,       (decimal) this.CellMargins.Top);
			writer.WriteDecimalAttribute (X.Attr.CellMargins_Bottom,    (decimal) this.CellMargins.Bottom);

			writer.WriteTypeAttribute    (X.Attr.Font,                  this.Font);
			writer.WriteDecimalAttribute (X.Attr.FontSize,              (decimal) this.FontSize);
			writer.WriteBoolAttribute    (X.Attr.AutomaticColumnWidths, this.AutomaticColumnWidths);
			writer.WriteStringAttribute  (X.Attr.Header,                this.Header);
			writer.WriteStringAttribute  (X.Attr.Footer,                this.Footer);
			writer.WriteStringAttribute  (X.Attr.Indent,                this.Indent);
			writer.WriteStringAttribute  (X.Attr.Watermark,             this.Watermark);
			writer.WriteEndElement ();

			writer.WriteEndElement ();
		}


		public readonly PdfStyle				Style;
		public readonly Size					PageSize;
		public readonly Margins					PageMargins;
		public readonly Margins					CellMargins;
		public readonly ExportFont				Font;
		public readonly double					FontSize;
		public readonly bool					AutomaticColumnWidths;
		public readonly string					Header;
		public readonly string					Footer;
		public readonly string					Indent;
		public readonly string					Watermark;
	}
}