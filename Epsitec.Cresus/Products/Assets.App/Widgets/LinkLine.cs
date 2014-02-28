//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class LinkLine : Widget
	{
		public LinkLine()
		{
			this.topX = -1000;  // invisible (caché à gauche)
		}


		public int TopX
		{
			get
			{
				return this.topX;
			}
			set
			{
				if (this.topX != value)
				{
					this.topX = value;
					this.Invalidate ();
				}
			}
		}


		protected override void OnPaintBackground(PaintEventArgs e)
		{
			var rect = this.Client.Bounds;

			e.Graphics.AddFilledRectangle (rect.Left, rect.Bottom, rect.Width, 3);

			if (this.topX > -100)  // marque visible ?
			{
				e.Graphics.AddFilledPath (this.TrianglePath);
			}

			e.Graphics.RenderSolid (ColorManager.WindowBackgroundColor);
		}

		private Path TrianglePath
		{
			get
			{
				var path = new Path ();

				var rect = this.Client.Bounds;

				path.MoveTo (new Point (this.topX, rect.Top));
				path.LineTo (new Point (this.topX - rect.Height*0.6, rect.Bottom));
				path.LineTo (new Point (this.topX + rect.Height*0.6, rect.Bottom));
				path.Close ();

				return path;
			}
		}


		private int topX;
	}
}