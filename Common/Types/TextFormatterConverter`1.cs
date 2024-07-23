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

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>TextFormatterConverter&lt;T&gt;</c> class simplifies the implementation of
    /// simple converters, which only handle one type with no special handling.
    /// </summary>
    /// <typeparam name="T">The type of the data being converted.</typeparam>
    public abstract class TextFormatterConverter<T> : ITextFormatterConverter
    {
        #region ITextFormatterConverter Members

        public IEnumerable<System.Type> GetConvertibleTypes()
        {
            yield return typeof(T);
        }

        public FormattedText ToFormattedText(
            object value,
            System.Globalization.CultureInfo culture,
            TextFormatterDetailLevel detailLevel
        )
        {
            if ((value is T) && (value != null))
            {
                switch (detailLevel)
                {
                    case TextFormatterDetailLevel.Default:
                    case TextFormatterDetailLevel.Title:
                    case TextFormatterDetailLevel.Compact:
                    case TextFormatterDetailLevel.Full:
                        return this.ToFormattedText((T)value, culture, detailLevel);

                    default:
                        throw new System.NotSupportedException(
                            string.Format("Detail level {0} not supported", detailLevel)
                        );
                }
            }
            else
            {
                return FormattedText.Empty;
            }
        }

        #endregion

        protected abstract FormattedText ToFormattedText(
            T value,
            System.Globalization.CultureInfo culture,
            TextFormatterDetailLevel detailLevel
        );
    }
}
