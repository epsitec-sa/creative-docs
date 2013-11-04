//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class CategoriesPopup : AbstractPopup
	{
		public CategoriesPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.visibleSelectedRow = -1;

			this.controller = new NavigationTreeTableController();
			this.nodeFiller = new NodeFiller (this.accessor);
			this.dataFiller = new CategoriesDataFiller(this.accessor, BaseType.Categories, this.controller, this.nodeFiller);

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();

				var node = this.nodeFiller.GetNode (this.visibleSelectedRow);
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
			this.controller.CreateUI (this.mainFrameBox, headerHeight: CategoriesPopup.HeaderHeight, footerHeight: 0);
			this.dataFiller.UpdateColumns ();
			this.UpdateController ();
		}

		private void CreateTitle(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Choix d'une catégorie d'immobilisation",
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				PreferredHeight  = CategoriesPopup.TitleHeight - 4,
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
			var parent = this.GetParent ();

			//	On calcule une hauteur adaptée au contenu, mais qui ne dépasse
			//	évidement pas la hauteur de la fenêtre principale.
			double h = parent.ActualHeight
					 - CategoriesPopup.HeaderHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 3/4 de la hauteur.
			int max = (int) (h*0.75) / CategoriesPopup.RowHeight;

			int rows = System.Math.Min (this.nodeFiller.NodesCount, max);
			rows = System.Math.Max (rows, 3);

			int dx = CategoriesPopup.PopupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = CategoriesPopup.TitleHeight
				   + CategoriesPopup.HeaderHeight
				   + rows * CategoriesPopup.RowHeight
				   + (int) AbstractScroller.DefaultBreadth;

			return new Size (dx, dy);
		}

		private void UpdateController(bool crop = true)
		{
			this.controller.RowsCount = this.nodeFiller.NodesCount;

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

			this.dataFiller.UpdateContent (firstRow, count, selection, crop);
		}


		/// <summary>
		/// Donne accès à toutes les catégories finales.
		/// </summary>
		private class NodeFiller : AbstractNodeFiller
		{
			public NodeFiller(DataAccessor accessor)
			{
				this.accessor = accessor;

				this.nodes = new List<Node> ();
				this.UpdateData ();
			}

			public override int NodesCount
			{
				get
				{
					return this.nodes.Count;
				}
			}

			public override Node GetNode(int index)
			{
				return this.nodes[index];
			}

			private void UpdateData()
			{
				//	Met à jour toutes les catégories d'immobilisation finales.
				this.nodes.Clear ();

				int count = this.accessor.GetObjectsCount (BaseType.Categories);
				for (int i=0; i<count; i++)
				{
					Guid currentGuid;
					int currentLevel;
					this.GetData (i, out currentGuid, out currentLevel);

					//	Par défaut, on considére que la ligne ne peut être ni étendue
					//	ni compactée.
					var type = TreeTableTreeType.Final;

					if (i < count-2)
					{
						Guid nextGuid;
						int nextLevel;
						this.GetData (i+1, out nextGuid, out nextLevel);

						//	Si le noeud suivant a un niveau plus élevé, il s'agit d'une
						//	ligne pouvant être étendue ou compactée.
						if (nextLevel > currentLevel)
						{
							type = TreeTableTreeType.Expanded;
						}
					}

					if (type == TreeTableTreeType.Final)
					{
						var node = new Node (currentGuid, -1, type);
						this.nodes.Add (node);
					}
				}
			}

			private void GetData(int row, out Guid guid, out int level)
			{
				//	Retourne une donnée.
				guid = Guid.Empty;
				level = 0;

				if (row >= 0 && row < this.accessor.GetObjectsCount (BaseType.Categories))
				{
					guid = this.accessor.GetObjectGuids (BaseType.Categories, row, 1).FirstOrDefault ();

					var obj = this.accessor.GetObject (BaseType.Categories, guid);
					var timestamp = new Timestamp (System.DateTime.MaxValue, 0);
					var p = ObjectCalculator.GetObjectSyntheticProperty (obj, timestamp, ObjectField.Level) as DataIntProperty;
					if (p != null)
					{
						level = p.Value;
					}
				}
			}

			private readonly DataAccessor					accessor;
			private readonly List<Node>						nodes;
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
		private readonly NodeFiller						nodeFiller;
		private readonly CategoriesDataFiller			dataFiller;

		private int										visibleSelectedRow;
	}
}