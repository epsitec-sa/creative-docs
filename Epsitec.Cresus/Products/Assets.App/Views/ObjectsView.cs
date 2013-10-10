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
		}

		public override void CreateUI(Widget parent, MainToolbar toolbar)
		{
			base.CreateUI (parent, toolbar);

			this.timelineBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.CreateTreeTable (this.listFrameBox);
			this.CreateTimeline (this.timelineBox);

			this.Update ();
		}


		protected override string Title
		{
			get
			{
				return "Objets d'immobilisation";
			}
		}


		protected override void OnCommandNew()
		{
		}

		protected override void OnCommandDelete()
		{
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
			this.treeTableController.SetColumns (ObjectsView.GetColumns (), 1);
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
					this.OnCommandEdit ();
				}
				else
				{
					this.treeTableSelectedRow = sel;
					this.UpdateTreeTableController ();
					this.Update ();
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
			var timestamp = new Timestamp (new System.DateTime (2013, 1, 1), 0);

			this.InitialiseTreeTable (this.treeTableController, first, selection, timestamp);
		}


		private static TreeTableColumnDescription[] GetColumns()
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
		#endregion


		#region Timeline
		private void CreateTimeline(Widget parent)
		{
			this.timelineStart = new System.DateTime (2013, 1, 1);
			this.timelineCellsCount = 365;
			this.timelineSelectedCell = -1;

			this.timelineController = new NavigationTimelineController
			{
				CellsCount = timelineCellsCount,
			};

			var frame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Bottom,
				PreferredHeight = 70,
			};

			this.timelineController.DateChanged += delegate
			{
				this.UpdateTimelineController ();
			};

			this.timelineController.CreateUI (frame);
			this.timelineController.Pivot = 0.0;
			this.timelineController.SetRows (ObjectsView.GetRows (false));
			this.UpdateTimelineController ();

			this.timelineController.CellClicked += delegate (object sender, int row, int rank)
			{
				if (row == 0)
				{
					this.timelineSelectedCell = this.timelineController.LeftVisibleCell + rank;
					this.UpdateTimelineController ();
				}
			};
		}

		private void UpdateTimelineController()
		{
			var date = ObjectsView.AddDays (timelineStart, this.timelineController.LeftVisibleCell);
			int cellsCount = System.Math.Min (this.timelineCellsCount, this.timelineController.VisibleCellsCount);
			int selection = this.timelineSelectedCell - this.timelineController.LeftVisibleCell;

			ObjectsView.InitialiseTimeline (this.timelineController, date, cellsCount, selection, false);
		}


		private static TimelineRowDescription[] GetRows(bool all)
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

		private static void InitialiseTimeline(NavigationTimelineController timeline, System.DateTime start, int cellsCount, int selection, bool all)
		{
			var dates = new List<TimelineCellDate> ();
			var glyphs = new List<TimelineCellGlyph> ();
			var values1 = new List<TimelineCellValue> ();
			var values2 = new List<TimelineCellValue> ();

			decimal? value1 = 10000.0m;
			decimal? value2 = 15000.0m;
			decimal? value3 = 25000.0m;

			for (int i = 0; i < cellsCount; i++)
			{
				var date = ObjectsView.AddDays (start, i);

				int ii = (int) (start.Ticks / Time.TicksPerDay) + i;
				var glyph = TimelineGlyph.Empty;

				if (ii%12 == 0)
				{
					glyph = TimelineGlyph.FilledCircle;
				}
				else if (ii%12 == 1)
				{
					glyph = TimelineGlyph.OutlinedCircle;
				}
				else if (ii%12 == 6)
				{
					glyph = TimelineGlyph.FilledSquare;
				}
				else if (ii%12 == 7)
				{
					glyph = TimelineGlyph.OutlinedSquare;
				}

				if (glyph != TimelineGlyph.Empty)
				{
					if (glyph == TimelineGlyph.OutlinedSquare)
					{
						value1 += 2000.0m;
					}
					else
					{
						value1 -= value1 * 0.10m;
					}

					value2 -= value2 * 0.25m;
					value3 -= value3 * 0.50m;
				}

				var v1 = value1;
				var v2 = value2;
				var v3 = value3;

				if (glyph == TimelineGlyph.Empty)
				{
					v1 = null;
					v2 = null;
					v3 = null;
				}

				if (v1.HasValue && v1.Value < 2000.0m)
				{
					v1 = null;
				}

				if (v2.HasValue && v2.Value < 2000.0m)
				{
					v2 = null;
				}

				if (v3.HasValue && v3.Value < 2000.0m)
				{
					v3 = null;
				}

				var d = new TimelineCellDate (date, isSelected: (i == selection));
				var g = new TimelineCellGlyph (glyph, isSelected: (i == selection));
				var x1 = new TimelineCellValue (v1, isSelected: (i == selection));
				var x2 = new TimelineCellValue (v2, v3, isSelected: (i == selection));

				dates.Add (d);
				glyphs.Add (g);
				values1.Add (x1);
				values2.Add (x2);
			}

			if (all)
			{
				timeline.SetRowValueCells (0, values1.ToArray ());
				timeline.SetRowValueCells (1, values2.ToArray ());
				timeline.SetRowGlyphCells (2, glyphs.ToArray ());
				timeline.SetRowDayCells (3, dates.ToArray ());
				timeline.SetRowDayOfWeekCells (4, dates.ToArray ());
				timeline.SetRowWeekOfYearCells (5, dates.ToArray ());
				timeline.SetRowMonthCells (6, dates.ToArray ());
			}
			else
			{
				timeline.SetRowGlyphCells (0, glyphs.ToArray ());
				timeline.SetRowDayCells (1, dates.ToArray ());
				timeline.SetRowMonthCells (2, dates.ToArray ());
			}
		}
		#endregion


		private static System.DateTime AddDays(System.DateTime date, int numberOfDays)
		{
			return new Date (date.Ticks + Time.TicksPerDay*numberOfDays).ToDateTime ();
		}


		private FrameBox						timelineBox;

		private NavigationTreeTableController	treeTableController;
		private int								treeTableRowsCount;
		private int								treeTableSelectedRow;

		private NavigationTimelineController	timelineController;
		private System.DateTime					timelineStart;
		private int								timelineCellsCount;
		private int								timelineSelectedCell;
	}
}
