//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour XmlExport.
	/// </summary>
	public class XmlExportProfile : AbstractExportProfile
	{
		public XmlExportProfile(string bodyTag, string recordTag, string indent, string endOfLine, bool camelCase, bool compact)
		{
			this.BodyTag   = bodyTag;
			this.RecordTag = recordTag;
			this.Indent    = indent;
			this.EndOfLine = endOfLine;
			this.CamelCase = camelCase;
			this.Compact   = compact;
		}

		public static XmlExportProfile Default = new XmlExportProfile ("data", "record", "\t", "\r\n", true, false);

		public readonly string					BodyTag;
		public readonly string					RecordTag;
		public readonly string					Indent;
		public readonly string					EndOfLine;
		public readonly bool					CamelCase;
		public readonly bool					Compact;
	}
}