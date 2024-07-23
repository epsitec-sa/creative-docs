/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


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

        public char FieldSeparator { get; set; }

        public char LineSeparator { get; set; }

        public char QuoteChar { get; set; }

        public System.Text.Encoding Encoding { get; set; }

        public string[] ColumnNames { get; set; }
    }
}
