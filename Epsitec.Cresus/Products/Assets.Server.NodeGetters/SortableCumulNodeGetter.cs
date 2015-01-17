//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Accès en lecture à des données cumulées, enrichies d'un contenu
	/// triable (ComparableData).
	/// CumulNode -> SortableCumulNode
	/// </summary>
	public class SortableCumulNodeGetter : INodeGetter<SortableCumulNode>  // outputNodes
	{
		public SortableCumulNodeGetter(INodeGetter<CumulNode> inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;

			this.sortingInstructions = SortingInstructions.Empty;
		}


		public void SetParams(Timestamp? timestamp, SortingInstructions	instructions)
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

		public SortableCumulNode this[int index]
		{
			get
			{
				var node      = this.inputNodes[index];

				var primary   = this.GetComparableData (node, this.sortingInstructions.PrimaryField);
				var secondary = this.GetComparableData (node, this.sortingInstructions.SecondaryField);

				return new SortableCumulNode (node.Guid, node.BaseType, node.Level, node.Ratio, node.Type, node.GroupIndex, node.Cumuls, primary, secondary);
			}
		}

		private ComparableData GetComparableData(CumulNode node, ObjectField field)
		{
			decimal value;
			if (node.Cumuls.TryGetValue (field, out value))
			{
				return new ComparableData (value);
			}
			else
			{
				var obj = this.accessor.GetObject (this.baseType, node.Guid);
				return ObjectProperties.GetComparableData (this.accessor, obj, this.timestamp, field);
			}
		}


		private readonly INodeGetter<CumulNode>	inputNodes;
		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;

		private Timestamp?						timestamp;
		private SortingInstructions				sortingInstructions;
	}
}
