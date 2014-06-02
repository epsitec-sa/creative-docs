//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Choix d'un objet dans la base de données des groupes d'immobilisations ou
	/// des comptes (plan comptable).
	/// </summary>
	public class GroupsPopup : AbstractPopup
	{
		public GroupsPopup(DataAccessor accessor, BaseType baseType, Guid selectedGuid)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.controller = new NavigationTreeTableController();

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodeGetter = this.accessor.GetNodeGetter (this.baseType);
			this.nodeGetter = new GroupTreeNodeGetter (this.accessor, this.baseType, primaryNodeGetter);

			if (this.baseType == BaseType.Groups)
			{
				this.dataFiller = new SingleGroupsTreeTableFiller (this.accessor, this.nodeGetter);
			}
			else if (this.baseType == BaseType.Accounts)
			{
				this.dataFiller = new SingleAccountsTreeTableFiller (this.accessor, this.nodeGetter);
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Unsupported BaseType {0}", this.baseType.ToString ()));
			}

			this.nodeGetter.SetParams (null, this.dataFiller.DefaultSorting);
			this.visibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => x.Guid == selectedGuid);

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();

				var node = this.nodeGetter[this.visibleSelectedRow];
				this.OnNavigate (node.Guid);
				this.ClosePopup ();
			};

			this.controller.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnCompactOrExpand (this.controller.TopVisibleRow + row);
			};
		}


		protected override Size DialogSize
		{
			get
			{
				return this.GetSize ();
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Choix du groupe");
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<TreeNode>.FillColumns (this.controller, this.dataFiller, "Popup.Groups");
			//?this.nodeGetter.SetParams (null, TreeTableFiller<TreeNode>.GetSortingInstructions (this.controller));

			this.UpdateController ();
		}


		private void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			var guid = this.SelectedGuid;

			this.nodeGetter.CompactOrExpand (row);
			this.UpdateController ();

			this.SelectedGuid = guid;
		}

		private Guid SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.visibleSelectedRow;
				if (sel != -1 && sel < this.nodeGetter.Count)
				{
					return this.nodeGetter[sel].Guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			//	Sélectionne l'objet ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour sélectionner la prochaine ligne
			//	visible, vers le haut.
			set
			{
				this.visibleSelectedRow = this.nodeGetter.SearchBestIndex (value);
				this.UpdateController ();
			}
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
			int max = (int) (h*0.5) / GroupsPopup.rowHeight;

			int rows = System.Math.Min (this.nodeGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = this.PopupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + rows * GroupsPopup.rowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}

		private int PopupWidth
		{
			get
			{
				if (this.baseType == BaseType.Groups)
				{
					return 200;  // colonne nom
				}
				else if (this.baseType == BaseType.Accounts)
				{
					return 100 + 300;  // colonnes numéro et compte
				}
				else
				{
					throw new System.InvalidOperationException (string.Format ("Unsupported BaseType {0}", this.baseType.ToString ()));
				}
			}
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<TreeNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}


		#region Events handler
		private void OnNavigate(Guid guid)
		{
			this.Navigate.Raise (this, guid);
		}

		public event EventHandler<Guid> Navigate;
		#endregion


		private const int rowHeight        = 18;

		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly NavigationTreeTableController	controller;
		private readonly GroupTreeNodeGetter			nodeGetter;
		private readonly AbstractTreeTableFiller<TreeNode> dataFiller;

		private int										visibleSelectedRow;
	}
}