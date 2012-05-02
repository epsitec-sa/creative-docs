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
	public class GraphWidget : Widget
	{
		public GraphWidget(AbstractController controller)
		{
			this.controller = controller;
			this.engine = new GraphEngine ();
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

		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var cube = this.controller.DataAccessor.CubeToDraw;

			if (cube != null && this.options != null)
			{
				Rectangle rect = this.Client.Bounds;
				this.engine.PaintFull (cube, this.options, graphics, rect);
			}
		}


		private readonly AbstractController		controller;
		private readonly GraphEngine			engine;

		private GraphOptions					options;
	}
}
