//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsTimelineController
	{
		public ObjectsTimelineController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.timelineData = new TimelineData (this.accessor);
			this.timelineMode = TimelineMode.Extended;
		}

		public void CreateUI(Widget parent)
		{
			this.CreateTimeline (parent);
		}


		public TimelineMode						TimelineMode
		{
			get
			{
				return this.timelineMode;
			}
			set
			{
				if (this.timelineMode != value)
				{
					this.timelineMode = value;

					this.UpdateRows ();
					this.UpdateTimelineData ();
					this.UpdateTimelineController ();
				}
			}
		}

		public int								SelectedCell
		{
			get
			{
				return this.selectedCell;
			}
			set
			{
				if (this.selectedCell != value)
				{
					this.selectedCell = value;

					this.UpdateTimelineController ();
					this.OnCellClicked (this.selectedCell);
				}
			}
		}

		public Timestamp?						SelectedTimestamp
		{
			get
			{
				if (this.selectedCell != -1 && this.controller != null)
				{
					var cell = this.timelineData.GetCell (this.selectedCell);

					if (cell.HasValue)
					{
						return cell.Value.Timestamp;
					}
				}

				return null;
			}
		}

		public bool								HasSelectedEvent
		{
			get
			{
				var cell = this.timelineData.GetCell (this.selectedCell);
				return (cell.HasValue && cell.Value.TimelineGlyph == TimelineGlyph.Empty);
			}
		}

		public Guid								ObjectGuid
		{
			get
			{
				return this.objectGuid;
			}
			set
			{
				if (this.objectGuid != value)
				{
					this.objectGuid = value;

					this.UpdateTimelineData ();
					this.UpdateTimelineController ();
				}
			}
		}


		public int?								FirstEventIndex
		{
			get
			{
				int count = this.timelineData.CellsCount;
				for (int i = 0; i < count; i++)
				{
					var cell = this.timelineData.GetCell (i);

					if (cell.Value.TimelineGlyph != TimelineGlyph.Empty)
					{
						return i;
					}
				}

				return null;
			}
		}

		public int?								LastEventIndex
		{
			get
			{
				int count = this.timelineData.CellsCount;
				for (int i = count-1; i >= 0; i--)
				{
					var cell = this.timelineData.GetCell (i);

					if (cell.Value.TimelineGlyph != TimelineGlyph.Empty)
					{
						return i;
					}
				}

				return null;
			}
		}

		public int?								NowEventIndex
		{
			get
			{
				var now = Timestamp.Now;

				int count = this.timelineData.CellsCount;
				for (int i = 0; i < count; i++)
				{
					var cell = this.timelineData.GetCell (i);

					if (cell.Value.Timestamp == now)
					{
						return i;
					}
				}

				return null;
			}
		}


		private void CreateTimeline(Widget parent)
		{
			this.selectedCell = -1;

			this.controller = new NavigationTimelineController ();

			this.frameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.controller.DateChanged += delegate
			{
				this.UpdateTimelineController ();
			};

			this.controller.CreateUI (this.frameBox);
			this.controller.RelativeWidth = this.IsExtended ? 1.0 : 2.0;
			this.controller.ShowLabels = this.IsShowLabels;
			this.controller.Pivot = 0.0;

			this.UpdateRows ();
			this.UpdateTimelineData ();
			this.UpdateTimelineController ();

			//	Connexion des événements.
			this.controller.CellClicked += delegate (object sender, int row, int rank)
			{
				if (row == this.GlyphRow)
				{
					int sel = this.controller.LeftVisibleCell + rank;

					if (this.selectedCell == sel)
					{
						this.OnStartEdition (sel);
					}
					else
					{
						this.SelectedCell = sel;
					}
				}
			};
		}


		private void UpdateRows()
		{
			this.frameBox.PreferredHeight = this.Height;

			this.controller.SetRows (this.GetTimelineRows ());
		}

		private int Height
		{
			get
			{
				int height = (int) AbstractScroller.DefaultBreadth + 18*3;

				if (this.HasGraph)
				{
					height += 18*2;
				}

				if (this.IsWeeksOfYear)
				{
					height += 18*1;
				}

				if (this.IsDaysOfWeek)
				{
					height += 18*1;
				}

				return height;
			}
		}


		private TimelineRowDescription[] GetTimelineRows()
		{
			//	Retourne les descriptions des lignes, de bas en haut.
			var list = new List<TimelineRowDescription> ();

			if (this.HasGraph)
			{
				list.Add (new TimelineRowDescription (TimelineRowType.Value, "Valeurs", relativeHeight: 2.0));
			}

			list.Add (new TimelineRowDescription (TimelineRowType.Glyph, "Evénements"));

			if (this.IsDaysOfWeek)
			{
				list.Add (new TimelineRowDescription (TimelineRowType.DaysOfWeek, ""));
			}

			if (this.IsExtended)
			{
				list.Add (new TimelineRowDescription (TimelineRowType.Days, "Jour"));
			}
			else
			{
				list.Add (new TimelineRowDescription (TimelineRowType.DaysMonths, "Jour"));
			}

			if (this.IsWeeksOfYear)
			{
				list.Add (new TimelineRowDescription (TimelineRowType.WeekOfYear, "Semaine"));
			}

			if (this.IsExtended)
			{
				list.Add (new TimelineRowDescription (TimelineRowType.Months, "Mois"));
			}
			else
			{
				list.Add (new TimelineRowDescription (TimelineRowType.Years, "Année"));
			}

			return list.ToArray ();
		}


		private void UpdateTimelineData()
		{
			var start = new System.DateTime (this.accessor.StartDate.Year, 1, 1);
			var end   = new System.DateTime (this.accessor.StartDate.Year+1, 12, 31);

			this.timelineData.Compute (this.objectGuid, this.timelineMode, start, end);

			this.controller.CellsCount = this.timelineData.CellsCount;
		}


		private void UpdateTimelineController()
		{
			int visibleCount = this.controller.VisibleCellsCount;
			int cellsCount   = this.timelineData.CellsCount;
			int count        = System.Math.Min (visibleCount, cellsCount);
			int firstCell    = this.controller.LeftVisibleCell;
			int selection    = this.selectedCell;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de cellules.
				selection = System.Math.Min (selection, cellsCount-1);

				//	Si la sélection est avant la zone visible, on recule firstCell.
				firstCell = System.Math.Min (firstCell, selection);

				//	Si la sélection est après la zone visible, on avance firstCell.
				firstCell = System.Math.Max (firstCell, selection-count+1);

				if (this.controller.LeftVisibleCell != firstCell)
				{
					this.controller.LeftVisibleCell = firstCell;
				}

				selection -= this.controller.LeftVisibleCell;
			}

			var dates  = new List<TimelineCellDate> ();
			var glyphs = new List<TimelineCellGlyph> ();
			var values = new List<TimelineCellValue> ();

			if (firstCell > 0)
			{
				//	S'il existe une cellule précédente, cachée à gauche, il est nécessaire
				//	de la donner, pour que le dessin de l'origine du graphique soit correct.
				var cell = this.timelineData.GetSyntheticCell (firstCell-1);
				var v = new TimelineCellValue (-1, cell.Value.Values);
				values.Add (v);
			}

			for (int i = 0; i < count; i++)
			{
				var cell = this.timelineData.GetCell (firstCell+i);

				if (cell == null)
				{
					break;
				}
				else
				{
					var d = new TimelineCellDate (cell.Value.Timestamp.Date, isSelected: (i == selection));
					var g = new TimelineCellGlyph (cell.Value.TimelineGlyph, isSelected: (i == selection));
					var v = new TimelineCellValue (i, cell.Value.Values, isSelected: (i == selection));

					dates.Add (d);
					glyphs.Add (g);
					values.Add (v);
				}
			}

			int line = 0;

			if (this.HasGraph)
			{
				decimal min, max;
				this.timelineData.GetMinMax (out min, out max);
				this.controller.SetRowValueCells (line++, values.ToArray (), min, max);
			}

			this.controller.SetRowGlyphCells (line++, glyphs.ToArray ());

			if (this.IsDaysOfWeek)
			{
				this.controller.SetRowDayOfWeekCells (line++, dates.ToArray ());
			}

			if (this.IsExtended)
			{
				this.controller.SetRowDayCells (line++, dates.ToArray ());
			}
			else
			{
				this.controller.SetRowDayMonthCells (line++, dates.ToArray ());
			}

			if (this.IsWeeksOfYear)
			{
				this.controller.SetRowWeekOfYearCells (line++, dates.ToArray ());
			}

			if (this.IsExtended)
			{
				this.controller.SetRowMonthCells (line++, dates.ToArray ());
			}
			else
			{
				this.controller.SetRowYearCells (line++, dates.ToArray ());
			}
		}


		private bool IsShowLabels
		{
			get
			{
				return (this.timelineMode & TimelineMode.Labels) != 0;
			}
		}

		private bool IsWeeksOfYear
		{
			get
			{
				return (this.timelineMode & TimelineMode.WeeksOfYear) != 0;
			}
		}

		private bool IsDaysOfWeek
		{
			get
			{
				return (this.timelineMode & TimelineMode.DaysOfWeek) != 0;
			}
		}

		private bool IsExtended
		{
			get
			{
				return (this.timelineMode & TimelineMode.Extended) != 0;
			}
		}

		private int GlyphRow
		{
			get
			{
				return this.HasGraph ? 1 : 0;
			}
		}

		private bool HasGraph
		{
			get
			{
				return (this.timelineMode & TimelineMode.Graph) != 0;
			}
		}


		#region Events handler
		private void OnCellClicked(int cell)
		{
			if (this.CellClicked != null)
			{
				this.CellClicked (this, cell);
			}
		}

		public delegate void CellClickedEventHandler(object sender, int cell);
		public event CellClickedEventHandler CellClicked;


		private void OnStartEdition(int row)
		{
			if (this.StartEdition != null)
			{
				this.StartEdition (this, row);
			}
		}

		public delegate void StartEditionEventHandler(object sender, int row);
		public event StartEditionEventHandler StartEdition;
		#endregion


		private readonly DataAccessor			accessor;
		private readonly TimelineData			timelineData;

		private FrameBox						frameBox;
		private NavigationTimelineController	controller;
		private int								selectedCell;
		private TimelineMode					timelineMode;
		private Guid							objectGuid;
	}
}
