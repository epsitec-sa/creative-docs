//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class DragController
	{
		public DragController(Widget widget, Point mouseOrigin, System.Action<DragController, Point> dragAction)
		{
			this.widget = widget;
			this.widgetOrigin = this.widget.MapClientToScreen (Point.Zero);
			this.mouseOrigin = this.widget.MapClientToScreen (mouseOrigin);
			this.dragAction = dragAction;

			this.widget.MouseMove +=
				(sender, e) =>
				{
					this.ProcessMouseMove (e.Point,
						delegate
						{
							this.widget.Enable = false;
							this.widget.MouseCursor = MouseCursor.AsHand;
						});

					e.Cancel = true;
				};

			this.widget.SetEngaged (true);
		}

		public Widget Widget
		{
			get
			{
				return this.widget;
			}
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

		public Margins WindowPadding
		{
			get;
			set;
		}

		public static void Attach(Widget widget,
			System.Action<DragController> beginAction,
			System.Action<DragController, Point> dragAction,
			System.Action<DragController, bool> endAction)
		{
			DragController drag = null;

			widget.Pressed +=
				(sender, e) =>
				{
					switch (e.Message.Button)
					{
						case MouseButtons.Left:
							drag = new DragController (widget, e.Point, dragAction);
							if (beginAction != null)
							{
								beginAction (drag);
							}
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
					if (drag != null)
					{
						bool moved = drag.ProcessDragEnd ();

						if (endAction != null)
						{
							endAction (drag, moved);
						}
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

			this.dragWindow.DefineWidget (this.clone, this.widget.ActualSize, this.WindowPadding);
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
				
				if (this.dragAction != null)
                {
					this.dragAction (this, mouse);
                }
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
			this.widget.SetEngaged (false);

			return ok;
		}

		private readonly Widget widget;
		private readonly Point mouseOrigin;
		private readonly Point widgetOrigin;
		private readonly System.Action<DragController, Point> dragAction;
		private DragWindow dragWindow;
		private CloneView clone;
	}
}
