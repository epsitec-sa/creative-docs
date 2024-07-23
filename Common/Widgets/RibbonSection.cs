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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe RibbonSection est la classe de base pour toutes les sections de rubans.
    /// </summary>
    public class RibbonSection : Widget
    {
        public RibbonSection()
        {
            this.title = new TextLayout();
            this.title.SetEmbedder(this);

            this.Padding = new Margins(5, 5, 5, RibbonSection.LabelHeight + 5);
        }

        public RibbonSection(RibbonPage page)
            : this()
        {
            page.Items.Add(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { }

            base.Dispose(disposing);
        }

        public string Title
        {
            get { return this.title.Text; }
            set { this.title.Text = value; }
        }

#if false
		public override Margins GetInternalPadding()
		{
			return new Margins (4, 4, 4, 4);
		}
#endif

        protected Rectangle UsefulZone
        {
            //	Retourne la zone rectangulaire utile pour les widgets.
            get
            {
                Rectangle rect = this.Client.Bounds;
                rect.Bottom += RibbonSection.LabelHeight;
                rect.Deflate(4);
                return rect;
            }
        }

        protected override void UpdateClientGeometry()
        {
            //	Met à jour la géométrie.
            base.UpdateClientGeometry();
        }

        protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
        {
            IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

            Rectangle rect = this.Client.Bounds;

            Rectangle userRect = rect;
            userRect.Bottom += RibbonSection.LabelHeight;

            Rectangle textRect = rect;
            textRect.Top = textRect.Bottom + RibbonSection.LabelHeight;

            adorner.PaintRibbonSectionBackground(
                graphics,
                rect,
                userRect,
                textRect,
                this.title,
                this.GetPaintState()
            );
        }

        protected static readonly double LabelHeight = 14;

        protected TextLayout title;
    }
}
