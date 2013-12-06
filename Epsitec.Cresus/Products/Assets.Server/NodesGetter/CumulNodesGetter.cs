//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodesGetter
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des objets. Les groupes compactés sont
	/// vu avec des valeurs égales au total des sous-objets cachés.
	/// </summary>
	public class CumulNodesGetter : AbstractNodesGetter<CumulNode>  // outputNodes
	{
		public CumulNodesGetter(DataAccessor accessor, TreeObjectsNodesGetter inputNodes)
		{
			this.accessor   = accessor;
			this.inputNodes = inputNodes;

			this.outputNodes = new List<CumulNode> ();
		}


		public void SetParams(Timestamp? timestamp)
		{
			this.timestamp = timestamp;

			this.Compute ();
		}


		public override int Count
		{
			get
			{
				return this.outputNodes.Count;
			}
		}

		public override CumulNode this[int index]
		{
			get
			{
				return this.outputNodes[index];
			}
		}


		public ComputedAmount? GetValue(DataObject obj, CumulNode node, ObjectField field)
		{
			//	Retourne une valeur, en tenant compte des cumuls et des ratios.
			if (node.BaseType == BaseType.Objects)
			{
				//	S'il s'agit d'un objet, on retourne le montant en tenant compte du ratio.
				return CumulNodesGetter.GetValueAccordingToRatio (obj, this.timestamp, node.Ratio, field);
			}
			else
			{
				//	S'il s'agit d'un groupe et qu'il est compacté, on retourne le total cumulé.
				ComputedAmount ca;
				if (node.Cumuls.TryGetValue (field, out ca))
				{
					return ca;
				}
				else
				{
					return null;
				}
			}
		}


		private void Compute()
		{
			//	Lorsque des groupes sont compactés, ils peuvent cacher des objets.
			//	On calcule ici les totaux de toutes les valeurs des objets cachés,
			//	en tenant compte des éventuels ratios.
			this.outputNodes.Clear ();

			int count = this.inputNodes.Count;
			for (int i=0; i<count; i++)
			{
				var treeNode = this.inputNodes[i];
				var cumulNode = new CumulNode (treeNode.Guid, treeNode.BaseType, treeNode.Level, treeNode.Ratio, treeNode.Type);

				var hiddenTreeNodes = this.inputNodes.GetHideNodes (i).ToArray ();
				if (hiddenTreeNodes.Length != 0)
				{
					this.ComputeCumuls (cumulNode.Cumuls, hiddenTreeNodes);
				}

				this.outputNodes.Add (cumulNode);
			}
		}

		private void ComputeCumuls(Dictionary<ObjectField, ComputedAmount> cumuls, TreeNode[] hiddenTreeNodes)
		{
			foreach (var hiddenTreeNode in hiddenTreeNodes)
			{
				if (hiddenTreeNode.BaseType == BaseType.Objects)
				{
					var obj = this.accessor.GetObject (hiddenTreeNode.BaseType, hiddenTreeNode.Guid);

					foreach (var field in DataAccessor.ValueFields)
					{
						var ca = CumulNodesGetter.GetValueAccordingToRatio (obj, this.timestamp, hiddenTreeNode.Ratio, field);
						if (ca.HasValue)
						{
							if (cumuls.ContainsKey (field))
							{
								cumuls[field] = new ComputedAmount (cumuls[field], ca.Value);
							}
							else
							{
								cumuls.Add (field, ca.Value);
							}
						}
					}
				}
			}
		}

		private static ComputedAmount? GetValueAccordingToRatio(DataObject obj, Timestamp? timestamp, decimal? ratio, ObjectField field)
		{
			if (obj == null)
			{
				return null;
			}
			else
			{
				var value = ObjectCalculator.GetObjectPropertyComputedAmount (obj, timestamp, field);

				if (value.HasValue && ratio.HasValue)
				{
					return new ComputedAmount (value.Value, ratio.Value);
				}

				return value;
			}
		}


		private readonly DataAccessor			accessor;
		private readonly TreeObjectsNodesGetter	inputNodes;
		private readonly List<CumulNode>		outputNodes;

		private Timestamp?						timestamp;
	}
}
