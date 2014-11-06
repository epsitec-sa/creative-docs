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
			this.EndOfLine = IOHelpers.ReadStringAttribute   (reader, "EndOfLine");
			this.CamelCase = IOHelpers.ReadBoolAttribute     (reader, "CamelCase");
			this.Compact   = IOHelpers.ReadBoolAttribute     (reader, "Compact");
			this.Encoding  = IOHelpers.ReadEncodingAttribute (reader, "Encoding");

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

			IOHelpers.WriteStringAttribute   (writer, "EndOfLine", this.EndOfLine);
			IOHelpers.WriteBoolAttribute     (writer, "CamelCase", this.CamelCase);
			IOHelpers.WriteBoolAttribute     (writer, "Compact",   this.Compact);
			IOHelpers.WriteEncodingAttribute (writer, "Encoding",  this.Encoding);

			writer.WriteEndElement ();
		}


		public readonly string					EndOfLine;
		public readonly bool					CamelCase;
		public readonly bool					Compact;
		public readonly Encoding				Encoding;
	}
}