//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class LockedPopup : AbstractPopup
	{
		public LockedPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.IsAll = true;
			this.IsDelete = false;
		}


		public bool								OneSelectionAllowed;
		public bool								IsAll;
		public bool								IsDelete;
		public System.DateTime?					Date;


		protected override Size DialogSize
		{
			get
			{
				int h = 172 + 8 + DateController.controllerHeight+10 + 8;
				return new Size (LockedPopup.popupWidth, h);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Gestion des verrous");

			int y = 50;

			//	Crée les lignes de bas en haut.
			{
				var line = this.CreateFrame (LockedPopup.margin, y, LockedPopup.popupWidth-LockedPopup.margin*2, LockedPopup.lineHeight);
				this.CreateAll (line);
				y += 21;
			}

			{
				var line = this.CreateFrame (LockedPopup.margin, y, LockedPopup.popupWidth-LockedPopup.margin*2, LockedPopup.lineHeight);
				this.CreateOne (line);
				y += 21;
			}

			y += 8;  // gap

			{
				y += 10;
				this.dateFrame = this.CreateFrame (LockedPopup.margin, y, 40+10+DateFieldController.controllerWidth, DateController.controllerHeight);
				this.CreateDate (this.dateFrame);
				y += DateController.controllerHeight;
			}

			y += 8;  // gap

			{
				var line = this.CreateFrame (LockedPopup.margin, y, LockedPopup.popupWidth-LockedPopup.margin*2, LockedPopup.lineHeight);
				this.CreateDelete (line);
				y += 21;
			}

			{
				var line = this.CreateFrame (LockedPopup.margin, y, LockedPopup.popupWidth-LockedPopup.margin*2, LockedPopup.lineHeight);
				this.CreateCreate (line);
				y += 21;
			}

			this.CreateButtons ();

			this.IsAll = !this.OneSelectionAllowed;

			this.UpdateButtons ();
		}

		private void CreateCreate(Widget parent)
		{
			this.radioCreate = new RadioButton
			{
				Parent     = parent,
				Text       = "Verrouiller",
				AutoToggle = false,
				Dock       = DockStyle.Fill,
			};

			this.radioCreate.Clicked += delegate
			{
				this.IsDelete = false;
				this.UpdateButtons ();
			};
		}

		private void CreateDelete(Widget parent)
		{
			this.radioDelete = new RadioButton
			{
				Parent     = parent,
				Text       = "Déverrouiller",
				AutoToggle = false,
				Dock       = DockStyle.Fill,
			};

			this.radioDelete.Clicked += delegate
			{
				this.IsDelete = true;
				this.UpdateButtons ();
			};
		}

		private void CreateDate(Widget parent)
		{
			this.dateController = new DateController (this.accessor)
			{
				Date            = this.Date,
				DateLabelWidth  = LockedPopup.indent,
				DateDescription = "Jusqu'au",
				TabIndex        = 1,
			};

			this.dateController.CreateUI (parent);

			this.dateController.DateChanged += delegate
			{
				this.Date = this.dateController.Date;
				this.UpdateButtons ();
			};
		}

		private void CreateOne(Widget parent)
		{
			this.radioOne = new RadioButton
			{
				Parent     = parent,
				Text       = "L'objet sélectionné",
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
				Text       = "Tous les objets",
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

			this.radioCreate.ActiveState = Misc.GetActiveState (!this.IsDelete);
			this.radioDelete.ActiveState = Misc.GetActiveState (this.IsDelete);

			this.radioOne.ActiveState = Misc.GetActiveState (!this.IsAll);
			this.radioAll.ActiveState = Misc.GetActiveState (this.IsAll);

			this.dateFrame.Visibility = !this.IsDelete;

			if (this.IsDelete)
			{
				this.okButton.Text = this.IsAll ? "Tout déverrouiller" : "Déverrouiller un";
			}
			else
			{
				this.okButton.Text = this.IsAll ? "Tout verrouiller" : "Verrouiller un";
			}

			this.okButton.Enable = this.IsDelete || this.Date.HasValue;
		}


		private const int lineHeight = 2+AbstractFieldController.lineHeight+2;
		private const int indent     = 51;
		private const int popupWidth = LockedPopup.margin*2 + 10 + LockedPopup.indent + DateController.controllerWidth;
		private const int margin     = 20;

		private readonly DataAccessor			accessor;

		private RadioButton						radioCreate;
		private RadioButton						radioDelete;
		private RadioButton						radioOne;
		private RadioButton						radioAll;
		private FrameBox						dateFrame;
		private DateController					dateController;
		private Button							okButton;
		private Button							cancelButton;
	}
}