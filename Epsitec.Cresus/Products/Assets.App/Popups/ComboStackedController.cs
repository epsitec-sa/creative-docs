//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class ComboStackedController : AbstractStackedController
	{
		public ComboStackedController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public int?								Value;


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			this.CreateLabel (parent, labelWidth, description);
			var controllerFrame = this.CreateControllerFrame (parent);

			this.controller = new EnumFieldController (this.accessor)
			{
				Value      = this.Value,
				LabelWidth = 0,
				EditWidth  = description.Width,
				TabIndex   = tabIndex,
			};

			this.InitializeEnums (description);
			this.controller.CreateUI (controllerFrame);

			this.controller.ValueEdited += delegate
			{
				this.Value = this.controller.Value;
				this.OnValueChanged (description);
			};
		}

		public override void SetFocus()
		{
			this.controller.SetFocus ();
		}


		private void InitializeEnums(StackedControllerDescription description)
		{
			var enums = new Dictionary<int, string> ();
			int index = 0;

			foreach (var label in description.Labels)
			{
				enums.Add (index++, label);
			}

			this.controller.Enums = enums;
		}


		private EnumFieldController				controller;
	}
}