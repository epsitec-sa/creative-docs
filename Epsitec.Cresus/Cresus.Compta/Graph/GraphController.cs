//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	public class GraphController
	{
		public GraphController(AbstractController controller)
		{
			this.controller = controller;
		}


		public void CreateUI(Widget parent)
		{
			this.mainFrame = new FrameBox
			{
				Parent     = parent,
				Dock       = DockStyle.Fill,
				Visibility = false,
			};

			this.graphWidget = new GraphWidget (this.controller)
			{
				Parent  = this.mainFrame,
				Options = this.controller.DataAccessor.Options.GraphOptions,
				Dock    = DockStyle.Fill,
			};

			ToolTip.Default.RegisterDynamicToolTipHost (this.graphWidget);  // pour voir les tooltips dynamiques
		}

		public bool Show
		{
			get
			{
				return this.mainFrame.Visibility;
			}
			set
			{
				this.mainFrame.Visibility = value;
			}
		}

		public void Update()
		{
			this.graphWidget.Invalidate ();
		}


		private readonly AbstractController		controller;

		private FrameBox						mainFrame;
		private GraphWidget						graphWidget;
	}
}
