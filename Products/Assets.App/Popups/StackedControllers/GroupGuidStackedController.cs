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

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class GroupGuidStackedController : AbstractStackedController
	{
		public GroupGuidStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodeGetter = this.accessor.GetNodeGetter (BaseType.Groups);
			this.nodeGetter = new GroupTreeNodeGetter (this.accessor, BaseType.Groups, primaryNodeGetter);
			this.nodeGetter.SetParams (null, SortingInstructions.Default);

			int width = GroupGuidStackedController.ControllerWidth - (int) AbstractScroller.DefaultBreadth;
			this.dataFiller = new SingleGroupsTreeTableFiller (this.accessor, this.nodeGetter, width);
		}


		public override bool					Enable
		{
			get
			{
				return this.controller.Enable;
			}
			set
			{
				this.controller.Enable = value;
			}
		}

		public Guid								Value
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				return this.value;
			}
			//	Sélectionne l'objet ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour sélectionner la prochaine ligne
			//	visible, vers le haut.
			set
			{
				if (this.value != value)
				{
					this.value = value;

					this.UpdateController ();
				}
			}
		}

		public int								Level
		{
			get
			{
				return this.nodeGetter.GetLevel ();
			}
			set
			{
				this.nodeGetter.SetLevel (value);
			}
		}


		public override int						RequiredHeight
		{
			get
			{
				return this.description.Height;
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, ref int tabIndex)
		{
			this.CreateLabel (parent, labelWidth);
			var controllerFrame = this.CreateControllerFrame (parent);
			controllerFrame.Padding = new Margins (2);

			this.controller = new NavigationTreeTableController (this.accessor);
			this.controller.CreateUI (controllerFrame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			TreeTableFiller<TreeNode>.FillColumns (this.controller, this.dataFiller, "Coontroller.Stacked.Groups");
			this.UpdateController ();

			//	Connexion des événements.
			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				int visibleSelectedRow = this.controller.TopVisibleRow + row;
				var node = this.nodeGetter[visibleSelectedRow];
				this.Value = node.Guid;

				this.UpdateController ();
				this.OnValueChanged ();
			};

			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnCompactOrExpand (this.controller.TopVisibleRow + row);
			};
		}

		private void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			var guid = this.Value;

			this.nodeGetter.CompactOrExpand (row);
			this.UpdateController ();

			this.Value = guid;
		}


		private void UpdateController(bool crop = true)
		{
			if (this.controller != null)
			{
				int visibleSelectedRow = this.nodeGetter.SearchBestIndex (value);
				TreeTableFiller<TreeNode>.FillContent (this.controller, this.dataFiller, visibleSelectedRow, crop);
			}
		}


		public const int ControllerWidth = 400;


		private readonly GroupTreeNodeGetter			nodeGetter;
		private readonly SingleGroupsTreeTableFiller	dataFiller;

		private Guid									value;
		private NavigationTreeTableController			controller;
	}
}