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

		public ItemViewLayout ItemViewLayout
		{
			get
			{
				return (ItemViewLayout) this.GetValue (ItemPanel.ItemViewLayoutProperty);
			}
			set
			{
				this.SetValue (ItemPanel.ItemViewLayoutProperty, value);
			}
		}

		public Drawing.Size ItemViewDefaultSize
		{
			get
			{
				return (Drawing.Size) this.GetValue (ItemPanel.ItemViewDefaultSizeProperty);
			}
			set
			{
				this.SetValue (ItemPanel.ItemViewDefaultSizeProperty, value);
			}
		}

		public Drawing.Rectangle Aperture
		{
			get
			{
				return this.aperture;
			}
			set
			{
				Drawing.Rectangle oldValue = this.aperture;
				Drawing.Rectangle newValue = value;
				
				if (oldValue != newValue)
				{
					this.aperture = newValue;
					this.HandleApertureChanged (oldValue, newValue);
				}
			}
		}

		public ItemView Detect(Drawing.Point pos)
		{
			IList<ItemView> views;

			using (new LockManager (this))
			{
				views = this.views;
			}

			return this.Detect (views, pos);
		}

		public int GetItemViewCount()
		{
			using (new LockManager (this))
			{
				return this.views.Count;
			}
		}

		public ItemView GetItemView(int index)
		{
			using (new LockManager (this))
			{
				return ItemPanel.TryGetItemView (this.views, index);
			}
		}

		

		internal void NotifyItemViewSizeChanged(ItemView view, Drawing.Size oldSize, Drawing.Size newSize)
		{
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
				this.HandleItemCollectionChanged (this, new CollectionChangedEventArgs (CollectionChangedAction.Reset));
			}
		}

		protected virtual void HandleItemViewLayoutChanged(ItemViewLayout oldValue, ItemViewLayout newValue)
		{
			this.RefreshLayout ();
		}

		protected virtual void HandleItemCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.RefreshItemViews ();
		}

		protected virtual void HandleApertureChanged(Drawing.Rectangle oldValue, Drawing.Rectangle newValue)
		{
			IEnumerable<ItemView> views;

			using (new LockManager (this))
			{
				views = this.views;
			}

			this.RecreateUserInterface (views, newValue);
		}

		private void RecreateUserInterface(IEnumerable<ItemView> views, Drawing.Rectangle aperture)
		{
			List<ItemView> dispose = new List<ItemView> ();
			List<ItemView> create  = new List<ItemView> ();
			
			foreach (ItemView view in views)
			{
				if (view.Widget != null)
				{
					if (view.Bounds.IntersectsWith (aperture))
					{
						//	Nothing to do : the item has a user interface and it will
						//	still be visible in the aperture.
					}
					else
					{
						//	The view is no longer visible; remember to dispose the
						//	user interface.

						dispose.Add (view);
					}
				}
				else
				{
					if (view.Bounds.IntersectsWith (aperture))
					{
						//	The view has no user interface, but it has just become
						//	visible in the aperture; remember to create the user
						//	interface.

						create.Add (view);
					}
				}
			}

			//	TODO: make this code asynchronous

			dispose.ForEach (
				delegate (ItemView view)
				{
					view.DisposeUserInterface ();
				} );

			create.ForEach (
				delegate (ItemView view)
				{
					view.CreateUserInterface (this);
				} );
		}

		private void RefreshItemViews()
		{
			ICollectionView items = this.Items;

			Dictionary<object, ItemView> currentViews = new Dictionary<object, ItemView> ();

			using (new LockManager (this))
			{
				foreach (ItemView view in this.views)
				{
					currentViews.Add (view.Item, view);
				}
			}

			List<ItemView> views = new List<ItemView> ();

			//	TODO: make this code asynchronous
			
			if ((items != null) &&
				(items.Items.Count > 0))
			{
				if (items.Groups.Count == 0)
				{
					this.CreateItemViews (views, items.Items, currentViews);
				}
				else
				{
					this.CreateItemViews (views, items.Groups, currentViews);
				}
			}

			this.RecreateUserInterface (views, this.aperture);

			using (new LockManager (this))
			{
				this.views = views;
				this.hotItemViewIndex = -1;
			}
		}

		private void RefreshLayout()
		{
			IEnumerable<ItemView> views;

			using (new LockManager (this))
			{
				views = this.views;
			}
			
			switch (this.ItemViewLayout)
			{
				case ItemViewLayout.VerticalList:
					this.PreferredSize = this.LayoutVerticalList (views);
					break;
			}
		}

		private void CreateItemViews(IList<ItemView> views, System.Collections.IList list, Dictionary<object, ItemView> currentViews)
		{
			foreach (object item in list)
			{
				ItemView view;

				if (currentViews.TryGetValue (item, out view))
				{
					currentViews.Remove (item);
					view.Index = views.Count;
					views.Add (view);
				}
				else
				{
					views.Add (this.CreateItemView (item, views.Count));
				}
			}
		}

		private ItemView CreateItemView(object item, int index)
		{
			ItemView view = new ItemView (item, this.ItemViewDefaultSize);
			
			view.Index = index;

			//	TODO: make this code asynchronous

			view.UpdatePreferredSize (this);
			
			return view;
		}

		private Drawing.Size LayoutVerticalList(IEnumerable<ItemView> views)
		{
			double dy = 0;
			double dx = 0;
			
			foreach (ItemView view in views)
			{
				dy += view.Size.Height;
				dx  = System.Math.Max (dx, view.Size.Width);
			}

			double y = dy;
			
			foreach (ItemView view in views)
			{
				double h = view.Size.Height;
				y -= h;
				view.Bounds = new Drawing.Rectangle (0, y, dx, h);
			}

			return new Drawing.Size (dx, dy);
		}

		private ItemView Detect(IList<ItemView> views, Drawing.Point pos)
		{
			if (this.hotItemViewIndex != -1)
			{
				foreach (ItemView view in ItemPanel.GetNearbyItemViews (views, this.hotItemViewIndex, 5))
				{
					if (view.Bounds.Contains (pos))
					{
						this.hotItemViewIndex = view.Index;
						return view;
					}
				}
			}
			
			foreach (ItemView view in views)
			{
				if (view.Bounds.Contains (pos))
				{
					this.hotItemViewIndex = view.Index;
					return view;
				}
			}

			this.hotItemViewIndex = -1;
			
			return null;
		}

		private static IEnumerable<ItemView> GetNearbyItemViews(IList<ItemView> views, int index, int count)
		{
			int min = index;
			int max = index+1;

			bool ok = true;

			while (ok)
			{
				ok = false;
				
				if (min >= 0)
				{
					if (count-- < 1)
					{
						yield break;
					}

					ok = true;
					yield return ItemPanel.TryGetItemView (views, min--);
				}
				
				if (max < views.Count)
				{
					if (count-- < 1)
					{
						yield break;
					}

					ok = true;
					yield return ItemPanel.TryGetItemView (views, max++);
				}
			}
		}
		
		private static ItemView TryGetItemView(IList<ItemView> views, int index)
		{
			if ((index >= 0) &&
				(index < views.Count))
			{
				return views[index];
			}
			else
			{
				return null;
			}
		}

		private void AssertLockOwned()
		{
			System.Diagnostics.Debug.Assert (this.lockAcquired > 0);
			System.Diagnostics.Debug.Assert (this.lockOwnerPid == System.Threading.Thread.CurrentThread.ManagedThreadId);
		}

		#region LockManager Class

		private class LockManager : System.IDisposable
		{
			public LockManager(ItemPanel panel)
			{
				this.panel = panel;
				System.Threading.Monitor.Enter (this.panel.exclusion);
				this.panel.lockAcquired++;
				this.panel.lockOwnerPid = System.Threading.Thread.CurrentThread.ManagedThreadId;
			}

			~LockManager()
			{
				throw new System.InvalidOperationException ("Lock not properly released");
			}
			
			#region IDisposable Members

			public void Dispose()
			{
				this.panel.lockAcquired--;
				System.Threading.Monitor.Exit (this.panel.exclusion);
				this.panel = null;
				System.GC.SuppressFinalize (this);
			}

			#endregion

			ItemPanel panel;
		}

		#endregion

		private static void NotifyItemsChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleItemsChanged ((ICollectionView) oldValue, (ICollectionView) newValue);
		}

		private static void NotifyItemViewLayoutChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleItemViewLayoutChanged ((ItemViewLayout) oldValue, (ItemViewLayout) newValue);
		}
		
		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register ("Items", typeof (ICollectionView), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanel.NotifyItemsChanged));
		public static readonly DependencyProperty ItemViewLayoutProperty = DependencyProperty.Register ("ItemViewLayout", typeof (ItemViewLayout), typeof (ItemPanel), new DependencyPropertyMetadata (ItemViewLayout.None, ItemPanel.NotifyItemViewLayoutChanged));
		public static readonly DependencyProperty ItemViewDefaultSizeProperty = DependencyProperty.Register ("ItemViewDefaultSize", typeof (Drawing.Size), typeof (ItemPanel), new DependencyPropertyMetadata (new Drawing.Size (80, 20)));

		List<ItemView> views
		{
			get
			{
				this.AssertLockOwned ();
				return this.lockedViews;
			}
			set
			{
				this.AssertLockOwned ();
				this.lockedViews = value;
			}
		}
		List<ItemView> lockedViews = new List<ItemView> ();
		object exclusion = new object ();
		int lockAcquired;
		int lockOwnerPid;
		int hotItemViewIndex = -1;
		Drawing.Rectangle aperture;
	}
}
