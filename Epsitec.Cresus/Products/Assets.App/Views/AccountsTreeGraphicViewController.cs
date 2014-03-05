//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AccountsTreeGraphicViewController : AbstractTreeGraphicViewController<TreeNode>
	{
		public AccountsTreeGraphicViewController(DataAccessor accessor, BaseType baseType, AbstractToolbarTreeController<TreeNode> treeTableController)
			: base (accessor, baseType, treeTableController)
		{
			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodeGetter = this.accessor.GetNodeGetter (this.baseType);
			this.nodeGetter = new GroupTreeNodeGetter (this.accessor, this.baseType, primaryNodeGetter);

			this.treeGraphicViewState = new TreeGraphicViewState ();
			this.treeGraphicViewState.Fields.Add (ObjectField.Number);
			this.treeGraphicViewState.Fields.Add (ObjectField.Name);
			this.treeGraphicViewState.FontFactors.Add (2.0);
			this.treeGraphicViewState.FontFactors.Add (1.0);
			this.treeGraphicViewState.ColumnWidth = 20;

			this.treeGraphicViewMode = TreeGraphicViewMode.VerticalFinalNode | TreeGraphicViewMode.AutoWidthFirstLine;
		}


		public override void CompactOrExpand(Guid guid)
		{
			int index = this.NodeGetter.SearchBestIndex (guid);
			this.NodeGetter.CompactOrExpand (index);

			this.UpdateData ();
		}

		public override void UpdateData()
		{
			if (this.treeGraphicViewState == null || this.scrollable == null)
			{
				return;
			}

			this.NodeGetter.SetParams (null, this.treeGraphicViewState.SortingInstructions);

			this.scrollable.Viewport.Children.Clear ();

			var parents = new List<Widget> ();
			parents.Add (this.scrollable.Viewport);

			var fields = this.GetFieds ();
			var fontFactors = this.GetFontFactors ();

			foreach (var node in this.NodeGetter.Nodes)
			{
				var level = node.Level;
				var parent = parents[level];

				var texts = this.GetTexts (this.baseType, node.Guid, fields);
				var w = this.CreateNode (parent, node.Guid, node.Level, node.Type, texts, fontFactors);

				if (parents.Count <= level+1)
				{
					parents.Add (null);
				}

				parents[level+1] = w;
			}
		}

		private GroupTreeNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as GroupTreeNodeGetter;
			}
		}
	}
}
