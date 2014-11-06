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
	/// Paramètres pour YamlExport.
	/// </summary>
	public class YamlExportProfile : AbstractExportProfile
	{
		public YamlExportProfile(string indent, string endOfLine, bool camelCase, Encoding encoding)
		{
			this.Indent    = indent;
			this.EndOfLine = endOfLine;
			this.CamelCase = camelCase;
			this.Encoding  = encoding;
		}

		public YamlExportProfile(System.Xml.XmlReader reader)
		{
			this.Indent    = IOHelpers.ReadStringAttribute (reader, "Indent");
			this.EndOfLine = IOHelpers.ReadStringAttribute (reader, "EndOfLine");
			this.CamelCase = IOHelpers.ReadBoolAttribute   (reader, "CamelCase");
			this.Encoding  = (Encoding) IOHelpers.ReadTypeAttribute (reader, "Encoding", typeof (Encoding));

			reader.Read ();
		}

	
		public static YamlExportProfile Default = new YamlExportProfile (
			TagConverters.Compile ("<SPACE><SPACE>"),
			TagConverters.Eol, true, Encoding.UTF8);

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

			IOHelpers.WriteStringAttribute (writer, "Indent",    this.Indent);
			IOHelpers.WriteStringAttribute (writer, "EndOfLine", this.EndOfLine);
			IOHelpers.WriteBoolAttribute   (writer, "CamelCase", this.CamelCase);
			IOHelpers.WriteTypeAttribute   (writer, "Encoding",  this.Encoding);

			writer.WriteEndElement ();
		}


		public readonly string					Indent;
		public readonly string					EndOfLine;
		public readonly bool					CamelCase;
		public readonly Encoding				Encoding;
	}
}