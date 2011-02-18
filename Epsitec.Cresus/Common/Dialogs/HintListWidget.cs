//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	/// item panel. It is tightly bound to the <see cref="HintListController"/>
	/// class.
	/// </summary>
	public sealed class HintListWidget : FrameBox
	{
		public HintListWidget()
		{
			this.header = new HintListHeaderWidget ()
			{
				Embedder = this,
				Dock = DockStyle.Top,
				PreferredHeight =  100
			};

			this.itemTable = new ItemTable ()
			{
				Embedder = this,
				Dock = DockStyle.Fill,
				HeaderVisibility = false,
				HorizontalScrollMode = ItemTableScrollMode.None,
				VerticalScrollMode = ItemTableScrollMode.ItemBased
			};

			this.itemTable.ItemPanel.CurrentItemTrackingMode = CurrentItemTrackingMode.AutoSelect;
 
			this.BackColor = Drawing.Color.FromName ("White");
		}


		public HintListSearchWidget				SearchWidget
		{
			get
			{
				return this.header.SearchWidget;
			}
		}
		
		public HintListHeaderWidget				Header
		{
			get
			{
				return this.header;
			}
		}
		
		public ICollectionView					Items
		{
			get
			{
				return this.items;
			}
			set
			{
				if (this.items != value)
				{
					if (this.items != null)
					{
						this.items.CurrentChanged -= this.HandleItemsCurrentChanged;
					}

					this.items = value;
					this.itemTable.Items = value;

					ItemPanel itemPanel = this.itemTable.ItemPanel;
					
					itemPanel.Refresh ();

					object    currentItem = this.items.CurrentItem;
					ItemView  itemView    = itemPanel.FindItemView (currentItem);

					System.Diagnostics.Debug.Assert ((currentItem == null) || (itemView != null));

					itemPanel.SelectItemView (itemView);
					itemPanel.FocusItemView (itemView);

					if (this.items != null)
					{
						this.items.CurrentChanged += this.HandleItemsCurrentChanged;
					}
				}
			}
		}

		
		public bool Navigate(Message message)
		{
			return this.itemTable.ItemPanel.Navigate (message);
		}
		
		
		private void HandleItemsCurrentChanged(object sender)
		{
			this.OnCurrentItemChanged ();
		}

		private void OnCurrentItemChanged()
		{
			if (this.CurrentItemChanged != null)
			{
				this.CurrentItemChanged (this);
			}
		}

		
		public event EventHandler				CurrentItemChanged;

		private readonly ItemTable				itemTable;
		private readonly HintListHeaderWidget	header;
		private ICollectionView					items;
	}
}
