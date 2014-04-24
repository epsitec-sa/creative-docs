//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class TextStackedController : AbstractStackedController
	{
		public TextStackedController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public string							Value;


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			this.controller = new StringFieldController (this.accessor)
			{
				Value      = this.Value,
				LabelWidth = labelWidth,
				Label      = description.Label,
				TabIndex   = tabIndex,
			};

			this.controller.CreateUI (parent);

			if (description.BottomMargin > 0)
			{
				new FrameBox
				{
					Parent  = parent,
					Dock    = DockStyle.Top,
					Margins = new Margins (0, 0, 0, description.BottomMargin),
				};
			}

			this.controller.ValueEdited += delegate
			{
				this.Value = this.controller.Value;
				this.UpdateWidgets ();
			};
		}


		private StringFieldController			controller;
	}
}