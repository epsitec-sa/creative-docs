//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class LabelStackedController : AbstractStackedController
	{
		public LabelStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
		}


		public override int						RequiredHeight
		{
			get
			{
				return this.description.Label.GetTextHeight (this.description.Width);
			}
		}

		public override int						RequiredLabelsWidth
		{
			get
			{
				return 0;
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			int h = description.Label.GetTextHeight (description.Width);

			//	On laisse un espace libre à gauche, dans la zone des labels.
			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = labelWidth,
				PreferredHeight  = h,
				Margins          = new Margins (0, 10, 3, 0),
			};

			//	On affiche le texte multiligne à droite, dans la zone des contrôles.
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
	}
}