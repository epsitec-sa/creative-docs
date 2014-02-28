//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class Amortizations
	{
		public Amortizations(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public List<Error> Preview(DateRange processRange)
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.Create (processRange, node.Guid));
			}

			return errors;
		}

		public List<Error> Fix()
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.Fix (node.Guid));
			}

			return errors;
		}

		public List<Error> Unpreview()
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.Unpreview (node.Guid));
			}

			return errors;
		}

		public List<Error> Delete(System.DateTime startDate)
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.Delete (startDate, node.Guid));
			}

			return errors;
		}


		public List<Error> Create(DateRange processRange, Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.GeneratesAmortizationsPreview (errors, processRange, objectGuid);

			return errors;
		}

		public List<Error> Fix(Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int count = Amortizations.FixEvents (obj, DateRange.Full);

			return errors;
		}

		public List<Error> Unpreview(Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int count = Amortizations.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			return errors;
		}

		public List<Error> Delete(System.DateTime startDate, Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int count = Amortizations.RemoveEvents (obj, EventType.AmortizationAuto, new DateRange (startDate, System.DateTime.MaxValue));

			return errors;
		}


		private void GeneratesAmortizationsPreview(List<Error> errors, DateRange processRange, Guid objectGuid)
		{
			//	Génère les aperçus d'amortissement pour un objet donné.
			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			//	Supprime tous les aperçus d'amortissement.
			Amortizations.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			//	Passe en revue les périodes.
			foreach (var period in this.GetPeriods (processRange, obj))
			{
				var ad = this.GetAmortizationDetails (obj, period.ExcludeTo.AddDays (-1));
				if (!ad.IsEmpty)
				{
					//	On crée un aperçu de l'amortissement au 31.12.
					this.CreateAmortizationPreview (obj, period.ExcludeTo.AddDays (-1), ad);
				}
			}

			//	Supprime tous les événements d'amortissement ordinaire après la date de fin.
			Amortizations.RemoveEvents (obj, EventType.AmortizationAuto, new DateRange (processRange.ExcludeTo, System.DateTime.MaxValue));

			//	Pour mettre à jour les éventuels amortissements extraordinaires suivants.
			AssetCalculator.UpdateAmounts (this.accessor, obj);
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

					while (beginDate < processRange.ExcludeTo)
					{
						var def = Amortizations.GetAmortizationDefinition (obj, new Timestamp (beginDate, 0));

						if (def.IsEmpty)
						{
							//	Si l'objet n'est pas entré au début de l'année, mais qu'il l'est plus
							//	tard dans la période choisie (processRange), on démarre un amortissement
							//	au début de l'année d'entrée, au prorata de la durée.
							def = Amortizations.GetAmortizationDefinition (obj, new Timestamp (inputDate, 0));

							if (def.IsEmpty)
							{
								beginDate = new System.DateTime (inputDate.Year, 1, 1);
							}
							else
							{
								beginDate = def.GetBeginRangeDate (beginDate);
							}
						}

						if (def.IsEmpty)
						{
							//	Si aucun amortissement n'est défini, on essaie de nouveaux amortissements
							//	à partir de l'année prochaine.
							beginDate = beginDate.AddYears (1);
						}
						else
						{
							var endDate = beginDate.AddMonths (def.PeriodMonthCount);
							yield return new DateRange (beginDate, endDate);

							beginDate = endDate;
						}
					}
				}
			}
		}

		private AmortizationDetails GetAmortizationDetails(DataObject obj, System.DateTime date)
		{
			//	Retourne tous les détails sur un amortissement ordinaire.
			var def = Amortizations.GetAmortizationDefinition (obj, new Timestamp (date, 0));
			if (def.IsEmpty)
			{
				return AmortizationDetails.Empty;
			}

			var beginDate = def.GetBeginRangeDate (date);
			var endDate = beginDate.AddMonths (def.PeriodMonthCount);
			var range = new DateRange (beginDate, endDate);

			if (AssetCalculator.IsEventLocked (obj, new Timestamp (range.ExcludeTo.AddSeconds (-1), 0)))
			{
				return AmortizationDetails.Empty;
			}

			//	S'il y a déjà un (ou plusieurs) amortissement (extra)ordinaire dans la période,
			//	on ne génère pas d'amortissement ordinaire.
			if (Amortizations.HasAmortizations (obj, EventType.AmortizationExtra, range) ||
				Amortizations.HasAmortizations (obj, EventType.AmortizationAuto, range))
			{
				return AmortizationDetails.Empty;
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

			var pd = ProrataDetails.ComputeProrata (range, valueDate, def.ProrataType);
			return new AmortizationDetails (def, pd);

#if false
			if (def.Type == AmortizationType.Degressive)  // amortissement dégressif ?
			{
				//	Si la période se termine le 01.01.2014 (exclu), on cherche une valeur
				//	au 30.12.2013 23h59, pour ne pas trouver l'amortissement lui-même.
				Timestamp? initialTimestamp;
				decimal? initialValue;
				Amortizations.GetAnyValue (obj, range.ExcludeTo.AddDays (-1).AddSeconds (-1), out initialTimestamp, out initialValue);

				if (initialValue.HasValue)
				{
					if (initialValue.Value > def.Residual)
					{
						//	Calcule un amortissement dégressif.
						var pd = ProrataDetails.ComputeProrata (range, initialTimestamp.Value.Date, def.ProrataType);
						return new AmortizationDetails (def, pd);
					}
				}

				return AmortizationDetails.Empty;
			}
			else  // amortissement linéaire ?
			{
				Timestamp? baseTimestamp;
				decimal? baseValue;
				Amortizations.GetBaseValue (obj, range.ExcludeTo.AddSeconds (-1), out baseTimestamp, out baseValue);

				if (baseValue.HasValue)
				{
					var pd = ProrataDetails.ComputeProrata (range, baseTimestamp.Value.Date, def.ProrataType);

					//	Cherche la dernière valeur amortie.
					//	Si la période se termine le 01.01.2014 (exclu), on cherche une valeur
					//	au 30.12.2013 23h59, pour ne pas trouver l'amortissement lui-même.
					decimal? amortizedValue;
					Amortizations.GetAmortizedValue (obj, range.ExcludeTo.AddDays (-1).AddSeconds (-1), baseTimestamp.Value, out amortizedValue);

					decimal initialValue;
					if (amortizedValue.HasValue)
					{
						initialValue = amortizedValue.Value;
					}
					else
					{
						initialValue = baseValue.Value;
					}

					//	Calcule un amortissement linéaire.
					if (initialValue > def.Residual)
					{
						return new AmortizationDetails (def, pd, initialValue, baseValue);
					}
				}

				return AmortizationDetails.Empty;
			}
#endif
		}


		private static AmortizationDefinition GetAmortizationDefinition(DataObject obj, Timestamp timestamp)
		{
			//	Collecte tous les champs qui définissent comment amortir. Ils peuvent provenir
			//	de plusieurs événements différents.
			var taux     = ObjectProperties.GetObjectPropertyDecimal (obj, timestamp, ObjectField.AmortizationRate);
			var type     = ObjectProperties.GetObjectPropertyInt     (obj, timestamp, ObjectField.AmortizationType);
			var period   = ObjectProperties.GetObjectPropertyInt     (obj, timestamp, ObjectField.Periodicity);
			var prorata  = ObjectProperties.GetObjectPropertyInt     (obj, timestamp, ObjectField.Prorata);
			var round    = ObjectProperties.GetObjectPropertyDecimal (obj, timestamp, ObjectField.Round);
			var residual = ObjectProperties.GetObjectPropertyDecimal (obj, timestamp, ObjectField.ResidualValue);

			if (taux.HasValue && type.HasValue && period.HasValue)
			{
				var t = (AmortizationType) type;
				var p = (Periodicity) period;
				var r = (ProrataType) prorata;

				return new AmortizationDefinition (taux.GetValueOrDefault (0.0m), t, p, r, round.GetValueOrDefault (0.0m), residual.GetValueOrDefault (0.0m));
			}
			else
			{
				return AmortizationDefinition.Empty;
			}
		}

		private void CreateAmortizationPreview(DataObject obj, System.DateTime date, AmortizationDetails details)
		{
			//	Crée l'événement d'aperçu d'amortissement.
			var e = this.accessor.CreateObjectEvent (obj, date, EventType.AmortizationPreview);

			if (e != null)
			{
				//	InitialAmount et BaseAmount seront calculés plus tard.
				var aa = new AmortizedAmount (details.Def.Type, null, null, 
					details.Def.EffectiveRate, details.Prorata.Numerator, details.Prorata.Denominator,
					details.Def.Round, details.Def.Residual);

				var p = new DataAmortizedAmountProperty (ObjectField.MainValue, aa);
				e.AddProperty (p);
			}
		}


		private static bool HasAmortizations(DataObject obj, EventType type, DateRange range)
		{
			//	Indique s'il existe un ou plusieurs amortissements dans un intervalle de dates.
			return obj.Events
				.Where (x => x.Type == type && range.IsInside (x.Timestamp.Date))
				.Any ();
		}

		private static int RemoveEvents(DataObject obj, EventType type, DateRange range)
		{
			//	Supprime tous les événements d'un objet d'un type donné compris dans
			//	un intervalle de dates.
			int count = 0;

			if (obj != null)
			{
				var guids = obj.Events
					.Where (x => x.Type == type && range.IsInside (x.Timestamp.Date))
					.Select (x => x.Guid)
					.ToArray ();

				foreach (var guid in guids)
				{
					var e = obj.GetEvent (guid);
					obj.RemoveEvent (e);
					count++;
				}
			}

			return count;
		}

		private static int FixEvents(DataObject obj, DateRange range)
		{
			//	Transforme tous les événements d'un objet compris dans un intervalle de dates,
			//	de AmortizationPreview en AmortizationAuto.
			int count = 0;

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

					var newEvent = new DataEvent (currentEvent.Timestamp, EventType.AmortizationAuto);
					newEvent.SetProperties (currentEvent);
					obj.AddEvent (newEvent);

					count++;
				}
			}

			return count;
		}


		private readonly DataAccessor			accessor;
	}
}
