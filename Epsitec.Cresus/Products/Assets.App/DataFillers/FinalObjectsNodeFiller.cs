//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	/// <summary>
	/// Donne accès à toutes les "feuilles" terminales.
	/// </summary>
	public class FinalObjectsNodeFiller : AbstractNodeFiller
	{
		public FinalObjectsNodeFiller(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.nodes = new List<Node> ();
			this.UpdateData ();
		}

		public override int NodesCount
		{
			get
			{
				return this.nodes.Count;
			}
		}

		public override Node GetNode(int index)
		{
			return this.nodes[index];
		}

		private void UpdateData()
		{
			//	Met à jour toutes les catégories d'immobilisation finales.
			this.nodes.Clear ();

			int count = this.accessor.GetObjectsCount (this.baseType);
			for (int i=0; i<count; i++)
			{
				Guid currentGuid;
				int currentLevel;
				this.GetData (i, out currentGuid, out currentLevel);

				//	Par défaut, on considère que la ligne ne peut être ni étendue
				//	ni compactée.
				var type = TreeTableTreeType.Final;

				if (i < count-2)
				{
					Guid nextGuid;
					int nextLevel;
					this.GetData (i+1, out nextGuid, out nextLevel);

					//	Si le noeud suivant a un niveau plus élevé, il s'agit d'une
					//	ligne pouvant être étendue ou compactée.
					if (nextLevel > currentLevel)
					{
						type = TreeTableTreeType.Expanded;
					}
				}

				if (type == TreeTableTreeType.Final)
				{
					var node = new Node (currentGuid, -1, type);
					this.nodes.Add (node);
				}
			}
		}

		private void GetData(int row, out Guid guid, out int level)
		{
			//	Retourne une donnée.
			guid = Guid.Empty;
			level = 0;

			if (row >= 0 && row < this.accessor.GetObjectsCount (this.baseType))
			{
				guid = this.accessor.GetObjectGuids (this.baseType, row, 1).FirstOrDefault ();

				var obj = this.accessor.GetObject (this.baseType, guid);
				var timestamp = new Timestamp (System.DateTime.MaxValue, 0);
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, timestamp, ObjectField.Level) as DataIntProperty;
				if (p != null)
				{
					level = p.Value;
				}
			}
		}

		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly List<Node>						nodes;
	}
}
