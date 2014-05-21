﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
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
	public class MergeNodeGetter : INodeGetter<LevelNode>  // outputNodes
	{
		public MergeNodeGetter(DataAccessor accessor, INodeGetter<LevelNode> groupNodes, INodeGetter<SortableNode> objectNodes)
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


		public int Count
		{
			get
			{
				return this.outputNodes.Count;
			}
		}

		public LevelNode this[int index]
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
			foreach (var objectNode in this.objectNodes.GetNodes ())
			{
				this.outputNodes.Add (new LevelNode (objectNode.Guid, BaseType.Assets, 0, null));
			}
		}

		private void MergeObjects()
		{
			foreach (var inputNode in this.groupNodes.GetNodes ())
			{
				var node = new LevelNode (inputNode.Guid, BaseType.Groups, inputNode.Level, null);
				this.outputNodes.Add (node);

				foreach (var objectNode in this.objectNodes.GetNodes ())
				{
					var obj = this.accessor.GetObject (BaseType.Assets, objectNode.Guid);

					foreach (var field in DataAccessor.GroupGuidRatioFields)
					{
						var gr = ObjectProperties.GetObjectPropertyGuidRatio
						(
							obj,
							this.timestamp,
							field,
							inputValue: true
						);

						if (gr.Guid == inputNode.Guid)  // objet faisant partie de ce groupe ?
						{
							node = new LevelNode (objectNode.Guid, BaseType.Assets, inputNode.Level+1, gr.Ratio);
							this.outputNodes.Add (node);
						}
					}
				}
			}
		}


		private readonly DataAccessor				accessor;
		private readonly INodeGetter<LevelNode>		groupNodes;
		private readonly INodeGetter<SortableNode>	objectNodes;
		private readonly List<LevelNode>			outputNodes;

		private Timestamp?							timestamp;
	}
}