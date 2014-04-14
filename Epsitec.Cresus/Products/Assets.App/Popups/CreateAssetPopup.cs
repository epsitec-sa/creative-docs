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
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'un
	/// nouvel objet, à savoir la date d'entrée et le nom de l'objet.
	/// </summary>
	public class CreateAssetPopup : AbstractPopup
	{
		public CreateAssetPopup(DataAccessor accessor)
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
				return new Size (CreateAssetPopup.popupWidth, CreateAssetPopup.popupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Création d'un nouvel objet");

			var line1 = this.CreateFrame (CreateAssetPopup.margin, 77, CreateAssetPopup.popupWidth-CreateAssetPopup.margin*2, DateController.controllerHeight);
			var line2 = this.CreateFrame (CreateAssetPopup.margin, 50, CreateAssetPopup.popupWidth-CreateAssetPopup.margin*2, CreateAssetPopup.lineHeight);

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
				DateLabelWidth  = CreateAssetPopup.indent,
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
				PreferredWidth   = CreateAssetPopup.indent,
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


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<System.DateTime, string> action)
		{
			if (target != null)
			{
				var popup = new CreateAssetPopup (accessor);

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "create")
					{
						action (popup.ObjectDate.Value, popup.ObjectName);
					}
				};
			}
		}
		#endregion


		private const int margin      = 20;
		private const int lineHeight  = 2 + AbstractFieldController.lineHeight + 2;
		private const int indent      = 71;
		private const int popupWidth  = CreateAssetPopup.margin*2 + CreateAssetPopup.indent + 10 + DateController.controllerWidth;
		private const int popupHeight = 120 + DateController.controllerHeight;

		private readonly DataAccessor			accessor;

		private TextField						textField;
		private Button							createButton;
		private Button							cancelButton;
	}
}