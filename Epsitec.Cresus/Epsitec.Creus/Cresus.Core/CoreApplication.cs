//	Copyright © 2008-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;
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
			Hack.PopulateUsers (this.Data.CreateDataContext ("hack"));
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
