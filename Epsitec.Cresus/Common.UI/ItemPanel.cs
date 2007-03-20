//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ItemPanel))]

namespace Epsitec.Common.UI
{
	using MouseButtons=Widgets.MouseButtons;
	using MessageType=Widgets.MessageType;
	using KeyCode=Widgets.KeyCode;
	using WidgetPaintState=Widgets.WidgetPaintState;

	/// <summary>
	/// The <c>ItemPanel</c> class represents a collection of items in a panel,
	/// where the collection is defined through a <see cref="ICollectionView"/>.
	/// </summary>
	public class ItemPanel : Widgets.FrameBox
	{
		public ItemPanel()
		{
			this.AutoDoubleClick = true;
			this.InternalState |= Widgets.InternalState.Focusable;
		}

		public ItemPanel(Widgets.Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		static ItemPanel()
		{
			DependencyPropertyMetadata metadata = Widgets.Widget.AutoDoubleClickProperty.DefaultMetadata.Clone ();

			metadata.DefineDefaultValue (true);

			Widgets.Widget.AutoDoubleClickProperty.OverrideMetadata (typeof (ItemPanel), metadata);
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

		public ItemPanelGroup ParentGroup
		{
			get
			{
				return this.parentGroup;
			}
		}

		public ItemPanel RootPanel
		{
			get
			{
				ItemPanelGroup parent = this.ParentGroup;

				if (this.parentGroup != null)
				{
					return this.parentGroup.ParentPanel.RootPanel;
				}
				else
				{
					return this;
				}
			}
		}

		public bool IsRootPanel
		{
			get
			{
				return this.parentGroup == null;
			}
		}

		public int PanelDepth
		{
			get
			{
				if (this.parentGroup != null)
				{
					return this.parentGroup.ParentPanel.PanelDepth + 1;
				}
				else
				{
					return 0;
				}
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

		public ItemViewShape ItemViewShape
		{
			get
			{
				switch (this.Layout)
				{
					case ItemPanelLayout.VerticalList:
						return ItemViewShape.Row;

					case ItemPanelLayout.ColumnsOfTiles:
					case ItemPanelLayout.RowsOfTiles:
						return ItemViewShape.Tile;

					default:
						throw new System.NotSupportedException (string.Format ("Layout {0} not supported", this.Layout));
				}
			}
		}

		public CurrentItemTrackingMode CurrentItemTrackingMode
		{
			get
			{
				return (CurrentItemTrackingMode) this.GetValue (ItemPanel.CurrentItemTrackingModeProperty);
			}
			set
			{
				this.SetValue (ItemPanel.CurrentItemTrackingModeProperty, value);
			}
		}

		public ItemPanelSelectionBehavior SelectionBehavior
		{
			get
			{
				return (ItemPanelSelectionBehavior) this.GetValue (ItemPanel.SelectionBehaviorProperty);
			}
			set
			{
				this.SetValue (ItemPanel.SelectionBehaviorProperty, value);
			}
		}

		public ItemPanelSelectionMode ItemSelectionMode
		{
			get
			{
				return (ItemPanelSelectionMode) this.RootPanel.GetValue (ItemPanel.ItemSelectionModeProperty);
			}
			set
			{
				this.RootPanel.SetValue (ItemPanel.ItemSelectionModeProperty, value);
			}
		}

		public ItemPanelSelectionMode GroupSelectionMode
		{
			get
			{
				return (ItemPanelSelectionMode) this.RootPanel.GetValue (ItemPanel.GroupSelectionModeProperty);
			}
			set
			{
				this.RootPanel.SetValue (ItemPanel.GroupSelectionModeProperty, value);
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

		public bool ItemViewDefaultExpanded
		{
			get
			{
				return (bool) this.GetValue (ItemPanel.ItemViewDefaultExpandedProperty);
			}
			set
			{
				this.SetValue (ItemPanel.ItemViewDefaultExpandedProperty, value);
			}
		}

		public Drawing.Rectangle Aperture
		{
			get
			{
				double yTop = this.PreferredHeight - System.Math.Max (this.apertureY, 0);
				double yBot = System.Math.Max (yTop - this.apertureHeight, 0);

				return new Drawing.Rectangle (this.apertureX, yTop-this.apertureHeight, this.apertureWidth, this.apertureHeight);
			}
			set
			{
				Drawing.Rectangle oldValue = this.Aperture;
				Drawing.Rectangle newValue = value;
				
				if (oldValue != newValue)
				{
					this.apertureWidth = value.Width;
					this.apertureHeight = value.Height;
					this.apertureX = value.Left;
					this.apertureY = this.PreferredHeight - value.Top;
					
					this.HandleApertureChanged (oldValue, newValue);
				}
			}
		}

		public Drawing.Margins AperturePadding
		{
			get
			{
				return (Drawing.Margins) this.GetValue (ItemPanel.AperturePaddingProperty);
			}
			set
			{
				this.SetValue (ItemPanel.AperturePaddingProperty, value);
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

		public int GetTotalRowCount()
		{
			IList<ItemView> views;

			switch (this.Layout)
			{
				case ItemPanelLayout.VerticalList:
					views = this.SafeGetViews ();
					return views.Count;

				case ItemPanelLayout.RowsOfTiles:
					return this.totalRowCount;

				case ItemPanelLayout.ColumnsOfTiles:
					//	TODO: calculer le nombre de lignes
					return 1;
				
				default:
					return -1;
			}
		}

		public int GetTotalColumnCount()
		{
			switch (this.Layout)
			{
				case ItemPanelLayout.VerticalList:
					return 1;

				case ItemPanelLayout.RowsOfTiles:
					return this.totalColumnCount;

				case ItemPanelLayout.ColumnsOfTiles:
					//	TODO: calculer le nombre de colonne
					return 1;

				default:
					return -1;
			}
		}

		public double GetRowHeight()
		{
			return (this.minItemHeight + this.maxItemHeight) / 2;
		}
		
		public int GetVisibleRowCount(double height)
		{
			double itemHeight = this.GetRowHeight ();

			if (itemHeight <= 0)
			{
				return -1;
			}
			else
			{
				return (int) (height / itemHeight);
			}
		}

		public Drawing.Rectangle GetItemViewBounds(ItemView view)
		{
			if (view == null)
			{
				return Drawing.Rectangle.Empty;
			}

			Drawing.Rectangle bounds = view.Bounds;
			ItemPanel         panel  = view.Owner;

			while (panel != this)
			{
				ItemPanelGroup group = panel.ParentGroup;

				bounds = panel.MapClientToParent (bounds);
				bounds = group.MapClientToParent (bounds);
				
				view  = panel.ParentGroup.ItemView;
				panel = view.Owner;

//-				bounds.Offset (view.Bounds.Location);
			}

			return bounds;
		}

		public Drawing.Rectangle GetExpandedItemViewBounds(ItemView view)
		{
			if (view == null)
			{
				return Drawing.Rectangle.Empty;
			}

			Drawing.Rectangle bounds = view.Bounds;
			ItemPanel         panel  = view.Owner;

			while (panel != this)
			{
				ItemPanelGroup group = panel.ParentGroup;

				double top   = panel.PreferredHeight;
				double right = panel.PreferredWidth;

				if (view.Bounds.Top == top)
				{
					bounds.Top += group.GetInternalPadding ().Top + group.Padding.Top + panel.Margins.Top;
				}
				if (view.Bounds.Right == right)
				{
					bounds.Right += group.GetInternalPadding ().Right + group.Padding.Right + panel.Margins.Right;
				}
				if (view.Bounds.Bottom == 0)
				{
					bounds.Bottom -= group.GetInternalPadding ().Bottom + group.Padding.Bottom + panel.Margins.Bottom;
				}
				if (view.Bounds.Left == 0)
				{
					bounds.Left -= group.GetInternalPadding ().Left + group.Padding.Left + panel.Margins.Left;
				}

				bounds = panel.MapClientToParent (bounds);
				bounds = group.MapClientToParent (bounds);

				view  = panel.ParentGroup.ItemView;
				panel = view.Owner;
			}

			return bounds;
		}

		public ItemView GetItemView(int index)
		{
			return ItemPanel.TryGetItemView (this.SafeGetViews (), index);
		}

		public ItemView GetItemViewAtRow(int rowIndex)
		{
			IEnumerable<ItemView> views = this.SafeGetViews ();

			foreach (ItemView view in views)
			{
				if (view.RowIndex == rowIndex)
				{
					return view;
				}
			}

			return null;
		}

		public ItemView GetItemViewAtColumn(int columnIndex)
		{
			IEnumerable<ItemView> views = this.SafeGetViews ();

			foreach (ItemView view in views)
			{
				if (view.ColumnIndex == columnIndex)
				{
					return view;
				}
			}

			return null;
		}

		public ItemView GetItemView(object item)
		{
			IEnumerable<ItemView> views = this.SafeGetViews ();

			foreach (ItemView view in views)
			{
				if (view.Item == item)
				{
					return view;
				}
			}

			return null;
		}

		public ItemView DetectItemView(Drawing.Point pos)
		{
			return this.FindItemView (
				delegate (ItemView item)
				{
					if ((item.IsVisible) &&
						(item.Bounds.Contains (pos)))
					{
						return true;
					}
					else
					{
						return false;
					}
				});
		}

		public ItemView FindItemView(System.Predicate<ItemView> match)
		{
			IEnumerable<ItemView> views = this.SafeGetViews ();

			foreach (ItemView view in views)
			{
				if (match (view))
				{
					return view;
				}

				if (view.IsGroup)
				{
					ItemView hit = view.Group.ChildPanel.FindItemView (match);
					
					if (hit != null)
					{
						return hit;
					}
				}
			}

			return null;
		}

		public ItemView FindItemView(object item)
		{
			if (item == null)
			{
				return null;
			}
			else
			{
				return this.FindItemView (
					delegate (ItemView view)
					{
						return view.Item == item;
					});
			}
		}
		
		public void DeselectAllItemViews()
		{
			SelectionState state = new SelectionState (this);

			IList<ItemView> selectedViews = this.RootPanel.GetSelectedItemViews ();

			if (selectedViews.Count > 0)
			{
				this.InternalDeselectItemViews (selectedViews);
			}

			state.GenerateEvents ();
		}
		
		public void SelectItemView(ItemView view)
		{
			if (view == null)
			{
				return;
			}

			SelectionState state = new SelectionState (this);

			this.InternalSelectItemView (view);

			state.GenerateEvents ();
		}

		#region SelectionState Class

		class SelectionState
		{
			public SelectionState(ItemPanel panel)
			{
				this.panel = panel;
				this.views = new List<ItemView> (this.panel.GetSelectedItemViews ());
			}

			public void GenerateEvents()
			{
				foreach (ItemView itemView in this.panel.GetSelectedItemViews ())
				{
					object item = itemView.Item;
					bool found = false;

					for (int i = 0; i < this.views.Count; i++)
					{
						if (this.views[i].Item == item)
						{
							this.views.RemoveAt (i);
							found = true;
							break;
						}
					}

					if (!found)
					{
						this.panel.OnSelectionChanged ();
						return;
					}
				}

				if (this.views.Count > 0)
				{
					this.panel.OnSelectionChanged ();
				}
			}

			ItemPanel panel;
			List<ItemView> views;
		}

		#endregion

		public void DeselectItemView(ItemView view)
		{
			SelectionState state = new SelectionState (this);

			this.InternalDeselectItemView (view);

			state.GenerateEvents ();
		}

		public void ExpandItemView(ItemView view, bool expand)
		{
			while (view != null)
			{
				if (view.IsGroup)
				{
//-					view.CreateUserInterface ();
					view.IsExpanded = expand;
				}

				ItemPanelGroup group = view.Owner.ParentGroup;

				if (group == null)
				{
					break;
				}

				view = group.ItemView;
			}
		}

		public IList<ItemView> GetSelectedItemViews()
		{
			return this.GetSelectedItemViews (null);
		}
		
		public IList<ItemView> GetSelectedItemViews(System.Predicate<ItemView> filter)
		{
			List<ItemView> list = new List<ItemView> ();
			
			this.GetSelectedItemViews (filter, list);

			return list;
		}

		public Drawing.Size GetContentsSize()
		{
			return this.PreferredSize;
		}

		internal void RefreshLayoutIfNeeded()
		{
			if (this.hasDirtyLayout)
			{
				this.RefreshLayout ();
			}
		}

		public void AsyncRefresh()
		{
			if (this.isRefreshPending == false)
			{
				this.isRefreshPending = true;
				this.QueueAsyncRefresh (RefreshOperations.Content);
			}
		}

		private void ExecuteAsyncRefreshOperations()
		{
			RefreshInfo[] infos;

			while (true)
			{
				lock (this.exclusion)
				{
					infos = this.refreshInfos.ToArray ();
					this.isRefreshing = true;
				}

				if (infos.Length == 0)
				{
					break;
				}

				System.Array.Sort<RefreshInfo> (infos);
				RefreshInfo info = infos[0];

				lock (this.exclusion)
				{
					this.refreshInfos.Remove (info);
				}

				info.Execute ();
			}

			lock (this.exclusion)
			{
				this.isRefreshing = false;
			}
		}

		private void QueueAsyncRefresh(RefreshOperations operations)
		{
			RefreshInfo info = new RefreshInfo (this, operations);
			ItemPanel   root = this.RootPanel;
			
			bool queue = false;

			lock (root.exclusion)
			{
				int index = root.refreshInfos.IndexOf (info);
				
				if (index >= 0)
				{
					info = root.refreshInfos[index];
					info.Add (operations);
				}
				else
				{
					root.refreshInfos.Add (info);
				}

				queue = !root.isRefreshing;
			}

			if (queue)
			{
				if (root.manualTrackingCounter == 0)
				{
					Widgets.Application.QueueAsyncCallback (root.ExecuteAsyncRefreshOperations);
				}
				else
				{
					//	The user expects the update to occur immediately; don't queue
					//	the refresh but execute it instead :
					
					this.ExecuteAsyncRefreshOperations ();
				}
			}
		}

		[System.Flags]
		private enum RefreshOperations
		{
			None = 0,
			Content = 1,
			Layout = 2,
		}

		private class RefreshInfo : System.IComparable<RefreshInfo>, System.IEquatable<RefreshInfo>
		{
			public RefreshInfo(ItemPanel panel, RefreshOperations operations)
			{
				this.panel = panel;
				this.depth = panel.PanelDepth;
				this.operations = operations;
			}


			public RefreshOperations Operations
			{
				get
				{
					return this.operations;
				}
			}

			public ItemPanel Panel
			{
				get
				{
					return this.panel;
				}
			}

			public int Depth
			{
				get
				{
					return this.depth;
				}
			}

			public void Add(RefreshOperations operations)
			{
				this.operations |= operations;
			}

			private ItemPanel panel;
			private int depth;
			private RefreshOperations operations;

			#region IEquatable<RefreshInfo> Members

			public bool Equals(RefreshInfo other)
			{
				return (this.panel == other.panel)
					&& (this.depth == other.depth);
			}

			#endregion

			#region IComparable<RefreshInfo> Members

			public int CompareTo(RefreshInfo other)
			{
				if (this.depth < other.depth)
				{
					return -1;
				}
				else if (this.depth > other.depth)
				{
					return 1;
				}
				else
				{
					return this.panel.VisualSerialId.CompareTo (other.panel.VisualSerialId);
				}
			}

			#endregion

			internal void Execute()
			{
				if ((this.operations & RefreshOperations.Content) != 0)
				{
					this.panel.Refresh ();
				}
				else if ((this.operations & RefreshOperations.Layout) != 0)
				{
					if (this.panel.hasDirtyLayout)
					{
						this.panel.RefreshLayout ();
					}
				}
			}
		}


		public void SyncRefreshIfNeeded()
		{
			if (this.isRefreshPending)
			{
				this.Refresh ();
			}
		}

		public void Refresh()
		{
			bool focus = this.ContainsKeyboardFocus;

			this.isRefreshPending = false;
			this.ClearUserInterface ();
			this.RefreshItemViews ();

			if (this.isCurrentShowPending)
			{
				ICollectionView view = this.RootPanel.Items;

				if (view != null)
				{
					this.isCurrentShowPending = false;
					ItemView item = this.GetItemView (view.CurrentPosition);

					if ((this.ItemSelectionMode == ItemPanelSelectionMode.ExactlyOne) ||
						(this.ItemSelectionMode == ItemPanelSelectionMode.ZeroOrOne))
					{
						this.SelectItemView (item);
					}

					this.Show (item);
				}
			}
			
			if (focus)
			{
			//	this.Focus ();
			//	this.TrackCurrentItem (false);
			}
		}

		internal void ClearUserInterface()
		{
			using (new LockManager (this))
			{
				foreach (ItemView view in this.views)
				{
					view.ClearUserInterface ();
				}
			}
		}

		internal void RefreshUserInterface()
		{
			this.RefreshItemViews ();
		}
		
		public void Show(ItemView view)
		{
			if (view == null)
			{
				return;
			}

			if (this.parentGroup != null)
			{
				this.RootPanel.Show (view);
			}
			else if (this.Aperture.IsSurfaceZero)
			{
				//	Nothing to do : there is no visible aperture, so don't bother
				//	moving it around !
			}
			else
			{
				this.ExpandItemView (view, true);

				if (this.hasDirtyLayout)
				{
					this.RefreshLayout ();
				}

				Drawing.Rectangle aperture = this.Aperture;
				Drawing.Rectangle bounds   = this.GetExpandedItemViewBounds (view);

				aperture.Deflate (this.AperturePadding);

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
						     (aperture.Left > bounds.Left))
					{
						ox = System.Math.Min (aperture.Right - bounds.Right, aperture.Left - bounds.Left);
					}

					if ((aperture.Top < bounds.Top) &&
						(aperture.Bottom < bounds.Bottom))
					{
						oy = System.Math.Max (aperture.Top - bounds.Top, aperture.Bottom - bounds.Bottom);
					}
					else if ((aperture.Top > bounds.Top) &&
						     (aperture.Bottom > bounds.Bottom))
					{
						oy = System.Math.Min (aperture.Top - bounds.Top, aperture.Bottom - bounds.Bottom);
					}
					
					aperture = this.Aperture;

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
			return view.IsGroup;
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
				this.QueueAsyncRefresh (RefreshOperations.Layout);
			}

			if (this.parentGroup != null)
			{
				this.parentGroup.ParentPanel.InvalidateLayout ();
			}
		}

		internal void DefineParentGroup(ItemPanelGroup parentGroup)
		{
			System.Diagnostics.Debug.Assert (this.parentGroup == null);
			
			this.parentGroup = parentGroup;
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

		internal void GetSelectedItemViews(System.Predicate<ItemView> filter, List<ItemView> list)
		{
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

				ItemPanelGroup group = view.Group;

				if (group != null)
				{
					group.ChildPanel.GetSelectedItemViews (filter, list);
				}
			}
		}

		protected virtual void HandleItemsChanged(ICollectionView oldValue, ICollectionView newValue)
		{
			if (oldValue != null)
			{
				oldValue.CollectionChanged -= this.HandleItemCollectionChanged;
				oldValue.CurrentChanged -= this.HandleItemCollectionCurrentChanged;
			}
			if (newValue != null)
			{
				newValue.CollectionChanged += this.HandleItemCollectionChanged;
				newValue.CurrentChanged += this.HandleItemCollectionCurrentChanged;
				
				this.HandleItemCollectionChanged (this, new CollectionChangedEventArgs (CollectionChangedAction.Reset));
				this.HandleItemCollectionCurrentChanged (this);
			}
		}

		protected virtual void HandleLayoutChanged(ItemPanelLayout oldValue, ItemPanelLayout newValue)
		{
			//	TODO: decide whether to call RefreshItemViews or RefreshLayout
			this.AsyncRefresh ();
		}

		protected virtual void HandleItemSelectionModeChanged(ItemPanelSelectionMode oldValue, ItemPanelSelectionMode newValue)
		{
		}

		protected virtual void HandleGroupSelectionModeChanged(ItemPanelSelectionMode oldValue, ItemPanelSelectionMode newValue)
		{
		}

		protected virtual void HandleSelectionBehaviorChanged(ItemPanelSelectionBehavior oldValue, ItemPanelSelectionBehavior newValue)
		{
		}

		protected virtual void HandleItemCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.AsyncRefresh ();
			this.isCurrentShowPending = true;
		}

		protected virtual void HandleItemCollectionCurrentChanged(object sender)
		{
			if (this.RootPanel == this)
			{
				if (this.manualTrackingCounter == 0)
				{
					this.TrackCurrentItem (true);
				}
			}

			if (this.CurrentChanged != null)
			{
				this.CurrentChanged (this);
			}
		}

		private void TrackCurrentItem(bool autoSelect)
		{
			if (this.CurrentItemTrackingMode == CurrentItemTrackingMode.None)
			{
				return;
			}
			
			if (this.PanelDepth == 0)
			{
				object item = this.Items.CurrentItem;

				if (item == null)
				{
					//	No current item at all...
				}
				else
				{
					ItemView view  = this.FindItemView (item);
					bool     focus = this.ContainsKeyboardFocus;

					if (view == null)
					{
						//	The item does not belong to the panel as such, but it
						//	belongs to a (sub)group, most probably.

						IList<CollectionViewGroup> path = CollectionView.GetGroupPath (this.Items, item);

						if (path != null)
						{
							ItemPanel panel = this;

							for (int i = 0; i < path.Count; i++)
							{
								view = panel.FindItemView (path[i]);
								view.IsExpanded = true;

								panel.TrackCurrentItem (view, focus, autoSelect);

								panel = view.Group.ChildPanel;
							}

							view = panel.FindItemView (item);
							panel.TrackCurrentItem (view, focus, autoSelect);
						}
					}

					this.TrackCurrentItem (view, focus, autoSelect);
				}
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("TrackCurrentItem: called for panel at depth=" + this.PanelDepth);
			}
		}

		private void TrackCurrentItem(ItemView view, bool focus, bool autoSelect)
		{
			if (view == null)
			{
				this.ClearFocus ();
			}
			else
			{
				this.Show (view);

				switch (this.CurrentItemTrackingMode)
				{
					case CurrentItemTrackingMode.AutoFocus:
						break;

					case CurrentItemTrackingMode.AutoSelect:
						if (autoSelect)
						{
							IList<ItemView> list = this.GetSelectedItemViews ();
							SelectionState state = new SelectionState (this);

							this.InternalDeselectItemViews (list);
							this.InternalSelectItemView (view);
							
							state.GenerateEvents ();
						}
						break;
				}

				if ((focus) &&
					(view.HasValidUserInterface))
				{
					view.Widget.Focus ();
				}
			}

			this.Invalidate ();
		}

		protected virtual void HandleItemViewDefaultSizeChanged(Drawing.Size oldValue, Drawing.Size newValue)
		{
			this.AsyncRefresh ();
		}
		
		protected virtual void HandleItemViewDefaultExpandedChanged(bool oldValue, bool newValue)
		{
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

			if (this.ApertureChanged != null)
			{
				this.ApertureChanged (this, new DependencyPropertyChangedEventArgs ("Aperture", oldValue, newValue));
			}
		}

		protected virtual void OnSelectionChanged()
		{
			if (this.SelectionChanged != null)
			{
				this.SelectionChanged (this);
			}
		}
		
		protected virtual void OnContentsSizeChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.ContentsSizeChanged != null)
			{
				this.ContentsSizeChanged (this, e);
			}
		}

		protected override void OnPressed(Widgets.MessageEventArgs e)
		{
			base.OnPressed (e);

			if (!e.Suppress)
			{
				ItemPanel root = this.RootPanel;
				
				root.manualTrackingCounter++;

				try
				{
					if (e.Message.Button == MouseButtons.Left)
					{
						this.HandleItemViewPressed (e.Message, this.Detect (e.Point));
					}
				}
				finally
				{
					root.manualTrackingCounter--;
				}
			}
		}

		protected override void OnReleased(Epsitec.Common.Widgets.MessageEventArgs e)
		{
			base.OnReleased (e);
			e.Message.Consumer = this;
		}

		private void HandleItemViewPressed(Widgets.Message message, ItemView view)
		{
			if (view == null)
			{
				return;
			}
			
			if (view.Owner != this)
			{
				view.Owner.HandleItemViewPressed (message, view);
			}
			else
			{
				IList<ItemView> list = this.GetSelectedItemViews ();
				bool modifySelection = message.IsControlPressed;
				
				if (message.IsShiftPressed && list.Count > 0)
				{
					this.ContinuousMouseSelection (list, this.FindItemView (this.Items.CurrentItem), view, modifySelection);
				}
				else
				{
					this.ManualSelection (view, modifySelection);
				}

				message.Consumer = this;
				view.Widget.Focus ();
			}
		}

		protected override void OnContainsFocusChanged()
		{
			base.OnContainsFocusChanged ();

			if (this.ContainsKeyboardFocus)
			{
				this.RefreshFocus (true);
			}
			else
			{
				this.RefreshFocus (this.KeyboardFocus);
			}
		}

		protected override void ProcessMessage(Widgets.Message message, Drawing.Point pos)
		{
			ItemPanel root = this.RootPanel;
			
			root.manualTrackingCounter++;
			
			try
			{
				base.ProcessMessage (message, pos);

				if (message.IsMouseType)
				{
					if (message.MessageType == MessageType.MouseDown)
					{
						this.Focus ();
					}

					if (message.MessageType == MessageType.MouseMove &&
						message.Button == MouseButtons.None)
					{
						ItemView item = this.Detect (pos);

						if (this.enteredItemView != item)
						{
							this.enteredItemView = item;
							this.Invalidate ();
						}
					}
				}
				else if (message.IsKeyType)
				{
					if (message.MessageType == MessageType.KeyDown)
					{
						root.ProcessKeyDown (message);
					}
				}
			}
			finally
			{
				root.manualTrackingCounter--;
			}
		}


		/// <summary>
		/// Processes the key down event.
		/// </summary>
		/// <param name="message">The event message.</param>
		private void ProcessKeyDown(Widgets.Message message)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);
			System.Diagnostics.Debug.Assert (this.Items != null);

			if (message.IsAltPressed)
			{
				return;
			}

			switch (message.KeyCode)
			{
				case KeyCode.Space:
					this.ProcessSpaceKey (message.IsControlPressed);
					message.Handled = true;
					break;

				case KeyCode.Add:
					this.ProcessExpandKey (true);
					message.Handled = true;
					break;

				case KeyCode.Substract:
					this.ProcessExpandKey (false);
					message.Handled = true;
					break;
			}
			
			if (!message.Handled)
			{
				//	Change the current item (this does not select the item).

				ItemView oldFocusedItemView = this.GetFocusedItemView ();
				ItemView newFocusedItemView;

				if (this.ProcessNavigationKeys (message.KeyCode))
				{
					ItemView currentItemView = this.FindItemView (this.Items.CurrentItem);
					
					if (currentItemView != null)
					{
						currentItemView.Owner.TrackCurrentItem (currentItemView, true, false);
					}

					newFocusedItemView = this.GetFocusedItemView ();

					if ((!message.IsControlPressed) &&
						(!message.IsAltPressed))
					{
						IList<ItemView> list = this.GetSelectedItemViews ();
						SelectionState state = new SelectionState (this);

						if (message.IsShiftPressed && list.Count > 0)
						{
							this.ContinuousKeySelection (list, oldFocusedItemView, newFocusedItemView);
						}
						else
						{
							this.InternalDeselectItemViews (list);
							this.InternalSelectItemView (newFocusedItemView);
						}

						state.GenerateEvents ();
					}

					if (oldFocusedItemView != newFocusedItemView)
					{
						if (newFocusedItemView != null)
						{
							newFocusedItemView.Owner.Show (newFocusedItemView);
						}
					}
					
					message.Handled = true;
				}
			}
		}

		private void ProcessExpandKey(bool expand)
		{
			ItemView itemView = this.GetFocusedItemView ();

			if (itemView != null)
			{
				ItemView group = ItemViewWidget.FindGroupItemView (itemView);

				if (group != null)
				{
					group.IsExpanded = expand;
					group.Owner.Focus (group);
				}
			}
		}

		private void ProcessSpaceKey(bool modifySelection)
		{
			//	Behaves as if the user had pressed the mouse button; this will
			//	either select just the current item or add/remove the item from
			//	the selection if CTRL is pressed.

			ItemView itemView = this.GetFocusedItemView ();

			if (itemView != null)
			{
				itemView.Owner.ManualSelection (itemView, modifySelection);
			}
		}

		private void Focus(ItemView itemView)
		{
			if (itemView != null)
			{
				this.RootPanel.RecordFocus (itemView.Item);

				if (itemView.HasValidUserInterface)
				{
					ItemViewWidget widget = itemView.Widget;
					widget.Focus ();
				}
			}
		}




		private void ContinuousKeySelection(IList<ItemView> list, ItemView oldCurrent, ItemView newCurrent)
		{
			ItemView first = ItemPanel.GetFirstSelectedItemView (list);
			ItemView last  = ItemPanel.GetLastSelectedItemView (list);

			int index1 = (first == null) ? -1 : first.Index;
			int index2 = (last  == null) ? -1 : last.Index;

			int oldIndex = (oldCurrent == null) ? -1 : oldCurrent.Index;
			int newIndex = (newCurrent == null) ? -1 : newCurrent.Index;

			if (oldIndex == index1)
			{
				index1 = newIndex;
			}
			else if (oldIndex == index2)
			{
				index2 = newIndex;
			}
			else
			{
				index1 = newIndex;
				index2 = newIndex;
			}

			index1 = System.Math.Max (0, index1);
			index2 = System.Math.Max (0, index2);

			if (index1 > index2)
			{
				int temp = index1;
				index1 = index2;
				index2 = temp;
			}
			
			if (newIndex < oldIndex)
			{
				for (int i = index2; i >= index1; i--)
				{
					this.InternalSelectItemView (this.GetItemView (i));
				}
			}
			else
			{
				for (int i = index1; i <= index2; i++)
				{
					this.InternalSelectItemView (this.GetItemView (i));
				}
			}

			List<ItemView> deselect = new List<ItemView> ();
			foreach (ItemView view in list)
			{
				if (view.Index < index1 ||
					view.Index > index2)
				{
					deselect.Add (view);
				}
			}
			this.InternalDeselectItemViews (deselect);

			this.Items.MoveCurrentToPosition (newIndex);
		}

		private void ContinuousMouseSelection(IList<ItemView> list, ItemView current, ItemView clicked, bool expand)
		{
			int currentIndex = current.Index;
			int clickedIndex = clicked.Index;
			int index1 = -1;
			int index2 = -1;

			if (currentIndex < clickedIndex)
			{
				for (int i=currentIndex; i<=clickedIndex; i++)
				{
					this.InternalSelectItemView(this.GetItemView(i));
				}

				index1 = currentIndex;
				index2 = clickedIndex;
			}

			if (currentIndex > clickedIndex)
			{
				for (int i=currentIndex; i>=clickedIndex; i--)
				{
					this.InternalSelectItemView(this.GetItemView(i));
				}

				index1 = clickedIndex;
				index2 = currentIndex;
			}

			if (!expand && index1 != -1 && index2 != -1)
			{
				List<ItemView> deselect = new List<ItemView>();
				foreach (ItemView view in list)
				{
					if (view.Index < index1 ||
					view.Index > index2)
					{
						deselect.Add(view);
					}
				}
				this.InternalDeselectItemViews(deselect);
			}

			this.Items.MoveCurrentToPosition(clickedIndex);
		}

		private bool ProcessNavigationKeys(Widgets.KeyCode keyCode)
		{
			switch (this.Layout)
			{
				case ItemPanelLayout.VerticalList:
					return this.ProcessArrowKeysVerticalList (keyCode);

				case ItemPanelLayout.RowsOfTiles:
					return this.ProcessArrowKeysRowsOfTiles (keyCode);

				case ItemPanelLayout.ColumnsOfTiles:
					break;
			}

			return false;
		}

		private bool ProcessArrowKeysVerticalList(Widgets.KeyCode keyCode)
		{
			int pos = this.Items.CurrentPosition;

			switch (keyCode)
			{
				case KeyCode.ArrowUp:
					if (pos > 0)
					{
						this.Items.MoveCurrentToPrevious ();
					}
					break;

				case KeyCode.ArrowDown:
					if (pos < this.Items.Items.Count-1)
					{
						this.Items.MoveCurrentToNext ();
					}
					break;

				case KeyCode.Home:
					this.Items.MoveCurrentToFirst ();
					break;

				case KeyCode.End:
					this.Items.MoveCurrentToLast ();
					break;

				case KeyCode.PageUp:
					this.ProcessPageUpMove ();
					break;

				case KeyCode.PageDown:
					this.ProcessPageDownMove ();
					break;

				default:
					return false;
			}

			return true;
		}

		private bool ProcessArrowKeysRowsOfTiles(Widgets.KeyCode keyCode)
		{
			int pos = this.Items.CurrentPosition;

			switch (keyCode)
			{
				case KeyCode.ArrowLeft:
					if (pos > 0 && pos%this.totalColumnCount > 0)
					{
						this.Items.MoveCurrentToPrevious ();
					}
					break;

				case KeyCode.ArrowRight:
					if (pos < this.Items.Items.Count-1 && pos%this.totalColumnCount < this.totalColumnCount-1)
					{
						this.Items.MoveCurrentToNext ();
					}
					break;

				case KeyCode.ArrowUp:
					pos -= this.totalColumnCount;
					if (pos >= 0)
					{
						this.Items.MoveCurrentToPosition (pos);
					}
					break;

				case KeyCode.ArrowDown:
					pos += this.totalColumnCount;
					if (pos < this.Items.Items.Count)
					{
						this.Items.MoveCurrentToPosition (pos);
					}
					break;

				case KeyCode.Home:
					this.Items.MoveCurrentToFirst ();
					break;

				case KeyCode.End:
					this.Items.MoveCurrentToLast ();
					break;

				case KeyCode.PageUp:
					this.ProcessPageUpMove ();
					break;

				case KeyCode.PageDown:
					this.ProcessPageDownMove ();
					break;

				default:
					return false;
			}

			return true;
		}

		private void ProcessPageUpMove()
		{
			//	Déplace sur le premier visible entièrement ou, s'il est déjà l'élément courant,
			//	une page avant, en tenant compte de la géométrie des items.
			int pos = this.Items.CurrentPosition;
			int first, last;
			this.GetFirstAndListVisibleItem(out first, out last);
			
			if (this.Layout == ItemPanelLayout.VerticalList)
			{
				if (pos == first)
				{
					pos = pos-(last-first-1);
				}
				else
				{
					pos = first;
				}
			}

			if (this.Layout == ItemPanelLayout.RowsOfTiles)
			{
				int visibleRows = (last-first+1)/this.totalColumnCount;

				if (pos/this.totalColumnCount == first/this.totalColumnCount)
				{
					pos = pos-visibleRows*this.totalColumnCount;
				}
				else
				{
					pos = first + pos%this.totalColumnCount;
				}
			}

			pos = System.Math.Max(pos, 0);
			pos = System.Math.Min(pos, this.Items.Items.Count-1);
			this.Items.MoveCurrentToPosition(pos);
		}

		private void ProcessPageDownMove()
		{
			//	Déplace sur le dernier visible entièrement ou, s'il est déjà l'élément courant,
			//	une page après, en tenant compte de la géométrie des items.
			int pos = this.Items.CurrentPosition;
			int first, last;
			this.GetFirstAndListVisibleItem(out first, out last);
			
			if (this.Layout == ItemPanelLayout.VerticalList)
			{
				if (pos == last-1)
				{
					pos = pos+(last-first-1);
				}
				else
				{
					pos = last-1;
				}
			}

			if (this.Layout == ItemPanelLayout.RowsOfTiles)
			{
				int visibleRows = (last-first+1)/this.totalColumnCount;

				if (pos/this.totalColumnCount == last/this.totalColumnCount)
				{
					pos = pos+visibleRows*this.totalColumnCount;
				}
				else
				{
					pos = last/this.totalColumnCount*this.totalColumnCount + pos%this.totalColumnCount;
				}
			}

			pos = System.Math.Max(pos, 0);
			pos = System.Math.Min(pos, this.Items.Items.Count-1);
			this.Items.MoveCurrentToPosition(pos);
		}

		private void GetFirstAndListVisibleItem(out int first, out int last)
		{
			IEnumerable<ItemView> views = this.SafeGetViews();
			first = 0;
			last = 0;
			bool inside = false;
			int index = 0;

			foreach (ItemView view in views)
			{
				index = view.Index;

				if (view.IsVisible)
				{
					if (!inside)  // premier visible ?
					{
						first = index;
						inside = true;
					}
				}
				else
				{
					if (inside)  // premier caché ?
					{
						last = index-1;
						inside = false;
					}
				}
			}

			if (inside)
			{
				last = index;
			}
		}

		private void ManualSelection(ItemView view, bool modifySelection)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);

			if (view == null)
			{
				return;
			}
			
			bool select = !view.IsSelected;
			bool action = false;

			SelectionState state = new SelectionState (this);

			switch (this.SelectionBehavior)
			{
				case ItemPanelSelectionBehavior.Automatic:
					action = true;
					break;

				case ItemPanelSelectionBehavior.Manual:
					action = true;
					if (!modifySelection)
					{
						this.InternalDeselectItemViews (this.GetSelectedItemViews ());
					}
					break;

				case ItemPanelSelectionBehavior.ManualOne:
					action = true;
					if (!modifySelection)
					{
						this.InternalDeselectItemViews (this.GetSelectedItemViews ());
						select = true;
					}
					break;
			}

			if (action)
			{
				if (select)
				{
					this.InternalSelectItemView (view);
				}
				else
				{
					this.InternalDeselectItemView (view);
				}
			}

			state.GenerateEvents ();
		}

		private static ItemView GetFirstSelectedItemView(IEnumerable<ItemView> list)
		{
			int      index = int.MaxValue;
			ItemView found = null;

			foreach (ItemView view in list)
			{
				if (view.Index < index)
				{
					index = view.Index;
					found = view;
				}
			}

			return found;
		}

		private static ItemView GetLastSelectedItemView(IEnumerable<ItemView> list)
		{
			int      index = -1;
			ItemView found = null;

			foreach (ItemView view in list)
			{
				if (view.Index > index)
				{
					index = view.Index;
					found = view;
				}
			}

			return found;
		}

		internal object GetFocusedItem()
		{
			return this.RootPanel.focusedItem;
		}

		internal ItemView GetFocusedItemView()
		{
			ItemPanel root = this.RootPanel;
			object    item = root.focusedItem;

			if (item == null)
			{
				return null;
			}
			else
			{
				return this.FindItemView (item);
			}
		}

		protected override void OnExited(Widgets.MessageEventArgs e)
		{
			base.OnExited(e);

			if (this.enteredItemView != null)
			{
				this.enteredItemView = null;
				this.Invalidate();
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
#if false
			IEnumerable<ItemView> views = this.SafeGetViews ();
			Widgets.IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect = Drawing.Rectangle.Intersection (clipRect, this.Aperture);
			ICollectionView items = this.RootPanel.Items;

			object focusedItem = items == null ? null : items.CurrentItem;

			foreach (ItemView view in views)
			{
				if (view.Bounds.IntersectsWith (rect))
				{
					WidgetPaintState state = WidgetPaintState.Enabled;
					Drawing.Rectangle viewBounds = Drawing.Rectangle.Intersection (rect, view.Bounds);

					if (view.IsSelected)
					{
						state |= WidgetPaintState.Selected;
					}
					if (this.IsFocused)
					{
						if (view.Item == focusedItem)
						{
							state |= WidgetPaintState.Focused;
						}

						state |= WidgetPaintState.InheritedFocus;
					}
					if (view == this.enteredItem)
					{
						state |= WidgetPaintState.Entered;
					}

					adorner.PaintCellBackground (graphics, view.Bounds, state);
				}
			}
#endif
		}

		protected override void DispatchMessage(Widgets.Message message, Drawing.Point pos)
		{
			base.DispatchMessage (message, pos);
		}
		
		private IList<ItemView> SafeGetViews()
		{
			using (new LockManager (this))
			{
				return this.views;
			}
		}

		private void InternalSelectItemView(ItemView view)
		{
			if (view.IsSelected)
			{
				this.SetItemViewSelection (view, true);
			}
			else
			{
				IList<ItemView> selectedViews;
				ItemPanelSelectionMode selectionMode;

				if (ItemPanel.ContainsItem (view))
				{
					//	The view specifies a plain item.

					selectedViews = this.RootPanel.GetSelectedItemViews (ItemPanel.ContainsItem);
					selectionMode = this.ItemSelectionMode;
				}
				else
				{
					//	The view specifies a group.

					selectedViews = this.RootPanel.GetSelectedItemViews (ItemPanel.ContainsGroup);
					selectionMode = this.GroupSelectionMode;
				}

				System.Diagnostics.Debug.Assert (selectedViews.Contains (view) == false);

				switch (selectionMode)
				{
					case ItemPanelSelectionMode.None:
						this.InternalDeselectItemViews (selectedViews);
						break;

					case ItemPanelSelectionMode.ExactlyOne:
					case ItemPanelSelectionMode.ZeroOrOne:
						this.InternalDeselectItemViews (selectedViews);
						this.SetItemViewSelection (view, true);
						break;

					case ItemPanelSelectionMode.Multiple:
					case ItemPanelSelectionMode.OneOrMore:
						this.SetItemViewSelection (view, true);
						break;
				}
			}
		}

		private void InternalDeselectItemView(ItemView view)
		{
			if (view.IsSelected)
			{
				IList<ItemView> selectedViews;
				ItemPanelSelectionMode selectionMode;

				if (ItemPanel.ContainsItem (view))
				{
					//	The view specifies a plain item.

					selectedViews = this.RootPanel.GetSelectedItemViews (ItemPanel.ContainsItem);
					selectionMode = this.ItemSelectionMode;
				}
				else
				{
					//	The view specifies a group.

					selectedViews = this.RootPanel.GetSelectedItemViews (ItemPanel.ContainsGroup);
					selectionMode = this.GroupSelectionMode;
				}

				System.Diagnostics.Debug.Assert (selectedViews.Contains (view));

				switch (selectionMode)
				{
					case ItemPanelSelectionMode.None:
						this.InternalDeselectItemViews (selectedViews);
						break;

					case ItemPanelSelectionMode.ExactlyOne:
					case ItemPanelSelectionMode.OneOrMore:
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

		private void InternalDeselectItemViews(IEnumerable<ItemView> views)
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
				view.Select (selection);
				
				if (view.IsVisible)
				{
					this.Invalidate (Drawing.Rectangle.Intersection (this.Aperture, view.Bounds));
				}
			}

			if (selection)
			{
				if (view.IsGroup)
				{
					//	A group was selected. This will not change the current item in
					//	the collection view.
				}
				else
				{
					ItemPanel rootPanel = this.RootPanel;
					ICollectionView items = rootPanel.Items;
					
					if (items != null)
					{
						items.MoveCurrentTo (view.Item);
						
						rootPanel.Invalidate ();
					}
				}
			}
		}

		private void RecreateUserInterface(IEnumerable<ItemView> views, Drawing.Rectangle aperture)
		{
			List<ItemView> clear  = new List<ItemView> ();
			List<ItemView> create = new List<ItemView> ();
			
			foreach (ItemView view in views)
			{
				if (view.HasValidUserInterface)
				{
					if (view.Bounds.IntersectsWith (aperture))
					{
						//	Nothing to do : the item has a user interface and it will
						//	still be visible in the aperture.
					}
					else
					{
						//	The view is no longer visible; remember to dispose or clear the
						//	user interface.

						clear.Add (view);
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

			clear.ForEach (
				delegate (ItemView view)
				{
					view.ClearUserInterface ();
				} );
			
			create.ForEach (
				delegate (ItemView view)
				{
					view.CreateUserInterface ();
				} );
		}

		private void RefreshItemViews()
		{
			RefreshState    state = new RefreshState (this);
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

			if ((items != null) &&
				(items.Items.Count > 0))
			{
				//	We are executing in the root panel; create the item views for
				//	every item (or every group and sub-group) :
				
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
				//	We are executing in a sub-panel, which is used to represent
				//	the contents of a group :
				
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

			if (this.Aperture.IsValid)
			{
				this.RecreateUserInterface (views, this.Aperture);
			}

			List<ItemView> dispose = new List<ItemView> ();
			
			using (new LockManager (this))
			{
				foreach (ItemView view in this.views)
				{
					if (view.Widget != null)
					{
						if (!views.Contains (view))
						{
							dispose.Add (view);
						}
					}
				}
				
				this.views = views;
				this.hotItemViewIndex = -1;
			}

			dispose.ForEach (
				delegate (ItemView view)
				{
					view.DisposeUserInterface ();
				});


			state.GenerateEvents ();
		}
		
		public void RefreshLayout()
		{
			RefreshState state = new RefreshState (this);

			this.RefreshLayout (this.SafeGetViews ());

			state.GenerateEvents ();
		}

		private void RefreshLayout(IEnumerable<ItemView> views)
		{
			foreach (ItemView view in views)
			{
				view.UpdateSize ();
			}
			
			this.hasDirtyLayout = false;
			
			switch (this.Layout)
			{
				case ItemPanelLayout.VerticalList:
					this.LayoutVerticalList (views);
					break;

				case ItemPanelLayout.RowsOfTiles:
					this.LayoutRowsOfTiles (views);
					break;
				
				case ItemPanelLayout.ColumnsOfTiles:
					this.LayoutColumnsOfTiles (views);
					break;
			}
		}

		private void CreateItemViews(IList<ItemView> views, System.Collections.IList list, Dictionary<object, ItemView> currentViews)
		{
			lock (list)
			{
				foreach (object item in list)
				{
					ItemView view;

					//	Try to find and recycle the view for the specified item; this
					//	not only speeds up recreation of the item views, it also allows
					//	to keep track of the state associated with an item (selected or
					//	expanded, for instance).

					if (currentViews.TryGetValue (item, out view))
					{
						currentViews.Remove (item);
						view.DefineIndex (views.Count);
						views.Add (view);
					}
					else
					{
						views.Add (this.CreateItemView (item, views.Count));
					}
				}
			}
		}

		private ItemView CreateItemView(object item, int index)
		{
			ItemView view = new ItemView (item, this, this.ItemViewDefaultSize);
			
			view.DefineIndex (index);
			view.IsExpanded = this.ItemViewDefaultExpanded;

			if (view.IsGroup)
			{
				if (view.Group == null)
				{
					view.DefineGroup (new ItemPanelGroup (view));
				}

				view.Group.ChildPanel.RefreshItemViews ();
				view.DefineSize (view.Group.GetBestFitSize ());
			}

			return view;
		}

		private void LayoutVerticalList(IEnumerable<ItemView> views)
		{
			double minDy = 0;
			double minDx = 0;
			double maxDy = 0;
			double maxDx = 0;

			double dy = 0;
			
			foreach (ItemView view in views)
			{
				Drawing.Size size = view.Size;

				if (dy == 0)
				{
					minDx = size.Width;
					minDy = size.Height;
					maxDx = size.Width;
					maxDy = size.Height;
				}
				else
				{
					minDx = System.Math.Min (minDx, size.Width);
					minDy = System.Math.Min (minDy, size.Height);
					maxDx = System.Math.Min (maxDx, size.Width);
					maxDy = System.Math.Min (maxDy, size.Height);
				}

				dy += size.Height;
			}

			this.minItemWidth  = minDx;
			this.maxItemWidth  = maxDx;
			
			this.minItemHeight = minDy;
			this.maxItemHeight = maxDy;

			double y = dy;
			int row = 0;
			
			this.UpdatePreferredSize (maxDx, dy);
			
			foreach (ItemView view in views)
			{
				double h = view.Size.Height;
				y -= h;
				view.Bounds = new Drawing.Rectangle (0, y, maxDx, h);
				view.RowIndex = row++;
				view.ColumnIndex = 0;
			}
		}

		private void LayoutRowsOfTiles(IEnumerable<ItemView> views)
		{
			double width = this.Aperture.Width;

			int rowCount = 0;
			int colCount = 0;
			
			double minDy = 0;
			double minDx = 0;
			double maxDy = 0;
			double maxDx = 0;

			double dx = 0;
			double dy = 0;
			double ly = 0;
			double lx = 0;

			int c = 0;

			foreach (ItemView view in views)
			{
				Drawing.Size size = view.Size;

				if ((dx == 0) && (dy == 0))
				{
					minDx = size.Width;
					minDy = size.Height;
					maxDx = size.Width;
					maxDy = size.Height;
				}
				else
				{
					minDx = System.Math.Min (minDx, size.Width);
					minDy = System.Math.Min (minDy, size.Height);
					maxDx = System.Math.Min (maxDx, size.Width);
					maxDy = System.Math.Min (maxDy, size.Height);
				}

				dx += size.Width;

				if (dx > width)
				{
					colCount = System.Math.Max (c, colCount);

					dx  = size.Width;
					dy += ly;
					ly  = 0;
					c   = 0;
					rowCount++;
				}

				c++;
				lx = System.Math.Max (lx, dx);
				ly = System.Math.Max (ly, size.Height);
			}

			colCount = System.Math.Max (c, colCount);

			this.minItemWidth  = minDx;
			this.maxItemWidth  = maxDx;

			this.minItemHeight = minDy;
			this.maxItemHeight = maxDy;

			if (ly > 0)
			{
				dy += ly;
				rowCount++;
			}

			double x = 0;
			double y = dy;

			ly = 0;
			int row = 0;
			int column = 0;

			foreach (ItemView view in views)
			{
				double h = view.Size.Height;
				double w = view.Size.Width;

				if (x+w > width)
				{
					x  = 0;
					y -= ly;
					ly = 0;
					column = 0;
					row++;
				}

				view.Bounds = new Drawing.Rectangle (x, y-h, w, h);
				view.RowIndex = row;
				view.ColumnIndex = column;

				x += w;
				ly = System.Math.Max (ly, h);
				column++;
			}

			this.totalRowCount    = rowCount;
			this.totalColumnCount = colCount;
			this.UpdatePreferredSize (lx, dy);
		}

		private void LayoutColumnsOfTiles(IEnumerable<ItemView> views)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		private void UpdatePreferredSize(double width, double height)
		{
			Drawing.Size oldValue = this.PreferredSize;

			this.PreferredWidth  = width;
			this.PreferredHeight = height;
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

		#region RefreshState Class

		class RefreshState
		{
			public RefreshState(ItemPanel panel)
			{
				this.panel = panel;
				this.savedContentsSize = this.panel.GetContentsSize ();
			}

			
			public void GenerateEvents()
			{
				Drawing.Size oldContentsSize = this.savedContentsSize;
				Drawing.Size newContentsSize = this.panel.GetContentsSize ();

				if (oldContentsSize != newContentsSize)
				{
					this.panel.OnContentsSizeChanged (new DependencyPropertyChangedEventArgs ("ContentsSize", oldContentsSize, newContentsSize));
				}
			}

			private ItemPanel panel;
			private Drawing.Size savedContentsSize;
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

		private static void NotifyItemSelectionModeChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleItemSelectionModeChanged ((ItemPanelSelectionMode) oldValue, (ItemPanelSelectionMode) newValue);
		}

		private static void NotifyGroupSelectionModeChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleGroupSelectionModeChanged ((ItemPanelSelectionMode) oldValue, (ItemPanelSelectionMode) newValue);
		}

		private static void NotifySelectionBehaviorChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleSelectionBehaviorChanged ((ItemPanelSelectionBehavior) oldValue, (ItemPanelSelectionBehavior) newValue);
		}

		private static void NotifyItemViewDefaultSizeChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleItemViewDefaultSizeChanged ((Drawing.Size) oldValue, (Drawing.Size) newValue);
		}

		private static void NotifyItemViewDefaultExpandedChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleItemViewDefaultExpandedChanged ((bool) oldValue, (bool) newValue);
		}


		public event Support.EventHandler<DependencyPropertyChangedEventArgs> ApertureChanged;

		public event Support.EventHandler<DependencyPropertyChangedEventArgs> ContentsSizeChanged;

		public event Support.EventHandler SelectionChanged;

		public event Support.EventHandler CurrentChanged;

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register ("Items", typeof (ICollectionView), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanel.NotifyItemsChanged));
		public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register ("Layout", typeof (ItemPanelLayout), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelLayout.None, ItemPanel.NotifyLayoutChanged));
		public static readonly DependencyProperty ItemSelectionModeProperty = DependencyProperty.Register ("ItemSelectionMode", typeof (ItemPanelSelectionMode), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelSelectionMode.None, ItemPanel.NotifyItemSelectionModeChanged));
		public static readonly DependencyProperty GroupSelectionModeProperty = DependencyProperty.Register ("GroupSelectionMode", typeof (ItemPanelSelectionMode), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelSelectionMode.None, ItemPanel.NotifyGroupSelectionModeChanged));
		public static readonly DependencyProperty SelectionBehaviorProperty = DependencyProperty.Register ("SelectionBehavior", typeof (ItemPanelSelectionBehavior), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelSelectionBehavior.Automatic, ItemPanel.NotifySelectionBehaviorChanged));
		public static readonly DependencyProperty ItemViewDefaultSizeProperty = DependencyProperty.Register ("ItemViewDefaultSize", typeof (Drawing.Size), typeof (ItemPanel), new DependencyPropertyMetadata (new Drawing.Size (80, 20), ItemPanel.NotifyItemViewDefaultSizeChanged));
		public static readonly DependencyProperty ItemViewDefaultExpandedProperty = DependencyProperty.Register ("ItemViewDefaultExpanded", typeof (bool), typeof (ItemPanel), new DependencyPropertyMetadata (false, ItemPanel.NotifyItemViewDefaultExpandedChanged));
		public static readonly DependencyProperty AperturePaddingProperty = DependencyProperty.Register ("AperturePadding", typeof (Drawing.Margins), typeof (ItemPanel), new DependencyPropertyMetadata (Drawing.Margins.Zero));
		public static readonly DependencyProperty CurrentItemTrackingModeProperty = DependencyProperty.Register ("CurrentItemTrackingMode", typeof (CurrentItemTrackingMode), typeof (ItemPanel), new DependencyPropertyMetadata (CurrentItemTrackingMode.None));

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

		List<ItemView>					lockedViews = new List<ItemView> ();
		object							exclusion = new object ();
		int								lockAcquired;
		int								lockOwnerPid;
		int								hotItemViewIndex = -1;
		double							apertureX, apertureY, apertureWidth, apertureHeight;
		List<ItemPanelGroup>			groups = new List<ItemPanelGroup> ();
		List<RefreshInfo>				refreshInfos = new List<RefreshInfo> ();
		
		bool							hasDirtyLayout;
		bool							isRefreshing;
		bool							isRefreshPending;
		bool							isCurrentShowPending;
		int								manualTrackingCounter;

		ItemPanelGroup					parentGroup;

		ItemView						enteredItemView;
		object							focusedItem;

		double							minItemWidth;
		double							maxItemWidth;
		double							minItemHeight;
		double							maxItemHeight;

		int								totalRowCount;
		int								totalColumnCount;

		internal void NotifyFocusChanged(ItemViewWidget widget, bool focus)
		{
			ItemPanel root = this.RootPanel;
			ItemView  view = widget.ItemView;
			object    item = view.Item;

			System.Diagnostics.Debug.WriteLine (string.Format ("NotifyFocus: {0} {1} {2}", view.Index, focus, view.Item));

			if (focus)
			{
				root.RecordFocus (item);
			}
			else if (root.focusedItem == item)
			{
				root.ClearFocus ();
			}
		}

		internal void RecordFocus(object item)
		{
			if (this.focusedItem == item)
			{
				return;
			}

			System.Diagnostics.Debug.Assert (item != null);
			System.Diagnostics.Debug.WriteLine (string.Format ("RecordFocus: {0} item={1} group={2}", item, this.Items.Items.IndexOf (item), this.Items.Groups.IndexOf (item as CollectionViewGroup)));
			
			this.focusedItem = item;
		}

		internal void ClearFocus()
		{
			if (this.focusedItem == null)
			{
				return;
			}

			System.Diagnostics.Debug.WriteLine (string.Format ("ClearFocus: {0} item={1} group={2}", this.focusedItem, this.Items.Items.IndexOf (this.focusedItem), this.Items.Groups.IndexOf (this.focusedItem as CollectionViewGroup)));

			this.focusedItem = null;
		}

		internal void NotifyWidgetClicked(ItemViewWidget widget, Widgets.Message message, Drawing.Point pos)
		{
			ItemPanel root = this.RootPanel;

			root.manualTrackingCounter++;

			try
			{
				if (message.Button == MouseButtons.Left)
				{
					this.HandleItemViewPressed (message, widget.ItemView);
				}
			}
			finally
			{
				root.manualTrackingCounter--;
			}
		}
	}
}
