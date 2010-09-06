//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
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
	public class PreviewViewController : CoreViewController
	{
		public PreviewViewController(string name, CoreData data)
			: base (name)
		{
			this.data = data;
		}

		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.mainFrame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Fill,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
//-				MinWidth = 200,
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
		
		private readonly CoreData data;
		
		private FrameBox mainFrame;
	}
}