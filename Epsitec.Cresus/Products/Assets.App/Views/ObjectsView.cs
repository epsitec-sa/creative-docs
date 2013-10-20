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
			this.isWithTimelineView = true;
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
			//	Crée la bonne vue (With/Without Timeline) tout en transférant un maximum
			//	de l'aspect de l'ancienne dans la nouvelle vue.
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

			if (this.isWithTimelineView)
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
				this.isWithTimelineView = !this.isWithTimelineView;
				this.CreateView ();
			};
		}


		private bool							isWithTimelineView;
		private AbstractObjectsView				currentView;
		private FrameBox						frameBox;
	}
}
