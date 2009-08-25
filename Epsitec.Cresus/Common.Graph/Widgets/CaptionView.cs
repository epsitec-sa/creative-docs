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


		/// <summary>
		/// Finds the index of the caption at the specified position.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>The caption index or <c>-1</c>.</returns>
		public int FindCaptionIndex(Point pos)
		{
			if (this.captions != null)
			{
				return this.captions.DetectCaption (Rectangle.Deflate (this.Client.Bounds, this.Padding), pos);
			}
			else
			{
				return -1;
			}
		}
		

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (this.Client.Bounds);
			graphics.RenderSolid (Color.FromBrightness (1));

			if (this.captions != null)
			{
				this.captions.Render (graphics, Rectangle.Deflate (this.Client.Bounds, this.Padding));
			}
		}


		private CaptionPainter captions;
	}
}
