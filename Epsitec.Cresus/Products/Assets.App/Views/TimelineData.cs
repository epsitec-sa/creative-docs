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
					var date = TimelineData.AddDays (start.Date, i);

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

						if (index == -1)
						{
							index = this.GetIndex (t.Value);

							var cell = new TimelineCell
							{
								Timestamp     = t.Value,
								TimelineGlyph = glyph,
							};

							this.cells.Insert (index, cell);
						}
						else
						{
							var cell = new TimelineCell
							{
								Timestamp     = this.cells[index].Timestamp,
								TimelineGlyph = glyph,
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
		}



		private static System.DateTime AddDays(System.DateTime date, int numberOfDays)
		{
			return new Date (date.Ticks + Time.TicksPerDay*numberOfDays).ToDateTime ();
		}


		private readonly DataAccessor accessor;
		private readonly List<TimelineCell> cells;
	}
}
