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
		public AbstractView(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public virtual void CreateUI(Widget parent, MainToolbar toolbar)
		{
			this.mainToolbar = toolbar;

			var topBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 5),
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
		}


		protected virtual string Title
		{
			get
			{
				return null;
			}
		}


		protected virtual void Update()
		{
			this.editFrameBox.Visibility = this.isEditing;
		}


		public static AbstractView CreateView(ViewType viewType, DataAccessor accessor)
		{
			switch (viewType)
			{
				case ViewType.Objects:
					return new ObjectsView (accessor);

				case ViewType.Categories:
					return new CategoriesView (accessor);

				case ViewType.Groups:
					return new GroupsView (accessor);

				case ViewType.Events:
					return new EventsView (accessor);

				case ViewType.Reports:
					return new ReportsView (accessor);

				case ViewType.Settings:
					return new SettingsView (accessor);

				default:
					return null;
			}
		}


		protected readonly DataAccessor			accessor;

		protected MainToolbar					mainToolbar;

		protected FrameBox						listFrameBox;
		protected FrameBox						editFrameBox;

		protected TopTitle						listTopTitle;
		protected TopTitle						editTopTitle;

		protected bool							isEditing;
	}
}
