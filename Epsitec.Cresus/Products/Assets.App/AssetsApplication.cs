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
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App
{
	public class AssetsApplication : CoreInteractiveApp
	{
		public AssetsApplication()
		{

		}

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
			policy.UseEmbeddedServer = true;

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
				BackColor = ColorManager.WindowBackgroundColor,
				Name      = "PopupParentFrame",
			};

			var mandat = DummyMandat.GetDummyMandat ();
			var accessor = new DataAccessor (mandat);

			var ui = new AssetsUI (accessor);
			ui.CreateUI (frame);

			//?this.CreateTestTimelineProvider (frame);
		}


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

			//?timeline.SetRows (AssetsApplication.GetRows (true));

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


		private BusinessContext					businessContext;
	}
}
