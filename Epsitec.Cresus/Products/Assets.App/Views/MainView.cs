//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainView
	{
		public void CreateUI(Widget parent)
		{
			this.toolbar = new MainToolbar ();
			this.toolbar.CreateUI (parent);

			this.viewTitle = new StaticText
			{
				Parent           = parent,
				PreferredHeight  = 30,
				Dock             = DockStyle.Top,
				ContentAlignment = ContentAlignment.MiddleCenter,
			};

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

			if (this.view == null)
			{
				this.viewTitle.Text = null;
			}
			else
			{
				this.view.CreateUI (this.viewBox);

				this.viewTitle.Text = this.view.Title;
				this.viewTitle.TextLayout.DefaultFontSize = 20.0;
			}
		}


		private MainToolbar toolbar;
		private FrameBox viewBox;
		private StaticText viewTitle;
		private AbstractView view;
	}
}
