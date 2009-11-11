//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.UI;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class DataCubeTableDetails : FrameBox
	{
		public DataCubeTableDetails()
		{
			this.CreateUI ();
		}

		public GraphDataCube Cube
		{
			get
			{
				return this.cube;
			}
			set
			{
				if (this.cube != value)
				{
					this.cube = value;
					this.InvalidateUI ();
				}
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = this.Client.Bounds;

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Color.FromBrightness (1));
			
			var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			var border  = adorner.ColorBorder;

			graphics.AddFilledRectangle (0, rect.Height-1, rect.Width, 1);
			graphics.AddFilledRectangle (0, 0, rect.Width, 1);
			graphics.RenderSolid (border);
		}
		
		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintForegroundImplementation (graphics, clipRect);
		}


		private void InvalidateUI()
		{
			this.DeleteUI ();
			this.CreateUI ();
		}


		private void CreateUI()
		{
		}

		private void DeleteUI()
		{
			this.Children.Widgets.ForEach (widget => widget.Dispose ());
		}


		private GraphDataCube cube;
	}
}
