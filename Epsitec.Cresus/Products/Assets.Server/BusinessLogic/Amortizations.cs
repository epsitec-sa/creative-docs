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
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.Create (processRange, node.Guid));
			}

			return errors;
		}

		public List<Error> Fix()
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.Fix (node.Guid));
			}

			return errors;
		}

		public List<Error> Unpreview()
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.Unpreview (node.Guid));
			}

			return errors;
		}

		public List<Error> Delete(System.DateTime startDate)
		{
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.Delete (startDate, node.Guid));
			}

			return errors;
		}


		public List<Error> Create(DateRange processRange, Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			this.GeneratesAmortizationsPreview (errors, processRange, objectGuid);

			return errors;
		}

		public List<Error> Fix(Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int count = Amortizations.FixEvents (obj, DateRange.Full);

			return errors;
		}

		public List<Error> Unpreview(Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int count = Amortizations.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			return errors;
		}

		public List<Error> Delete(System.DateTime startDate, Guid objectGuid)
		{
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int count = Amortizations.RemoveEvents (obj, EventType.AmortizationAuto, new DateRange (startDate, System.DateTime.MaxValue));

			return errors;
		}


		private void GeneratesAmortizationsPreview(List<Error> errors, DateRange processRange, Guid objectGuid)
		{
			//	Génère les aperçus d'amortissement pour un objet donné.
			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			//	Supprime tous les aperçus d'amortissement.
			Amortizations.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			//	Passe en revue les tranches de temps.
			var beginDate = new System.DateTime (processRange.IncludeFrom.Year, 1, 1);
			int counterDone = 0;

			while (beginDate < processRange.ExcludeTo)
			{
				var amortizationDefinition = Amortizations.GetAmortizationDefinition (obj, new Timestamp (beginDate, 0));
				if (amortizationDefinition.IsEmpty)
				{
					var error = new Error (ErrorType.AmortizationUndefined, objectGuid);
					errors.Add (error);

					//	Si aucun amortissement n'est défini, on essaie de nouveaux amortissements
					//	à partir de l'année prochaine.
					beginDate = beginDate.AddYears (1);
				}
				else
				{
					var endDate = beginDate.AddMonths (amortizationDefinition.PeriodMonthCount);
					var range = new DateRange (beginDate, endDate);

					bool ok = this.GeneratesAmortizationPreview (errors, obj, amortizationDefinition, range, ref counterDone);
					if (!ok)
					{
						break;
					}

					beginDate = endDate;
				}
			}

			//	Supprime tous les événements d'amortissement ordinaire après la date de fin.
			Amortizations.RemoveEvents (obj, EventType.AmortizationAuto, new DateRange (processRange.ExcludeTo, System.DateTime.MaxValue));

			var generate = new Error (ErrorType.AmortizationGenerate, objectGuid, counterDone);
			errors.Add (generate);
		}

		private bool GeneratesAmortizationPreview(List<Error> errors, DataObject obj, AmortizationDefinition amortizationDefinition, DateRange range, ref int counterDone)
		{
			//	Génère l'aperçu d'amortissement d'un objet pour une période donnée.
			//	Retourne false s'il faut stopper.
			if (ObjectCalculator.IsEventLocked (obj, new Timestamp (range.ExcludeTo.AddSeconds (-1), 0)))
			{
				var error = new Error (ErrorType.AmortizationOutObject, obj.Guid);
				errors.Add (error);
				return true;  // continue
			}

			//	S'il y a déjà un (ou plusieurs) amortissement (extra)ordinaire dans la période,
			//	on ne génère pas d'amortissement ordinaire.
			if (Amortizations.HasAmortizations (obj, EventType.AmortizationExtra, range) ||
				Amortizations.HasAmortizations (obj, EventType.AmortizationAuto,  range))
			{
				var error = new Error (ErrorType.AmortizationAlreadyDone, obj.Guid);
				errors.Add (error);
				return true;  // continue
			}

			//	Génère l'aperçu d'amortissement.
			if (amortizationDefinition.Type == AmortizationType.Degressive)  // amortissement dégressif ?
			{
				Timestamp? timestamp;
				decimal? initial;
				Amortizations.GetAnyValue (obj, range.ExcludeTo.AddSeconds (-1), out timestamp, out initial);

				if (!initial.HasValue)
				{
					var error = new Error (ErrorType.AmortizationEmptyAmount, obj.Guid);
					errors.Add (error);
				}
				else
				{
					//	Calcule un amortissement dégressif.
					var prorata = Amortizations.Prorata (timestamp.Value.Date, range, amortizationDefinition.ProrataType);
					var newValue = initial.Value - (initial.Value * amortizationDefinition.EffectiveRate * prorata);
					newValue = Amortizations.Round (newValue, amortizationDefinition.Round);

					if (newValue < amortizationDefinition.Residual)
					{
						//	On génère un dernier amortissement à la valeur résiduelle.
						this.CreateAmortizationPreview (obj, range.ExcludeTo.AddDays (-1), initial.Value, amortizationDefinition.Residual, false);
						counterDone++;

						var error = new Error (ErrorType.AmortizationResidualReached, obj.Guid);
						errors.Add (error);
						return false;  // stoppe
					}
					else
					{
						this.CreateAmortizationPreview (obj, range.ExcludeTo.AddDays (-1), initial.Value, newValue, true);
						counterDone++;
					}
				}
			}
			else  // amortissement linéaire ?
			{
				Timestamp? timestamp;
				decimal? initial;
				Amortizations.GetManualValue (obj, range.ExcludeTo.AddSeconds (-1), out timestamp, out initial);

				if (!initial.HasValue)
				{
					var error = new Error (ErrorType.AmortizationEmptyAmount, obj.Guid);
					errors.Add (error);
				}
				else
				{
					var prorata = Amortizations.Prorata (timestamp.Value.Date, range, amortizationDefinition.ProrataType);
					var delta = initial.Value * amortizationDefinition.EffectiveRate * prorata;

					decimal? amortization;
					Amortizations.GetAmortizationValue (obj, range.ExcludeTo.AddSeconds (-1), timestamp.Value, out amortization);

					if (amortization.HasValue)
					{
						initial = amortization;
					}

					var newValue = initial.Value - delta;
					newValue = Amortizations.Round (newValue, amortizationDefinition.Round);

					if (newValue < amortizationDefinition.Residual)
					{
						//	On génère un dernier amortissement à la valeur résiduelle.
						this.CreateAmortizationPreview (obj, range.ExcludeTo.AddDays (-1), initial.Value, amortizationDefinition.Residual, false);
						counterDone++;

						var error = new Error (ErrorType.AmortizationResidualReached, obj.Guid);
						errors.Add (error);
						return false;  // stoppe
					}
					else
					{
						this.CreateAmortizationPreview (obj, range.ExcludeTo.AddDays (-1), initial.Value, newValue, false);
						counterDone++;
					}
				}
			}

			return true;  // continue
		}


		private static AmortizationDefinition GetAmortizationDefinition(DataObject obj, Timestamp timestamp)
		{
			//	Collecte tous les champs qui définissent comment amortir. Ils peuvent provenir
			//	de plusieurs événements différents.
			//	TOTO: Il faudrait être capable de trouver une définition dans le futur, au cas où
			//	l'objet est entré après le début de l'année (prorata) !!!
			var taux     = ObjectCalculator.GetObjectPropertyDecimal (obj, timestamp, ObjectField.AmortizationRate);
			var type     = ObjectCalculator.GetObjectPropertyInt     (obj, timestamp, ObjectField.AmortizationType);
			var period   = ObjectCalculator.GetObjectPropertyInt     (obj, timestamp, ObjectField.Periodicity);
			var prorata  = ObjectCalculator.GetObjectPropertyInt     (obj, timestamp, ObjectField.Prorata);
			var round    = ObjectCalculator.GetObjectPropertyDecimal (obj, timestamp, ObjectField.Round);
			var residual = ObjectCalculator.GetObjectPropertyDecimal (obj, timestamp, ObjectField.ResidualValue);

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

		private void CreateAmortizationPreview(DataObject obj, System.DateTime date, decimal currentValue, decimal newValue, bool rate)
		{
			//	Crée l'événement d'aperçu d'amortissement.
			var e = this.accessor.CreateObjectEvent (obj, date, EventType.AmortizationPreview);

			if (e != null)
			{
				var v = new ComputedAmount (currentValue, newValue, rate);
				var p = new DataComputedAmountProperty (ObjectField.MainValue, v);
				e.AddProperty (p);

				//	Pour mettre à jour les éventuels amortissements extraordinaires suivants.
				//	Accésoireemnt, cela recalcule l'événement que l'on vient de créer, mais
				//	cela devrait être sans conséquence.
				ObjectCalculator.UpdateComputedAmounts (obj);
			}
		}


		private static void GetAnyValue(DataObject obj, System.DateTime date, out Timestamp? timestamp, out decimal? value)
		{
			//	En remontant dans le temps, retourne la première définition de la valeur
			//	d'un objet.
			for (int i=obj.EventsCount-1; i>=0; i--)
			{
				var e = obj.GetEvent (i);

				var ca = e.GetProperty (ObjectField.MainValue) as DataComputedAmountProperty;

				if (ca != null && e.Timestamp.Date < date)
				{
					timestamp = e.Timestamp;
					value     = ca.Value.FinalAmount;
					return;
				}
			}

			timestamp = null;
			value     = null;
			return;
		}

		private static void GetManualValue(DataObject obj, System.DateTime date, out Timestamp? timestamp, out decimal? value)
		{
			//	En remontant dans le temps, retourne la première définition de la valeur
			//	d'un objet qui ne soit pas un amortissement.
			for (int i=obj.EventsCount-1; i>=0; i--)
			{
				var e = obj.GetEvent (i);

				if (!Amortizations.IsAmortization (e.Type))
				{
					var ca = e.GetProperty (ObjectField.MainValue) as DataComputedAmountProperty;

					if (ca != null && e.Timestamp.Date < date)
					{
						timestamp = e.Timestamp;
						value     = ca.Value.FinalAmount;
						return;
					}
				}
			}

			timestamp = null;
			value     = null;
			return;
		}

		private static void GetAmortizationValue(DataObject obj, System.DateTime date, Timestamp limit, out decimal? value)
		{
			//	En remontant dans le temps (mais pas avant limit), retourne la première
			//	définition de la valeur d'un objet qui soit un amortissement.
			for (int i=obj.EventsCount-1; i>=0; i--)
			{
				var e = obj.GetEvent (i);

				if (e.Timestamp < limit)
				{
					break;
				}

				if (Amortizations.IsAmortization (e.Type))
				{
					var property = e.GetProperty (ObjectField.MainValue) as DataComputedAmountProperty;

					if (property != null && e.Timestamp.Date < date)
					{
						value = property.Value.FinalAmount;
						return;
					}
				}
			}

			value = null;
			return;
		}

		private static bool IsAmortization(EventType type)
		{
			return type == EventType.AmortizationAuto
				|| type == EventType.AmortizationExtra
				|| type == EventType.AmortizationPreview;
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

		private static decimal Prorata(System.DateTime dateValue, DateRange range, ProrataType prorata)
		{
			//	Retourne le facteur pour le prorata, compris entre 0.0 et 1.0.
			//	Pour une dateValue en début d'année, on retourne 1.0.
			//	Pour une dateValue en milieu d'année, on retourne 0.5.
			//	Pour une dateValue en fin d'année, on retourne 0.0.
			if (dateValue >= range.IncludeFrom)
			{
				switch (prorata)
				{
					case ProrataType.None:
						break;

					default:
						int total = range.ExcludeTo.Subtract (range.IncludeFrom).Days;
						int days = System.Math.Min (dateValue.Subtract (range.IncludeFrom).Days, total);
						return 1.0m - ((decimal) days / (decimal) total);
				}
			}

			return 1.0m;
		}

		private static decimal Round(decimal value, decimal round)
		{
			if (round > 0.0m)
			{
				if (value < 0)
				{
					value -= round/2;
				}
				else
				{
					value += round/2;
				}

				return value - (value % round);
			}
			else
			{
				return value;
			}
		}


		private readonly DataAccessor			accessor;
	}
}
