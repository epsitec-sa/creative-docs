//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphApplication : Application
	{
		public GraphApplication()
		{
		}

		public bool IsReady
		{
			get;
			private set;
		}

		public override string ShortWindowTitle
		{
			get
			{
				return "Crésus Graphe";
			}
		}

		internal void SetupDataSet()
		{
			this.graphDataSet = new GraphDataSet ();
		}
		

		internal void SetupInterface()
		{
			Window window = new Window ();

			window.Text = this.ShortWindowTitle;
			window.ClientSize = new Epsitec.Common.Drawing.Size (624, 400);

			FrameBox frame = new FrameBox ()
			{
				Margins = new Margins (8, 8, 8, 8),
				Dock = DockStyle.Fill,
				Parent = window.Root
			};

			this.scrollList = new ScrollListMultiSelect ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 300,
				Parent = frame
			};

			VSplitter splitter = new VSplitter ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 8,
				Parent = frame
			};

			this.chartView = new ChartView ()
			{
				Dock = DockStyle.Left,
				PreferredWidth = 300,
				Parent = frame
			};

			this.scrollList.Items.AddRange (graphDataSet.DataTable.RowLabels);

			this.Window = window;
			this.IsReady = true;
		}


		private ScrollList scrollList;
		private ChartView chartView;

		private GraphDataSet graphDataSet;
	}
}
