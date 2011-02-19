//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets.Behaviors;

namespace Epsitec.Cresus.Graph.Widgets
{
	/// <summary>
	/// The <c>AnchoredPalette</c> class can be used to implement a palette which
	/// is constrained within a parent widget, but can be dragged around by the
	/// user. When the parent gets resized, the palette follows the borders which
	/// are the nearest to its position.
	/// </summary>
	public class AnchoredPalette : FrameBox, IDragBehaviorHost
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AnchoredPalette"/> class.
		/// </summary>
		public AnchoredPalette()
		{
			this.dragBehavior = new DragBehavior (this);
			this.AutoCapture = true;
			this.DrawFullFrame = true;
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
				double x = (this.Anchor & AnchorStyles.Left) != 0 ? this.Margins.Left : this.Margins.Right;
				double y = (this.Anchor & AnchorStyles.Bottom) != 0 ? this.Margins.Bottom : this.Margins.Top;

				return new Point (this.Margins.Left, this.Margins.Bottom);
			}
		}

		public bool OnDragBegin(Point cursor)
		{
			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			this.Margins = new Margins (this.Margins.Left + e.Offset.X, this.Margins.Right - e.Offset.X, this.Margins.Top - e.Offset.Y, this.Margins.Bottom + e.Offset.Y);
			this.Invalidate ();
			this.Window.SynchronousRepaint ();
		}

		public void OnDragEnd()
		{
			var center = this.ActualBounds.Center;
			var parent = this.Parent.Client.Bounds.Center;
			var anchor = AnchorStyles.None;
			double x1, x2, y1, y2;

			if (center.X < parent.X)
			{
				anchor |= AnchorStyles.Left;
			}
			else
			{
				anchor |= AnchorStyles.Right;
			}

			if (center.Y < parent.Y)
			{
				anchor |= AnchorStyles.Bottom;
			}
			else
			{
				anchor |= AnchorStyles.Top;
			}

			x1 = this.ActualBounds.Left;
			x2 = this.Parent.Client.Size.Width - this.ActualBounds.Right;
			y1 = this.ActualBounds.Bottom;
			y2 = this.Parent.Client.Size.Height - this.ActualBounds.Top;
			
			this.Anchor = anchor;
			this.Margins = new Margins (x1, x2, y2, y1);
		}

		#endregion


		private readonly DragBehavior			dragBehavior;
	}
}
