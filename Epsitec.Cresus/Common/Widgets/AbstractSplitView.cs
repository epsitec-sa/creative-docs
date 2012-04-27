//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
				if (this.ratio != value)
				{
					this.ratio = value;
					this.UpdateRatio ();
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

		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateRatio ();
		}

		protected abstract void UpdateRatio();

		
		private double							ratio;
	}
}
