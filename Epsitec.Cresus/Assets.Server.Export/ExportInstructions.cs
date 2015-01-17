//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public struct ExportInstructions
	{
		public ExportInstructions(ExportFormat format, string filename)
		{
			this.Format    = format;
			this.Filename  = filename;
		}

		public ExportInstructions(System.Xml.XmlReader reader)
		{
			this.Format   = (ExportFormat) IOHelpers.ReadTypeAttribute (reader, "Format", typeof (ExportFormat));
			this.Filename = IOHelpers.ReadStringAttribute (reader, "Filename");

			reader.Read ();
		}


		public bool IsEmpty
		{
			get
			{
				return this.Format == ExportFormat.Unknown
					&& string.IsNullOrEmpty (this.Filename);
			}
		}

		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			IOHelpers.WriteTypeAttribute   (writer, "Format",   this.Format);
			IOHelpers.WriteStringAttribute (writer, "Filename", this.Filename);

			writer.WriteEndElement ();
		}


		public static ExportInstructions Empty = new ExportInstructions (ExportFormat.Unknown, null);

		public readonly ExportFormat			Format;
		public readonly string					Filename;
	}
}