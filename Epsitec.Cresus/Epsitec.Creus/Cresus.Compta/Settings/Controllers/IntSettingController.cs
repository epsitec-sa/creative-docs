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
			this.Data.EditedValue = this.Data.Value;
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
					this.Data.EditedValue = i.Value;
				}
				else
				{
					this.Data.EditedValue = -1;
				}

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

			if (this.Data.EditedValue == -1)
			{
				this.SetError ("Ce n'est pas un nombre");
			}
			else if (this.Data.EditedValue < this.Data.MinValue || this.Data.EditedValue > this.Data.MaxValue)
			{
				this.SetError (string.Format ("Doit être compris entre {0} et {1}", this.Data.MinValue.ToString (), this.Data.MaxValue.ToString ()));
			}
			else
			{
				this.SetError (FormattedText.Null);
				this.Data.Value = this.Data.EditedValue;
			}
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
