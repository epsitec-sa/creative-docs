//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Common;
using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Types;

namespace Epsitec.Common.Pdf.Labels
{
	public class LabelRenderer
	{
		public void Render(Port port, FormattedText text, Rectangle bounds, LabelPageLayout layout)
		{
			if (layout.ShouldPaintFrame)
			{
				using (var path = new Path ())
				{
					path.AppendRectangle (bounds);

					port.LineWidth = 1.0;  // épaisseur de 0.1mm
					port.Color = Color.FromBrightness (0.8);  // gris clair
					port.PaintOutline (path);
				}
			}

			bounds.Deflate (layout.LabelMargins);
			
			this.RenderLabel (port, text, bounds, layout);
		}
		
		protected virtual void RenderLabel(Port port, FormattedText text, Rectangle bounds, LabelPageLayout layout)
		{
			port.PaintText (bounds, text, layout.TextStyle);
		}
	}
}

