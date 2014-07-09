//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
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

					if (this.textField != null)
					{
						if (this.ignoreChanges.IsZero)
						{
							using (this.ignoreChanges.Enter ())
							{
								this.textField.Text = this.value;
								this.textField.SelectAll ();
							}
						}
					}

					this.UpdateButtons ();
				}
			}
		}

		private void UpdateValue()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.textField.Text = this.value;
				this.textField.SelectAll ();
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

			AbstractFieldController.UpdateTextField (this.textField, this.propertyState, this.isReadOnly, this.hasError);
			this.UpdateButtons ();
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent          = this.frameBox,
				Dock            = DockStyle.Left,
				PreferredWidth  = this.EditWidth-AbstractFieldController.lineHeight,
				PreferredHeight  = AbstractFieldController.lineHeight,
				TabIndex        = this.TabIndex,
				Text            = this.value,
			};

			//	Petit triangle "v" par-dessus la droite du bouton principal, sans fond
			//	afin de prendre la couleur du bouton principal.
			var arrowButton = new GlyphButton
			{
				Parent           = this.textField,
				GlyphShape       = GlyphShape.TriangleDown,
				ButtonStyle      = ButtonStyle.ToolItem,
				Anchor           = AnchorStyles.Right,
				PreferredWidth   = AbstractFieldController.lineHeight,
				PreferredHeight  = AbstractFieldController.lineHeight,
			};

			this.CreateGotoAccountButton ();
			this.UpdatePropertyState ();

			//	Connexion des événements.
			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.textField.Text;
						this.OnValueEdited (this.Field);
					}
				}
			};

			this.textField.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus ();
				}
				else  // perdu le focus ?
				{
					this.UpdateValue ();
				}
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
			if (this.gotoButton != null)
			{
				this.gotoButton.Visibility = !string.IsNullOrEmpty (this.value);
			}
		}


		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();

			base.SetFocus ();
		}


		private void ShowPopup()
		{
			var baseType = this.accessor.Mandat.GetAccountsBase (this.Date);
			var popup = new AccountsPopup (this.accessor, baseType, this.Value);
			
			popup.Create (this.textField, leftOrRight: true);
			
			popup.Navigate += delegate (object sender, string account)
			{
				this.Value = account;
				this.OnValueEdited (this.Field);
			};
		}


		private TextField						textField;
		private IconButton						gotoButton;
		private string							value;
	}
}
