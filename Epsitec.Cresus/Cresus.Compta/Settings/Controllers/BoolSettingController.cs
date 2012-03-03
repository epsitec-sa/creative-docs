//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Settings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public class BoolSettingController : AbstractSettingController
	{
		public BoolSettingController(AbstractSettingData data, System.Action actionChanged)
			: base (data, actionChanged)
		{
			this.Data.EditedValue = this.Data.Value;
		}

		public override void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				TabIndex        = ++this.tabIndex,
			};

			this.button = new CheckButton
			{
				Parent          = frame,
				FormattedText   = VerboseSettings.GetDescription (data.Type),
				PreferredWidth  = 480,
				ActiveState     = this.Data.Value ? ActiveState.Yes : ActiveState.No,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 0, 0, 2),
				TabIndex        = ++this.tabIndex,
			};

			this.CreateError (frame);

			this.button.ActiveStateChanged += delegate
			{
				this.Data.EditedValue = (this.button.ActiveState == ActiveState.Yes);
				this.changedAction ();
			};
		}

		public override void Validate()
		{
			if (this.data.ValidateAction != null)
			{
				var error = this.data.ValidateAction ();
				if (!error.IsNullOrEmpty)
				{
					this.SetError (error);
					return;
				}
			}

			this.SetError (FormattedText.Null);
			this.Data.Value = this.Data.EditedValue;
		}


		private BoolSettingData Data
		{
			get
			{
				return this.data as BoolSettingData;
			}
		}


		private CheckButton button;
	}
}
