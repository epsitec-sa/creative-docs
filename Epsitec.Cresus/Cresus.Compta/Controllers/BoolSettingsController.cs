//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	public class BoolSettingsController : AbstractSettingsController
	{
		public BoolSettingsController(AbstractSettingsData data)
			: base (data)
		{
		}

		public override void CreateUI(Widget parent)
		{
			var button = new CheckButton
			{
				Parent          = parent,
				FormattedText   = VerboseSettings.GetDescription (data.Name),
				PreferredHeight = 20,
				ActiveState     = this.Data.Value ? ActiveState.Yes : ActiveState.No,
				Dock            = DockStyle.Top,
//?				Margins         = new Margins (AbstractSettingsController.labelWidth, 0, 0, 2),
				Margins         = new Margins (0, 0, 0, 2),
				TabIndex        = ++this.tabIndex,
			};

			button.ActiveStateChanged += delegate
			{
				this.Data.Value = (button.ActiveState == ActiveState.Yes);
			};
		}

		private BoolSettingsData Data
		{
			get
			{
				return this.data as BoolSettingsData;
			}
		}
	}
}
