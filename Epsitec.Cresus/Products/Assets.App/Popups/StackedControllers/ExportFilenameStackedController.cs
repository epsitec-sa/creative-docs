//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Dialogs;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class ExportFilenameStackedController : AbstractStackedController
	{
		public ExportFilenameStackedController(DataAccessor accessor, StackedControllerDescription description)
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

		public ExportFormat						Format;


		public override int						RequiredHeight
		{
			get
			{
				return ExportFilenameStackedController.height;
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, ref int tabIndex, StackedControllerDescription description)
		{
			this.parent = parent;
			this.description = description;

			this.CreateLabel (parent, labelWidth, description);
			var controllerFrame = this.CreateControllerFrame (parent);

			this.controller = new StringFieldController (this.accessor)
			{
				Value      = this.Value,
				LabelWidth = 0,
				EditWidth  = description.Width - ExportFilenameStackedController.browseWidth,
				TabIndex   = ++tabIndex,
			};

			this.controller.CreateUI (controllerFrame);
			this.CreateBrowser (parent, ++tabIndex);

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


		private void CreateBrowser(Widget parent, int tabIndex)
		{
			var button = new ColoredButton
			{
				Parent           = parent,
				Text             = "Parcourir...",
				NormalColor      = ColorManager.ToolbarBackgroundColor,
				HoverColor       = ColorManager.HoverColor,
				Dock             = DockStyle.Right,
				TabIndex         = tabIndex,
				PreferredWidth   = ExportFilenameStackedController.browseWidth - 2,
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
			const string title      = "Nom du fichier à exporter";
			string initialDirectory = string.IsNullOrEmpty (this.Value) ? null : System.IO.Path.GetDirectoryName (this.Value);
			string filename         = string.IsNullOrEmpty (this.Value) ? null : System.IO.Path.GetFileName (this.Value);
			string ext              = ExportInstructionsHelpers.GetFormatExt  (this.Format);
			string formatName       = ExportInstructionsHelpers.GetFormatName (this.Format);

			var f = FileSaveDialog.ShowDialog (this.parent.Window, title, initialDirectory, filename, ext, formatName);

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