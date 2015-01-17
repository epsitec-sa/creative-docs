//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Export.Helpers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour TextExport.
	/// </summary>
	public class TextExportProfile : AbstractExportProfile
	{
		public TextExportProfile(string columnSeparator, string columnBracket, string escape, string endOfLine, bool hasHeader, bool inverted, Encoding encoding)
		{
			this.ColumnSeparator = columnSeparator;
			this.ColumnBracket   = columnBracket;
			this.Escape          = escape;
			this.EndOfLine       = endOfLine;
			this.HasHeader       = hasHeader;
			this.Inverted        = inverted;
			this.Encoding        = encoding;
		}

		public TextExportProfile(System.Xml.XmlReader reader)
		{
			this.ColumnSeparator = IOHelpers.ReadStringAttribute   (reader, "ColumnSeparator");
			this.ColumnBracket   = IOHelpers.ReadStringAttribute   (reader, "ColumnBracket");
			this.Escape          = IOHelpers.ReadStringAttribute   (reader, "Escape");
			this.EndOfLine       = IOHelpers.ReadStringAttribute   (reader, "EndOfLine");
			this.HasHeader       = IOHelpers.ReadBoolAttribute     (reader, "HasHeader");
			this.Inverted        = IOHelpers.ReadBoolAttribute     (reader, "Inverted");
			this.Encoding        = IOHelpers.ReadEncodingAttribute (reader, "Encoding");

			reader.Read ();
		}

	
		public static TextExportProfile CsvProfile = new TextExportProfile (";", "\"", "\"", TagConverters.Eol, true, false, Encoding.UTF8);
		public static TextExportProfile TxtProfile = new TextExportProfile (TagConverters.Compile ("<TAB>"), null, "\\", TagConverters.Eol, true, false, Encoding.UTF8);

		public string							FinalColumnSeparator
		{
			get
			{
				return TagConverters.GetFinalText (this.ColumnSeparator);
			}
		}

		public string							FinalColumnBracket
		{
			get
			{
				return TagConverters.GetFinalText (this.ColumnBracket);
			}
		}

		public string							FinalEscape
		{
			get
			{
				return TagConverters.GetFinalText (this.Escape);
			}
		}

		public string							FinalEndOfLine
		{
			get
			{
				return TagConverters.GetFinalText (this.EndOfLine);
			}
		}


		public override void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			IOHelpers.WriteStringAttribute   (writer, "ColumnSeparator", this.ColumnSeparator);
			IOHelpers.WriteStringAttribute   (writer, "ColumnBracket",   this.ColumnBracket);
			IOHelpers.WriteStringAttribute   (writer, "Escape",          this.Escape);
			IOHelpers.WriteStringAttribute   (writer, "EndOfLine",       this.EndOfLine);
			IOHelpers.WriteBoolAttribute     (writer, "HasHeader",       this.HasHeader);
			IOHelpers.WriteBoolAttribute     (writer, "Inverted",        this.Inverted);
			IOHelpers.WriteEncodingAttribute (writer, "Encoding",        this.Encoding);

			writer.WriteEndElement ();
		}


		public readonly string					ColumnSeparator;
		public readonly string					ColumnBracket;
		public readonly string					Escape;
		public readonly string					EndOfLine;
		public readonly bool					HasHeader;
		public readonly bool					Inverted;
		public readonly Encoding				Encoding;
	}
}