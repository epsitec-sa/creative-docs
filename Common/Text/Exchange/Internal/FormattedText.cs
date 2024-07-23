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


namespace Epsitec.Common.Text.Exchange.Internal
{
    /// <summary>
    /// La classe <c>FormattedText</c> ne sert qu'à pouvoir stocker le format
    /// presse-papier interne (qui existe sous forme de string) dans le presse-
    /// papier Windows. C'est nécessaire à cause de la gestion particulière du
    /// presse-papier avec .NET
    /// </summary>
    [System.Serializable]
    public class FormattedText
    {
        // ******************************************************************
        // TODO bl-net8-cross clipboard
        // - clipboard related class, some stuff removed (stub)
        // ******************************************************************
        public FormattedText()
        {
            this.encodedText = "";
        }

        public FormattedText(string encodedText)
        {
            this.encodedText = encodedText;
        }

        public string EncodedText
        {
            get { return this.encodedText; }
            set { this.encodedText = value; }
        }

        /*        public static System.Windows.Forms.DataFormats.Format ClipboardFormat
                {
                    get { return FormattedText.format; }
                }
        */
        public override string ToString()
        {
            return this.EncodedText;
        }

        static FormattedText()
        {
            // Registers a new data format with the windows Clipboard
            //format = System.Windows.Forms.DataFormats.GetFormat("Epsitec.FormattedText");
        }

        //private static System.Windows.Forms.DataFormats.Format format;
        private string encodedText;
    }
}
