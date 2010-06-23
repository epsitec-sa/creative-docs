//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets.Tools
{
	using Win32Api = Epsitec.Common.Widgets.Platform.Win32Api;
	
	/// <summary>
	/// The <c>MagnifierDragSource</c> class represents the widget which starts
	/// a magnified color picker.
	/// </summary>
	public class MagnifierDragSource : Widget, Behaviors.IDragBehaviorHost
	{
		public MagnifierDragSource()
		{
			this.color = Drawing.Color.Empty;
			this.dragBehavior = new Behaviors.DragBehavior (this);
		}

		public MagnifierDragSource(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Drawing.Color				HotColor
		{
			get
			{
				return this.color;
			}
		}

		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clipRect)
		{
			double dx = this.Client.Size.Width;
			double dy = this.Client.Size.Height;
			double cx = dx / 2;
			double cy = dy / 2;

			double r = System.Math.Min (cx, cy) - 1;

			Drawing.Color color1 = Drawing.Color.FromAlphaRgb (0.3, 0.3, 0.8, 1.0);
			Drawing.Color color2 = Drawing.Color.FromRgb (0, 0, 0.7);

			WidgetPaintState paintState = this.GetPaintState ();
			if ((paintState & WidgetPaintState.Enabled) == 0)
			{
				double bright = color1.GetBrightness ();

				color1 = Drawing.Color.FromAlphaRgb (color1.A, bright, bright, bright);
				color2 = Adorners.Factory.Active.ColorTextFieldBorder (false);
			}

			graphics.AddFilledCircle (cx, cy, r);
			graphics.RenderSolid (color1);
			graphics.LineWidth = 0.5;
			graphics.AddCircle (cx, cy, r);

			double sx = 5;
			double sy = 5;
			double ox = cx - sx/2 - 0.5;
			double oy = cy - sy/2 - 0.5;

			using (Drawing.Path path = new Drawing.Path ())
			{
				path.MoveTo (ox+2, oy+0);
				path.LineTo (ox+0, oy+0);
				path.LineTo (ox+0, oy+2);
				path.MoveTo (ox+0, oy+sy+1-2);
				path.LineTo (ox+0, oy+sy+1-0);
				path.LineTo (ox+2, oy+sy+1-0);
				path.MoveTo (ox+sx+1-2, oy+sy+1-0);
				path.LineTo (ox+sx+1-0, oy+sy+1-0);
				path.LineTo (ox+sx+1-0, oy+sy+1-2);
				path.MoveTo (ox+sx+1-0, oy+2);
				path.LineTo (ox+sx+1-0, oy+0);
				path.LineTo (ox+sx+1-2, oy+0);

				graphics.Rasterizer.AddOutline (path, 0.5);
			}

			graphics.RenderSolid (color2);
		}
		
		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if (! this.dragBehavior.ProcessMessage (message, pos))
			{
				base.ProcessMessage (message, pos);
			}
		}
		
		
		#region IDragBehaviorHost Members
		public Drawing.Point				DragLocation
		{
			get
			{
				Drawing.Point pos = this.MapClientToScreen (new Drawing.Point (this.Client.Size.Width / 2, this.Client.Size.Height / 2));
				
				pos.X = pos.X - this.magnifier.ZoomWindow.ClientSize.Width  / 2 - 2;
				pos.Y = pos.Y - this.magnifier.ZoomWindow.ClientSize.Height / 2 - 2;
				
				return pos;
			}
		}

		public bool OnDragBegin(Drawing.Point cursor)
		{
			if (this.magnifier == null)
			{
				this.magnifier = new Magnifier ();
				this.magnifier.IsColorPicker = true;
				this.magnifier.HotColorChanged += this.HandleMagnifierHotColorChanged;
			}
			
			MouseCursor.Hide ();

			this.magnifier.ZoomWindow.WindowLocation = this.DragLocation;
			this.magnifier.ZoomView.Invalidate ();
			this.magnifier.Show ();
			
			this.isSampling = true;
			
			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			this.magnifier.ZoomWindow.WindowLocation = this.DragLocation + e.Offset;
			this.magnifier.ZoomView.Invalidate ();
		}

		public void OnDragEnd()
		{
			this.isSampling = false;
			
			this.magnifier.Hide ();
			
			MouseCursor.Show ();
		}
		#endregion
		
		private void HandleMagnifierHotColorChanged(object sender)
		{
			if (this.isSampling)
			{
				this.color = this.magnifier.HotColor;
				this.OnHotColorChanged ();
			}
		}
		
		protected virtual void OnHotColorChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("HotColorChanged");
			if (handler != null)
			{
				handler(this);
			}
		}
		
		
		public event EventHandler			HotColorChanged
		{
			add
			{
				this.AddUserEventHandler("HotColorChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("HotColorChanged", value);
			}
		}
		
		
		private bool						isSampling;
		private Behaviors.DragBehavior		dragBehavior;
		private Magnifier					magnifier;
		private Drawing.Color				color;
	}
}
