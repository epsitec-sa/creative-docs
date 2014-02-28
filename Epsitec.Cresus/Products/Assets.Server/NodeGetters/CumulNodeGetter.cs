//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des objets. Les groupes compactés sont
	/// vu avec des valeurs égales au total des sous-objets cachés.
	/// </summary>
	public class CumulNodeGetter : AbstractNodeGetter<CumulNode>  // outputNodes
	{
		public CumulNodeGetter(DataAccessor accessor, TreeObjectsNodeGetter inputNodes)
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


		public decimal? GetValue(DataObject obj, CumulNode node, ObjectField field)
		{
			//	Retourne une valeur, en tenant compte des cumuls et des ratios.
			if (obj != null)
			{
				if (node.BaseType == BaseType.Assets)
				{
					//	S'il s'agit d'un objet, on retourne le montant en tenant compte du ratio.
					return CumulNodeGetter.GetValueAccordingToRatio (obj, this.timestamp, node.Ratio, field);
				}
				else
				{
					//	S'il s'agit d'un groupe et qu'il est compacté, on retourne le total cumulé.
					decimal v;
					if (node.Cumuls.TryGetValue (field, out v))
					{
						return v;
					}
				}
			}

			return null;
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

		private void ComputeCumuls(Dictionary<ObjectField, decimal> cumuls, TreeNode[] hiddenTreeNodes)
		{
			foreach (var hiddenTreeNode in hiddenTreeNodes)
			{
				if (hiddenTreeNode.BaseType == BaseType.Assets)
				{
					var obj = this.accessor.GetObject (BaseType.Assets, hiddenTreeNode.Guid);

					foreach (var field in this.accessor.ValueFields)
					{
						var v = CumulNodeGetter.GetValueAccordingToRatio (obj, this.timestamp, hiddenTreeNode.Ratio, field);
						if (v.HasValue)
						{
							if (cumuls.ContainsKey (field))  // deuxième et suivante valeur ?
							{
								cumuls[field] += v.Value;  // addition
							}
							else  // première valeur ?
							{
								cumuls[field] = v.Value;
							}
						}
					}
				}
			}
		}

		private static decimal? GetValueAccordingToRatio(DataObject obj, Timestamp? timestamp, decimal? ratio, ObjectField field)
		{
			//	Retourne la valeur d'un champ ObjectField.Valeur*, en tenant compte du ratio.
			if (obj != null)
			{
				decimal? m = null;

				if (field == ObjectField.MainValue)
				{
					var value = ObjectProperties.GetObjectPropertyAmortizedAmount (obj, timestamp, field);

					if (value.HasValue && value.Value.FinalAmortizedAmount.HasValue)
					{
						m = value.Value.FinalAmortizedAmount.Value;
					}
				}
				else
				{
					var value = ObjectProperties.GetObjectPropertyComputedAmount (obj, timestamp, field);

					if (value.HasValue && value.Value.FinalAmount.HasValue)
					{
						m = value.Value.FinalAmount.Value;
					}
				}

				if (m.HasValue)
				{
					if (ratio.HasValue)  // y a-t-il un ratio ?
					{
						return m.Value * ratio.Value;
					}
					else
					{
						return m;
					}
				}
			}

			return null;
		}


		private readonly DataAccessor			accessor;
		private readonly TreeObjectsNodeGetter	inputNodes;
		private readonly List<CumulNode>		outputNodes;

		private Timestamp?						timestamp;
	}
}
