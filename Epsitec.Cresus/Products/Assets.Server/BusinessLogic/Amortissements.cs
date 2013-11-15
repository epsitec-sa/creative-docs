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


		public void GeneratesAmortissementsAuto()
		{
			var getter = this.accessor.GetNodesGetter (BaseType.Objects);

			foreach (var node in getter.Nodes)
			{
				this.GeneratesAmortissementsAuto (node.Guid);
			}
		}

		public void GeneratesAmortissementsAuto(Guid objectGuid)
		{
			//	Première ébauche totalement naïve et fausse !
			//	TODO: ...
			var obj = this.accessor.GetObject (BaseType.Objects, objectGuid);

			ObjectCalculator.RemoveAmortissementsAuto (obj);

			System.DateTime? date1, date2;
			decimal? taux, rest;
			string type;
			int? freq;
			if (!this.GetAmortissement (obj, out date1, out date2, out taux, out type, out freq, out rest))
			{
				return;
			}

			for (int j=0; j<100; j++)
			{
				System.DateTime date;

				if (j == 0)
				{
					if (!date1.HasValue)
					{
						continue;
					}
					date = date1.Value;
				}
				else
				{
					var d = date2.Value.Day;
					var m = date2.Value.Month;
					var y = date2.Value.Year;

					m += (j-1)*freq.Value;

					while (m > 12)
					{
						m -=12;
						y++;
					}

					date = new System.DateTime (y, m, d);
				}

				var currentValues = this.GetValeur (obj, date);
				var newValues = new List<decimal?> ();

				for (int i=0; i<3; i++)
				{
					var v = currentValues[i].GetValueOrDefault (0);

					v -= v*taux.Value;

					if (v < rest.Value)
					{
						newValues.Add (null);
					}
					else
					{
						newValues.Add (v);
					}
				}

				this.CreateAmortissementAuto (obj, date, currentValues, newValues);
			}
		}

		private bool GetAmortissement(DataObject obj,
			out System.DateTime? date1, out System.DateTime? date2,
			out decimal? taux, out string type, out int? freq, out decimal? rest)
		{
			date1 = ObjectCalculator.GetObjectPropertyDate    (obj, null, ObjectField.DateAmortissement1);
			date2 = ObjectCalculator.GetObjectPropertyDate    (obj, null, ObjectField.DateAmortissement2);
			taux  = ObjectCalculator.GetObjectPropertyDecimal (obj, null, ObjectField.TauxAmortissement);
			type  = ObjectCalculator.GetObjectPropertyString  (obj, null, ObjectField.TypeAmortissement);
			freq  = ObjectCalculator.GetObjectPropertyInt     (obj, null, ObjectField.FréquenceAmortissement);
			rest  = ObjectCalculator.GetObjectPropertyDecimal (obj, null, ObjectField.ValeurRésiduelle);

			if (!date2.HasValue && date1.HasValue)
			{
				date2 = date1;
				date1 = null;
			}

			if (string.IsNullOrEmpty (type))
			{
				type = "Linéaire";
			}

			if (!rest.HasValue)
			{
				rest = 1.0m;
			}

			if (!freq.HasValue)
			{
				freq = 1;
			}

			return (date2.HasValue && taux.HasValue);
		}

		private List<decimal?> GetValeur(DataObject obj, System.DateTime date)
		{
			var list = new List<decimal?> ();
			var timestamp = new Timestamp(date, 0);

			for (int i=0; i<3; i++)  // Valeur1..3
			{
				ComputedAmount? m = null;
				switch (i)
				{
					case 0:
						m = ObjectCalculator.GetObjectPropertyComputedAmount (obj, timestamp, ObjectField.Valeur1);
						break;

					case 1:
						m = ObjectCalculator.GetObjectPropertyComputedAmount (obj, timestamp, ObjectField.Valeur2);
						break;

					case 2:
						m = ObjectCalculator.GetObjectPropertyComputedAmount (obj, timestamp, ObjectField.Valeur3);
						break;
				}

				if (m.HasValue)
				{
					list.Add (m.Value.FinalAmount);
				}
				else
				{
					list.Add (null);
				}
			}

			return list;
		}

		private void CreateAmortissementAuto(DataObject obj, System.DateTime date, List<decimal?> currentValues, List<decimal?> newValues)
		{
			var e = this.accessor.CreateObjectEvent (obj, date, EventType.AmortissementAuto);

			if (e != null)
			{
				for (int i=0; i<3; i++)  // Valeur1..3
				{
					if (newValues[i].HasValue)
					{
						var v = new ComputedAmount (currentValues[i].GetValueOrDefault (0), newValues[i].GetValueOrDefault (0), true);
						DataComputedAmountProperty p = null;

						switch (i)
						{
							case 0:
								p = new DataComputedAmountProperty (ObjectField.Valeur1, v);
								break;

							case 1:
								p = new DataComputedAmountProperty (ObjectField.Valeur2, v);
								break;

							case 2:
								p = new DataComputedAmountProperty (ObjectField.Valeur3, v);
								break;
						}

						if (p != null)
						{
							e.AddProperty (p);
						}
					}
				}
			}
		}


		private readonly DataAccessor accessor;
	}
}
