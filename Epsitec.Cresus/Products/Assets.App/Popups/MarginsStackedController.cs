//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class MarginsStackedController : AbstractStackedController
	{
		public MarginsStackedController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public Margins							Value;


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
				this.ShowMarginsPopup ();
			};
		}


		private void ShowMarginsPopup()
		{
			var popup = new MarginsPopup (this.accessor, "Marges dans la page")
			{
				Value = this.Value,
			};

			popup.Create (this.button, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.Value = popup.Value;
					this.UpdateButton ();
				}
			};
		}

		private string Summary
		{
			get
			{
				if (this.Value.Left == this.Value.Right &&
					this.Value.Left == this.Value.Top   &&
					this.Value.Left == this.Value.Bottom)
				{
					var l = TypeConverters.DecimalToString ((decimal) this.Value.Left);

					return string.Format (" {0} mm", l);
				}
				else
				{
					var l = TypeConverters.DecimalToString ((decimal) this.Value.Left);
					var r = TypeConverters.DecimalToString ((decimal) this.Value.Right);
					var t = TypeConverters.DecimalToString ((decimal) this.Value.Top);
					var b = TypeConverters.DecimalToString ((decimal) this.Value.Bottom);

					return string.Format (" {0} mm, {1} mm, {2} mm, {3} mm", l, r, t, b);
				}
			}
		}

		private void UpdateButton()
		{
			this.button.Text = this.Summary;
		}


		public const int height = AbstractFieldController.lineHeight + 4;

		private ColoredButton					button;
	}
}