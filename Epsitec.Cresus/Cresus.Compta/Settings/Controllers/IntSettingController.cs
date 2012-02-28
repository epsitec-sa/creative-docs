//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Settings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public class IntSettingsController : AbstractSettingController
	{
		public IntSettingsController(AbstractSettingData data, System.Action actionChanged)
			: base (data, actionChanged)
		{
		}

		public override void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent   = parent,
				Dock     = DockStyle.Top,
				TabIndex = ++this.tabIndex,
			};

			this.CreateLabel (frame);

			this.field = new TextFieldUpDown
			{
				Parent          = frame,
				FormattedText   = Converters.IntToString (this.Data.Value),
				PreferredWidth  = 60,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 0, 0, 2),
				TabIndex        = ++this.tabIndex,
			};

			this.CreateError (frame);

			this.field.TextChanged += delegate
			{
				int? i = Converters.ParseInt (this.field.FormattedText);
				if (i.HasValue)
				{
					this.Data.Value = i.Value;
					this.actionChanged ();
				}
			};
		}


		private IntSettingData Data
		{
			get
			{
				return this.data as IntSettingData;
			}
		}


		private TextFieldUpDown field;
	}
}
