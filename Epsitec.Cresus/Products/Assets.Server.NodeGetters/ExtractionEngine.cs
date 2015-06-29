//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class ExtractionEngine
	{
		public ExtractionEngine(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public void SetParams(Timestamp? timestamp, List<ExtractionInstructions> extractionInstructions)
		{
			this.timestamp              = timestamp;
			this.extractionInstructions = extractionInstructions;
		}


		public decimal? GetValue(DataObject obj, CumulNode node, ObjectField field)
		{
			//	Retourne une valeur, en tenant compte des cumuls et des ratios.
			if (obj != null)
			{
				if (node.BaseType == BaseType.Assets)
				{
					//	S'il s'agit d'un objet, on retourne le montant en tenant compte du ratio.
					return this.GetValueAccordingToRatio (this.accessor, obj, this.timestamp, node.Ratio, field);
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


		public decimal? GetValueAccordingToRatio(DataAccessor accessor, DataObject obj, Timestamp? timestamp, decimal? ratio, ObjectField field)
		{
			//	Retourne la valeur d'un champ, en tenant compte du ratio.
			if (obj != null)
			{
				decimal? m = null;

				if (field == ObjectField.MainValue)
				{
					//	Traite le cas de la valeur comptable principale.
					var value = ObjectProperties.GetObjectPropertyAmortizedAmount (obj, timestamp, field);

					if (value.HasValue)
					{
						var aa = value.Value.FinalAmount;
						if (aa.HasValue)
						{
							m = aa.Value;
						}
					}
				}
				else if (this.extractionInstructions.Select (x => x.ResultField).Contains (field))
				{
					//	Traite le cas des valeurs supplémentaires extraites pour les rapports
					//	(ObjectField.MCH2Report+n).
					var ei = this.extractionInstructions.Where (x => x.ResultField == field).FirstOrDefault ();
					m = ExtractionEngine.GetExtractionInstructions (accessor, obj, ei);
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

		private static decimal? GetExtractionInstructions(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Calcule un montant à extraire des données, selon les instructions ExtractionInstructions.
			if (obj != null)
			{
				switch (extractionInstructions.ExtractionAmount)
				{
					case ExtractionAmount.StateAt:
						return ExtractionEngine.GetStateAt (accessor, obj, extractionInstructions);

					case ExtractionAmount.DeltaFiltered:
						return ExtractionEngine.GetDeltaFiltered (accessor, obj, extractionInstructions);

					case ExtractionAmount.LastFiltered:
						return ExtractionEngine.GetLastFiltered (accessor, obj, extractionInstructions);

					case ExtractionAmount.Amortizations:
						return ExtractionEngine.GetAmortizations (accessor, obj, extractionInstructions);

					case ExtractionAmount.UserColumn:
						return ExtractionEngine.GetUserColumn (accessor, obj, extractionInstructions);

					default:
						throw new System.InvalidOperationException (string.Format ("Unknown ExtractionAmount {0}", extractionInstructions.ExtractionAmount));
				}
			}

			return null;
		}

		private static decimal? GetStateAt(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Retourne la valeur définie à la fin de la période, ou antérieurement.
			var timestamp = new Timestamp(extractionInstructions.Range.ExcludeTo, 0);
			var p = obj.GetSyntheticProperty (timestamp, ObjectField.MainValue) as DataAmortizedAmountProperty;

			if (p != null)
			{
				return p.Value.FinalAmount;
			}

			return null;
		}

		private static decimal? GetDeltaFiltered(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Pour une période donnée, retourne la variation d'une valeur suite à un
			//	type d'événement donné (début - fin, donc une valeur qui diminue suite
			//	à un amortissement par exemple retourne une valeur positive).
			decimal? lastValue = null;
			decimal? value = null;

			foreach (var e in obj.Events)
			{
				var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
				if (p != null)
				{
					var aa = p.Value.FinalAmount;
					if (aa.HasValue)
					{
						if (ExtractionEngine.CompareEventTypes (extractionInstructions.FilteredEventType, e.Type) &&
							extractionInstructions.Range.IsInside (e.Timestamp.Date))
						{
							if (lastValue.HasValue)
							{
								value = lastValue.Value - aa.Value;
							}
							else
							{
								value = aa.Value;
							}
						}

						lastValue = aa.Value;
					}
				}
			}

			return value;
		}

		private static decimal? GetLastFiltered(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Pour une période donnée, retourne la dernière valeur définie dans un événement
			//	d'un type donné.
			decimal? value = null;

			foreach (var e in obj.Events)
			{
				var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
				if (p != null)
				{
					var aa = p.Value.FinalAmount;
					if (aa.HasValue)
					{
						if (ExtractionEngine.CompareEventTypes (extractionInstructions.FilteredEventType, e.Type) &&
							extractionInstructions.Range.IsInside (e.Timestamp.Date))
						{
							value = aa.Value;
						}
					}
				}
			}

			return value;
		}

		private static decimal? GetAmortizations(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Retourne le total des amortissements effectués dans une période donnée.
			decimal? lastValue = null;
			decimal? sum = null;

			foreach (var e in obj.Events)
			{
				var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
				if (p != null)
				{
					var aa = p.Value.FinalAmount;
					if (aa.HasValue)
					{
						if (ExtractionEngine.CompareEventTypes (extractionInstructions.FilteredEventType, e.Type) &&
							extractionInstructions.Range.IsInside (e.Timestamp.Date))
						{
							decimal value;

							if (lastValue.HasValue)
							{
								value = lastValue.Value - aa.Value;
							}
							else
							{
								value = aa.Value;
							}

							if (sum.HasValue)
							{
								sum += value;
							}
							else
							{
								sum = value;
							}
						}

						lastValue = aa.Value;
					}
				}
			}

			return sum;
		}

		private static decimal? GetUserColumn(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			var timestamp = new Timestamp (extractionInstructions.Range.ExcludeTo, 0);
			var p = obj.GetSyntheticProperty (timestamp, extractionInstructions.ResultField) as DataComputedAmountProperty;

			if (p != null)
			{
				return p.Value.FinalAmount;
			}

			return null;
		}

		private static bool CompareEventTypes(EventType extractionType, EventType eventType)
		{
			if (extractionType == EventType.AmortizationAuto)
			{
				return eventType == extractionType
					|| eventType == EventType.AmortizationPreview;
			}
			else
			{
				return eventType == extractionType;
			}
		}


		private readonly DataAccessor			accessor;
		private Timestamp?						timestamp;
		private List<ExtractionInstructions>	extractionInstructions;
	}
}
