//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class ObjectsPopup : AbstractPopup
	{
		public ObjectsPopup(DataAccessor accessor, Guid selectedGuid, bool onlyContainers)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController();

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodesGetter = this.accessor.GetNodesGetter (BaseType.Objects);
			this.nodesGetter = new TreeNodesGetter (this.accessor, BaseType.Objects, primaryNodesGetter);

			this.nodesGetter.UpdateData (onlyContainers ? TreeNodeOutputMode.OnlyParents : TreeNodeOutputMode.All);
			this.visibleSelectedRow = this.nodesGetter.Nodes.ToList ().FindIndex (x => x.Guid == selectedGuid);

			this.dataFiller = new ObjectsTreeTableFiller (this.accessor, BaseType.Objects, this.controller, this.nodesGetter);

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
			this.CreateTitle (this.mainFrameBox);
			this.CreateCloseButton ();
			
			this.controller.CreateUI (this.mainFrameBox, headerHeight: ObjectsPopup.HeaderHeight, footerHeight: 0);
			this.controller.AllowsMovement = false;

			this.dataFiller.UpdateColumns ();
			this.UpdateController ();
		}

		private void CreateTitle(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Choix de l'objet parent",
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				PreferredHeight  = ObjectsPopup.TitleHeight - 4,
				BackColor        = ColorManager.SelectionColor,
			};

			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Top,
				PreferredHeight  = 4,
				BackColor        = ColorManager.SelectionColor,
			};
		}


		private Size GetSize()
		{
			// TODO: faire autrement, car le mode est leftOrRight = false !
			var parent = this.GetParent ();

			//	On calcule une hauteur adaptée au contenu, mais qui ne dépasse
			//	évidement pas la hauteur de la fenêtre principale.
			double h = parent.ActualHeight
					 - ObjectsPopup.HeaderHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 1/2 de la hauteur.
			int max = (int) (h*0.5) / ObjectsPopup.RowHeight;

			int rows = System.Math.Min (this.nodesGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = ObjectsPopup.PopupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = ObjectsPopup.TitleHeight
				   + ObjectsPopup.HeaderHeight
				   + rows * ObjectsPopup.RowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}

		private void UpdateController(bool crop = true)
		{
			this.controller.RowsCount = this.nodesGetter.Count;

			int visibleCount = this.controller.VisibleRowsCount;
			int rowsCount    = this.controller.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.controller.TopVisibleRow;
			int selection    = this.visibleSelectedRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = this.controller.GetTopVisibleRow (selection);
				}

				if (this.controller.TopVisibleRow != firstRow)
				{
					this.controller.TopVisibleRow = firstRow;
				}

				selection -= this.controller.TopVisibleRow;
			}

			this.dataFiller.UpdateContent (firstRow, count, selection);
		}


		#region Events handler
		private void OnNavigate(Guid guid)
		{
			if (this.Navigate != null)
			{
				this.Navigate (this, guid);
			}
		}

		public delegate void NavigateEventHandler(object sender, Guid guid);
		public event NavigateEventHandler Navigate;
		#endregion


		private static readonly int TitleHeight      = 24;
		private static readonly int HeaderHeight     = 22;
		private static readonly int RowHeight        = 18;
		private static readonly int PopupWidth       = 390;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly TreeNodesGetter				nodesGetter;
		private readonly ObjectsTreeTableFiller			dataFiller;

		private int										visibleSelectedRow;
	}
}