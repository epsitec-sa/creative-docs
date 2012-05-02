//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
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
			if (message.MessageType == MessageType.MouseMove)
			{
				var surface = this.engine.Detect (pos);

				if (this.hilitedSurface != surface)
				{
					this.hilitedSurface = surface;
					this.Invalidate ();
				}
			}

			base.ProcessMessage (message, pos);
		}
		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var cube = this.controller.DataAccessor.CubeToDraw;

			if (cube != null && this.options != null)
			{
				Rectangle rect = this.Client.Bounds;
				this.engine.PaintFull (cube, this.options, graphics, rect);

				if (this.hilitedSurface != null)
				{
					this.PaintHilitedSurface (graphics, this.hilitedSurface);
				}
			}
		}

		private void PaintHilitedSurface(Graphics graphics, GraphSurface surface)
		{
			if (!surface.Rect.IsSurfaceZero)
			{
				graphics.AddFilledRectangle (surface.Rect);
				graphics.RenderSolid (Color.FromAlphaColor (0.5, Color.FromName ("White")));

				graphics.LineWidth = 3;
				graphics.AddRectangle (surface.Rect);
				graphics.RenderSolid (Color.FromName ("White"));
				graphics.LineWidth = 1;

				graphics.AddRectangle (surface.Rect);
				graphics.RenderSolid (Color.FromName ("Black"));
			}
			else if (surface.Path != null)
			{
				graphics.Color = Color.FromAlphaColor (0.5, Color.FromName ("White"));
				graphics.PaintSurface (surface.Path);

				graphics.LineWidth = 3;
				graphics.Color = Color.FromName ("White");
				graphics.PaintOutline (surface.Path);
				graphics.LineWidth = 1;

				graphics.Color = Color.FromName ("Black");
				graphics.PaintOutline (surface.Path);
			}
		}


		#region Helpers.IToolTipHost
		public object GetToolTipCaption(Point pos)
		{
			//	Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
			return this.engine.GetTooltip (this.hilitedSurface);
		}
		#endregion

	
		private readonly AbstractController		controller;
		private readonly GraphEngine			engine;

		private GraphOptions					options;
		private GraphSurface					hilitedSurface;
	}
}
