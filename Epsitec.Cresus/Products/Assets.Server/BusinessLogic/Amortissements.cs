//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class Amortissements
	{
		public Amortissements(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public List<Error> GeneratesAmortissementsAuto(DateRange processRange)
		{
			//	Génère les amortissements automatiques pour tous les objets.
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.GeneratesAmortissementsAuto (processRange, node.Guid));
			}

			return errors;
		}

		public List<Error> GeneratesAmortissementsAuto(DateRange processRange, Guid objectGuid)
		{
			//	Génère les amortissements automatiques pour un objet donné.
			//	TODO: Le changement de DataAmortissement pendant l'intervalle n'est pas géré !
			//	TODO: Calculer les amortissements partiels au prorata de la durée.
			//	TODO: Implémenter le mode linéaire.
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			int counterDone = 0;

			var da = this.GetDataAmortissement (obj, processRange.FromTimestamp);
			if (da.IsEmpty)
			{
				da = this.GetDataAmortissement (obj, processRange.ToTimestamp);
			}
			if (da.IsEmpty)
			{
				var error = new Error (ErrorType.AmortissementUndefined, objectGuid);
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
				if (Amortissements.HasAmortissements (obj, range))
				{
					var error = new Error (ErrorType.AmortissementAlreadyDone, objectGuid);
					errors.Add (error);
				}
				else
				{
					var et = ObjectCalculator.GetPlausibleEventTypes (obj, range.ToTimestamp);
					if (!et.Contains (EventType.AmortissementExtra))
					{
						var error = new Error (ErrorType.AmortissementOutObject, objectGuid);
						errors.Add (error);
					}
					else
					{
						var ca = ObjectCalculator.GetObjectPropertyComputedAmount (obj, range.ToTimestamp, ObjectField.ValeurComptable);
						if (!ca.HasValue || !ca.Value.FinalAmount.HasValue)
						{
							var error = new Error (ErrorType.AmortissementEmptyAmount, objectGuid);
							errors.Add (error);
						}
						else
						{
							//	Calcule un amortissement dégressif.
							var currentValue = ca.Value.FinalAmount.Value;
							var newValue = currentValue - (currentValue * da.EffectiveRate);

							this.CreateAmortissementAuto (obj, range.IncludeTo, currentValue, newValue);
							counterDone++;
						}
					}
				}
			}

			var generate = new Error (ErrorType.AmortissementGenerate, objectGuid, counterDone);
			errors.Add (generate);
			return errors;
		}


		public List<Error> RemovesAmortissementsAuto(DateRange processRange)
		{
			//	Supprime les amortissements automatiques pour tous les objets.
			var errors = new List<Error> ();
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				errors.AddRange (this.RemovesAmortissementsAuto (processRange, node.Guid));
			}

			if (errors.Count == 0)
			{
				errors.Add (new Error (ErrorType.AmortissementRemove, Guid.Empty, 0));
			}

			return errors;
		}

		public List<Error> RemovesAmortissementsAuto(DateRange processRange, Guid objectGuid)
		{
			//	Supprime les amortissements automatiques pour un objet donné.
			var errors = new List<Error> ();

			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			int counterDone = Amortissements.RemovesAmortissementsAuto (obj, processRange);

			if (counterDone > 0)
			{
				var error = new Error (ErrorType.AmortissementRemove, objectGuid, counterDone);
				errors.Add (error);
			}

			if (errors.Count == 0)
			{
				errors.Add (new Error (ErrorType.AmortissementRemove, objectGuid, 0));
			}

			return errors;
		}


		private DataAmortissement GetDataAmortissement(DataObject obj, Timestamp timestamp)
		{
			var taux   = ObjectCalculator.GetObjectPropertyDecimal (obj, timestamp, ObjectField.TauxAmortissement);
			var type   = ObjectCalculator.GetObjectPropertyString  (obj, timestamp, ObjectField.TypeAmortissement);
			var period = ObjectCalculator.GetObjectPropertyString  (obj, timestamp, ObjectField.Périodicité);
			var rest   = ObjectCalculator.GetObjectPropertyDecimal (obj, timestamp, ObjectField.ValeurRésiduelle);

			var t = Amortissements.ParseTypeAmortissement (type);
			var p = Amortissements.ParsePeriod (period);

			return new DataAmortissement (taux.GetValueOrDefault (0.0m), t, p, rest.GetValueOrDefault (0.0m));

		}

		private static TypeAmortissement ParseTypeAmortissement(string text)
		{
			//	TODO: provisoire
			if (!string.IsNullOrEmpty (text))
			{
				text = text.ToLower ();

				if (text.StartsWith ("lin"))  // linéaire ?
				{
					return TypeAmortissement.Linear;
				}
				else if (text.StartsWith ("dég") || text.StartsWith ("deg"))  // dégressif ?
				{
					return TypeAmortissement.Linear;
				}
			}

			return TypeAmortissement.Unknown;
		}

		private static int ParsePeriod(string text)
		{
			//	TODO: provisoire
			if (!string.IsNullOrEmpty (text))
			{
				text = text.ToLower ();

				if (text.StartsWith ("an"))  // annuel ?
				{
					return 12;
				}
				else if (text.StartsWith ("sem"))  // semestriel ?
				{
					return 6;
				}
				else if (text.StartsWith ("tri"))  // trimestriel ?
				{
					return 3;
				}
				else if (text.StartsWith ("men"))  // mensuel ?
				{
					return 1;
				}
			}

			return 0;
		}


		private void CreateAmortissementAuto(DataObject obj, System.DateTime date, decimal currentValue, decimal newValue)
		{
			var e = this.accessor.CreateObjectEvent (obj, date, EventType.AmortissementAuto);

			if (e != null)
			{
				var v = new ComputedAmount (currentValue, newValue, true);
				var p = new DataComputedAmountProperty (ObjectField.ValeurComptable, v);
				e.AddProperty (p);
			}
		}


		private static bool HasAmortissements(DataObject obj, DateRange range)
		{
			//	Indique s'il existe un ou plusieurs amortissements (automatique ou manuel)
			//	dans un intervalle de dates.
			if (obj != null)
			{
				return obj.Events
					.Where (x =>
						(x.Type == EventType.AmortissementAuto || x.Type == EventType.AmortissementExtra)
						&& range.IsInside (x.Timestamp.Date))
					.Any ();

			}

			return false;
		}

		private static int RemovesAmortissementsAuto(DataObject obj, DateRange range)
		{
			//	Supprime tous les événements d'amortissement automatique d'un objet
			//	compris dans un intervalle de dates.
			int count = 0;

			if (obj != null)
			{
				var guids = obj.Events
					.Where (x => x.Type == EventType.AmortissementAuto && range.IsInside (x.Timestamp.Date))
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


		private readonly DataAccessor			accessor;
	}
}
