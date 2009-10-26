//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Graph.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class SnapshotsController
	{
		public SnapshotsController(WorkspaceController workspace)
		{
			this.workspace = workspace;
			
			this.itemsController = new ItemListController<Widget> ()
			{
				ItemLayoutMode = ItemLayoutMode.Flow,
				OverlapX = -2,
				OverlapY = -2,
			};
		}
		
		
		public Widget Container
		{
			get
			{
				return this.container;
			}
		}


		public void SetupUI(Widget container)
		{
			this.container = container;

			this.itemsController.SetupUI (this.container);
		}

		public void Refresh()
		{
			this.itemsController.Clear ();

			foreach (var chartSnapShot in this.workspace.Document.ChartSnapshots.Where (x => x.Visibility))
			{
				var view = new SnapshotMiniChartView ()
				{
					Anchor = AnchorStyles.TopLeft,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					PreferredWidth = WorkspaceController.DefaultViewWidth,
					PreferredHeight = WorkspaceController.DefaultViewHeight,
					Padding = new Margins (4, 4, 4, 4),
					Renderer = chartSnapShot.CreateRenderer (false),
					Scale = 0.5,
				};

				string iconName = "manifest:Epsitec.Common.Graph.Images.Glyph.DropItem.icon";
				string guidName = chartSnapShot.GuidName;

				view.DefineIconButton (ButtonVisibility.ShowOnlyWhenEntered, iconName,
					delegate
					{
						GraphActions.DocumentHideSnapshot (guidName);
					});



				this.itemsController.Add (view);
			}
		}


		private readonly WorkspaceController	workspace;
		private Widget							container;
		private ItemListController<Widget>		itemsController;
	}
}
