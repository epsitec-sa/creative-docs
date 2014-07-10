//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
			this.UpdateButtons ();

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
					var viewState = AccountsView.GetViewState (this.accessor, this.Date, this.value);
					this.OnGoto (viewState);
				}
			};
		}

		private void UpdateButtons()
		{
			if (this.button != null)
			{
				if (string.IsNullOrEmpty (this.value))
				{
					this.UpdateButton (null);
				}
				else
				{
					var baseType = this.accessor.Mandat.GetAccountsBase (this.Date);

					if (baseType.AccountsDateRange.IsEmpty)
					{
						this.UpdateButton (this.value, "Aucun plan comptable à cette date");
					}
					else
					{
						var summary = AccountsLogic.GetSummary (this.accessor, baseType, this.value);

						if (string.IsNullOrEmpty (summary))
						{
							this.UpdateButton (this.value, "Inconnu dans le plan comptable");
						}
						else
						{
							this.UpdateButton (summary);
						}
					}
				}
			}
		}

		private void UpdateButton(string number, string error = null)
		{
			if (string.IsNullOrEmpty (number))  // aucun compte défini ?
			{
				this.button.Text = null;
				AbstractFieldController.UpdateButton (this.button, this.PropertyState, this.isReadOnly, isError: false);

				this.gotoButton.Visibility = false;
			}
			else
			{
				if (string.IsNullOrEmpty (error))  // compte connu ?
				{
					this.button.Text = number;
					AbstractFieldController.UpdateButton (this.button, this.PropertyState, this.isReadOnly, isError: false);

					this.gotoButton.Visibility = true;
					this.gotoButton.Enable     = true;
				}
				else  // compte inconnu ?
				{
					this.button.Text = string.Concat (number, " — ", error);
					AbstractFieldController.UpdateButton (this.button, this.PropertyState, this.isReadOnly, isError: true);

					this.gotoButton.Visibility = true;
					this.gotoButton.Enable     = false;
				}
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
