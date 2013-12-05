//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture à des groupes, enrichis du parent (Guid)
	/// et des valeurs pour le tri (ComparableData).
	/// GuidNode -> ParentNode
	/// </summary>
	public class GroupParentNodesGetter : AbstractNodesGetter<ParentNode>  // outputNodes
	{
		public GroupParentNodesGetter(AbstractNodesGetter<GuidNode> inputNodes, DataAccessor accessor)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;

			this.sortingInstructions = SortingInstructions.Empty;
		}


		public void SetParams(Timestamp? timestamp, SortingInstructions instructions)
		{
			this.timestamp           = timestamp;
			this.sortingInstructions = instructions;
		}


		public override int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public override ParentNode this[int index]
		{
			get
			{
				var node      = this.inputNodes[index];
				var obj       = this.accessor.GetObject (BaseType.Groups, node.Guid);
				var parent    = this.GetParent (obj);
				var primary   = ObjectCalculator.GetComparableData (obj, this.timestamp, this.sortingInstructions.PrimaryField);
				var secondary = ObjectCalculator.GetComparableData (obj, this.timestamp, this.sortingInstructions.SecondaryField);

				return new ParentNode (node.Guid, parent, primary, secondary);
			}
		}


		private Guid GetParent(DataObject obj)
		{
			if (obj != null)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.timestamp, ObjectField.Parent) as DataGuidProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return Guid.Empty;
		}


		private readonly AbstractNodesGetter<GuidNode>	inputNodes;
		private readonly DataAccessor					accessor;

		private Timestamp?								timestamp;
		private SortingInstructions						sortingInstructions;
	}
}
