//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EntriesTreeGraphicController : AbstractTreeGraphicController<TreeNode>
	{
		public EntriesTreeGraphicController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.treeGraphicViewState = new TreeGraphicState ();

			this.treeGraphicViewState.Fields.Add (ObjectField.EntryDate);
			this.treeGraphicViewState.Fields.Add (ObjectField.EntryDebitAccount);
			this.treeGraphicViewState.Fields.Add (ObjectField.EntryCreditAccount);
			this.treeGraphicViewState.Fields.Add (ObjectField.EntryTitle);
			this.treeGraphicViewState.Fields.Add (ObjectField.EntryAmount);

			this.treeGraphicViewState.FontFactors.Add (0.8);
			this.treeGraphicViewState.FontFactors.Add (1.2);
			this.treeGraphicViewState.FontFactors.Add (1.2);
			this.treeGraphicViewState.FontFactors.Add (0.8);
			this.treeGraphicViewState.FontFactors.Add (1.2);

			this.treeGraphicViewMode = TreeGraphicMode.AutoWidthAllLines | TreeGraphicMode.CompressEmptyValues;
		}


		public override void UpdateController(AbstractNodeGetter<TreeNode> nodeGetter, Guid selectedGuid, bool crop = true)
		{
			if (this.treeGraphicViewState == null || this.scrollable == null)
			{
				return;
			}

			this.scrollable.Viewport.Children.Clear ();

			var parents = new List<Widget> ();
			parents.Add (this.scrollable.Viewport);

			var fields = this.GetFieds ();
			var fontFactors = this.GetFontFactors ();

			var ng = nodeGetter as GroupTreeNodeGetter;
			int deep = this.GetDeep (ng);

			foreach (var node in ng.Nodes)
			{
				var level = node.Level;
				var parent = parents[level];

				double fontSize = AbstractTreeGraphicController<TreeNode>.GetFontSize (deep, level);

				var values = this.GetValues (this.baseType, node.Guid, fields);
				var w = this.CreateTile (parent, node.Guid, level, fontSize, node.Type, values, fontFactors);

				if (parents.Count <= level+1)
				{
					parents.Add (null);
				}

				parents[level+1] = w;
			}

			this.UpdateSelection (selectedGuid, crop);
		}

		private int GetDeep(GroupTreeNodeGetter nodeGetter)
		{
			return nodeGetter.Nodes.Max (x => x.Level) + 1;
		}
	}
}
