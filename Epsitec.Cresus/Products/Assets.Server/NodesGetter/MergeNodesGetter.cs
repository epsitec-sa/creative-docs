//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Gère l'accès en lecture à la fusion des objets dans les groupes.
	/// LevelNode + SortableNode -> LevelNode
	///    Group  +    Object    ->   mixte
	/// 
	///  >  Group
	///    >  Object
	///    >  Object
	///  >  Group
	///    >  Object
	///  >  Group
	///  >  Group
	///    >  Object
	///    >  Object
	///    >  Object
	///  >  Group
	///    >  Object
	///    >  Object
	/// 
	/// </summary>
	public class MergeNodesGetter : AbstractNodesGetter<LevelNode>  // outputNodes
	{
		public MergeNodesGetter(DataAccessor accessor, AbstractNodesGetter<LevelNode> groupNodes, AbstractNodesGetter<SortableNode> objectNodes)
		{
			this.accessor    = accessor;
			this.groupNodes  = groupNodes;
			this.objectNodes = objectNodes;

			this.outputNodes = new List<LevelNode> ();
		}


		public void SetParams(Timestamp? timestamp)
		{
			this.timestamp = timestamp;
			this.UpdateData ();
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

		private void UpdateData()
		{
			this.outputNodes.Clear ();

			if (this.groupNodes.Count == 0)
			{
				this.AddObjects ();
			}
			else
			{
				this.MergeObjects ();
			}
		}

		private void AddObjects()
		{
			foreach (var objectNode in this.objectNodes.Nodes)
			{
				this.outputNodes.Add (new LevelNode (objectNode.Guid, BaseType.Objects, 0));
			}
		}

		private void MergeObjects()
		{
			foreach (var inputNode in this.groupNodes.Nodes)
			{
				var node = new LevelNode (inputNode.Guid, BaseType.Groups, inputNode.Level);
				this.outputNodes.Add (node);

				foreach (var objectNode in this.objectNodes.Nodes)
				{
					var obj = this.accessor.GetObject (BaseType.Objects, objectNode.Guid);

					for (int i=0; i<10; i++)  // ObjectField.GroupGuid0..9
					{
						var groupGuid = ObjectCalculator.GetObjectPropertyGuid
						(
							obj,
							this.timestamp,
							ObjectField.GroupGuid+i,
							inputValue: true
						);

						if (groupGuid == inputNode.Guid)  // objet faisant partie de ce groupe ?
						{
							node = new LevelNode (objectNode.Guid, BaseType.Objects, inputNode.Level+1);
							this.outputNodes.Add (node);
						}
					}
				}
			}
		}


		private readonly DataAccessor						accessor;
		private readonly AbstractNodesGetter<LevelNode>		groupNodes;
		private readonly AbstractNodesGetter<SortableNode>	objectNodes;
		private readonly List<LevelNode>					outputNodes;

		private Timestamp?									timestamp;
	}
}
