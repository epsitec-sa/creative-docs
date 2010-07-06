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


		public Printers.AbstractEntityPrinter EntityPrinter
		{
			get;
			set;
		}

		public AbstractEntity Entity
		{
			get;
			set;
		}



		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.EntityPrinter != null && this.Entity != null)
			{
				double sx = this.Client.Bounds.Width  / this.EntityPrinter.PageSize.Width;
				double sy = this.Client.Bounds.Height / this.EntityPrinter.PageSize.Height;
				double scale = System.Math.Min (sx, sy);

				Rectangle bounds = new Rectangle (0, 0, this.EntityPrinter.PageSize.Width*scale, this.EntityPrinter.PageSize.Height*scale);

				graphics.AddFilledRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (1));

				graphics.AddRectangle (bounds);
				graphics.RenderSolid (Color.FromBrightness (0));

				Transform initial = graphics.Transform;
				graphics.ScaleTransform (scale, scale, 0.0, 0.0);

				this.EntityPrinter.Print (graphics, this.Client.Bounds);

				graphics.Transform = initial;
			}
		}
	}
}
