﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	/// <summary>
	/// Permet de choisir un compte, soit en éditant directement le numéro, soit en
	/// choisissant dans une liste.
	/// </summary>
	public class AccountFieldController : AbstractFieldController
	{
		public AccountFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public System.DateTime?					ForcedDate;
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
					this.UpdateWidgets ();
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

			this.UpdateWidgets ();
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent           = this.frameBox,
				Dock             = DockStyle.Left,
				PreferredWidth   = this.EditWidth-AbstractFieldController.lineHeight*2,
				PreferredHeight  = AbstractFieldController.lineHeight,
				TabIndex         = this.TabIndex,
			};

			//	Petit triangle "v" à droite du champ éditable, pour faire comme un TextFieldCombo.
			var arrowButton = new GlyphButton
			{
				Parent           = this.frameBox,
				GlyphShape       = GlyphShape.TriangleDown,
				ButtonStyle      = ButtonStyle.Combo,
				Dock             = DockStyle.Left,
				PreferredWidth   = AbstractFieldController.lineHeight,
				PreferredHeight  = AbstractFieldController.lineHeight,
			};

			this.CreateGotoAccountButton ();
			this.UpdatePropertyState ();
			this.UpdateWidgets ();

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

			this.textField.IsFocusedChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.hasFocus = true;
					this.UpdateWidgets ();
					this.SetFocus ();
				}
				else  // perdu le focus ?
				{
					this.hasFocus = false;
					this.UpdateWidgets ();
				}
			};

			arrowButton.Clicked += delegate
			{
				this.ShowPopup ();
			};
		}

		private void CreateGotoAccountButton()
		{
			//	Crée le bouton permettant de sauter dans le plan comptable.
			this.gotoButton = this.CreateGotoButton ();

			ToolTip.Default.SetToolTip (this.gotoButton, "Montrer les détails du compte");

			this.gotoButton.Clicked += delegate
			{
				if (!string.IsNullOrEmpty (this.value))
				{
					var viewState = AccountsView.GetViewState (this.accessor, this.EffectiveDate, this.value);
					this.OnGoto (viewState);
				}
			};
		}

		private void UpdateWidgets()
		{
			//	Lorsque le widget a le focus, on affiche juste le numéro du compte.
			//	Lorsque le widget n'a pas le focus, on affiche plus d'informations.
			//	Par exemple:
			//	"1000 Caisse"
			//	"1111 — Inconnu dans le plan comptable"
			//	En cas d'erreur, le champ éditable change de couleur.

			if (this.textField != null)
			{
				string explanationsValue;
				bool error;

				if (string.IsNullOrEmpty (this.value))  // aucun compte ?
				{
					explanationsValue = null;
					error = false;
					this.gotoButton.Visibility = false;
				}
				else  // compte présent ?
				{
					//	Cherche le plan comptable correspondant à la date.
					var baseType = this.accessor.Mandat.GetAccountsBase (this.EffectiveDate);

					if (baseType.AccountsDateRange.IsEmpty)  // pas de plan comptable ?
					{
						explanationsValue = AccountFieldController.AddError (this.value, "Aucun plan comptable à cette date");
						error = true;
						this.gotoButton.Visibility = false;
					}
					else  // plan comptable trouvé ?
					{
						//	Cherche le résumé du compte (numéro et titre).
						var summary = AccountsLogic.GetSummary (this.accessor, baseType, this.value);

						if (string.IsNullOrEmpty (summary))  // compte inexistant ?
						{
							explanationsValue = AccountFieldController.AddError (this.value, "Inconnu dans le plan comptable");
							error = true;
							this.gotoButton.Visibility = false;
						}
						else
						{
							explanationsValue = summary;  // par exemple "1000 Caisse"
							error = false;
							this.gotoButton.Visibility = true;
						}
					}
				}

				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						if (this.hasFocus)
						{
							//	Si on a le focus, on met juste le numéro du compte.
							this.textField.Text = this.value;
						}
						else
						{
							//	Si on a pas le focus, on met le texte explicatif complet.
							this.textField.Text = explanationsValue;
							this.textField.SelectAll ();
						}
					}
				}

				AbstractFieldController.UpdateTextField (this.textField, this.propertyState, this.isReadOnly, hasError: error);
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
			//	Affiche le popup pour choisir un compte dans le plan comptable.
			var baseType = this.accessor.Mandat.GetAccountsBase (this.EffectiveDate);
			var popup = new AccountsPopup (this.accessor, baseType, this.Value);
			
			popup.Create (this.textField, leftOrRight: true);
			
			popup.Navigate += delegate (object sender, string account)
			{
				this.Value = account;
				this.SetFocus ();
				this.OnValueEdited (this.Field);
			};
		}


		private System.DateTime EffectiveDate
		{
			get
			{
				if (this.ForcedDate.HasValue)  // y a-t-il une date forcée ?
				{
					//	Si oui, elle prend le dessus.
					return this.ForcedDate.Value;
				}
				else
				{
					return this.Date;
				}
			}
		}


		private static string AddError(string text, string error)
		{
			//	Retourne un texte explicatif composé du numéro du compte et de l'erreur.
			if (string.IsNullOrEmpty (text))
			{
				return null;
			}
			else
			{
				if (string.IsNullOrEmpty (error))
				{
					return text;
				}
				else
				{
					return string.Concat (text, AccountFieldController.errorSeparator, error);
				}
			}
		}

		private const string errorSeparator = " — ";


		private TextField						textField;
		private IconButton						gotoButton;
		private string							value;
		private bool							hasFocus;
	}
}