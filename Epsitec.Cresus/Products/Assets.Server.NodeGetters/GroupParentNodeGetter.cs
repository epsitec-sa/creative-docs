//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Accès en lecture à des groupes, enrichis du parent (Guid)
	/// et des valeurs pour le tri (ComparableData).
	/// GuidNode -> ParentNode
	/// </summary>
	public class GroupParentNodeGetter : INodeGetter<ParentNode>  // outputNodes
	{
		public GroupParentNodeGetter(INodeGetter<GuidNode> inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;

			this.sortingInstructions = SortingInstructions.Empty;
		}


		public void SetParams(Timestamp? timestamp, SortingInstructions instructions)
		{
			this.timestamp           = timestamp;
			this.sortingInstructions = instructions;
		}


		public int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public ParentNode this[int index]
		{
			get
			{
				var node      = this.inputNodes[index];
				var obj       = this.accessor.GetObject (this.baseType, node.Guid);
				var parent    = this.GetParent (obj);
				var primary   = ObjectProperties.GetComparableData (this.accessor, obj, this.timestamp, this.sortingInstructions.PrimaryField);
				var secondary = ObjectProperties.GetComparableData (this.accessor, obj, this.timestamp, this.sortingInstructions.SecondaryField);

				return new ParentNode (node.Guid, parent, primary, secondary);
			}
		}


		private Guid GetParent(DataObject obj)
		{
			if (obj != null)
			{
				var p = ObjectProperties.GetObjectSyntheticProperty (obj, this.timestamp, ObjectField.GroupParent) as DataGuidProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return Guid.Empty;
		}


		private readonly INodeGetter<GuidNode>	inputNodes;
		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;

		private Timestamp?						timestamp;
		private SortingInstructions				sortingInstructions;
	}
}
