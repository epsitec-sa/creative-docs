﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	public class CustomFrameBox : FrameBox
	{
		public CustomFrameBox()
		{
		}


		public bool ToComplete
		{
			//	Indique si le champ nécessite un complément avant de pouvoir créer l'écriture.
			get
			{
				return this.toComplete;
			}
			set
			{
				if (this.toComplete != value)
				{
					this.toComplete = value;
					this.Invalidate ();
				}
			}
		}

		public bool EmptyLineAdorner
		{
			//	Indique si le champ fait partie d'une ligne vide.
			get
			{
				return this.emptyLineAdorner;
			}
			set
			{
				if (this.emptyLineAdorner != value)
				{
					this.emptyLineAdorner = value;
					this.Invalidate ();
				}
			}
		}

		public bool BaseTVAAdorner
		{
			//	Dessine la première partie d'une flèche de haut en bas.
			get
			{
				return this.baseTVAAdorner;
			}
			set
			{
				if (this.baseTVAAdorner != value)
				{
					this.baseTVAAdorner = value;
					this.Invalidate ();
				}
			}
		}

		public bool CodeTVAAdorner
		{
			//	Dessine la seconde partie d'une flèche de haut en bas.
			get
			{
				return this.codeTVAAdorner;
			}
			set
			{
				if (this.codeTVAAdorner != value)
				{
					this.codeTVAAdorner = value;
					this.Invalidate ();
				}
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			if (this.BackColor.IsVisible)
			{
				graphics.AddFilledRectangle (this.Client.Bounds);
				graphics.RenderSolid (this.BackColor);
			}

			Rectangle rect = this.GetFrameRectangle ();

			if (this.toComplete)
			{
				//	Dessine un cadre vert pointillé.
				rect.Deflate (0.5);
				using (Path path = new Path (rect))
				{
					graphics.PaintDashedOutline (path, 2, 3, 5, CapStyle.Square, UIBuilder.FieldToCompleteColor);
				}
			}

			if (this.emptyLineAdorner)
			{
				//	Dessine des hachures grises translucides dans le fond.
				graphics.LineWidth = 7;

				for (double x = rect.Left-rect.Height; x < rect.Right; x+=20)
				{
					graphics.AddLine (x, rect.Bottom, x+rect.Height, rect.Top);
				}

				graphics.RenderSolid (UIBuilder.FieldEmptyLineColor);
				graphics.LineWidth = 1;
			}

			if (this.baseTVAAdorner)
			{
				//	Dessine la première partie d'une flèche de haut en bas.
				Point o = rect.BottomRight;
				double h = System.Math.Floor (rect.Height/2);

				graphics.AddLine (o.X-h*2, o.Y+h, o.X-h, o.Y+h);  // _
				graphics.AddLine (o.X-h, o.Y+h, o.X-h, o.Y);      //  |

				graphics.RenderSolid (adorner.ColorBorder);
			}

			if (this.codeTVAAdorner)
			{
				//	Dessine la seconde partie d'une flèche de haut en bas.
				Point o = rect.TopRight;
				double h = System.Math.Floor (rect.Height/2);
				double a = rect.Height*0.2;

				graphics.AddLine (o.X-h, o.Y, o.X-h, o.Y-h);            //   |
				graphics.AddLine (o.X-h, o.Y-h, o.X-h*2, o.Y-h);        // <-
				graphics.AddLine (o.X-h*2, o.Y-h, o.X-h*2+a, o.Y-h+a);  // 
				graphics.AddLine (o.X-h*2, o.Y-h, o.X-h*2+a, o.Y-h-a);  // 

				graphics.RenderSolid (adorner.ColorBorder);
			}
		}


		private bool			toComplete;
		private bool			emptyLineAdorner;
		private bool			baseTVAAdorner;
		private bool			codeTVAAdorner;
	}
}
