//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsView : AbstractView
	{
		public ObjectsView(DataAccessor accessor)
			: base (accessor)
		{
			this.timelineData = new TimelineData (this.accessor);
			this.timelineMode = TimelineMode.Extended;
		}

		public override void CreateUI(Widget parent, MainToolbar toolbar)
		{
			base.CreateUI (parent, toolbar);

			this.timelineFrameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.treeTableToolbar = new TreeTableToolbar ();
			this.treeTableToolbar.CreateUI (this.listFrameBox);

			this.timelineToolbar = new TimelineToolbar ();
			this.timelineToolbar.CreateUI (this.timelineFrameBox);
			this.timelineToolbar.TimelineMode = this.timelineMode;

			this.editToolbar = new EditToolbar ();
			this.editToolbar.CreateUI (this.editFrameBox);

			this.CreateTreeTable (this.listFrameBox);
			this.CreateTimeline (this.timelineFrameBox);

			this.Update ();

			// provisoire:
			this.editToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Disable);
			this.editToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);

			this.treeTableToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.New:
						this.OnTreeTableNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnTreeTableDelete ();
						break;

					case ToolbarCommand.Edit:
						this.OnTreeTableEdit ();
						break;

					case ToolbarCommand.Deselect:
						this.OnTreeTableDeselect ();
						break;
				}
			};

			this.timelineToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.First:
						this.OnTimelineFirst ();
						break;

					case ToolbarCommand.New:
						this.OnTimelineNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnTimelineDelete ();
						break;

					case ToolbarCommand.Deselect:
						this.OnTimelineDeselect ();
						break;
				}
			};

			this.editToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.Accept:
						this.OnEditAccept ();
						break;

					case ToolbarCommand.Cancel:
						this.OnEditCancel ();
						break;
				}
			};

			this.timelineToolbar.ModeChanged += delegate
			{
				this.timelineMode = this.timelineToolbar.TimelineMode;

				this.UpdateTimelineData ();
				this.UpdateTimelineController ();
			};
		}


		protected override string Title
		{
			get
			{
				return "Objets d'immobilisation";
			}
		}


		protected void OnTreeTableNew()
		{
		}

		protected void OnTreeTableDelete()
		{
		}

		protected void OnTreeTableEdit()
		{
			this.isEditing = true;
			this.Update ();
		}

		protected void OnTreeTableDeselect()
		{
			this.SelectedRow = -1;
			this.Update ();
		}

		protected void OnTimelineFirst()
		{
			this.timelineSelectedCell = this.FirstTimelineEventIndex;

			this.UpdateTimelineController ();
			this.UpdateTreeTableController ();
			this.UpdateTimelineToolbar ();
		}

		protected void OnTimelineNew()
		{
		}

		protected void OnTimelineDelete()
		{
		}

		protected void OnTimelineDeselect()
		{
			this.timelineSelectedCell = -1;

			this.UpdateTimelineController ();
			this.UpdateTreeTableController ();
			this.UpdateTimelineToolbar ();
		}

		protected void OnEditAccept()
		{
			this.isEditing = false;
			this.Update ();
		}

		protected void OnEditCancel()
		{
			this.isEditing = false;
			this.Update ();
		}



		protected override void Update()
		{
			base.Update ();

			this.UpdateTreeTableToolbar ();
			this.UpdateTimelineToolbar ();
		}

		protected override int SelectedRow
		{
			get
			{
				return this.treeTableSelectedRow;
			}
			set
			{
				if (this.treeTableSelectedRow != value)
				{
					this.treeTableSelectedRow = value;

					this.UpdateTreeTableController ();

					this.UpdateTimelineData ();
					this.UpdateTimelineController ();

					this.Update ();
				}
			}
		}


		#region TreeTable
		private void CreateTreeTable(Widget parent)
		{
			this.treeTableRowsCount = this.accessor.ObjectsCount;
			this.treeTableSelectedRow = -1;

			this.treeTableController = new NavigationTreeTableController
			{
				RowsCount = treeTableRowsCount,
			};

			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.treeTableController.CreateUI (frame, footerHeight: 0);
			this.treeTableController.SetColumns (ObjectsView.GetTreeTableColumns (), 1);
			this.UpdateTreeTableController ();

			this.treeTableController.RowChanged += delegate
			{
				this.UpdateTreeTableController ();
			};

			this.treeTableController.RowClicked += delegate (object sender, int column, int row)
			{
				int sel = this.treeTableController.TopVisibleRow + row;

				if (this.treeTableSelectedRow == sel)
				{
					this.OnTreeTableEdit ();
				}
				else
				{
					this.SelectedRow = sel;
				}
			};

			this.treeTableController.ContentChanged += delegate (object sender)
			{
				this.UpdateTreeTableController ();
			};

			this.treeTableController.TreeButtonClicked += delegate (object sender, int row, TreeTableTreeType type)
			{
			};
		}

		private void UpdateTreeTableController()
		{
			var first = this.treeTableController.TopVisibleRow;
			int selection = this.treeTableSelectedRow - this.treeTableController.TopVisibleRow;

			var timestamp = this.SelectedTimestamp;

			if (!timestamp.HasValue)
			{
				timestamp = new Timestamp (this.accessor.StartDate, 0);
			}

			this.InitialiseTreeTable (this.treeTableController, first, selection, timestamp.Value);
		}


		private static TreeTableColumnDescription[] GetTreeTableColumns()
		{
			var list = new List<TreeTableColumnDescription> ();

			list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,    200, "Objet"));
			list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,   50, "N°"));
			list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  120, "Responsable"));
			list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,   60, "Couleur"));
			list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  200, "Numéro de série"));
			list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, 120, "Valeur comptable"));
			list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, 120, "Valeur assurance"));

			return list.ToArray ();
		}

		private void InitialiseTreeTable(NavigationTreeTableController treeTable, int firstRow, int selection, Timestamp timestamp)
		{
			var cf = new List<TreeTableCellTree> ();
			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellString> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellString> ();
			var c5 = new List<TreeTableCellDecimal> ();
			var c6 = new List<TreeTableCellDecimal> ();

			var count = treeTable.VisibleRowsCount;
			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.accessor.ObjectsCount)
				{
					break;
				}

				var guid = this.accessor.GetObjectGuid (firstRow+i);
				var properties = this.accessor.GetObjectProperties (guid, timestamp);

				int level = DataAccessor.GetIntProperty (properties, (int) ObjectField.Level).GetValueOrDefault ();
				var type = level == 3 ? TreeTableTreeType.Final : TreeTableTreeType.Extended;

				var nom         = DataAccessor.GetStringProperty (properties, (int) ObjectField.Nom);
				var numéro      = DataAccessor.GetStringProperty (properties, (int) ObjectField.Numéro);
				var responsable = DataAccessor.GetStringProperty (properties, (int) ObjectField.Responsable);
				var couleur     = DataAccessor.GetStringProperty (properties, (int) ObjectField.Couleur);
				var série       = DataAccessor.GetStringProperty (properties, (int) ObjectField.NuméroSérie);
				var valeur1     = DataAccessor.GetDecimalProperty (properties, (int) ObjectField.Valeur1);
				var valeur2     = DataAccessor.GetDecimalProperty (properties, (int) ObjectField.Valeur2);

				var sf = new TreeTableCellTree (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString (true, numéro, isSelected: (i == selection));
				var s2 = new TreeTableCellString (true, responsable, isSelected: (i == selection));
				var s3 = new TreeTableCellString (true, couleur, isSelected: (i == selection));
				var s4 = new TreeTableCellString (true, série, isSelected: (i == selection));
				var s5 = new TreeTableCellDecimal (true, valeur1, isSelected: (i == selection));
				var s6 = new TreeTableCellDecimal (true, valeur2, isSelected: (i == selection));

				cf.Add (sf);
				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
				c6.Add (s6);
			}

			treeTable.SetColumnCells (0, cf.ToArray ());
			treeTable.SetColumnCells (1, c1.ToArray ());
			treeTable.SetColumnCells (2, c2.ToArray ());
			treeTable.SetColumnCells (3, c3.ToArray ());
			treeTable.SetColumnCells (4, c4.ToArray ());
			treeTable.SetColumnCells (5, c5.ToArray ());
			treeTable.SetColumnCells (6, c6.ToArray ());
		}

		private Guid? SelectedGuid
		{
			get
			{
				if (this.treeTableSelectedRow == -1)
				{
					return null;
				}
				else
				{
					return this.accessor.GetObjectGuid (this.treeTableSelectedRow);
				}
			}
		}

		private void UpdateTreeTableToolbar()
		{
			if (this.isEditing)
			{
				this.treeTableToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Disable);
				this.treeTableToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Disable);
				this.treeTableToolbar.SetCommandState (ToolbarCommand.Edit,     ToolbarCommandState.Disable);
				this.treeTableToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
			}
			else
			{
				if (this.treeTableSelectedRow == -1)
				{
					this.treeTableToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Disable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Edit,     ToolbarCommandState.Disable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
				}
				else
				{
					this.treeTableToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Edit,     ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Enable);
				}
			}
		}
		#endregion


		#region Timeline
		private void CreateTimeline(Widget parent)
		{
			this.timelineSelectedCell = -1;

			this.timelineController = new NavigationTimelineController();

			var frame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Bottom,
				PreferredHeight = AbstractScroller.DefaultBreadth + 18*3,
			};

			this.timelineController.DateChanged += delegate
			{
				this.UpdateTimelineController ();
			};

			this.timelineController.CreateUI (frame);
			this.timelineController.Pivot = 0.0;
			this.timelineController.SetRows (ObjectsView.GetTimelineRows (false));

			this.UpdateTimelineData ();
			this.UpdateTimelineController ();

			this.timelineController.CellClicked += delegate (object sender, int row, int rank)
			{
				if (row == 0)
				{
					this.timelineSelectedCell = this.timelineController.LeftVisibleCell + rank;

					this.UpdateTimelineController ();
					this.UpdateTreeTableController ();
					this.UpdateTimelineToolbar ();
				}
			};
		}

		private static TimelineRowDescription[] GetTimelineRows(bool all)
		{
			var list = new List<TimelineRowDescription> ();

			if (all)
			{
				list.Add (new TimelineRowDescription (TimelineRowType.Value, "Valeur assurance", relativeHeight: 2.0));
				list.Add (new TimelineRowDescription (TimelineRowType.Value, "Valeur comptable", relativeHeight: 2.0, valueColor1: Color.FromName ("Green"), valueColor2: Color.FromName ("Red")));
				list.Add (new TimelineRowDescription (TimelineRowType.Glyph, "Evénements"));
				list.Add (new TimelineRowDescription (TimelineRowType.Days, "Jours"));
				list.Add (new TimelineRowDescription (TimelineRowType.DaysOfWeek, ""));
				list.Add (new TimelineRowDescription (TimelineRowType.WeekOfYear, "Semaines"));
				list.Add (new TimelineRowDescription (TimelineRowType.Month, "Mois"));
			}
			else
			{
				list.Add (new TimelineRowDescription (TimelineRowType.Glyph, "Evénements"));
				list.Add (new TimelineRowDescription (TimelineRowType.Days, "Jours"));
				list.Add (new TimelineRowDescription (TimelineRowType.Month, "Mois"));
			}

			return list.ToArray ();
		}

		private void UpdateTimelineData()
		{
			var start = new System.DateTime (this.accessor.StartDate.Year, 1, 1);
			var end   = new System.DateTime (this.accessor.StartDate.Year, 12, 31);

			this.timelineData.Compute (this.SelectedGuid, this.timelineMode, start, end);

			this.timelineController.CellsCount = this.timelineData.CellsCount;
		}

		private void UpdateTimelineController()
		{
			int selection = this.timelineSelectedCell - this.timelineController.LeftVisibleCell;
			this.InitialiseTimeline (this.timelineController, selection);
		}


		private void InitialiseTimeline(NavigationTimelineController timeline, int selection)
		{
			var dates  = new List<TimelineCellDate> ();
			var glyphs = new List<TimelineCellGlyph> ();

			int count = timeline.VisibleCellsCount;
			int firstCell = timeline.LeftVisibleCell;

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

					dates.Add (d);
					glyphs.Add (g);
				}
			}

			timeline.SetRowGlyphCells (0, glyphs.ToArray ());
			timeline.SetRowDayCells   (1, dates.ToArray ());
			timeline.SetRowMonthCells (2, dates.ToArray ());
		}

		public int FirstTimelineEventIndex
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

				return 0;
			}
		}

		public Timestamp? SelectedTimestamp
		{
			get
			{
				if (this.timelineSelectedCell == -1 || this.timelineController == null)
				{
					return null;
				}
				else
				{
					return this.timelineData.GetCell (this.timelineSelectedCell).Value.Timestamp;
				}
			}
		}

		private void UpdateTimelineToolbar()
		{
			if (this.isEditing)
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.First,    ToolbarCommandState.Disable);
				this.timelineToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Disable);
				this.timelineToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Disable);
				this.timelineToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
			}
			else
			{
				bool empty = true;

				if (this.timelineSelectedCell != -1)
				{
					var cell = this.timelineData.GetCell (this.timelineSelectedCell);
					empty = cell.Value.TimelineGlyph == TimelineGlyph.Empty;
				}

				if (empty)
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.New,    ToolbarCommandState.Enable);
					this.timelineToolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Disable);
				}
				else
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.New,    ToolbarCommandState.Enable);
					this.timelineToolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Enable);
				}

				if (this.timelineSelectedCell == this.FirstTimelineEventIndex)
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.First, ToolbarCommandState.Disable);
				}
				else
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.First, ToolbarCommandState.Enable);
				}

				if (this.timelineSelectedCell == -1)
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
				}
				else
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Enable);
				}
			}
		}
		#endregion


		private readonly TimelineData			timelineData;

		private TreeTableToolbar				treeTableToolbar;
		private TimelineToolbar					timelineToolbar;
		private EditToolbar						editToolbar;

		private FrameBox						timelineFrameBox;

		private NavigationTreeTableController	treeTableController;
		private int								treeTableRowsCount;
		private int								treeTableSelectedRow;

		private NavigationTimelineController	timelineController;
		private int								timelineSelectedCell;
		private TimelineMode					timelineMode;
	}
}
