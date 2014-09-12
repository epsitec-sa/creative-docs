//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.TreeGraphicControllers
{
	public class GroupsTreeGraphicController : AbstractTreeGraphicController<TreeNode>, System.IDisposable
	{
		public GroupsTreeGraphicController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.treeGraphicViewState = new TreeGraphicState ();
			this.treeGraphicViewState.Fields.Add (ObjectField.Name);
			this.treeGraphicViewState.FontFactors.Add (1.0);

			this.treeGraphicViewMode = TreeGraphicMode.AutoWidthAllLines;
		}

		public override void Dispose()
		{
		}


		public override void UpdateController(INodeGetter<TreeNode> nodeGetter, Guid selectedGuid, bool crop = true)
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

			foreach (var node in ng.GetNodes ())
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
			return nodeGetter.GetNodes ().Max (x => x.Level) + 1;
		}
	}
}
