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
			//	Si un filtre est défini, on génère la liste filtrée this.outputNodes.
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

						if (!gr.IsEmpty)
						{
							if (this.IsInsideGroup (gr.Guid))
							{
								var node = new GuidNode (obj.Guid);
								this.outputNodes.Add (node);
							}
						}
					}
				}
			}
		}

		private bool IsInsideGroup(Guid groupGuid)
		{
			//	On considère que l'objet appartient au groupe s'il se réfère directement au
			//	groupe, ou s'il se réfère à un groupe fils.
			if (groupGuid == this.filterGuid)  // se réfère directement au groupe ?
			{
				return true;
			}

			//	On cherche si le groupe est un fils de this.filterGuid.
			while (true)
			{
				groupGuid = this.GetParentGroup (groupGuid);

				if (groupGuid.IsEmpty)
				{
					return false;
				}
				else if (groupGuid == this.filterGuid)
				{
					return true;
				}
			}
		}

		private Guid GetParentGroup(Guid groupGuid)
		{
			var group = this.accessor.GetObject (BaseType.Groups, groupGuid);

			if (group != null)
			{
				var parent = ObjectProperties.GetObjectPropertyGuid
				(
					group,
					this.timestamp,
					ObjectField.GroupParent,
					inputValue: true
				);

				if (!parent.IsEmpty)
				{
					return parent;
				}
			}

			return Guid.Empty;
		}


		private readonly DataAccessor			accessor;
		private readonly INodeGetter<GuidNode>	objectNodes;
		private readonly List<GuidNode>			outputNodes;

		private Timestamp?						timestamp;
		private Guid							filterGuid;
	}
}
