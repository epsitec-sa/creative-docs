//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>TileTabBook</c> implements a simplified tab book, for use in the
	/// edition tiles.
	/// </summary>
	public class TileTabBook : FrameBox
	{
		public TileTabBook(IEnumerable<TabPageDef> items)
		{
			this.items = new List<TabPageDef> (items);
			this.CreateUI ();
		}

		
		public IEnumerable<TabPageDef> Items
		{
			get
			{
				return this.items;
			}
		}

		
		public void SelectTabPage(TabPageDef description)
		{
			if (this.selectedItemName != description.Name)
			{
				this.selectedItemName = description.Name;
				this.RefreshTabPageSelection ();

				if (description != null)
				{
					description.ExecuteAction ();
				}
			}
		}

		
		private void CreateUI()
		{
			foreach (var child in this.Children.Widgets)
			{
				child.Clicked -= this.HandleChildClicked;
				child.Dispose ();
			}

			var lastPageDescription = this.items.LastOrDefault ();

			foreach (var pageDescription in this.items)
			{
				var name = pageDescription.Name;
				var text = pageDescription.Text;

				var tilePage = new Widgets.Tiles.ArrowedTileTabPageButton (pageDescription)
				{
					Parent = this,
					PreferredHeight = 24 + Widgets.Tiles.TileArrow.Breadth,
					Margins = new Margins (0, pageDescription == lastPageDescription ? 0 : -1, 0, 0),
					Dock = DockStyle.StackFill,
				};

				tilePage.Clicked += this.HandleChildClicked;
			}
		}

		private void HandleChildClicked(object sender, MessageEventArgs e)
		{
			var page = sender as Widgets.Tiles.ArrowedTileTabPageButton;

			if (page == null)
			{
				this.SelectTabPage (null);
			}
			else
			{
				this.SelectTabPage (page.TabPageDef);
			}
		}

		private void RefreshTabPageSelection()
		{
			foreach (Widgets.Tiles.ArrowedTileTabPageButton page in this.Children.Widgets)
			{
				bool isSelected = page.TabPageDef.Name == this.selectedItemName;

				page.SetSelected (isSelected);
				page.TabPageDef.SetPageWidgetsVisibility (isSelected);
			}
		}

		private readonly List<TabPageDef> items;
		private string selectedItemName;
	}
}
