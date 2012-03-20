//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		public bool EmptyLine
		{
			//	Indique si le champ fait partie d'une ligne vide.
			get
			{
				return this.emptyLine;
			}
			set
			{
				if (this.emptyLine != value)
				{
					this.emptyLine = value;
					this.Invalidate ();
				}
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
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

			if (this.emptyLine)
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
		}


		private bool			toComplete;
		private bool			emptyLine;
	}
}
