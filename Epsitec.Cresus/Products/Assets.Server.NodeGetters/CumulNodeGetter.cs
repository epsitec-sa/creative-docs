﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Gère l'accès en lecture "en arbre" à des objets. Les groupes compactés sont
	/// vu avec des valeurs égales au total des sous-objets cachés.
	/// </summary>
	public class CumulNodeGetter : INodeGetter<CumulNode>  // outputNodes
	{
		public CumulNodeGetter(DataAccessor accessor, TreeObjectsNodeGetter inputNodes)
		{
			this.accessor   = accessor;
			this.inputNodes = inputNodes;

			this.outputNodes = new List<CumulNode> ();
			this.extractionInstructions = new List<ExtractionInstructions> ();
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
			if (obj != null)
			{
				if (node.BaseType == BaseType.Assets)
				{
					//	S'il s'agit d'un objet, on retourne le montant en tenant compte du ratio.
					return this.GetValueAccordingToRatio (obj, this.timestamp, node.Ratio, field);
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

					//	On prend toutes les valeurs définies pour la base des Assets (ValueFields),
					//	ainsi que les valeurs à extraire en vue d'un rapport.
					foreach (var field in this.accessor.AssetValueFields.Union (this.extractionInstructions.Select (x => x.ResultField)))
					{
						var v = this.GetValueAccordingToRatio (obj, this.timestamp, hiddenTreeNode.Ratio, field);
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

		private decimal? GetValueAccordingToRatio(DataObject obj, Timestamp? timestamp, decimal? ratio, ObjectField field)
		{
			//	Retourne la valeur d'un champ, en tenant compte du ratio.
			if (obj != null)
			{
				decimal? m = null;

				if (field == ObjectField.MainValue)
				{
					//	Traite le cas de la valeur comptable principale.
					var value = ObjectProperties.GetObjectPropertyAmortizedAmount (obj, timestamp, field);

					if (value.HasValue && value.Value.FinalAmortizedAmount.HasValue)
					{
						m = value.Value.FinalAmortizedAmount.Value;
					}
				}
				else if (this.extractionInstructions.Select (x => x.ResultField).Contains (field))
				{
					//	Traite le cas des valeurs supplémentaires extraites pour les rapports
					//	(ObjectField.MCH2Report+n).
					var ei = this.extractionInstructions.Where (x => x.ResultField == field).FirstOrDefault ();
					m = CumulNodeGetter.GetExtractionInstructions (obj, ei);
				}
				else
				{
					//	Traite le cas des valeurs supplémentaires définies par l'utilisateur
					//	pour les Assets (valeur d'assurance, valeur imposable, etc.)
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

		private static decimal? GetExtractionInstructions(DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Calcule un montant à extraire des données, selon les instructions ExtractionInstructions.
			if (obj != null)
			{
				switch (extractionInstructions.ExtractionAmount)
				{
					case ExtractionAmount.StateAt:
						return CumulNodeGetter.GetStateAt (obj, extractionInstructions);

					case ExtractionAmount.Filtered:
						return CumulNodeGetter.GetFiltered (obj, extractionInstructions);

					case ExtractionAmount.Amortizations:
						return CumulNodeGetter.GetAmortizations (obj, extractionInstructions);

					default:
						throw new System.InvalidOperationException (string.Format ("Unknown ExtractionAmount {0}", extractionInstructions.ExtractionAmount));
				}
			}

			return null;
		}

		private static decimal? GetStateAt(DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Retourne la valeur définie à la fin de la période, ou antérieurement.
			var timestamp = new Timestamp(extractionInstructions.Range.ExcludeTo, 0);
			var p = obj.GetSyntheticProperty (timestamp, ObjectField.MainValue) as DataAmortizedAmountProperty;

			if (p != null)
			{
				return p.Value.FinalAmortizedAmount;
			}

			return null;
		}

		private static decimal? GetFiltered(DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Pour une période donnée, retourne la dernière valeur définie dans un événement
			//	d'un type donné.
			decimal? value = null;

			foreach (var e in obj.Events.Where (x =>
				x.Type == extractionInstructions.FilteredEventType &&
				extractionInstructions.Range.IsInside (x.Timestamp.Date)))
			{
				var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
				if (p != null && p.Value.FinalAmortizedAmount.HasValue)
				{
					value = p.Value.FinalAmortizedAmount.Value;
				}
			}

			return value;
		}

		private static decimal? GetAmortizations(DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Retourne le total des amortissements effectués dans une période donnée.
			decimal? sum = null;

			foreach (var e in obj.Events.Where (x =>
				x.Type == extractionInstructions.FilteredEventType &&
				extractionInstructions.Range.IsInside (x.Timestamp.Date)))
			{
				var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
				if (p != null)
				{
					if (sum.HasValue)
					{
						sum += p.Value.FinalAmortization;
					}
					else
					{
						sum = p.Value.FinalAmortization;
					}
				}
			}

			return sum;
		}


		private readonly DataAccessor			accessor;
		private readonly TreeObjectsNodeGetter	inputNodes;
		private readonly List<CumulNode>		outputNodes;
		private readonly List<ExtractionInstructions> extractionInstructions;

		private Timestamp?						timestamp;
	}
}