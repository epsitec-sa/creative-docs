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
	public class IntStackedController : AbstractStackedController
	{
		public IntStackedController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public int?								Value;


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			this.controller = new IntFieldController (this.accessor)
			{
				Value      = this.Value,
				LabelWidth = labelWidth,
				Label      = description.Label,
				EditWidth  = IntStackedController.width,
				TabIndex   = tabIndex,
			};

			this.controller.CreateUI (parent);

			this.controller.ValueEdited += delegate
			{
				this.Value = this.controller.Value;
				this.UpdateWidgets ();
				this.OnValueChanged (description);
			};
		}


		public const int width  = 50;
		public const int height = AbstractFieldController.lineHeight;

		private IntFieldController				controller;
	}
}