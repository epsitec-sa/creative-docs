//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Graph
{
	public class GraphWidget : Widget, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public GraphWidget(AbstractController controller)
		{
			this.controller = controller;
			this.engine = new GraphEngine ();
		}


		public GraphEngine GraphEngine
		{
			get
			{
				return this.engine;
			}
		}

		public GraphOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
				this.Invalidate ();
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			switch (message.MessageType)
			{
				case MessageType.MouseMove:
					this.ProcessMouseMove (message, pos);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseDown:
					this.ProcessMouseDown (message, pos);
					message.Captured = true;
					message.Consumer = this;
					break;

				case MessageType.MouseUp:
					this.ProcessMouseUp (message, pos);
					message.Captured = true;
					message.Consumer = this;
					break;
			}

			base.ProcessMessage (message, pos);
		}

		protected override void UpdateClientGeometry()
		{
		}
		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var cube = this.controller.DataAccessor.CubeToDraw;

			if (cube != null && this.options != null)
			{
				Rectangle rect = this.Client.Bounds;
				this.engine.PaintFull (cube, this.options, graphics, rect);
			}
		}


		private void ProcessMouseDown(Message message, Point pos)
		{
			this.isMouseDown = true;

			this.selectedHandle = this.engine.DetectHandle (pos);
			if (this.selectedHandle != -1)
			{
				return;
			}
			
			this.engine.SelectedSurfaceId = this.engine.DetectSurface (pos);

			if (this.engine.SelectedSurfaceId.Type == GraphSurfaceType.Legend)
			{
				var lr = this.engine.LegendsRect;
				this.dragSize = lr.Size;
				this.dragOffset = pos - lr.BottomLeft;
			}

			if (this.engine.SelectedSurfaceId.Type == GraphSurfaceType.Margins)
			{
				var dr = this.engine.DrawingRect;
				this.dragSize = dr.Size;
				this.dragOffset = pos - dr.BottomLeft;
			}

			if (this.engine.SelectedSurfaceId.Type == GraphSurfaceType.Title)
			{
				var tr = this.engine.TitleRect;
				this.dragSize = tr.Size;
				this.dragOffset = pos - tr.BottomLeft;
			}
		}

		private void ProcessMouseUp(Message message, Point pos)
		{
			this.isMouseDown = false;
			this.engine.HilitedSurfaceId = GraphSurfaceId.Empty;
			this.engine.Options.TempDraggedColumnPos = Point.Zero;
			this.Invalidate ();
		}

		private void ProcessMouseMove(Message message, Point pos)
		{
			if (this.isMouseDown)
			{
				if (this.selectedHandle != -1)
				{
					if (this.engine.SelectedSurfaceId.Type == GraphSurfaceType.Legend)
					{
						this.engine.Options.TempDraggedColumnPos = pos;
					}

					this.engine.SetHandle (this.selectedHandle, pos);
					this.Invalidate ();
				}
				else if (this.engine.SelectedSurfaceId.Type == GraphSurfaceType.Legend)
				{
					var x = (pos.X - this.dragOffset.X) / (this.Client.Bounds.Width  - this.dragSize.Width);
					var y = (pos.Y - this.dragOffset.Y) / (this.Client.Bounds.Height - this.dragSize.Height);

					x = System.Math.Max (x, 0.0);
					x = System.Math.Min (x, 1.0);
					y = System.Math.Max (y, 0.0);
					y = System.Math.Min (y, 1.0);

					this.options.LegendPositionRel = new Point (x, y);
					this.Invalidate ();
				}
				else if (this.engine.SelectedSurfaceId.Type == GraphSurfaceType.Margins)
				{
					var fullRect = this.engine.FullRect;
					var drawingRect = new Rectangle (pos.X-this.dragOffset.X, pos.Y-this.dragOffset.Y, this.dragSize.Width, this.dragSize.Height);

					drawingRect = Rectangles.MoveInside (drawingRect, fullRect);

					var left   = drawingRect.Left - fullRect.Left;
					var right  = fullRect.Right - drawingRect.Right;
					var bottom = drawingRect.Bottom - fullRect.Bottom;
					var top    = fullRect.Top - drawingRect.Top;

					this.options.MarginsAbs = new Margins (left, right, top, bottom);
					this.Invalidate ();
				}
				else if (this.engine.SelectedSurfaceId.Type == GraphSurfaceType.Title)
				{
					this.engine.TitleRect = new Rectangle (pos.X-this.dragOffset.X, pos.Y-this.dragOffset.Y, this.dragSize.Width, this.dragSize.Height);
					this.Invalidate ();
				}
			}
			else
			{
				var hilite = GraphSurfaceId.Empty;

				var rank = this.engine.DetectHandle (pos);
				if (rank == -1)
				{
					hilite = this.engine.DetectSurface (pos);
				}

				if (this.engine.HilitedSurfaceId != hilite)
				{
					this.engine.HilitedSurfaceId = hilite;
					this.Invalidate ();
				}
			}
		}


		#region Helpers.IToolTipHost
		public object GetToolTipCaption(Point pos)
		{
			//	Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
			return this.engine.GetTooltip (this.engine.HilitedSurfaceId);
		}
		#endregion


		private readonly AbstractController		controller;
		private readonly GraphEngine			engine;

		private GraphOptions					options;
		private bool							isMouseDown;
		private int								selectedHandle;
		private Size							dragSize;
		private Point							dragOffset;
	}
}
