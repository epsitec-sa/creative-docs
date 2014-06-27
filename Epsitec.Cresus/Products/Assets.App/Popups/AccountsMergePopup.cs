//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class AccountsMergePopup : AbstractPopup
	{
		public AccountsMergePopup(DataAccessor accessor, List<AccountMergeTodo> todo)
		{
			this.accessor = accessor;
			this.todo     = todo;

			this.controller = new NavigationTreeTableController();

			this.nodeGetter = new AccountsMergeNodeGetter ();
			this.dataFiller = new AccountsMergeTreeTableFiller (this.accessor, this.nodeGetter);

			this.visibleSelectedRow = -1;

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();
				this.UpdateModify ();
			};
		}


		protected override Size DialogSize
		{
			get
			{
				int w = AccountsMergeTreeTableFiller.TotalWidth + (int) AbstractScroller.DefaultBreadth;
				return new Size (w, 400);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Fusion des comptes importés dans le plan comptable");

			var frame = new FrameBox
			{
				Parent  = this.mainFrameBox,
				Dock    = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			this.CreateButtons ();
			this.CreateModify ();

			TreeTableFiller<AccountMergeTodo>.FillColumns (this.controller, this.dataFiller, "Popup.Groups");

			this.UpdateController ();
			this.UpdateModify ();
		}

		private void CreateModify()
		{
			//	Crée la zone pour modifier la façon de fusionner un compte.
			var frame = new FrameBox
			{
				Parent              = this.mainFrameBox,
				Dock                = DockStyle.Bottom,
				PreferredHeight     = 30,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Margins             = new Margins (10, 0, 0, 10),
			};

			{
				var text = "Ajouter";
				var width = text.GetTextWidth ();

				this.radioAdd = new RadioButton
				{
					Parent         = frame,
					Text           = text,
					PreferredWidth = 20 + width + 10,
					Dock           = DockStyle.Left,
					AutoFocus      = false,
				};
			}

			{
				var text = "Fusionner avec";
				var width = text.GetTextWidth ();

				this.radioMerge = new RadioButton
				{
					Parent         = frame,
					Text           = text,
					PreferredWidth = 20 + width + 10,
					Dock           = DockStyle.Left,
					AutoFocus      = false,
				};
			}

			this.modifyButton = new Button
			{
				Parent           = frame,
				ButtonStyle      = ButtonStyle.Icon,
				AutoFocus        = false,
				Dock             = DockStyle.Fill,
				ContentAlignment = ContentAlignment.MiddleLeft,
			};

			this.info = new StaticText
			{
				Parent = frame,
				Dock   = DockStyle.Fill,
			};

			this.radioAdd.Clicked += delegate
			{
				this.ActionAdd ();
			};

			this.radioMerge.Clicked += delegate
			{
				this.ActionMerge ();
			};

			this.modifyButton.Clicked += delegate
			{
				this.ShowAccountPopup ();
			};
		}

		private void CreateButtons()
		{
			//	Crée les boutons tout en bas du Popup.
			var footer = this.CreateFooter ();

			this.CreateFooterAcceptButton (footer, DockStyle.Left,  "ok",     "Importer avec fusion");
			this.CreateFooterCancelButton (footer, DockStyle.Right, "cancel", "Annuler");
		}


		private void UpdateModify()
		{
			if (this.visibleSelectedRow == -1)
			{
				this.radioAdd    .Visibility = false;
				this.radioMerge  .Visibility = false;
				this.modifyButton.Visibility = false;
				this.info        .Visibility = false;
			}
			else
			{
				var node = this.nodeGetter[this.visibleSelectedRow];

				this.radioAdd    .Visibility = !node.RequiredMerge;
				this.radioMerge  .Visibility = !node.RequiredMerge;
				this.modifyButton.Visibility = !node.RequiredMerge;
				this.info        .Visibility =  node.RequiredMerge;

				if (node.RequiredMerge)
				{
					this.info.Text = "La fusion est obligatoire, car les comptes ont les mêmes numéros.";
				}
				else
				{
					if (node.IsAdd)
					{
						this.radioAdd.ActiveState = ActiveState.Yes;
						this.radioMerge.ActiveState = ActiveState.No;

						this.modifyButton.Visibility = false;
					}
					else
					{
						this.radioAdd.ActiveState = ActiveState.No;
						this.radioMerge.ActiveState = ActiveState.Yes;

						this.modifyButton.Visibility = true;
						this.modifyButton.Text = "   " + AccountsLogic.GetSummary (node.MergeWithAccount);
					}
				}
			}
		}


		private void ActionAdd()
		{
			var todo = this.SelectedTodo;
			this.SelectedTodo = AccountMergeTodo.NewAdd (todo.ImportedAccount);

			this.UpdateAfterModify ();
		}

		private void ActionMerge()
		{
			var todo = this.SelectedTodo;
			this.SelectedTodo = AccountMergeTodo.NewMerge (todo.ImportedAccount, this.GetDefaultAccount (todo.ImportedAccount), todo.RequiredMerge);

			this.UpdateAfterModify ();
		}

		private void ShowAccountPopup()
		{
			var todo = this.SelectedTodo;
			var popup = new GroupsPopup (this.accessor, BaseType.Accounts, todo.MergeWithAccount.Guid);
			
			popup.Create (this.modifyButton, leftOrRight: false);
			
			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.SelectedTodo = AccountMergeTodo.NewMerge (todo.ImportedAccount, this.GetCurrentAccount (guid), todo.RequiredMerge);

				this.UpdateAfterModify ();
			};
		}

		private DataObject GetDefaultAccount(DataObject account)
		{
			var accounts = this.accessor.Mandat.GetData (BaseType.Accounts);

			var best = AccountsSearching.ApproximativeTitleSearch (accounts, account);
			if (best != null)
			{
				return best;
			}

			return this.accessor.Mandat.GetData (BaseType.Accounts).Last ();
		}

		private DataObject GetCurrentAccount(Guid guid)
		{
			return this.accessor.GetObject (BaseType.Accounts, guid);
		}

		private AccountMergeTodo SelectedTodo
		{
			get
			{
				return this.todo[this.visibleSelectedRow];
			}
			set
			{
				this.todo[this.visibleSelectedRow] = value;
			}
		}


		private void UpdateAfterModify()
		{
			this.UpdateController ();
			this.UpdateModify ();
		}

		private void UpdateController(bool crop = true)
		{
			this.nodeGetter.SetParams (this.todo);
			TreeTableFiller<AccountMergeTodo>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly AccountsMergeNodeGetter		nodeGetter;
		private readonly AccountsMergeTreeTableFiller	dataFiller;

		private List<AccountMergeTodo>					todo;
		private int										visibleSelectedRow;
		private RadioButton								radioAdd;
		private RadioButton								radioMerge;
		private Button									modifyButton;
		private StaticText								info;
	}
}