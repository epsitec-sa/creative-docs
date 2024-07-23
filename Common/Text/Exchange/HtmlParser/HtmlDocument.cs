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

namespace Epsitec.Common.Text.Exchange.HtmlParser
{
    /// <summary>
    /// This is the basic HTML document object used to represent a sequence of HTML.
    /// </summary>
    public class HtmlDocument
    {
        HtmlNodeCollection mNodes = new HtmlNodeCollection(null);
        private string mXhtmlHeader =
            "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">";

        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        /// <param name="wantSpaces">true : want spaces.</param>
        internal HtmlDocument(string html, bool wantSpaces)
        {
            HtmlParser parser = new HtmlParser();
            parser.RemoveEmptyElementText = !wantSpaces;
            mNodes = parser.Parse(html);
        }

        public string DocTypeXHTML
        {
            get { return mXhtmlHeader; }
            set { mXhtmlHeader = value; }
        }

        /// <summary>
        /// This is the collection of nodes used to represent this document.
        /// </summary>
        public HtmlNodeCollection Nodes
        {
            get { return mNodes; }
        }

        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        /// <returns>An instance of the newly created object.</returns>
        public static HtmlDocument Create(string html)
        {
            return new HtmlDocument(html, false);
        }

        /// <summary>
        /// This will create a new document object by parsing the HTML specified.
        /// </summary>
        /// <param name="html">The HTML to parse.</param>
        /// <param name="wantSpaces">Set this to true if you want to preserve all whitespace from the input HTML</param>
        /// <returns>An instance of the newly created object.</returns>
        public static HtmlDocument Create(string html, bool wantSpaces)
        {
            return new HtmlDocument(html, wantSpaces);
        }

        /// <summary>
        /// This will return the HTML used to represent this document.
        /// </summary>
        public string HTML
        {
            get
            {
                StringBuilder html = new StringBuilder();
                foreach (HtmlNode node in Nodes)
                {
                    html.Append(node.HTML);
                }
                return html.ToString();
            }
        }

        /// <summary>
        /// This will return the XHTML document used to represent this document.
        /// </summary>
        ///
        public string XHTML
        {
            get
            {
                StringBuilder html = new StringBuilder();
                if (mXhtmlHeader != null)
                {
                    html.Append(mXhtmlHeader);
                    html.Append("\r\n");
                }
                foreach (HtmlNode node in Nodes)
                {
                    html.Append(node.XHTML);
                }
                return html.ToString();
            }
        }
    }
}
