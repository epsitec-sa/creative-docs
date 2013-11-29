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
	public class CreateObjectPopup : AbstractPopup
	{
		public CreateObjectPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			//	Met par défaut une date au premier janvier de l'année en cours.
			this.ObjectDate = new Timestamp (new System.DateTime (System.DateTime.Now.Year, 1, 1), 0).Date;
		}


		public System.DateTime?					ObjectDate;
		public string							ObjectName;


		protected override Size DialogSize
		{
			get
			{
				return new Size (CreateObjectPopup.PopupWidth, CreateObjectPopup.PopupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Création d'un nouvel objet");

			var line1 = this.CreateFrame (CreateObjectPopup.Margin, 110, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margin*2, DateController.ControllerHeight);
						this.CreateSeparator (0, 90, CreateObjectPopup.PopupWidth);
			var line2 = this.CreateFrame (CreateObjectPopup.Margin, 50, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margin*2, CreateObjectPopup.LineHeight);

			this.CreateDate    (line1);
			this.CreateName    (line2);
			this.CreateButtons ();

			this.UpdateButtons ();
			this.textField.Focus ();
		}

		private void CreateDate(Widget parent)
		{
			var dateController = new DateController (this.accessor)
			{
				DateDescription = "Date d'entrée",
				TabIndex        = 1,
				Date            = this.ObjectDate,
			};

			dateController.CreateUI (parent);

			dateController.DateChanged += delegate
			{
				this.ObjectDate = dateController.Date;
				this.UpdateButtons ();
			};
		}

		private void CreateName(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Nom",
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateObjectPopup.Indent,
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
				TabIndex         = 2,
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
			this.createButton.Enable = this.ObjectDate.HasValue &&
									   !string.IsNullOrEmpty (this.ObjectName);
		}


		private static readonly int LineHeight  = 2+17+2;
		private static readonly int Indent      = DateController.ColumnWidth1;
		private static readonly int PopupWidth  = 320;
		private static readonly int PopupHeight = 153+DateController.ControllerHeight;
		private static readonly int Margin      = 20;

		private readonly DataAccessor					accessor;

		private TextField								textField;
		private Button									createButton;
		private Button									cancelButton;
	}
}