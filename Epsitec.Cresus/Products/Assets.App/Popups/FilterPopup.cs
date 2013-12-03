//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Choix d'un filtre dans la base de données des groupes.
	/// </summary>
	public class FilterPopup : AbstractPopup
	{
		public FilterPopup(DataAccessor accessor, Guid selectedGuid)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController();

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodesGetter = this.accessor.GetNodesGetter (BaseType.Groups);
			this.nodesGetter = new GroupTreeNodesGetter (this.accessor, primaryNodesGetter);
			this.nodesGetter.SetParams (null, SortingInstructions.Default);

			this.visibleSelectedRow = this.nodesGetter.Nodes.ToList ().FindIndex (x => x.Guid == selectedGuid);
			this.hasFilter = (this.visibleSelectedRow != -1);

			this.dataFiller = new SingleGroupsTreeTableFiller (this.accessor, this.nodesGetter);

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();

				var node = this.nodesGetter[this.visibleSelectedRow];
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
			this.CreateTitle ("Filtre");
			this.CreateCloseButton ();

			this.CreateController ();
			this.CreateButton ();
		}

		private void CreateController()
		{
			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<TreeNode>.FillColumns (this.controller, this.dataFiller, 0);
			this.UpdateController ();

			this.InitialCompact ();
		}

		private void CreateButton()
		{
			if (this.hasFilter)
			{
				var button = new Button
				{
					Parent          = this.mainFrameBox,
					Text            = "Annuler le filtre",
					ButtonStyle     = ButtonStyle.Icon,
					PreferredHeight = 30,
					Dock            = DockStyle.Bottom,
				};

				button.Clicked += delegate
				{
					this.OnNavigate (Guid.Empty);
					this.ClosePopup ();
				};
			}
		}


		private void InitialCompact()
		{
			var guid = this.SelectedGuid;

			int row = 0;
			while (row < this.nodesGetter.Count)
			{
				var node = this.nodesGetter[row];

				if (node.Level == 1)
				{
					this.nodesGetter.CompactOrExpand (row);
				}

				row++;
			}

			this.SelectedGuid = guid;
		}

		private void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			var guid = this.SelectedGuid;

			this.nodesGetter.CompactOrExpand (row);
			this.UpdateController ();

			this.SelectedGuid = guid;
		}

		private Guid SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.visibleSelectedRow;
				if (sel != -1 && sel < this.nodesGetter.Count)
				{
					return this.nodesGetter[sel].Guid;
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
				this.visibleSelectedRow = this.nodesGetter.SearchBestIndex (value);
				this.UpdateController ();
			}
		}


		private Size GetSize()
		{
			var parent = this.GetParent ();

			double h = parent.ActualHeight
					 - AbstractScroller.DefaultBreadth;

			int dx = FilterPopup.popupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = (int) (h * 0.5);

			return new Size (dx, dy);
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


		private const int popupWidth = 200;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly GroupTreeNodesGetter			nodesGetter;
		private readonly SingleGroupsTreeTableFiller	dataFiller;
		private readonly bool							hasFilter;

		private int										visibleSelectedRow;
	}
}