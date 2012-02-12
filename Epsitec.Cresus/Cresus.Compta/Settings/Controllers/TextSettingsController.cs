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
		public TextSettingsController(AbstractSettingsData data)
			: base (data)
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

			var width = this.PreferredWidth;

			var field = new TextField
			{
				Parent          = frame,
				FormattedText   = this.Data.Value,
				PreferredWidth  = width.GetValueOrDefault (100),
				PreferredHeight = 20,
				Dock            = width.HasValue ? DockStyle.Left : DockStyle.Fill,
				Margins         = new Margins (0, 0, 0, 2),
				TabIndex        = ++this.tabIndex,
			};

			field.TextChanged += delegate
			{
				this.Data.Value = field.FormattedText;
			};
		}

		private double? PreferredWidth
		{
			get
			{
				if (this.Data.MaxLength < 100)
				{
					return this.Data.MaxLength * 8;  // on estime la largeur à 8 pixels / caractère
				}
				else
				{
					return null;  // infini
				}
			}
		}

		private TextSettingsData Data
		{
			get
			{
				return this.data as TextSettingsData;
			}
		}
	}
}
