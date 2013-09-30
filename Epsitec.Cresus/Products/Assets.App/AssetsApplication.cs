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

			System.Diagnostics.Debug.WriteLine ("Ready");
			System.Threading.Thread.Sleep (5*1000);

//			this.TestWriteEntities ();
			
			this.TestReadEntities ("A1000");
			System.Threading.Thread.Sleep (3*1000);

			this.TestReadEntities ("A1002");
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

			//?this.CreateTestTimelineBase (frame);
			this.CreateTestTimelineController (frame);
			//?this.CreateTestTimelineProvider (frame);
		}

		private void CreateTestTimelineBase(Widget parent)
		{
			var timeline = new Timeline ()
			{
				Parent     = parent,
				Dock       = DockStyle.Fill,
				Margins    = new Margins (10, 10, 220, 10),
				Pivot      = 0.0,
				ShowLabels = true,
			};

			var start = new System.DateTime (2013, 11, 20);

			timeline.SetRows (AssetsApplication.GetRows (true));
			AssetsApplication.InitialiseTimeline (timeline, start, 100, -1);

			timeline.CellClicked += delegate (object sender, int row, int rank)
			{
				if (row == 2)
				{
					AssetsApplication.InitialiseTimeline (timeline, start, 100, rank);
				}
			};
		}

		private void CreateTestTimelineController(Widget parent)
		{
			this.start = new System.DateTime (2013, 1, 1);
			this.cellsCount = 365;
			this.selectedCell = -1;

			this.controller = new NavigationTimelineController
			{
				CellsCount = cellsCount,
			};

			var frame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Bottom,
				PreferredHeight = 70,
				Margins         = new Margins (10),
			};

			this.controller.DateChanged += delegate
			{
				this.UpdateController ();
			};

			this.controller.CreateUI (frame);
			this.controller.Timeline.Pivot = 0.0;
			this.controller.Timeline.SetRows (AssetsApplication.GetRows (false));
			this.UpdateController ();

			this.controller.Timeline.CellClicked += delegate (object sender, int row, int rank)
			{
				if (row == 0)
				{
					this.selectedCell = this.controller.LeftVisibleCell + rank;
					this.UpdateController ();
				}
			};
		}

		private void UpdateController()
		{
			var date = AssetsApplication.AddDays (start, this.controller.LeftVisibleCell);
			int cellsCount = System.Math.Min (this.cellsCount, this.controller.Timeline.VisibleCellsCount);
			int selection = this.selectedCell - this.controller.LeftVisibleCell;

			AssetsApplication.InitialiseTimeline (this.controller.Timeline, date, cellsCount, selection);
		}

		private NavigationTimelineController controller;
		private System.DateTime start;
		private int cellsCount;
		private int selectedCell;

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

		private static void InitialiseTimeline(Timeline timeline, System.DateTime start, int cellsCount, int selection)
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

		private static System.DateTime AddDays(System.DateTime date, int numberOfDays)
		{
			return new Date (date.Ticks + Time.TicksPerDay*numberOfDays).ToDateTime ();
		}


		private BusinessContext					businessContext;
	}
}
