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

namespace Epsitec.Common.Text.Exchange.HtmlParser
{
    class SpanStyleElements
    {
        /// <summary>
        /// La classe SpanStyleElements s'occupe de décoder une chaine du type
        /// "FONT-SIZE: 9pt; FONT-FAMILY: 'Times Roman' ..."
        /// </summary>

        System.Collections.Generic.Dictionary<string, string> dict =
            new System.Collections.Generic.Dictionary<string, string>();

        public SpanStyleElements(string spanstylestring)
        {
            string[] sp = spanstylestring.Split(semicolonseparators);

            foreach (string element in sp)
            {
                string[] pair = element.Split(SpanStyleElements.colonseparators);
                pair[0] = pair[0].Trim().ToLower();
                pair[1] = pair[1].Trim(SpanStyleElements.quotestotrim);
                dict[pair[0]] = pair[1];
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (KeyValuePair<string, string> kv in this.dict)
            {
                yield return kv.Key;
            }
        }

        public string this[string index]
        {
            get
            {
                string output = null;
                this.dict.TryGetValue(index, out output);
                return output;
            }
        }

        private static char[] semicolonseparators = ";".ToCharArray();
        private static char[] colonseparators = ":".ToCharArray();
        private static char[] quotestotrim = " '\"\r\n".ToCharArray();
    }
}
