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

namespace Epsitec.Common.Text.Exchange
{
    class StyleDefinition
    {
        public StyleDefinition(
            string caption,
            int ident,
            TextStyleClass textStyleClass,
            string[] baseStyleCaptions,
            string serialized,
            bool isDefaultStyle
        )
        {
            this.caption = caption;
            this.ident = ident;
            this.textStyleClass = textStyleClass;
            this.baseStyleCaptions = baseStyleCaptions;
            this.serialized = serialized;
            this.isDefaultStyle = isDefaultStyle;
        }

        private TextStyleClass textStyleClass;

        public TextStyleClass TextStyleClass
        {
            get { return textStyleClass; }
        }

        private string caption;

        public string Caption
        {
            get { return caption; }
        }

        private string serialized;

        public string Serialized
        {
            get { return serialized; }
        }

        private string[] baseStyleCaptions;

        public string[] BaseStyleCaptions
        {
            get { return baseStyleCaptions; }
        }

        private bool isDefaultStyle;

        public bool IsDefaultStyle
        {
            get { return isDefaultStyle; }
        }

        private int ident;

        public int Ident
        {
            get { return ident; }
        }
    }
}
