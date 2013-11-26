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


		public void GeneratesAmortissementsAuto(System.DateTime dateFrom, System.DateTime dateTo)
		{
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				this.GeneratesAmortissementsAuto (dateFrom, dateTo, node.Guid);
			}
		}

		public void GeneratesAmortissementsAuto(System.DateTime dateFrom, System.DateTime dateTo, Guid objectGuid)
		{
			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);

			//	S'il y a déjà un ou plusieurs amortissements, on ne fait rien.
			if (Amortissements.HasAmortissements (obj, dateFrom, dateTo))
			{
				return;
			}

			var end = dateTo.AddDays (1).AddTicks (-1);  // 31.12 -> 1er janvier moins un chouia
			var amortissement = this.GetAmortissement (obj, new Timestamp (end, int.MaxValue));
			if (!amortissement.IsValid)
			{
				return;
			}

			var start = new Timestamp (dateFrom, 0);
			var ca = ObjectCalculator.GetObjectPropertyComputedAmount (obj, start, ObjectField.Valeur1);
			if (!ca.HasValue || !ca.Value.FinalAmount.HasValue)
			{
				return;
			}

			var currentValue = ca.Value.FinalAmount.Value;
			var newValue = currentValue - (currentValue * amortissement.Rate);

			this.CreateAmortissementAuto (obj, dateTo, currentValue, newValue);
		}


		public void RemovesAmortissementsAuto(System.DateTime dateFrom, System.DateTime dateTo)
		{
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				this.RemovesAmortissementsAuto (dateFrom, dateTo, node.Guid);
			}
		}

		public void RemovesAmortissementsAuto(System.DateTime dateFrom, System.DateTime dateTo, Guid objectGuid)
		{
			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);
			System.Diagnostics.Debug.Assert (obj != null);

			Amortissements.RemovesAmortissementsAuto (obj, dateFrom, dateTo);
		}


		private DataAmortissement GetAmortissement(DataObject obj, Timestamp timestamp)
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
				var p = new DataComputedAmountProperty (ObjectField.Valeur1, v);
				e.AddProperty (p);
			}
		}


		private static bool HasAmortissements(DataObject obj, System.DateTime dateFrom, System.DateTime dateTo)
		{
			//	Indique s'il existe un ou plusieurs amortissements (automatique ou manuel)
			//	dans un intervale de dates.
			if (obj != null)
			{
				return obj.Events
					.Where (x => (x.Type == EventType.AmortissementAuto || x.Type == EventType.AmortissementExtra)
						&& x.Timestamp.Date >= dateFrom
						&& x.Timestamp.Date <= dateTo)
					.Any ();

			}

			return false;
		}

		private static void RemovesAmortissementsAuto(DataObject obj, System.DateTime dateFrom, System.DateTime dateTo)
		{
			//	Supprime tous les événements d'amortissement automatique d'un objet
			//	compris dans un intervale de dates.
			if (obj != null)
			{
				var guids = obj.Events
					.Where (x => x.Type == EventType.AmortissementAuto
						&& x.Timestamp.Date >= dateFrom
						&& x.Timestamp.Date <= dateTo)
					.Select (x => x.Guid)
					.ToArray ();

				foreach (var guid in guids)
				{
					var e = obj.GetEvent (guid);
					obj.RemoveEvent (e);
				}
			}
		}


		private readonly DataAccessor			accessor;
	}
}
