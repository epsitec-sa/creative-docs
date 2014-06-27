﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public DecimalStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
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


		public override int						RequiredHeight
		{
			get
			{
				return DecimalStackedController.height;
			}
		}

		public override int						RequiredControllerWidth
		{
			get
			{
				return DecimalStackedController.width + 10 + 50 + 4;  // place pour les unités
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


		private const int width  = 90;
		private const int height = AbstractFieldController.lineHeight + 4;

		private decimal?						value;
		private DecimalFieldController			controller;
	}
}