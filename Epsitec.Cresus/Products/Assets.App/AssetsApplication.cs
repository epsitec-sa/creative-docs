//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Assets.Data.Entities;

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
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


		protected override Window CreateWindow(Size size)
		{
			var window = base.CreateWindow (new Size (1000, 700));

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

			AssetsApplication.SelectedMandat = AssetsApplication.Factories.ToList ().FindIndex (x => x.IsDefault);

			var accessor = new DataAccessor();
			AssetsApplication.InitializeMandat (accessor, AssetsApplication.SelectedMandat, "Exemple", new System.DateTime (2010, 1, 1));

			var ui = new AssetsUI (accessor);
			ui.CreateUI (frame);
		}


		public static int SelectedMandat;

		public static int MandatCount
		{
			get
			{
				return AssetsApplication.Factories.Count ();
			}
		}

		public static string GetMandatName(int rank)
		{
			return AssetsApplication.Factories.ToArray ()[rank].Name;
		}

		public static void InitializeMandat(DataAccessor accessor, int rank, string name, System.DateTime startDate)
		{
			AssetsApplication.Factories.ToArray ()[rank].Create (accessor, name, startDate);
		}


		private static IEnumerable<MandatFactory> Factories
		{
			get
			{
				yield return new MandatFactory
				{
					Name = "MCH2 vide",
					Create = delegate (DataAccessor accessor, string name, System.DateTime startDate)
					{
						using (var factory = new MCH2MandatFactory (accessor))
						{
							factory.Create (name, startDate, false);
						}
					},
				};

				yield return new MandatFactory
				{
					Name = "MCH2 avec exemples",
					IsDefault = true,
					Create = delegate (DataAccessor accessor, string name, System.DateTime startDate)
					{
						using (var factory = new MCH2MandatFactory (accessor))
						{
							factory.Create (name, startDate, true);
						}
					},
				};

				yield return new MandatFactory
				{
					Name = "Entreprise vide",
					Create = delegate (DataAccessor accessor, string name, System.DateTime startDate)
					{
						using (var factory = new CompanyMandatFactory (accessor))
						{
							factory.Create (name, startDate, false);
						}
					},
				};

				yield return new MandatFactory
				{
					Name = "Entreprise avec exemples",
					Create = delegate (DataAccessor accessor, string name, System.DateTime startDate)
					{
						using (var factory = new CompanyMandatFactory (accessor))
						{
							factory.Create (name, startDate, true);
						}
					},
				};
			}
		}

		private class MandatFactory
		{
			public string						Name;
			public bool							IsDefault;
			public System.Action<DataAccessor, string, System.DateTime> Create;
		}


		private BusinessContext					businessContext;
	}
}
