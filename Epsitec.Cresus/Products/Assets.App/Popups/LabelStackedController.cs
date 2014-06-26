//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class LabelStackedController : AbstractStackedController
	{
		public LabelStackedController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			int h = description.Label.GetTextHeight (description.Width);

			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = labelWidth,
				PreferredHeight  = h,
				Margins          = new Margins (0, 10, 3, 0),
			};

			new StaticText
			{
				Parent           = parent,
				Text             = description.Label,
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode    = TextBreakMode.None,
				Dock             = DockStyle.Fill,
				PreferredHeight  = h,
			};
		}


		public const int height = AbstractFieldController.lineHeight + 4;
	}
}