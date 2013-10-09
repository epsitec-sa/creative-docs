//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainView
	{
		public void CreateUI(Widget parent)
		{
			this.parent = parent;

			MouseCursorManager.SetWindow (parent.Window);

			this.toolbar = new MainToolbar ();
			this.toolbar.CreateUI (parent);

			this.viewBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.toolbar.ViewChanged += delegate (object sender, ViewType viewType)
			{
				this.UpdateView ();
			};

			this.UpdateView ();
		}


		private void UpdateView()
		{
			this.toolbar.SetCommandState (ToolbarCommand.Edit,   ToolbarCommandState.Hide);
			this.toolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
			this.toolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);

			this.viewBox.Children.Clear ();

			this.view = AbstractView.CreateView (this.toolbar.ViewType);

			if (this.view != null)
			{
				this.view.CreateUI (this.viewBox, this.toolbar);
			}
		}


		private Widget							parent;
		private MainToolbar						toolbar;
		private FrameBox						viewBox;
		private AbstractView					view;
	}
}
