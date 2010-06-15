//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>TileContainer</c> contains a stack of tiles. It paints a frame which
	/// takes into account the tile arrows.
	/// </summary>
	public class TileContainer : FrameBox
	{
		public TileContainer()
		{
			this.TabNavigationMode = Common.Widgets.TabNavigationMode.ForwardTabActive | Common.Widgets.TabNavigationMode.ForwardToChildren;
			this.TabIndex = 1;
		}


		public CoreViewController Controller
		{
			get;
			set;
		}

		protected override void MeasureMinMax(ref Size min, ref Size max)
		{
			//-base.MeasureMinMax (ref min, ref max);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var adorner = Common.Widgets.Adorners.Factory.Active;
			var margins = new Margins (0.5, TileArrow.Breadth + 0.5, 0.5, 0.5);
			var frame   = Rectangle.Deflate (this.Client.Bounds, margins);

			graphics.AddRectangle (frame);
			graphics.RenderSolid (adorner.ColorBorder);
		}

		protected override Widget TabNavigate(int index, TabNavigationDir dir, Widget[] siblings)
		{
			if ((dir == TabNavigationDir.Backwards) ||
				(dir == TabNavigationDir.Forwards))
            {
				var e = new TabNavigateEventArgs (dir);
				var window = this.Window;

				this.OnTabNavigating (e);

				if (e.Cancel)
				{
					//	If the navigation was canceled, this implies that the currently focused
					//	widget has to be kept focused (it might have been focused manually in the
					//	process of handling the TabNavigating event).
					
					return window.FocusedWidget ?? this;
				}

				var result = e.ReplacementWidget;

				if (result != null)
				{
					return result;
				}
            }
			
			return base.TabNavigate (index, dir, siblings);
		}


		protected virtual void OnTabNavigating(TabNavigateEventArgs e)
		{
			var handler = this.TabNavigating;

			if (handler != null)
			{
				handler (this, e);
			}
		}

		public event EventHandler<TabNavigateEventArgs>		TabNavigating;
	}
}