//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des objets. Les groupes compactés sont
	/// vus avec des valeurs égales au total des sous-objets cachés.
	/// </summary>
	public class CumulNodeGetter : INodeGetter<CumulNode>  // outputNodes
	{
		public CumulNodeGetter(DataAccessor accessor, TreeObjectsNodeGetter inputNodes)
		{
			this.accessor   = accessor;
			this.inputNodes = inputNodes;

			this.outputNodes = new List<CumulNode> ();
			this.extractionInstructions = new List<ExtractionInstructions> ();

			this.extractionEngine = new ExtractionEngine (this.accessor);
		}


		public void SetParams(Timestamp? timestamp, List<ExtractionInstructions> extractionInstructions)
		{
			//	La liste des instructions d'extraction est utile pour la production de rapports.
			this.timestamp = timestamp;

			this.extractionInstructions.Clear ();
			if (extractionInstructions != null)
			{
				this.extractionInstructions.AddRange (extractionInstructions);
			}

			this.extractionEngine.SetParams (this.timestamp, this.extractionInstructions);

			this.Compute ();
		}


		public int Count
		{
			get
			{
				return this.outputNodes.Count;
			}
		}

		public CumulNode this[int index]
		{
			get
			{
				return this.outputNodes[index];
			}
		}


		public decimal? GetValue(DataObject obj, CumulNode node, ObjectField field)
		{
			//	Retourne une valeur, en tenant compte des cumuls et des ratios.
			return this.extractionEngine.GetValue (obj, node, field);
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
				var cumulNode = new CumulNode (treeNode.Guid, treeNode.BaseType, treeNode.Level, treeNode.Ratio, treeNode.Type, treeNode.GroupIndex);

				var hiddenTreeNodes = this.inputNodes.GetHiddenNodes (i).ToArray ();
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

					//	On prend toutes les valeurs définies pour la base des Assets (ValueFields),
					//	ainsi que les valeurs à extraire en vue d'un rapport.
					foreach (var field in this.accessor.AssetValueFields.Union (this.extractionInstructions.Select (x => x.ResultField)))
					{
						var v = this.extractionEngine.GetValueAccordingToRatio (obj, this.timestamp, hiddenTreeNode.Ratio, field);
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


		private readonly DataAccessor			accessor;
		private readonly TreeObjectsNodeGetter	inputNodes;
		private readonly List<CumulNode>		outputNodes;
		private readonly List<ExtractionInstructions> extractionInstructions;
		private readonly ExtractionEngine		extractionEngine;

		private Timestamp?						timestamp;
	}
}
