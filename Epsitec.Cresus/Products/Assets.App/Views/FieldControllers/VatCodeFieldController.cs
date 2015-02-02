//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	/// <summary>
	/// Permet de choisir un code TVA, soit en éditant directement le numéro, soit en
	/// choisissant dans une liste.
	/// </summary>
	public class VatCodeFieldController : AbstractFieldController
	{
		public VatCodeFieldController(DataAccessor accessor)
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
				TabIndex         = ++this.TabIndex,
			};

			//	Petit triangle "v" à droite du champ éditable, pour faire comme un TextFieldCombo.
			this.arrowButton = new GlyphButton
			{
				Parent           = this.frameBox,
				GlyphShape       = GlyphShape.TriangleDown,
				ButtonStyle      = ButtonStyle.Combo,
				Dock             = DockStyle.Left,
				PreferredWidth   = AbstractFieldController.lineHeight,
				PreferredHeight  = AbstractFieldController.lineHeight,
			};

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

			this.arrowButton.Clicked += delegate
			{
				this.ShowPopup ();
			};
		}

		private void UpdateWidgets()
		{
			//	Lorsque le widget a le focus, on affiche juste le code TVA.
			//	Lorsque le widget n'a pas le focus, on affiche plus d'informations.
			//	Par exemple:
			//	"TVARED 3.6% Prestations du secteur d'hébergement"
			//	"XX — Inconnu dans le plan comptable"
			//	En cas d'erreur, le champ éditable change de couleur.

			if (this.textField != null)
			{
				bool hasError;
				string explanationsValue = VatCodesLogic.GetExplanation (this.accessor, this.EffectiveDate, this.value, out hasError);

				this.hasError = hasError;

				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						if (this.hasFocus)
						{
							//	Si on a le focus, on met juste le code.
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

				var type = AbstractFieldController.GetFieldColorType (this.propertyState, isLocked: this.isReadOnly, isError: this.hasError);
				AbstractFieldController.UpdateTextField (this.textField, type, this.isReadOnly);

				this.arrowButton.Enable = !this.isReadOnly;
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
			//	Affiche le popup pour choisir un code de TVA.
			var baseType = this.accessor.Mandat.GetVatCodesBase (this.EffectiveDate);

			VatCodesPopup.Show (this.textField, this.accessor, baseType, this.Label, this.Value,
				delegate (string vatCode)
			{
				this.Value = vatCode;

				this.SetFocus ();
				this.OnValueEdited (this.Field);
			});
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


		private TextField						textField;
		private GlyphButton						arrowButton;
		private string							value;
		private bool							hasFocus;
	}
}
