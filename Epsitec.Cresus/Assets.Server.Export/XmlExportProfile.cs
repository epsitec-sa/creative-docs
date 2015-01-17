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
	/// Paramètres pour XmlExport.
	/// </summary>
	public class XmlExportProfile : AbstractExportProfile
	{
		public XmlExportProfile(string bodyTag, string recordTag, string indent, string endOfLine, bool camelCase, bool compact, Encoding encoding)
		{
			this.BodyTag   = bodyTag;
			this.RecordTag = recordTag;
			this.Indent    = indent;
			this.EndOfLine = endOfLine;
			this.CamelCase = camelCase;
			this.Compact   = compact;
			this.Encoding  = encoding;
		}

		public XmlExportProfile(System.Xml.XmlReader reader)
		{
			this.BodyTag   = IOHelpers.ReadStringAttribute   (reader, "BodyTag");
			this.RecordTag = IOHelpers.ReadStringAttribute   (reader, "RecordTag");
			this.Indent    = IOHelpers.ReadStringAttribute   (reader, "Indent");
			this.EndOfLine = IOHelpers.ReadStringAttribute   (reader, "EndOfLine");
			this.CamelCase = IOHelpers.ReadBoolAttribute     (reader, "CamelCase");
			this.Compact   = IOHelpers.ReadBoolAttribute     (reader, "Compact");
			this.Encoding  = IOHelpers.ReadEncodingAttribute (reader, "Encoding");

			reader.Read ();
		}

	
		public static XmlExportProfile Default = new XmlExportProfile ("data", "record",
			TagConverters.Compile ("<TAB>"), TagConverters.Eol, true, false, Encoding.UTF8);

		public string							FinalIndent
		{
			get
			{
				return TagConverters.GetFinalText (this.Indent);
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

			IOHelpers.WriteStringAttribute   (writer, "BodyTag",   this.BodyTag);
			IOHelpers.WriteStringAttribute   (writer, "RecordTag", this.RecordTag);
			IOHelpers.WriteStringAttribute   (writer, "Indent",    this.Indent);
			IOHelpers.WriteStringAttribute   (writer, "EndOfLine", this.EndOfLine);
			IOHelpers.WriteBoolAttribute     (writer, "CamelCase", this.CamelCase);
			IOHelpers.WriteBoolAttribute     (writer, "Compact",   this.Compact);
			IOHelpers.WriteEncodingAttribute (writer, "Encoding",  this.Encoding);

			writer.WriteEndElement ();
		}


		public readonly string					BodyTag;
		public readonly string					RecordTag;
		public readonly string					Indent;
		public readonly string					EndOfLine;
		public readonly bool					CamelCase;
		public readonly bool					Compact;
		public readonly Encoding				Encoding;
	}
}