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

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Widgets
{
    /// <summary>
    /// La classe ButtonFontFace est un IconButton contenant un échantillon de police FontSample.
    /// </summary>
    public class ButtonFontFace : IconButton
    {
        public ButtonFontFace()
        {
            this.sample = new FontSample(this);
            this.sample.IsSampleAbc = true;
            this.sample.IsCenter = true;
            this.sample.Dock = DockStyle.Fill;
            this.sample.Margins = new Margins(0, 0, 3, 2);
        }

        public ButtonFontFace(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public OpenType.FontIdentity FontIdentity
        {
            get { return this.sample.FontIdentity; }
            set { this.sample.FontIdentity = value; }
        }

        protected override void OnActiveStateChanged()
        {
            base.OnActiveStateChanged();
            this.sample.ActiveState = this.ActiveState;
        }

        protected FontSample sample;
    }
}
