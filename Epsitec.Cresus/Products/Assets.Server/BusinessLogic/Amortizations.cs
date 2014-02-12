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
				var amortizationData = this.GetAmortizationData (obj, new Timestamp (beginDate, 0));
				if (amortizationData.IsEmpty)
				{
					var error = new Error (ErrorType.AmortizationUndefined, objectGuid);
					errors.Add (error);
					return;
				}

				var endDate = beginDate.AddMonths (amortizationData.PeriodMonthCount);
				var range = new DateRange (beginDate, endDate.AddDays (-1));

				this.GeneratesAmortizationPreview (errors, obj, amortizationData, range, ref counterDone);

				beginDate = endDate;
			}

			var generate = new Error (ErrorType.AmortizationGenerate, objectGuid, counterDone);
			errors.Add (generate);
		}

		private void GeneratesAmortizationPreview(List<Error> errors, DataObject obj, AmortizationData amortizationData, DateRange range, ref int counterDone)
		{
			//	Génère l'aperçu d'amortissement d'un objet pour une période donnée.
			if (ObjectCalculator.IsEventLocked (obj, new Timestamp (range.IncludeTo, 0)))
			{
				var error = new Error (ErrorType.AmortizationOutObject, obj.Guid);
				errors.Add (error);
				return;
			}

			//	Supprime tous les événements d'amortissement ordinaire de la période.
			Amortizations.RemoveEvents (obj, EventType.AmortizationAuto, range);

			//	S'il y a déjà un (ou plusieurs) amortissement extraordinaire dans la période,
			//	on ne génère pas d'amortissement ordinaire.
			if (Amortizations.HasAmortizations (obj, EventType.AmortizationExtra, range))
			{
				var error = new Error (ErrorType.AmortizationAlreadyDone, obj.Guid);
				errors.Add (error);
				return;
			}

			//	Génère l'aperçu d'amortissement.
			if (amortizationData.Type == AmortizationType.Degressive)
			{
				var ca = ObjectCalculator.GetObjectPropertyComputedAmount (obj, range.ToTimestamp, ObjectField.MainValue);
				if (!ca.HasValue || !ca.Value.FinalAmount.HasValue)
				{
					var error = new Error (ErrorType.AmortizationEmptyAmount, obj.Guid);
					errors.Add (error);
				}
				else
				{
					//	Calcule un amortissement dégressif.
					var currentValue = ca.Value.FinalAmount.Value;
					var newValue = currentValue - (currentValue * amortizationData.EffectiveRate);

					this.CreateAmortizationPreview (obj, range.IncludeTo, currentValue, newValue);
					counterDone++;
				}
			}
			else
			{
			}

			counterDone++;
		}


		private AmortizationData GetAmortizationData(DataObject obj, Timestamp timestamp)
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
