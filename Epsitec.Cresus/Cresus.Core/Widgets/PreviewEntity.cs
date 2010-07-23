//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	public class PreviewEntity : Widget
	{
		public PreviewEntity()
		{
		}

		public PreviewEntity(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public void BuildSections(Printers.AbstractEntityPrinter entityPrinter)
		{
			this.entityPrinter = entityPrinter;

			this.entityPrinter.IsPreview = true;
			this.entityPrinter.BuildSections ();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.entityPrinter != null)
			{
				double sx = this.Client.Bounds.Width  / this.entityPrinter.PageSize.Width;
				double sy = this.Client.Bounds.Height / this.entityPrinter.PageSize.Height;
				double scale = System.Math.Min (sx, sy);

				//	Dessine le fond d'une page blanche.
				Rectangle bounds = new Rectangle (0, 0, System.Math.Floor (this.entityPrinter.PageSize.Width*scale), System.Math.Floor (this.entityPrinter.PageSize.Height*scale));

				graphics.AddFilledRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (1));

				//	Dessine l'entité dans la page.
				Transform initial = graphics.Transform;
				graphics.ScaleTransform (scale, scale, 0.0, 0.0);

				this.entityPrinter.PrintCurrentPage (graphics);

				graphics.Transform = initial;

				//	Dessine le cadre de la page en dernier, pour recouvrir la page.
				bounds.Deflate (0.5);

				graphics.LineWidth = 1;
				graphics.AddRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}


		private Printers.AbstractEntityPrinter entityPrinter;
	}
}
