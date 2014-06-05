//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		public static YamlExportProfile Default = new YamlExportProfile ("  ", "\r\n", true, Encoding.UTF8);

		public readonly string					Indent;
		public readonly string					EndOfLine;
		public readonly bool					CamelCase;
		public readonly Encoding				Encoding;
	}
}