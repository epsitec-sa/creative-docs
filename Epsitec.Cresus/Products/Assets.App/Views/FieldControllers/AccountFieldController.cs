//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	public class AccountFieldController : AbstractFieldController
	{
		public AccountFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public System.DateTime					Date;

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
					this.UpdateButtons ();
				}
			}
		}

		protected override void ClearValue()
		{
			this.Value = null;
			this.OnValueEdited (this.Field);
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			if (this.button != null)
			{
				AbstractFieldController.UpdateButton (this.button, this.PropertyState, this.isReadOnly);
			}

			this.UpdateButtons ();
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.button = new ColoredButton
			{
				Parent           = this.frameBox,
				HoverColor       = ColorManager.HoverColor,
				ContentAlignment = ContentAlignment.MiddleLeft,
				Dock             = DockStyle.Left,
				PreferredWidth   = this.EditWidth-AbstractFieldController.lineHeight,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 10, 0, 0),
				TabIndex         = this.TabIndex,
				Text             = this.value,
			};

			//	Petit triangle "v" par-dessus la droite du bouton principal, sans fond
			//	afin de prendre la couleur du bouton principal.
			var arrowButton = new GlyphButton
			{
				Parent           = this.button,
				GlyphShape       = GlyphShape.TriangleDown,
				ButtonStyle      = ButtonStyle.ToolItem,
				Anchor           = AnchorStyles.Right,
				PreferredWidth   = AbstractFieldController.lineHeight,
				PreferredHeight  = AbstractFieldController.lineHeight,
			};

			this.CreateGotoAccountButton ();
			this.UpdatePropertyState ();

			//	Connexion des événements.
			this.button.Clicked += delegate
			{
				this.ShowPopup ();
			};

			arrowButton.Clicked += delegate
			{
				this.ShowPopup ();
			};
		}

		private void CreateGotoAccountButton()
		{
			this.gotoButton = this.CreateGotoButton ();

			ToolTip.Default.SetToolTip (this.gotoButton, "Montrer les détails du compte");

			this.gotoButton.Clicked += delegate
			{
				if (!string.IsNullOrEmpty (this.value))
				{
					var viewState = AccountsView.GetViewState (this.value);
					this.OnGoto (viewState);
				}
			};
		}

		private void UpdateButtons()
		{
			if (this.button != null)
			{
				this.button.Text = this.value;
			}

			if (this.gotoButton != null)
			{
				this.gotoButton.Visibility = !string.IsNullOrEmpty (this.value);
			}
		}


		private void ShowPopup()
		{
			var baseType = this.accessor.Mandat.GetAccountsBase (this.Date);
			var popup = new AccountsPopup (this.accessor, baseType, this.Value);
			
			popup.Create (this.button, leftOrRight: true);
			
			popup.Navigate += delegate (object sender, string account)
			{
				this.Value = account;
				this.OnValueEdited (this.Field);
			};
		}


		private ColoredButton					button;
		private IconButton						gotoButton;
		private string							value;
	}
}
