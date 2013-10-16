//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce contrôleur inclut une Timeline et un ascenseur horizontal permettant la
	/// navigation en fonction du nombre de cellules CellsCount.
	/// Il cache complètement la Timeline sous-jacente à l'aide d'une Facade.
	/// </summary>
	public class NavigationTimelineController
	{
		public void CreateUI(Widget parent)
		{
			this.timeline = new Timeline ()
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.scroller = new HScroller ()
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.timeline.SizeChanged += delegate
			{
				this.UpdateScroller ();
			};

			this.timeline.CellClicked += delegate (object sender, int row, int rank)
			{
				this.OnCellClicked (row, rank);
			};

			this.timeline.CellDoubleClicked += delegate (object sender, int row, int rank)
			{
				this.OnCellDoubleClicked (row, rank);
			};

			this.scroller.ValueChanged += delegate
			{
				this.OnDateChanged ();
			};
		}


		public int								CellsCount
		{
			get
			{
				return this.cellsCount;
			}
			set
			{
				if (this.cellsCount != value)
				{
					this.cellsCount = value;
					this.UpdateScroller ();
				}
			}
		}

		public int								LeftVisibleCell
		{
			get
			{
				return (int) this.scroller.Value;
			}
			set
			{
				this.scroller.Value = value;
			}
		}

		public int GetLeftVisibleCell(int sel)
		{
			int count = System.Math.Min (this.timeline.VisibleCellsCount, this.cellsCount);

			sel = System.Math.Min (sel + count/2, this.cellsCount-1);
			sel = System.Math.Max (sel - count, 0);

			return sel;
		}


		#region Timeline Facade
		public bool								ShowLabels
		{
			get
			{
				return this.timeline.ShowLabels;
			}
			set
			{
				this.timeline.ShowLabels = value;
			}
		}

		public double							Pivot
		{
			get
			{
				return this.timeline.Pivot;
			}
			set
			{
				this.timeline.Pivot = value;
			}
		}

		public int								VisibleCellsCount
		{
			get
			{
				return this.timeline.VisibleCellsCount;
			}
		}

		public double							RelativeWidth
		{
			get
			{
				return this.timeline.RelativeWidth;
			}
			set
			{
				this.timeline.RelativeWidth = value;
			}
		}

		public void SetRows(TimelineRowDescription[] descriptions)
		{
			this.timeline.SetRows (descriptions);
		}

		public void SetRowYearCells(int rank, TimelineCellDate[] cells)
		{
			this.timeline.SetRowYearCells (rank, cells);
		}

		public void SetRowMonthCells(int rank, TimelineCellDate[] cells)
		{
			this.timeline.SetRowMonthCells (rank, cells);
		}

		public void SetRowWeekOfYearCells(int rank, TimelineCellDate[] cells)
		{
			this.timeline.SetRowWeekOfYearCells (rank, cells);
		}

		public void SetRowDayOfWeekCells(int rank, TimelineCellDate[] cells)
		{
			this.timeline.SetRowDayOfWeekCells (rank, cells);
		}

		public void SetRowDayCells(int rank, TimelineCellDate[] cells)
		{
			this.timeline.SetRowDayCells (rank, cells);
		}

		public void SetRowDayMonthCells(int rank, TimelineCellDate[] cells)
		{
			this.timeline.SetRowDayMonthCells (rank, cells);
		}

		public void SetRowValueCells(int rank, TimelineCellValue[] cells, decimal min, decimal max)
		{
			this.timeline.SetRowValueCells (rank, cells, min, max);
		}

		public void SetRowGlyphCells(int rank, TimelineCellGlyph[] cells)
		{
			this.timeline.SetRowGlyphCells (rank, cells);
		}
		#endregion


		private void UpdateScroller()
		{
			if (this.timeline == null || this.scroller == null)
			{
				return;
			}

			var totalCells   = (decimal) this.cellsCount;
			var visibleCells = (decimal) this.timeline.VisibleCellsCount;

			if (visibleCells < 0 || totalCells == 0)
			{
				this.scroller.Resolution = 1.0m;
				this.scroller.VisibleRangeRatio = 1.0m;

				this.scroller.MinValue = 0.0m;
				this.scroller.MaxValue = 1.0m;

				this.scroller.SmallChange = 1.0m;
				this.scroller.LargeChange = 1.0m;
			}
			else
			{
				this.scroller.Resolution = 1.0m;
				this.scroller.VisibleRangeRatio = System.Math.Min (visibleCells/totalCells, 1.0m);

				this.scroller.MinValue = 0.0m;
				this.scroller.MaxValue = System.Math.Max (totalCells - visibleCells, 0.0m);

				this.scroller.SmallChange = 1.0m;
				this.scroller.LargeChange = visibleCells;
			}

			this.OnContentChanged ();  // on demande de mettre à jour le contenu
		}


		#region Events handler
		private void OnDateChanged()
		{
			if (this.DateChanged != null)
			{
				this.DateChanged (this);
			}
		}

		public delegate void DateChangedEventHandler(object sender);
		public event DateChangedEventHandler DateChanged;

	
		private void OnCellClicked(int row, int rank)
		{
			if (this.CellClicked != null)
			{
				this.CellClicked (this, row, rank);
			}
		}

		public delegate void CellClickedEventHandler(object sender, int row, int rank);
		public event CellClickedEventHandler CellClicked;


		private void OnCellDoubleClicked(int row, int rank)
		{
			if (this.CellDoubleClicked != null)
			{
				this.CellDoubleClicked (this, row, rank);
			}
		}

		public delegate void CellDoubleClickedEventHandler(object sender, int row, int rank);
		public event CellDoubleClickedEventHandler CellDoubleClicked;


		private void OnContentChanged()
		{
			if (this.ContentChanged != null)
			{
				this.ContentChanged (this);
			}
		}

		public delegate void ContentChangedEventHandler(object sender);
		public event ContentChangedEventHandler ContentChanged;
		#endregion


		private Timeline timeline;
		private HScroller scroller;
		private int cellsCount;
	}
}