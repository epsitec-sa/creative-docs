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
    public class NativeTextOut
    {
        public NativeTextOut()
        {
            this.Initialize();
        }

        private void Initialize() { }

        public void AppendTextLine(string text)
        {
            this.output.AppendLine(text);
        }

        public void AppendStyleLine(string text)
        {
            this.styles.AppendLine(text);
        }

        internal Internal.FormattedText FormattedText
        {
            get { return new Internal.FormattedText(this.ToString()); }
        }

        public override string ToString()
        {
            return this.styles.ToString() + this.output.ToString();
        }

        private System.Text.StringBuilder output = new System.Text.StringBuilder();
        private System.Text.StringBuilder styles = new System.Text.StringBuilder();
    }
}
