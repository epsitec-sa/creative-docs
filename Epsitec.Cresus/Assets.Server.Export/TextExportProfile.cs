//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour TextExport.
	/// </summary>
	public struct TextExportProfile
	{
		public TextExportProfile(string columnSeparator, string columnBracket, string escape, string endOfLine, bool hasHeader, bool inverted)
		{
			this.ColumnSeparator = columnSeparator;
			this.ColumnBracket   = columnBracket;
			this.Escape          = escape;
			this.EndOfLine       = endOfLine;
			this.HasHeader       = hasHeader;
			this.Inverted        = inverted;
		}

		public static TextExportProfile CsvProfile = new TextExportProfile (",", "\"", "\"", "\r\n", true, false);
		public static TextExportProfile TxtProfile = new TextExportProfile ("\t", null, "\\", "\r\n", true, false);

		public readonly string					ColumnSeparator;
		public readonly string					ColumnBracket;
		public readonly string					Escape;
		public readonly string					EndOfLine;
		public readonly bool					HasHeader;
		public readonly bool					Inverted;
	}
}