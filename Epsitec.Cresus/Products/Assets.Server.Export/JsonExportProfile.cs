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
	/// Paramètres pour JsonExport.
	/// </summary>
	public class JsonExportProfile : AbstractExportProfile
	{
		public JsonExportProfile(string endOfLine, bool camelCase, Encoding encoding)
		{
			this.EndOfLine = endOfLine;
			this.CamelCase = camelCase;
			this.Encoding  = encoding;
		}

		public JsonExportProfile(System.Xml.XmlReader reader)
		{
			this.EndOfLine = reader.ReadStringAttribute   (X.Attr.EndOfLine);
			this.CamelCase = reader.ReadBoolAttribute     (X.Attr.CamelCase);
			this.Compact   = reader.ReadBoolAttribute     (X.Attr.Compact);
			this.Encoding  = reader.ReadEncodingAttribute (X.Attr.Encoding);

			reader.Read ();
		}

	
		public static JsonExportProfile Default = new JsonExportProfile (TagConverters.Eol, true, Encoding.UTF8);

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

			writer.WriteStringAttribute   (X.Attr.EndOfLine, this.EndOfLine);
			writer.WriteBoolAttribute     (X.Attr.CamelCase, this.CamelCase);
			writer.WriteBoolAttribute     (X.Attr.Compact,   this.Compact);
			writer.WriteEncodingAttribute (X.Attr.Encoding,  this.Encoding);

			writer.WriteEndElement ();
		}


		public readonly string					EndOfLine;
		public readonly bool					CamelCase;
		public readonly bool					Compact;
		public readonly Encoding				Encoding;
	}
}