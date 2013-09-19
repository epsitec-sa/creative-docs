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
				BackColor = Color.FromName ("LightBlue"),
			};

			this.CreateTestTimeLine (frame);
		}

		private void CreateTestTimeLine(Widget parent)
		{
			var timeline = new Timeline ()
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (10, 10, 335, 10),
				Pivot   = 0.0,
			};

			var list = new List<TimelineCell> ();
			var start = new Date (2013, 3, 20);  // 20 mars 2013

			for (int i = 0; i < 100; i++)
			{
				var cell = new TimelineCell ()
				{
					Date       = AssetsApplication.AddDays (start, i),
					Glyph      = (i % 3 == 0) ? TimelineCellGlyph.FilledCircle : TimelineCellGlyph.Empty,
					IsSelected = (i == 2),
				};

				list.Add (cell);
			}

			timeline.SetCells (list.ToArray ());
		}

		private static Date AddDays(Date date, int numberOfDays)
		{
			return new Date (date.Ticks + Time.TicksPerDay*numberOfDays);
		}


		private BusinessContext					businessContext;
	}
}
