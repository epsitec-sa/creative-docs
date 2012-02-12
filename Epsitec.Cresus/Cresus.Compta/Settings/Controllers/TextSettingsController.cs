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
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Settings.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Settings.Controllers
{
	public class TextSettingsController : AbstractSettingsController
	{
		public TextSettingsController(AbstractSettingsData data, System.Action actionChanged)
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
				this.Data.Value = this.field.FormattedText;
				this.actionChanged ();
			};
		}

		private double PreferredWidth
		{
			get
			{
				return System.Math.Min (this.Data.MaxLength * 8, 250);  // on estime la largeur à 8 pixels / caractère
			}
		}

		private TextSettingsData Data
		{
			get
			{
				return this.data as TextSettingsData;
			}
		}


		private TextField field;
	}
}
