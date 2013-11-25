//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsToolbarTimelineController
	{
		public ObjectsToolbarTimelineController(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.timelineData = new TimelineData (this.accessor, this.baseType);
			this.timelineMode = TimelineMode.Compacted;
		}


		public bool								DataFreezed;


		public void CreateUI(Widget parent)
		{
			this.toolbar = new TimelineToolbar ();
			this.toolbar.CreateUI (parent);

			this.CreateTimeline (parent);
			this.toolbar.TimelineMode = this.timelineMode;

			//	Connexion des événements.
			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.First:
						this.OnFirst ();
						break;

					case ToolbarCommand.Last:
						this.OnLast ();
						break;

					case ToolbarCommand.Prev:
						this.OnPrev ();
						break;

					case ToolbarCommand.Next:
						this.OnNext ();
						break;

					case ToolbarCommand.Now:
						this.OnNow ();
						break;

					case ToolbarCommand.New:
						this.OnNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnDelete ();
						break;

					case ToolbarCommand.Deselect:
						this.OnDeselect ();
						break;
				}
			};

			this.toolbar.ModeChanged += delegate
			{
				this.TimelineMode = this.toolbar.TimelineMode;
			};
		}

		public void Update()
		{
			using (new SaveCurrentDate (this))
			{
				this.UpdateData ();
				this.UpdateController ();
				this.UpdateToolbar ();
			}
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

					using (new SaveCurrentDate (this))
					{
						this.UpdateRows ();
						this.UpdateData ();
						this.UpdateController ();
						this.UpdateToolbar ();
					}
				}
			}
		}

		public Timestamp?						SelectedTimestamp
		{
			get
			{
				return this.selectedTimestamp;
			}
			set
			{
				if (this.selectedTimestamp != value)
				{
					this.selectedTimestamp = value;

					this.UpdateController ();
					this.UpdateToolbar ();

					this.OnSelectedCellChanged (this.SelectedCell);
				}
			}
		}

		public int								SelectedCell
		{
			get
			{
				return this.timelineData.GetCellIndex (this.selectedTimestamp);
			}
			set
			{
				var cell = this.timelineData[value];
				if (cell.HasValue)
				{
					this.SelectedTimestamp = cell.Value.Timestamp;
				}
				else
				{
					this.SelectedTimestamp = null;
				}
			}
		}

		public bool								HasSelectedEvent
		{
			get
			{
				var cell = this.timelineData[this.selectedTimestamp];
				return (cell.HasValue && cell.Value.Glyph != TimelineGlyph.Empty);
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
					this.obj = this.accessor.GetObject (this.baseType, this.objectGuid);

					using (new SaveCurrentDate (this))
					{
						this.UpdateData ();
						this.UpdateController ();
						this.UpdateToolbar ();
					}
				}
			}
		}


		public int GetEventIndex(System.DateTime? dateTime)
		{
			return this.timelineData.GetCellIndex (dateTime);
		}

		public int? GetEventIndex(Timestamp? timestamp)
		{
			if (timestamp.HasValue)
			{
				int count = this.timelineData.CellsCount;
				for (int i = 0; i < count; i++)
				{
					var cell = this.timelineData[i];

					if (cell.HasValue && cell.Value.Timestamp == timestamp)
					{
						return i;
					}
				}
			}

			return null;
		}


		private void OnFirst()
		{
			var index = this.FirstEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
			}
		}

		private void OnPrev()
		{
			var index = this.PrevEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
			}
		}

		private void OnNext()
		{
			var index = this.NextEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
			}
		}

		private void OnLast()
		{
			var index = this.LastEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
			}
		}

		private void OnNow()
		{
			var index = this.NowEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
			}
		}

		private void OnNew()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.New);
			var timestamp = this.SelectedTimestamp;

			if (target != null && timestamp.HasValue)
			{
				System.DateTime? createDate = timestamp.Value.Date;

				var popup = new NewEventPopup
				{
					BaseType   = this.baseType,
					DataObject = this.obj,
					Timestamp  = timestamp.Value,
				};

				popup.Create (target);

				popup.DateChanged += delegate (object sender, System.DateTime? dateTime)
				{
					if (dateTime.HasValue)
					{
						this.SelectedTimestamp = new Timestamp (dateTime.Value, 0);
					}
					else
					{
						this.SelectedTimestamp = null;
					}

					if (dateTime.HasValue)
					{
						createDate = dateTime.Value;
					}
				};

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (createDate.HasValue)
					{
						this.CreateEvent (createDate.Value, name);
					}
				};
			}
		}

		private void OnDelete()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous supprimer l'événement sélectionné ?",
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.accessor.RemoveObjectEvent (this.obj, this.SelectedTimestamp);
						this.Update ();
						this.OnUpdateAll ();
					}
				};
			}
		}

		private void OnDeselect()
		{
			this.SelectedTimestamp = null;
		}


		private void CreateEvent(System.DateTime date, string buttonName)
		{
			var guid = this.objectGuid;

			if (!guid.IsEmpty)
			{
				var type = ObjectsToolbarTimelineController.ParseEventType (buttonName);
				var e = this.accessor.CreateObjectEvent (this.obj, date, type);

				if (e != null)
				{
					this.Update ();
					this.SelectedTimestamp = e.Timestamp;
					this.OnStartEditing (type);
					this.OnUpdateAll ();
				}
			}
		}

		private static EventType ParseEventType(string text)
		{
			var type = EventType.Unknown;
			System.Enum.TryParse<EventType> (text, out type);
			return type;
		}

	

		private void CreateTimeline(Widget parent)
		{
			this.selectedTimestamp = null;

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
			this.UpdateData ();
			this.UpdateController ();
			this.UpdateToolbar ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
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
					this.OnCellDoubleClicked (sel);
				}
			};
		}


		private void UpdateRows()
		{
			this.frameBox.PreferredHeight = this.RequiredHeight;

			this.controller.RelativeWidth = this.IsExpanded ? 1.0 : 2.0;
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

				if (this.IsExpanded)
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

				if (this.IsExpanded)
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


		private void UpdateData()
		{
			var start = this.accessor.StartDate;
			var end   = this.accessor.EndDate;

			this.timelineData.Compute (this.objectGuid, this.timelineMode, start, end, this.CurrentDate);

			this.controller.CellsCount = this.timelineData.CellsCount;
		}


		private void UpdateController(bool crop = true)
		{
			int visibleCount = this.controller.VisibleCellsCount;
			int cellsCount   = this.timelineData.CellsCount;
			int count        = System.Math.Min (visibleCount, cellsCount);
			int firstCell    = this.controller.LeftVisibleCell;
			int selection    = this.SelectedCell;

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
				var cell = this.timelineData[firstCell+i];

				if (cell == null)
				{
					break;
				}
				else
				{
					var d = new TimelineCellDate (cell.Value.Timestamp.Date, isSelected: (i == selection));
					var g = new TimelineCellGlyph (cell.Value.Glyph, cell.Value.IsLocked, false, cell.Value.Tooltip, isSelected: (i == selection));
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

			if (this.IsExpanded)
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

			if (this.IsExpanded)
			{
				this.controller.SetRowMonthCells (line++, dates.ToArray ());
			}
			else
			{
				this.controller.SetRowYearCells (line++, dates.ToArray ());
			}
		}


		private void UpdateToolbar()
		{
			int sel = this.SelectedCell;

			this.UpdateCommand (ToolbarCommand.First, sel, this.FirstEventIndex);
			this.UpdateCommand (ToolbarCommand.Prev,  sel, this.PrevEventIndex);
			this.UpdateCommand (ToolbarCommand.Next,  sel, this.NextEventIndex);
			this.UpdateCommand (ToolbarCommand.Last,  sel, this.LastEventIndex);
			this.UpdateCommand (ToolbarCommand.Now,   sel, this.NowEventIndex);

			this.toolbar.UpdateCommand (ToolbarCommand.New, !this.objectGuid.IsEmpty && this.SelectedTimestamp.HasValue);
			this.toolbar.UpdateCommand (ToolbarCommand.Delete, this.HasSelectedEvent);

			this.toolbar.UpdateCommand (ToolbarCommand.Deselect, sel != -1);
		}

		private void UpdateCommand(ToolbarCommand command, int selectedCell, int? newSelection)
		{
			bool enable = (newSelection.HasValue && selectedCell != newSelection.Value);
			this.toolbar.UpdateCommand (command, enable);
		}


		private int? FirstEventIndex
		{
			get
			{
				if (this.PrevEventIndex.HasValue)
				{
					int count = this.timelineData.CellsCount;
					for (int i = 0; i < count; i++)
					{
						var cell = this.timelineData[i];

						if (cell.HasValue && cell.Value.Glyph != TimelineGlyph.Empty)
						{
							return i;
						}
					}
				}

				return null;
			}
		}

		private int? PrevEventIndex
		{
			get
			{
				if (this.SelectedCell != -1)
				{
					int i = this.SelectedCell - 1;
					while (i >= 0)
					{
						var cell = this.timelineData[i];

						if (cell.HasValue && cell.Value.Glyph != TimelineGlyph.Empty)
						{
							return i;
						}

						i--;
					}
				}

				return null;
			}
		}

		private int? NextEventIndex
		{
			get
			{
				if (this.SelectedCell != -1)
				{
					int count = this.timelineData.CellsCount;
					int i = this.SelectedCell + 1;
					while (i < count)
					{
						var cell = this.timelineData[i];

						if (cell.HasValue && cell.Value.Glyph != TimelineGlyph.Empty)
						{
							return i;
						}

						i++;
					}
				}

				return null;
			}
		}

		private int? LastEventIndex
		{
			get
			{
				if (this.NextEventIndex.HasValue)
				{
					int count = this.timelineData.CellsCount;
					for (int i = count-1; i >= 0; i--)
					{
						var cell = this.timelineData[i];

						if (cell.HasValue && cell.Value.Glyph != TimelineGlyph.Empty)
						{
							return i;
						}
					}
				}

				return null;
			}
		}

		private int? NowEventIndex
		{
			get
			{
				var now = Timestamp.Now;

				int count = this.timelineData.CellsCount;
				for (int i = 0; i < count; i++)
				{
					var cell = this.timelineData[i];

					if (cell.HasValue && cell.Value.Timestamp == now)
					{
						return i;
					}
				}

				return null;
			}
		}


		private class SaveCurrentDate : System.IDisposable
		{
			public SaveCurrentDate(ObjectsToolbarTimelineController controller)
			{
				this.controller = controller;
				this.currentDate = this.controller.CurrentDate;
			}

			public void Dispose()
			{
				this.controller.CurrentDate = this.currentDate;
			}

			private readonly ObjectsToolbarTimelineController	controller;
			private readonly System.DateTime?					currentDate;
		}

		private System.DateTime? CurrentDate
		{
			get
			{
				var currentTimestamp = this.SelectedTimestamp;

				if (currentTimestamp.HasValue)
				{
					return currentTimestamp.Value.Date;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (value.HasValue)
				{
					var index = this.GetEventIndex (value.Value);
					if (index != -1)
					{
						this.SelectedCell = index;
					}
				}
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

		private bool IsExpanded
		{
			get
			{
				return (this.timelineMode & TimelineMode.Expanded) != 0;
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
		private void OnSelectedCellChanged(int cell)
		{
			this.SelectedCellChanged.Raise (this, cell);
		}

		public event EventHandler<int> SelectedCellChanged;


		private void OnCellDoubleClicked(int row)
		{
			this.CellDoubleClicked.Raise (this, row);
		}

		public event EventHandler<int> CellDoubleClicked;


		private void OnStartEditing(EventType eventType)
		{
			this.StartEditing.Raise (this, eventType);
		}

		public event EventHandler<EventType> StartEditing;


		private void OnUpdateAll()
		{
			this.UpdateAll.Raise (this);
		}

		public event EventHandler UpdateAll;
		#endregion


		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;
		private readonly TimelineData			timelineData;

		private FrameBox						frameBox;
		private TimelineToolbar					toolbar;
		private NavigationTimelineController	controller;
		private	Timestamp?						selectedTimestamp;
		private TimelineMode					timelineMode;
		private Guid							objectGuid;
		private DataObject						obj;
	}
}
