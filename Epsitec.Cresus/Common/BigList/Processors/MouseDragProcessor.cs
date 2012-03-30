//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Processors
{
	public sealed class MouseDragProcessor : EventProcessor
	{
		private MouseDragProcessor(IEventProcessorHost host, Message message, Point pos, IEnumerable<MouseDragFrame> frames)
		{
			this.host = host;
			this.draggingProcessor = this.host as IDraggingProcessor;

			this.policy = this.host.GetPolicy<MouseDragProcessorPolicy> ();
			this.button = message.Button;
			this.originalFrames = frames.Where (x => this.policy.Filter (x)).ToArray ();
			this.currentFrames = this.originalFrames.ToArray ();
			this.origin = this.Constrain (pos);
		}

		
		public static bool Attach(IEventProcessorHost host, Message message, Point pos, IEnumerable<MouseDragFrame> frames)
		{
			if (host.EventProcessors.OfType<MouseDragProcessor> ().Any ())
			{
				return false;
			}

			var proc = new MouseDragProcessor (host, message, pos, frames);

			if (proc.originalFrames.Length == 0)
			{
				return false;
			}

			proc.host.Register (proc);
			proc.Process (message, pos);

			return true;
		}

		
		protected override bool Process(Widgets.Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseDown:
					this.ProcessMove (pos);
					return true;

				case MessageType.MouseMove:
					this.ProcessMove (pos);
					return true;

				case MessageType.MouseUp:
					this.ProcessMove (pos);
					
					if (this.button == message.Button)
					{
						this.ProcessEnd ();
						this.host.Remove (this);
					}
					
					return true;
			}
			
			return false;
		}

		protected override void Paint(Graphics graphics, Rectangle clipRect)
		{
			using (var path = new Path ())
			{
				foreach (var rect in this.currentFrames)
				{
					if (clipRect.IntersectsWith (rect.Bounds))
					{
						path.AppendRectangle (rect.Bounds);
					}
				}

				if (path.IsEmpty)
				{
					return;
				}

				graphics.Color = Color.FromName ("Orange");
				
				graphics.LineWidth = 1.0;
				graphics.LineJoin  = JoinStyle.Miter;
				graphics.LineCap   = CapStyle.Square;
				
				graphics.PaintOutline (path);
			}
		}

		
		private void ProcessMove(Point pos)
		{
			var delta = this.Constrain (pos) - this.origin;

			for (int i = 0; i < this.originalFrames.Length; i++)
			{
				this.currentFrames[i] = MouseDragProcessor.ProcessDrag (this.originalFrames[i], delta);
			}
		}

		private void ProcessEnd()
		{
			if (this.draggingProcessor == null)
			{
				return;
			}

			for (int i = 0; i < this.currentFrames.Length; i++)
			{
				this.draggingProcessor.ApplyDrag (this.originalFrames[i], this.currentFrames[i]);
			}
		}

		private Point Constrain(Point pos)
		{
			foreach (var dragRectangle in this.originalFrames)
			{
				pos = MouseDragProcessor.ConstrainMove (dragRectangle, pos);
			}

			return pos;
		}


		private static Point ConstrainMove(MouseDragFrame dragRectangle, Point pos)
		{
			if (dragRectangle.Constraint.IsEmpty)
			{
				return pos;
			}
			else
			{
				return dragRectangle.Constraint.Constrain (pos);
			}
		}
		
		private static MouseDragFrame ProcessDrag(MouseDragFrame original, Point delta)
		{
			switch (original.Direction)
			{
				case MouseDragDirection.Horizontal:
					delta = new Point (delta.X, 0);
					break;

				case MouseDragDirection.Vertical:
					delta = new Point (0, delta.Y);
					break;

				case MouseDragDirection.None:
					delta = Point.Zero;
					break;

				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", original.Direction.GetQualifiedName ()));
			}

			var pos = original.Bounds.GetGrip (original.Grip) + delta;
			
			if (original.Constraint.IsValid)
			{
				pos = original.Constraint.Constrain (pos);
			}

			var bounds = Rectangle.DefineGrip (original.Bounds, original.Grip, pos);

			return new MouseDragFrame (original, bounds);
		}

		
		private readonly IEventProcessorHost host;
		private readonly IDraggingProcessor draggingProcessor;
		private readonly MouseDragProcessorPolicy policy;
		private readonly Point origin;
		private readonly MouseButtons button;
		private readonly MouseDragFrame[] originalFrames;
		private readonly MouseDragFrame[] currentFrames;
	}
}
