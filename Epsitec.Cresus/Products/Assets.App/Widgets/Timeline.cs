//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class Timeline : Widget
	{
		public Timeline()
		{
			this.Display = TimelineDisplay.Month | TimelineDisplay.Days | TimelineDisplay.Glyphs;
		}

		public TimelineDisplay					Display
		{
			get
			{
				return this.display;
			}
			set
			{
				if (this.display != value)
				{
					this.display = value;
					this.CreateChildrens ();
					this.Invalidate ();
				}
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
				return (int) (this.ActualBounds.Width / this.CellDim);
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

		private int								CellDim
		{
			get
			{
				return (int) (this.ActualBounds.Height / this.LineCount);
			}
		}

		private int								LineCount
		{
			get
			{
				return Timeline.displayLines.Where (x => (x & this.display) != 0).Count ();
			}
		}

		
		public void SetCells(TimelineCell[] cells)
		{
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
			//	Crée toutes les lignes-enfant en fonction du mode this.Display.
			this.Children.Clear ();

			foreach (var displayLine in Timeline.displayLines)
			{
				if ((displayLine & this.display) != 0)
				{
					var row = this.CreateRow (displayLine);

					if (row != null)
					{
						this.Children.Add (row);

						row.Dock = DockStyle.Bottom;

						row.CellClicked += delegate (object sender, int rank)
						{
							this.OnCellClicked (row.Display, rank);
						};
					}
				}
			}
		}

		private void UpdateChildrensGeometry()
		{
			//	Met à jour la géométrie de toutes les lignes-enfant.
			int dim = this.CellDim;

			foreach (var children in this.Children)
			{
				var row = children as AbstractTimelineRow;
				System.Diagnostics.Debug.Assert (row != null);

				row.PreferredHeight = dim;
			}
		}

		private void UpdateChildrensPivot()
		{
			//	Met à jour le pivot de toutes les lignes-enfant.
			int dim = this.CellDim;

			foreach (var children in this.Children)
			{
				var row = children as AbstractTimelineRow;
				System.Diagnostics.Debug.Assert (row != null);

				row.Pivot = this.pivot;
			}
		}

		private AbstractTimelineRow CreateRow(TimelineDisplay display)
		{
			//	Crée une ligne-enfant.
			switch (display)
			{
				case TimelineDisplay.Month:
					return new TimelineRowMonths (display);

				case TimelineDisplay.WeeksOfYear:
					return new TimelineRowWeeksOfYear (display);

				case TimelineDisplay.DaysOfWeek:
					return new TimelineRowDaysOfWeek (display);

				case TimelineDisplay.Days:
					return new TimelineRowDays (display);

				case TimelineDisplay.Glyphs:
					return new TimelineRowGlyphs (display);

				default:
					return null;
			}
		}

		private static TimelineDisplay[] displayLines =
		{
			//	Détermine l'ordre des lignes-enfant à afficher, de bas en haut.
			TimelineDisplay.Glyphs,
			TimelineDisplay.Days,
			TimelineDisplay.DaysOfWeek,
			TimelineDisplay.WeeksOfYear,
			TimelineDisplay.Month,
		};


		#region Events handler
		private void OnCellClicked(TimelineDisplay display, int rank)
		{
			if (this.CellClicked != null)
			{
				this.CellClicked (this, display, rank);
			}
		}

		public delegate void CellClickedEventHandler(object sender, TimelineDisplay display, int rank);
		public event CellClickedEventHandler CellClicked;
		#endregion


		private TimelineDisplay					display;
		private double							pivot;
		private TimelineCell[]					cells;
	}
}