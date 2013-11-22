//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture à des données quelconques, enrichies de la OrderValue (string).
	/// GuidNode -> OrderNode
	/// </summary>
	public class OrderNodesGetter : AbstractNodesGetter<OrderNode>  // outputNodes
	{
		public OrderNodesGetter(AbstractNodesGetter<GuidNode> inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;

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

		public override OrderNode this[int index]
		{
			get
			{
				var node      = this.inputNodes[index];
				var obj       = this.accessor.GetObject (this.baseType, node.Guid);
				var primary   = ObjectCalculator.GetComparableData (obj, this.Timestamp, this.SortingInstructions.PrimaryField);
				var secondary = ObjectCalculator.GetComparableData (obj, this.Timestamp, this.SortingInstructions.SecondaryField);

				return new OrderNode (node.Guid, primary, secondary);
			}
		}


		private readonly AbstractNodesGetter<GuidNode>	inputNodes;
		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
	}
}
