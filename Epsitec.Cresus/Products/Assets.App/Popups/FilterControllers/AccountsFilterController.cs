//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Ce contrôleur gère la ligne de recherche présente en bas du popup de choix d'un compte.
	/// En plus de la ligne contenant de texte de recherche, il gère une partie optionnelle
	/// permettant de choisir les catégories des comptes à filtrer.
	/// </summary>
	public class AccountsFilterController : AbstractFilterController
	{
		public AccountsFilterController(AccountCategory existingCategories)
			: base ()
		{
			this.existingCategories = existingCategories;
		}


		public AccountCategory					Categories
		{
			//	Catégories de comptes choisies par l'utilisateur.
			get
			{
				return this.categories;
			}
			set
			{
				this.categories = value;
			}
		}


		protected override bool CreateOptionsUI(Widget parent)
		{
			//	Crée la partie optionnelle permettant de choisir les catégories de comptes.
			int margin = 10 + Res.Strings.Popup.FilterController.Label.ToString ().GetTextWidth ();

			this.categoriesFrame2 = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = AbstractFilterController.filterMargins + AbstractFilterController.filterHeight,
				Dock            = DockStyle.Bottom,
				Padding         = new Margins (AbstractFilterController.filterMargins, AbstractFilterController.filterMargins, 0, AbstractFilterController.filterMargins),
				BackColor       = ColorManager.WindowBackgroundColor,
				Visibility      = false,
			};

			new FrameBox
			{
				Parent           = this.categoriesFrame2,
				PreferredWidth   = margin,
				Margins          = new Margins (0, 10, 0, 0),
				Dock             = DockStyle.Left,
			};

			this.categoriesRevenusButton       = this.CreateCategoryButton (this.categoriesFrame2, Res.Strings.Popup.Accounts.Category.Revenus .ToString (), AccountCategory.Revenu);
			this.categoriesDepensesButton      = this.CreateCategoryButton (this.categoriesFrame2, Res.Strings.Popup.Accounts.Category.Depenses.ToString (), AccountCategory.Depense);
			this.categoriesRecettesButton      = this.CreateCategoryButton (this.categoriesFrame2, Res.Strings.Popup.Accounts.Category.Recettes.ToString (), AccountCategory.Recette);

			this.categoriesFrame1 = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = AbstractFilterController.filterMargins + AbstractFilterController.filterHeight,
				Dock            = DockStyle.Bottom,
				Padding         = new Margins (AbstractFilterController.filterMargins, AbstractFilterController.filterMargins, AbstractFilterController.filterMargins, 0),
				BackColor       = ColorManager.WindowBackgroundColor,
				Visibility      = false,
			};

			new FrameBox
			{
				Parent           = this.categoriesFrame1,
				PreferredWidth   = margin,
				Margins          = new Margins (0, 10, 0, 0),
				Dock             = DockStyle.Left,
			};

			this.categoriesActifsButton        = this.CreateCategoryButton (this.categoriesFrame1, Res.Strings.Popup.Accounts.Category.Actifs       .ToString (), AccountCategory.Actif);
			this.categoriesPassifsButton       = this.CreateCategoryButton (this.categoriesFrame1, Res.Strings.Popup.Accounts.Category.Passifs      .ToString (), AccountCategory.Passif);
			this.categoriesChargesButton       = this.CreateCategoryButton (this.categoriesFrame1, Res.Strings.Popup.Accounts.Category.Charges      .ToString (), AccountCategory.Charge);
			this.categoriesProduitsButton      = this.CreateCategoryButton (this.categoriesFrame1, Res.Strings.Popup.Accounts.Category.Produits     .ToString (), AccountCategory.Produit);
			this.categoriesExploitationsButton = this.CreateCategoryButton (this.categoriesFrame1, Res.Strings.Popup.Accounts.Category.Exploitations.ToString (), AccountCategory.Exploitation);

			this.UpdateCategories ();

			return true;
		}


		protected override bool					OptionsVisibility
		{
			get
			{
				return this.categoriesFrame1.Visibility;
			}
			set
			{
				this.categoriesFrame1.Visibility = value;
				this.categoriesFrame2.Visibility = value;
				this.UpdateOptionsButton ();
			}
		}


		private CheckButton CreateCategoryButton(Widget parent, string text, AccountCategory category)
		{
			if ((this.existingCategories & category) == 0)
			{
				return null;
			}
			else
			{
				var button = new CheckButton
				{
					Parent         = parent,
					Text           = text,
					PreferredWidth = 20 + text.GetTextWidth (),
					Margins        = new Margins (0, 10, 0, 0),
					Dock           = DockStyle.Left,
					AutoToggle     = false,
				};

				button.Clicked += delegate
				{
					this.categories ^= category;
					this.UpdateCategories ();
					this.OnFilterChanged ();
				};

				return button;
			}
		}

		private void UpdateCategories()
		{
			this.UpdateCategory (this.categoriesActifsButton,        AccountCategory.Actif);
			this.UpdateCategory (this.categoriesPassifsButton,       AccountCategory.Passif);
			this.UpdateCategory (this.categoriesChargesButton,       AccountCategory.Charge);
			this.UpdateCategory (this.categoriesProduitsButton,      AccountCategory.Produit);
			this.UpdateCategory (this.categoriesExploitationsButton, AccountCategory.Exploitation);
			this.UpdateCategory (this.categoriesRevenusButton,       AccountCategory.Revenu);
			this.UpdateCategory (this.categoriesDepensesButton,      AccountCategory.Depense);
			this.UpdateCategory (this.categoriesRecettesButton,      AccountCategory.Recette);
		}

		private void UpdateCategory(CheckButton button, AccountCategory category)
		{
			if (button != null)
			{
				button.ActiveState = (this.categories & category) == 0 ? ActiveState.No : ActiveState.Yes;
			}
		}


		private readonly AccountCategory				existingCategories;

		private AccountCategory							categories;
		private FrameBox								categoriesFrame1;
		private FrameBox								categoriesFrame2;
		private CheckButton								categoriesActifsButton;
		private CheckButton								categoriesPassifsButton;
		private CheckButton								categoriesChargesButton;
		private CheckButton								categoriesProduitsButton;
		private CheckButton								categoriesExploitationsButton;
		private CheckButton								categoriesRevenusButton;
		private CheckButton								categoriesDepensesButton;
		private CheckButton								categoriesRecettesButton;
	}
}