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
		public TimelineData(DataAccessor accessor)
		{
			this.accessor = accessor;
			this.cells = new List<TimelineCell> ();
		}


		public void Compute(Guid? objectGuid, bool compact, Timestamp start, int visibleCellCount)
		{
			this.cells.Clear ();

			if (!compact)
			{
				for (int i = 0; i < visibleCellCount; i++)
				{
					var date = TimelineData.AddDays (start.Date, i);

					var cell = new TimelineCell
					{
						Timestamp     = new Timestamp (date, 0),
						TimelineGlyph = TimelineGlyph.Empty,
					};

					this.cells.Add (cell);
				}
			}

			if (objectGuid.HasValue)
			{
				int eventCount = this.accessor.GetObjectEventsCount (objectGuid.Value);

				for (int i=0; i<eventCount; i++)
				{
					var t = this.accessor.GetObjectEventTimestamp (objectGuid.Value, i);
					if (t.HasValue && t.Value >= start)
					{
						var index = this.cells.FindIndex (x => x.Timestamp == t.Value);
						if (index == -1)
						{
							index = this.GetIndex (t.Value);

							var cell = new TimelineCell
							{
								Timestamp     = t.Value,
								TimelineGlyph = TimelineGlyph.FilledCircle,
							};

							this.cells.Insert (index, cell);
						}
						else
						{
							this.cells[index].TimelineGlyph = TimelineGlyph.FilledCircle;
						}
					}
				}
			}
		}

		private int GetIndex(Timestamp timestamp)
		{
			int i = 0;
			while (i < this.cells.Count)
			{
				if (this.cells[i].Timestamp > timestamp)
				{
					i--;
					break;
				}

				i++;
			}

			return i;
		}


		public TimelineCell GetCell(int index)
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


		public class TimelineCell
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
