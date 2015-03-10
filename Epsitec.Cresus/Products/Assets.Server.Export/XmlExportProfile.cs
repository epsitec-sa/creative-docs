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
			this.BodyTag   = reader.ReadStringAttribute   (X.Attr.BodyTag);
			this.RecordTag = reader.ReadStringAttribute   (X.Attr.RecordTag);
			this.Indent    = reader.ReadStringAttribute   (X.Attr.Indent);
			this.EndOfLine = reader.ReadStringAttribute   (X.Attr.EndOfLine);
			this.CamelCase = reader.ReadBoolAttribute     (X.Attr.CamelCase);
			this.Compact   = reader.ReadBoolAttribute     (X.Attr.Compact);
			this.Encoding  = reader.ReadEncodingAttribute (X.Attr.Encoding);

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

			writer.WriteStringAttribute   (X.Attr.BodyTag,   this.BodyTag);
			writer.WriteStringAttribute   (X.Attr.RecordTag, this.RecordTag);
			writer.WriteStringAttribute   (X.Attr.Indent,    this.Indent);
			writer.WriteStringAttribute   (X.Attr.EndOfLine, this.EndOfLine);
			writer.WriteBoolAttribute     (X.Attr.CamelCase, this.CamelCase);
			writer.WriteBoolAttribute     (X.Attr.Compact,   this.Compact);
			writer.WriteEncodingAttribute (X.Attr.Encoding,  this.Encoding);

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