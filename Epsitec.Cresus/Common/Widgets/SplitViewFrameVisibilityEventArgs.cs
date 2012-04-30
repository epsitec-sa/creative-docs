//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	public class SplitViewFrameVisibilityEventArgs : Epsitec.Common.Support.EventArgs
	{
		public SplitViewFrameVisibilityEventArgs(SplitViewFrameVisibility oldValue, SplitViewFrameVisibility newValue)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		public SplitViewFrameVisibility OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public SplitViewFrameVisibility NewValue
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
