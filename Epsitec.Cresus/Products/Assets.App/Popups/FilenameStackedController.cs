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
	public class FilenameStackedController : AbstractStackedController
	{
		public FilenameStackedController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public string							Value;


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			this.CreateLabel (parent, labelWidth, description);
			var controllerFrame = this.CreateControllerFrame (parent);

			this.controller = new StringFieldController (this.accessor)
			{
				Value      = this.Value,
				LabelWidth = 0,
				EditWidth  = description.Width - FilenameStackedController.browseWidth - 4,
				TabIndex   = tabIndex,
			};

			this.controller.CreateUI (controllerFrame);
			this.CreateBrowser (parent);

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


		private void CreateBrowser(Widget parent)
		{
			var button = new Button
			{
				Parent           = parent,
				Text             = "Parcourir...",
				Dock             = DockStyle.Right,
				ButtonStyle      = ButtonStyle.Icon,
				AutoFocus        = false,
				PreferredWidth   = FilenameStackedController.browseWidth - 2,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (2, 0, 0, 0),
			};

			button.Clicked += delegate
			{
				this.ShowFilenameDialog ();
			};
		}

		private void ShowFilenameDialog()
		{
		}


		private const int browseWidth = 75;

		private StringFieldController			controller;
	}
}