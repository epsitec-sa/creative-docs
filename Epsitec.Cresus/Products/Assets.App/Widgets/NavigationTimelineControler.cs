//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class NavigationTimelineControler
	{
		public NavigationTimelineControler()
		{
			this.minDate = System.DateTime.MinValue;
			this.maxDate = System.DateTime.MaxValue;
		}

		public void CreateUI(Widget parent)
		{
			this.timeline = new Timeline ()
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.scroller = new HScroller ()
			{
				Parent  = parent,
				Dock    = DockStyle.Bottom,
//				Margins = new Margins (0, 0, 2, 0),
			};

			this.timeline.SizeChanged += delegate
			{
				this.UpdateScroller ();
			};

			this.scroller.ValueChanged += delegate
			{
			};

			this.UpdateScroller ();
		}

		public Timeline							Timeline
		{
			get
			{
				return this.timeline;
			}
		}

		public System.DateTime					MinDate
		{
			get
			{
				return this.minDate;
			}
			set
			{
				if (this.minDate != value)
				{
					this.minDate = value;
					this.UpdateScroller ();
				}
			}
		}

		public System.DateTime					MaxDate
		{
			get
			{
				return this.maxDate;
			}
			set
			{
				if (this.maxDate != value)
				{
					this.maxDate = value;
					this.UpdateScroller ();
				}
			}
		}


		private void UpdateScroller()
		{
			if (this.timeline == null || this.scroller == null)
			{
				return;
			}

			this.scroller.MinValue = this.minDate.Ticks;
			this.scroller.MaxValue = this.maxDate.Ticks;

			var a = (decimal) (this.maxDate - this.minDate).Days;
			var b = (decimal) this.timeline.VisibleCellCount;

			this.scroller.VisibleRangeRatio = System.Math.Min (b/a, 1.0m);

			this.scroller.SmallChange = Time.TicksPerDay;
			this.scroller.LargeChange = Time.TicksPerDay * b;
		}


		private Timeline timeline;
		private HScroller scroller;

		private System.DateTime minDate;
		private System.DateTime maxDate;
	}
}