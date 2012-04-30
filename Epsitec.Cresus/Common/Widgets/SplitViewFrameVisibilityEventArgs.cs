//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>SplitViewFrameVisibilityEventArgs</c> class provides the old and new value for the
	/// <see cref="AbstractSplitView.FrameVisibility"/> property.
	/// </summary>
	public class SplitViewFrameVisibilityEventArgs : Epsitec.Common.Support.EventArgs
	{
		public SplitViewFrameVisibilityEventArgs(SplitViewFrameVisibility oldValue, SplitViewFrameVisibility newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		
		public SplitViewFrameVisibility			OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public SplitViewFrameVisibility			NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		
		private readonly SplitViewFrameVisibility oldValue;
		private readonly SplitViewFrameVisibility newValue;
	}
}
