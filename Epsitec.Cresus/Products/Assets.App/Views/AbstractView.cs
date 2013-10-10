//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractView
	{
		public AbstractView(DataMandat mandat)
		{
			this.mandat = mandat;
		}

		public virtual void CreateUI(Widget parent, MainToolbar toolbar)
		{
			this.toolbar = toolbar;

			var topBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 2),
			};

			this.listFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
			};

			this.editFrameBox = new FrameBox
			{
				Parent         = topBox,
				Dock           = DockStyle.Right,
				PreferredWidth = 500,
				Margins        = new Margins (5, 0, 0, 0),
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.listTopTitle = new TopTitle
			{
				Parent = this.listFrameBox,
			};

			this.listTopTitle.SetTitle (this.Title);

			this.editTopTitle = new TopTitle
			{
				Parent = this.editFrameBox,
			};

			this.editTopTitle.SetTitle ("Edition");

			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.New:
						this.OnCommandNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnCommandDelete ();
						break;

					case ToolbarCommand.Edit:
						this.OnCommandEdit ();
						break;

					case ToolbarCommand.Accept:
						this.OnCommandAccept ();
						break;

					case ToolbarCommand.Cancel:
						this.OnCommandCancel ();
						break;
				}
			};
		}


		protected virtual string Title
		{
			get
			{
				return null;
			}
		}


		protected virtual void OnCommandNew()
		{
		}

		protected virtual void OnCommandDelete()
		{
		}

		protected void OnCommandEdit()
		{
			this.isEditing = true;
			this.Update ();
		}

		protected void OnCommandAccept()
		{
			this.isEditing = false;
			this.Update ();
		}

		protected void OnCommandCancel()
		{
			if (this.isEditing)
			{
				this.isEditing = false;
			}
			else
			{
				this.SelectedRow = -1;
			}

			this.Update ();
		}


		protected void Update()
		{
			this.editFrameBox.Visibility = this.isEditing;

			if (this.SelectedRow == -1)
			{
				this.toolbar.SetCommandState (ToolbarCommand.New,    ToolbarCommandState.Enable);
				this.toolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Hide);
				this.toolbar.SetCommandState (ToolbarCommand.Edit,   ToolbarCommandState.Hide);
				this.toolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.toolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}
			else
			{
				if (this.isEditing)
				{
					this.toolbar.SetCommandState (ToolbarCommand.New,    ToolbarCommandState.Hide);
					this.toolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Hide);
					this.toolbar.SetCommandState (ToolbarCommand.Edit,   ToolbarCommandState.Hide);
					this.toolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Enable);
					this.toolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
				}
				else
				{
					this.toolbar.SetCommandState (ToolbarCommand.New,    ToolbarCommandState.Enable);
					this.toolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Enable);
					this.toolbar.SetCommandState (ToolbarCommand.Edit,   ToolbarCommandState.Enable);
					this.toolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Disable);
					this.toolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
				}
			}
		}

		protected virtual int SelectedRow
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}


		public static AbstractView CreateView(ViewType viewType, DataMandat mandat)
		{
			switch (viewType)
			{
				case ViewType.Objects:
					return new ObjectsView (mandat);

				case ViewType.Categories:
					return new CategoriesView (mandat);

				case ViewType.Groups:
					return new GroupsView (mandat);

				case ViewType.Events:
					return new EventsView (mandat);

				case ViewType.Reports:
					return new ReportsView (mandat);

				case ViewType.Settings:
					return new SettingsView (mandat);

				default:
					return null;
			}
		}


		protected readonly DataMandat			mandat;

		protected MainToolbar					toolbar;

		protected FrameBox						listFrameBox;
		protected FrameBox						editFrameBox;

		protected TopTitle						listTopTitle;
		protected TopTitle						editTopTitle;

		protected bool							isEditing;
	}
}
