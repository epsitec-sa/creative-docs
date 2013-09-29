//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ce contrôleur inclut une Timeline et un ascenseur horizontal permettant la
	/// navigation entre deux bornes spécifiées sous forme de dates.
	/// </summary>
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
				this.OnDateChanged ();
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

		public System.DateTime					Date
		{
			get
			{
				return new System.DateTime ((long) this.scroller.Value);
			}
		}


		private void UpdateScroller()
		{
			if (this.timeline == null || this.scroller == null)
			{
				return;
			}

			var totalCell    = (decimal) (this.maxDate - this.minDate).Days + 1;
			var visibleCell  = (decimal) this.timeline.VisibleCellCount;
			var visibleTicks = (decimal) Time.TicksPerDay * (visibleCell-1);

			this.scroller.Resolution = (decimal) Time.TicksPerDay;
			this.scroller.VisibleRangeRatio = System.Math.Min (visibleCell/totalCell, 1.0m);

			this.scroller.MinValue = (decimal) this.minDate.Ticks;
			this.scroller.MaxValue = System.Math.Max ((decimal) this.maxDate.Ticks - visibleTicks, this.scroller.MinValue);

			this.scroller.SmallChange = (decimal) Time.TicksPerDay;
			this.scroller.LargeChange = visibleTicks;

			this.OnDateChanged ();  // met à jour la timeline
		}


		#region Events handler
		private void OnDateChanged()
		{
			if (this.DateChanged != null)
			{
				this.DateChanged (this);
			}
		}

		public delegate void DateChangedEventHandler(object sender);
		public event DateChangedEventHandler DateChanged;
		#endregion


		private Timeline timeline;
		private HScroller scroller;

		private System.DateTime minDate;
		private System.DateTime maxDate;
	}
}