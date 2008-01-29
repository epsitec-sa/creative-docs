//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (HintListController))]

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>HintListWidget</c> class represents a hint list using a dedicated
	/// item panel.
	/// </summary>
	public sealed class HintListWidget : FrameBox
	{
		public HintListWidget()
		{
			this.itemTable = new ItemTable ()
			{
				Embedder = this,
				Dock = DockStyle.Fill,
				HeaderVisibility = false,
				HorizontalScrollMode = ItemTableScrollMode.Linear,
				VerticalScrollMode = ItemTableScrollMode.ItemBased
			};
 
			this.factory = new ItemViewFactory (this);
			this.BackColor = Drawing.Color.FromName ("White");
		}


		public ICollectionView Items
		{
			get
			{
				return this.items;
			}
			set
			{
				if (this.items != value)
				{
					this.items = value;
					this.itemTable.Items = value;
					this.itemTable.ItemPanel.CustomItemViewFactoryGetter = x => this.factory;
				}
			}
		}

		private class ItemViewFactory : IItemViewFactory
		{
			public ItemViewFactory(HintListWidget widget)
			{
				this.widget = widget;
			}

			#region IItemViewFactory Members

			public ItemViewWidget CreateUserInterface(ItemView itemView)
			{
				ItemViewWidget container = new ItemViewWidget (itemView);
				AbstractEntity data      = itemView.Item as AbstractEntity;
				StaticText     widget    = new StaticText (container);

				widget.Text          = TextLayout.ConvertToTaggedText (data.Dump ());
				widget.PreferredSize = widget.GetBestFitSize ();

				return container;
			}

			public void DisposeUserInterface(ItemViewWidget widget)
			{
				widget.Dispose ();
			}

			public Epsitec.Common.Drawing.Size GetPreferredSize(ItemView itemView)
			{
				return itemView.Owner.ItemViewDefaultSize;
			}

			#endregion


			private readonly HintListWidget widget;
		}


		private readonly ItemTable itemTable;
		private readonly ItemViewFactory factory;
		private ICollectionView items;
	}
}
