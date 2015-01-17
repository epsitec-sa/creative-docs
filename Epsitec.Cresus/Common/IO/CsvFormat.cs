//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The <c>CsvFormat</c> class defines the CSV file format.
	/// </summary>
	public sealed class CsvFormat
	{
		public CsvFormat()
		{
			this.FieldSeparator = ';';
			this.LineSeparator = '\n';
			this.QuoteChar = '\"';
			this.Encoding = System.Text.Encoding.Default;
		}

		public char FieldSeparator
		{
			get;
			set;
		}

		public char LineSeparator
		{
			get;
			set;
		}

		public char QuoteChar
		{
			get;
			set;
		}

		public System.Text.Encoding Encoding
		{
			get;
			set;
		}

		public string[] ColumnNames
		{
			get;
			set;
		}
	}
}
