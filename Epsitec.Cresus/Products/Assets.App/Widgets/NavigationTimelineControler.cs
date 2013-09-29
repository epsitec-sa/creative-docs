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
			this.minDate = new Date (System.DateTime.MinValue);
			this.maxDate = new Date (System.DateTime.MaxValue);
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
				Margins = new Margins (0, 0, 2, 0),
			};

			this.scroller.ValueChanged += delegate
			{
			};
		}

		public Timeline Timeline
		{
			get
			{
				return this.timeline;
			}
		}

		public Date MinDate
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

		public Date MaxDate
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
		}


		private Timeline timeline;
		private HScroller scroller;

		private Date minDate;
		private Date maxDate;
	}
}