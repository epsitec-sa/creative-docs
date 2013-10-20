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
		public ObjectsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.isTimelineView = true;
		}


		public override void Dispose()
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.frameBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
			};

			this.CreateView ();
		}

		public override void OnCommand(ToolbarCommand command)
		{
			base.OnCommand (command);

			if (this.currentView != null)
			{
				this.currentView.OnCommand (command);
			}
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

				this.currentView.Dispose ();
				this.currentView = null;
			}

			this.frameBox.Children.Clear ();

			if (this.isTimelineView)
			{
				this.currentView = new ObjectsWithTimelineView (this.accessor, this.mainToolbar);
			}
			else
			{
				this.currentView = new ObjectsWithoutTimelineView (this.accessor, this.mainToolbar);
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
