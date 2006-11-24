//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
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

		public ItemPanelLayout Layout
		{
			get
			{
				return (ItemPanelLayout) this.GetValue (ItemPanel.LayoutProperty);
			}
			set
			{
				this.SetValue (ItemPanel.LayoutProperty, value);
			}
		}

		public ItemPanelSelectionMode ItemSelection
		{
			get
			{
				return (ItemPanelSelectionMode) this.GetValue (ItemPanel.ItemSelectionProperty);
			}
			set
			{
				this.SetValue (ItemPanel.ItemSelectionProperty, value);
			}
		}

		public ItemPanelSelectionMode GroupSelection
		{
			get
			{
				return (ItemPanelSelectionMode) this.GetValue (ItemPanel.GroupSelectionProperty);
			}
			set
			{
				this.SetValue (ItemPanel.GroupSelectionProperty, value);
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
			return this.Detect (this.SafeGetViews (), pos);
		}

		public int GetItemViewCount()
		{
			return this.SafeGetViews ().Count;
		}

		public ItemView GetItemView(int index)
		{
			return ItemPanel.TryGetItemView (this.SafeGetViews (), index);
		}


		public void SelectItemView(ItemView view)
		{
			if (view.IsSelected)
			{
				this.SetItemViewSelection (view, true);
			}
			else
			{
				IList<ItemView>        selectedViews;
				ItemPanelSelectionMode selectionMode;
				
				if (ItemPanel.ContainsItem (view))
				{
					//	The view specifies a plain item.
					
					selectedViews = this.GetSelectedItemViews (ItemPanel.ContainsItem);
					selectionMode = this.ItemSelection;
				}
				else
				{
					//	The view specifies a group.

					selectedViews = this.GetSelectedItemViews (ItemPanel.ContainsGroup);
					selectionMode = this.GroupSelection;
				}

				System.Diagnostics.Debug.Assert (selectedViews.Contains (view) == false);
				
				switch (selectionMode)
				{
					case ItemPanelSelectionMode.None:
						this.DeselectItemViews (selectedViews);
						break;

					case ItemPanelSelectionMode.ExactlyOne:
					case ItemPanelSelectionMode.ZeroOrOne:
						this.DeselectItemViews (selectedViews);
						this.SetItemViewSelection (view, true);
						break;

					case ItemPanelSelectionMode.Multiple:
						this.SetItemViewSelection (view, true);
						break;
				}
			}
		}

		public void DeselectItemView(ItemView view)
		{
			if (view.IsSelected)
			{
				IList<ItemView> selectedViews;
				ItemPanelSelectionMode selectionMode;

				if (ItemPanel.ContainsItem (view))
				{
					//	The view specifies a plain item.

					selectedViews = this.GetSelectedItemViews (ItemPanel.ContainsItem);
					selectionMode = this.ItemSelection;
				}
				else
				{
					//	The view specifies a group.

					selectedViews = this.GetSelectedItemViews (ItemPanel.ContainsGroup);
					selectionMode = this.GroupSelection;
				}

				System.Diagnostics.Debug.Assert (selectedViews.Contains (view));

				switch (selectionMode)
				{
					case ItemPanelSelectionMode.None:
						this.DeselectItemViews (selectedViews);
						break;

					case ItemPanelSelectionMode.ExactlyOne:
						if (selectedViews.Count > 1)
						{
							this.SetItemViewSelection (view, false);
						}
						break;

					case ItemPanelSelectionMode.ZeroOrOne:
					case ItemPanelSelectionMode.Multiple:
						this.SetItemViewSelection (view, false);
						break;
				}
			}
			else
			{
				this.SetItemViewSelection (view, false);
			}
		}

		public void ExpandItemView(ItemView view, bool expand)
		{
			if (ItemPanel.ContainsGroup (view))
			{
				view.IsExpanded = expand;
			}
		}

		public IList<ItemView> GetSelectedItemViews()
		{
			return this.GetSelectedItemViews (null);
		}
		
		public IList<ItemView> GetSelectedItemViews(System.Predicate<ItemView> filter)
		{
			List<ItemView> list = new List<ItemView> ();
			IEnumerable<ItemView> views = this.SafeGetViews ();

			foreach (ItemView view in views)
			{
				if (view.IsSelected)
				{
					if ((filter == null) ||
						(filter (view)))
					{
						list.Add (view);
					}
				}
			}
			
			return list;
		}

		public void Show(ItemView view)
		{
			if (this.Aperture.IsSurfaceZero)
			{
				//	Nothing to do : there is no visible aperture, so don't bother
				//	moving it around !
			}
			else
			{
				Drawing.Rectangle aperture = this.Aperture;
				Drawing.Rectangle bounds   = view.Bounds;

				if (aperture.Contains (bounds))
				{
					//	Nothing to do : the view is already completely visible in
					//	the current aperture.
				}
				else
				{
					double ox = 0;
					double oy = 0;
					
					if ((aperture.Right < bounds.Right) &&
						(aperture.Left < bounds.Left))
					{
						ox = System.Math.Max (aperture.Right - bounds.Right, aperture.Left - bounds.Left);
					}
					else if ((aperture.Right > bounds.Right) &&
						/**/ (aperture.Left > bounds.Left))
					{
						ox = System.Math.Min (aperture.Right - bounds.Right, aperture.Left - bounds.Left);
					}

					if ((aperture.Top < bounds.Top) &&
						(aperture.Bottom < bounds.Bottom))
					{
						oy = System.Math.Max (aperture.Top - bounds.Top, aperture.Bottom - bounds.Bottom);
					}
					else if ((aperture.Top > bounds.Top) &&
						/**/ (aperture.Bottom > bounds.Bottom))
					{
						oy = System.Math.Min (aperture.Top - bounds.Top, aperture.Bottom - bounds.Bottom);
					}

					aperture.Offset (-ox, -oy);

					this.Aperture = aperture;
				}
			}
		}

		#region Filter Methods

		public static bool ContainsItem(ItemView view)
		{
			return (view.Item is CollectionViewGroup) ? false : true;
		}

		public static bool ContainsGroup(ItemView view)
		{
			return (view.Item is CollectionViewGroup) ? true : false;
		}

		#endregion
		
		internal void NotifyItemViewSizeChanged(ItemView view, Drawing.Size oldSize, Drawing.Size newSize)
		{
			this.InvalidateLayout ();
		}

		private void InvalidateLayout()
		{
			if (this.hasDirtyLayout == false)
			{
				this.hasDirtyLayout = true;
				Widgets.Application.QueueAsyncCallback (this.RefreshLayout);
			}
		}

		internal void SetParentGroup(ItemPanelGroup parentGroup)
		{
			if (this.parentGroup != parentGroup)
			{
				this.parentGroup = parentGroup;
				this.RefreshItemViews ();
			}
		}

		internal void AddPanelGroup(ItemPanelGroup group)
		{
			using (new LockManager (this))
			{
				this.groups.Add (group);
			}
		}

		internal void RemovePanelGroup(ItemPanelGroup group)
		{
			using (new LockManager (this))
			{
				this.groups.Remove (group);
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
				this.HandleItemCollectionChanged (this, new CollectionChangedEventArgs (CollectionChangedAction.Reset));
			}
		}

		protected virtual void HandleLayoutChanged(ItemPanelLayout oldValue, ItemPanelLayout newValue)
		{
			this.RefreshLayout ();
		}

		protected virtual void HandleItemSelectionChanged(ItemPanelSelectionMode oldValue, ItemPanelSelectionMode newValue)
		{
			this.RefreshLayout ();
		}

		protected virtual void HandleGroupSelectionChanged(ItemPanelSelectionMode oldValue, ItemPanelSelectionMode newValue)
		{
			this.RefreshLayout ();
		}

		protected virtual void HandleItemCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.RefreshItemViews ();
		}

		protected virtual void HandleApertureChanged(Drawing.Rectangle oldValue, Drawing.Rectangle newValue)
		{
			this.RecreateUserInterface (this.SafeGetViews (), newValue);

			ItemPanelGroup[] groups;

			using (new LockManager (this))
			{
				groups = this.groups.ToArray ();
			}

			for (int i = 0; i < groups.Length; i++)
			{
				groups[i].RefreshAperture (this.Aperture);
			}
		}

		private IList<ItemView> SafeGetViews()
		{
			using (new LockManager (this))
			{
				return this.views;
			}
		}

		private void DeselectItemViews(IEnumerable<ItemView> views)
		{
			foreach (ItemView view in views)
			{
				this.SetItemViewSelection (view, false);
			}
		}

		private void SetItemViewSelection(ItemView view, bool selection)
		{
			if (view.IsSelected != selection)
			{
				view.IsSelected = selection;
				
				if (view.IsVisible)
				{
					this.Invalidate (Drawing.Rectangle.Intersection (this.Aperture, view.Bounds));
				}
			}

			if (selection)
			{
				if (ItemPanel.ContainsGroup (view))
				{
					//	A group was selected. This will not change the current item in
					//	the collection view.
				}
				else
				{
					this.Items.MoveCurrentTo (view.Item);
				}
			}
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
			else if (this.parentGroup != null)
			{
				CollectionViewGroup group = this.parentGroup.CollectionViewGroup;

				if (group.HasSubgroups)
				{
					this.CreateItemViews (views, group.Subgroups, currentViews);
				}
				else
				{
					this.CreateItemViews (views, group.Items as System.Collections.IList, currentViews);
				}
			}

			this.RefreshLayout (views);
			this.RecreateUserInterface (views, this.aperture);

			using (new LockManager (this))
			{
				this.views = views;
				this.hotItemViewIndex = -1;
			}
		}
		
		private void RefreshLayout()
		{
			this.RefreshLayout (this.SafeGetViews ());
		}
		
		private void RefreshLayout(IEnumerable<ItemView> views)
		{
			this.hasDirtyLayout = false;
			
			switch (this.Layout)
			{
				case ItemPanelLayout.VerticalList:
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

		private static void NotifyLayoutChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleLayoutChanged ((ItemPanelLayout) oldValue, (ItemPanelLayout) newValue);
		}

		private static void NotifyItemSelectionChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleItemSelectionChanged ((ItemPanelSelectionMode) oldValue, (ItemPanelSelectionMode) newValue);
		}

		private static void NotifyGroupSelectionChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleGroupSelectionChanged ((ItemPanelSelectionMode) oldValue, (ItemPanelSelectionMode) newValue);
		}
		
		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register ("Items", typeof (ICollectionView), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanel.NotifyItemsChanged));
		public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register ("Layout", typeof (ItemPanelLayout), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelLayout.None, ItemPanel.NotifyLayoutChanged));
		public static readonly DependencyProperty ItemSelectionProperty = DependencyProperty.Register ("ItemSelection", typeof (ItemPanelSelectionMode), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelSelectionMode.None, ItemPanel.NotifyItemSelectionChanged));
		public static readonly DependencyProperty GroupSelectionProperty = DependencyProperty.Register ("GroupSelection", typeof (ItemPanelSelectionMode), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelSelectionMode.None, ItemPanel.NotifyGroupSelectionChanged));
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
		List<ItemPanelGroup> groups = new List<ItemPanelGroup> ();
		ItemPanelGroup parentGroup;
		bool hasDirtyLayout;
	}
}
