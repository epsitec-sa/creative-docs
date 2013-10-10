//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsView : AbstractView
	{
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
			var OO = ObjectsView.GetTreeTableObjects ();

			this.treeTableRowsCount = OO.Length;
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

			ObjectsView.InitialiseTreeTable (this.treeTableController, first, selection);
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

		private static void InitialiseTreeTable(NavigationTreeTableController treeTable, int firstRow, int selection)
		{
			var OO = ObjectsView.GetTreeTableObjects ();

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
				if (firstRow+i >= OO.Length)
				{
					break;
				}

				var O = OO[firstRow+i];

				var type = O.Level == 3 ? TreeTableTreeType.Final : TreeTableTreeType.Extended;
				if (i == 0)
				{
					type = TreeTableTreeType.Compacted;  // juste pour en voir un !
				}

				var sf = new TreeTableCellTree (true, O.Level, type, O.Nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString (true, O.Numéro, isSelected: (i == selection));
				var s2 = new TreeTableCellString (true, O.Responsable, isSelected: (i == selection));
				var s3 = new TreeTableCellString (true, O.Couleur, isSelected: (i == selection));
				var s4 = new TreeTableCellString (true, O.NuméroSérie, isSelected: (i == selection));
				var s5 = new TreeTableCellDecimal (true, O.ValeurComptable, isSelected: (i == selection));
				var s6 = new TreeTableCellDecimal (true, O.ValeurAssurance, isSelected: (i == selection));

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

		private static TreeTableObject[] GetTreeTableObjects()
		{
			var O = new TreeTableObject
			{
				Nom = "Immobilisations",
				Level = 0,
			};

				var O1 = new TreeTableObject
				{
					Parent = O,
					Level = 1,
					Nom = "Bâtiments",
					Numéro = "1",
				};

					var O11 = new TreeTableObject
					{
						Parent = O1,
						Level = 2,
						Nom = "Immeubles",
						Numéro = "1.1",
					};

						var O111 = new TreeTableObject
						{
							Parent = O11,
							Level = 3,
							Nom = "Centre administratif",
							Numéro = "1.1.1",
							ValeurComptable = 2450000.0m,
							ValeurAssurance = 3000000.0m,
							Responsable = "Paul",
						};

						var O112 = new TreeTableObject
						{
							Parent = O11,
							Level = 3,
							Nom = "Centre logistique",
							Numéro = "1.1.2",
							ValeurComptable = 4550000.0m,
							ValeurAssurance = 6000000.0m,
							Responsable = "Paul",
						};

						var O113 = new TreeTableObject
						{
							Parent = O11,
							Level = 3,
							Nom = "Centre d'expédition",
							Numéro = "1.1.3",
							ValeurComptable = 2100000.0m,
							ValeurAssurance = 3000000.0m,
							Responsable = "Sandra",
						};

						var O114 = new TreeTableObject
						{
							Parent = O11,
							Level = 3,
							Nom = "Showroom",
							Numéro = "1.1.4",
							ValeurComptable = 8750000.0m,
							ValeurAssurance = 5500000.0m,
							Responsable = "Eric",
						};

					var O12 = new TreeTableObject
					{
						Parent = O1,
						Level = 2,
						Nom = "Usines",
						Numéro = "1.2",
					};

						var O121 = new TreeTableObject
						{
							Parent = O12,
							Level = 3,
							Nom = "Centre d'usinage",
							Numéro = "1.2.1",
							ValeurComptable = 10400000.0m,
							ValeurAssurance = 13000000.0m,
							Responsable = "René",
						};

						var O122 = new TreeTableObject
						{
							Parent = O12,
							Level = 3,
							Nom = "Centre d'assemblage",
							Numéro = "1.2.2",
							ValeurComptable = 8000000.0m,
							ValeurAssurance = 9500000.0m,
							Responsable = "Ernest",
						};

					var O13 = new TreeTableObject
					{
						Parent = O1,
						Level = 2,
						Nom = "Entrepôts",
						Numéro = "1.3",
					};

						var O131 = new TreeTableObject
						{
							Parent = O13,
							Level = 3,
							Nom = "Dépôt principal",
							Numéro = "1.3.1",
							ValeurComptable = 2100000.0m,
							ValeurAssurance = 3500000.0m,
							Responsable = "Anne-Sophie",
						};

						var O132 = new TreeTableObject
						{
							Parent = O13,
							Level = 3,
							Nom = "Dépôt secondaire",
							Numéro = "1.3.2",
							ValeurComptable = 5320000.0m,
							ValeurAssurance = 5000000.0m,
							Responsable = "Paul",
						};

						var O133 = new TreeTableObject
						{
							Parent = O13,
							Level = 3,
							Nom = "Centre de recyclage",
							Numéro = "1.3.3",
							ValeurComptable = 1200000.0m,
							ValeurAssurance = 1500000.0m,
							Responsable = "Victoria",
						};

				var O2 = new TreeTableObject
				{
					Parent = O,
					Level = 1,
					Nom = "Véhicules",
					Numéro = "2",
				};

					var O21 = new TreeTableObject
					{
						Parent = O2,
						Level = 2,
						Nom = "Camions",
						Numéro = "2.1",
					};

						var O211 = new TreeTableObject
						{
							Parent = O21,
							Level = 3,
							Nom = "Scania X20",
							Numéro = "2.1.1",
							ValeurComptable = 150000.0m,
							ValeurAssurance = 160000.0m,
							Responsable = "Jean-François-Paul-Eric-Georges-André",
							Couleur = "Blanc",
							NuméroSérie = "25004-800-65210-45R",
						};

						var O212 = new TreeTableObject
						{
							Parent = O21,
							Level = 3,
							Nom = "Scania X30 semi",
							Numéro = "2.1.2",
							ValeurComptable = 180000.0m,
							ValeurAssurance = 200000.0m,
							Responsable = "Serge",
							Couleur = "Rouge",
							NuméroSérie = "25004-800-20087-20X",
						};

						var O213 = new TreeTableObject
						{
							Parent = O21,
							Level = 3,
							Nom = "Volvo T-200",
							Numéro = "2.1.3",
							ValeurComptable = 90000.0m,
							ValeurAssurance = 75000.0m,
							Responsable = "Jean-Pierre",
							Couleur = "Jaune",
							NuméroSérie = "T40-56-200-65E4",
						};

						var O214 = new TreeTableObject
						{
							Parent = O21,
							Level = 3,
							Nom = "Volvo R-500",
							Numéro = "2.1.4",
							ValeurComptable = 110000.0m,
							ValeurAssurance = 120000.0m,
							Responsable = "Olivier",
							Couleur = "Blanc",
							NuméroSérie = "T50-00-490-9PQ8",
						};

					var O22 = new TreeTableObject
					{
						Parent = O2,
						Level = 2,
						Nom = "Camionnettes",
						Numéro = "2.2",
					};

						var O221 = new TreeTableObject
						{
							Parent = O22,
							Level = 3,
							Nom = "Renault Doblo",
							Numéro = "2.2.1",
							ValeurComptable = 25000.0m,
							ValeurAssurance = 28000.0m,
							Responsable = "Francine",
							Couleur = "Blanc",
							NuméroSérie = "456-321-132-898908",
						};

						var O222 = new TreeTableObject
						{
							Parent = O22,
							Level = 3,
							Nom = "Ford Transit",
							Numéro = "2.2.2",
							ValeurComptable = 30000.0m,
							ValeurAssurance = 32000.0m,
							Responsable = "Jean-Bernard",
							Couleur = "Blanc",
							NuméroSérie = "4380003293-343213-4",
						};

					var O23 = new TreeTableObject
					{
						Parent = O2,
						Level = 2,
						Nom = "Voitures",
						Numéro = "2.3",
					};

						var O231 = new TreeTableObject
						{
							Parent = O23,
							Level = 3,
							Nom = "Citroën C4 Picasso",
							Numéro = "2.3.1",
							ValeurComptable = 22000.0m,
							ValeurAssurance = 25000.0m,
							Responsable = "Simon",
							Couleur = "Noir",
							NuméroSérie = "D456-0003232-0005",
						};

						var O232 = new TreeTableObject
						{
							Parent = O23,
							Level = 3,
							Nom = "Opel Corsa",
							Numéro = "2.3.2",
							ValeurComptable = 9000.0m,
							ValeurAssurance = 10000.0m,
							Responsable = "Frédérique",
							Couleur = "Noir",
							NuméroSérie = "45-3292302-544545-8 ABCDEFGHIJKLMNOPQRSTUVWXYZ",
						};

						var O233 = new TreeTableObject
						{
							Parent = O23,
							Level = 3,
							Nom = "Fiat Panda",
							Numéro = "2.3.3",
							ValeurComptable = 8000.0m,
							ValeurAssurance = 5000.0m,
							Responsable = "Dominique",
							Couleur = "Gris métalisé",
							NuméroSérie = "780004563233232",
						};

						var O234 = new TreeTableObject
						{
							Parent = O23,
							Level = 3,
							Nom = "Fiat Uno",
							Numéro = "2.3.4",
							ValeurComptable = 11000.0m,
							ValeurAssurance = 10000.0m,
							Responsable = "Denise",
							Couleur = "Bleu",
							NuméroSérie = "456000433434002",
						};

						var O235 = new TreeTableObject
						{
							Parent = O23,
							Level = 3,
							Nom = "Fiat Uno",
							Numéro = "2.3.5",
							ValeurComptable = 12000.0m,
							ValeurAssurance = 13000.0m,
							Responsable = "Marie",
							Couleur = "Antracite",
							NuméroSérie = "789787332009822",
						};

						var O236 = new TreeTableObject
						{
							Parent = O23,
							Level = 3,
							Nom = "Toyota Yaris Verso",
							Numéro = "2.3.6",
							ValeurComptable = 16000.0m,
							ValeurAssurance = 12000.0m,
							Responsable = "Christiane",
							Couleur = "Gris",
							NuméroSérie = "F40T-500023-40232-30987-M",
						};

			TreeTableObject[] OO =
			{
				O,
					O1,
						O11,
							O111, O112, O113, O114,
						O12,
							O121, O122,
						O13,
							O131, O132, O133,
					O2,
						O21,
							O211, O212, O213, O214,
						O22,
							O221, O222,
						O23,
							O231, O232, O233, O234, O235, O236,
			};

			return OO;
		}

		private class TreeTableObject
		{
			public TreeTableObject Parent;
			public int Level;
			public string Nom;
			public string Numéro;
			public decimal? ValeurComptable;
			public decimal? ValeurAssurance;
			public string Responsable;
			public string Couleur;
			public string NuméroSérie;
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
