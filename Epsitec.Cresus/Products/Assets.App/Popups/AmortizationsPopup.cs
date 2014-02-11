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

			this.IsAll = true;
		}


		public string							Title;
		public string							ActionOne;
		public string							ActionAll;
		public bool								DateFromAllowed;
		public bool								DateToAllowed;
		public bool								OneSelectionAllowed;
		public bool								IsAll;
		public System.DateTime?					DateFrom;
		public System.DateTime?					DateTo;


		protected override Size DialogSize
		{
			get
			{
				int h = 130;

				if (this.DateFromAllowed || this.DateToAllowed)
				{
					h += 8;  // gap
				}

				if (this.DateFromAllowed)
				{
					h += DateController.controllerHeight+10;
				}

				if (this.DateToAllowed)
				{
					h += DateController.controllerHeight+10;
				}

				return new Size (AmortizationsPopup.popupWidth, h);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle (this.Title);

			int y = 50;

			//	Crée les lignes de bas en haut.
			{
				var line = this.CreateFrame (AmortizationsPopup.margin, y, AmortizationsPopup.popupWidth-AmortizationsPopup.margin*2, AmortizationsPopup.lineHeight);
				this.CreateAll (line);
				y += 21;
			}

			{
				var line = this.CreateFrame (AmortizationsPopup.margin, y, AmortizationsPopup.popupWidth-AmortizationsPopup.margin*2, AmortizationsPopup.lineHeight);
				this.CreateOne (line);
				y += 21;
			}

			if (this.DateFromAllowed || this.DateToAllowed)
			{
				y += 8;  // gap
			}

			if (this.DateToAllowed)
			{
				y += 10;
				var line = this.CreateFrame (AmortizationsPopup.margin, y, 40+10+DateFieldController.controllerWidth, DateController.controllerHeight);
				this.CreateTo (line);
				y += DateController.controllerHeight;
			}

			if (this.DateFromAllowed)
			{
				y += 10;
				var line = this.CreateFrame (AmortizationsPopup.margin, y, 40+10+DateFieldController.controllerWidth, DateController.controllerHeight);
				this.CreateFrom (line);
				y += DateController.controllerHeight;
			}

			this.CreateButtons ();

			this.IsAll = !this.OneSelectionAllowed;

			this.UpdateButtons ();
		}

		private void CreateFrom(Widget parent)
		{
			this.dateFromController = new DateController (this.accessor)
			{
				Date            = this.DateFrom,
				DateLabelWidth  = 40,
				DateDescription = "Depuis",
				TabIndex        = 1,
			};

			this.dateFromController.CreateUI (parent);

			this.dateFromController.DateChanged += delegate
			{
				this.DateFrom = this.dateFromController.Date;
				this.UpdateButtons ();
			};
		}

		private void CreateTo(Widget parent)
		{
			this.dateToController = new DateController (this.accessor)
			{
				Date            = this.DateTo,
				DateLabelWidth  = 40,
				DateDescription = "Jusqu'à",
				TabIndex        = 2,
			};

			this.dateToController.CreateUI (parent);

			this.dateToController.DateChanged += delegate
			{
				this.DateTo = this.dateToController.Date;
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

			this.radioOne.ActiveState = Misc.GetActiveState (!this.IsAll);
			this.radioAll.ActiveState = Misc.GetActiveState (this.IsAll);

			this.okButton.Text = this.IsAll ? this.ActionAll : this.ActionOne;

			this.okButton.Enable = this.DateFrom.HasValue
								&& this.DateTo.HasValue
								&& this.DateFrom < this.DateTo;
		}


		private const int lineHeight = 2+AbstractFieldController.lineHeight+2;
		private const int popupWidth = 330;
		private const int margin     = 20;

		private readonly DataAccessor			accessor;

		private RadioButton						radioOne;
		private RadioButton						radioAll;
		private DateController					dateFromController;
		private DateController					dateToController;
		private Button							okButton;
		private Button							cancelButton;
	}
}