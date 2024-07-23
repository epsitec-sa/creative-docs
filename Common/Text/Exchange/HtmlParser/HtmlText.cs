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

using System.ComponentModel;

namespace Epsitec.Common.Text.Exchange.HtmlParser
{
    /// <summary>
    /// The HtmlText node represents a simple piece of text from the document.
    /// </summary>

    public class HtmlText : HtmlNode
    {
        protected string mText;

        /// <summary>
        /// This constructs a new node with the given text content.
        /// </summary>
        /// <param name="text"></param>
        public HtmlText(string text)
        {
            mText = text;
        }

        /// <summary>
        /// This is the text associated with this node.
        /// </summary>
        [Category("General"), Description("The text located in this text node")]
        public string Text
        {
            get { return mText; }
            set { mText = value; }
        }

        /// <summary>
        /// This will return the text for outputting inside an HTML document.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

        internal bool NoEscaping
        {
            get
            {
                if (mParent == null)
                {
                    return false;
                }
                else
                {
                    return ((HtmlElement)mParent).NoEscaping;
                }
            }
        }

        /// <summary>
        /// This will return the HTML to represent this text object.
        /// </summary>
        public override string HTML
        {
            get
            {
                if (NoEscaping)
                {
                    return Text;
                }
                else
                {
                    return HtmlEncoder.EncodeValue(Text);
                }
            }
        }

        /// <summary>
        /// This will return the XHTML to represent this text object.
        /// </summary>
        public override string XHTML
        {
            get { return HtmlEncoder.EncodeValue(Text); }
        }
    }
}
