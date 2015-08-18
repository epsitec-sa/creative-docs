//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
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


		public IEnumerable<string>				ValuesFieldNames
		{
			get
			{
				yield return DataDescriptions.GetObjectFieldDescription (ObjectField.MainValue);

				foreach (var field in this.accessor.UserFieldsAccessor.GetUserFields (BaseType.AssetsUserFields)
					.Where (x => x.Type == FieldType.ComputedAmount))
				{
					yield return field.Name;
				}
			}
		}


		public void Compute(Guid? objectGuid, TimelineMode mode, System.DateTime start, System.DateTime end, System.DateTime? forcedDate)
		{
			this.cells.Clear ();

			if ((mode & TimelineMode.Expanded) != 0)
			{
				this.AddStartEnd (start, end);
			}

			//if ((mode & TimelineMode.Compacted) != 0)
			//if (false)
			//{
			//	this.AddFirstMonthDay (start, end);
			//}

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
				var a = obj.Events;

				for (int i=0; i<a.Count; i++)
				{
					var e = a[i];
					var t = e.Timestamp;

					if (t.Date >= start)
					{
						if (t.Date > end)
						{
							break;
						}

						var index = this.cells.FindIndex (x => x.Timestamp == t);
						var type = e.Type;
						var mode  = AssetsLogic.IsAmortizationEnded (obj, e);
						var glyph = TimelineData.TypeToGlyph (type, mode);

						if (index == -1)
						{
							var cell = new TimelineCell
							{
								Timestamp = t,
								Glyph     = glyph,
								Tooltip   = LogicDescriptions.GetTooltip (this.accessor, obj, t, type, 8),
								Values    = this.GetValues (obj, t),
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
								Values    = this.GetValues (obj, t),
							};

							this.cells[index] = cell;
						}
					}
				}
			}
		}

		private decimal?[] GetValues(DataObject obj, Timestamp timestamp)
		{
			var list = new List<decimal?> ();

			list.Add (this.GetMainValue (obj, timestamp));

			foreach (var field in this.accessor.UserFieldsAccessor.GetUserFields (BaseType.AssetsUserFields)
				.Where (x => x.Type == FieldType.ComputedAmount)
				.Select (x => x.Field))
			{
				list.Add (this.GetUserValue (obj, timestamp, field));
			}

			return list.ToArray ();
		}

		private decimal? GetMainValue(DataObject obj, Timestamp timestamp)
		{
			var p = ObjectProperties.GetObjectPropertyAmortizedAmount (obj, timestamp, ObjectField.MainValue, synthetic: false);

			if (p != null)
			{
				return p.Value.FinalAmount;
			}

			return null;
		}

		private decimal? GetUserValue(DataObject obj, Timestamp timestamp, ObjectField field)
		{
			if (field != ObjectField.Unknown)
			{
				var p = ObjectProperties.GetObjectPropertyComputedAmount (obj, timestamp, field, synthetic: false);

				if (p != null)
				{
					return p.Value.FinalAmount;
				}
			}

			return null;
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
					tooltip = Res.Strings.TimelineData.Now.ToString ();
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


		public static TimelineGlyph TypeToGlyph(EventType? type, TimelineGlyphMode mode = TimelineGlyphMode.Full)
		{
			switch (type.GetValueOrDefault (EventType.Unknown))
			{
				case EventType.Unknown:
					return TimelineGlyph.Empty;

				case EventType.PreInput:
					return new TimelineGlyph (TimelineGlyphShape.SquareHoles, mode);

				case EventType.Input:
					return new TimelineGlyph (TimelineGlyphShape.FilledSquare, mode);

				case EventType.Output:
					return new TimelineGlyph (TimelineGlyphShape.OutlinedSquare, mode);

				case EventType.Modification:
					return new TimelineGlyph (TimelineGlyphShape.FilledCircle, mode);

				case EventType.Increase:
					return new TimelineGlyph (TimelineGlyphShape.FilledUp, mode);

				case EventType.Decrease:
					return new TimelineGlyph (TimelineGlyphShape.FilledDown, mode);

				case EventType.Adjust:
					return new TimelineGlyph (TimelineGlyphShape.FilledStar, mode);

				case EventType.AmortizationAuto:
					return new TimelineGlyph (TimelineGlyphShape.PinnedDiamond, mode);

				case EventType.AmortizationPreview:
					return new TimelineGlyph (TimelineGlyphShape.OutlinedDiamond, mode);

				case EventType.AmortizationExtra:
					return new TimelineGlyph (TimelineGlyphShape.FilledDiamond, mode);

				case EventType.AmortizationSuppl:
					return new TimelineGlyph (TimelineGlyphShape.PlusDiamond, mode);

				case EventType.Locked:
					return new TimelineGlyph (TimelineGlyphShape.Locked, mode);

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
			var syntheticCell = new TimelineCell ();
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
						if (syntheticCell.Values == null)
						{
							syntheticCell.Values = new decimal?[cell.Value.Values.Length];
						}

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


		private readonly DataAccessor			accessor;
		private readonly BaseType				baseType;
		private readonly List<TimelineCell>		cells;
	}
}
