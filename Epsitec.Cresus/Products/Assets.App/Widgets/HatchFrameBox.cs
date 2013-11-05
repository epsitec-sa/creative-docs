//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class HatchFrameBox : FrameBox
	{
		public bool								Hatch
		{
			get
			{
				return this.hatch;
			}
			set
			{
				if (this.hatch != value)
				{
					this.hatch = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Common.Drawing.Graphics graphics, Common.Drawing.Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.hatch)
			{
				var rect = new Rectangle (0, 0, this.ActualWidth, this.ActualHeight);
				PaintHatch.Paint (graphics, rect);
			}
		}


		private bool hatch;
	}
}
