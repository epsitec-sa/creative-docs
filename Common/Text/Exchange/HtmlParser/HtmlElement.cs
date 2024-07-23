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

using System;
using System.ComponentModel;
using System.Text;

namespace Epsitec.Common.Text.Exchange.HtmlParser
{
    /// <summary>
    /// The HtmlElement object represents any HTML element. An element has a name
    /// and zero or more attributes.
    /// </summary>
    public class HtmlElement : HtmlNode
    {
        protected string mName;
        protected HtmlNodeCollection mNodes;
        protected HtmlAttributeCollection mAttributes;
        protected bool mIsTerminated;
        protected bool mIsExplicitlyTerminated;

        /// <summary>
        /// This constructs a new HTML element with the specified tag name.
        /// </summary>
        /// <param name="name">The name of this element</param>
        public HtmlElement(string name)
        {
            mNodes = new HtmlNodeCollection(this);
            mAttributes = new HtmlAttributeCollection(this);
            mName = name;
            mIsTerminated = false;
        }

        /// <summary>
        /// This is the tag name of the element. e.g. BR, BODY, TABLE etc.
        /// </summary>
        [Category("General"), Description("The name of the tag/element")]
        public string Name
        {
            get { return mName.ToLower(); }
            set { mName = value; }
        }

        /// <summary>
        /// This is the collection of all child nodes of this one. If this node is actually
        /// a text node, this will throw an InvalidOperationException exception.
        /// </summary>
        [Category("General"), Description("The set of child nodes")]
        public HtmlNodeCollection Nodes
        {
            get
            {
                if (IsText())
                {
                    throw new InvalidOperationException(
                        "An HtmlText node does not have child nodes"
                    );
                }
                return mNodes;
            }
        }

        /// <summary>
        /// This is the collection of attributes associated with this element.
        /// </summary>
        [Category("General"), Description("The set of attributes associated with this element")]
        public HtmlAttributeCollection Attributes
        {
            get { return mAttributes; }
        }

        /// <summary>
        /// This flag indicates that the element is explicitly closed using the "<name/>" method.
        /// </summary>
        internal bool IsTerminated
        {
            get
            {
                if (Nodes.Count > 0)
                {
                    return false;
                }
                else
                {
                    return mIsTerminated | mIsExplicitlyTerminated;
                }
            }
            set { mIsTerminated = value; }
        }

        /// <summary>
        /// This flag indicates that the element is explicitly closed using the "</name>" method.
        /// </summary>
        internal bool IsExplicitlyTerminated
        {
            get { return mIsExplicitlyTerminated; }
            set { mIsExplicitlyTerminated = value; }
        }

        internal bool NoEscaping
        {
            get { return "script".Equals(Name.ToLower()) || "style".Equals(Name.ToLower()); }
        }

        /// <summary>
        /// This will return the HTML representation of this element.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string value = "<" + mName;
            foreach (HtmlAttribute attribute in Attributes)
            {
                value += " " + attribute.ToString();
            }
            value += ">";
            return value;
        }

        [
            Category("General"),
            Description("A concatination of all the text associated with this element")
        ]
        public string Text
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (HtmlNode node in Nodes)
                {
                    if (node is HtmlText)
                    {
                        stringBuilder.Append(((HtmlText)node).Text);
                    }
                }
                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// This will return the HTML for this element and all subnodes.
        /// </summary>
        [Category("Output")]
        public override string HTML
        {
            get
            {
                StringBuilder html = new StringBuilder();
                html.Append("<" + mName);
                foreach (HtmlAttribute attribute in Attributes)
                {
                    html.Append(" " + attribute.HTML);
                }
                if (Nodes.Count > 0)
                {
                    html.Append(">");
                    foreach (HtmlNode node in Nodes)
                    {
                        html.Append(node.HTML);
                    }
                    html.Append("</" + mName + ">");
                }
                else
                {
                    if (IsExplicitlyTerminated)
                    {
                        html.Append("></" + mName + ">");
                    }
                    else if (IsTerminated)
                    {
                        html.Append("/>");
                    }
                    else
                    {
                        html.Append(">");
                    }
                }
                return html.ToString();
            }
        }

        /// <summary>
        /// This will return the XHTML for this element and all subnodes.
        /// </summary>
        [Category("Output")]
        public override string XHTML
        {
            get
            {
                if ("html".Equals(mName) && this.Attributes["xmlns"] == null)
                {
                    Attributes.Add(new HtmlAttribute("xmlns", "http://www.w3.org/1999/xhtml"));
                }
                StringBuilder html = new StringBuilder();
                html.Append("<" + mName.ToLower());
                foreach (HtmlAttribute attribute in Attributes)
                {
                    html.Append(" " + attribute.XHTML);
                }
                if (IsTerminated)
                {
                    html.Append("/>");
                }
                else
                {
                    if (Nodes.Count > 0)
                    {
                        html.Append(">");
                        foreach (HtmlNode node in Nodes)
                        {
                            html.Append(node.XHTML);
                        }
                        html.Append("</" + mName.ToLower() + ">");
                    }
                    else
                    {
                        html.Append("/>");
                    }
                }
                return html.ToString();
            }
        }
    }
}
