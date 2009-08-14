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
			this.FieldSeparator = ";";
			this.MultilineSeparator = "\n";
			this.Encoding = System.Text.Encoding.Default;
		}

		public string FieldSeparator
		{
			get;
			set;
		}

		public string MultilineSeparator
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
