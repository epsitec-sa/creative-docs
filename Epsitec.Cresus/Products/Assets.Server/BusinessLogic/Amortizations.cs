//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Data.Helpers;
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


		public List<AmortizationDetails> Preview(DateRange processRange, Guid objectGuid)
		{
			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			var list = this.GeneratesAmortizationsPreview (processRange, objectGuid);

			this.accessor.WarningsDirty = true;
			return list;
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


		private List<AmortizationDetails> GeneratesAmortizationsPreview(DateRange processRange, Guid objectGuid)
		{
			//	Génère les aperçus d'amortissement pour un objet donné.
			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			//	Supprime tous les aperçus d'amortissement.
			this.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			//	Passe en revue les périodes.
			System.DateTime? lastToDate = null;

			var list = new List<AmortizationDetails> ();

			foreach (var period in this.GetPeriods (processRange, obj))
			{
				var def = this.GetAmortizationDefinition (obj, new Timestamp (period.IncludeFrom, 0), lastToDate);

				var timestamp = obj.GetNewTimestamp (period.ExcludeTo.AddDays (-1));
				var history = Amortizations.GetHistoryDetails (this.accessor, obj, timestamp, def);

				if (!def.IsEmpty && !history.IsEmpty)
				{
					var details = new AmortizationDetails (def, history);

					this.CreateAmortizationPreview (obj, period.ExcludeTo.AddDays (-1), details);
					lastToDate = period.ExcludeTo;

					list.Add (details);
				}	
			}

			//	Supprime tous les événements d'amortissement ordinaire après la date de fin.
			this.RemoveEvents (obj, EventType.AmortizationAuto, new DateRange (processRange.ExcludeTo, System.DateTime.MaxValue));

			//	Pour mettre à jour les éventuels amortissements extraordinaires suivants.
			Amortizations.UpdateAmounts (this.accessor, obj);

			return list;
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

					System.DateTime? lastToDate = null;

					while (beginDate < processRange.ExcludeTo)
					{
						var def = this.GetAmortizationDefinition (obj, new Timestamp (beginDate, 0), lastToDate);
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
						lastToDate = endDate;
					}
				}
			}
		}

		private static System.DateTime? GetProrataDetails(DataObject obj, DateRange range, System.DateTime? lastToDate)
		{
			//	Retourne tous les détails sur un amortissement ordinaire.
			if (AssetCalculator.IsOutOfBoundsEvent (obj, new Timestamp (range.ExcludeTo.AddSeconds (-1), 0)))
			{
				return null;
			}

			//	S'il y a déjà un (ou plusieurs) amortissement (extra)ordinaire dans la période,
			//	on ne génère pas d'amortissement ordinaire.
			if (Amortizations.HasAmortizations (obj, EventType.AmortizationExtra, range) ||
				Amortizations.HasAmortizations (obj, EventType.AmortizationAuto,  range))
			{
				return null;
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

			return valueDate;
		}

		private static HistoryDetails GetHistoryDetails(DataAccessor accessor, DataObject obj, Timestamp timestamp, AmortizationDefinition def)
		{
			var      firstDate     = timestamp.Date;
			decimal? firstAmount   = null;
			var      lastDate      = timestamp.Date;
			var      baseDate      = timestamp.Date;
			decimal  baseAmount    = 0.0m;
			decimal  baseYearCount = AssetsLogic.GetFirstYearCount (accessor, obj, timestamp).GetValueOrDefault (10.0m);
			decimal  inputAmount   = 0.0m;

			foreach (var e in obj.Events.Where (x => x.Timestamp < timestamp))
			{
				var aa = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

				if (aa != null && aa.Value.FinalAmount.HasValue)
				{
					if (!firstAmount.HasValue)
					{
						firstDate   = e.Timestamp.Date;
						firstAmount = aa.Value.FinalAmount.Value;
						lastDate    = new System.DateTime (firstDate.Year, 1, 1).AddMonths ((int) (baseYearCount * 12.0m));
					}

					inputAmount = aa.Value.FinalAmount.Value;

					if (e.Type != EventType.AmortizationPreview &&
						e.Type != EventType.AmortizationAuto    )
					{
						if (e.Type == EventType.AmortizationExtra)
						{
							//	Un amortissement extraordinaire dans l'année est considéré
							//	comme une modification au début de l'année suivante.
							baseDate = Amortizations.GetFloorDate (e.Timestamp.Date, def.PeriodMonthCount).AddYears (1);
						}
						else
						{
							//	Une modification de valeur dans l'année est considérée
							//	comme une modification au début de l'année.
							baseDate = Amortizations.GetFloorDate (e.Timestamp.Date, def.PeriodMonthCount);
						}

						baseAmount = aa.Value.FinalAmount.Value;
						baseYearCount = (DateTime.Months (lastDate,baseDate)) / 12.0m;
					}
				}
			}

			return new HistoryDetails (firstDate, firstAmount.GetValueOrDefault (), baseDate, baseAmount, baseYearCount, inputAmount);
		}

		private static System.DateTime GetFloorDate(System.DateTime date, int monthCount)
		{
			//	Retourne une date arrondie au début d'une période annuelle, semestrielle,
			//	trimestrielle ou mensuelle.
			int mounths = (date.Year*12) + (date.Month-1);
			mounths = mounths - mounths%monthCount;

			return new System.DateTime (mounths/12, (mounths%12)+1, 1);
		}


		private AmortizationDefinition GetAmortizationDefinition(DataObject obj, Timestamp timestamp, System.DateTime? lastToDate)
		{
			//	Collecte tous les champs qui définissent comment amortir. Ils peuvent provenir
			//	de plusieurs événements différents.
			var exp    = ObjectProperties.GetObjectPropertyGuid (obj, timestamp, ObjectField.MethodGuid);
			var period = ObjectProperties.GetObjectPropertyInt  (obj, timestamp, ObjectField.Periodicity);

			if (!exp.IsEmpty && period.HasValue)
			{
				var methodObj = this.accessor.GetObject (BaseType.Methods, exp);

				if (methodObj != null)
				{
					var arguments = ArgumentsLogic.GetArgumentsDotNetCode (this.accessor, methodObj, obj, timestamp);
					var expression = ObjectProperties.GetObjectPropertyString (methodObj, null, ObjectField.Expression);
					var periodicity = (Periodicity) period;

					int pmc = AmortizedAmount.GetPeriodMonthCount (periodicity);

					var beginDate = AmortizationDefinition.GetBeginRangeDate (timestamp.Date, pmc);
					var endDate = beginDate.AddMonths (pmc);
					var range = new DateRange (beginDate, endDate);
					var date = Amortizations.GetProrataDetails (obj, range, lastToDate);

					if (date.HasValue)
					{
						return new AmortizationDefinition (range, date.Value, arguments, expression, periodicity);
					}
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

				var result = Amortizations.ComputeAmortization (this.accessor, details);

				var aa = AmortizedAmount.SetAmounts (p.Value, details.History.InputAmount, result.Value, result.Trace, result.Error);
				Amortizations.SetAmortizedAmount (e, aa);
			}
		}

		public static ExpressionResult ComputeAmortization(DataAccessor accessor, AmortizationDetails details)
		{
			//	Calcule un amortissement à partir d'une expression.
			if (!string.IsNullOrEmpty (details.Def.Expression))  // à partir d'une expression ?
			{
				return accessor.AmortizationExpressions.Evaluate (details);
			}

			return ExpressionResult.Empty;  // impossible de calculer quoi que ce soit
		}


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
					this.accessor.RemoveObjectEvent (obj, e, quick: true);
				}

				Amortizations.UpdateAmounts (this.accessor, obj);
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
							Amortizations.UpdateAmortizedAmount (accessor, obj, e, field, ref lastAmount);
						}
						else
						{
							Amortizations.UpdateComputedAmount (e, field, ref lastAmount);
						}
					}
				}
			}
		}

		private static void UpdateAmortizedAmount(DataAccessor accessor, DataObject asset, DataEvent e, ObjectField field, ref decimal? lastAmount)
		{
			var current = Amortizations.GetAmortizedAmount (e, field);

			if (current.HasValue)
			{
				if (lastAmount.HasValue)
				{
					current = AmortizedAmount.SetInitialAmount (current.Value, lastAmount);
					Amortizations.SetAmortizedAmount (e, current);
				}

				lastAmount = current.Value.FinalAmount;
			}

			if (!asset.Simulation)
			{
				Amortizations.CreateEntry (accessor, asset, e);
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

		private static AmortizedAmount? GetAmortizedAmount(DataEvent e, ObjectField field)
		{
			var p = e.GetProperty (field) as DataAmortizedAmountProperty;
			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
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


		public static AmortizationDetails GetDefaultDetails(string arguments, string expression)
		{
			//	Retourne les arguments par défaut, pour permettre de compiler une
			//	expression "à la main" (bouton "Compiler" dans la vue des méthodes),
			//	ou pour vérifier sa syntaxe (vue des Warnings).
			var startDate = new System.DateTime (2000, 1, 1);
			var endDate = startDate.AddYears (1);
			var range = new DateRange (startDate, endDate);

			var def = new AmortizationDefinition (range, startDate, arguments, expression, Periodicity.Annual);
			var history = new HistoryDetails (startDate, 100.0m, startDate, 10.0m, 100.0m, 100.0m);

			return new AmortizationDetails (def, history);
		}


		private readonly DataAccessor			accessor;
	}
}
