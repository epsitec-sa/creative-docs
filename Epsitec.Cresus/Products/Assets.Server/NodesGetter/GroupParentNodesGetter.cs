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
	/// et de la OrderValue (string).
	/// GuidNode -> ParentNode
	/// </summary>
	public class GroupParentNodesGetter : AbstractNodesGetter<ParentNode>  // outputNodes
	{
		public GroupParentNodesGetter(AbstractNodesGetter<GuidNode> inputNodes, DataAccessor accessor)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;

			this.SortingInstructions = SortingInstructions.Empty;
		}


		public Timestamp?						Timestamp;
		public SortingInstructions				SortingInstructions;


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
				var primary   = ObjectCalculator.GetComparableData (obj, this.Timestamp, this.SortingInstructions.PrimaryField);
				var secondary = ObjectCalculator.GetComparableData (obj, this.Timestamp, this.SortingInstructions.SecondaryField);

				return new ParentNode (node.Guid, parent, primary, secondary);
			}
		}


		private Guid GetParent(DataObject obj)
		{
			if (obj != null)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.Timestamp, ObjectField.Parent) as DataGuidProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return Guid.Empty;
		}


		private readonly AbstractNodesGetter<GuidNode>	inputNodes;
		private readonly DataAccessor					accessor;
	}
}
