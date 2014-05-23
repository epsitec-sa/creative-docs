//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.Export;
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
		public ExportFormat						Format;


		public override void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
			this.parent = parent;
			this.description = description;

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

		public void Update()
		{
			this.controller.Value = this.Value;
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
			//	Affiche le dialogue Windows standard permettant de choisir un fichier à enregistrer.
			var ext  = ExportInstructionsPopup.GetFormatExt  (this.Format);
			var name = ExportInstructionsPopup.GetFormatName (this.Format);

			var dialog = new FileSaveDialog
			{
				InitialDirectory     = System.IO.Path.GetDirectoryName (this.Value),
				FileName             = System.IO.Path.GetFileName (this.Value),
				DefaultExt           = ext,
				Title                = "Nom du fichier à exporter",
				PromptForOverwriting = true,
				OwnerWindow          = this.parent.Window,
			};

			var filter = new FilterItem (ext, name, ext);
			dialog.Filters.Add (filter);
			dialog.FilterIndex = 0;

			dialog.OpenDialog ();

			if (dialog.Result == DialogResult.Accept)
			{
				this.SetValue (dialog.FileName);
			}
		}

		private void SetValue(string value)
		{
			this.Value = value;
			this.controller.Value = value;

			this.OnValueChanged (this.description);
		}


		private const int browseWidth = 75;

		private Widget							parent;
		private StackedControllerDescription	description;
		private StringFieldController			controller;
	}
}