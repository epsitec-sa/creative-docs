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


using System.Collections.Generic;

namespace Epsitec.Common.IO
{
    public static class StringLineExtractor
    {
        public static IEnumerable<string> GetLines(string buffer)
        {
            int pos = 0;

            while (pos < buffer.Length)
            {
                int end = buffer.IndexOf('\n', pos);

                if (end < 0)
                {
                    end = buffer.Length;
                }

                if (pos < end)
                {
                    int trim;
                    if (buffer[end - 1] == '\r')
                    {
                        trim = 1;
                    }
                    else
                    {
                        trim = 0;
                    }

                    string line = buffer.Substring(pos, end - pos - trim);

                    yield return line;
                }

                pos = end + 1;
            }
        }

        public static IEnumerable<string> GetLines(
            System.IO.Stream stream,
            System.Text.Encoding encoding = null
        )
        {
            using (
                var reader = new System.IO.StreamReader(
                    stream,
                    encoding ?? System.Text.Encoding.Default
                )
            )
            {
                while (reader.EndOfStream == false)
                {
                    yield return reader.ReadLine();
                }
            }
        }
    }
}
