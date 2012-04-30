//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList.Widgets
{
	public class ItemScrollList : Widget
	{
		public ItemScrollList()
		{
			this.headerView = new ItemListColumnHeaderView ()
			{
				Parent = this,
				Dock = DockStyle.Top,
			};

			this.splitView = new VSplitView ()
			{
				Parent = this,
				Dock = DockStyle.Fill,
			};

			this.listViews = new List<ItemListVerticalContentView> ();
		}




		private readonly VSplitView				splitView;
		private readonly ItemListColumnHeaderView headerView;
		private readonly List<ItemListVerticalContentView> listViews;
	}
}
