//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'un
	/// nouveau contact, à savoir son nom.
	/// </summary>
	public class CreatePersonPopup : AbstractPopup
	{
		public CreatePersonPopup(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public string							ObjectName;
		public Guid								ObjectModel;


		protected override Size DialogSize
		{
			get
			{
				return new Size (CreatePersonPopup.popupWidth, CreatePersonPopup.popupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Création d'un nouveau contact");

			var line1 = this.CreateFrame (CreatePersonPopup.margin, 50, CreatePersonPopup.popupWidth-CreatePersonPopup.margin*2, CreatePersonPopup.lineHeight);

			this.CreateName    (line1);
			this.CreateButtons ();

			this.UpdateButtons ();
			this.textField.Focus ();
		}

		private void CreateName(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Nom",
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreatePersonPopup.indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var frame = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
				BackColor        = ColorManager.WindowBackgroundColor,
				Padding          = new Margins (2),
			};

			this.textField = new TextField
			{
				Parent           = frame,
				Dock             = DockStyle.Fill,
			};

			this.textField.TextChanged += delegate
			{
				this.ObjectName = this.textField.Text;
				this.UpdateButtons ();
			};
		}

		private void CreateButtons()
		{
			var footer = this.CreateFooter ();

			this.createButton = this.CreateFooterButton (footer, DockStyle.Left,  "create", "Créer");
			this.cancelButton = this.CreateFooterButton (footer, DockStyle.Right, "cancel", "Annuler");
		}


		private void UpdateButtons()
		{
			this.createButton.Enable = !string.IsNullOrEmpty (this.ObjectName);
		}


		private const int lineHeight  = 2+AbstractFieldController.lineHeight+2;
		private const int indent      = 40;
		private const int popupWidth  = 310;
		private const int popupHeight = 120;
		private const int margin      = 20;

		private readonly DataAccessor						accessor;

		private TextField									textField;
		private Button										createButton;
		private Button										cancelButton;
	}
}