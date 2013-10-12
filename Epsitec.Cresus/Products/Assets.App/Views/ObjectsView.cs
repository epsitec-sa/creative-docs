//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
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

			this.timelineFrameBox2 = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.timelineFrameBox1 = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.treeTableToolbar = new TreeTableToolbar ();
			this.treeTableToolbar.CreateUI (this.listFrameBox);

			this.timelineToolbar = new TimelineToolbar ();
			this.timelineToolbar.CreateUI (this.timelineFrameBox1);
			this.timelineToolbar.TimelineMode = this.timelineMode;

			this.editToolbar = new EditToolbar ();
			this.editToolbar.CreateUI (this.editFrameBox);

			this.CreateTreeTable (this.listFrameBox);
			this.CreateTimeline (this.timelineFrameBox2);

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

					case ToolbarCommand.Now:
						this.OnTimelineNow ();
						break;

					case ToolbarCommand.Last:
						this.OnTimelineLast ();
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

				this.timelineFrameBox2.Children.Clear ();
				this.CreateTimeline (this.timelineFrameBox2);

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

		protected void OnTimelineNow()
		{
			this.timelineSelectedCell = this.NowTimelineEventIndex.GetValueOrDefault (0);

			this.UpdateTimelineController ();
			this.UpdateTreeTableController ();
			this.UpdateTimelineToolbar ();
		}

		protected void OnTimelineLast()
		{
			this.timelineSelectedCell = this.LastTimelineEventIndex;

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

		private void UpdateTreeTableController()
		{
			var firstRow = this.treeTableController.TopVisibleRow;
			int selection = this.treeTableSelectedRow - this.treeTableController.TopVisibleRow;

			var timestamp = this.SelectedTimestamp;

			if (!timestamp.HasValue)
			{
				timestamp = new Timestamp (this.accessor.StartDate, 0);
			}

			var cf = new List<TreeTableCellTree> ();
			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellString> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellString> ();
			var c5 = new List<TreeTableCellDecimal> ();
			var c6 = new List<TreeTableCellDecimal> ();

			var count = this.treeTableController.VisibleRowsCount;
			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.accessor.ObjectsCount)
				{
					break;
				}

				var guid = this.accessor.GetObjectGuid (firstRow+i);
				var properties = this.accessor.GetObjectSyntheticProperties (guid, timestamp.Value);

				int level = DataAccessor.GetIntProperty (properties, (int) ObjectField.Level).GetValueOrDefault (-1);

				var type = TreeTableTreeType.Extended;

				if (level == -1)
				{
					type = TreeTableTreeType.None;
				}
				else if (level == 3)
				{
					type = TreeTableTreeType.Final;
				}

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

			this.treeTableController.SetColumnCells (0, cf.ToArray ());
			this.treeTableController.SetColumnCells (1, c1.ToArray ());
			this.treeTableController.SetColumnCells (2, c2.ToArray ());
			this.treeTableController.SetColumnCells (3, c3.ToArray ());
			this.treeTableController.SetColumnCells (4, c4.ToArray ());
			this.treeTableController.SetColumnCells (5, c5.ToArray ());
			this.treeTableController.SetColumnCells (6, c6.ToArray ());
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

			this.timelineController = new NavigationTimelineController ();

			double height = AbstractScroller.DefaultBreadth + 18*3;

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

			var frame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Bottom,
				PreferredHeight = height,
			};

			this.timelineController.DateChanged += delegate
			{
				this.UpdateTimelineController ();
			};

			this.timelineController.CreateUI (frame);
			this.timelineController.RelativeWidth = this.IsExtended ? 1.0 : 2.0;
			this.timelineController.ShowLabels = this.IsShowLabels;
			this.timelineController.Pivot = 0.0;
			this.timelineController.SetRows (this.GetTimelineRows ());

			this.UpdateTimelineData ();
			this.UpdateTimelineController ();

			this.timelineController.CellClicked += delegate (object sender, int row, int rank)
			{
				if (row == (this.HasGraph ? 1 : 0))
				{
					this.timelineSelectedCell = this.timelineController.LeftVisibleCell + rank;

					this.UpdateTimelineController ();
					this.UpdateTreeTableController ();
					this.UpdateTimelineToolbar ();
				}
			};
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

			this.timelineData.Compute (this.SelectedGuid, this.timelineMode, start, end);

			this.timelineController.CellsCount = this.timelineData.CellsCount;
		}

		private void UpdateTimelineController()
		{
			int visibleCount = this.timelineController.VisibleCellsCount;
			int cellsCount   = this.timelineData.CellsCount;
			int count        = System.Math.Min (visibleCount, cellsCount);
			int firstCell    = this.timelineController.LeftVisibleCell;
			int selection    = this.timelineSelectedCell;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de cellules.
				selection = System.Math.Min (selection, cellsCount-1);

				//	Si la sélection est avant la zone visible, on recule firstCell.
				firstCell = System.Math.Min (firstCell, selection);

				//	Si la sélection est après la zone visible, on avance firstCell.
				firstCell = System.Math.Max (firstCell, selection-count+1);

				if (this.timelineController.LeftVisibleCell != firstCell)
				{
					this.timelineController.LeftVisibleCell = firstCell;
				}

				selection -= this.timelineController.LeftVisibleCell;
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
				this.timelineController.SetRowValueCells (line++, values.ToArray (), min, max);
			}

			this.timelineController.SetRowGlyphCells (line++, glyphs.ToArray ());

			if (this.IsDaysOfWeek)
			{
				this.timelineController.SetRowDayOfWeekCells (line++, dates.ToArray ());
			}

			if (this.IsExtended)
			{
				this.timelineController.SetRowDayCells (line++, dates.ToArray ());
			}
			else
			{
				this.timelineController.SetRowDayMonthCells (line++, dates.ToArray ());
			}

			if (this.IsWeeksOfYear)
			{
				this.timelineController.SetRowWeekOfYearCells (line++, dates.ToArray ());
			}

			if (this.IsExtended)
			{
				this.timelineController.SetRowMonthCells (line++, dates.ToArray ());
			}
			else
			{
				this.timelineController.SetRowYearCells (line++, dates.ToArray ());
			}
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

		public int LastTimelineEventIndex
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

				return count-1;
			}
		}

		public int? NowTimelineEventIndex
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

		public Timestamp? SelectedTimestamp
		{
			get
			{
				if (this.timelineSelectedCell != -1 && this.timelineController != null)
				{
					var cell = this.timelineData.GetCell (this.timelineSelectedCell);

					if (cell.HasValue)
					{
						return cell.Value.Timestamp;
					}
				}

				return null;
			}
		}

		private void UpdateTimelineToolbar()
		{
			if (this.timelineSelectedCell == this.FirstTimelineEventIndex)
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.First, ToolbarCommandState.Disable);
			}
			else
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.First, ToolbarCommandState.Enable);
			}

			if (this.timelineSelectedCell == this.NowTimelineEventIndex.GetValueOrDefault (-999))
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.Now, ToolbarCommandState.Disable);
			}
			else
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.Now, ToolbarCommandState.Enable);
			}

			if (this.timelineSelectedCell == this.LastTimelineEventIndex)
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.Last, ToolbarCommandState.Disable);
			}
			else
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.Last, ToolbarCommandState.Enable);
			}

			if (this.isEditing)
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Disable);
				this.timelineToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Disable);
				this.timelineToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
			}
			else
			{
				bool noEventSelected = true;

				if (this.timelineSelectedCell != -1)
				{
					var cell = this.timelineData.GetCell (this.timelineSelectedCell);
					noEventSelected = (cell.HasValue && cell.Value.TimelineGlyph == TimelineGlyph.Empty);
				}

				if (noEventSelected)
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.New,    ToolbarCommandState.Enable);
					this.timelineToolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Disable);
				}
				else
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.New,    ToolbarCommandState.Enable);
					this.timelineToolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Enable);
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

		private bool HasGraph
		{
			get
			{
				return (this.timelineMode & TimelineMode.Graph) != 0;
			}
		}
		#endregion


		private readonly TimelineData			timelineData;

		private TreeTableToolbar				treeTableToolbar;
		private TimelineToolbar					timelineToolbar;
		private EditToolbar						editToolbar;

		private FrameBox						timelineFrameBox1;
		private FrameBox						timelineFrameBox2;

		private NavigationTreeTableController	treeTableController;
		private int								treeTableRowsCount;
		private int								treeTableSelectedRow;

		private NavigationTimelineController	timelineController;
		private int								timelineSelectedCell;
		private TimelineMode					timelineMode;
	}
}
