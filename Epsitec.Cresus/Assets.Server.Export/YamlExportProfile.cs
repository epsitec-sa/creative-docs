//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour XmlExport.
	/// </summary>
	public class YamlExportProfile : AbstractExportProfile
	{
		public YamlExportProfile(string indent, string endOfLine, bool camelCase)
		{
			this.Indent    = indent;
			this.EndOfLine = endOfLine;
			this.CamelCase = camelCase;
		}

		public static YamlExportProfile Default = new YamlExportProfile ("  ", "\r\n", true);

		public readonly string					Indent;
		public readonly string					EndOfLine;
		public readonly bool					CamelCase;
	}
}