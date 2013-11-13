//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Accès en lecture à des données quelconques, enrichies du niveau (Node.Level)
	/// selon la propriété ObjectField.Level.
	/// </summary>
	public class LevelNodesGetter : AbstractNodesGetter<LevelNode>  // outputNodes
	{
		public LevelNodesGetter(AbstractNodesGetter<GuidNode> inputNodes, DataAccessor accessor, BaseType baseType)
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

		public override LevelNode this[int index]
		{
			get
			{
				var node = this.inputNodes[index];
				var level = this.GetLevel (node.Guid);

				return new LevelNode (node.Guid, level);
			}
		}


		private int GetLevel(Guid guid)
		{
			var obj = this.accessor.GetObject (this.baseType, guid);
			if (obj != null)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, this.Timestamp, ObjectField.Level) as DataIntProperty;
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
