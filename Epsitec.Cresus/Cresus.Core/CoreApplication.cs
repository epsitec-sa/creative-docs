//	Copyright © 2008-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>CoreApplication</c> class implements the central application
	/// logic.
	/// </summary>
	public partial class CoreApplication : CoreInteractiveApp
	{
		public CoreApplication()
		{
		}


		public override string					ShortWindowTitle
		{
			get
			{
				return Res.Strings.ProductName.ToSimpleText ();
			}
		}

		public override string					ApplicationIdentifier
		{
			get
			{
				return Res.Strings.ProductAppId.ToSimpleText ();
			}
		}

		protected override void InitializeEmptyDatabase()
		{
			Hack.PopulateUsers (new BusinessContext (this.Data));
		}

		protected override void CreateManualComponents(IList<System.Action> initializers)
		{
			var orchestrator = new DataViewOrchestrator (this);

			initializers.Add (() => orchestrator.CreateUI (this.Window.Root));
		}

		protected override void SaveApplicationState(XDocument doc)
		{
			doc.Save (CoreApplication.Paths.SettingsPath);
		}

		protected override XDocument LoadApplicationState()
		{
			if (System.IO.File.Exists (CoreApplication.Paths.SettingsPath))
			{
				return XDocument.Load (CoreApplication.Paths.SettingsPath);
			}
			else
			{
				return null;
			}
		}
	}
}
