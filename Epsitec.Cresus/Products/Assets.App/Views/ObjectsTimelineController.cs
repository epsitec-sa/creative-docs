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
				return (cell.HasValue && cell.Value.TimelineGlyph != TimelineGlyph.Empty);
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


		public int? GetEventIndex(System.DateTime? dateTime)
		{
			if (dateTime.HasValue)
			{
				int count = this.timelineData.CellsCount;
				for (int i = 0; i < count; i++)
				{
					var cell = this.timelineData.GetCell (i);

					if (cell.HasValue && cell.Value.Timestamp.Date == dateTime.Value)
					{
						return i;
					}
				}
			}

			return null;
		}

		public int? GetEventIndex(Timestamp? timestamp)
		{
			if (timestamp.HasValue)
			{
				int count = this.timelineData.CellsCount;
				for (int i = 0; i < count; i++)
				{
					var cell = this.timelineData.GetCell (i);

					if (cell.HasValue && cell.Value.Timestamp == timestamp)
					{
						return i;
					}
				}
			}

			return null;
		}


		public int?								FirstEventIndex
		{
			get
			{
				int count = this.timelineData.CellsCount;
				for (int i = 0; i < count; i++)
				{
					var cell = this.timelineData.GetCell (i);

					if (cell.HasValue && cell.Value.TimelineGlyph != TimelineGlyph.Empty)
					{
						return i;
					}
				}

				return null;
			}
		}

		public int?								PrevEventIndex
		{
			get
			{
				if (this.selectedCell != -1)
				{
					int i = this.selectedCell - 1;
					while (i >= 0)
					{
						var cell = this.timelineData.GetCell (i);

						if (cell.HasValue && cell.Value.TimelineGlyph != TimelineGlyph.Empty)
						{
							return i;
						}

						i--;
					}
				}

				return null;
			}
		}

		public int?								NextEventIndex
		{
			get
			{
				if (this.selectedCell != -1)
				{
					int count = this.timelineData.CellsCount;
					int i = this.selectedCell + 1;
					while (i < count)
					{
						var cell = this.timelineData.GetCell (i);

						if (cell.HasValue && cell.Value.TimelineGlyph != TimelineGlyph.Empty)
						{
							return i;
						}

						i++;
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

					if (cell.HasValue && cell.Value.TimelineGlyph != TimelineGlyph.Empty)
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

					if (cell.HasValue && cell.Value.Timestamp == now)
					{
						return i;
					}
				}

				return null;
			}
		}


		public void Update()
		{
			this.UpdateRows ();
			this.UpdateTimelineData ();
			this.UpdateTimelineController ();
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

			this.controller.CreateUI (this.frameBox);
			this.controller.Pivot = 0.0;

			//	Pour que le calcul du nombre de cellules visibles soit correct.
			parent.Window.ForceLayout ();

			this.UpdateRows ();
			this.UpdateTimelineData ();
			this.UpdateTimelineController ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateTimelineController (crop);
			};

			this.controller.CellClicked += delegate (object sender, int row, int rank)
			{
				if (row == this.GlyphRow)
				{
					this.SelectedCell = this.controller.LeftVisibleCell + rank;
				}
			};

			this.controller.CellDoubleClicked += delegate (object sender, int row, int rank)
			{
				if (row == this.GlyphRow)
				{
					int sel = this.controller.LeftVisibleCell + rank;
					this.OnStartEdition (sel);
				}
			};
		}


		private void UpdateRows()
		{
			this.frameBox.PreferredHeight = this.RequiredHeight;

			this.controller.RelativeWidth = this.IsExtended ? 1.0 : 2.0;
			this.controller.ShowLabels = this.IsShowLabels;
			this.controller.SetRows (this.TimelineRows);

			this.frameBox.Window.ForceLayout ();
		}

		private int RequiredHeight
		{
			get
			{
				const int lineHeight = 18;

				int h = (int) AbstractScroller.DefaultBreadth + lineHeight*3;

				if (this.HasGraph)
				{
					h += lineHeight*2;
				}

				if (this.IsWeeksOfYear)
				{
					h += lineHeight*1;
				}

				if (this.IsDaysOfWeek)
				{
					h += lineHeight*1;
				}

				return h;
			}
		}


		private TimelineRowDescription[] TimelineRows
		{
			//	Retourne les descriptions des lignes, de bas en haut.
			get
			{
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
		}


		private void UpdateTimelineData()
		{
			var start = new System.DateTime (this.accessor.StartDate.Year, 1, 1);
			var end   = new System.DateTime (this.accessor.StartDate.Year+1, 12, 31);

			this.timelineData.Compute (this.objectGuid, this.timelineMode, start, end);

			this.controller.CellsCount = this.timelineData.CellsCount;
		}


		private void UpdateTimelineController(bool crop = true)
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

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstCell || selection >= firstCell+count))
				{
					firstCell = this.controller.GetLeftVisibleCell (selection);
				}

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
