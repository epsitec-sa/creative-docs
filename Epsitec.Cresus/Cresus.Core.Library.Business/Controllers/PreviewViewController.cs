//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class PreviewViewController : CoreViewController, IWidgetUpdater
	{
		public PreviewViewController(DataViewOrchestrator orchestrator)
			: base ("Preview", orchestrator)
		{
			this.data = this.Orchestrator.Data;
		}

		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);
			this.mainFrame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				MinWidth = 0,
				BackColor = Color.FromRgb (1.0, 1.0, 0.9)
			};
		}

		public void Add(Widget widget)
		{
			if ((widget.Dock == DockStyle.None) &&
				(widget.Anchor == AnchorStyles.None))
			{
				widget.Dock = DockStyle.Stacked;
			}

			this.mainFrame.Children.Add (widget);
		}


		public void Clear()
		{
			var widgets = this.mainFrame.Children.Widgets;
			widgets.ForEach (x => x.Dispose ());
		}

		#region IWidgetUpdater Members

		void IWidgetUpdater.Update()
		{
			this.OnUpdating ();
		}

		#endregion

		private void OnUpdating()
		{
			var handler = this.Updating;

			if (handler != null)
            {
				handler (this);
            }
		}

		
		public event EventHandler	Updating;

		private readonly CoreData data;
		
		private FrameBox mainFrame;
	}
}