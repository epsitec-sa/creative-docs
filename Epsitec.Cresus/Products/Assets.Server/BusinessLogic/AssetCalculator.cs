//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class AssetCalculator
	{
		public static Timestamp? GetFirstTimestamp(DataObject obj)
		{
			//	Retourne la date d'entrée d'un objet.
			if (obj.EventsCount > 0)
			{
				return obj.GetEvent (0).Timestamp;
			}

			return null;
		}

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
				var start = Timestamp.MinValue;
				bool isLocked = true;  // bloqué jusqu'au premier événement d'entrée

				int eventCount = obj.EventsCount;
				for (int i=0; i<eventCount; i++)
				{
					var e = obj.GetEvent (i);

					if (e.Type == EventType.Input)
					{
						var li = new LockedInterval
						{
							Start = start,
							End   = e.Timestamp,
						};

						intervals.Add (li);
						isLocked = false;
					}

					if (e.Type == EventType.Output)
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
						End   = Timestamp.MaxValue,
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
			var prevEvent = AssetCalculator.GetPrevEvent (obj, timestamp);
			var nextEvent = AssetCalculator.GetNextEvent (obj, timestamp);

			return !((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
					 nextEvent != TerminalEvent.In);
		}

		public static IEnumerable<EventType> GetPlausibleEventTypes(DataObject obj, Timestamp timestamp)
		{
			var prevEvent = AssetCalculator.GetPrevEvent (obj, timestamp);
			var nextEvent = AssetCalculator.GetNextEvent (obj, timestamp);

			if ((prevEvent == TerminalEvent.None || prevEvent == TerminalEvent.Out) &&
				nextEvent == TerminalEvent.None)
			{
				yield return EventType.Input;
			}

			if ((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
				nextEvent != TerminalEvent.In)
			{
				yield return EventType.Modification;
				yield return EventType.MainValue;
				yield return EventType.AmortizationExtra;
			}

			if ((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
				nextEvent == TerminalEvent.None)
			{
				yield return EventType.Output;
			}
		}

		private static TerminalEvent GetPrevEvent(DataObject obj, Timestamp timestamp)
		{
			if (obj != null)
			{
				int i = obj.Events.Where (x => x.Timestamp <= timestamp).Count () - 1;

				if (i >= 0 && i < obj.EventsCount)
				{
					switch (obj.GetEvent (i).Type)
					{
						case EventType.Input:
							return TerminalEvent.In;

						case EventType.Output:
							return TerminalEvent.Out;

						default:
							return TerminalEvent.Other;
					}
				}
			}

			return TerminalEvent.None;
		}

		private static TerminalEvent GetNextEvent(DataObject obj, Timestamp timestamp)
		{
			if (obj != null)
			{
				int i = obj.Events.Where (x => x.Timestamp <= timestamp).Count ();

				if (i >= 0 && i < obj.EventsCount)
				{
					switch (obj.GetEvent (i).Type)
					{
						case EventType.Input:
							return TerminalEvent.In;

						case EventType.Output:
							return TerminalEvent.Out;

						default:
							return TerminalEvent.Other;
					}
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
	}
}
