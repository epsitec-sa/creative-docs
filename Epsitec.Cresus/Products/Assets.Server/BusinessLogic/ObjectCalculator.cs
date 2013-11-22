//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class ObjectCalculator
	{
		public static Timestamp? GetLastTimestamp(DataObject obj)
		{
			//	Retourne la date à sélectionner dans la timeline après la sélection
			//	de l'objet.
			if (obj.EventsCount > 0)
			{
				return obj.GetEvent (obj.EventsCount-1).Timestamp;
			}

			return null;
		}


		#region Locked intervals logic
		public static bool IsLocked(List<LockedInterval> lockedIntervals, Timestamp timestamp)
		{
			//	Indique si une date est à l'intérieur d'un invervalle bloqué.
			foreach (var interval in lockedIntervals)
			{
				if (timestamp > interval.Start &&
					timestamp < interval.End)
				{
					return true;
				}
			}

			return false;
		}

		public static List<LockedInterval> GetLockedIntervals(DataObject obj)
		{
			//	Retourne la liste des intervalles bloqués, en fonction des événements
			//	d'entrée/sortie d'un objet.
			var intervals = new List<LockedInterval> ();

			if (obj != null)
			{
				var start = new Timestamp (System.DateTime.MinValue, 0);
				bool isLocked = true;  // bloqué jusqu'au premier événement d'entrée

				int eventCount = obj.EventsCount;
				for (int i=0; i<eventCount; i++)
				{
					var e = obj.GetEvent (i);

					if (e.Type == EventType.Entrée)
					{
						var li = new LockedInterval
						{
							Start = start,
							End   = e.Timestamp,
						};

						intervals.Add (li);
						isLocked = false;
					}

					if (e.Type == EventType.Sortie)
					{
						start = e.Timestamp;
						isLocked = true;
					}
				}

				if (isLocked)
				{
					var li = new LockedInterval
					{
						Start = start,
						End   = new Timestamp (System.DateTime.MaxValue, 0),
					};

					intervals.Add (li);
				}
			}

			return intervals;
		}

		public struct LockedInterval
		{
			public Timestamp Start;
			public Timestamp End;
		}
		#endregion


		#region Plausible event logic
		public static bool IsEventLocked(DataObject obj, Timestamp timestamp)
		{
			var prevEvent = ObjectCalculator.GetPrevEvent (obj, timestamp);
			var nextEvent = ObjectCalculator.GetNextEvent (obj, timestamp);

			return !((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
					 nextEvent != TerminalEvent.In);
		}

		public static IEnumerable<EventType> GetPlausibleEventTypes(BaseType baseType, DataObject obj, Timestamp timestamp)
		{
			switch (baseType)
			{
				case BaseType.Objects:
					return ObjectCalculator.GetPlausibleObjectEventTypes (obj, timestamp);

				case BaseType.Categories:
				case BaseType.Groups:
					return ObjectCalculator.GetPlausibleCategoryEventTypes (obj, timestamp);

				default:
					return null;
			}
		}

		private static IEnumerable<EventType> GetPlausibleObjectEventTypes(DataObject obj, Timestamp timestamp)
		{
			var prevEvent = ObjectCalculator.GetPrevEvent (obj, timestamp);
			var nextEvent = ObjectCalculator.GetNextEvent (obj, timestamp);

			if ((prevEvent == TerminalEvent.None || prevEvent == TerminalEvent.Out) &&
				nextEvent == TerminalEvent.None)
			{
				yield return EventType.Entrée;
			}

			if ((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
				nextEvent != TerminalEvent.In)
			{
				yield return EventType.Modification;
				yield return EventType.Réorganisation;
				yield return EventType.Augmentation;
				yield return EventType.Diminution;
				yield return EventType.AmortissementExtra;
			}

			if ((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
				nextEvent == TerminalEvent.None)
			{
				yield return EventType.Sortie;
			}
		}

		private static IEnumerable<EventType> GetPlausibleCategoryEventTypes(DataObject obj, Timestamp timestamp)
		{
			var prevEvent = ObjectCalculator.GetPrevEvent (obj, timestamp);
			var nextEvent = ObjectCalculator.GetNextEvent (obj, timestamp);

			if ((prevEvent == TerminalEvent.None || prevEvent == TerminalEvent.Out) &&
				nextEvent == TerminalEvent.None)
			{
				yield return EventType.Entrée;
			}

			if ((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
				nextEvent != TerminalEvent.In)
			{
				yield return EventType.Modification;
			}

			if ((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
				nextEvent == TerminalEvent.None)
			{
				yield return EventType.Sortie;
			}
		}

		private static TerminalEvent GetPrevEvent(DataObject obj, Timestamp timestamp)
		{
			int i = obj.Events.Where (x => x.Timestamp <= timestamp).Count () - 1;

			if (i >= 0 && i < obj.EventsCount)
			{
				switch (obj.GetEvent (i).Type)
				{
					case EventType.Entrée:
						return TerminalEvent.In;

					case EventType.Sortie:
						return TerminalEvent.Out;

					default:
						return TerminalEvent.Other;
				}
			}

			return TerminalEvent.None;
		}

		private static TerminalEvent GetNextEvent(DataObject obj, Timestamp timestamp)
		{
			int i = obj.Events.Where (x => x.Timestamp <= timestamp).Count ();

			if (i >= 0 && i < obj.EventsCount)
			{
				switch (obj.GetEvent (i).Type)
				{
					case EventType.Entrée:
						return TerminalEvent.In;

					case EventType.Sortie:
						return TerminalEvent.Out;

					default:
						return TerminalEvent.Other;
				}
			}

			return TerminalEvent.None;
		}

		private enum TerminalEvent
		{
			None,
			In,
			Out,
			Other,
		}
		#endregion


		public static bool IsExistingObject(DataObject obj, Timestamp timestamp)
		{
			if (obj != null && obj.EventsCount > 0)
			{
				var e = obj.GetEvent (0);
				return e.Timestamp <= timestamp;
			}

			return false;
		}


		#region Get properties
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

		public static Guid GetObjectPropertyGuid(DataObject obj, Timestamp? timestamp, ObjectField field, bool synthetic = true)
		{
			var p = ObjectCalculator.GetObjectProperty (obj, timestamp, field, synthetic) as DataGuidProperty;

			if (p == null)
			{
				return Guid.Empty;
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


		public static ComparableData GetComparableData(DataObject obj, Timestamp? timestamp, ObjectField field)
		{
			if (obj != null)
			{
				var p = ObjectCalculator.GetObjectSyntheticProperty (obj, timestamp, field);
				if (p != null)
				{
					if (p is DataIntProperty)
					{
						return new ComparableData ((p as DataIntProperty).Value);
					}
					else if (p is DataDecimalProperty)
					{
						return new ComparableData ((p as DataDecimalProperty).Value);
					}
					else if (p is DataComputedAmountProperty)
					{
						return new ComparableData ((p as DataComputedAmountProperty).Value.FinalAmount.GetValueOrDefault ());
					}
					else if (p is DataDateProperty)
					{
						return new ComparableData ((p as DataDateProperty).Value);
					}
					else if (p is DataStringProperty)
					{
						return new ComparableData ((p as DataStringProperty).Value);
					}
				}
			}

			return ComparableData.Empty;
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
				return obj.GetSingleProperty (timestamp, field);
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

				return obj.GetSyntheticProperty (timestamp.Value, field);
			}
		}
		#endregion


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


		#region Update computed amount
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
			var field = ObjectCalculator.RankToField (rank);
			if (field != ObjectField.Unknown)
			{
				var p = e.GetProperty (field) as DataComputedAmountProperty;
				if (p != null)
				{
					return p.Value;
				}
			}

			return null;
		}

		private static void SetComputedAmount(DataEvent e, int rank, ComputedAmount? value)
		{
			var field = ObjectCalculator.RankToField (rank);
			if (field != ObjectField.Unknown)
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
		}

		private static ObjectField RankToField(int rank)
		{
			switch (rank)
			{
				case 0:
					return ObjectField.Valeur1;

				case 1:
					return ObjectField.Valeur2;

				case 2:
					return ObjectField.Valeur3;

				default:
					return ObjectField.Unknown;
			}
		}
		#endregion
	}
}
