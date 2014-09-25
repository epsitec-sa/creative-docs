//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Choix d'un compte dans le plan comptable, avec possibilité de divers filtres.
	/// </summary>
	public class AccountsPopup : AbstractPopup
	{
		private AccountsPopup(DataAccessor accessor, BaseType baseType, string selectedAccount, AccountCategory categories)
		{
			this.accessor   = accessor;
			this.baseType   = baseType;
			this.categories = categories;

			this.controller = new NavigationTreeTableController();

			var primaryNodeGetter = this.accessor.GetNodeGetter (this.baseType);
			this.nodeGetter = new AccountsFilterNodeGetter (this.accessor, primaryNodeGetter);

			this.dataFiller = new SingleAccountsTreeTableFiller (this.accessor, this.nodeGetter)
			{
				BaseType = this.baseType,
			};

			this.UpdateGetter ();
			this.visibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => this.GetNumber (x.Guid) == selectedAccount);
			this.UpdateSelectedGuid ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();

				this.OnNavigate (this.SelectedAccount);
				this.ClosePopup ();
			};
		}


		private string SelectedAccount
		{
			get
			{
				var node = this.nodeGetter[this.visibleSelectedRow];
				return this.GetNumber (node.Guid);
			}
		}


		protected override Size DialogSize
		{
			get
			{
				return this.GetSize ();
			}
		}

		protected override void CreateUI()
		{
			string title;

			if (this.baseType.AccountsDateRange.IsEmpty)
			{
				title = Res.Strings.Popup.Accounts.NoAccounts.ToString ();
			}
			else
			{
				title = string.Format (Res.Strings.Popup.Accounts.Choice.ToString (), this.baseType.AccountsDateRange.ToNiceString ());
			}

			this.CreateTitle (title);
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			TreeTableFiller<GuidNode>.FillColumns (this.controller, this.dataFiller, "Popup.Groups");

			this.CreateCategoriesUI (this.mainFrameBox);
			this.CreateFilterUI (this.mainFrameBox);

			this.UpdateMiscButton ();
			this.UpdateController ();
		}

		private void CreateFilterUI(Widget parent)
		{
			//	Crée la partie inférieure permettant la saisie d'un filtre.
			const int margin = 5;
			const int height = 20;

			var footer = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = margin + height + margin,
				Dock            = DockStyle.Bottom,
				Padding         = new Margins (margin),
				BackColor       = ColorManager.WindowBackgroundColor,
			};

			var text = Res.Strings.Popup.Accounts.Filter.ToString ();

			new StaticText
			{
				Parent           = footer,
				Text             = text,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				PreferredWidth   = 10 + text.GetTextWidth (),
				Margins          = new Margins (0, 10, 0, 0),
				Dock             = DockStyle.Left,
			};

			this.filterField = new TextField
			{
				Parent           = footer,
				Dock             = DockStyle.Fill,
			};

			this.miscButton = new IconButton
			{
				Parent        = footer,
				AutoFocus     = false,
				Dock          = DockStyle.Right,
				PreferredSize = new Size (height, height),
				Margins       = new Margins (10, 0, 0, 0),
			};

			var clearButton = new IconButton
			{
				Parent        = footer,
				IconUri       = Misc.GetResourceIconUri ("Field.Delete"),
				AutoFocus     = false,
				Dock          = DockStyle.Right,
				PreferredSize = new Size (height, height),
				Margins       = new Margins (2, 0, 0, 0),
				Enable        = false,
			};

			//	Connexions des événements.
			this.filterField.TextChanged += delegate
			{
				this.UpdateGetter ();
				this.visibleSelectedRow = this.nodeGetter.SearchIndex (this.selectedGuid);
				this.UpdateController ();
				clearButton.Enable = !string.IsNullOrEmpty (this.filterField.Text);
			};

			clearButton.Clicked += delegate
			{
				this.filterField.Text = null;
			};

			miscButton.Clicked += delegate
			{
				this.categoriesFrame.Visibility = !this.categoriesFrame.Visibility;
				this.UpdateMiscButton ();
			};

			this.filterField.Focus ();
		}

		private void CreateCategoriesUI(Widget parent)
		{
			const int margin = 5;
			const int height = 20;

			this.categoriesFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = margin + height + margin,
				Dock            = DockStyle.Bottom,
				Padding         = new Margins (margin),
				BackColor       = ColorManager.WindowBackgroundColor,
				Visibility      = false,
			};

			new FrameBox
			{
				Parent           = this.categoriesFrame,
				PreferredWidth   = 10 + Res.Strings.Popup.Accounts.Filter.ToString ().GetTextWidth (),
				Margins          = new Margins (0, 10, 0, 0),
				Dock             = DockStyle.Left,
			};

			this.categoriesActifsButton        = this.CreateCaregoryButton (this.categoriesFrame, Res.Strings.Popup.Accounts.Category.Actifs.ToString (), AccountCategory.Actif);
			this.categoriesPassifsButton       = this.CreateCaregoryButton (this.categoriesFrame, Res.Strings.Popup.Accounts.Category.Passifs.ToString (),       AccountCategory.Passif);
			this.categoriesChargesButton       = this.CreateCaregoryButton (this.categoriesFrame, Res.Strings.Popup.Accounts.Category.Charges.ToString (),       AccountCategory.Charge);
			this.categoriesProduitsButton      = this.CreateCaregoryButton (this.categoriesFrame, Res.Strings.Popup.Accounts.Category.Produits.ToString (),      AccountCategory.Produit);
			this.categoriesExploitationsButton = this.CreateCaregoryButton (this.categoriesFrame, Res.Strings.Popup.Accounts.Category.Exploitations.ToString (), AccountCategory.Exploitation);

			this.UpdateCategories ();
		}

		private CheckButton CreateCaregoryButton(Widget parent, string text, AccountCategory category)
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
				this.UpdateGetter ();
				this.UpdateController ();
			};

			return button;
		}


		[Command (Res.CommandIds.AccountsPopup.Prev)]
		private void DoPrev()
		{
			if (this.visibleSelectedRow > 0)
			{
				this.visibleSelectedRow--;
				this.UpdateSelectedGuid ();
				this.UpdateController ();
			}
		}

		[Command (Res.CommandIds.AccountsPopup.Next)]
		private void DoNext()
		{
			if (this.visibleSelectedRow < this.nodeGetter.Count-1)
			{
				this.visibleSelectedRow++;
				this.UpdateSelectedGuid ();
				this.UpdateController ();
			}
		}


		private string GetNumber(Guid guid)
		{
			return AccountsLogic.GetNumber (this.accessor, this.baseType, guid);
		}


		private Size GetSize()
		{
			// TODO: faire autrement, car le mode est leftOrRight = false !
			var parent = this.GetParent ();

			//	On calcule une hauteur adaptée au contenu, mais qui ne dépasse
			//	évidement pas la hauteur de la fenêtre principale.
			double h = parent.ActualHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 1/2 de la hauteur.
			int max = (int) (h*0.5) / AccountsPopup.rowHeight;

			int rows = System.Math.Min (this.nodeGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = this.PopupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + rows * AccountsPopup.rowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}

		private int PopupWidth
		{
			get
			{
				return SingleAccountsTreeTableFiller.TotalWidth;
			}
		}

		private void UpdateSelectedGuid()
		{
			if (this.visibleSelectedRow == -1)
			{
				this.selectedGuid = Guid.Empty;
			}
			else
			{
				this.selectedGuid = this.nodeGetter[this.visibleSelectedRow].Guid;
			}
		}

		private void UpdateMiscButton()
		{
			if (this.categoriesFrame.Visibility)
			{
				this.miscButton.IconUri  = Misc.GetResourceIconUri ("Triangle.Up");
			}
			else
			{
				this.miscButton.IconUri  = Misc.GetResourceIconUri ("Triangle.Down");
			}
		}

		private void UpdateCategories()
		{
			this.categoriesActifsButton       .ActiveState = (this.categories & AccountCategory.Actif       ) != 0 ? ActiveState.Yes : ActiveState.No;
			this.categoriesPassifsButton      .ActiveState = (this.categories & AccountCategory.Passif      ) != 0 ? ActiveState.Yes : ActiveState.No;
			this.categoriesChargesButton      .ActiveState = (this.categories & AccountCategory.Charge      ) != 0 ? ActiveState.Yes : ActiveState.No;
			this.categoriesProduitsButton     .ActiveState = (this.categories & AccountCategory.Produit     ) != 0 ? ActiveState.Yes : ActiveState.No;
			this.categoriesExploitationsButton.ActiveState = (this.categories & AccountCategory.Exploitation) != 0 ? ActiveState.Yes : ActiveState.No;
		}

		private void UpdateGetter()
		{
			string filter = (this.filterField == null) ? null : this.filterField.Text;
			this.nodeGetter.SetParams (this.baseType, null, filter, this.categories);
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<GuidNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		private void OnNavigate(string account)
		{
			this.Navigate.Raise (this, account);
		}

		public event EventHandler<string> Navigate;
		#endregion


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, BaseType baseType, string selectedAccount, AccountCategory categories, System.Action<string, AccountCategory> action)
		{
			//	Affiche le popup pour choisir un compte dans le plan comptable.
			var popup = new AccountsPopup (accessor, baseType, selectedAccount, categories);
			
			popup.Create (target, leftOrRight: true);
			
			popup.Navigate += delegate (object sender, string account)
			{
				action (account, popup.categories);
			};

			popup.Closed += delegate (object sender, ReasonClosure raison)
			{
				if (raison == ReasonClosure.AcceptKey)
				{
					action (popup.SelectedAccount, popup.categories);
				}
			};
		}
		#endregion


		private const int rowHeight = 18;

		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly NavigationTreeTableController	controller;
		private readonly AccountsFilterNodeGetter		nodeGetter;
		private readonly AbstractTreeTableFiller<GuidNode> dataFiller;

		private AccountCategory							categories;
		private int										visibleSelectedRow;
		private Guid									selectedGuid;
		private TextField								filterField;
		private IconButton								miscButton;
		private FrameBox								categoriesFrame;
		private CheckButton								categoriesActifsButton;
		private CheckButton								categoriesPassifsButton;
		private CheckButton								categoriesChargesButton;
		private CheckButton								categoriesProduitsButton;
		private CheckButton								categoriesExploitationsButton;
	}
}