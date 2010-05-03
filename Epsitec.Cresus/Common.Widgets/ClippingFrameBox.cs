//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

[assembly: DependencyClass (typeof (ClippingFrameBox))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ClippingFrameBox</c> class ...
	/// </summary>
	public class ClippingFrameBox : FrameBox
	{
		public ClippingFrameBox()
		{
		}

		public ClippingFrameBox(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		protected override void MeasureMinMax(ref Size min, ref Size max)
		{
			Size oldMin = min;
			Size oldMax = max;
			
			base.MeasureMinMax (ref min, ref max);

			min = new Size (oldMin.Width, min.Height);
			max = new Size (oldMax.Width, max.Height);
		}
	}
}
