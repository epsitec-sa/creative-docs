//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.TreeGraphicControllers
{
	public class EntriesTreeGraphicController : AbstractTreeGraphicController<EntryNode>
	{
		public EntriesTreeGraphicController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.treeGraphicViewState = new TreeGraphicState ();

			this.treeGraphicViewState.Fields.Add (ObjectField.EntryDate);
			this.treeGraphicViewState.Fields.Add (ObjectField.EntryDebitAccount);
			this.treeGraphicViewState.Fields.Add (ObjectField.EntryTitle);
			this.treeGraphicViewState.Fields.Add (ObjectField.EntryAmount);

			this.treeGraphicViewState.FontFactors.Add (0.8);
			this.treeGraphicViewState.FontFactors.Add (1.2);
			this.treeGraphicViewState.FontFactors.Add (0.8);
			this.treeGraphicViewState.FontFactors.Add (1.2);

			this.treeGraphicViewMode = TreeGraphicMode.AutoWidthAllLines | TreeGraphicMode.CompressEmptyValues;
		}


		public override void UpdateController(INodeGetter<EntryNode> nodeGetter, Guid selectedGuid, bool crop = true)
		{
			if (this.treeGraphicViewState == null || this.scrollable == null)
			{
				return;
			}

			this.scrollable.Viewport.Children.Clear ();

			var parents = new List<Widget> ();
			parents.Add (this.scrollable.Viewport);

			var fontFactors = this.GetFontFactors ();

			var ng = nodeGetter as EntriesNodeGetter;
			int deep = this.GetDeep (ng);

			foreach (var node in ng.GetNodes ())
			{
				var level = node.Level;
				var parent = parents[level];

				double fontSize = AbstractTreeGraphicController<EntryNode>.GetFontSize (deep, level);

				var values = this.GetValues (node);
				var w = this.CreateTile (parent, node.EntryGuid, level, fontSize, node.NodeType, values, fontFactors);

				if (parents.Count <= level+1)
				{
					parents.Add (null);
				}

				parents[level+1] = w;
			}

			this.UpdateSelection (selectedGuid, crop);
		}

		private TreeGraphicValue[] GetValues(EntryNode node)
		{
			var list = new List<TreeGraphicValue> ();

			list.Add (new TreeGraphicValue (TypeConverters.DateToString (node.Date), null));

			if (!string.IsNullOrEmpty (node.Debit) ||
				!string.IsNullOrEmpty (node.Credit))
			{
				list.Add (new TreeGraphicValue (node.Debit + " — " + node.Credit, null));
			}
			
			list.Add (new TreeGraphicValue (node.Title, null));
			list.Add (new TreeGraphicValue (TypeConverters.AmountToString (node.Value), null));

			return list.ToArray ();
		}

		private int GetDeep(EntriesNodeGetter nodeGetter)
		{
			return nodeGetter.GetNodes ().Max (x => x.Level) + 1;
		}
	}
}
