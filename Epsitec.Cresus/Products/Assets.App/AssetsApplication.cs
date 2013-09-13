//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

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
			

			window.Show ();
			window.MakeActive ();
		}


		private BusinessContext					businessContext;
	}
}
