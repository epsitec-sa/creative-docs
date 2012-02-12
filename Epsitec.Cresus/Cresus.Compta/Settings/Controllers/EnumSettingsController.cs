//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Settings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public class EnumSettingsController : AbstractSettingsController
	{
		public EnumSettingsController(AbstractSettingsData data, System.Action actionChanged)
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

			var field = new TextFieldCombo
			{
				Parent          = frame,
				IsReadOnly      = true,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				FormattedText   = VerboseSettings.GetDescription (this.Data.Value),
				PreferredWidth  = 100,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 0, 0, 2),
				TabIndex        = ++this.tabIndex,
			};

			this.InitializeCombo (field);

			field.SelectedItemChanged += delegate
			{
				this.UpdateValue (field.SelectedItemIndex);
				this.actionChanged ();
			};
		}

		private void InitializeCombo(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			foreach (var e in this.Data.Enum)
			{
				combo.Items.Add (VerboseSettings.GetDescription (e));
			}
		}

		private void UpdateValue(int sel)
		{
			if (sel >= 0 && sel < this.Data.Enum.Count ())
			{
				this.Data.Value = this.Data.Enum.ElementAt (sel);
			}
		}

		private EnumSettingsData Data
		{
			get
			{
				return this.data as EnumSettingsData;
			}
		}
	}
}
