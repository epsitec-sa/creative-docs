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
	/// Ne retourne que les objets qui appartiennent à un groupe donné.
	/// Si filterGuid vaut IsEmpty, on retourne tous les objets.
	/// </summary>
	public class FilterNodeGetter : INodeGetter<GuidNode>  // outputNodes
	{
		public FilterNodeGetter(DataAccessor accessor, INodeGetter<GuidNode> objectNodes)
		{
			this.accessor    = accessor;
			this.objectNodes = objectNodes;

			this.outputNodes = new List<GuidNode> ();
		}


		public void SetParams(Timestamp? timestamp, Guid filterGuid)
		{
			this.timestamp  = timestamp;
			this.filterGuid = filterGuid;

			this.UpdateData ();
		}


		public int Count
		{
			get
			{
				if (this.filterGuid.IsEmpty)
				{
					return this.objectNodes.Count;
				}
				else
				{
					return this.outputNodes.Count;
				}
			}
		}

		public GuidNode this[int index]
		{
			get
			{
				if (this.filterGuid.IsEmpty)
				{
					if (index >= 0 && index < this.objectNodes.Count)
					{
						return this.objectNodes[index];
					}
					else
					{
						return GuidNode.Empty;
					}
				}
				else
				{
					if (index >= 0 && index < this.outputNodes.Count)
					{
						return this.outputNodes[index];
					}
					else
					{
						return GuidNode.Empty;
					}
				}
			}
		}


		private void UpdateData()
		{
			this.outputNodes.Clear ();

			if (!this.filterGuid.IsEmpty)
			{
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

						if (gr.Guid == this.filterGuid)  // objet faisant partie de ce groupe ?
						{
							var node = new GuidNode (obj.Guid);
							this.outputNodes.Add (node);
						}
					}
				}
			}
		}


		private readonly DataAccessor			accessor;
		private readonly INodeGetter<GuidNode>	objectNodes;
		private readonly List<GuidNode>			outputNodes;

		private Timestamp?						timestamp;
		private Guid							filterGuid;
	}
}
