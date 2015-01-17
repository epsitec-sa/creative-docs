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
			
			//?this.cache.CollectionChanged += this.HandleSourceCollectionChanged;
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
			//?var cells = this.GetTimelineCells ();
			//?this.SetWidgetCells (cells);
		}


#if false
		private Widgets.TimelineCell[] GetTimelineCells()
		{
			var pivot = this.widget.BeforePivotCount;
			var count = this.widget.VisibleCellCount;
			
			var cells = new Widgets.TimelineCell[count];

			this.GeneratePastCells (cells, pivot);
			this.GenerateFutureCells (cells, pivot);

			return cells;
		}


		private void GeneratePastCells(Widgets.TimelineCell[] cells, int pivot)
		{
			int cacheIndex = 0;
			var nextEvent  = this.cache[--cacheIndex];
			var nextDate   = this.source.Date.AddDays (-1);

			for (int i = 0; i < pivot; i++)
			{
				int index = pivot - i - 1;

				if ((nextEvent != null) &&
					(nextEvent.Date == nextDate))
				{
					cells[index] = this.CreateEventTimelineCell (nextEvent);

					nextEvent = this.cache[--cacheIndex];

					if ((nextEvent != null) &&
						(nextEvent.Date == nextDate))
					{
						continue;
					}
				}
				else
				{
					cells[index] = this.CreateEmptyTimelineCell (nextDate);
				}
				
				nextDate = nextDate.AddDays (-1);
			}
		}
		
		private void GenerateFutureCells(Widgets.TimelineCell[] cells, int pivot)
		{
			int cacheIndex = 0;
			var nextEvent  = this.cache[cacheIndex++];
			var nextDate   = this.source.Date;

			for (int index = pivot; index < cells.Length; index++)
			{
				if ((nextEvent != null) &&
					(nextEvent.Date == nextDate))
				{
					cells[index] = this.CreateEventTimelineCell (nextEvent);

					nextEvent = this.cache[cacheIndex++];

					if ((nextEvent != null) &&
						(nextEvent.Date == nextDate))
					{
						continue;
					}
				}
				else
				{
					cells[index] = this.CreateEmptyTimelineCell (nextDate);
				}
				
				nextDate = nextDate.AddDays (1);
			}
		}

		private Epsitec.Cresus.Assets.App.Widgets.TimelineCell CreateEventTimelineCell(TimelineEventCell ev)
		{
			return this.converter.Convert (ev);
		}

		private Epsitec.Cresus.Assets.App.Widgets.TimelineCell CreateEmptyTimelineCell(Date date)
		{
			return new Widgets.TimelineCell (date, Widgets.TimelineGlyph.Empty);
		}

		private void HandleSourceCollectionChanged(object o, CollectionChangedEventArgs e)
		{
			this.Refresh ();
		}

		private void SetWidgetCells(Widgets.TimelineCell[] cells)
		{
			this.SynchronizationContext.Post (this.SetWidgetCellsCallback, cells);
		}

		private void SetWidgetCellsCallback(object state)
		{
			Widgets.TimelineCell[] cells = (Widgets.TimelineCell[]) state;
			this.widget.SetCells (cells);
		}
#endif
		
		
		private readonly Widgets.Timeline		widget;
		private readonly InfiniteCollection<TimelineEventCell> cache;
		private readonly MockTimelineEventClient source;
		private readonly TimelineCellConverter	converter;
	}
}
