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
		private AccountsPopup(DataAccessor accessor, BaseType baseType, string title, string selectedAccount, AccountCategory categories)
		{
			this.accessor = accessor;
			this.baseType = baseType;
			this.title    = title;

			this.controller = new NavigationTreeTableController();

			var existingCategories = AccountsLogic.GetCategories (this.accessor, this.baseType);
			this.filterController = new AccountsFilterController (existingCategories);
			this.filterController.Categories = categories;

			var primaryNodeGetter = this.accessor.GetNodeGetter (this.baseType);
			this.nodeGetter = new GroupTreeNodeGetter (this.accessor, baseType, primaryNodeGetter);

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


		private string							SelectedAccount
		{
			get
			{
				var node = this.nodeGetter[this.visibleSelectedRow];
				return this.GetNumber (node.Guid);
			}
		}

		private AccountCategory					Categories
		{
			get
			{
				return this.filterController.Categories;
			}
		}


		protected override Size					DialogSize
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

			this.CreateTitle (UniversalLogic.NiceJoin (this.title, title));
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			TreeTableFiller<TreeNode>.FillColumns (this.controller, this.dataFiller, "Popup.Groups");

			this.CreateFilterUI (this.mainFrameBox);

			this.UpdateController ();
		}

		private void CreateFilterUI(Widget parent)
		{
			//	Crée la partie inférieure permettant la saisie d'un filtre.
			this.filterController.CreateUI (parent);

			this.filterController.FilterChanged += delegate
			{
				this.UpdateGetter ();
				this.visibleSelectedRow = this.nodeGetter.SearchBestIndex (this.selectedGuid);
				this.UpdateController ();
			};
		}


		[Command (Res.CommandIds.AccountsPopup.Prev)]
		private void DoPrev()
		{
			//	Appelé lorsque l'utilisateur presse sur "flèche en haut".
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
			//	Appelé lorsque l'utilisateur presse sur "flèche en bas".
			if (this.visibleSelectedRow < this.nodeGetter.Count-1)
			{
				this.visibleSelectedRow++;
				this.UpdateSelectedGuid ();
				this.UpdateController ();
			}
		}


		protected override bool					IsAcceptEnable
		{
			get
			{
				var node = this.nodeGetter[this.visibleSelectedRow];
				return this.Filter (node.Guid);
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
				   + (int) AbstractScroller.DefaultBreadth
				   + AbstractFilterController.height;

			return new Size (dx, dy);
		}

		private int								PopupWidth
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

		private void UpdateGetter()
		{
			if (this.filterController == null || string.IsNullOrEmpty (this.filterController.Filter))
			{
				this.searchEngine = null;
			}
			else
			{
				var definition = SearchDefinition.Default.FromPattern (this.filterController.Filter);
				this.searchEngine = new SearchEngine (definition);
			}

			this.nodeGetter.SetParams (null, this.dataFiller.DefaultSorting, this.Filter);
		}

		private bool Filter(Guid guid)
		{
			var account = this.accessor.GetObject (this.baseType, guid);

			var accType  = (AccountType) ObjectProperties.GetObjectPropertyInt (account, null, ObjectField.AccountType);

			if (accType == AccountType.Normal)
			{
				var category = (AccountCategory) ObjectProperties.GetObjectPropertyInt (account, null, ObjectField.AccountCategory);

				if ((this.filterController.Categories & category) == 0)
				{
					return false;  // caché
				}

				if (this.searchEngine != null)
				{
					var number = ObjectProperties.GetObjectPropertyString (account, null, ObjectField.Number, inputValue: true);
					var name   = ObjectProperties.GetObjectPropertyString (account, null, ObjectField.Name);

					if (!this.searchEngine.IsMatching (number) &&
						!this.searchEngine.IsMatching (name))
					{
						return false;  // caché
					}
				}

				return true;  // visible
			}
			else
			{
				return false;  // caché
			}
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<TreeNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		private void OnNavigate(string account)
		{
			this.Navigate.Raise (this, account);
		}

		public event EventHandler<string> Navigate;
		#endregion


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, BaseType baseType, string title, string selectedAccount, AccountCategory categories, System.Action<string, AccountCategory> action)
		{
			//	Affiche le popup pour choisir un compte dans le plan comptable.
			var popup = new AccountsPopup (accessor, baseType, title, selectedAccount, categories);
			
			popup.Create (target, leftOrRight: true);
			
			popup.Navigate += delegate (object sender, string account)
			{
				action (account, popup.Categories);
			};

			popup.Closed += delegate (object sender, ReasonClosure raison)
			{
				if (raison == ReasonClosure.AcceptKey)
				{
					action (popup.SelectedAccount, popup.Categories);
				}
			};
		}
		#endregion


		private const int rowHeight     = 18;

		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly string							title;
		private readonly NavigationTreeTableController	controller;
		private readonly AccountsFilterController		filterController;
		private readonly GroupTreeNodeGetter			nodeGetter;
		private readonly AbstractTreeTableFiller<TreeNode> dataFiller;

		private int										visibleSelectedRow;
		private Guid									selectedGuid;
		private SearchEngine							searchEngine;
	}
}