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

			this.scroller.ValueChanged += delegate
			{
				this.OnDateChanged ();
			};

			this.UpdateScroller ();
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
		}


		#region Timeline Facade
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

		public IEnumerable<AbstractTimelineRow> Rows
		{
			get
			{
				return this.timeline.Rows;
			}
		}

		public void SetRows(List<AbstractTimelineRow> rows)
		{
			this.timeline.SetRows (rows);
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

			this.scroller.Resolution = 1.0m;
			this.scroller.VisibleRangeRatio = System.Math.Min (visibleCells/totalCells, 1.0m);

			this.scroller.MinValue = 0.0m;
			this.scroller.MaxValue = System.Math.Max ((decimal) this.cellsCount - visibleCells, 0.0m);

			this.scroller.SmallChange = 1.0m;
			this.scroller.LargeChange = visibleCells;

			this.OnDateChanged ();  // met à jour la timeline
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
		#endregion


		private Timeline timeline;
		private HScroller scroller;
		private int cellsCount;
	}
}