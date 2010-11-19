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
	/// <summary>
	/// Ce widget montre le contenu d'une page imprimée dans une zone rectangulaire.
	/// </summary>
	public class ContinuousPagePreviewer : Widget
	{
		public ContinuousPagePreviewer()
		{
		}


		public Printers.AbstractDocumentPrinter DocumentPrinter
		{
			get
			{
				return this.documentPrinter;
			}
			set
			{
				this.documentPrinter = value;
			}
		}

		public double ContinuousHeight
		{
			get
			{
				return (Printers.AbstractDocumentPrinter.continuousHeight - this.documentPrinter.ContinuousVerticalMax) * this.ContinuousScale;
			}
		}

		public double ContinuousVerticalOffset
		{
			get
			{
				return this.continuousVerticalOffset;
			}
			set
			{
				if (this.continuousVerticalOffset != value)
				{
					this.continuousVerticalOffset = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.documentPrinter != null)
			{
				var clientBounds = this.Client.Bounds;

				double scale = this.ContinuousScale;
				double offsetX = 0;
				double offsetY = this.continuousVerticalOffset - Printers.AbstractDocumentPrinter.continuousHeight*scale + clientBounds.Height;

				//	Dessine le fond d'une page blanche.
				Rectangle bounds = new Rectangle (offsetX, offsetY, System.Math.Floor (this.documentPrinter.RequiredPageSize.Width*scale), System.Math.Floor (this.documentPrinter.RequiredPageSize.Height*scale));

				graphics.AddFilledRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (1));  // fond blanc

				//	Dessine l'entité dans la page.
				Transform initial = graphics.Transform;
				graphics.TranslateTransform (offsetX, offsetY);
				graphics.ScaleTransform (scale, scale, 0.0, 0.0);

				this.documentPrinter.CurrentPage = 0;
				this.documentPrinter.PrintBackgroundCurrentPage (graphics);
				this.documentPrinter.PrintForegroundCurrentPage (graphics);

				graphics.Transform = initial;

				//	Dessine le cadre de la page en dernier, pour recouvrir la page.
				bounds.Deflate (0.5);

				graphics.LineWidth = 1;
				graphics.AddRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (0));
			}
		}

		private double ContinuousScale
		{
			get
			{
				return this.Client.Bounds.Width / this.documentPrinter.RequiredPageSize.Width;
			}
		}


		private Printers.AbstractDocumentPrinter	documentPrinter;
		private double								continuousVerticalOffset;
	}
}
