//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class AssetsToolbarTimelineController : IDirty, System.IDisposable
	{
		public AssetsToolbarTimelineController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
		{
			this.accessor       = accessor;
			this.commandContext = commandContext;
			this.baseType       = baseType;

			this.timelineData = new TimelineData (this.accessor, this.baseType);
			this.timelineMode = TimelineMode.Compacted;

			this.commandDispatcher = new CommandDispatcher (this.GetType ().FullName, CommandDispatcherLevel.Primary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandDispatcher.RegisterController (this);  // nécesaire pour [Command (Res.CommandIds...)]
		}

		public void Dispose()
		{
			if (this.toolbar != null)
			{
				this.toolbar.Dispose ();
			}

			this.commandDispatcher.Dispose ();
		}


		#region IDirty Members
		public bool DirtyData
		{
			get;
			set;
		}
		#endregion

	
		public void CreateUI(Widget parent)
		{
			this.toolbar = new TimelineToolbar (this.accessor, this.commandContext);
			this.toolbar.CreateUI (parent);

			this.CreateTimeline (parent);
		}

		[Command (Res.CommandIds.Timeline.Labels)]
		private void OnLabels()
		{
			this.ChangeMode (TimelineMode.Labels);
		}

		[Command (Res.CommandIds.Timeline.Compacted)]
		private void OnCompacted()
		{
			this.ChangeMode (TimelineMode.Compacted);
		}

		[Command (Res.CommandIds.Timeline.Expanded)]
		private void OnExtended()
		{
			this.ChangeMode (TimelineMode.Expanded);
		}

		[Command (Res.CommandIds.Timeline.WeeksOfYear)]
		private void OnWeeksOfYear()
		{
			this.ChangeMode (TimelineMode.WeeksOfYear);
		}

		[Command (Res.CommandIds.Timeline.DaysOfWeek)]
		private void OnDaysOfWeek()
		{
			this.ChangeMode (TimelineMode.DaysOfWeek);
		}

		[Command (Res.CommandIds.Timeline.Graph)]
		private void OnGraph()
		{
			this.ChangeMode (TimelineMode.Graph);
		}

		[Command (Res.CommandIds.Timeline.First)]
		private void OnFirst()
		{
			var index = this.FirstEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
				this.controller.SetFocus ();
			}
		}

		[Command (Res.CommandIds.Timeline.Prev)]
		private void OnPrev()
		{
			var index = this.PrevEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
				this.controller.SetFocus ();
			}
		}

		[Command (Res.CommandIds.Timeline.Next)]
		private void OnNext()
		{
			var index = this.NextEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
				this.controller.SetFocus ();
			}
		}

		[Command (Res.CommandIds.Timeline.Last)]
		private void OnLast()
		{
			var index = this.LastEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
				this.controller.SetFocus ();
			}
		}

		[Command (Res.CommandIds.Timeline.Now)]
		private void OnNow()
		{
			var index = this.NowEventIndex;

			if (index.HasValue)
			{
				this.SelectedCell = index.Value;
				this.controller.SetFocus ();
			}
		}

		[Command (Res.CommandIds.Timeline.Date)]
		private void OnDate(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var popup = new DatePopup (this.accessor)
			{
				Date = this.selectedTimestamp.HasValue ? this.selectedTimestamp.Value.Date : Timestamp.Now.Date,
			};

			var target = this.toolbar.GetTarget (e);
			popup.Create (target, leftOrRight: false);

			popup.DateChanged += delegate
			{
				if (popup.Date.HasValue)
				{
					this.UpdateData (popup.Date);
					this.SelectedTimestamp = new Timestamp (popup.Date.Value, 0);
				}
			};
		}

		[Command (Res.CommandIds.Timeline.New)]
		private void OnNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			var timestamp = this.SelectedTimestamp;

			if (target != null && timestamp.HasValue)
			{
				CreateEventPopup.Show (target, this.accessor, this.baseType, this.obj, timestamp.Value,
				timestampChanged: delegate (Timestamp? t)
				{
					this.SelectedTimestamp = t;
				},
				action: delegate (System.DateTime date, string name)
				{
					this.CreateEvent (date, name);
				});
			}
		}

		[Command (Res.CommandIds.Timeline.Delete)]
		private void OnDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			if (AssetCalculator.IsLocked (this.obj, this.SelectedTimestamp.GetValueOrDefault ()))
			{
				MessagePopup.ShowAssetsDeleteEventWarning (target);
			}
			else
			{
				YesNoPopup.ShowAssetsDeleteEventQuestion (target, delegate
				{
					this.accessor.UndoManager.Start ();
					var desc = UndoManager.GetDescription (Res.Commands.Timeline.Delete.Description, AssetsLogic.GetSummary (this.accessor, this.obj.Guid, this.SelectedTimestamp));
					this.accessor.UndoManager.SetDescription (desc);

					this.accessor.RemoveObjectEvent (this.obj, this.SelectedTimestamp);
					this.UpdateData ();
					this.OnDeepUpdate ();

					this.accessor.UndoManager.SetAfterViewState ();
				});
			}
		}

		[Command (Res.CommandIds.Timeline.Deselect)]
		private void OnDeselect()
		{
			this.SelectedTimestamp = null;
		}

		[Command (Res.CommandIds.Timeline.Copy)]
		private void OnCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			if (this.obj != null && this.selectedTimestamp.HasValue)
			{
				var ev = this.obj.GetEvent (this.selectedTimestamp.Value);
				this.accessor.Clipboard.CopyEvent (this.accessor, ev);

				this.UpdateToolbar ();
			}
			else
			{
				MessagePopup.ShowError (target, Res.Strings.ToolbarControllers.AssetsTimeline.Copy.EmptySelection.ToString ());
			}
		}

		[Command (Res.CommandIds.Timeline.Paste)]
		private void OnPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			if (this.obj != null && this.accessor.Clipboard.HasEvent)
			{
				EventPastePopup.Show (target, this.accessor, this.obj,
				this.accessor.Clipboard.EventType,
				this.accessor.Clipboard.EventTimestamp.Value.Date,
				dateChanged: delegate (System.DateTime? date)
				{
					if (date.HasValue)
					{
						this.SelectedTimestamp = new Timestamp (date.Value, 0);
					}
					else
					{
						this.SelectedTimestamp = null;
					}
				},
				action: delegate (System.DateTime date)
				{
					this.accessor.UndoManager.Start ();

					var ev = this.accessor.Clipboard.PasteEvent (this.accessor, this.obj, date);

					if (ev == null)
					{
						MessagePopup.ShowError (target, Res.Strings.ToolbarControllers.AssetsTimeline.Paste.Wrong.ToString ());
					}
					else
					{
						this.UpdateData ();
						this.SelectedTimestamp = ev.Timestamp;
						this.OnStartEditing (ev.Type, ev.Timestamp);
						this.OnDeepUpdate ();
					}

					var desc = UndoManager.GetDescription (Res.Commands.Timeline.Paste.Description, AssetsLogic.GetSummary (this.accessor, this.obj.Guid, ev.Timestamp));
					this.accessor.UndoManager.SetDescription (desc);
					this.accessor.UndoManager.SetAfterViewState ();
				});
			}
			else
			{
				MessagePopup.ShowError (target, Res.Strings.ToolbarControllers.AssetsTimeline.Paste.Empty.ToString ());
			}
		}


		public void UpdateData()
		{
			using (new SaveCurrentDate (this))
			{
				this.UpdateData (this.CurrentDate);
				this.UpdateController ();
				this.UpdateToolbar ();
			}
		}


		private void ChangeMode(TimelineMode mode)
		{
			var t = this.TimelineMode;

			if (mode == TimelineMode.Compacted)
			{
				t |=  TimelineMode.Compacted;
				t &= ~TimelineMode.Expanded;
			}
			else if (mode == TimelineMode.Expanded)
			{
				t |=  TimelineMode.Expanded;
				t &= ~TimelineMode.Compacted;
			}
			else
			{
				t ^= mode;
			}

			this.TimelineMode = t;
			this.UpdateToolbar ();
		}

		private TimelineMode					TimelineMode
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
						this.UpdateData (this.CurrentDate);
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
						this.UpdateData (this.CurrentDate);
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


		private void CreateEvent(System.DateTime date, string buttonName)
		{
			var guid = this.objectGuid;

			if (!guid.IsEmpty)
			{
				this.accessor.UndoManager.Start ();

				var type = AssetsToolbarTimelineController.ParseEventType (buttonName);
				var e = this.accessor.CreateAssetEvent (this.obj, date, type);

				if (e != null)
				{
					this.UpdateData ();
					this.SelectedTimestamp = e.Timestamp;
					this.OnStartEditing (type, e.Timestamp);
					this.OnDeepUpdate ();
				}

				var desc = UndoManager.GetDescription (Res.Commands.Timeline.New.Description, AssetsLogic.GetSummary (this.accessor, this.obj.Guid, e.Timestamp));
				this.accessor.UndoManager.SetDescription (desc);
				this.accessor.UndoManager.SetAfterViewState ();
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

			CommandDispatcher.SetDispatcher (this.frameBox, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]

			this.controller.CreateUI (this.frameBox);
			this.controller.Pivot = 0.0;

			//	Pour que le calcul du nombre de cellules visibles soit correct.
			parent.Window.ForceLayout ();

			this.UpdateRows ();
			this.UpdateData (this.CurrentDate);
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

			this.controller.CellRightClicked += delegate (object sender, int row, int rank, Point pos)
			{
				if (row == this.GlyphRow)
				{
					this.SelectedCell = this.controller.LeftVisibleCell + rank;
					this.ShowContextMenu (pos);
				}
			};

			this.controller.DokeySelect += delegate (object sender, KeyCode key)
			{
				this.OnDokeySelect (key);
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
					h += lineHeight*3;
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
					list.Add (new TimelineRowDescription (TimelineRowType.Values, Res.Strings.ToolbarControllers.AssetsTimeline.Row.Values.ToString (), relativeHeight: 3.0));
				}

				list.Add (new TimelineRowDescription (TimelineRowType.Glyph, Res.Strings.ToolbarControllers.AssetsTimeline.Row.Events.ToString ()));

				if (this.IsDaysOfWeek)
				{
					list.Add (new TimelineRowDescription (TimelineRowType.DaysOfWeek, ""));
				}

				if (this.IsExpanded)
				{
					list.Add (new TimelineRowDescription (TimelineRowType.Days, Res.Strings.ToolbarControllers.AssetsTimeline.Row.Days.ToString ()));
				}
				else
				{
					list.Add (new TimelineRowDescription (TimelineRowType.DaysMonths, Res.Strings.ToolbarControllers.AssetsTimeline.Row.DaysMonths.ToString ()));
				}

				if (this.IsWeeksOfYear)
				{
					list.Add (new TimelineRowDescription (TimelineRowType.WeekOfYear, Res.Strings.ToolbarControllers.AssetsTimeline.Row.WeekOfYear.ToString ()));
				}

				if (this.IsExpanded)
				{
					list.Add (new TimelineRowDescription (TimelineRowType.Months, Res.Strings.ToolbarControllers.AssetsTimeline.Row.Months.ToString ()));
				}
				else
				{
					list.Add (new TimelineRowDescription (TimelineRowType.Years, Res.Strings.ToolbarControllers.AssetsTimeline.Row.Years.ToString ()));
				}

				return list.ToArray ();
			}
		}


		private void UpdateData(System.DateTime? forcedDate)
		{
			var start = this.accessor.StartDate;
			var end   = this.accessor.StartDate.AddYears (100);

			this.timelineData.Compute (this.objectGuid, this.timelineMode, start, end, forcedDate);

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
					var g = new TimelineCellGlyph (cell.Value.Glyph, cell.Value.Flags, cell.Value.Tooltip, isSelected: (i == selection));
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
				this.controller.SetRowValueCells (line++, values.ToArray (), min, max, this.timelineData.ValuesFieldNames);
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


		private void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			MenuPopup.Show (this.toolbar, this.frameBox, pos,
				Res.Commands.Timeline.New,
				Res.Commands.Timeline.Delete,
				null,
				Res.Commands.Timeline.Copy,
				Res.Commands.Timeline.Paste);
		}

		private void OnDokeySelect(KeyCode key)
		{
			switch (key)
			{
				case KeyCode.Home:
					this.OnFirst ();
					break;
		
				case KeyCode.ArrowLeft:
				case KeyCode.PageUp:
					this.OnPrev ();
					break;
		
				case KeyCode.ArrowRight:
				case KeyCode.PageDown:
					this.OnNext ();
					break;
		
				case KeyCode.End:
					this.OnLast ();
					break;
			}
		}


		private void UpdateToolbar()
		{
			int sel = this.SelectedCell;

			this.toolbar.SetActiveState (Res.Commands.Timeline.Labels,      (this.timelineMode & TimelineMode.Labels     ) != 0);
			this.toolbar.SetActiveState (Res.Commands.Timeline.Compacted,   (this.timelineMode & TimelineMode.Compacted  ) != 0);
			this.toolbar.SetActiveState (Res.Commands.Timeline.Expanded,    (this.timelineMode & TimelineMode.Expanded   ) != 0);
			this.toolbar.SetActiveState (Res.Commands.Timeline.WeeksOfYear, (this.timelineMode & TimelineMode.WeeksOfYear) != 0);
			this.toolbar.SetActiveState (Res.Commands.Timeline.DaysOfWeek,  (this.timelineMode & TimelineMode.DaysOfWeek ) != 0);
			this.toolbar.SetActiveState (Res.Commands.Timeline.Graph,       (this.timelineMode & TimelineMode.Graph      ) != 0);

			this.UpdateCommand (Res.Commands.Timeline.First, sel, this.FirstEventIndex);
			this.UpdateCommand (Res.Commands.Timeline.Prev,  sel, this.PrevEventIndex);
			this.UpdateCommand (Res.Commands.Timeline.Next,  sel, this.NextEventIndex);
			this.UpdateCommand (Res.Commands.Timeline.Last,  sel, this.LastEventIndex);
			this.UpdateCommand (Res.Commands.Timeline.Now,   sel, this.NowEventIndex);

			this.toolbar.SetEnable (Res.Commands.Timeline.Date, true);

			this.toolbar.SetEnable (Res.Commands.Timeline.New,    !this.objectGuid.IsEmpty && this.SelectedTimestamp.HasValue);
			this.toolbar.SetEnable (Res.Commands.Timeline.Delete, this.HasSelectedEvent);

			this.toolbar.SetEnable (Res.Commands.Timeline.Deselect, sel != -1);
			this.toolbar.SetEnable (Res.Commands.Timeline.Copy,     this.HasSelectedEvent);
			this.toolbar.SetEnable (Res.Commands.Timeline.Paste,    this.accessor.Clipboard.HasEvent);
		}

		private void UpdateCommand(Command command, int selectedCell, int? newSelection)
		{
			bool enable = (newSelection.HasValue && selectedCell != newSelection.Value);
			this.toolbar.SetEnable (command, enable);
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
			public SaveCurrentDate(AssetsToolbarTimelineController controller)
			{
				this.controller = controller;
				this.currentDate = this.controller.CurrentDate;
			}

			public void Dispose()
			{
				this.controller.CurrentDate = this.currentDate;
			}

			private readonly AssetsToolbarTimelineController	controller;
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


		private void OnStartEditing(EventType eventType, Timestamp timestamp)
		{
			this.StartEditing.Raise (this, eventType, timestamp);
		}

		public event EventHandler<EventType, Timestamp> StartEditing;


		private void OnDeepUpdate()
		{
			this.DeepUpdate.Raise (this);
		}

		public event EventHandler DeepUpdate;
		#endregion


		private readonly DataAccessor			accessor;
		private readonly CommandDispatcher		commandDispatcher;
		private readonly CommandContext			commandContext;
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
