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

			while (beginDate <= processRange.IncludeTo)
			{
				var da = this.GetDataAmortization (obj, new Timestamp (beginDate, 0));
				if (da.IsEmpty)
				{
					var error = new Error (ErrorType.AmortizationUndefined, objectGuid);
					errors.Add (error);
					return;
				}

				var endDate = beginDate.AddMonths (da.PeriodMonthCount);
				var range = new DateRange (beginDate, endDate.AddDays (-1));

				this.GeneratesAmortizationsPreview (errors, obj, range, ref counterDone);

				beginDate = endDate;
			}

#if false
			var firstEvent = obj.GetEvent (0);
			if (firstEvent == null)
			{
				var error = new Error (ErrorType.AmortizationUndefined, objectGuid);
				errors.Add (error);
				return;
			}

			var da = this.GetDataAmortization (obj, firstEvent.Timestamp);
			if (da.IsEmpty)
			{
				var error = new Error (ErrorType.AmortizationUndefined, objectGuid);
				errors.Add (error);
				return;
			}

			//	Supprime tous les aperçus d'amortissement.
			Amortizations.RemoveEvents (obj, EventType.AmortizationPreview, DateRange.Full);

			var beginDate = new System.DateTime (firstEvent.Timestamp.Date.Year, 1, 1);
			var endDate   = beginDate.AddMonths (da.PeriodMonthCount);
			var range = new DateRange (beginDate, endDate.AddDays (-1));

			//?var lastDate = obj.Events.LastOrDefault ().Timestamp.Date;
			int counterDone = 0;

			while (range.IncludeFrom <= date)
			{
				this.GeneratesAmortizationsPreview (errors, obj, range, ref counterDone);

				//	Cherche si la période contient une nouvelle définition d'amortissement.
				var nda = this.GetDataAmortization (obj, new Timestamp (beginDate, 0));
				if (!nda.IsEmpty)
				{
					da = nda;
				}

				beginDate = endDate;
				endDate   = beginDate.AddMonths (da.PeriodMonthCount);
				range = new DateRange (beginDate, endDate.AddDays (-1));
			}
#endif

			var generate = new Error (ErrorType.AmortizationGenerate, objectGuid, counterDone);
			errors.Add (generate);
		}

		private void GeneratesAmortizationsPreview(List<Error> errors, DataObject obj, DateRange range, ref int counterDone)
		{
			//	Génère les aperçus d'amortissement d'un objet pour une période donnée.

			//	Supprime tous les événements d'amortissement ordinaire de la période.
			Amortizations.RemoveEvents (obj, EventType.AmortizationAuto, range);

			var rangeEvents = obj.Events.Where (x => range.IsInside (x.Timestamp.Date));
			//?if (!rangeEvents.Any ())
			//?{
			//?	return;
			//?}

			//	S'il y a déjà un (ou plusieurs) amortissement extraordinaire dans la période,
			//	on ne génère pas d'amortissement ordinaire.
			if (rangeEvents.Where (x => x.Type == EventType.AmortizationExtra).Any ())
			{
				return;
			}

			//	Génère l'amortissement ordinaire.
			this.CreateAmortizationPreview (obj, range.IncludeTo, 900, 800);  // TODO ???
			counterDone++;
		}




		public List<Error> GeneratesAmortizationsAuto(DateRange processRange)
		{
			//	Génère les amortissements automatiques pour tous les objets.
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.GeneratesAmortizationsAuto (processRange, node.Guid));
			}

			return errors;
		}

		public List<Error> GeneratesAmortizationsAuto(DateRange processRange, Guid objectGuid)
		{
			//	Génère les amortissements automatiques pour un objet donné.
			//	TODO: Le changement de AmortizationData pendant l'intervalle n'est pas géré !
			//	TODO: Calculer les amortissements partiels au prorata de la durée.
			//	TODO: Implémenter le mode linéaire.
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			int counterDone = 0;

			var da = this.GetDataAmortization (obj, processRange.FromTimestamp);
			if (da.IsEmpty)
			{
				da = this.GetDataAmortization (obj, processRange.ToTimestamp);
			}
			if (da.IsEmpty)
			{
				var error = new Error (ErrorType.AmortizationUndefined, objectGuid);
				errors.Add (error);
				return errors;
			}
			if (da.Error != ErrorType.Ok)
			{
				var error = new Error (da.Error, objectGuid);
				errors.Add (error);
				return errors;
			}

			var ranges = LogicRange.GetRanges (processRange, da.Period);
			foreach (var range in ranges)
			{
				if (Amortizations.HasAmortizations (obj, range))
				{
					var error = new Error (ErrorType.AmortizationAlreadyDone, objectGuid);
					errors.Add (error);
				}
				else
				{
					var et = ObjectCalculator.GetPlausibleEventTypes (obj, range.ToTimestamp);
					if (!et.Contains (EventType.AmortizationExtra))
					{
						var error = new Error (ErrorType.AmortizationOutObject, objectGuid);
						errors.Add (error);
					}
					else
					{
						var ca = ObjectCalculator.GetObjectPropertyComputedAmount (obj, range.ToTimestamp, ObjectField.MainValue);
						if (!ca.HasValue || !ca.Value.FinalAmount.HasValue)
						{
							var error = new Error (ErrorType.AmortizationEmptyAmount, objectGuid);
							errors.Add (error);
						}
						else
						{
							//	Calcule un amortissement dégressif.
							var currentValue = ca.Value.FinalAmount.Value;
							var newValue = currentValue - (currentValue * da.EffectiveRate);

							this.CreateAmortizationAuto (obj, range.IncludeTo, currentValue, newValue);
							counterDone++;
						}
					}
				}
			}

			var generate = new Error (ErrorType.AmortizationGenerate, objectGuid, counterDone);
			errors.Add (generate);
			return errors;
		}


		public List<Error> RemovesAmortizationsAuto(DateRange processRange)
		{
			//	Supprime les amortissements automatiques pour tous les objets.
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.RemovesAmortizationsAuto (processRange, node.Guid));
			}

			if (errors.Count == 0)
			{
				errors.Add (new Error (ErrorType.AmortizationRemove, Guid.Empty, 0));
			}

			return errors;
		}

		public List<Error> RemovesAmortizationsAuto(DateRange processRange, Guid objectGuid)
		{
			//	Supprime les amortissements automatiques pour un objet donné.
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int counterDone = Amortizations.RemovesAmortizationsAuto (obj, processRange);

			if (counterDone > 0)
			{
				var error = new Error (ErrorType.AmortizationRemove, objectGuid, counterDone);
				errors.Add (error);
			}

			if (errors.Count == 0)
			{
				errors.Add (new Error (ErrorType.AmortizationRemove, objectGuid, 0));
			}

			return errors;
		}


		private AmortizationData GetDataAmortization(DataObject obj, Timestamp timestamp)
		{
			var taux     = ObjectCalculator.GetObjectPropertyDecimal (obj, timestamp, ObjectField.AmortizationRate);
			var type     = ObjectCalculator.GetObjectPropertyInt     (obj, timestamp, ObjectField.AmortizationType);
			var period   = ObjectCalculator.GetObjectPropertyInt     (obj, timestamp, ObjectField.Periodicity);
			var residual = ObjectCalculator.GetObjectPropertyDecimal (obj, timestamp, ObjectField.ResidualValue);

			if (taux.HasValue && type.HasValue && period.HasValue)
			{
				var t = (AmortizationType) type;
				var p = (Periodicity) period;

				return new AmortizationData (taux.GetValueOrDefault (0.0m), t, p, residual.GetValueOrDefault (0.0m));
			}
			else
			{
				return AmortizationData.Empty;
			}
		}


		private void CreateAmortizationAuto(DataObject obj, System.DateTime date, decimal currentValue, decimal newValue)
		{
			var e = this.accessor.CreateObjectEvent (obj, date, EventType.AmortizationAuto);

			if (e != null)
			{
				var v = new ComputedAmount (currentValue, newValue, true);
				var p = new DataComputedAmountProperty (ObjectField.MainValue, v);
				e.AddProperty (p);
			}
		}

		private void CreateAmortizationPreview(DataObject obj, System.DateTime date, decimal currentValue, decimal newValue)
		{
			var e = this.accessor.CreateObjectEvent (obj, date, EventType.AmortizationPreview);

			if (e != null)
			{
				var v = new ComputedAmount (currentValue, newValue, true);
				var p = new DataComputedAmountProperty (ObjectField.MainValue, v);
				e.AddProperty (p);
			}
		}


		private static bool HasAmortizations(DataObject obj, DateRange range)
		{
			//	Indique s'il existe un ou plusieurs amortissements (automatique ou manuel)
			//	dans un intervalle de dates.
			if (obj != null)
			{
				return obj.Events
					.Where (x =>
						(x.Type == EventType.AmortizationAuto || x.Type == EventType.AmortizationExtra)
						&& range.IsInside (x.Timestamp.Date))
					.Any ();

			}

			return false;
		}

		private static int RemovesAmortizationsAuto(DataObject obj, DateRange range)
		{
			//	Supprime tous les événements d'amortissement automatique d'un objet
			//	compris dans un intervalle de dates.
			int count = 0;

			if (obj != null)
			{
				var guids = obj.Events
					.Where (x => x.Type == EventType.AmortizationAuto && range.IsInside (x.Timestamp.Date))
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
