//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ItemPanel))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemPanel</c> class represents a collection of items in a panel,
	/// where the collection is defined through a <see cref="ICollectionView"/>.
	/// </summary>
	public class ItemPanel : Widgets.FrameBox
	{
		public ItemPanel()
		{
		}
		
		public ICollectionView Items
		{
			get
			{
				return (ICollectionView) this.GetValue (ItemPanel.ItemsProperty);
			}
			set
			{
				this.SetValue (ItemPanel.ItemsProperty, value);
			}
		}

		protected virtual void HandleItemsChanged(ICollectionView oldValue, ICollectionView newValue)
		{
			if (oldValue != null)
			{
				oldValue.CollectionChanged -= this.HandleItemCollectionChanged;
			}
			if (newValue != null)
			{
				newValue.CollectionChanged += this.HandleItemCollectionChanged;
			}
		}

		protected virtual void HandleItemCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.RefreshItemViews ();
		}

		private void RefreshItemViews()
		{
			ICollectionView items = this.Items;

			Dictionary<object, ItemView> currentViews = new Dictionary<object, ItemView> ();
			
			foreach (ItemView view in this.views)
			{
				currentViews.Add (view.Item, view);
			}
			
			this.views.Clear ();

			if ((items == null) ||
				(items.Items.Count == 0))
			{
				return;
			}

			if (items.Groups.Count == 0)
			{
				this.CreateItemViews (items.Items, currentViews);
			}
			else
			{
				this.CreateItemViews (items.Groups, currentViews);
			}
		}

		private void CreateItemViews(System.Collections.IList list, Dictionary<object, ItemView> currentViews)
		{
			foreach (object item in list)
			{
				ItemView view;

				if (currentViews.TryGetValue (item, out view))
				{
					currentViews.Remove (item);
					this.views.Add (view);
				}
				else
				{
					this.views.Add (this.CreateItemView (item));
				}
			}
		}

		private ItemView CreateItemView(object item)
		{
			//	TODO: create the proper item view for this item
			
			return new ItemView (item, this.itemViewDefaultSize);
		}

		private static void NotifyItemsChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleItemsChanged ((ICollectionView) oldValue, (ICollectionView) newValue);
		}
		
		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register ("Items", typeof (ICollectionView), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanel.NotifyItemsChanged));

		List<ItemView> views = new List<ItemView> ();
		object exclusion = new object ();
		Drawing.Size itemViewDefaultSize = new Drawing.Size (80, 20);
	}
}
