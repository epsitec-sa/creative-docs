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
	/// Ce contrôleur gère le bilan de la comptabilité.
	/// </summary>
	public class BilanController : DoubleController
	{
		public BilanController(Application app, BusinessContext businessContext, MainWindowController mainWindowController)
			: base (app, businessContext, mainWindowController)
		{
			this.dataAccessor = new BilanDataAccessor (this);

			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.Bilan.ViewSettings");
		}


		protected override void CreateOptions(FrameBox parent)
		{
			this.optionsController = new BilanOptionsController (this);
			this.optionsController.CreateUI (parent, this.OptionsChanged);
			this.optionsController.ShowPanel = this.mainWindowController.ShowOptionsPanel;

			this.UpdateColumnMappers ();
		}

		protected override void UpdateTitle()
		{
			this.SetTitle ("Bilan intermédiaire");
			this.SetSubtitle (this.période.ShortTitle);
		}
	}
}
