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


using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>TextValue</c> structure is used to represent a text value of a given
    /// category, provided by an <see cref="AbstractEntity"/>.
    /// </summary>
    public struct TextValue : System.IEquatable<TextValue>
    {
        public TextValue(string simpleText)
            : this(TextValueCategory.Summary, simpleText) { }

        public TextValue(TextValueCategory category, string simpleText)
        {
            this.category = category;
            this.simpleText = simpleText;
            this.formattedText = FormattedText.Empty;
        }

        public TextValue(FormattedText formattedText)
            : this(TextValueCategory.Summary, formattedText) { }

        public TextValue(TextValueCategory category, FormattedText formattedText)
        {
            this.category = category;
            this.simpleText = null;
            this.formattedText = TextFormatter.FormatText(formattedText);
        }

        public TextValue(TextValue keyword)
        {
            this.category = keyword.category;
            this.simpleText = keyword.simpleText;
            this.formattedText = keyword.formattedText;
        }

        public TextValueCategory Category
        {
            get { return this.category; }
        }

        public string SimpleText
        {
            get
            {
                if (this.simpleText == null)
                {
                    return this.formattedText.ToSimpleText();
                }
                else
                {
                    return this.simpleText;
                }
            }
        }

        public FormattedText FormattedText
        {
            get
            {
                if (this.formattedText.IsNull())
                {
                    return FormattedText.FromSimpleText(this.simpleText);
                }
                else
                {
                    return this.formattedText;
                }
            }
        }

        public TextFormat Format
        {
            get
            {
                var format = TextFormat.None;

                if (this.simpleText != null)
                {
                    format |= TextFormat.Simple;
                }
                if (this.formattedText.IsNull() == false)
                {
                    format |= TextFormat.Formatted;
                }

                return format;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is TextValue)
            {
                return this.Equals((TextValue)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.category.GetHashCode()
                ^ (this.simpleText == null ? 0 : this.simpleText.GetHashCode())
                ^ (this.formattedText.IsNull() ? 0 : this.formattedText.GetHashCode());
        }

        #region IEquatable<Keyword> Members

        public bool Equals(TextValue other)
        {
            if (this.category == other.category)
            {
                if ((this.simpleText != null) || (other.simpleText != null))
                {
                    return this.SimpleText == other.SimpleText;
                }
                else
                {
                    return this.FormattedText == other.FormattedText;
                }
            }

            return false;
        }

        #endregion


        private readonly TextValueCategory category;
        private readonly string simpleText;
        private readonly FormattedText formattedText;
    }
}
