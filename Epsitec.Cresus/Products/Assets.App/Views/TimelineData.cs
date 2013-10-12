//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Types;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class TimelineData
	{
		/// <summary>
		/// Cette classe détermine le contenu d'une timeline de façon totalement indépendante
		/// de la UI (notamment de la classe NavigationTimelineController).
		/// </summary>
		public TimelineData(DataAccessor accessor)
		{
			this.accessor = accessor;
			this.cells = new List<TimelineCell> ();
		}


		public void Compute(Guid? objectGuid, TimelineMode mode, System.DateTime start, System.DateTime end)
		{
			this.cells.Clear ();

			if (mode == TimelineMode.Extended)
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
						TimelineGlyph = TimelineGlyph.Empty,
					};

					this.cells.Add (cell);
				}
			}

			if (mode == TimelineMode.Compacted)
			{
				//	Crée des cellules pour tous premiers du mois compris entre 'start' et 'end'.
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
						TimelineGlyph = TimelineGlyph.Empty,
					};

					this.cells.Add (cell);

					month++;
					if (month > 12)
					{
						month = 1;
						year++;
					}
				}

				//	Si nécessaire, crée une cellule pour aujourd'hui.
				var now = System.DateTime.Now;

				if (!this.cells.Where (x => x.Timestamp.Date == now).Any ())
				{
					var cell = new TimelineCell
					{
						Timestamp     = Timestamp.Now,
						TimelineGlyph = TimelineGlyph.Empty,
					};

					int i = this.cells.Where (x => x.Timestamp.Date < now).Count ();
					this.cells.Insert (i, cell);  // la liste doit être triée chronologiquement
				}
			}

			if (objectGuid.HasValue)
			{
				int eventCount = this.accessor.GetObjectEventsCount (objectGuid.Value);

				for (int i=0; i<eventCount; i++)
				{
					var t = this.accessor.GetObjectEventTimestamp (objectGuid.Value, i);

					if (t.HasValue && t.Value.Date >= start)
					{
						if (t.Value.Date > end)
						{
							break;
						}

						var index = this.cells.FindIndex (x => x.Timestamp == t.Value);
						var glyph = this.GetGlyph (objectGuid.Value, i);

						var properties = this.accessor.GetObjectSingleProperties (objectGuid.Value, t.Value);

						var value1 = DataAccessor.GetDecimalProperty (properties, (int) ObjectField.Valeur1);
						var value2 = DataAccessor.GetDecimalProperty (properties, (int) ObjectField.Valeur2);

						if (index == -1)
						{
							var cell = new TimelineCell
							{
								Timestamp     = t.Value,
								TimelineGlyph = glyph,
								Value1        = value1,
								Value2        = value2,
							};

							index = this.GetIndex (t.Value);
							this.cells.Insert (index, cell);
						}
						else
						{
							var cell = new TimelineCell
							{
								Timestamp     = this.cells[index].Timestamp,
								TimelineGlyph = glyph,
								Value1        = value1,
								Value2        = value2,
							};

							this.cells[index] = cell;
						}
					}
				}
			}
		}

		private TimelineGlyph GetGlyph(Guid objectGuid, int eventIndex)
		{
			var type = this.accessor.GetObjectEventType (objectGuid, eventIndex);

			if (type.HasValue)
			{
				return TimelineData.TypeToGlyph ((EventType) type.Value);
			}

			return TimelineGlyph.FilledCircle;
		}

		private static TimelineGlyph TypeToGlyph(EventType type)
		{
			switch (type)
			{
				case EventType.Entrée:
					return TimelineGlyph.FilledSquare;

				case EventType.Sortie:
					return TimelineGlyph.OutlinedSquare;

				case EventType.Modification:
					return TimelineGlyph.FilledCircle;

				case EventType.Augmentation:
					return TimelineGlyph.FilledUp;

				case EventType.Diminution:
					return TimelineGlyph.FilledDown;

				default:
					return TimelineGlyph.FilledCircle;
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
				if (cell.Value1.HasValue)
				{
					min = System.Math.Min (min, cell.Value1.Value);
					max = System.Math.Max (max, cell.Value1.Value);
				}

				if (cell.Value2.HasValue)
				{
					min = System.Math.Min (min, cell.Value2.Value);
					max = System.Math.Max (max, cell.Value2.Value);
				}
			}
		}

		public TimelineCell? GetSyntheticCell(int index)
		{
			var syntheticCell = new TimelineCell ();
			bool first = true;

			while (index >= 0)
			{
				var cell = this.GetCell (index--);

				if (cell.HasValue)
				{
					if (first)
					{
						syntheticCell.Timestamp     = cell.Value.Timestamp;
						syntheticCell.TimelineGlyph = cell.Value.TimelineGlyph;
						first = false;
					}

					if (!syntheticCell.Value1.HasValue && cell.Value.Value1.HasValue)
					{
						syntheticCell.Value1 = cell.Value.Value1;
					}

					if (!syntheticCell.Value2.HasValue && cell.Value.Value2.HasValue)
					{
						syntheticCell.Value2 = cell.Value.Value2;
					}

					if (syntheticCell.Value1.HasValue && syntheticCell.Value2.HasValue)
					{
						break;
					}
				}
			}

			return syntheticCell;
		}

		public TimelineCell? GetCell(int index)
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


		public struct TimelineCell
		{
			public Timestamp		Timestamp;
			public TimelineGlyph	TimelineGlyph;
			public decimal?			Value1;
			public decimal?			Value2;
		}


		private readonly DataAccessor accessor;
		private readonly List<TimelineCell> cells;
	}
}
