//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class MarginsStackedController : AbstractStackedController
	{
		public MarginsStackedController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public Margins							Value
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

					if (this.button != null)
					{
						this.UpdateButton ();
					}
				}
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			this.CreateLabel (parent, labelWidth, description);
			var controllerFrame = this.CreateControllerFrame (parent);

			this.button = new ColoredButton
			{
				Parent           = controllerFrame,
				ContentAlignment = ContentAlignment.MiddleLeft,
				NormalColor      = ColorManager.ToolbarBackgroundColor,
				HoverColor       = ColorManager.HoverColor,
				Dock             = DockStyle.Fill,
			};

			this.UpdateButton ();

			this.button.Clicked += delegate
			{
				this.ShowMarginsPopup (description.Label);
			};
		}


		private void ShowMarginsPopup(string title)
		{
			var popup = new MarginsPopup (this.accessor, title)
			{
				Value = this.Value,
			};

			popup.Create (this.button, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.Value = popup.Value;
				}
			};
		}

		private void UpdateButton()
		{
			this.button.Text = " " + MarginsPopup.GetDescription (this.Value);
		}


		public const int height = AbstractFieldController.lineHeight + 4;

		private Margins							value;
		private ColoredButton					button;
	}
}