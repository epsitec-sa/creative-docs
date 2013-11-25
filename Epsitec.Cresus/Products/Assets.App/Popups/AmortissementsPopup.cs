//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class AmortissementsPopup : AbstractPopup
	{
		public AmortissementsPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.isCreate = true;
			this.isAll = true;
		}


		public System.DateTime?					DateFrom;
		public System.DateTime?					DateTo;


		protected override Size DialogSize
		{
			get
			{
				return new Size (AmortissementsPopup.PopupWidth, AmortissementsPopup.PopupHeight);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Amortissements");

			var line1 = this.CreateFrame (AmortissementsPopup.Margin, 171, AmortissementsPopup.PopupWidth-AmortissementsPopup.Margin*2, AmortissementsPopup.LineHeight);
			var line2 = this.CreateFrame (AmortissementsPopup.Margin, 150, AmortissementsPopup.PopupWidth-AmortissementsPopup.Margin*2, AmortissementsPopup.LineHeight);
			var line3 = this.CreateFrame (AmortissementsPopup.Margin, 121, AmortissementsPopup.PopupWidth-AmortissementsPopup.Margin*2, AmortissementsPopup.LineHeight);
			var line4 = this.CreateFrame (AmortissementsPopup.Margin, 100, AmortissementsPopup.PopupWidth-AmortissementsPopup.Margin*2, AmortissementsPopup.LineHeight);
			var line5 = this.CreateFrame (AmortissementsPopup.Margin,  71, AmortissementsPopup.PopupWidth-AmortissementsPopup.Margin*2, AmortissementsPopup.LineHeight);
			var line6 = this.CreateFrame (AmortissementsPopup.Margin,  50, AmortissementsPopup.PopupWidth-AmortissementsPopup.Margin*2, AmortissementsPopup.LineHeight);
			var line7 = this.CreateFrame (0,  0, AmortissementsPopup.PopupWidth, 30);

			this.CreateCreate  (line1);
			this.CreateRemove  (line2);
			this.CreateFrom    (line3);
			this.CreateTo      (line4);
			this.CreateOne     (line5);
			this.CreateAll     (line6);
			this.CreateButtons (line7);

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
				this.isCreate = true;
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
				this.isCreate = false;
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
				this.isAll = false;
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
				this.isAll = true;
				this.UpdateButtons ();
			};
		}

		private void CreateButtons(Widget parent)
		{
			this.okButton = new Button
			{
				Parent        = parent,
				Name          = "ok",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (AmortissementsPopup.PopupWidth/2 - 5, parent.PreferredHeight),
				Margins       = new Margins (0, 5, 0, 0),
			};

			this.cancelButton = new Button
			{
				Parent        = parent,
				Name          = "cancel",
				Text          = "Annuler",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (AmortissementsPopup.PopupWidth/2 - 5, parent.PreferredHeight),
				Margins       = new Margins (5, 0, 0, 0),
			};

			this.okButton.Clicked += this.HandleButtonClicked;
			this.cancelButton.Clicked += this.HandleButtonClicked;
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			var button = sender as Button;

			this.ClosePopup ();
			this.OnButtonClicked (button.Name);
		}


		private void UpdateButtons()
		{
			this.radioCreate.ActiveState =  this.isCreate ? ActiveState.Yes : ActiveState.No;
			this.radioRemove.ActiveState = !this.isCreate ? ActiveState.Yes : ActiveState.No;
			this.radioOne   .ActiveState = !this.isAll    ? ActiveState.Yes : ActiveState.No;
			this.radioAll   .ActiveState =  this.isAll    ? ActiveState.Yes : ActiveState.No;

			if (this.isCreate)
			{
				this.okButton.Text = this.isAll ? "Tout générer" : "Générer un";
			}
			else
			{
				this.okButton.Text = this.isAll ? "Tout supprimer" : "Supprimer un";
			}

			this.okButton.Enable = this.DateFrom.HasValue
								&& this.DateTo.HasValue
								&& this.DateFrom < this.DateTo;
		}


		private static readonly int TitleHeight = 24;
		private static readonly int LineHeight  = 2+17+2;
		private static readonly int PopupWidth  = 300;
		private static readonly int PopupHeight = 230;
		private static readonly int Margin      = 20;

		private readonly DataAccessor						accessor;

		private RadioButton									radioCreate;
		private RadioButton									radioRemove;
		private RadioButton									radioOne;
		private RadioButton									radioAll;
		private DateFieldController							dateFromController;
		private DateFieldController							dateToController;
		private bool										isCreate;
		private bool										isAll;
		private Button										okButton;
		private Button										cancelButton;
	}
}