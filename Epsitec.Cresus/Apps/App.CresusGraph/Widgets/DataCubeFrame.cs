//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class DataCubeFrame : FrameBox
	{
		public DataCubeFrame()
		{
			this.AutoFocus = false;
			Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure (this, true);
		}


		public double FrameWidth
		{
			get;
			set;
		}

		public double FrameHeight
		{
			get;
			set;
		}

		public void UpdateGeometry()
		{
			var parent = this.Parent;

			if ((parent == null) ||
				(parent.Window == null))
			{
				return;
			}

			if (!parent.IsActualGeometryValid)
			{
				parent.Window.ForceLayout ();
			}
			
			var button = this.FindDataCubeButton ();

			if (button == null)
			{
				return;
			}
			
			var cubeButtonBounds  = button.MapClientToRoot (button.Client.Bounds);
			var parentFrameBounds = parent.MapClientToRoot (this.Parent.Client.Bounds);

			double xLeft  = cubeButtonBounds.Left+2 - parentFrameBounds.Left;
			double xRight = parentFrameBounds.Width - cubeButtonBounds.Right-2;

			double top   = button.ActualHeight + button.Parent.Padding.Height + button.Parent.Margins.Height - 4;
			double left  = xLeft;
			double right = xRight;
			double width = cubeButtonBounds.Width-4 + 2*16;

			if (width < this.FrameWidth)
            {
				left  -= System.Math.Floor ((this.FrameWidth-width) / 2);
				right -= System.Math.Ceiling ((this.FrameWidth-width) / 2);
            }

			if (right < 4)
			{
				left += (right-4);
				right = 4;
			}
			else if (left < 4)
			{
				right += (left-4);
				left = 4;
			}

			if (right > xRight-16)
			{
				left -= right - xRight;
				right = xRight+2;
			}
			if (left > xLeft-16)
            {
				right -= left - xLeft;
				left   = xLeft+2;
            }

			System.Diagnostics.Debug.WriteLine (string.Format ("l={0} r={1}", left, right));

			this.Margins = new Margins (left, right, top, 0);
			this.PreferredWidth = this.FrameWidth; // parentFrameBounds.Width - left - right;
			this.PreferredHeight = this.FrameHeight;
		}

		protected override void OnParentChanged(Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnParentChanged (e);

			Widget oldParent = (Widget) e.OldValue;
			Widget newParent = (Widget) e.NewValue;

			if (oldParent != null)
            {
				oldParent.SizeChanged -= this.HandleParentSizeChanged;
            }
			if (newParent != null)
			{
				newParent.SizeChanged += this.HandleParentSizeChanged;
				this.UpdateGeometry ();
			}
		}

		private void HandleParentSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateGeometry ();
		}

		public override Margins GetInternalPadding()
		{
			return new Margins (3, 3, 5+2, 3+2);	//	top and bottom +2 pixels because of rounded corner radius
		}
        
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("inside: {0}", Rectangle.Deflate (this.Client.Bounds, this.GetInternalPadding ())));

			Widget button = this.FindDataCubeButton();

			if (button == null)
			{
				return;
			}

			var cubeButtonBounds = button.MapClientToRoot (button.Client.Bounds);
			var frameBounds      = this.ActualBounds;

			double x1 = cubeButtonBounds.Left+2 - frameBounds.Left;
			double x2 = cubeButtonBounds.Right-2 - frameBounds.Left;

			var frame   = this.MapRootToClient (cubeButtonBounds);
			var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			double r  = 4;
			var rect  = Rectangle.Deflate (this.Client.Bounds, new Margins (2, 2, r, 2));
			double dx = rect.Width;
			double dy = rect.Height - 2;

			using (Path path = new Path ())
			{
				path.MoveTo (x2-0.5, rect.Top+r);

				double rr = rect.Right;

				if (rr < x2+2*r)
				{
					rr = x2;
				}
				else
				{
					path.ArcTo (x2-0.5, rect.Top-0.5, x2-0.5+r, rect.Top-0.5);
					path.LineTo (rr-0.5-r, rect.Top-0.5);
					path.ArcTo (rr-0.5, rect.Top-0.5, rr-0.5, rect.Top-0.5-r);
				}

				path.LineTo (rr-0.5, rect.Bottom+0.5+r);
				path.ArcTo (rr-0.5, rect.Bottom+0.5, rr-0.5-r, rect.Bottom+0.5);
				path.LineTo (rect.Left+0.5+r, rect.Bottom+0.5);
				path.ArcTo (rect.Left+0.5, rect.Bottom+0.5, rect.Left+0.5, rect.Bottom+0.5+r);
				path.LineTo (rect.Left+0.5, rect.Top-0.5-r);
				path.ArcTo (rect.Left+0.5, rect.Top-0.5, rect.Left+0.5+r, rect.Top-0.5);
				path.LineTo (x1-r, rect.Top-0.5);
				path.ArcTo (x1, rect.Top-0.5, x1, rect.Top+r);

				graphics.Rasterizer.AddOutline (path, 5, CapStyle.Butt, JoinStyle.Round);
				graphics.RenderSolid (Color.FromAlphaRgb (0.1, 0, 0.0, 0.0));
				
				graphics.Rasterizer.AddOutline (path, 3, CapStyle.Butt, JoinStyle.Round);
				graphics.RenderSolid (Color.FromAlphaRgb (0.1, 0, 0.0, 0.0));
				
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (Color.FromBrightness (1));

				graphics.Rasterizer.AddOutline (path, 1, CapStyle.Butt, JoinStyle.Round);
				graphics.RenderSolid (adorner.ColorBorder);
			}
		}
		
		public Widget FindDataCubeButton()
		{
			return this.Window.Root.FindAllChildren (w => w is DataCubeButton).FirstOrDefault ();
		}
	}
}
