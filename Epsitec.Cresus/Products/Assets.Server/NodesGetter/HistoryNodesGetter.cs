//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Accès en lecture aux événements d'un objet pour l'historique d'une propriété.
	/// On ne s'intéresse donc qu'aux événements qui définissent la propriété, en
	/// plus de celui placé à ForcedTimestamp.
	/// événements d'un objet -> GuidNode
	/// </summary>
	public class HistoryNodesGetter : AbstractNodesGetter<GuidNode>  // outputNodes
	{
		public HistoryNodesGetter()
		{
			this.nodes = new List<GuidNode> ();
		}


		public DataObject						DataObject;
		public ObjectField						Field;
		public Timestamp?						ForcedTimestamp;


		public override int Count
		{
			get
			{
				return this.nodes.Count;
			}
		}

		public override GuidNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.nodes.Count)
				{
					return this.nodes[index];
				}
				else
				{
					return GuidNode.Empty;
				}
			}
		}

		public override void UpdateData()
		{
			this.nodes.Clear ();

			if (this.DataObject != null)
			{
				foreach (var e in this.DataObject.Events)
				{
					if (e.Timestamp != this.ForcedTimestamp)
					{
						var p = e.GetProperty (this.Field);
						if (p == null)
						{
							continue;
						}
					}

					this.nodes.Add (new GuidNode (e.Guid));
				}
			}
		}


		private readonly List<GuidNode> nodes;
	}
}
