//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Widgets
{
	public class GraphWidget : Widget
	{
		public GraphWidget()
		{
			this.engine = new GraphEngine ();
		}


		public Cube Cube
		{
			get
			{
				return this.cube;
			}
			set
			{
				this.cube = value;
				this.Invalidate ();
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

		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			if (this.cube != null && this.options != null)
			{
				Rectangle rect = this.Client.Bounds;
				this.engine.PaintFull (this.cube, this.options, graphics, rect);
			}
		}


		private readonly GraphEngine	engine;

		private Cube					cube;
		private GraphOptions			options;
	}
}
