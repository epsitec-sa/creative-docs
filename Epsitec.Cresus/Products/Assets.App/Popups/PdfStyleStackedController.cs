//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class PdfStyleStackedController : AbstractStackedController
	{
		public PdfStyleStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
		}


		public PdfStyle							Value
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


		public override int						RequiredHeight
		{
			get
			{
				return PdfStyleStackedController.height;
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
				this.ShowStylePopup ();
			};
		}


		private void ShowStylePopup()
		{
			var popup = new PdfStylePopup (this.accessor)
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
			this.button.Text = " " + PdfStyleHelpers.GetDescription (this.Value);
		}


		private const int height = AbstractFieldController.lineHeight + 4;

		private PdfStyle						value;
		private ColoredButton					button;
	}
}