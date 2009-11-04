//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class DragController
	{
		public DragController(Widget widget, Point mouseOrigin)
		{
			this.widget = widget;
			this.widgetOrigin = this.widget.MapClientToScreen (Point.Zero);
			this.mouseOrigin = this.widget.MapClientToScreen (mouseOrigin);

			this.widget.MouseMove +=
				(sender, e) =>
				{
					this.ProcessMouseMove (e.Point,
						delegate
						{
							this.widget.Enable = false;
							this.widget.MouseCursor = MouseCursor.AsHand;
						});

					e.Suppress = true;
				};
		}


		public bool LockX
		{
			get;
			set;
		}
		
		public bool LockY
		{
			get;
			set;
		}

		public static void Attach(Widget widget)
		{
			DragController drag = null;

			widget.Pressed +=
				(sender, e) =>
				{
					switch (e.Message.Button)
					{
						case MouseButtons.Left:
							drag = new DragController (widget, e.Point);
//							drag.DefineMouseMoveBehaviour (MouseCursor.AsHand);
							break;

						case MouseButtons.Right:
//							this.ShowInputContextMenu (cube, widget, e.Point);
							break;
					}

					Window.SetCaptureAndRetireMessage (widget, e.Message);
				};

			widget.Released +=
				(sender, e) =>
				{
					if ((drag != null) &&
						(drag.ProcessDragEnd () == false))
					{
						//	...
					}

					drag = null;
				};
		}

        private void CreateDragWindow()
		{
			this.dragWindow = new DragWindow ()
			{
				Alpha = 0.6,
				WindowLocation = this.widgetOrigin,
				Owner = this.widget.Window,
			};

			this.clone = new CloneView ()
						{
							Model = this.widget,
						};

			this.dragWindow.DefineWidget (this.clone, this.widget.ActualSize, Margins.Zero);
			this.dragWindow.Show ();
		}

		private bool ProcessMouseMove(Point mouse, System.Action dragStartAction)
		{
			mouse = this.widget.MapClientToScreen (mouse);
			
			if (this.dragWindow == null)
			{
				if (Point.Distance (mouse, this.mouseOrigin) > 2.0)
				{
					this.CreateDragWindow ();
					dragStartAction ();
				}
			}

			if (this.dragWindow != null)
			{
				double x = this.widgetOrigin.X + (this.LockX ? 0 : mouse.X - this.mouseOrigin.X);
				double y = this.widgetOrigin.Y + (this.LockY ? 0 : mouse.Y - this.mouseOrigin.Y);

				this.dragWindow.WindowLocation = new Point (x, y);
				
				//	...
			}
			
			return false;
		}

		public bool ProcessDragEnd()
		{
			bool ok = this.dragWindow != null;

			if (this.dragWindow != null)
			{
				this.dragWindow.Dispose ();
				this.dragWindow = null;
			}

			#if false
			if (this.outputInsertionMark != null)
			{
				this.outputInsertionMark.Dispose ();
				this.outputInsertionMark = null;
			}
			#endif

			this.widget.Enable = true;
			this.widget.MouseCursor = null;
			this.widget.ClearUserEventHandlers (Widget.EventNames.MouseMoveEvent);

			return ok;
		}

		private Widget widget;
		private Point mouseOrigin;
		private Point widgetOrigin;
		private DragWindow dragWindow;
		private CloneView clone;
	}
}
