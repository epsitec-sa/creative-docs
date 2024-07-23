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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe Cell implémente un conteneur pour peupler des tableaux et
    /// des grilles.
    /// </summary>
    public class Cell : AbstractGroup
    {
        public Cell()
        {
            this.InheritsParentFocus = true;
            this.hasBottomSeparator = true;
            this.hasRightSeparator = true;
        }

        public Cell(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public void Insert(Widget widget)
        {
            widget.SetParent(this);

            if ((widget.Dock == DockStyle.None) && (widget.Anchor == AnchorStyles.None))
            {
                widget.SetManualBounds(widget.Parent.ActualBounds);
            }

            if (this.cellArray != null)
            {
                this.cellArray.NotifyCellChanged(this);
            }
        }

        public void Remove(Widget widget)
        {
            this.Children.Remove(widget);

            if (this.cellArray != null)
            {
                this.cellArray.NotifyCellChanged(this);
            }
        }

        public void Clear()
        {
            this.Children.Clear();

            if (this.cellArray != null)
            {
                this.cellArray.NotifyCellChanged(this);
            }
        }

        public AbstractCellArray CellArray
        {
            get { return this.cellArray; }
        }

        public int RankColumn
        {
            get { return this.rankColumn; }
        }

        public int RankRow
        {
            get { return this.rankRow; }
        }

        public bool IsHilite
        {
            get { return this.isHilite; }
            set
            {
                if (this.isHilite != value)
                {
                    this.isHilite = value;
                    this.Invalidate();
                }
            }
        }

        public bool IsFlyOver
        {
            get { return this.isFlyOver; }
            set
            {
                if (this.isFlyOver != value)
                {
                    this.isFlyOver = value;
                    this.Invalidate();
                }
            }
        }

        public bool HasBottomSeparator
        {
            //	Il suffit qu'une seule cellule dise ne pas avoir de séparateur pour que toute la ligne
            //	horizontale de séparation soit omise.
            get { return this.hasBottomSeparator; }
            set
            {
                if (this.hasBottomSeparator != value)
                {
                    this.hasBottomSeparator = value;
                    this.Invalidate();
                }
            }
        }

        public bool HasRightSeparator
        {
            //	Il suffit qu'une seule cellule dise ne pas avoir de séparateur pour que toute la ligne
            //	verticale de séparation soit omise.
            get { return this.hasRightSeparator; }
            set
            {
                if (this.hasRightSeparator != value)
                {
                    this.hasRightSeparator = value;
                    this.Invalidate();
                }
            }
        }

        internal void SetArrayRank(AbstractCellArray array, int column, int row)
        {
            this.cellArray = array;
            this.rankColumn = column;
            this.rankRow = row;
        }

        protected override void PaintBackgroundImplementation(
            Drawing.Graphics graphics,
            Drawing.Rectangle clipRect
        )
        {
            IAdorner adorner = Widgets.Adorners.Factory.Active;

            Drawing.Rectangle rect = this.Client.Bounds;
            WidgetPaintState state = this.GetPaintState();

            if (!this.BackColor.IsEmpty)
            {
                graphics.AddFilledRectangle(rect);
                graphics.RenderSolid(this.BackColor);
            }

            if (this.isHilite)
            {
                graphics.AddFilledRectangle(rect);
                graphics.RenderSolid(this.cellArray.HiliteColor);
            }

            if (this.isFlyOver)
            {
                Drawing.Color color = Drawing.Color.FromAlphaColor(0.2, adorner.ColorCaption);
                graphics.AddFilledRectangle(rect);
                graphics.RenderSolid(color);
            }

            adorner.PaintCellBackground(graphics, rect, state);
        }

        protected AbstractCellArray cellArray;
        protected int rankColumn;
        protected int rankRow;
        protected bool isHilite;
        protected bool isFlyOver;
        protected bool hasBottomSeparator;
        protected bool hasRightSeparator;
    }
}
