//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Dialogs;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class FilenameStackedController  : AbstractStackedController
	{
		public FilenameStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
		}


		public string							DialogTitle;
		public string							DialogExtensions;
		public string							DialogFormatName;
		public bool								Save;

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
				return FilenameStackedController.height;
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, ref int tabIndex)
		{
			this.parent = parent;

			this.CreateLabel (parent, labelWidth);
			var controllerFrame = this.CreateControllerFrame (parent);

			this.controller = new StringFieldController (this.accessor)
			{
				Value      = this.Value,
				LabelWidth = 0,
				EditWidth  = this.description.Width - FilenameStackedController.browseWidth - 4,
				TabIndex   = tabIndex,
			};

			this.controller.CreateUI (controllerFrame);
			tabIndex = this.controller.TabIndex;

			this.CreateBrowser (parent, ++tabIndex);

			this.controller.ValueEdited += delegate
			{
				this.Value = this.controller.Value;
				this.OnValueChanged ();
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
				Parent          = parent,
				Text            = Res.Strings.FilenameStackedController.Browse.ToString (),
				Dock            = DockStyle.Right,
				TabIndex        = tabIndex,
				PreferredWidth  = FilenameStackedController.browseWidth - 2,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (2, 0, 0, 0),
			};

			button.Clicked += delegate
			{
				this.ShowFilenameDialog ();
			};
		}

		private void ShowFilenameDialog()
		{
			//	Affiche le dialogue permettant de choisir le fichier.
			string initialDirectory = string.IsNullOrEmpty (this.Value) ? null : System.IO.Path.GetDirectoryName (this.Value);
			string filename         = string.IsNullOrEmpty (this.Value) ? null : System.IO.Path.GetFileName (this.Value);

			if (this.Save)
			{
				var f = FileSaveDialog.ShowDialog (this.parent.Window, this.DialogTitle, initialDirectory, filename, this.DialogExtensions, this.DialogFormatName);

				if (!string.IsNullOrEmpty (f))
				{
					this.SetValue (f);
				}
			}
			else
			{
				var f = FileOpenDialog.ShowDialog (this.parent.Window, this.DialogTitle, initialDirectory, filename, this.DialogExtensions, this.DialogFormatName);

				if (!string.IsNullOrEmpty (f))
				{
					this.SetValue (f);
				}
			}
		}

		private void SetValue(string value)
		{
			this.Value = value;
			this.controller.Value = value;

			this.OnValueChanged ();
		}


		private const int height = AbstractFieldController.lineHeight + 4;
		private const int browseWidth = 75;

		private string							value;
		private Widget							parent;
		private StringFieldController			controller;
	}
}