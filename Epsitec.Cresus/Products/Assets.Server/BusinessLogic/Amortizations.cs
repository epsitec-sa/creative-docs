//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class Amortizations
	{
		public Amortizations(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.entries = new Entries (this.accessor);
		}


		public List<Error> Preview(DateRange processRange)
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.GetNodes ())
			{
				errors.AddRange (this.Create (processRange, node.Guid));
			}

			return errors;
		}

		public List<Error> Fix(System.DateTime endDate)
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.GetNodes ())
			{
				errors.AddRange (this.Fix (endDate, node.Guid));
			}

			return errors;
		}

		public List<Error> Unpreview()
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.GetNodes ())
			{
				errors.AddRange (this.Unpreview (node.Guid));
			}

			return errors;
		}

		public List<Error> Delete(System.DateTime startDate)
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodeGetter (BaseType.Assets);

			foreach (var node in getter.GetNodes ())
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

			this.accessor.WarningsDirty = true;

			return errors;
		}

		public List<Error> Fix(System.DateTime endDate, Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int count = this.FixEvents (obj, new DateRange (System.DateTime.MinValue, endDate));

			this.accessor.WarningsDirty = true;

			return errors;
		}

		public List<Error> Unpreview(Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int count = this.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			this.accessor.WarningsDirty = true;

			return errors;
		}

		public List<Error> Delete(System.DateTime startDate, Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.RemoveEvents (obj, EventType.AmortizationPreview, new DateRange (startDate, System.DateTime.MaxValue));
			this.RemoveEvents (obj, EventType.AmortizationAuto,    new DateRange (startDate, System.DateTime.MaxValue));

			this.accessor.WarningsDirty = true;

			return errors;
		}


		private void GeneratesAmortizationsPreview(List<Error> errors, DateRange processRange, Guid objectGuid)
		{
			//	Génère les aperçus d'amortissement pour un objet donné.
			var obj = this.accessor.GetObject (BaseType.Assets, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			//	Supprime tous les aperçus d'amortissement.
			this.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			//	Passe en revue les périodes.
			System.DateTime? lastToDate = null;

			foreach (var period in Amortizations.GetPeriods (processRange, obj))
			{
				var ad = Amortizations.GetAmortizationDetails (obj, period.IncludeFrom, lastToDate);
				if (!ad.IsEmpty)
				{
					//	On crée un aperçu de l'amortissement au 31.12.
					this.CreateAmortizationPreview (obj, period.ExcludeTo.AddDays (-1), ad);

					lastToDate = period.ExcludeTo;
				}
			}

			//	Supprime tous les événements d'amortissement ordinaire après la date de fin.
			this.RemoveEvents (obj, EventType.AmortizationAuto, new DateRange (processRange.ExcludeTo, System.DateTime.MaxValue));

			//	Pour mettre à jour les éventuels amortissements extraordinaires suivants.
			Amortizations.UpdateAmounts (this.accessor, obj);
		}

		private static IEnumerable<DateRange> GetPeriods(DateRange processRange, DataObject obj)
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
						var def = Amortizations.GetAmortizationDefinition (obj, new Timestamp (beginDate, 0));
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

		private static AmortizationDetails GetAmortizationDetails(DataObject obj, System.DateTime date, System.DateTime? lastToDate)
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

			if (AssetCalculator.IsOutOfBoundsEvent (obj, new Timestamp (range.ExcludeTo.AddSeconds (-1), 0)))
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

			if (lastToDate.HasValue && valueDate < lastToDate.Value)
			{
				//	Si une partie de la période a déjà été amortie (par exemple suite à un
				//	changement de périodicité), on utilise la dernière date amortie pour
				//	calculer l'amortissement "au prorata".
				valueDate = lastToDate.Value;
			}

			var pd = ProrataDetails.ComputeProrata (range, valueDate, def.ProrataType);
			return new AmortizationDetails (def, pd);
		}


		private static AmortizationDefinition GetAmortizationDefinition(DataObject obj, Timestamp timestamp)
		{
			//	Collecte tous les champs qui définissent comment amortir. Ils peuvent provenir
			//	de plusieurs événements différents.
			var method   = ObjectProperties.GetObjectPropertyInt     (obj, timestamp, ObjectField.AmortizationMethod);
			var taux     = ObjectProperties.GetObjectPropertyDecimal (obj, timestamp, ObjectField.AmortizationRate);
			var type     = ObjectProperties.GetObjectPropertyInt     (obj, timestamp, ObjectField.AmortizationType);
			var years    = ObjectProperties.GetObjectPropertyDecimal (obj, timestamp, ObjectField.AmortizationYearCount);
			var period   = ObjectProperties.GetObjectPropertyInt     (obj, timestamp, ObjectField.Periodicity);
			var prorata  = ObjectProperties.GetObjectPropertyInt     (obj, timestamp, ObjectField.Prorata);
			var round    = ObjectProperties.GetObjectPropertyDecimal (obj, timestamp, ObjectField.Round);
			var residual = ObjectProperties.GetObjectPropertyDecimal (obj, timestamp, ObjectField.ResidualValue);

			if (method.HasValue && taux.HasValue && type.HasValue && years.HasValue && period.HasValue)
			{
				var m = (AmortizationMethod) method;
				var t = (AmortizationType) type;
				var p = (Periodicity) period;
				var r = (ProrataType) prorata;

				//	Calcule le rang de l'amortissement, égal au nombre d'amortissements antérieurs.
				var rank = obj.Events
					.Where (x => x.Timestamp < timestamp && (
						x.Type == EventType.AmortizationPreview ||
						x.Type == EventType.AmortizationAuto ||
						x.Type == EventType.AmortizationExtra))
					.Count ();

				return new AmortizationDefinition (m, taux.GetValueOrDefault (0.0m), t, rank, years.Value, p, r, round.GetValueOrDefault (0.0m), residual.GetValueOrDefault (0.0m));
			}
			else
			{
				return AmortizationDefinition.Empty;
			}
		}

		private void CreateAmortizationPreview(DataObject obj, System.DateTime date, AmortizationDetails details)
		{
			//	Crée l'événement d'aperçu d'amortissement.
			if (details.Def.None)
			{
				//	Si le taux est nul, on ne génère pas d'amortissement.
				return;
			}

			var e = this.accessor.CreateAssetEvent (obj, date, EventType.AmortizationPreview);

			if (e != null)
			{
				var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
				System.Diagnostics.Debug.Assert (p != null);

				//	InitialAmount et BaseAmount seront calculés plus tard.
				var aa = AmortizedAmount.SetPreview
				(
					p.Value,
					details.Def.Rate,
					details.Def.Type,
					details.Prorata.Numerator,
					details.Prorata.Denominator,
					details.Def.Periodicity,
					details.Def.Round,
					details.Def.Residual,
					EntryScenario.AmortizationAuto
				);

				Amortizations.SetAmortizedAmount (e, aa);
			}
		}

		public static AmortizedAmount InitialiseAmortizedAmount(DataObject obj, DataEvent e, Timestamp timestamp,
			AmortizationType amortizationType, EntryScenario entryScenario)
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

			var prorata = ProrataDetails.ComputeProrata (range, valueDate, def.ProrataType);

			return new AmortizedAmount
			(
				def.Method, def.Rate, amortizationType, def.YearRank, def.YearCount, def.Periodicity,
				null, null, null,
				prorata.Numerator, prorata.Denominator, def.Round, def.Residual, entryScenario, timestamp.Date,
				obj.Guid, e.Guid, Guid.Empty, 0
			);
		}


		private static bool HasAmortizations(DataObject obj, EventType type, DateRange range)
		{
			//	Indique s'il existe un ou plusieurs amortissements dans un intervalle de dates.
			return obj.Events
				.Where (x => x.Type == type && range.IsInside (x.Timestamp.Date))
				.Any ();
		}

		private int RemoveEvents(DataObject obj, EventType type, DateRange range)
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
					this.accessor.RemoveObjectEvent (obj, e);
					count++;
				}
			}

			return count;
		}

		private int FixEvents(DataObject obj, DateRange range)
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

					var newEvent = new DataEvent (this.accessor.UndoManager, currentEvent.Guid, currentEvent.Timestamp, EventType.AmortizationAuto);
					newEvent.SetProperties (currentEvent);
					obj.AddEvent (newEvent);

					count++;
				}
			}

			return count;
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
					decimal? lastBase   = null;

					foreach (var e in obj.Events)
					{
						if (field == ObjectField.MainValue)
						{
							Amortizations.UpdateAmortizedAmount (accessor, e, ref lastAmount, ref lastBase);
						}
						else
						{
							Amortizations.UpdateComputedAmount (e, field, ref lastAmount);
						}
					}
				}
			}
		}

		private static void UpdateAmortizedAmount(DataAccessor accessor, DataEvent e, ref decimal? lastAmount, ref decimal? lastBase)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;

			if (p != null)
			{
				var aa = p.Value;

				if (aa.AmortizationType == AmortizationType.Unknown)  // montant fixe ?
				{
					aa = AmortizedAmount.SetPreviousAmount (aa, lastAmount);
					Amortizations.SetAmortizedAmount (e, aa);

					lastAmount = aa.FinalAmortizedAmount;
					lastBase   = aa.FinalAmortizedAmount;
				}
				else  // amortissement ?
				{
					aa = AmortizedAmount.SetAmortizedAmount (aa, lastAmount, lastBase);
					Amortizations.SetAmortizedAmount (e, aa);

					lastAmount = aa.FinalAmortizedAmount;
				}

				aa = Entries.CreateEntry (accessor, aa);  // génère ou met à jour les écritures
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
			//	Par défaut, un champ est utile/visible. On teste spécifiquement les champs
			//	inutiles à cacher.
			switch (method)
			{
				case AmortizationMethod.Rate:
					//	Si l'amortissement est calculé selon le taux, le nombre d'années
					//	ne sert à rien.
					return field == ObjectField.AmortizationYearCount;

				case AmortizationMethod.YearCount:
					//	Si l'amortissement est calculé selon le nombre d'années, le taux
					//	ne sert à rien.
					return field == ObjectField.AmortizationRate
						|| field == ObjectField.Prorata;

				default:
					//	S'il n'y a pas d'amortissement généré automatiquement, tous les
					//	champs suivants ne servent à rien.
					return field == ObjectField.AmortizationYearCount
						|| field == ObjectField.AmortizationRate
						|| field == ObjectField.AmortizationType
						|| field == ObjectField.Periodicity
						|| field == ObjectField.Prorata
						|| field == ObjectField.Round
						|| field == ObjectField.ResidualValue;
			}
		}

	
		private readonly DataAccessor			accessor;
		private readonly Entries				entries;
	}
}
