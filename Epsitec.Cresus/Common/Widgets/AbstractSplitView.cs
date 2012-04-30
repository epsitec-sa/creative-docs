//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
namespace Epsitec.Common.Widgets
{
	public abstract class AbstractSplitView : Widget
	{
		protected AbstractSplitView()
		{
		}

		
		public double							Ratio
		{
			get
			{
				return this.ratio;
			}
			set
			{
				if (value < 0)
				{
					value = 0;
				}
				if (value > 1)
				{
					value = 1;
				}

				if (this.ratio != value)
				{
					this.ratio = value;
					this.UpdateRatioAndNotify ();
				}
			}
		}

		public abstract Widget					Frame1
		{
			get;
		}

		public abstract Widget					Frame2
		{
			get;
		}

		public abstract AbstractScroller		Scroller1
		{
			get;
		}

		public abstract AbstractScroller		Scroller2
		{
			get;
		}

		public abstract SplitViewFrameVisibility FrameVisibility
		{
			get;
		}


		protected void UpdateRatioAndNotify()
		{
			var oldValue = this.FrameVisibility;

			this.UpdateRatio ();

			var newValue = this.FrameVisibility;

			if (oldValue != newValue)
			{
				this.OnFrameVisibilityChanged (new SplitViewFrameVisibilityEventArgs (oldValue, newValue));
			}
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateRatioAndNotify ();
		}

		protected abstract void UpdateRatio();

		protected virtual void OnFrameVisibilityChanged(SplitViewFrameVisibilityEventArgs e)
		{
			this.RaiseUserEvent<SplitViewFrameVisibilityEventArgs> (EventNames.FrameVisibilityChanged, e);
		}

		
		#region EventNames Class

		private new static class EventNames
		{
			public const string FrameVisibilityChanged = "FrameVisibilityChanged";
		}

		#endregion

		
		public event EventHandler<SplitViewFrameVisibilityEventArgs> FrameVisibilityChanged
		{
			add
			{
				this.AddUserEventHandler (EventNames.FrameVisibilityChanged, value);
			}
			remove
			{
				this.RemoveUserEventHandler (EventNames.FrameVisibilityChanged, value);
			}
		}


		private double							ratio;
	}
}