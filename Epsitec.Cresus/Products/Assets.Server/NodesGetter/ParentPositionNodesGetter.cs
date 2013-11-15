//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture à des données quelconques, enrichies du parent (Guid)
	/// et de la position (int).
	/// GuidNode -> ParentPositionNode
	/// </summary>
	public class ParentPositionNodesGetter : AbstractNodesGetter<ParentPositionNode>  // outputNodes
	{
		public ParentPositionNodesGetter(AbstractNodesGetter<GuidNode> inputNodes, DataAccessor accessor, BaseType baseType)
		{
			this.inputNodes = inputNodes;
			this.accessor   = accessor;
			this.baseType   = baseType;
		}


		public Timestamp? Timestamp;


		public override int Count
		{
			get
			{
				return this.inputNodes.Count;
			}
		}

		public override ParentPositionNode this[int index]
		{
			get
			{
				var node     = this.inputNodes[index];
				var obj      = this.accessor.GetObject (this.baseType, node.Guid);
				var parent   = this.GetParent   (obj);
				var position = this.GetPosition (obj);

				return new ParentPositionNode (node.Guid, parent, position);
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

		private int GetPosition(DataObject obj)
		{
			if (obj != null)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.Timestamp, ObjectField.Position) as DataIntProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return 0;
		}


		private readonly AbstractNodesGetter<GuidNode>	inputNodes;
		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
	}
}
