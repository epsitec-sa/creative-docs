//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	public class MergeNodesGetter : AbstractNodesGetter<LevelNode>  // outputNodes
	{
		public MergeNodesGetter(DataAccessor accessor, AbstractNodesGetter<LevelNode> groupNodes, AbstractNodesGetter<GuidNode> objectNodes)
		{
			this.accessor    = accessor;
			this.groupNodes  = groupNodes;
			this.objectNodes = objectNodes;

			this.outputNodes = new List<LevelNode> ();
		}


		public override int Count
		{
			get
			{
				return this.outputNodes.Count;
			}
		}

		public override LevelNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.outputNodes.Count)
				{
					return this.outputNodes[index];
				}
				else
				{
					return LevelNode.Empty;
				}
			}
		}

		public override void UpdateData()
		{
			this.outputNodes.Clear ();

			foreach (var inputNode in this.groupNodes.Nodes)
			{
				this.outputNodes.Add (new LevelNode (inputNode.Guid, BaseType.Groups, inputNode.Level));

				foreach (var objectNode in this.objectNodes.Nodes)
				{
					var obj = this.accessor.GetObject (BaseType.Objects, objectNode.Guid);

					for (int i=0; i<10; i++)
					{
						var groupGuid = ObjectCalculator.GetObjectPropertyGuid (obj, null, ObjectField.GroupGuid+i);
						if (groupGuid == inputNode.Guid)
						{
							this.outputNodes.Add (new LevelNode (objectNode.Guid, BaseType.Objects, inputNode.Level+1));
						}
					}
				}
			}
		}


		private readonly DataAccessor						accessor;
		private readonly AbstractNodesGetter<LevelNode>		groupNodes;
		private readonly AbstractNodesGetter<GuidNode>		objectNodes;
		private readonly List<LevelNode>					outputNodes;
	}
}
