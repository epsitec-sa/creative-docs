//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainView
	{
		public void CreateUI(Widget parent)
		{
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
			this.viewBox.Children.Clear ();

			this.view = AbstractView.CreateView (this.toolbar.ViewType);

			if (this.view != null)
			{
				this.view.CreateUI (this.viewBox);
			}
		}


		private MainToolbar toolbar;
		private FrameBox viewBox;
		private AbstractView view;
	}
}
