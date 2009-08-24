//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Widgets
{
	public class CaptionView : FrameBox
	{
		public CaptionView()
		{
		}


		public CaptionPainter Captions
		{
			get
			{
				return this.captions;
			}
			set
			{
				if (this.captions != value)
				{
					this.captions = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (this.Client.Bounds);
			graphics.RenderSolid (Color.FromBrightness (1));

			if (this.captions != null)
			{
				this.captions.Render (graphics, this.Client.Bounds);
			}
		}


		private CaptionPainter captions;
	}
}
