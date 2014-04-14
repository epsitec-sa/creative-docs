//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class TimelineData
	{
		/// <summary>
		/// Cette classe détermine le contenu d'une timeline de façon totalement indépendante
		/// de la UI (notamment de la classe NavigationTimelineController).
		/// </summary>
		public TimelineData(DataAccessor accessor, BaseType baseType)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.cells = new List<TimelineCell> ();
		}


		public void Compute(Guid? objectGuid, TimelineMode mode, System.DateTime start, System.DateTime end, System.DateTime? forcedDate)
		{
			this.cells.Clear ();

			if ((mode & TimelineMode.Expanded) != 0)
			{
				this.AddStartEnd (start, end);
			}

			//if ((mode & TimelineMode.Compacted) != 0)
			if (false)
			{
				this.AddFirstMonthDay (start, end);
			}

			//	Si nécessaire, ajoute les cellules forcées.
			this.AddForcedDate (Timestamp.Now.Date);
			this.AddForcedDate (forcedDate);

			//	Ajoute les cellules correspondant aux événements de l'objet.
			if (objectGuid.HasValue)
			{
				this.AddObject (start, end, objectGuid.Value);
				this.UpdateFlags (objectGuid.Value);
			}
		}

		private void AddStartEnd(System.DateTime start, System.DateTime end)
		{
			//	Crée des cellules pour tous les jours compris entre 'start' et 'end'.
			for (int i = 0; i < 365*100; i++)
			{
				var date = start.Date.AddDays (i);

				if (date > end.Date)
				{
					break;
				}

				var cell = new TimelineCell
				{
					Timestamp     = new Timestamp (date, 0),
					Glyph = TimelineGlyph.Empty,
				};

				this.cells.Add (cell);
			}
		}

		private void AddFirstMonthDay(System.DateTime start, System.DateTime end)
		{
			//	Crée des cellules pour tous les premiers du mois compris entre 'start' et 'end'.
			int year  = start.Year;
			int month = start.Month;

			for (int i = 0; i < 12*100; i++)
			{
				var date = new System.DateTime (year, month, 1);

				if (date > end.Date)
				{
					break;
				}

				var cell = new TimelineCell
				{
					Timestamp     = new Timestamp (date, 0),
					Glyph = TimelineGlyph.Empty,
				};

				this.cells.Add (cell);

				month++;
				if (month > 12)
				{
					month = 1;
					year++;
				}
			}
		}

		private void AddObject(System.DateTime start, System.DateTime end, Guid objectGuid)
		{
			//	Ajoute les cellules correspondant aux événements de l'objet.
			//	S'il existe déjà une cellule à la date concernée, on modifie la
			//	cellule pour représenter l'événement de l'objet.
			var obj = this.accessor.GetObject (this.baseType, objectGuid);

			if (obj != null)
			{
				int eventCount = obj.EventsCount;
				var userValueField = this.UserValueField;

				for (int i=0; i<eventCount; i++)
				{
					var e = obj.GetEvent (i);
					var t = e.Timestamp;

					if (t.Date >= start)
					{
						if (t.Date > end)
						{
							break;
						}

						var index = this.cells.FindIndex (x => x.Timestamp == t);
						var type = e.Type;
						var glyph = TimelineData.TypeToGlyph (type);

						var value1 = ObjectProperties.GetObjectPropertyComputedAmount (obj, t, ObjectField.MainValue);

						ComputedAmount? value2 = null;
						if (userValueField != ObjectField.Unknown)
						{
							value2 = ObjectProperties.GetObjectPropertyComputedAmount (obj, t, userValueField);
						}

						decimal? v1 = value1 != null && value1.HasValue ? value1.Value.FinalAmount : null;
						decimal? v2 = value2 != null && value2.HasValue ? value2.Value.FinalAmount : null;

						if (index == -1)
						{
							var cell = new TimelineCell
							{
								Timestamp = t,
								Glyph     = glyph,
								Tooltip   = LogicDescriptions.GetTooltip (this.accessor, obj, t, type, 8),
								Values    = new decimal?[] { v1, v2 },
							};

							index = this.GetIndex (t);
							this.cells.Insert (index, cell);
						}
						else
						{
							var cell = new TimelineCell
							{
								Timestamp = this.cells[index].Timestamp,
								Glyph     = glyph,
								Tooltip   = LogicDescriptions.GetTooltip (this.accessor, obj, t, type, 8),
								Values    = new decimal?[] { v1, v2 },
							};

							this.cells[index] = cell;
						}
					}
				}
			}
		}

		private ObjectField UserValueField
		{
			get
			{
				var userField = this.accessor.GlobalSettings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type == FieldType.ComputedAmount)
					.FirstOrDefault ();

				if (userField.IsEmpty)
				{
					return ObjectField.Unknown;
				}
				else
				{
					return userField.Field;
				}
			}
		}

		private void AddForcedDate(System.DateTime? date)
		{
			//	Ajoute une date dans la liste des cellules, si elle n'y est pas déjà.
			if (date.HasValue &&
				!this.cells.Where (x => x.Timestamp.Date == date.Value).Any ())
			{
				string tooltip = null;

				if (date.Value == Timestamp.Now.Date)
				{
					tooltip = "Aujourd'hui";
				}

				var cell = new TimelineCell
				{
					Timestamp = new Timestamp(date.Value, 0),
					Glyph     = TimelineGlyph.Empty,
					Tooltip   = tooltip,
				};

				//	On calcule l'index où insérer la cellule, pour que la liste reste
				//	triée chronologiquement.
				int i = this.cells.Where (x => x.Timestamp.Date < date.Value).Count ();
				this.cells.Insert (i, cell);
			}
		}


		private void UpdateFlags(Guid objectGuid)
		{
			//	Toutes les dates avant un événement d'entrée ou après un événement de
			//	sortie sont marquées comme bloquées. Elles auront un fond hachuré.
			var obj = this.accessor.GetObject (this.baseType, objectGuid);
			var outOfBoundsIntervals = AssetCalculator.GetOutOfBoundsIntervals (obj);
			var lockedTimestamp = AssetCalculator.GetLockedTimestamp (obj);

			for (int i=0; i<this.cells.Count; i++)
			{
				var cell = this.cells[i];
				var flags = DataCellFlags.None;

				if (cell.Timestamp < lockedTimestamp)
				{
					flags |= DataCellFlags.Locked;
				}

				if (AssetCalculator.IsOutOfBounds (outOfBoundsIntervals, cell.Timestamp))
				{
					flags |= DataCellFlags.OutOfBounds;
				}

				if (flags != DataCellFlags.None)
				{
					cell.Flags = flags;
					this.cells[i] = cell;
				}
			}
		}


		public static TimelineGlyph TypeToGlyph(EventType? type)
		{
			switch (type.GetValueOrDefault (EventType.Unknown))
			{
				case EventType.Unknown:
					return TimelineGlyph.Empty;

				case EventType.Input:
					return TimelineGlyph.FilledSquare;

				case EventType.Output:
					return TimelineGlyph.OutlinedSquare;

				case EventType.Modification:
					return TimelineGlyph.FilledCircle;

				case EventType.Revaluation:
					return TimelineGlyph.FilledStar;

				case EventType.Revalorization:
					return TimelineGlyph.FilledUp;

				case EventType.MainValue:
					return TimelineGlyph.FilledDown;

				case EventType.AmortizationAuto:
					return TimelineGlyph.PinnedDiamond;

				case EventType.AmortizationPreview:
					return TimelineGlyph.OutlinedDiamond;

				case EventType.AmortizationExtra:
					return TimelineGlyph.FilledDiamond;

				case EventType.Locked:
					return TimelineGlyph.Locked;

				default:
					return TimelineGlyph.Undefined;
			}
		}

		private int GetIndex(Timestamp timestamp)
		{

			return this.cells.Where (x => x.Timestamp < timestamp).Count ();
		}


		public int CellsCount
		{
			get
			{
				return this.cells.Count;
			}
		}

		public void GetMinMax(out decimal min, out decimal max)
		{
			min = decimal.MaxValue;
			max = decimal.MinValue;

			foreach (var cell in this.cells)
			{
				if (cell.Values != null)
				{
					foreach (var value in cell.Values)
					{
						if (value.HasValue)
						{
							min = System.Math.Min (min, value.Value);
							max = System.Math.Max (max, value.Value);
						}
					}
				}
			}
		}

		public TimelineCell? GetSyntheticCell(int index)
		{
			var syntheticCell = new TimelineCell ()
			{
				Values = new decimal?[TimelineData.MaxValues],
			};

			bool first = true;

			while (index >= 0)
			{
				var cell = this[index--];

				if (cell.HasValue)
				{
					if (first)
					{
						syntheticCell.Timestamp = cell.Value.Timestamp;
						syntheticCell.Glyph     = cell.Value.Glyph;
						first = false;
					}

					if (cell.Value.Values != null)
					{
						int count = System.Math.Min (syntheticCell.Values.Length, cell.Value.Values.Length);
						for (int i=0; i<count; i++)
						{
							if (!syntheticCell.Values[i].HasValue && cell.Value.Values[i].HasValue)
							{
								syntheticCell.Values[i] = cell.Value.Values[i];
							}
						}

						if (syntheticCell.Values.Where (x => x.HasValue).Count () == syntheticCell.Values.Length)
						{
							break;
						}
					}
				}
			}

			return syntheticCell;
		}


		public int GetCellIndex(System.DateTime? dateTime)
		{
			if (dateTime.HasValue)
			{
				return this.cells.FindIndex (x => x.Timestamp.Date == dateTime.Value);
			}
			else
			{
				return -1;
			}
		}

		public int GetCellIndex(Timestamp? timestamp)
		{
			if (timestamp.HasValue)
			{
				return this.cells.FindIndex (x => x.Timestamp == timestamp.Value);
			}
			else
			{
				return -1;
			}
		}


		public TimelineCell? this[Timestamp? timestamp]
		{
			get
			{
				if (timestamp.HasValue)
				{
					return this.cells.Where (x => x.Timestamp == timestamp.Value).FirstOrDefault ();
				}
				else
				{
					return null;
				}
			}
		}

		public TimelineCell? this[int index]
		{
			get
			{
				if (index >= 0 && index < this.cells.Count)
				{
					return this.cells[index];
				}
				else
				{
					return null;
				}
			}
		}


		public struct TimelineCell
		{
			public Timestamp		Timestamp;
			public TimelineGlyph	Glyph;
			public DataCellFlags	Flags;
			public string			Tooltip;
			public decimal?[]		Values;
		}


		public static readonly int MaxValues = 2;

		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;
		private readonly List<TimelineCell>		cells;
	}
}
