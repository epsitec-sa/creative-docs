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


		public AbstractCumulValue GetValueAccordingToRatio(DataAccessor accessor, DataObject obj, Timestamp? timestamp, decimal? ratio, ObjectField field)
		{
			//	Retourne la valeur d'un champ, en tenant compte du ratio.
			if (obj != null)
			{
				AbstractCumulValue v = null;

				if (field == ObjectField.MainValue)
				{
					//	Traite le cas de la valeur comptable principale.
					var value = ObjectProperties.GetObjectPropertyAmortizedAmount (obj, timestamp, field);

					if (value.HasValue)
					{
						var aa = value.Value.FinalAmount;
						if (aa.HasValue)
						{
							v = new DecimalCumulValue (aa.Value);
						}
					}
				}
				else if (this.extractionInstructions.Select (x => x.ResultField).Contains (field))
				{
					//	Traite le cas des valeurs supplémentaires extraites pour les rapports
					//	(ObjectField.MCH2Report+n).
					var ei = this.extractionInstructions.Where (x => x.ResultField == field).FirstOrDefault ();
					v = ExtractionEngine.GetExtractionInstructions (accessor, obj, ei);
				}
				else
				{
					//	Traite le cas des valeurs supplémentaires définies par l'utilisateur
					//	pour les Assets (valeur d'assurance, valeur imposable, etc.)
					var value = ObjectProperties.GetObjectPropertyComputedAmount (obj, timestamp, field);

					if (value.HasValue && value.Value.FinalAmount.HasValue)
					{
						v = new DecimalCumulValue (value.Value.FinalAmount.Value);
					}
				}

				if (v is DecimalCumulValue)
				{
					var d = v as DecimalCumulValue;

					if (d.IsExist)
					{
						if (ratio.HasValue)  // y a-t-il un ratio ?
						{
							return new DecimalCumulValue (d.Value * ratio.Value);
						}
						else
						{
							return v;
						}
					}
				}
				else
				{
					return v;
				}
			}

			return null;
		}

		private static AbstractCumulValue GetExtractionInstructions(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Calcule un montant à extraire des données, selon les instructions ExtractionInstructions.
			if (obj != null)
			{
				switch (extractionInstructions.ExtractionAmount)
				{
					case ExtractionAmount.StateAt:
						return ExtractionEngine.GetStateAt (accessor, obj, extractionInstructions);

					case ExtractionAmount.LastFiltered:
						return ExtractionEngine.GetLastFiltered (accessor, obj, extractionInstructions);

					case ExtractionAmount.DeltaSum:
						return ExtractionEngine.GetDeltaSum (accessor, obj, extractionInstructions);

					case ExtractionAmount.UserColumn:
						return ExtractionEngine.GetUserColumn (accessor, obj, extractionInstructions);

					default:
						throw new System.InvalidOperationException (string.Format ("Unknown ExtractionAmount {0}", extractionInstructions.ExtractionAmount));
				}
			}

			return null;
		}

		private static AbstractCumulValue GetStateAt(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Retourne la valeur définie à la fin de la période, ou antérieurement.
			var timestamp = new Timestamp(extractionInstructions.Range.ExcludeTo, 0);
			var p = obj.GetSyntheticProperty (timestamp, ObjectField.MainValue) as DataAmortizedAmountProperty;

			if (p != null)
			{
				return new DecimalCumulValue (p.Value.FinalAmount);
			}

			return null;
		}

		private static AbstractCumulValue GetLastFiltered(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Pour une période donnée, retourne la dernière valeur définie dans un événement
			//	d'un type donné.
			DecimalCumulValue value = null;

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
							value = new DecimalCumulValue (aa.Value);
						}
					}
				}
			}

			return value;
		}

		private static AbstractCumulValue GetDeltaSum(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			//	Retourne le total des variations effectuées par un événement donné dans une période donnée.
			//	La baisse d'une valeur retourne un montant négatif si Inverted est false.
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
								value = aa.Value - lastValue.Value;
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

			if (sum.HasValue)
			{
				return new DecimalCumulValue (extractionInstructions.Inverted ? -sum : sum);
			}
			else
			{
				return new DecimalCumulValue (sum);
			}
		}

		private static AbstractCumulValue GetUserColumn(DataAccessor accessor, DataObject obj, ExtractionInstructions extractionInstructions)
		{
			var timestamp = new Timestamp (extractionInstructions.Range.ExcludeTo, 0);

			{
				var p = obj.GetSyntheticProperty (timestamp, extractionInstructions.ResultField) as DataComputedAmountProperty;

				if (p != null)
				{
					return new DecimalCumulValue (p.Value.FinalAmount);
				}
			}

			{
				var p = obj.GetSyntheticProperty (timestamp, extractionInstructions.ResultField) as DataDateProperty;

				if (p != null)
				{
					return new DateCumulValue (p.Value);
				}
			}

			{
				var p = obj.GetSyntheticProperty (timestamp, extractionInstructions.ResultField) as DataStringProperty;

				if (p != null)
				{
					return new StringCumulValue (p.Value);
				}
			}

			return null;
		}

		private static bool CompareEventTypes(EventType[] extractionTypes, EventType eventType)
		{
			return extractionTypes.Where (x => x == eventType).Any ();
		}


		private readonly DataAccessor			accessor;
		private Timestamp?						timestamp;
		private List<ExtractionInstructions>	extractionInstructions;
	}
}
