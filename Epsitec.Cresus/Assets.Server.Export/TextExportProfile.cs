//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		public static TextExportProfile CsvProfile = new TextExportProfile (";", "\"",  "\"", "\r\n", true, false, Encoding.UTF8);
		public static TextExportProfile TxtProfile = new TextExportProfile ("\t", null, "\\", "\r\n", true, false, Encoding.UTF8);

		public readonly string					ColumnSeparator;
		public readonly string					ColumnBracket;
		public readonly string					Escape;
		public readonly string					EndOfLine;
		public readonly bool					HasHeader;
		public readonly bool					Inverted;
		public readonly Encoding				Encoding;
	}
}