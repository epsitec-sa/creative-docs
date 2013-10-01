//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public abstract class AbstractTreeTableColumn : Widget
	{
		public AbstractTreeTableColumn()
		{
			this.detectedHoverRow = -1;
			this.hilitedHoverRow = -1;
		}


		public int								ColumnIndex
		{
			get;
			set;
		}

		public string							HeaderDescription
		{
			get;
			set;
		}

		public string							FooterDescription
		{
			get;
			set;
		}

		public int								HeaderHeight
		{
			get;
			set;
		}

		public int								FooterHeight
		{
			get;
			set;
		}

		public int								RowHeight
		{
			get;
			set;
		}

		public int								VisibleCellCount
		{
			get
			{
				return (int) ((this.ActualHeight - this.HeaderHeight - this.FooterHeight) / this.RowHeight);
			}
		}

		public int								HilitedHoverRow
		{
			get
			{
				return this.hilitedHoverRow;
			}
			set
			{
				if (this.hilitedHoverRow != value)
				{
					this.hilitedHoverRow = value;
					this.Invalidate ();
				}
			}
		}

	
		protected override void OnClicked(MessageEventArgs e)
		{
			base.OnClicked (e);
		}

		protected override void OnMouseMove(MessageEventArgs e)
		{
			this.SetDetectedHoverRow (this.Detect (e.Point));
			base.OnMouseMove (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			this.SetDetectedHoverRow (-1);
			base.OnExited (e);
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.InitializeAfterCellsChanged ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.PaintHeader (graphics);
			this.PaintFooter (graphics);
		}
		

		protected virtual void InitializeAfterCellsChanged()
		{
		}


		private void PaintHeader(Graphics graphics)
		{
			if (this.HeaderHeight > 0)
			{
				var rect = this.HeaderRect;

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (ColorManager.TreeTableBackgroundColor);

				if (!string.IsNullOrEmpty (this.HeaderDescription))
				{
					rect.Deflate (this.DescriptionMargin, 0, 0, 0);

					var font = Font.DefaultFont;

					graphics.Color = ColorManager.TextColor;
					graphics.PaintText (rect, this.HeaderDescription, font, this.FontSize, ContentAlignment.MiddleLeft);
				}
			}
		}

		private void PaintFooter(Graphics graphics)
		{
			if (this.FooterHeight > 0)
			{
				var rect = this.FooterRect;

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (ColorManager.TreeTableBackgroundColor);

				if (!string.IsNullOrEmpty (this.FooterDescription))
				{
					rect.Deflate (this.DescriptionMargin, 0, 0, 0);

					var font = Font.DefaultFont;

					graphics.Color = ColorManager.TextColor;
					graphics.PaintText (rect, this.FooterDescription, font, this.FontSize, ContentAlignment.MiddleLeft);
				}
			}
		}

		private int Detect(Point pos)
		{
			if (this.HeaderRect.Contains (pos))
			{
				return AbstractTreeTableColumn.HeaderRank;
			}

			if (this.FooterRect.Contains (pos))
			{
				return AbstractTreeTableColumn.FooterRank;
			}

			int count = this.VisibleCellCount;

			for (int rank = 0; rank < count; rank++)
			{
				var rect = this.GetCellsRect (rank);

				if (rect.Contains (pos))
				{
					return rank;
				}
			}

			return -1;
		}

		private void SetDetectedHoverRow(int row)
		{
			if (this.detectedHoverRow != row)
			{
				this.detectedHoverRow = row;
				this.OnCellHovered (row);
			}
		}

		private Rectangle HeaderRect
		{
			get
			{
				return new Rectangle (0, this.ActualHeight-this.HeaderHeight, this.ActualWidth, this.HeaderHeight);
			}
		}

		private Rectangle FooterRect
		{
			get
			{
				return new Rectangle (0, 0, this.ActualWidth, this.FooterHeight);
			}
		}

		protected int DescriptionMargin
		{
			get
			{
				return (int) (this.RowHeight * 0.5);
			}
		}

		protected double FontSize
		{
			get
			{
				return this.RowHeight * 0.6;
			}
		}

		protected Rectangle GetCellsRect(int y)
		{
			int p1 = this.GetVerticalPosition (y+1);
			int p2 = this.GetVerticalPosition (y);

			return new Rectangle (0, p1, this.ActualWidth, p2-p1);
		}

		private int GetVerticalPosition(int rank)
		{
			//	Retourne la position verticale, avec une subile répartition du reste
			//	pour que la dernière cellule touche toujours le bas.
			double dim = (this.ActualHeight - this.HeaderHeight - this.FooterHeight) / this.VisibleCellCount;
			return (int) this.ActualHeight - this.HeaderHeight - (int) (rank * dim);
		}


		#region Events handler
		private void OnCellClicked(int row)
		{
			if (this.CellClicked != null)
			{
				this.CellClicked (this, row);
			}
		}

		public delegate void CellClickedEventHandler(object sender, int row);
		public event CellClickedEventHandler CellClicked;

		private void OnCellHovered(int row)
		{
			if (this.CellHovered != null)
			{
				this.CellHovered (this, row);
			}
		}

		public delegate void CellHoveredEventHandler(object sender, int row);
		public event CellHoveredEventHandler CellHovered;
		#endregion


		public const int HeaderRank = 1000000;
		public const int FooterRank = 1000001;


		protected int							detectedHoverRow;
		protected int							hilitedHoverRow;
	}
}
