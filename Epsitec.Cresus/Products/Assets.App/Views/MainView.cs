//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class MainView
	{
		public MainView(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

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

			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				if (command == ToolbarCommand.Open)
				{
					this.OnOpen ();
				}
				else
				{
					if (this.view != null)
					{
						this.view.OnCommand (command);
					}
				}
			};

			this.UpdateView ();
		}


		private void UpdateView()
		{
			this.viewBox.Children.Clear ();

			if (this.view != null)
			{
				this.view.Dispose ();
				this.view = null;
			}

			this.view = AbstractView.CreateView (this.toolbar.ViewType, this.accessor, this.toolbar);

			if (this.view != null)
			{
				this.view.CreateUI (this.viewBox);
			}
		}

		private void OnOpen()
		{
			this.ShowPopup ();
		}

		private void ShowPopup()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.Open);

			var popup = new SimplePopup ()
			{
				SelectedItem = AssetsApplication.SelectedMandat,
			};

			for (int i=0; i<AssetsApplication.MandatCount; i++)
			{
				var mandat = AssetsApplication.GetMandat (i);
				popup.Items.Add ("Ouvrir le mandat \"" + mandat.Name + "\"");
			}

			popup.Create (target, leftOrRight: false);

			popup.ItemClicked += delegate (object sender, int rank)
			{
				this.OpenMandat (rank);
			};
		}

		private void OpenMandat(int rank)
		{
			AssetsApplication.SelectedMandat = rank;
			this.accessor.Mandat = AssetsApplication.GetMandat (rank);

			this.UpdateView ();
		}


		private readonly DataAccessor			accessor;

		private Widget							parent;
		private MainToolbar						toolbar;
		private FrameBox						viewBox;
		private AbstractView					view;
	}
}
