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

		public static IEnumerable<EventType> EventTypes
		{
			get
			{
				yield return EventType.Input;
				yield return EventType.Modification;
				yield return EventType.Reorganization;
				yield return EventType.Increase;
				yield return EventType.Decrease;
				yield return EventType.AmortizationExtra;
				yield return EventType.Output;
			}
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
				yield return EventType.Reorganization;
				yield return EventType.Increase;
				yield return EventType.Decrease;
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


		#region Update amounts
		public static void UpdateAmounts(DataAccessor accessor, DataObject obj)
		{
			//	Répercute les valeurs des montants selon la chronologie des événements.
			if (obj != null)
			{
				foreach (var field in accessor.ValueFields)
				{
					decimal? lastAmount = null;
					decimal? lastBase   = null;

					foreach (var e in obj.Events)
					{
						if (field == ObjectField.MainValue)
						{
							AssetCalculator.UpdateAmortizedAmount (e, field, ref lastAmount, ref lastBase);
						}
						else
						{
							AssetCalculator.UpdateComputedAmount (e, field, ref lastAmount);
						}
					}
				}
			}
		}

		private static void UpdateAmortizedAmount(DataEvent e, ObjectField field, ref decimal? lastAmount, ref decimal? lastBase)
		{
			var current = AssetCalculator.GetAmortizedAmount (e, field);

			if (current.HasValue)
			{
				if (current.Value.AmortizationType == AmortizationType.Unknown)  // montant fixe ?
				{
					lastBase = current.Value.FinalAmortizedAmount;
				}
				else  // amortissement ?
				{
					current = new AmortizedAmount
					(
						current.Value.AmortizationType,
						lastAmount.HasValue ? lastAmount.Value : current.Value.InitialAmount,
						lastBase.HasValue ? lastBase.Value : current.Value.BaseAmount,
						current.Value.EffectiveRate,
						current.Value.ProrataNumerator,
						current.Value.ProrataDenominator,
						current.Value.RoundAmount,
						current.Value.ResidualAmount
					);

					AssetCalculator.SetAmortizedAmount (e, field, current);
				}

				lastAmount = current.Value.FinalAmortizedAmount;
			}
		}

		private static void UpdateComputedAmount(DataEvent e, ObjectField field, ref decimal? lastAmount)
		{
			var current = AssetCalculator.GetComputedAmount (e, field);

			if (current.HasValue)
			{
				if (lastAmount.HasValue == false)
				{
					lastAmount = current.Value.FinalAmount;
				}
				else
				{
					current = new ComputedAmount (lastAmount.Value, current.Value);
					lastAmount = current.Value.FinalAmount;
					AssetCalculator.SetComputedAmount (e, field, current);
				}
			}
		}

		private static AmortizedAmount? GetAmortizedAmount(DataEvent e, ObjectField field)
		{
			var p = e.GetProperty (field) as DataAmortizedAmountProperty;
			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		private static void SetAmortizedAmount(DataEvent e, ObjectField field, AmortizedAmount? value)
		{
			if (value.HasValue)
			{
				var newProperty = new DataAmortizedAmountProperty (field, value.Value);
				e.AddProperty (newProperty);
			}
			else
			{
				e.RemoveProperty (field);
			}
		}

		private static ComputedAmount? GetComputedAmount(DataEvent e, ObjectField field)
		{
			var p = e.GetProperty (field) as DataComputedAmountProperty;
			if (p == null)
			{
				return null;
			}
			else
			{
				return p.Value;
			}
		}

		private static void SetComputedAmount(DataEvent e, ObjectField field, ComputedAmount? value)
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
		#endregion
	}
}
