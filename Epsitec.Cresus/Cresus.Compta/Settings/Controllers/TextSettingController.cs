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
	public class TextSettingController : AbstractSettingController
	{
		public TextSettingController(AbstractSettingData data, System.Action actionChanged)
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

			this.field = new TextField
			{
				Parent          = frame,
				FormattedText   = this.Data.Value,
				PreferredWidth  = this.PreferredWidth,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 0, 0, 2),
				TabIndex        = ++this.tabIndex,
			};

			this.CreateError (frame);

			this.field.TextChanged += delegate
			{
				this.Data.EditedValue = this.field.FormattedText;
				this.changedAction ();
			};
		}

		public override void Validate()
		{
			if (this.data.ValidateAction != null)
			{
				var error = this.data.ValidateAction ();
				if (!error.IsNullOrEmpty ())
				{
					this.SetError (error);
					return;
				}
			}

			this.SetError (FormattedText.Null);
			this.Data.Value = this.Data.EditedValue;
		}

	
		private double PreferredWidth
		{
			get
			{
				return System.Math.Min (this.Data.MaxLength * 8, 250);  // on estime la largeur à 8 pixels / caractère
			}
		}

		private TextSettingData Data
		{
			get
			{
				return this.data as TextSettingData;
			}
		}


		private TextField field;
	}
}
