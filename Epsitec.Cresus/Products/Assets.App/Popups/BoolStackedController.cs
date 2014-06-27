//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class BoolStackedController : AbstractStackedController
	{
		public BoolStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
		}


		public bool								Value;


		public override int						RequiredHeight
		{
			get
			{
				return BoolStackedController.checkHeight;
			}
		}

		public override int						RequiredControllerWidth
		{
			get
			{
				return 30 + this.description.Label.GetTextWidth ();
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
			var button = new CheckButton
			{
				Parent          = parent,
				Text            = description.Label,
				ActiveState     = this.Value ? ActiveState.Yes : ActiveState.No,
				AutoFocus       = false,
				PreferredHeight = BoolStackedController.checkHeight,
				Dock            = DockStyle.Top,
				Margins         = new Margins (labelWidth+10, 0, 0, 0),
			};

			button.ActiveStateChanged += delegate
			{
				this.Value = button.ActiveState == ActiveState.Yes;
				this.OnValueChanged (description);
			};
		}


		private const int checkHeight = 17;
	}
}