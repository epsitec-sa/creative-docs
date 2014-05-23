//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Export
{
	public struct ExportTextProfile
	{
		public ExportTextProfile(string columnSeparator, string columnBracket, string escape, string endOfLine)
		{
			this.ColumnSeparator = columnSeparator;
			this.ColumnBracket   = columnBracket;
			this.Escape          = escape;
			this.EndOfLine       = endOfLine;
		}

		public static ExportTextProfile CsvProfile = new ExportTextProfile (",", "\"", "\"", "\r\n");
		public static ExportTextProfile TxtProfile = new ExportTextProfile ("\t", null, "\\", "\r\n");

		public readonly string					ColumnSeparator;
		public readonly string					ColumnBracket;
		public readonly string					Escape;
		public readonly string					EndOfLine;
	}
}