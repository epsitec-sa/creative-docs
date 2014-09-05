//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
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


		#region Out of bounds intervals logic
		public static bool IsOutOfBounds(List<OutOfBoundsInterval> outOfBoundsIntervals, Timestamp timestamp)
		{
			//	Indique si une date est à l'intérieur d'un invervalle bloqué.
			foreach (var interval in outOfBoundsIntervals)
			{
				if (timestamp > interval.Start &&
					timestamp < interval.End)
				{
					return true;
				}
			}

			return false;
		}

		public static List<OutOfBoundsInterval> GetOutOfBoundsIntervals(DataObject obj)
		{
			//	Retourne la liste des intervalles bloqués, en fonction des événements
			//	d'entrée/sortie d'un objet.
			var intervals = new List<OutOfBoundsInterval> ();

			if (obj != null)
			{
				var start = Timestamp.MinValue;
				bool isOutOfBounds = true;  // bloqué jusqu'au premier événement d'entrée

				int eventCount = obj.EventsCount;
				for (int i=0; i<eventCount; i++)
				{
					var e = obj.GetEvent (i);

					if (e.Type == EventType.Input)
					{
						var li = new OutOfBoundsInterval
						{
							Start = start,
							End   = e.Timestamp,
						};

						intervals.Add (li);
						isOutOfBounds = false;
					}

					if (e.Type == EventType.Output)
					{
						start = e.Timestamp;
						isOutOfBounds = true;
					}
				}

				if (isOutOfBounds)
				{
					var li = new OutOfBoundsInterval
					{
						Start = start,
						End   = Timestamp.MaxValue,
					};

					intervals.Add (li);
				}
			}

			return intervals;
		}

		public struct OutOfBoundsInterval
		{
			public Timestamp Start;
			public Timestamp End;
		}
		#endregion


		#region Plausible event logic
		public static bool IsOutOfBoundsEvent(DataObject obj, Timestamp timestamp)
		{
			if (AssetCalculator.IsLocked (obj, timestamp))
			{
				return true;
			}

			var prevEvent = AssetCalculator.GetPrevEvent (obj, timestamp);
			var nextEvent = AssetCalculator.GetNextEvent (obj, timestamp);

			return !((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
					 nextEvent != TerminalEvent.In);
		}

		public static IEnumerable<EventType> GetPlausibleEventTypes(DataObject obj, Timestamp timestamp)
		{
			if (!AssetCalculator.IsLocked (obj, timestamp))
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
					yield return EventType.Decrease;
					yield return EventType.Increase;
					yield return EventType.MainValue;
					yield return EventType.AmortizationExtra;
					yield return EventType.Locked;
				}

				if ((prevEvent == TerminalEvent.In || prevEvent == TerminalEvent.Other) &&
					nextEvent == TerminalEvent.None)
				{
					yield return EventType.Output;
				}
			}
		}

		private static TerminalEvent GetPrevEvent(DataObject obj, Timestamp timestamp)
		{
			if (obj != null)
			{
				int i = obj.Events.Where (x => x.Timestamp <= timestamp).Count () - 1;

				if (i >= 0 && i < obj.EventsCount)
				{
					return AssetCalculator.GetTerminalEvent (obj.GetEvent (i).Type);
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
					return AssetCalculator.GetTerminalEvent (obj.GetEvent (i).Type);
				}
			}

			return TerminalEvent.None;
		}

		private static TerminalEvent GetTerminalEvent(EventType eventType)
		{
			switch (eventType)
			{
				case EventType.Input:
					return TerminalEvent.In;

				case EventType.Output:
					return TerminalEvent.Out;

				default:
					return TerminalEvent.Other;
			}
		}

		private enum TerminalEvent
		{
			None,
			In,
			Out,
			Other,
		}
		#endregion


		#region Locked event logic
		public static bool IsLockable(DataAccessor accessor, Guid guid, System.DateTime createDate)
		{
			//	Indique si une opération de verrouillage est possible. La présence d'un
			//	événement d'aperçu d'amortissement empêche l'opération.
			if (guid.IsEmpty)  // tous ?
			{
				var getter = accessor.GetNodeGetter (BaseType.Assets);

				foreach (var node in getter.GetNodes ())
				{
					var obj = accessor.GetObject (BaseType.Assets, node.Guid);
					var timestamp = AssetCalculator.GetPreviewTimestamp (obj);
					if (timestamp.HasValue && timestamp.Value.Date < createDate)
					{
						return false;
					}
				}
			}
			else  // un seul ?
			{
				var obj = accessor.GetObject (BaseType.Assets, guid);
				var timestamp = AssetCalculator.GetPreviewTimestamp (obj);
				if (timestamp.HasValue && timestamp.Value.Date < createDate)
				{
					return false;
				}
			}

			return true;
		}

		public static void Locked(DataAccessor accessor, Guid guid, bool isDelete, System.DateTime createDate)
		{
			//	Effectue une action initiée par LockedPopup.
			DataObject obj = null;
			if (!guid.IsEmpty)
			{
				obj = accessor.GetObject (BaseType.Assets, guid);
			}

			if (isDelete)  // déverrouille ?
			{
				if (guid.IsEmpty)  // tous ?
				{
					var getter = accessor.GetNodeGetter (BaseType.Assets);

					foreach (var node in getter.GetNodes ())
					{
						obj = accessor.GetObject (BaseType.Assets, node.Guid);
						AssetCalculator.RemoveLockedEvent (obj);
					}
				}
				else  // un seul ?
				{
					AssetCalculator.RemoveLockedEvent (obj);
				}
			}
			else  // verrouille ?
			{
				if (guid.IsEmpty)  // tous ?
				{
					var getter = accessor.GetNodeGetter (BaseType.Assets);

					foreach (var node in getter.GetNodes ())
					{
						obj = accessor.GetObject (BaseType.Assets, node.Guid);
						AssetCalculator.CreateLockedEvent (accessor, obj, createDate);
					}
				}
				else  // un seul ?
				{
					AssetCalculator.CreateLockedEvent (accessor, obj, createDate);
				}
			}
		}

		private static void CreateLockedEvent(DataAccessor accessor, DataObject obj, System.DateTime date)
		{
			//	Crée l'événement Locked (cadenas) de l'objet, s'il est après l'événement d'entrée.
			if (obj != null)
			{
				//	On supprime l'éventuel événement Locked existant, pour garantir
				//	qu'il n'en existe qu'un seul.
				AssetCalculator.RemoveLockedEvent (obj);

				if (!AssetCalculator.IsOutOfBoundsEvent (obj, new Timestamp (date, 0)))
				{
					accessor.CreateAssetEvent (obj, date, EventType.Locked);
				}
			}
		}

		public static void RemoveLockedEvent(DataObject obj)
		{
			//	Supprime l'événement Locked (cadenas) de l'objet, s'il existe.
			//	Rappel: Il ne doit y avoir qu'un seul événement Locked par objet !
			if (obj != null)
			{
				var e = obj.Events.Where (x => x.Type == EventType.Locked).LastOrDefault ();
				if (e != null)
				{
					obj.RemoveEvent (e);
				}
			}
		}

		public static bool IsLocked(DataObject obj, Timestamp timestamp)
		{
			//	Retourne true s'il existe un événement Locked (cadenas) postérieur.
			if (obj != null)
			{
				var t = AssetCalculator.GetLockedTimestamp (obj);
				if (t.HasValue)
				{
					return timestamp < t.Value;
				}
			}

			return false;
		}

		public static Timestamp? GetLockedTimestamp(DataObject obj)
		{
			//	Retourne le Timestamp de l'événement Locked (cadenas), s'il existe.
			//	Normalement, il doit n'en exister qu'un seul par objet. Si d'aventure il en
			//	existait plusieurs, c'est le plus récent qui fait foi !
			if (obj != null)
			{
				var e = obj.Events.Where (x => x.Type == EventType.Locked).LastOrDefault ();
				if (e != null)
				{
					return e.Timestamp;
				}
			}

			return null;
		}

		private static Timestamp? GetPreviewTimestamp(DataObject obj)
		{
			//	Retourne le Timestamp de l'événement d'aperçu d'amortisement le plus vieux.
			if (obj != null)
			{
				var e = obj.Events.Where (x => x.Type == EventType.AmortizationPreview).FirstOrDefault ();
				if (e != null)
				{
					return e.Timestamp;
				}
			}

			return null;
		}
		#endregion
	}
}
