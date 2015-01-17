//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class TextStackedController : AbstractStackedController
	{
		public TextStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
		}


		public string							Value
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
				return TextStackedController.height;
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, ref int tabIndex)
		{
			this.CreateLabel (parent, labelWidth);
			var controllerFrame = this.CreateControllerFrame (parent);

			this.controller = new StringFieldController (this.accessor)
			{
				Value      = this.Value,
				LabelWidth = 0,
				EditWidth  = this.description.Width - 4,
				TabIndex   = tabIndex,
			};

			this.controller.CreateUI (controllerFrame);
			tabIndex = this.controller.TabIndex;

			this.controller.ValueEdited += delegate
			{
				this.Value = this.controller.Value;
				this.OnValueChanged ();
			};
		}

		public override void SetFocus()
		{
			this.controller.SetFocus ();
		}


		private const int height = AbstractFieldController.lineHeight + 4;

		private string							value;
		private StringFieldController			controller;
	}
}