//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			this.businessContext = new BusinessContext (this.Data, true);

			var window = this.Window;
			
			window.Root.BackColor = Common.Drawing.Color.FromName ("White");
			this.CreateUI (window);	

			window.Show ();
			window.MakeActive ();
		}


		private void CreateUI(Window window)
		{
			var frame = new FrameBox
			{
				Parent    = window.Root,
				Dock      = DockStyle.Fill,
				BackColor = Color.FromBrightness (0.8),
			};

			this.CreateTestTimeLine (frame);
		}

		private void CreateTestTimeLine(Widget parent)
		{
#if true
			//	Test pour DR.
	#if false
			var timeline = new Timeline ()
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (10, 10, 335, 10),
				Pivot   = 0.0,
			};

			timeline.SetRows (AssetsApplication.GetRows ());
#else
			var timeline = new Timeline ()
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (10, 10, 270, 10),
				Pivot   = 0.0,
			};

			timeline.SetRows (AssetsApplication.GetRows (true));
	#endif
			AssetsApplication.InitialiseTimeline (timeline, -1);

			timeline.CellClicked += delegate (object sender, int row, int rank)
			{
				if (row == 2)
				{
					AssetsApplication.InitialiseTimeline (timeline, rank);
				}
			};
#else
			//	Test pour PA.
			var timeline = new Timeline ()
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (10, 10, 335, 10),
				Pivot   = 0.25,
			};

			timeline.SetRows (AssetsApplication.GetRows ());

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
#endif
		}

		private static List<AbstractTimelineRow> GetRows(bool all = false)
		{
			var list = new List<AbstractTimelineRow> ();

			if (all)
			{
				var row = new TimelineRowValues ()
				{
					Description = "Valeur comptable",
					RelativeHeight = 2.0,
					ValueDisplayMode = TimelineValueDisplayMode.All,
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

		private static void InitialiseTimeline(Timeline timeline, int selection)
		{
			var dates  = new List<TimelineCellDate> ();
			var glyphs = new List<TimelineCellGlyph> ();
			var values = new List<TimelineCellValue> ();

			var start = new Date (2013, 11, 20);  // 20 novembre 2013
			decimal? value = 10000.0m;

			for (int i = 0; i < 100; i++)
			{
				var glyph = TimelineGlyph.Empty;

				if (i%12 == 0)
				{
					glyph = TimelineGlyph.FilledCircle;
				}
				else if (i%12 == 1)
				{
					glyph = TimelineGlyph.OutlinedCircle;
				}
				else if (i%12 == 6)
				{
					glyph = TimelineGlyph.FilledSquare;
				}
				else if (i%12 == 7)
				{
					glyph = TimelineGlyph.OutlinedSquare;
				}

				if (glyph != TimelineGlyph.Empty)
				{
					if (glyph == TimelineGlyph.OutlinedSquare)
					{
						value += 2000.0m;
					}
					else
					{
						value -= value * 0.10m;
					}
				}

				var v = value;

				if (glyph == TimelineGlyph.Empty)
				{
					v = null;
				}

				var d = new TimelineCellDate (AssetsApplication.AddDays (start, i), isSelected: (i == selection));
				var g = new TimelineCellGlyph (glyph, isSelected: (i == selection));
				var x = new TimelineCellValue (v, isSelected: (i == selection));

				dates.Add (d);
				glyphs.Add (g);
				values.Add (x);
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
					row.SetCells (values.ToArray ());
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

		private static Date AddDays(Date date, int numberOfDays)
		{
			return new Date (date.Ticks + Time.TicksPerDay*numberOfDays);
		}


		private BusinessContext					businessContext;
	}
}
