//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class GradientFrameBox : FrameBox
	{
		public GradientFrameBox()
		{
		}

		public GradientFrameBox(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public Color BackColor1
		{
			//	Si IsVerticalGradient = false, c'est la couleur de gauche
			//	Si IsVerticalGradient = true, c'est la couleur du bas
			get
			{
				return this.backColor1;
			}
			set
			{
				if (this.backColor1 != value)
				{
					this.backColor1 = value;
					this.Invalidate ();
				}
			}
		}

		public Color BackColor2
		{
			//	Si IsVerticalGradient = false, c'est la couleur de droite
			//	Si IsVerticalGradient = true, c'est la couleur du haut
			get
			{
				return this.backColor2;
			}
			set
			{
				if (this.backColor2 != value)
				{
					this.backColor2 = value;
					this.Invalidate ();
				}
			}
		}

		public bool IsVerticalGradient
		{
			get
			{
				return this.isVerticalGradient;
			}
			set
			{
				if (this.isVerticalGradient != value)
				{
					this.isVerticalGradient = value;
					this.Invalidate ();
				}
			}
		}

		public double LeftPercentOffset
		{
			get
			{
				return this.leftPercentOffset;
			}
			set
			{
				if (this.leftPercentOffset != value)
				{
					this.leftPercentOffset = value;
					this.Invalidate ();
				}
			}
		}

		public double RightPercentOffset
		{
			get
			{
				return this.rightPercentOffset;
			}
			set
			{
				if (this.rightPercentOffset != value)
				{
					this.rightPercentOffset = value;
					this.Invalidate ();
				}
			}
		}

		public double BottomPercentOffset
		{
			get
			{
				return this.bottomPercentOffset;
			}
			set
			{
				if (this.bottomPercentOffset != value)
				{
					this.bottomPercentOffset = value;
					this.Invalidate ();
				}
			}
		}

		public double TopPercentOffset
		{
			get
			{
				return this.topPercentOffset;
			}
			set
			{
				if (this.topPercentOffset != value)
				{
					this.topPercentOffset = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = this.Client.Bounds;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (this.backColor1);

			var p1 = new Point (rect.Left+rect.Width*this.leftPercentOffset, rect.Bottom+rect.Height*this.bottomPercentOffset);
			var p2 = new Point (rect.Right-rect.Width*this.rightPercentOffset, rect.Top-rect.Height*this.topPercentOffset);
			var gradientRect = new Rectangle (p1, p2);

			graphics.AddFilledRectangle (gradientRect);

			if (this.isVerticalGradient)
			{
				graphics.PaintVerticalGradient (gradientRect, this.backColor1, this.backColor2);
			}
			else
			{
				graphics.PaintHorizontalGradient (gradientRect, this.backColor1, this.backColor2);
			}
		}


		private Color			backColor1;
		private Color			backColor2;
		private bool			isVerticalGradient;
		private double			leftPercentOffset;
		private double			rightPercentOffset;
		private double			bottomPercentOffset;
		private double			topPercentOffset;
	}
}
