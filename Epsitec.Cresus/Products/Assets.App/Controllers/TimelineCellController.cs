//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Assets.App.Client;
using Epsitec.Cresus.Assets.App.Converters;

using Epsitec.Cresus.Assets.Core.Business;
using Epsitec.Cresus.Assets.Core.Collections;

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Epsitec.Cresus.Assets.App.Controllers
{
	public class TimelineCellController
	{
		public TimelineCellController(Widgets.Timeline widget, MockTimelineEventClient source)
		{
			this.widget = widget;
			this.source = source;
			this.cache  = new InfiniteCollection<TimelineEventCell> (this.source);

			this.converter = new TimelineCellConverter ();
			
			this.cache.CollectionChanged += this.HandleSourceCollectionChanged;
		}

		
		public SynchronizationContext			SynchronizationContext
		{
			get
			{
				return Epsitec.Common.Widgets.Application.SynchronizationContext;
			}
		}


		public void ClearCache()
		{
			this.cache.Clear ();
		}
		
		public void Refresh()
		{
			var cells = this.GetTimelineCells ();
			this.SetWidgetCells (cells);
		}


		private Widgets.TimelineCell[] GetTimelineCells()
		{
			var count = this.widget.VisibleCellCount;
			var cells = new Widgets.TimelineCell[count];
			var pivot = this.widget.BeforePivotCount;

			var cacheIndex = 0;
			var nextEvent  = this.cache[--cacheIndex];
			var nextDate   = this.source.Date.AddDays (-1);

			for (int i = 0; i < pivot; i++)
			{
				int index = pivot - i - 1;

				if (nextEvent == null)
				{
					cells[index] = new Widgets.TimelineCell (nextDate, Widgets.TimelineCellGlyph.Empty);
					nextDate = nextDate.AddDays (-1);
				}
				else
				{
					if (nextEvent.Date == nextDate)
					{
						cells[index] = this.converter.Convert (nextEvent);

						nextEvent = this.cache[--cacheIndex];

						if ((nextEvent == null) ||
							(nextEvent.Date != nextDate))
						{
							nextDate = nextDate.AddDays (-1);
						}
					}
					else
					{
						cells[index] = new Widgets.TimelineCell (nextDate, Widgets.TimelineCellGlyph.Empty);
						nextDate = nextDate.AddDays (-1);
					}
				}
			}

			cacheIndex = 0;
			nextEvent  = this.cache[cacheIndex++];
			nextDate   = this.source.Date;

			for (int i = pivot; i < count; i++)
			{
				int index = i;

				if (nextEvent == null)
				{
					cells[index] = new Widgets.TimelineCell (nextDate, Widgets.TimelineCellGlyph.Empty);
					nextDate = nextDate.AddDays (1);
				}
				else
				{
					if (nextEvent.Date == nextDate)
					{
						cells[index] = this.converter.Convert (nextEvent);

						nextEvent = this.cache[cacheIndex++];

						if ((nextEvent == null) ||
							(nextEvent.Date != nextDate))
						{
							nextDate = nextDate.AddDays (1);
						}
					}
					else
					{
						cells[index] = new Widgets.TimelineCell (nextDate, Widgets.TimelineCellGlyph.Empty);
						nextDate = nextDate.AddDays (1);
					}
				}
			}

			return cells;
		}


		private void HandleSourceCollectionChanged(object o, CollectionChangedEventArgs e)
		{
			this.Refresh ();
		}

		private void SetWidgetCells(Widgets.TimelineCell[] cells)
		{
			SendOrPostCallback callback =
				state =>
				{
					this.widget.SetCells (cells);
				};

			this.SynchronizationContext.Post (callback, null);
		}
		
		
		private readonly Widgets.Timeline		widget;
		private readonly InfiniteCollection<TimelineEventCell> cache;
		private readonly MockTimelineEventClient source;
		private readonly TimelineCellConverter	converter;
	}
}
