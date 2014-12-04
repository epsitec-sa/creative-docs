﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class Amortizations
	{
		public Amortizations(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public void Preview(DateRange processRange)
		{
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.GetNodes ())
			{
				this.Preview (processRange, node.Guid);
			}
		}

		public void Fix(System.DateTime endDate)
		{
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.GetNodes ())
			{
				this.Fix (endDate, node.Guid);
			}
		}

		public void Unpreview()
		{
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.GetNodes ())
			{
				this.Unpreview (node.Guid);
			}
		}

		public void Delete(System.DateTime startDate)
		{
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.GetNodes ())
			{
				this.Delete (startDate, node.Guid);
			}
		}


		public void Preview(DateRange processRange, Guid objectGuid)
		{
			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.GeneratesAmortizationsPreview (processRange, objectGuid);

			this.accessor.WarningsDirty = true;
		}

		public void Fix(System.DateTime endDate, Guid objectGuid)
		{
			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.FixEvents (obj, new DateRange (System.DateTime.MinValue, endDate));

			this.accessor.WarningsDirty = true;
		}

		public void Unpreview(Guid objectGuid)
		{
			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			this.accessor.WarningsDirty = true;
		}

		public void Delete(System.DateTime startDate, Guid objectGuid)
		{
			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.RemoveEvents (obj, EventType.AmortizationPreview, new DateRange (startDate, System.DateTime.MaxValue));
			this.RemoveEvents (obj, EventType.AmortizationAuto,    new DateRange (startDate, System.DateTime.MaxValue));

			this.accessor.WarningsDirty = true;
		}


		private void GeneratesAmortizationsPreview(DateRange processRange, Guid objectGuid)
		{
			//	Génère les aperçus d'amortissement pour un objet donné.
			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			//	Supprime tous les aperçus d'amortissement.
			this.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			//	Passe en revue les périodes.
			System.DateTime? lastToDate = null;

			foreach (var period in this.GetPeriods (processRange, obj))
			{
				var def     = this.GetAmortizationDefinition (obj, new Timestamp (period.IncludeFrom, 0));
				var prorata = Amortizations.GetProrataDetails (obj, def, period.IncludeFrom, lastToDate);

				var timestamp = obj.GetNewTimestamp (period.ExcludeTo.AddDays (-1));
				var history = Amortizations.GetHistoryDetails (obj, timestamp);

				if (!def.IsEmpty && !history.IsEmpty)
				{
					var details = new AmortizationDetails (def, prorata, history);

					this.CreateAmortizationPreview (obj, period.ExcludeTo.AddDays (-1), details);
					lastToDate = period.ExcludeTo;
				}	
			}

			//	Supprime tous les événements d'amortissement ordinaire après la date de fin.
			this.RemoveEvents (obj, EventType.AmortizationAuto, new DateRange (processRange.ExcludeTo, System.DateTime.MaxValue));

			//	Pour mettre à jour les éventuels amortissements extraordinaires suivants.
			Amortizations.UpdateAmounts (this.accessor, obj);
		}

		private IEnumerable<DateRange> GetPeriods(DateRange processRange, DataObject obj)
		{
			//	Retourne la liste des périodes pour lesquelles il faudra tenter des amortissements.
			if (obj.EventsCount > 0)
			{
				//	Cherche la date d'entrée de l'objet.
				var inputDate = AssetCalculator.GetFirstTimestamp (obj).Value.Date;

				if (inputDate <= processRange.ExcludeTo)
				{
					var beginDate = new System.DateTime (processRange.IncludeFrom.Year, 1, 1);

					if (beginDate < inputDate)
					{
						beginDate = inputDate;
					}

					while (beginDate < processRange.ExcludeTo)
					{
						var def = this.GetAmortizationDefinition (obj, new Timestamp (beginDate, 0));
						System.DateTime endDate;

						if (def.IsEmpty)
						{
							//	Si aucune définition d'amortissement n'existe, on essaie de nouveaux
							//	amortissements à partir de l'année prochaine.
							endDate = beginDate.AddYears (1);
						}
						else
						{
							endDate = beginDate.AddMonths (def.PeriodMonthCount);
							endDate = def.GetBeginRangeDate (endDate);

							yield return new DateRange (beginDate, endDate);
						}

						beginDate = endDate;
					}
				}
			}
		}

		private static ProrataDetails GetProrataDetails(DataObject obj, AmortizationDefinition def, System.DateTime date, System.DateTime? lastToDate)
		{
			//	Retourne tous les détails sur un amortissement ordinaire.
			if (def.IsEmpty)
			{
				return ProrataDetails.Empty;
			}

			var beginDate = def.GetBeginRangeDate (date);
			var endDate = beginDate.AddMonths (def.PeriodMonthCount);
			var range = new DateRange (beginDate, endDate);

			if (AssetCalculator.IsOutOfBoundsEvent (obj, new Timestamp (range.ExcludeTo.AddSeconds (-1), 0)))
			{
				return ProrataDetails.Empty;
			}

			//	S'il y a déjà un (ou plusieurs) amortissement (extra)ordinaire dans la période,
			//	on ne génère pas d'amortissement ordinaire.
			if (Amortizations.HasAmortizations (obj, EventType.AmortizationExtra, range) ||
				Amortizations.HasAmortizations (obj, EventType.AmortizationAuto,  range))
			{
				return ProrataDetails.Empty;
			}

			//	Génère l'aperçu d'amortissement.
			var valueDate = range.IncludeFrom;

			var inputDate = AssetCalculator.GetFirstTimestamp (obj).Value.Date;
			if (range.IsInside (inputDate))
			{
				//	Si l'objet est entrée durant la période, on utilise sa date d'entrée
				//	pour calculer l'amortissement "au prorata".
				valueDate = inputDate;
			}

			if (lastToDate.HasValue && valueDate < lastToDate.Value)
			{
				//	Si une partie de la période a déjà été amortie (par exemple suite à un
				//	changement de périodicité), on utilise la dernière date amortie pour
				//	calculer l'amortissement "au prorata".
				valueDate = lastToDate.Value;
			}

			return ProrataDetails.ComputeProrata (range, valueDate, def.ProrataType);
		}

		private static HistoryDetails GetHistoryDetails(DataObject obj, Timestamp timestamp)
		{
			decimal  baseAmount    = 0.0m;
			decimal  initialAmount = 0.0m;
			int      yearRank      = 0;
			int      periodRank    = 0;

			//	Calcule le rang de l'année ainsi que le rang dans l'année.
			//	Supposons, par exemple, qu'il existe les amortissements semestriels
			//	aux dates suivantes :
			//				yearRank periodRank
			//	30.06.2000		0		0
			//	31.12.2000		0		1
			//	30.06.2001		1		0
			//	31.12.2001		1		1
			//	30.06.2002		2		0
			//	31.12.2002		2		1

			int lastYear   = -1;

			foreach (var e in obj.Events.Where (x => x.Timestamp < timestamp))
			{
				var aa = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

				if (aa != null && aa.Value.FinalAmount.HasValue)
				{
					initialAmount = aa.Value.FinalAmount.Value;
				}

				if (e.Type == EventType.AmortizationPreview ||
					e.Type == EventType.AmortizationAuto    ||
					e.Type == EventType.AmortizationExtra   )
				{
					if (lastYear != e.Timestamp.Date.Year)
					{
						if (lastYear != -1)
						{
							yearRank++;
							periodRank = 0;
						}

						lastYear = e.Timestamp.Date.Year;
					}

					periodRank++;
				}
				else
				{
					if (aa != null && aa.Value.FinalAmount.HasValue)
					{
						baseAmount = aa.Value.FinalAmount.Value;
					}
				}
			}

			return new HistoryDetails (baseAmount, initialAmount, (decimal) yearRank, (decimal) periodRank);
		}


		private AmortizationDefinition GetAmortizationDefinition(DataObject obj, Timestamp timestamp)
		{
			//	Collecte tous les champs qui définissent comment amortir. Ils peuvent provenir
			//	de plusieurs événements différents.
			var exp       = ObjectProperties.GetObjectPropertyGuid            (obj, timestamp, ObjectField.MethodGuid);
			var taux      = ObjectProperties.GetObjectPropertyDecimal         (obj, timestamp, ObjectField.AmortizationRate);
			var years     = ObjectProperties.GetObjectPropertyDecimal         (obj, timestamp, ObjectField.AmortizationYearCount);
			var period    = ObjectProperties.GetObjectPropertyInt             (obj, timestamp, ObjectField.Periodicity);
			var prorata   = ObjectProperties.GetObjectPropertyInt             (obj, timestamp, ObjectField.Prorata);
			var round     = ObjectProperties.GetObjectPropertyDecimal         (obj, timestamp, ObjectField.Round);
			var residual  = ObjectProperties.GetObjectPropertyDecimal         (obj, timestamp, ObjectField.ResidualValue);
			var mainValue = ObjectProperties.GetObjectPropertyAmortizedAmount (obj, timestamp, ObjectField.MainValue);

			if (!exp.IsEmpty && taux.HasValue && years.HasValue && period.HasValue &&
				mainValue.HasValue && mainValue.Value.FinalAmount.HasValue)
			{
				var methodObj = this.accessor.GetObject (BaseType.Methods, exp);

				if (methodObj != null)
				{
					var method = (AmortizationMethod) ObjectProperties.GetObjectPropertyInt (methodObj, null, ObjectField.AmortizationMethod);
					var expression = ObjectProperties.GetObjectPropertyString (methodObj, null, ObjectField.Expression);

					return new AmortizationDefinition (method, expression, taux.GetValueOrDefault (0.0m),
						years.GetValueOrDefault (0.0m),
						(Periodicity) period, (ProrataType) prorata,
						round.GetValueOrDefault (0.0m), residual.GetValueOrDefault (0.0m),
						mainValue.Value.FinalAmount.GetValueOrDefault (0.0m));
				}
			}

			return AmortizationDefinition.Empty;
		}

		private void CreateAmortizationPreview(DataObject obj, System.DateTime date, AmortizationDetails details)
		{
			//	Crée l'événement d'aperçu d'amortissement.
			var e = this.accessor.CreateAssetEvent (obj, date, EventType.AmortizationPreview);

			if (e != null)
			{
				var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
				System.Diagnostics.Debug.Assert (p != null);

				var finalAmount = Amortizations.ComputeAmortization (this.accessor, details).Value;

				var aa = AmortizedAmount.SetAmounts (p.Value, details.History.InitialAmount, finalAmount);
				Amortizations.SetAmortizedAmount (e, aa);
			}
		}

		public static AbstractCalculator.Result ComputeAmortization(DataAccessor accessor, AmortizationDetails details)
		{
			//	Calcule un amortissement, soit à partir d'une méthode 'built-in',
			//	soit à partir d'une expression.
			AbstractCalculator calculator = null;

			switch (details.Def.Method)
			{
				case AmortizationMethod.RateLinear:
					calculator = new RateLinearCalculator (details);
					break;

				case AmortizationMethod.RateDegressive:
					calculator = new RateDegressiveCalculator (details);
					break;

				case AmortizationMethod.YearsLinear:
					calculator = new YearsLinearCalculator (details);
					break;

				case AmortizationMethod.YearsDegressive:
					calculator = new YearsDegressiveCalculator (details);
					break;
			}

			if (calculator != null)  // à partir d'une méthode 'built-in' ?
			{
				var value = calculator.Evaluate ();
				return new AbstractCalculator.Result (value, null);
			}

			if (!string.IsNullOrEmpty (details.Def.Expression))  // à partir d'une expression ?
			{
				return accessor.AmortizationExpressions.Evaluate (details);
			}

			return AbstractCalculator.Result.Empty;  // impossible de calculer quoi que ce soit
		}

#if false
		public static AmortizedAmount InitialiseAmortizedAmount(DataAccessor accessor,
			DataObject obj, DataEvent e, Timestamp timestamp,
			bool fixAmount, EntryScenario entryScenario)
		{
			//	Initialise une fois pour toutes le AmortizedAmount d'un objet.
			//	Collecte tous les champs qui définissent comment générer l'écriture.
			//	Ils peuvent provenir de plusieurs événements différents.
			var inputDate = AssetCalculator.GetFirstTimestamp (obj).Value.Date;
			var defTimestamp = timestamp;

			//	On doit parcourir toutes les périodes, pour trouver la définition
			//	initiatrice de la période.
			var period = Amortizations.GetPeriods (new DateRange (inputDate, inputDate.AddYears (100)), obj)
				.Where (x => defTimestamp.Date >= x.IncludeFrom && defTimestamp.Date < x.ExcludeTo)
				.FirstOrDefault ();

			if (period.AtLeastOneTime)
			{
				defTimestamp = new Timestamp (period.IncludeFrom, 0);
			}

			var def = Amortizations.GetAmortizationDefinition (obj, defTimestamp);

			var beginDate = def.GetBeginRangeDate (defTimestamp.Date);
			var endDate = beginDate.AddMonths (def.PeriodMonthCount);
			var range = new DateRange (beginDate, endDate);

			var valueDate = range.IncludeFrom;

			if (range.IsInside (inputDate))
			{
				//	Si l'objet est entré durant la période, on utilise sa date d'entrée
				//	pour calculer l'amortissement "au prorata".
				valueDate = inputDate;
			}

			AmortizationMethod method;
			string exp;

			if (fixAmount)
			{
				method = AmortizationMethod.None;
				exp = null;
			}
			else
			{
				method = MethodsLogic.GetMethod (accessor, def.ExpressionGuid);
				exp = MethodsLogic.GetExpression (accessor, def.ExpressionGuid);
			}

			var prorata = ProrataDetails.ComputeProrata (range, valueDate, def.ProrataType);

			return new AmortizedAmount
			(
				method, exp, def.Rate,
				def.YearRank, def.YearCount,
				def.PeriodRank, def.Periodicity,
				null, null, null, null, null,
				prorata.Numerator, prorata.Denominator, def.Round, def.Residual,
				entryScenario, timestamp.Date,
				obj.Guid, e.Guid, Guid.Empty, 0
			);
		}
#endif


		private static bool HasAmortizations(DataObject obj, EventType type, DateRange range)
		{
			//	Indique s'il existe un ou plusieurs amortissements dans un intervalle de dates.
			return obj.Events
				.Where (x => x.Type == type && range.IsInside (x.Timestamp.Date))
				.Any ();
		}

		private void RemoveEvents(DataObject obj, EventType type, DateRange range)
		{
			//	Supprime tous les événements d'un objet d'un type donné compris dans
			//	un intervalle de dates.
			if (obj != null)
			{
				var guids = obj.Events
					.Where (x => x.Type == type && range.IsInside (x.Timestamp.Date))
					.Select (x => x.Guid)
					.ToArray ();

				foreach (var guid in guids)
				{
					var e = obj.GetEvent (guid);
					this.accessor.RemoveObjectEvent (obj, e);
				}
			}
		}

		private void FixEvents(DataObject obj, DateRange range)
		{
			//	Transforme tous les événements d'un objet compris dans un intervalle de dates,
			//	de AmortizationPreview en AmortizationAuto.
			if (obj != null)
			{
				var guids = obj.Events
					.Where (x => x.Type == EventType.AmortizationPreview && range.IsInside (x.Timestamp.Date))
					.Select (x => x.Guid)
					.ToArray ();

				foreach (var guid in guids)
				{
					var currentEvent = obj.GetEvent (guid);
					obj.RemoveEvent (currentEvent);

					var newEvent = new DataEvent (this.accessor.UndoManager, currentEvent.Guid, currentEvent.Timestamp, EventType.AmortizationAuto);
					newEvent.SetProperties (currentEvent);
					obj.AddEvent (newEvent);
				}
			}
		}


		#region Update amounts
		public static void UpdateAmounts(DataAccessor accessor, DataObject obj)
		{
			//	Répercute les valeurs des montants selon la chronologie des événements.
			//	Les montants des écritures sont également mis à jour.
			if (obj != null)
			{
				foreach (var field in accessor.AssetValueFields)
				{
					decimal? lastAmount = null;

					foreach (var e in obj.Events)
					{
						if (field == ObjectField.MainValue)
						{
							Amortizations.CreateEntry (accessor, obj, e);
						}
						else
						{
							Amortizations.UpdateComputedAmount (e, field, ref lastAmount);
						}
					}
				}
			}
		}

		private static void CreateEntry(DataAccessor accessor, DataObject asset, DataEvent e)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

			if (p != null)
			{
				var aa = p.Value;

				aa = Entries.CreateEntry (accessor, asset, e, aa);  // génère ou met à jour les écritures
				Amortizations.SetAmortizedAmount (e, aa);
			}
		}

		private static void UpdateComputedAmount(DataEvent e, ObjectField field, ref decimal? lastAmount)
		{
			var current = Amortizations.GetComputedAmount (e, field);

			if (current.HasValue)
			{
				current = new ComputedAmount (lastAmount, current.Value);
				lastAmount = current.Value.FinalAmount;
				Amortizations.SetComputedAmount (e, field, current);
			}
		}

		private static ComputedAmount? GetComputedAmount(DataEvent e, ObjectField field)
		{
			var p = e.GetProperty (field) as DataComputedAmountProperty;
			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		private static void SetComputedAmount(DataEvent e, ObjectField field, ComputedAmount? value)
		{
			if (value.HasValue)
			{
				var newProperty = new DataComputedAmountProperty (field, value.Value);
				e.AddProperty (newProperty);
			}
			else
			{
				e.RemoveProperty (field);
			}
		}

		public static void SetAmortizedAmount(DataEvent e, AmortizedAmount? value)
		{
			if (value.HasValue)
			{
				var newProperty = new DataAmortizedAmountProperty (ObjectField.MainValue, value.Value);
				e.AddProperty (newProperty);
			}
			else
			{
				e.RemoveProperty (ObjectField.MainValue);
			}
		}
		#endregion


		public static bool IsHidden(AmortizationMethod method, ObjectField field)
		{
			//	Indique si un champ n'a pas de sens pour une méthode d'amortissement donnée.
			//	Par défaut, un champ est visible. On teste spécifiquement les champs à cacher.
			//	Ainsi, les champs ajoutés lors de développments futurs seront visibles par défaut.
			//	false -> champ utile à montrer normalement
			//	true  -> champ inutile à cacher
			switch (method)
			{
				case AmortizationMethod.RateLinear:
				case AmortizationMethod.RateDegressive:
					//	Si l'amortissement est calculé selon le taux, le nombre d'années
					//	ne sert à rien.
					return field == ObjectField.AmortizationYearCount;

				case AmortizationMethod.YearsLinear:
				case AmortizationMethod.YearsDegressive:
					//	Si l'amortissement est calculé selon le nombre d'années, le taux
					//	ne sert à rien.
					return field == ObjectField.AmortizationRate
						|| field == ObjectField.Prorata;

				case AmortizationMethod.Custom:
					return false;

				default:
					//	S'il n'y a pas d'amortissement généré automatiquement, tous les
					//	champs suivants ne servent à rien.
					return field == ObjectField.AmortizationYearCount
						|| field == ObjectField.AmortizationRate
						|| field == ObjectField.Periodicity
						|| field == ObjectField.Prorata
						|| field == ObjectField.Round
						|| field == ObjectField.ResidualValue;
			}
		}

	
		private readonly DataAccessor			accessor;
	}
}
