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
		public AbstractTimelineRow()
		{
			this.RelativeHeight = 1.0;
			this.hoverRank = -1;
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

		protected virtual void InitializeAfterCellsChanged()
		{
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


		protected double						pivot;
		protected int							hoverRank;
	}
}
