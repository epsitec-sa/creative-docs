//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Classe générique qui représente une ligne de Timeline.
	/// </summary>
	public abstract class AbstractTimelineRow : Widget
	{
		public AbstractTimelineRow()
		{
			this.RelativeHeight    = 1.0;
			this.RelativeWidth     = 1.0;
			this.detectedHoverRank = -1;
			this.hilitedHoverRank  = -1;
		}


		public double							RelativeHeight
		{
			get;
			set;
		}

		public string							Description
		{
			get;
			set;
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
					this.Invalidate ();
				}
			}
		}

		public int								CellWidth
		{
			get;
			set;
		}

		public double							RelativeWidth
		{
			get;
			set;
		}

		public int								LabelWidth
		{
			get;
			set;
		}

		public int								VisibleCellCount
		{
			get
			{
				return (int) ((this.ActualWidth - this.LabelWidth) / this.CellWidth);
			}
		}

		public int								HilitedHoverRank
		{
			get
			{
				return this.hilitedHoverRank;
			}
			set
			{
				if (this.hilitedHoverRank != value)
				{
					this.hilitedHoverRank = value;
					this.Invalidate ();
				}
			}
		}

		public bool								PermanentGrid
		{
			get
			{
				return this.permanentGrid;
			}
			set
			{
				if (this.permanentGrid != value)
				{
					this.permanentGrid = value;
					this.Invalidate ();
				}
			}
		}

		public void ClearDetectedHoverRank()
		{
			this.SetDetectedHoverRank (-1);
		}


		protected override void OnClicked(MessageEventArgs e)
		{
			if ((e.Message.Button & MouseButtons.Right) == 0)
			{
				this.OnCellClicked (this.detectedHoverRank);
			}
			else
			{
				var pos = this.MapClientToScreen (e.Point);
				this.OnCellRightClicked (this.detectedHoverRank, pos);
			}
			base.OnClicked (e);
		}

		protected override void OnDoubleClicked(MessageEventArgs e)
		{
			if ((e.Message.Button & MouseButtons.Right) == 0)
			{
				this.OnCellDoubleClicked (this.detectedHoverRank);
			}
			base.OnDoubleClicked (e);
		}

		protected override void OnMouseMove(MessageEventArgs e)
		{
			if ((e.Message.Button & MouseButtons.Right) == 0)
			{
				this.SetDetectedHoverRank (this.Detect (e.Point));
			}
			base.OnMouseMove (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			if ((e.Message.Button & MouseButtons.Right) == 0)
			{
				this.SetDetectedHoverRank (-1);
			}
			base.OnExited (e);
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (new Rectangle (Point.Zero, this.ActualSize));
			graphics.RenderSolid (ColorManager.GetBackgroundColor ());

			this.PaintLabel (graphics);
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

		private void SetDetectedHoverRank(int rank)
		{
			if (this.detectedHoverRank != rank)
			{
				this.detectedHoverRank = rank;
				this.OnCellHovered (rank);
			}
		}


		protected void PaintGrid(Graphics graphics, Rectangle rect, int currentRank, int hilitedRank, bool isGroup = false, double decrease = 0.1)
		{
			//	Dessine une portion de grille dans une cellule, sous forme de 2 traits,
			//	en bas et à droite. Plus la distance jusqu'à la cellule survolée est
			//	grande et plus l'effet est estompé.
			if (hilitedRank != -1 || this.permanentGrid)
			{
				rect.Deflate (0.5);

				graphics.AddLine (rect.BottomLeft, rect.BottomRight);
				
				//	Les lignes correspondant à un groupe n'ont pas les séparateurs verticaux.
				if (!isGroup)
				{
					graphics.AddLine (rect.BottomRight, rect.TopRight);
				}

				Color color;

				if (this.permanentGrid)
				{
					color = ColorManager.GridColor;
				}
				else
				{
					var delta = System.Math.Abs (currentRank - hilitedRank);
					var alpha = System.Math.Max (1.0 - delta * decrease, 0.0);
					color = Color.FromAlphaColor (alpha, ColorManager.GridColor);
				}

				graphics.RenderSolid (color);
			}
		}

		protected void PaintLabel(Graphics graphics)
		{
			if (this.LabelWidth > 0 && (this.hilitedHoverRank != -1 || this.permanentGrid))
			{
				this.PaintGrid (graphics, this.BoxLabelRect, 0, 0);
			}

			if (this.LabelWidth > 0 && !string.IsNullOrEmpty (this.Description))
			{
				var font = Font.DefaultFont;
				graphics.Color = ColorManager.TextColor;
				graphics.PaintText (this.TextLabelRect, this.Description, font, this.FontSize, ContentAlignment.MiddleLeft);
			}
		}

		private Rectangle BoxLabelRect
		{
			get
			{
				return new Rectangle (0, 0, this.LabelWidth, this.ActualHeight);
			}
		}

		private Rectangle TextLabelRect
		{
			get
			{
				return new Rectangle (this.LabelMargin, 0, this.LabelWidth - this.LabelMargin, this.ActualHeight);
			}
		}

		private int LabelMargin
		{
			get
			{
				return (int) (this.CellWidth / this.RelativeWidth * 0.5);
			}
		}

		protected double FontSize
		{
			get
			{
				return this.CellWidth / this.RelativeWidth * 0.6;
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
			double dim = (this.ActualWidth - this.LabelWidth) / this.VisibleCellCount;
			return this.LabelWidth + (int) (rank * dim);
		}


		#region Events handler
		private void OnCellClicked(int rank)
		{
			this.CellClicked.Raise (this, rank);
		}

		public event EventHandler<int> CellClicked;


		private void OnCellDoubleClicked(int rank)
		{
			this.CellDoubleClicked.Raise (this, rank);
		}

		public event EventHandler<int> CellDoubleClicked;


		private void OnCellRightClicked(int rank, Point pos)
		{
			this.CellRightClicked.Raise (this, rank, pos);
		}

		public event EventHandler<int, Point> CellRightClicked;


		private void OnCellHovered(int rank)
		{
			this.CellHovered.Raise (this, rank);
		}

		public event EventHandler<int> CellHovered;
		#endregion


		protected double						pivot;
		protected int							detectedHoverRank;
		protected int							hilitedHoverRank;
		protected bool							permanentGrid;
	}
}
