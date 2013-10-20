//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsView : AbstractView
	{
		public ObjectsView(DataAccessor accessor)
			: base (accessor)
		{
			this.isTimelineView = true;
		}

		public override void CreateUI(Widget parent, MainToolbar toolbar)
		{
			base.CreateUI (parent, toolbar);

			this.frameBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
			};

			this.CreateView ();
		}

		protected override void Update()
		{
			this.currentView.Update ();
		}


		private void CreateView()
		{
			Guid guid = Guid.Empty;
			Timestamp? timestamp = null;

			if (this.currentView != null)
			{
				guid = this.currentView.ObjectGuid;
				timestamp = this.currentView.Timestamp;
			}

			this.frameBox.Children.Clear ();

			if (this.isTimelineView)
			{
				this.currentView = new ObjectsTimelineView (this.accessor, this.mainToolbar);
			}
			else
			{
				this.currentView = new ObjectsTreeTableView (this.accessor, this.mainToolbar);
			}

			this.currentView.CreateUI (this.frameBox);
			this.currentView.ObjectGuid = guid;
			this.currentView.Timestamp = timestamp;

			this.currentView.ViewChanged += delegate
			{
				this.isTimelineView = !this.isTimelineView;
				this.CreateView ();
			};
		}


		private bool							isTimelineView;
		private AbstractObjectsView				currentView;
		private FrameBox						frameBox;
	}
}
