//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Options.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les pertes et profits de la comptabilité.
	/// </summary>
	public class PPController : DoubleController
	{
		public PPController(ComptaApplication app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new PPDataAccessor (this);
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new PPOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.mainWindowController.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void UpdateTitle()
		{
			this.SetGroupTitle ("Pertes et Profits");
			this.SetTitle ("Pertes et Profits");
			this.SetSubtitle (this.période.ShortTitle);
		}
	}
}
