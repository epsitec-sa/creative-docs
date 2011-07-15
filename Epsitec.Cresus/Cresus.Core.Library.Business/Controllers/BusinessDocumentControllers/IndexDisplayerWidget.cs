//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class IndexDisplayerWidget : Widget
	{
		public IndexDisplayerWidget()
		{
			this.colors = new List<Color> ();
		}

		public IndexDisplayerWidget(Widget embedder)
			: this ()
		{
			this.SetEmbedder(embedder);
		}


		public List<Color> Colors
		{
			get
			{
				return this.colors;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			int bandWidth = (int) rect.Width / IndexDisplayerWidget.maxDeep;

			for (int i = 0; i < colors.Count; i++)
			{
				Rectangle band = new Rectangle (rect.Left+bandWidth*i, rect.Bottom, rect.Width-bandWidth*i, rect.Height);

				graphics.AddFilledRectangle (band);
				graphics.RenderSolid (this.colors[i]);
			}

			for (int i = 1; i < colors.Count; i++)
			{
				Rectangle band = new Rectangle (rect.Left+bandWidth*i, rect.Bottom, rect.Width-bandWidth*i, rect.Height);

				graphics.AddLine (band.Left+0.5, band.Bottom, band.Left+0.5, band.Top);
				graphics.RenderSolid (adorner.ColorBorder);
			}
		}


		private static readonly int maxDeep = 4;

		private readonly List<Color>			colors;
	}
}
