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

using System.Text;

namespace Epsitec.Common.Text.Exchange
{
    public class ExchangeStringBuilder
    {
        public ExchangeStringBuilder()
        {
            theBuilder = new System.Collections.Generic.List<byte>();
        }

        public int Length
        {
            get { return this.theBuilder.Count; }
        }

        public byte this[int index]
        {
            get { return this.theBuilder[index]; }
        }

        public void Append(string str)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] encodedBytes = utf8.GetBytes(str);

            for (int i = 0; i < encodedBytes.Length; i++)
            {
                this.theBuilder.Add(encodedBytes[i]);
            }
#if DEBUG
            this.debugstring.Append(str);
#endif
        }

        public void AppendLine(string str)
        {
            this.length += str.Length + 2;
            this.Append(str);
            this.theBuilder.Add((byte)'\r');
            this.theBuilder.Add((byte)'\n');
#if DEBUG
            this.debugstring.Append("\r\n");
#endif
        }

        public override string ToString()
        {
            StringBuilder tmpBuilder = new StringBuilder();

            for (int i = 0; i < theBuilder.Count; i++)
            {
                tmpBuilder.Append((char)theBuilder[i]);
            }

            return tmpBuilder.ToString();
        }

        private System.Collections.Generic.List<byte> theBuilder;
        private System.Text.StringBuilder debugstring = new System.Text.StringBuilder();
        private int length;
    }
}
