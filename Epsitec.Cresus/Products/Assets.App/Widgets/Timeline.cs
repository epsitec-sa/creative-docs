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
				return this.rows.Sum (x => x.RelativeHeight);
			}
		}


		public void SetRows(TimelineRowDescription[] rows)
		{
			//	Descriptions des lignes à afficher, de haut en bas.
			this.rows = rows;

			this.CreateChildrens ();
			this.Invalidate ();
		}

		public void SetCells(TimelineCell[] cells)
		{
			//	Descriptions des cellules à afficher, de gauche à droite.
			this.cells = cells;

			foreach (var children in this.Children)
			{
				var row = children as AbstractTimelineRow;
				System.Diagnostics.Debug.Assert (row != null);

				row.SetCells (this.cells);
			}
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateChildrensGeometry ();
		}


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

		private void UpdateChildrensGeometry()
		{
			//	Met à jour la géométrie de toutes les lignes-enfant.
			int w = this.CellWidth;
			int bottom = 0;

			foreach (var children in this.Children)
			{
				var row = children as AbstractTimelineRow;
				System.Diagnostics.Debug.Assert (row != null);

				var h = (int) (w * row.Row.RelativeHeight);
				var top = this.ActualHeight-bottom-h;

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

		private AbstractTimelineRow CreateRow(TimelineRowDescription row)
		{
			//	Crée une ligne-enfant.
			switch (row.Type)
			{
				case TimelineRowType.Month:
					return new TimelineRowMonths (row);

				case TimelineRowType.WeeksOfYear:
					return new TimelineRowWeeksOfYear (row);

				case TimelineRowType.DaysOfWeek:
					return new TimelineRowDaysOfWeek (row);

				case TimelineRowType.Days:
					return new TimelineRowDays (row);

				case TimelineRowType.Glyphs:
					return new TimelineRowGlyphs (row);

				case TimelineRowType.Values:
					return new TimelineRowValues (row);

				default:
					return null;
			}
		}


		#region Events handler
		private void OnCellClicked(TimelineRowDescription row, int rank)
		{
			if (this.CellClicked != null)
			{
				this.CellClicked (this, row, rank);
			}
		}

		public delegate void CellClickedEventHandler(object sender, TimelineRowDescription row, int rank);
		public event CellClickedEventHandler CellClicked;
		#endregion


		private TimelineCell[]					cells;
		private TimelineRowDescription[]		rows;
		private double							pivot;
	}
}