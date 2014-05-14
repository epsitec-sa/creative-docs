//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Accès en lecture aux événements d'un objet pour l'historique d'une propriété.
	/// On ne s'intéresse donc qu'aux événements qui définissent la propriété, en
	/// plus de celui placé à ForcedTimestamp.
	/// événements d'un objet -> GuidNode
	/// </summary>
	public class HistoryNodeGetter : AbstractNodeGetter<GuidNode>  // outputNodes
	{
		public HistoryNodeGetter()
		{
			this.nodes = new List<GuidNode> ();
		}


		public void SetParams(DataObject dataObject, ObjectField field, Timestamp? forcedTimestamp)
		{
			this.dataObject      = dataObject;
			this.field           = field;
			this.forcedTimestamp = forcedTimestamp;

			this.UpdateData ();
		}


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


		private void UpdateData()
		{
			this.nodes.Clear ();

			if (this.dataObject != null)
			{
				foreach (var e in this.dataObject.Events)
				{
					if (e.Timestamp != this.forcedTimestamp)
					{
						var p = e.GetProperty (this.field);
						if (p == null)
						{
							continue;
						}
					}

					this.nodes.Add (new GuidNode (e.Guid));
				}
			}
		}


		private readonly List<GuidNode>			nodes;

		private DataObject						dataObject;
		private ObjectField						field;
		private Timestamp?						forcedTimestamp;
	}
}
