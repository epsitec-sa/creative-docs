//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets.Behaviors;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class AnchoredPalette : FrameBox, IDragBehaviorHost
	{
		public AnchoredPalette()
		{
			this.dragBehavior = new DragBehavior (this);
			this.AutoCapture = true;
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.dragBehavior.ProcessMessage (message, pos))
			{
				//	eat the event
			}
			else
			{
				base.ProcessMessage (message, pos);
			}
		}

		#region IDragBehaviorHost Members

		public Point DragLocation
		{
			get
			{
				return new Point (this.Margins.Left, this.Margins.Bottom);
			}
		}

		public bool OnDragBegin(Point cursor)
		{
			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			double x = this.Margins.Left;
			double y = this.Margins.Bottom;

			this.Margins = new Margins (e.Offset.X + x, 0, 0, e.Offset.Y + y);
			this.Invalidate ();
			this.Window.SynchronousRepaint ();
		}

		public void OnDragEnd()
		{
		}

		#endregion


		private readonly DragBehavior dragBehavior;
	}
}
