//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class DecimalStackedController : AbstractStackedController
	{
		public DecimalStackedController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public decimal?							Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;

					if (this.controller != null)
					{
						this.controller.Value = this.Value;
					}
				}
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			this.CreateLabel (parent, labelWidth, description);
			var controllerFrame = this.CreateControllerFrame (parent);

			this.controller = new DecimalFieldController (this.accessor)
			{
				Value         = this.Value,
				DecimalFormat = description.DecimalFormat,
				LabelWidth    = 0,
				EditWidth     = DecimalStackedController.width,
				TabIndex      = tabIndex,
			};

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


		public const int width  = 90;
		public const int height = AbstractFieldController.lineHeight + 4;

		private decimal?						value;
		private DecimalFieldController			controller;
	}
}