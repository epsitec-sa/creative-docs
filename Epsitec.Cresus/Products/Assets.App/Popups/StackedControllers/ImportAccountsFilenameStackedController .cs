//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Dialogs;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class ImportAccountsFilenameStackedController  : AbstractStackedController
	{
		public ImportAccountsFilenameStackedController(DataAccessor accessor, StackedControllerDescription description)
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
				return ImportAccountsFilenameStackedController.height;
			}
		}


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
				EditWidth  = description.Width - ImportAccountsFilenameStackedController.browseWidth,
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
				PreferredWidth   = ImportAccountsFilenameStackedController .browseWidth - 2,
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
			//	Affiche le dialogue permettant de choisir un fichier à exporter.
			const string title      = "Nom du plan comptable Crésus à importer";
			string initialDirectory = string.IsNullOrEmpty (this.Value) ? null : System.IO.Path.GetDirectoryName (this.Value);
			string filename         = string.IsNullOrEmpty (this.Value) ? null : System.IO.Path.GetFileName (this.Value);
			const string ext        = ".crp";
			const string formatName = "Plan comptable Crésus";

			var f = FileOpenDialog.ShowDialog (this.parent.Window, title, initialDirectory, filename, ext, formatName);

			if (!string.IsNullOrEmpty (f))
			{
				this.SetValue (f);
			}
		}

		private void SetValue(string value)
		{
			this.Value = value;
			this.controller.Value = value;

			this.OnValueChanged (this.description);
		}


		private const int height = AbstractFieldController.lineHeight + 4;
		private const int browseWidth = 75;

		private string							value;
		private Widget							parent;
		private StackedControllerDescription	description;
		private StringFieldController			controller;
	}
}