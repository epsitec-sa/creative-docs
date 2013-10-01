//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Assets.Data.Entities;

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App
{
	public class AssetsApplication : CoreInteractiveApp
	{
		public override string					ShortWindowTitle
		{
			get
			{
				return "Crésus Immobilisations";
			}
		}
		
		public override string					ApplicationIdentifier
		{
			get
			{
				return "Cr.Assets";
			}
		}

		
		public override bool StartupLogin()
		{
			return true;
		}

		
		protected override Window CreateWindow()
		{
			var window = base.CreateWindow ();

//-			window.MakeTitlelessResizableWindow ();

			return window;
		}

		protected override void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.ExecuteQuit (dispatcher, e);
		}

		protected override CoreAppPolicy CreateDefaultPolicy()
		{
			var policy = base.CreateDefaultPolicy ();

			policy.RequiresCoreCommandHandlers = false;

			return policy;
		}

		protected override void CreateManualComponents(IList<System.Action> initializers)
		{
			initializers.Add (this.InitializeApplication);
		}

		protected override System.Xml.Linq.XDocument LoadApplicationState()
		{
			return null;
		}

		protected override void SaveApplicationState(System.Xml.Linq.XDocument doc)
		{
		}

		private void InitializeApplication()
		{
			this.businessContext = new BusinessContext (this.Data, enableReload: true);

			var window = this.Window;
			
			window.Root.BackColor = Common.Drawing.Color.FromName ("White");
			this.CreateUI (window);	

			window.Show ();
			window.MakeActive ();

#if false
			System.Diagnostics.Debug.WriteLine ("Ready");
			System.Threading.Thread.Sleep (5*1000);

//			this.TestWriteEntities ();
			
			this.TestReadEntities ("A1000");
			System.Threading.Thread.Sleep (3*1000);

			this.TestReadEntities ("A1002");
#endif
		}

		private void TestWriteEntities()
		{
			var coreData = this.Data;

			for (int a = 1000; a < 1100; a++)
			{
				using (var context = new BusinessContext (coreData, enableReload: true))
				{
					System.Diagnostics.Debug.Write ("Asset " + a);
					
					var asset = context.CreateEntity<AssetEntity> ();

					asset.AssetId = string.Format ("A{0}", a);

					for (int i = 0; i < 50; i++)
					{
						var change = context.CreateEntity<AssetChangeSetEntity> ();
						var prop1  = context.CreateEntity<AssetObjectPropertyEntity> ();
						var val1   = context.CreateEntity<AssetObjectValueEntity> ();

						prop1.ChangeSet = change;
						val1.ChangeSet = change;

						change.DateTime = new System.DateTime (2013, 1, 1).AddDays (i * 7);
						change.Asset = asset;
						change.Xxx = i.ToString ();
					}

					System.Diagnostics.Debug.Write ("Saving asset " + a);
					context.SaveChanges (LockingPolicy.ReleaseLock);
				}
			}
		}

		private void TestReadEntities(string assetId)
		{
			var coreData = this.Data;

			using (var context = new BusinessContext (coreData, enableReload: true))
			{
				var example = new AssetChangeSetEntity ();
				example.Asset = new AssetEntity ();
				example.Asset.AssetId = assetId;

				System.Diagnostics.Debug.WriteLine ("Querying asset " + assetId);
				
				var changes = context.DataContext.GetByExample (example);

				System.Diagnostics.Debug.WriteLine ("Got " + changes.Count + " items");

				var ex2 = new AssetObjectPropertyEntity ();
				var ex3 = new AssetObjectValueEntity ();

				ex2.ChangeSet = new AssetChangeSetEntity ();
				ex2.ChangeSet.Asset = new AssetEntity ();
				ex2.ChangeSet.Asset.AssetId = assetId;

				ex3.ChangeSet = new AssetChangeSetEntity ();
				ex3.ChangeSet.Asset = new AssetEntity ();
				ex3.ChangeSet.Asset.AssetId = assetId;

				var props = context.DataContext.GetByExample (ex2).Where (x => x.ChangeSet.Asset.AssetId == assetId).ToList ();
				var vals  = context.DataContext.GetByExample (ex3).Where (x => x.ChangeSet.Asset.AssetId == assetId).ToList ();

				System.Diagnostics.Debug.WriteLine ("Found " + props.Count + " properties and " + vals.Count + " values");
			}
		}


		private void CreateUI(Window window)
		{
			//?Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookMetal");
			//?Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookSimply");
			Epsitec.Common.Widgets.Adorners.Factory.SetActive ("LookFlat");

			var frame = new FrameBox
			{
				Parent    = window.Root,
				Dock      = DockStyle.Fill,
				BackColor = Color.FromBrightness (0.8),
			};

			this.CreateTestTimelineController (frame);
			this.CreateTestTreeTableController (frame);
			//?this.CreateTestTimelineProvider (frame);
		}


		private void CreateTestTimelineController(Widget parent)
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
				Margins         = new Margins (10, 10, 0, 10),
			};

			this.timelineController.DateChanged += delegate
			{
				this.UpdateTimelineController ();
			};

			this.timelineController.CreateUI (frame);
			this.timelineController.Pivot = 0.0;
			this.timelineController.SetRows (AssetsApplication.GetRows (false));
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
			var date = AssetsApplication.AddDays (timelineStart, this.timelineController.LeftVisibleCell);
			int cellsCount = System.Math.Min (this.timelineCellsCount, this.timelineController.VisibleCellsCount);
			int selection = this.timelineSelectedCell - this.timelineController.LeftVisibleCell;

			AssetsApplication.InitialiseTimeline (this.timelineController, date, cellsCount, selection);
		}

		private NavigationTimelineController timelineController;
		private System.DateTime timelineStart;
		private int timelineCellsCount;
		private int timelineSelectedCell;


		private void CreateTestTimelineProvider(Widget parent)
		{
			var timeline = new Timeline ()
			{
				Parent     = parent,
				Dock       = DockStyle.Fill,
				Margins    = new Margins (10, 10, 335, 10),
				Pivot      = 0.25,
				ShowLabels = true,
			};

			timeline.SetRows (AssetsApplication.GetRows (true));

			var button = new Button ()
			{
				Parent = parent,
				Anchor = AnchorStyles.TopLeft,
				Margins = new Margins (10, 0, 280, 0),
				Text = "Coup de sac",
				PreferredWidth = 120,
				PreferredHeight = 25,
			};

			var provider   = new Client.MockTimelineEventClient (Date.Today);
			var controller = new Controllers.TimelineCellController (timeline, provider);

			button.Clicked += (o, e) => { provider.ChangeRandomSeed (); controller.ClearCache (); };

			controller.Refresh ();
		}

		private static List<AbstractTimelineRow> GetRows(bool all)
		{
			var list = new List<AbstractTimelineRow> ();

			if (all)
			{
				var row = new TimelineRowValues ()
				{
					Description = "Valeur assurance",
					RelativeHeight = 2.0,
					ValueDisplayMode = TimelineValueDisplayMode.All,
				};

				list.Add (row);
			}

			if (all)
			{
				var row = new TimelineRowValues ()
				{
					Description = "Valeur comptable",
					RelativeHeight = 2.0,
//					ValueDisplayMode = TimelineValueDisplayMode.Dots | TimelineValueDisplayMode.Lines,
					ValueDisplayMode = TimelineValueDisplayMode.All,
					Color1 = Color.FromName ("Green"),
					Color2 = Color.FromName ("Red"), 
				};

				list.Add (row);
			}

			{
				var row = new TimelineRowGlyphs ()
				{
					Description = "Evénements",
				};

				list.Add (row);
			}

			{
				var row = new TimelineRowDays ()
				{
					Description = "Jours",
				};

				list.Add (row);
			}

			if (all)
			{
				var row = new TimelineRowDaysOfWeek ()
				{
					Description = "",
				};

				list.Add (row);
			}

			if (all)
			{
				var row = new TimelineRowWeeksOfYear ()
				{
					Description = "Semaines",
				};

				list.Add (row);
			}
			
			{
				var row = new TimelineRowMonths ()
				{
					Description = "Mois",
				};

				list.Add (row);
			}

			return list;
		}

		private static void InitialiseTimeline(NavigationTimelineController timeline, System.DateTime start, int cellsCount, int selection)
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
				var date = AssetsApplication.AddDays (start, i);

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

			foreach (var r in timeline.Rows)
			{
				if (r is TimelineRowGlyphs)
				{
					var row = r as TimelineRowGlyphs;
					row.SetCells (glyphs.ToArray ());
				}
				else if (r is TimelineRowValues)
				{
					var row = r as TimelineRowValues;
					row.SetCells (row.RowIndex == 0 ? values1.ToArray () : values2.ToArray ());
				}
				else if (r is TimelineRowDays)
				{
					var row = r as TimelineRowDays;
					row.SetCells (dates.ToArray ());
				}
				else if (r is TimelineRowDaysOfWeek)
				{
					var row = r as TimelineRowDaysOfWeek;
					row.SetCells (dates.ToArray ());
				}
				else if (r is TimelineRowMonths)
				{
					var row = r as TimelineRowMonths;
					row.SetCells (dates.ToArray ());
				}
				else if (r is TimelineRowWeeksOfYear)
				{
					var row = r as TimelineRowWeeksOfYear;
					row.SetCells (dates.ToArray ());
				}
			}
		}


		private void CreateTestTreeTableController(Widget parent)
		{
			var OO = AssetsApplication.GetTreeTableObjects ();

			this.treeTableRowsCount = OO.Length;
			this.treeTableSelectedRow = -1;

			this.treeTableController = new NavigationTreeTableController
			{
				RowsCount = treeTableRowsCount,
			};

			var frame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (10),
			};

			this.treeTableController.RowChanged += delegate
			{
				this.UpdateTreeTableController ();
			};

			this.treeTableController.CreateUI (frame);
			this.treeTableController.SetColumns (AssetsApplication.GetColumns ());
			this.UpdateTreeTableController ();

			this.treeTableController.RowClicked += delegate (object sender, int column, int row)
			{
				this.treeTableSelectedRow = this.treeTableController.TopVisibleRow + row;
				this.UpdateTreeTableController ();
			};
		}

		private void UpdateTreeTableController()
		{
			var first = this.treeTableController.TopVisibleRow;
			int cellsCount = System.Math.Min (this.treeTableRowsCount, this.treeTableController.VisibleRowsCount);
			int selection = this.treeTableSelectedRow - this.treeTableController.TopVisibleRow;

			AssetsApplication.InitialiseTreeTable (this.treeTableController, first, selection);
		}

		private NavigationTreeTableController treeTableController;
		private int treeTableRowsCount;
		private int treeTableSelectedRow;


		private static List<AbstractTreeTableColumn> GetColumns()
		{
			var list = new List<AbstractTreeTableColumn> ();

			{
				var c = new TreeTableColumnString
				{
					PreferredWidth = 80,
					HeaderDescription = "Numéro",
					FooterDescription = "Colonne 1, pied",
				};

				list.Add (c);
			}

			{
				var c = new TreeTableColumnString
				{
					PreferredWidth = 120,
					HeaderDescription = "Responsable",
					FooterDescription = "Colonne 2, pied",
				};

				list.Add (c);
			}

			{
				var c = new TreeTableColumnString
				{
					PreferredWidth = 150,
					HeaderDescription = "Couleur",
					FooterDescription = "Colonne 3, pied",
				};

				list.Add (c);
			}

			{
				var c = new TreeTableColumnString
				{
					PreferredWidth = 200,
					HeaderDescription = "Numéro de série",
					FooterDescription = "Colonne 4, pied",
				};

				list.Add (c);
			}

			{
				var c = new TreeTableColumnDecimal
				{
					PreferredWidth = 120,
					HeaderDescription = "Valeur comptable",
					FooterDescription = "Colonne 5, pied",
				};

				list.Add (c);
			}

			{
				var c = new TreeTableColumnDecimal
				{
					PreferredWidth = 120,
					HeaderDescription = "Valeur assurance",
					FooterDescription = "Colonne 6, pied",
				};

				list.Add (c);
			}

			return list;
		}

		private static void InitialiseTreeTable(NavigationTreeTableController treeTable, int firstRow, int selection)
		{
			var OO = AssetsApplication.GetTreeTableObjects ();

			treeTable.ColumnFirst.HeaderDescription = "Objet";
			treeTable.ColumnFirst.FooterDescription = "Liste, pied";

			var cf = new List<TreeTableCellFirst> ();
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

				var type = O.Level == 3 ? TreeTableFirstType.Final : TreeTableFirstType.Extended;

				var sf = new TreeTableCellFirst (O.Level, type, O.Nom, isSelected: (i == selection));
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

			treeTable.ColumnFirst.SetCellFirsts (cf.ToArray ());

			int cc = 0;
			foreach (var c in treeTable.Columns)
			{
				var stringColumn = c as TreeTableColumnString;
				var decimalColumn = c as TreeTableColumnDecimal;

				switch (cc)
				{
					case 0:
						stringColumn.SetCellStrings (c1.ToArray ());
						break;
					case 1:
						stringColumn.SetCellStrings (c2.ToArray ());
						break;
					case 2:
						stringColumn.SetCellStrings (c3.ToArray ());
						break;
					case 3:
						stringColumn.SetCellStrings (c4.ToArray ());
						break;
					case 4:
						decimalColumn.SetCellDecimals (c5.ToArray ());
						break;
					case 5:
						decimalColumn.SetCellDecimals (c6.ToArray ());
						break;
				}

				cc++;
			}
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
							Responsable = "François",
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
							NuméroSérie = "45-3292302-544545-8",
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
	
		
		private static System.DateTime AddDays(System.DateTime date, int numberOfDays)
		{
			return new Date (date.Ticks + Time.TicksPerDay*numberOfDays).ToDateTime ();
		}


		private BusinessContext					businessContext;
	}
}
