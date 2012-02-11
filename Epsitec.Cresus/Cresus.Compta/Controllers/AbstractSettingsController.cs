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
	public abstract class AbstractSettingsController
	{
		public AbstractSettingsController(AbstractSettingsData data)
		{
			this.data = data;
		}

		public virtual void CreateUI(Widget parent)
		{
		}

		protected void CreateLabel(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				FormattedText    = VerboseSettings.GetDescription (this.data.Name),
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = AbstractSettingsController.labelWidth-10,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};
		}


		protected readonly static int labelWidth = 210;

		protected readonly AbstractSettingsData	data;

		protected int tabIndex;
	}
}
