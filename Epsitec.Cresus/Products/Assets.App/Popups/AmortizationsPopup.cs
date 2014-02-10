//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class AmortizationsPopup : AbstractPopup
	{
		public AmortizationsPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.IsCreate = true;
			this.IsAll    = true;
		}


		public bool								OneSelectionAllowed;
		public bool								IsCreate;
		public bool								IsAll;
		public System.DateTime?					DateFrom;
		public System.DateTime?					DateTo;


		protected override Size DialogSize
		{
			get
			{
				return new Size (AmortizationsPopup.popupWidth, AmortizationsPopup.popupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Amortissements");

			var line1 = this.CreateFrame (AmortizationsPopup.margin, 171, AmortizationsPopup.popupWidth-AmortizationsPopup.margin*2, AmortizationsPopup.lineHeight);
			var line2 = this.CreateFrame (AmortizationsPopup.margin, 150, AmortizationsPopup.popupWidth-AmortizationsPopup.margin*2, AmortizationsPopup.lineHeight);
			var line3 = this.CreateFrame (AmortizationsPopup.margin, 121, 40+10+DateFieldController.controllerWidth, AmortizationsPopup.lineHeight);
			var line4 = this.CreateFrame (AmortizationsPopup.margin, 100, 40+10+DateFieldController.controllerWidth, AmortizationsPopup.lineHeight);
			var line5 = this.CreateFrame (AmortizationsPopup.margin,  71, AmortizationsPopup.popupWidth-AmortizationsPopup.margin*2, AmortizationsPopup.lineHeight);
			var line6 = this.CreateFrame (AmortizationsPopup.margin,  50, AmortizationsPopup.popupWidth-AmortizationsPopup.margin*2, AmortizationsPopup.lineHeight);

			this.CreateCreate  (line1);
			this.CreateRemove  (line2);
			this.CreateFrom    (line3);
			this.CreateTo      (line4);
			this.CreateOne     (line5);
			this.CreateAll     (line6);
			this.CreateButtons ();

			this.IsAll = !this.OneSelectionAllowed;

			this.UpdateButtons ();
		}

		private void CreateCreate(Widget parent)
		{
			this.radioCreate = new RadioButton
			{
				Parent     = parent,
				Text       = "Générer les amortissements automatiques",
				AutoToggle = false,
				Dock       = DockStyle.Fill,
			};

			this.radioCreate.Clicked += delegate
			{
				this.IsCreate = true;
				this.UpdateButtons ();
			};
		}

		private void CreateRemove(Widget parent)
		{
			this.radioRemove = new RadioButton
			{
				Parent     = parent,
				Text       = "Supprimer les amortissements automatiques",
				AutoToggle = false,
				Dock       = DockStyle.Fill,
			};

			this.radioRemove.Clicked += delegate
			{
				this.IsCreate = false;
				this.UpdateButtons ();
			};
		}

		private void CreateFrom(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Du",
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = 40,
				Margins          = new Margins (0, 10, 0, 0),
				Dock             = DockStyle.Left,
			};

			var frame = new FrameBox
			{
				Parent    = parent,
				Dock      = DockStyle.Fill,
				BackColor = ColorManager.WindowBackgroundColor,
			};

			this.dateFromController = new DateFieldController
			{
				Label      = null,
				LabelWidth = 0,
				Value      = this.DateFrom,
			};

			this.dateFromController.HideAdditionalButtons = true;
			this.dateFromController.CreateUI (frame);

			this.dateFromController.ValueEdited += delegate
			{
				this.DateFrom = this.dateFromController.Value;
				this.UpdateButtons ();
			};
		}

		private void CreateTo(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Au",
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = 40,
				Margins          = new Margins (0, 10, 0, 0),
				Dock             = DockStyle.Left,
			};

			var frame = new FrameBox
			{
				Parent    = parent,
				Dock      = DockStyle.Fill,
				BackColor = ColorManager.WindowBackgroundColor,
			};

			this.dateToController = new DateFieldController
			{
				Label      = null,
				LabelWidth = 0,
				Value      = this.DateTo,
			};

			this.dateToController.HideAdditionalButtons = true;
			this.dateToController.CreateUI (frame);

			this.dateToController.ValueEdited += delegate
			{
				this.DateTo = this.dateToController.Value;
				this.UpdateButtons ();
			};
		}

		private void CreateOne(Widget parent)
		{
			this.radioOne = new RadioButton
			{
				Parent     = parent,
				Text       = "Pour l'objet sélectionné",
				AutoToggle = false,
				Dock       = DockStyle.Fill,
			};

			this.radioOne.Clicked += delegate
			{
				this.IsAll = false;
				this.UpdateButtons ();
			};
		}

		private void CreateAll(Widget parent)
		{
			this.radioAll = new RadioButton
			{
				Parent     = parent,
				Text       = "Pour tous les objets",
				AutoToggle = false,
				Dock       = DockStyle.Fill,
			};

			this.radioAll.Clicked += delegate
			{
				this.IsAll = true;
				this.UpdateButtons ();
			};
		}

		private void CreateButtons()
		{
			var footer = this.CreateFooter ();

			this.okButton     = this.CreateFooterButton (footer, DockStyle.Left,  "ok",     null);
			this.cancelButton = this.CreateFooterButton (footer, DockStyle.Right, "cancel", "Annuler");
		}


		private void UpdateButtons()
		{
			this.radioOne.Enable = this.OneSelectionAllowed;

			this.radioCreate.ActiveState = Misc.GetActiveState ( this.IsCreate);
			this.radioRemove.ActiveState = Misc.GetActiveState (!this.IsCreate);
			this.radioOne   .ActiveState = Misc.GetActiveState (!this.IsAll   );
			this.radioAll   .ActiveState = Misc.GetActiveState ( this.IsAll   );

			if (this.IsCreate)
			{
				this.okButton.Text = this.IsAll ? "Tout générer" : "Générer un";
			}
			else
			{
				this.okButton.Text = this.IsAll ? "Tout supprimer" : "Supprimer un";
			}

			this.okButton.Enable = this.DateFrom.HasValue
								&& this.DateTo.HasValue
								&& this.DateFrom < this.DateTo;
		}


		private const int lineHeight  = 2+AbstractFieldController.lineHeight+2;
		private const int popupWidth  = 300;
		private const int popupHeight = 230;
		private const int margin      = 20;

		private readonly DataAccessor			accessor;

		private RadioButton						radioCreate;
		private RadioButton						radioRemove;
		private RadioButton						radioOne;
		private RadioButton						radioAll;
		private DateFieldController				dateFromController;
		private DateFieldController				dateToController;
		private Button							okButton;
		private Button							cancelButton;
	}
}