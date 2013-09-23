//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Timeline de base, constituée de lignes AbstractTimelineRow créées en fonction du
	/// tableau de TimelineRowDescription. On ne gère ici aucun déplacement dans le temps.
	/// On se contente d'afficher les TimelineCell passées avec SetCells.
	/// Un seul événement CellClicked permet de connaître la ligne et la cellule cliquée.
	/// </summary>
	public class Timeline : Widget
	{
		public Timeline()
		{
		}


		public IEnumerable<AbstractTimelineRow>	Rows
		{
			get
			{
				return this.Children.Cast<AbstractTimelineRow> ();
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
					this.UpdateChildrensPivot ();
					this.Invalidate ();
				}
			}
		}

		public int								VisibleCellCount
		{
			get
			{
				return (int) (this.ActualBounds.Width / this.CellWidth);
			}
		}

		public int								BeforePivotCount
		{
			get
			{
				var num   = this.VisibleCellCount;
				var pivot = this.Pivot;

				return (int) (num * pivot);
			}
		}
		
		public int								AfterPivotCount
		{
			get
			{
				return this.VisibleCellCount - this.BeforePivotCount;
			}
		}

		private int								CellWidth
		{
			get
			{
				return (int) (this.ActualBounds.Height / this.RelativeHeights);
			}
		}

		private double							RelativeHeights
		{
			get
			{
				return this.Children.Cast<AbstractTimelineRow> ().Sum (x => x.RelativeHeight);
			}
		}


		public void SetRows(List<AbstractTimelineRow> rows)
		{
			this.Children.Clear ();

			int index = 0;

			foreach (var row in rows)
			{
				row.RowIndex = index;
				this.Children.Add (row);
				index++;
			}
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateChildrensGeometry ();
		}


#if false
		private void CreateChildrens()
		{
			//	Crée toutes les lignes-enfant en fonction du tableau de TimelineRowDescription.
			this.Children.Clear ();

			foreach (var desc in this.rows.Reverse ())
			{
				var row = this.CreateRow (desc);

				if (row != null)
				{
					this.Children.Add (row);

					row.Anchor = AnchorStyles.All;

					row.CellClicked += delegate (object sender, int rank)
					{
						this.OnCellClicked (desc, rank);
					};
				}
			}
		}
#endif

		private void UpdateChildrensGeometry()
		{
			//	Met à jour la géométrie de toutes les lignes-enfant.
			int w = this.CellWidth;
			int bottom = 0;

			foreach (var children in this.Children)
			{
				var row = children as AbstractTimelineRow;
				System.Diagnostics.Debug.Assert (row != null);

				var h = (int) (w * row.RelativeHeight);
				var top = this.ActualHeight-bottom-h;

				row.Anchor = AnchorStyles.All;
				row.CellWidth = w;
				row.Margins = new Margins (0, 0, top, bottom);
				row.PreferredHeight = h;

				bottom += h;
			}
		}

		private void UpdateChildrensPivot()
		{
			//	Met à jour le pivot de toutes les lignes-enfant.
			foreach (var children in this.Children)
			{
				var row = children as AbstractTimelineRow;
				System.Diagnostics.Debug.Assert (row != null);

				row.Pivot = this.pivot;
			}
		}


		#region Events handler
		private void OnCellClicked(int row, int rank)
		{
			if (this.CellClicked != null)
			{
				this.CellClicked (this, row, rank);
			}
		}

		public delegate void CellClickedEventHandler(object sender, int row, int rank);
		public event CellClickedEventHandler CellClicked;
		#endregion


		private double							pivot;
	}
}