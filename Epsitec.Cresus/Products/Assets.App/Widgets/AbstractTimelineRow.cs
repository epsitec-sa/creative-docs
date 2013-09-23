//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Classe générique qui représente une ligne de Timeline.
	/// </summary>
	public abstract class AbstractTimelineRow : Widget
	{
		public AbstractTimelineRow(TimelineRowDescription row)
		{
			this.row = row;
			this.hoverRank = -1;
		}


		public TimelineRowDescription			Row
		{
			get
			{
				return this.row;
			}
		}

		public double							Pivot
		{
			get
			{
				return this.pivot;
			}
			set
			{
				if (this.pivot != value)
				{
					this.pivot = value;
					this.InitializeAfterCellsChanged ();
					this.Invalidate ();
				}
			}
		}

		public int								CellWidth
		{
			get;
			set;
		}

		public int								VisibleCellCount
		{
			get
			{
				return (int) (this.ActualBounds.Width / this.CellWidth);
			}
		}

		private int								HoverRank
		{
			get
			{
				return this.hoverRank;
			}
			set
			{
				if (this.hoverRank != value)
				{
					this.hoverRank = value;
					this.Invalidate ();
				}
			}
		}

	
		public void SetCells(TimelineCell[] cells)
		{
			this.cells = cells;
			this.InitializeAfterCellsChanged ();
			this.Invalidate ();
		}

		protected virtual void InitializeAfterCellsChanged()
		{
		}


		protected override void OnClicked(MessageEventArgs e)
		{
			this.OnCellClicked (this.hoverRank);
			base.OnClicked (e);
		}

		protected override void OnMouseMove(MessageEventArgs e)
		{
			this.HoverRank = this.Detect (e.Point);
			base.OnMouseMove (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			this.HoverRank = -1;
			base.OnExited (e);
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.InitializeAfterCellsChanged ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.cells == null || this.VisibleCellCount == 0)
			{
				return;
			}

			this.Paint (graphics);
		}



		private int Detect(Point pos)
		{
			int x = (int) pos.X;
			int count = this.VisibleCellCount;

			for (int rank = 0; rank < count; rank++)
			{
				int p1 = this.GetHorizontalPosition (rank);
				int p2 = this.GetHorizontalPosition (rank+1);

				if (x >= p1 && x < p2)
				{
					return rank;
				}
			}

			return -1;
		}

		protected virtual void Paint(Graphics graphics)
		{
			int x = 0;
			int index = 0;
			var lastCell = new TimelineCell ();  // cellule invalide

			for (int rank = 0; rank <= this.VisibleCellCount; rank++)
			{
				var cell = this.GetCell (rank);
				if (!this.IsSame (lastCell, cell) && x != rank)
				{
					var rect = this.GetCellsRect (x, rank);
					bool isHover = (this.hoverRank >= x && this.hoverRank < rank);

					this.PaintCellBackground (graphics, rect, lastCell, isHover, index);
					this.PaintCellForeground (graphics, rect, lastCell, isHover, index);

					index++;
					x = rank;
				}

				lastCell = cell;
			}
		}


		protected Rectangle GetCellsRect(int x1, int x2)
		{
			int p1 = this.GetHorizontalPosition (x1);
			int p2 = this.GetHorizontalPosition (x2);

			return new Rectangle (p1, 0, p2-p1, this.ActualHeight);
		}

		private int GetHorizontalPosition(int rank)
		{
			//	Retourne la position horizontale, avec une subile répartition du reste
			//	pour que la cellule de droite touche toujours le bord droite.
			double dim = this.ActualWidth / this.VisibleCellCount;
			return (int) (rank * dim);
		}


		protected virtual bool IsSame(TimelineCell c1, TimelineCell c2)
		{
			return false;
		}

		protected virtual void PaintCellBackground(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover, int index)
		{
			//	Dessine le fond.
			var color = this.GetCellColor (cell, isHover, index);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);
		}

		protected virtual void PaintCellForeground(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover, int index)
		{
			//	Dessine le contenu.
			var text = this.GetCellText (cell);
			var font = Font.DefaultFont;
			graphics.Color = ColorManager.TextColor;
			graphics.PaintText (rect, text, font, rect.Height*0.6, ContentAlignment.MiddleCenter);
		}

		protected virtual Color GetCellColor(TimelineCell cell, bool isHover, int index)
		{
			return ColorManager.GetBackgroundColor ();
		}

		protected virtual string GetCellText(TimelineCell cell)
		{
			return null;
		}


		protected TimelineCell GetCell(int rank)
		{
			if (rank < this.VisibleCellCount)
			{
				int index = this.GetListIndex (rank);

				if (index >= 0 && index < this.cells.Length)
				{
					return this.cells[index];
				}
			}

			return new TimelineCell ();  // retourne une cellule invalide
		}

		private int GetListIndex(int rank)
		{
			if (rank >= 0 && rank < this.cells.Length)
			{
				int offset = (int) ((double) (this.cells.Length - this.VisibleCellCount) * this.pivot);
				return rank + offset;
			}
			else
			{
				return -1;
			}
		}


		#region Events handler
		private void OnCellClicked(int rank)
		{
			if (this.CellClicked != null)
			{
				this.CellClicked (this, rank);
			}
		}

		public delegate void CellClickedEventHandler(object sender, int rank);
		public event CellClickedEventHandler CellClicked;
		#endregion


		private readonly TimelineRowDescription	row;
		protected TimelineCell[]				cells;
		private double							pivot;
		protected int							hoverRank;
	}
}
