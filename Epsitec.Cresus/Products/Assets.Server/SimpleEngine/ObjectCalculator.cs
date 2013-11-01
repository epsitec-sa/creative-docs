//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class ObjectCalculator
	{
		public static bool IsExistingObject(DataObject obj, Timestamp timestamp)
		{
			if (obj.EventsCount > 0)
			{
				var e = obj.GetEvent (0);
				return e.Timestamp <= timestamp;
			}

			return false;
		}


		public static int? GetObjectPropertyInt(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectCalculator.GetObjectProperty (obj, timestamp, field, synthetic) as DataIntProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static decimal? GetObjectPropertyDecimal(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectCalculator.GetObjectProperty (obj, timestamp, field, synthetic) as DataDecimalProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static ComputedAmount? GetObjectPropertyComputedAmount(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectCalculator.GetObjectProperty (obj, timestamp, field, synthetic) as DataComputedAmountProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static System.DateTime? GetObjectPropertyDate(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectCalculator.GetObjectProperty (obj, timestamp, field, synthetic) as DataDateProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		public static string GetObjectPropertyString(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectCalculator.GetObjectProperty (obj, timestamp, field, synthetic) as DataStringProperty;

			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}


		public static AbstractDataProperty GetObjectProperty(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic)
		{
			if (synthetic || !timestamp.HasValue)
			{
				return ObjectCalculator.GetObjectSyntheticProperty (obj, timestamp, field);
			}
			else
			{
				return ObjectCalculator.GetObjectSingleProperty (obj, timestamp.Value, field);
			}
		}

		public static AbstractDataProperty GetObjectSingleProperty(DataObject obj, Timestamp timestamp, ObjectField field)
		{
			//	Retourne l'état d'une propriété d'un objet à la date exacte.
			if (obj == null)
			{
				return null;
			}
			else
			{
				return obj.GetSingleProperty (timestamp, (int) field);
			}
		}

		public static AbstractDataProperty GetObjectSyntheticProperty(DataObject obj, Timestamp? timestamp, ObjectField field)
		{
			//	Retourne l'état d'une propriété d'un objet à la date exacte ou antérieurement.
			if (obj == null)
			{
				return null;
			}
			else
			{
				if (!timestamp.HasValue)
				{
					timestamp = new Timestamp (System.DateTime.MaxValue, 0);
				}

				return obj.GetSyntheticProperty (timestamp.Value, (int) field);
			}
		}


		public static void RemoveAmortissementsAuto(DataObject obj)
		{
			//	Supprime tous les événements d'amortissement automatique d'un objet.
			if (obj != null)
			{
				var guids = obj.Events.Where (x => x.Type == EventType.AmortissementAuto).Select (x => x.Guid);

				foreach (var guid in guids)
				{
					var e = obj.GetEvent (guid);
					obj.RemoveEvent (e);
				}
			}
		}

	
		public static void UpdateComputedAmounts(DataObject obj)
		{
			if (obj != null)
			{
				for (int i=0; i<3; i++)  // Valeur1..3
				{
					decimal? last = null;

					foreach (var e in obj.Events)
					{
						var current = ObjectCalculator.GetComputedAmount (e, i);

						if (current.HasValue)
						{
							if (last.HasValue == false)
							{
								last = current.Value.FinalAmount;
								current = new ComputedAmount (last);
								ObjectCalculator.SetComputedAmount (e, i, current);
							}
							else
							{
								current = new ComputedAmount (last.Value, current.Value);
								last = current.Value.FinalAmount;
								ObjectCalculator.SetComputedAmount (e, i, current);
							}
						}
					}
				}
			}
		}

		private static ComputedAmount? GetComputedAmount(DataEvent e, int rank)
		{
			int id = ObjectCalculator.RankToField (rank);
			if (id != -1)
			{
				var p = e.GetProperty (id) as DataComputedAmountProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}

		private static void SetComputedAmount(DataEvent e, int rank, ComputedAmount? value)
		{
			int id = ObjectCalculator.RankToField (rank);
			if (id != -1)
			{
				if (value.HasValue)
				{
					var newProperty = new DataComputedAmountProperty (id, value.Value);
					e.AddProperty (newProperty);
				}
				else
				{
					e.RemoveProperty (id);
				}
			}
		}

		private static int RankToField(int rank)
		{
			switch (rank)
			{
				case 0:
					return (int) ObjectField.Valeur1;

				case 1:
					return (int) ObjectField.Valeur2;

				case 2:
					return (int) ObjectField.Valeur3;

				default:
					return -1;
			}
		}
	}
}
