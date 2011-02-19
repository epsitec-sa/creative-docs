//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	public class ItemPanel : Widgets.Widget
	{
		public ItemPanel()
		{
			this.AutoDoubleClick = true;
			this.InternalState |= Widgets.WidgetInternalState.Focusable;
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

		/// <summary>
		/// Gets or sets the <see cref="ICollectionView"/> collection of items
		/// represented by this panel.
		/// </summary>
		/// <value>The collection view.</value>
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

		/// <summary>
		/// Gets the parent group for this panel, if any.
		/// </summary>
		/// <value>The parent group or <c>null</c> if the panel does not belong
		/// to a group.</value>
		public ItemPanelGroup ParentGroup
		{
			get
			{
				return this.parentGroup;
			}
		}

		/// <summary>
		/// Gets the root panel.
		/// </summary>
		/// <value>The root panel or <c>this</c> if this panel is at the root
		/// of the tree.</value>
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

		/// <summary>
		/// Gets a value indicating whether this panel is the root panel of the
		/// tree.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this panel is the root panel; otherwise, <c>false</c>.
		/// </value>
		public bool IsRootPanel
		{
			get
			{
				return this.parentGroup == null;
			}
		}

		/// <summary>
		/// Gets the panel depth in the tree.
		/// </summary>
		/// <value>The panel depth (the root panel is at depth <c>0</c>).</value>
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

		/// <summary>
		/// Gets or sets the layout for the root panel.
		/// </summary>
		/// <value>The layout of the panel.</value>
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

		/// <summary>
		/// Gets or sets the layout for the top level groups.
		/// </summary>
		/// <value>The layout of the groups.</value>
		public ItemPanelLayout LayoutGroups
		{
			get
			{
				return (ItemPanelLayout) this.GetValue (ItemPanel.LayoutGroupsProperty);
			}
			set
			{
				this.SetValue (ItemPanel.LayoutGroupsProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the layout for the subgroups of the top level groups.
		/// </summary>
		/// <value>The layout of the subgroups.</value>
		public ItemPanelLayout LayoutSubgroups
		{
			get
			{
				return (ItemPanelLayout) this.GetValue (ItemPanel.LayoutSubgroupsProperty);
			}
			set
			{
				this.SetValue (ItemPanel.LayoutSubgroupsProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the preferred layout width. This is useful to specify
		/// how much horizontal space the <see cref="ItemTable"/> provides in
		/// its aperture.
		/// </summary>
		/// <value>The preferred layout width.</value>
		public double PreferredLayoutWidth
		{
			get
			{
				return this.layoutWidth;
			}
			set
			{
				if (this.layoutWidth != value)
				{
					this.layoutWidth = value;
					this.InvalidateLayout ();
				}
			}
		}

		/// <summary>
		/// Gets or sets the custom item view factory getter associated with this
		/// item panel. This takes precedence over the factory getter implemented
		/// by class <see cref="ItemViewFactories.Factory"/>.
		/// </summary>
		/// <value>The custom item view factory getter.</value>
		public ItemViewFactories.ItemViewFactoryGetter CustomItemViewFactoryGetter
		{
			get
			{
				return this.customItemViewFactoryGetter;
			}
			set
			{
				this.customItemViewFactoryGetter = value;
			}
		}
		
		
		/// <summary>
		/// Gets the shape used by the item views in this panel.
		/// </summary>
		/// <value>The item view shape.</value>
		public ItemViewShape ItemViewShape
		{
			get
			{
				switch (this.GetPanelLayout ())
				{
					case ItemPanelLayout.VerticalList:
						return ItemViewShape.Row;

					case ItemPanelLayout.ColumnsOfTiles:
					case ItemPanelLayout.RowsOfTiles:
						return ItemViewShape.Tile;

					default:
						throw new System.NotSupportedException (string.Format ("Layout {0} not supported", this.GetPanelLayout ()));
				}
			}
		}

		/// <summary>
		/// Gets or sets the tracking mode for the current item.
		/// </summary>
		/// <value>The tracking mode.</value>
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

		/// <summary>
		/// Gets or sets the selection behavior.
		/// </summary>
		/// <value>The selection behavior.</value>
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

		/// <summary>
		/// Gets or sets the item selection mode.
		/// </summary>
		/// <value>The item selection mode.</value>
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

		/// <summary>
		/// Gets or sets the group selection mode.
		/// </summary>
		/// <value>The group selection mode.</value>
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

		/// <summary>
		/// Gets or sets the default size of an item view.
		/// </summary>
		/// <value>The default size of an item view.</value>
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

		/// <summary>
		/// Gets or sets a value indicating whether the group item views should
		/// be expanded by default.
		/// </summary>
		/// <value>
		/// <c>true</c> if the group item views should be expanded by default;
		/// otherwise, <c>false</c>.
		/// </value>
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

		/// <summary>
		/// Gets or sets the aperture for the current panel. This determines
		/// which item views will need a visual representation. The aperture
		/// is usually defined by the embedding <see cref="ItemTable"/>.
		/// </summary>
		/// <value>The aperture.</value>
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

		/// <summary>
		/// Gets or sets the aperture padding, which can be used to restrict
		/// the usable aperture, for instance to make room for a partial line
		/// at the bottom of the aperture.
		/// </summary>
		/// <value>The aperture padding.</value>
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

		public override bool ContainsKeyboardFocus
		{
			get
			{
				return this.containsKeyboardFocus || base.ContainsKeyboardFocus;
			}
		}

		/// <summary>
		/// Gets the layout used by this panel. This queries the root panel for
		/// the appropriate <c>Layout</c>, <c>LayoutGroups</c> or <c>LayoutSubgroups</c>
		/// property, depending on the depth of this panel.
		/// </summary>
		/// <returns>The layout used by this panel.</returns>
		public ItemPanelLayout GetPanelLayout()
		{
			switch (this.PanelDepth)
			{
				case 0:
					return this.Layout;

				case 1:
					return this.RootPanel.LayoutGroups;

				default:
					return this.RootPanel.LayoutSubgroups;
			}
		}

		/// <summary>
		/// Detects which local item view is at the specified position.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>The detected item view or <c>null</c> if none can be found.</returns>
		public ItemView Detect(Drawing.Point pos)
		{
			return this.Detect (this.SafeGetViews (), pos);
		}

		/// <summary>
		/// Gets the number of item views in this panel.
		/// </summary>
		/// <returns>The number of item views in this panel</returns>
		public int GetItemViewCount()
		{
			return this.SafeGetViews ().Count;
		}

		/// <summary>
		/// Gets the total row count in this panel.
		/// </summary>
		/// <returns>The total row count.</returns>
		public int GetTotalRowCount()
		{
			IList<ItemView> views;

			switch (this.GetPanelLayout ())
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

		/// <summary>
		/// Gets the total column count in this panel.
		/// </summary>
		/// <returns>The total column count.</returns>
		public int GetTotalColumnCount()
		{
			switch (this.GetPanelLayout ())
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

		/// <summary>
		/// Gets the mean height for a row in this panel. This is computed as
		/// the mean between the minimum row height and the maximum row height.
		/// </summary>
		/// <returns>The mean row height.</returns>
		public double GetRowHeight()
		{
			return (this.minItemHeight + this.maxItemHeight) / 2;
		}

		/// <summary>
		/// Gets the number of visible rows which fit into the given height.
		/// This is an approximation based on the mean row height.
		/// </summary>
		/// <param name="height">The height.</param>
		/// <returns>The number of visible rows which fit into the given height.</returns>
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

		/// <summary>
		/// Gets the panel relative bounds of the item view.
		/// </summary>
		/// <param name="view">The item view.</param>
		/// <returns>The panel relative bounds.</returns>
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

		/// <summary>
		/// Gets the panel relative bounds of the item view. If the item view
		/// logically touches one of the panel borders (top/bottom or left/right),
		/// then the bounds are expanded in order to include the item view's
		/// parent group(s).
		/// </summary>
		/// <param name="view">The item view.</param>
		/// <returns>The (possibly expanded) panel relative bounds.</returns>
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

				System.Diagnostics.Debug.Assert (group != null);

				double top   = panel.PreferredHeight;
				double right = panel.PreferredWidth;

				//	If the view touches the edges of the panel, then we will have
				//	to expand the bounds to include the containing group frame :

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

				//	Move up one level in the tree, until we reach the top level
				//	panel...

				view  = panel.ParentGroup.ItemView;
				panel = view.Owner;
			}

			return bounds;
		}

		/// <summary>
		/// Gets the local item view at the specified index in the current panel. The
		/// index is compared to the <c>ItemView.Index</c> property.
		/// </summary>
		/// <param name="index">The local index.</param>
		/// <returns>The item view or <c>null</c> if it cannot be found.</returns>
		public ItemView GetItemView(int index)
		{
			return ItemPanel.TryGetItemView (this.SafeGetViews (), index);
		}

		/// <summary>
		/// Gets the first local item view located at the specified row.
		/// </summary>
		/// <param name="rowIndex">Index of the row.</param>
		/// <returns>The item view or <c>null</c> if it cannot be found.</returns>
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

		/// <summary>
		/// Gets the first local item view at the specified column.
		/// </summary>
		/// <param name="columnIndex">Index of the column.</param>
		/// <returns>The item view or <c>null</c> if it cannot be found.</returns>
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

		/// <summary>
		/// Gets the local item view which represents the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The item view or <c>null</c> if it cannot be found.</returns>
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

		/// <summary>
		/// Detects the item view at the specified position. This will also detect
		/// item views in groups and subgroups.
		/// </summary>
		/// <param name="pos">The position.</param>
		/// <returns>The item view or <c>null</c> if it cannot be found.</returns>
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

		/// <summary>
		/// Finds the item view based on a predicate. This will walk through every
		/// local item view, and also every item view in the groups and subgroups.
		/// </summary>
		/// <param name="match">The test predicate.</param>
		/// <returns>The item view or <c>null</c> if it cannot be found.</returns>
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

		/// <summary>
		/// Finds all item views matching a predicate. This will walk through every
		/// local item view, and also every item view in the groups and subgroups.
		/// </summary>
		/// <param name="match">The item view test predicate.</param>
		/// <param name="groupMatch">The group test predicate (or <c>null</c>). Groups
		/// for which this predicate returns <c>false</c> won't be explored.</param>
		/// <returns>
		/// The list of item views; the list is empty if no item  view can
		/// be found.
		/// </returns>
		public IList<ItemView> FindItemViews(System.Predicate<ItemView> match, System.Predicate<ItemView> groupMatch)
		{
			List<ItemView>        list  = new List<ItemView> ();
			IEnumerable<ItemView> views = this.SafeGetViews ();

			foreach (ItemView view in views)
			{
				if (match (view))
				{
					list.Add (view);
				}

				if (view.IsGroup)
				{
					if ((groupMatch == null) ||
						(groupMatch (view)))
					{
						list.AddRange (view.Group.ChildPanel.FindItemViews (match, groupMatch));
					}
				}
			}

			return list;
		}

		/// <summary>
		/// Finds the item view which represents the specified item. This will walk
		/// through every local item view, and also every item view in the groups
		/// and subgroups.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The item view or <c>null</c> if it cannot be found.</returns>
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

		/// <summary>
		/// Deselects all item views, including those in groups and subgroups.
		/// </summary>
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

		/// <summary>
		/// Selects the specified item view. The item view can belong to a group
		/// or a subgroup.
		/// </summary>
		/// <param name="view">The item view.</param>
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

		public void FocusItemView(ItemView view)
		{
			if (view == null)
			{
				this.ClearFocus ();
			}
			else
			{
				this.Focus (view);
			}
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
				List<ItemView> selected   = new List<ItemView> ();
				List<ItemView> deselected = new List<ItemView> ();

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
						selected.Add (itemView);
					}
				}
				
				deselected.AddRange (this.views);

				if ((selected.Count > 0) ||
					(deselected.Count > 0))
				{
					this.panel.OnSelectionChanged (selected, deselected);
				}
			}

			ItemPanel panel;
			List<ItemView> views;
		}

		#endregion

		/// <summary>
		/// Deselects the specified item view. The item view can belong to a group
		/// or a subgroup.
		/// </summary>
		/// <param name="view">The item view.</param>
		public void DeselectItemView(ItemView view)
		{
			SelectionState state = new SelectionState (this);

			this.InternalDeselectItemView (view);

			state.GenerateEvents ();
		}

		/// <summary>
		/// Expands or contracts the specified item view. The item view can belong
		/// to a group or a subgroup. This will also expand the parent groups, if
		/// required.
		/// </summary>
		/// <param name="view">The item view.</param>
		/// <param name="expand">if set to <c>true</c>, expand the specified item view.</param>
		public void ExpandItemView(ItemView view, bool expand)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);

			while (view != null)
			{
				if (view.IsGroup)
				{
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

		/// <summary>
		/// Expands the parent group of a specified item view, and recursively
		/// every parent.
		/// </summary>
		/// <param name="view">The item view.</param>
		public void ExpandParentItemView(ItemView view)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);

			ItemPanelGroup group = view.Owner.ParentGroup;

			if (group != null)
			{
				this.ExpandItemView (group.ItemView, true);
			}
		}

		/// <summary>
		/// Gets the list of selected item views, including those found in the
		/// groups and subgroups.
		/// </summary>
		/// <returns>The selected item views.</returns>
		public IList<ItemView> GetSelectedItemViews()
		{
			return this.GetSelectedItemViews (null);
		}

		/// <summary>
		/// Gets the list of selected item views, including those found in the
		/// groups and subgroups, filtered by a given predicate.
		/// </summary>
		/// <param name="filter">The predicate used to filter the list.</param>
		/// <returns>The selected item views.</returns>
		public IList<ItemView> GetSelectedItemViews(System.Predicate<ItemView> filter)
		{
			List<ItemView> list = new List<ItemView> ();
			
			this.GetSelectedItemViews (filter, list);

			return list;
		}

		/// <summary>
		/// Gets the size of the item panel contents.
		/// </summary>
		/// <returns>The size of the item panel contents.</returns>
		public Drawing.Size GetContentsSize()
		{
			return this.PreferredSize;
		}

		/// <summary>
		/// Shows the specified item view, which may belong to a group or a
		/// subgroup. The parent groups get automatically expanded if this
		/// is required and the aperture is moved accordingly.
		/// </summary>
		/// <param name="view">The item view.</param>
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
				this.ExpandParentItemView (view);

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

		/// <summary>
		/// Refresh asynchronously the full item panel. This will rebuild the
		/// internal list of item views, recreate their user interfaces, redo
		/// the layout, etc.
		/// </summary>
		public void AsyncRefresh()
		{
			if (this.isRefreshPending == false)
			{
				this.isRefreshPending = true;
				this.QueueAsyncRefresh (RefreshOperations.Content);
			}
		}

		/// <summary>
		/// Refresh the full item panel. This will rebuild the internal list
		/// of item views, recreate their user interfaces, redo the layout,
		/// etc.
		/// </summary>
		public void Refresh()
		{
			this.isRefreshPending = false;
			this.containsKeyboardFocus = base.ContainsKeyboardFocus;
			this.ClearUserInterface ();
			this.RefreshItemViews ();
			this.containsKeyboardFocus = false;

			if (this.IsRootPanel)
			{
				if (this.isCurrentShowPending)
				{
					ICollectionView view = this.RootPanel.Items;

					if (view != null)
					{
						this.isCurrentShowPending = false;
						this.TrackCurrentItem (true);
					}
				}
			}
		}

		/// <summary>
		/// Executes a refresh if this is needed, i.e. if <c>AsyncRefresh</c>
		/// was called and no refresh was executed since.
		/// </summary>
		public void SyncRefreshIfNeeded()
		{
			if (this.isRefreshPending)
			{
				this.Refresh ();
			}
		}

		/// <summary>
		/// Refreshes the layout of the item panel.
		/// </summary>
		public void RefreshLayout()
		{
			RefreshState state = new RefreshState (this);

			this.RefreshLayout (this.SafeGetViews ());

			state.GenerateEvents ();
		}

		internal void RefreshLayoutIfNeeded()
		{
			if (this.hasDirtyLayout)
			{
				this.RefreshLayout ();
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

		#region RefreshOperations and RefreshInfo

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
					long a = this.panel.GetVisualSerialId ();
					long b = other.panel.GetVisualSerialId ();
					return a.CompareTo (b);
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

		#endregion

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

		protected virtual void HandleLayoutGroupsChanged(ItemPanelLayout oldValue, ItemPanelLayout newValue)
		{
			//	TODO: decide whether to call RefreshItemViews or RefreshLayout
			this.AsyncRefresh ();
		}

		protected virtual void HandleLayoutSubgroupsChanged(ItemPanelLayout oldValue, ItemPanelLayout newValue)
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
			System.Diagnostics.Debug.Assert (this.IsRootPanel);

			if (this.CurrentItemTrackingMode == CurrentItemTrackingMode.None)
			{
				return;
			}
			
			if (this.PanelDepth == 0)
			{
				object   item  = this.Items.CurrentItem;
				ItemView view  = null;
				bool     focus = this.ContainsKeyboardFocus;

				if (item == null)
				{
					//	No current item at all...
				}
				else
				{
					view  = this.FindItemView (item);
					
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
				}
				
				this.TrackCurrentItem (view, focus, autoSelect);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine ("TrackCurrentItem: called for panel at depth=" + this.PanelDepth);
			}
		}

		private void TrackCurrentItem(ItemView view, bool focus, bool autoSelect)
		{
			CurrentItemTrackingMode mode = this.RootPanel.CurrentItemTrackingMode;

			if (view == null)
			{
				this.ClearFocus ();
				
				focus      = false;
				autoSelect = ((mode == CurrentItemTrackingMode.AutoSelectAndDeselect) || (this.Items.IsEmpty)) ? autoSelect : false;
			}
			else
			{
				this.Show (view);
			}
			
			switch (mode)
			{
				case CurrentItemTrackingMode.AutoFocus:
					break;

				case CurrentItemTrackingMode.AutoSelectAndDeselect:
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

		protected virtual void OnSelectionChanged(IList<ItemView> selected, IList<ItemView> deselected)
		{
			this.selectionChanged = true;
			this.selectionSelected.AddRange (selected);
			this.selectionDeselected.AddRange (deselected);
			
			if (Widgets.Message.CurrentState.Buttons == MouseButtons.None)
			{
				this.NotifySelectionChanged ();
			}
			else
			{
				if (this.SelectionChanging != null)
				{
					this.SelectionChanging (this);
				}
			}
		}

		private void NotifySelectionChanged()
		{
			if (this.selectionChanged)
			{
				if (this.SelectionChanged != null)
				{
					ItemPanelSelectionChangedEventArgs e = new ItemPanelSelectionChangedEventArgs (this.selectionSelected, this.selectionDeselected);
					
					this.SelectionChanged (this, e);
					
					if (e.Cancel)
					{
						//	Cancel the selection change; we should notify this, somehow,
						//	but I've no idea how. Generating another SelectionChanged
						//	event is not a solution, since this could again be canceled.
						//	For now, just cancel the selection silently...

						this.InternalDeselectItemViews (this.selectionSelected);

						foreach (ItemView item in this.selectionDeselected)
						{
							this.InternalSelectItemView (item);
						}
					}
				}

				this.selectionChanged = false;
				this.selectionSelected.Clear ();
				this.selectionDeselected.Clear ();
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

			//	If nobody ate the event, then we may handle it here and determine
			//	what item has to be selected/deselected :

			if (!e.Cancel)
			{
				ItemPanel root = this.RootPanel;
				
				root.manualTrackingCounter++;

				try
				{
					if (e.Message.Button == MouseButtons.Left)
					{
						ItemView view = this.Detect (e.Point);
						
						if (view != null)
						{
							root.HandleItemViewPressed (e.Message, view);
						}
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

			this.NotifySelectionChanged ();
		}

		private void HandleItemViewPressed(Widgets.Message message, ItemView view)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);

			if (view == null)
			{
				return;
			}

			if (message.ButtonDownCount == 1)
			{
				IList<ItemView> list = this.GetSelectedItemViews ();
				bool modifySelection = message.IsControlPressed;

				if (message.IsShiftPressed && list.Count > 0)
				{
					ItemView first = null;

					if (this.firstSelectedItem != null)
					{
						first = this.FindItemView (this.firstSelectedItem.Target);
					}
					if (first == null)
					{
						first = this.FindItemView (this.Items.CurrentItem);
					}

					this.ContinuousMouseSelection (list, first, view, modifySelection);
				}
				else
				{
					this.ManualSelection (view, modifySelection);
				}

				this.Focus (view);
			}
			
			message.Consumer = this;
			message.ForceCapture = true;
		}

		protected override void OnContainsFocusChanged()
		{
			base.OnContainsFocusChanged ();

			if (this.IsRootPanel)
			{
				this.Invalidate ();
			}
		}

		protected override bool AboutToGetFocus(Widgets.TabNavigationDir dir, Widgets.TabNavigationMode mode, out Widgets.Widget focus)
		{
			if (base.AboutToGetFocus (dir, mode, out focus))
			{
				ItemView view = this.RootPanel.focusedItemView;

				if (view != null)
				{
					focus = view.Widget;
				}

				return true;
			}
			else
			{
				return false;
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
				if (this.Navigate (message))
				{
					message.Handled = true;
				}
			}
		}

		public bool Navigate(Widgets.Message message)
		{
			//	Change the current item (this does not select the item).

			ItemView oldFocusedItemView = this.GetFocusedItemView ();
			ItemView newFocusedItemView;
			ItemView oldCurrentItemView = this.FindItemView (this.Items.CurrentItem);
			ItemView newCurrentItemView;

			if (this.ProcessNavigationKeys (message.KeyCode))
			{
				newCurrentItemView = this.FindItemView (this.Items.CurrentItem);

				if ((newCurrentItemView != null) &&
						(newCurrentItemView != oldCurrentItemView))
				{
					newCurrentItemView.Owner.TrackCurrentItem (newCurrentItemView, true, false);
				}

				newFocusedItemView = this.GetFocusedItemView ();

				if (newCurrentItemView != oldCurrentItemView)
				{
					if (!message.IsControlPressed)
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
				}

				if (oldFocusedItemView != newFocusedItemView)
				{
					if (newFocusedItemView != null)
					{
						newFocusedItemView.Owner.Show (newFocusedItemView);
					}
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		private void ProcessExpandKey(bool expand)
		{
			ItemView itemView = this.GetFocusedItemView ();

			if (itemView != null)
			{
				ItemView group = ItemViewWidget.FindGroupItemView (itemView);

			again:

				if (group != null)
				{
					if (group.IsExpanded == expand)
					{
						if (expand == false)
						{
							ItemPanel parent = group.Owner;

							if ((parent != null) &&
								(parent.ParentGroup != null))
							{
								group = group.Owner.ParentGroup.ItemView;
								goto again;
							}
						}
					}
					else
					{
						group.IsExpanded = expand;
						this.Focus (group);
					}
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
				this.ManualSelection (itemView, modifySelection);
			}
		}

		private void Focus(ItemView itemView)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);

			if (itemView != null)
			{
				this.RecordFocus (itemView);

				ItemViewWidget widget = itemView.Widget;

				if (widget != null)
				{
					widget.Focus ();
				}
			}
		}


		internal void NotifyWidgetPressed(ItemViewWidget widget, Widgets.Message message, Drawing.Point pos)
		{
			Widgets.MessageEventArgs e = new Widgets.MessageEventArgs (message, widget.MapClientToParent (pos));

			base.OnPressed (e);

			if (!e.Cancel)
			{
				ItemPanel root = this.RootPanel;

				root.manualTrackingCounter++;

				try
				{
					if (message.Button == MouseButtons.Left)
					{
						root.HandleItemViewPressed (message, widget.ItemView);
					}
				}
				finally
				{
					root.manualTrackingCounter--;
				}
			}
		}

		internal void NotifyFocusChanged(ItemViewWidget widget, bool focus)
		{
			if (this.IsRootPanel)
			{
				ItemView view = widget.ItemView;

				if (focus)
				{
					this.RecordFocus (view);
				}
				else
				{
					if (this.focusedItemView == view)
					{
						this.ClearFocus ();
					}
				}
			}
			else
			{
				this.RootPanel.NotifyFocusChanged (widget, focus);
			}
		}

		private void RecordFocus(ItemView itemView)
		{
			ItemPanelNavigator navigator = this.GetNavigator ();

			navigator.NavigateTo (itemView);

			if (this.focusedItemView == itemView)
			{
				return;
			}

			this.focusedItemView = itemView;
		}

		private new void ClearFocus()
		{
			if (this.focusedItemView == null)
			{
				return;
			}

			this.focusedItemView = null;
		}

		private ItemPanelNavigator GetNavigator()
		{
			if (this.IsRootPanel)
			{
				lock (this.exclusion)
				{
					if (this.navigator == null)
					{
						this.navigator = new ItemPanelNavigator (this);
					}
				}
				return this.navigator;
			}
			else
			{
				return this.RootPanel.GetNavigator ();
			}
		}


		private void ContinuousKeySelection(IList<ItemView> list, ItemView oldCurrent, ItemView newCurrent)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);
			System.Diagnostics.Debug.Assert (this.Items != null);

			System.Collections.IList collection = this.Items.Items;

			ItemView view1 = ItemPanel.GetFirstSelectedItemView (collection, list);
			ItemView view2 = ItemPanel.GetLastSelectedItemView (collection, list);

			int index1 = (view1 == null) ? -1 : collection.IndexOf (view1.Item);
			int index2 = (view2 == null) ? -1 : collection.IndexOf (view2.Item);

			int oldIndex = (oldCurrent == null) ? -1 : collection.IndexOf (oldCurrent.Item);
			int newIndex = (newCurrent == null) ? -1 : collection.IndexOf (newCurrent.Item);

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
					this.InternalSelectItemView (this.FindItemView (collection[i]));
				}
			}
			else
			{
				for (int i = index1; i <= index2; i++)
				{
					this.InternalSelectItemView (this.FindItemView (collection[i]));
				}
			}

			List<ItemView> deselect = new List<ItemView> ();
			
			foreach (ItemView view in list)
			{
				int index = collection.IndexOf (view.Item);
				
				if ((index < index1) ||
					(index > index2))
				{
					deselect.Add (view);
				}
			}
			
			this.InternalDeselectItemViews (deselect);

			this.Items.MoveCurrentToPosition (newIndex);
		}

		private void ContinuousMouseSelection(IList<ItemView> list, ItemView current, ItemView clicked, bool modifySelection)
		{
			System.Collections.IList collection = this.Items.Items;

			int currentIndex = collection.IndexOf (current.Item);
			int clickedIndex = collection.IndexOf (clicked.Item);
			int index1 = -1;
			int index2 = -1;

			System.Diagnostics.Debug.WriteLine (string.Format ("Current: {0} Clicked: {1}", currentIndex, clickedIndex));

			if (currentIndex < clickedIndex)
			{
				for (int i=currentIndex; i<=clickedIndex; i++)
				{
					this.InternalSelectItemView(this.FindItemView (collection[i]));
				}

				index1 = currentIndex;
				index2 = clickedIndex;
			}

			if (currentIndex > clickedIndex)
			{
				for (int i=currentIndex; i>=clickedIndex; i--)
				{
					this.InternalSelectItemView(this.FindItemView (collection[i]));
				}

				index1 = clickedIndex;
				index2 = currentIndex;
			}

			if (!modifySelection && index1 != -1 && index2 != -1)
			{
				List<ItemView> deselect = new List<ItemView>();
				
				foreach (ItemView view in list)
				{
					int index = collection.IndexOf (view.Item);

					if ((index < index1) ||
						(index > index2))
					{
						deselect.Add (view);
					}
				}
				
				this.InternalDeselectItemViews(deselect);
			}

			this.Items.MoveCurrentToPosition(clickedIndex);
		}

		private bool ProcessNavigationKeys(Widgets.KeyCode keyCode)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);
			
			ItemPanelLayout layout;
			ItemView        current;

			if (this.focusedItemView == null)
			{
				layout  = this.GetPanelLayout ();
				current = this.FindItemView (this.Items.CurrentItem);
			}
			else
			{
				layout  = this.focusedItemView.Owner.GetPanelLayout ();
				current = this.focusedItemView;
			}

			switch (layout)
			{
				case ItemPanelLayout.VerticalList:
					return this.ProcessNavigationKeysRowsOfTiles (current, keyCode);
//					return this.ProcessNavigationKeysVerticalList (current, keyCode);

				case ItemPanelLayout.RowsOfTiles:
					return this.ProcessNavigationKeysRowsOfTiles (current, keyCode);

				case ItemPanelLayout.ColumnsOfTiles:
					break;
			}

			return false;
		}

		private bool ProcessNavigationKeysVerticalList(ItemView current, Widgets.KeyCode keyCode)
		{
			if ((current != null) &&
				(current.IsGroup))
			{
				//	The focus is currently set on a group, not an item. We will
				//	have to move through the groups at the current depth.

			again:
				ItemPanelGroup group;

				ItemPanel panel = current.Owner;
				ItemView  view  = null;

				int index    = current.Index;
				int minIndex = 0;
				int maxIndex = panel.GetItemViewCount ()-1;

				index = System.Math.Max (index, minIndex);
				index = System.Math.Min (index, maxIndex);
				
				switch (keyCode)
				{
					case KeyCode.ArrowUp:
						if (index <= minIndex)
						{
							group = ItemPanelGroup.GetLastSiblingInPreviousParent (current.Group);

							if (group != null)
							{
								view = group.ItemView;
							}
						}
						else
						{
							view = panel.GetItemView (index-1);
						}
						break;

					case KeyCode.ArrowDown:
						if (index >= maxIndex)
						{
							group = ItemPanelGroup.GetFirstSiblingInNextParent (current.Group);

							if (group != null)
							{
								view = group.ItemView;
							}
						}
						else
						{
							view = panel.GetItemView (index+1);
						}
						break;

					case KeyCode.Home:
						view  = panel.GetItemView (minIndex);
						group = ItemPanelGroup.GetLastSiblingInPreviousParent (current.Group);

						if (group != null)
						{
							current = group.ItemView;
							goto again;
						}
						break;

					case KeyCode.End:
						view  = panel.GetItemView (maxIndex);
						group = ItemPanelGroup.GetFirstSiblingInNextParent (current.Group);

						if (group != null)
						{
							current = group.ItemView;
							goto again;
						}
						break;

					case KeyCode.PageUp:
						//	TODO: monte d'une page parmi les groupes
						break;

					case KeyCode.PageDown:
						//	TODO: descend d'une page parmi les groupes
						break;

					default:
						return false;
				}

				if (view != null)
				{
					this.Focus (view);
				}
			}
			else
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
						this.ProcessVerticalListPageUpMove ();
						break;

					case KeyCode.PageDown:
						this.ProcessVerticalListPageDownMove ();
						break;

					default:
						return false;
				}
			}
			
			return true;
		}

		private bool ProcessNavigationKeysRowsOfTiles(ItemView current, Widgets.KeyCode keyCode)
		{
#if true
			//	TODO: écrire ici un code "géographique" qui se base exclusivement
			//	sur les informations géométriques retournées par la méthode 
			//	ItemPanel.GetItemViewBounds(ItemView).

			ItemPanelNavigator navigator = this.GetNavigator ();
			ItemView view = null;
			
			switch (keyCode)
			{
				case KeyCode.ArrowLeft:
					if (navigator.Navigate (Widgets.Direction.Left))
					{
						view = navigator.Current;
					}
					break;

				case KeyCode.ArrowRight:
					if (navigator.Navigate (Widgets.Direction.Right))
					{
						view = navigator.Current;
					}
					break;

				case KeyCode.ArrowUp:
					if (navigator.Navigate (Widgets.Direction.Up))
					{
						view = navigator.Current;
					}
					break;

				case KeyCode.ArrowDown:
					if (navigator.Navigate (Widgets.Direction.Down))
					{
						view = navigator.Current;
					}
					break;

				case KeyCode.PageUp:
					view = this.ProcessNavigationPageUp (navigator);
					break;

				case KeyCode.PageDown:
					view = this.ProcessNavigationPageDown (navigator);
					break;

				case KeyCode.Home:
					this.Items.MoveCurrentToFirst ();
					break;

				case KeyCode.End:
					this.Items.MoveCurrentToLast ();
					break;

				default:
					return false;
			}

			if (view != null)
			{
				this.Focus (view);

				if (!view.IsGroup)
				{
					this.Items.MoveCurrentTo (view.Item);
				}
			}
			
			return true;
#else
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
					this.ProcessVerticalListPageUpMove ();
					break;

				case KeyCode.PageDown:
					this.ProcessVerticalListPageDownMove ();
					break;

				default:
					return false;
			}

			return true;
#endif
		}

		private ItemView ProcessNavigationPageUp(ItemPanelNavigator navigator)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);

			ItemView          startView   = navigator.Current;
			Drawing.Rectangle startBounds = this.GetItemViewBounds (startView);

			if (navigator.Navigate (Widgets.Direction.Up))
			{
				Drawing.Rectangle aperture = this.Aperture;
				ItemView          view     = navigator.Current;
				Drawing.Rectangle bounds   = this.GetItemViewBounds (view);

				if (aperture.Contains (bounds))
				{
					//	We are still in the fully visible part of the panel; we will
					//	just keep on moving up until we get outside of the aperture or
					//	we hit the top of the panel.

					while (navigator.Navigate (Widgets.Direction.Up))
					{
						view   = navigator.Current;
						bounds = this.GetItemViewBounds (view);

						if (!aperture.Contains (bounds))
						{
							navigator.Navigate (Widgets.Direction.Down);
							break;
						}
					}
				}
				else
				{
					//	We were previously already located at the top of the aperture;
					//	this means we will have to move up by an aperture height.
					
					while (navigator.Navigate (Widgets.Direction.Up))
					{
						view   = navigator.Current;
						bounds = this.GetItemViewBounds (view);

						double distance = bounds.Top - startBounds.Top;

						if (distance > aperture.Height)
						{
							navigator.Navigate (Widgets.Direction.Down);
							break;
						}
					}
				}

				return navigator.Current;
			}

			return null;
		}

		private ItemView ProcessNavigationPageDown(ItemPanelNavigator navigator)
		{
			System.Diagnostics.Debug.Assert (this.IsRootPanel);

			ItemView          startView   = navigator.Current;
			Drawing.Rectangle startBounds = this.GetItemViewBounds (startView);

			if (navigator.Navigate (Widgets.Direction.Down))
			{
				Drawing.Rectangle aperture = this.Aperture;
				ItemView          view     = navigator.Current;
				Drawing.Rectangle bounds   = this.GetItemViewBounds (view);

				if (aperture.Contains (bounds))
				{
					//	We are still in the fully visible part of the panel; we will
					//	just keep on moving down until we get outside of the aperture or
					//	we hit the bottom of the panel.

					while (navigator.Navigate (Widgets.Direction.Down))
					{
						view   = navigator.Current;
						bounds = this.GetItemViewBounds (view);

						if (!aperture.Contains (bounds))
						{
							navigator.Navigate (Widgets.Direction.Up);
							break;
						}
					}
				}
				else
				{
					//	We were previously already located at the bottom of the aperture;
					//	this means we will have to move down by an aperture height.

					while (navigator.Navigate (Widgets.Direction.Down))
					{
						view   = navigator.Current;
						bounds = this.GetItemViewBounds (view);

						double distance = startBounds.Top - bounds.Top;

						if (distance > aperture.Height)
						{
							navigator.Navigate (Widgets.Direction.Up);
							break;
						}
					}
				}

				return navigator.Current;
			}

			return null;
		}

		private void ProcessVerticalListPageUpMove()
		{
			//	Déplace sur le premier visible entièrement ou, s'il est déjà l'élément courant,
			//	une page avant, en tenant compte de la géométrie des items.
			int pos = this.Items.CurrentPosition;
			int first, last;
			this.GetFirstAndLastVisibleItem(out first, out last);

			ItemPanelLayout layout = this.GetPanelLayout ();
			
			if (layout == ItemPanelLayout.VerticalList)
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

			if (layout == ItemPanelLayout.RowsOfTiles)
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

		private void ProcessVerticalListPageDownMove()
		{
			//	Déplace sur le dernier visible entièrement ou, s'il est déjà l'élément courant,
			//	une page après, en tenant compte de la géométrie des items.
			int pos = this.Items.CurrentPosition;
			int first, last;
			this.GetFirstAndLastVisibleItem(out first, out last);

			ItemPanelLayout layout = this.GetPanelLayout ();
			
			if (layout == ItemPanelLayout.VerticalList)
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

			if (layout == ItemPanelLayout.RowsOfTiles)
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

		private void GetFirstAndLastVisibleItem(out int first, out int last)
		{
			IEnumerable<ItemView> views = this.SafeGetViews();
			first = 0;
			last = 0;
			bool inside = false;
			int index = 0;

			foreach (ItemView view in views)
			{
				index = view.GetCollectionIndex ();

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
					this.firstSelectedItem = new System.WeakReference (view.Item);
				}
				else
				{
					this.InternalDeselectItemView (view);
					this.firstSelectedItem = null;
				}
			}

			state.GenerateEvents ();
		}

		private static ItemView GetFirstSelectedItemView(System.Collections.IList collection, IEnumerable<ItemView> list)
		{
			int      index = int.MaxValue;
			ItemView found = null;

			foreach (ItemView view in list)
			{
				int viewIndex = collection.IndexOf (view.Item);

				if (viewIndex < index)
				{
					index = viewIndex;
					found = view;
				}
			}

			return found;
		}

		private static ItemView GetLastSelectedItemView(System.Collections.IList collection, IEnumerable<ItemView> list)
		{
			int      index = -1;
			ItemView found = null;

			foreach (ItemView view in list)
			{
				int viewIndex = collection.IndexOf (view.Item);

				if (viewIndex > index)
				{
					index = viewIndex;
					found = view;
				}
			}

			return found;
		}

		internal ItemView GetFocusedItemView()
		{
			return this.RootPanel.focusedItemView;
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
		}

		public override bool AcceptsFocus
		{
			get
			{
				//	Only the root panel may accept the focus.
				
				if (this.IsRootPanel)
				{
					return base.AcceptsFocus;
				}
				else
				{
					return false;
				}
			}
		}

		protected override void DispatchMessage(Widgets.Message message, Drawing.Point pos)
		{
			if ((message.MessageType == MessageType.MouseUp) &&
				(message.Button == MouseButtons.Left))
			{
				if (message.Captured)
				{
					switch (message.ButtonDownCount)
					{
						case 1:
							this.OnReleased (new Epsitec.Common.Widgets.MessageEventArgs (message, pos));
							break;
						
						case 2:
							this.OnDoubleClicked (new Widgets.MessageEventArgs (message, pos));
							break;
					}

					message.Consumer = this;
					return;
				}
			}
			
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
			if (view == null)
			{
				return;
			}

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

		internal void RecreateUserInterface()
		{
			this.RecreateUserInterface (this.SafeGetViews (), this.Aperture);
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
			ICollectionView items = this.RootPanel.Items;
			
			Dictionary<object, ItemView> currentViews = new Dictionary<object, ItemView> ();

			using (new LockManager (this))
			{
				foreach (ItemView view in this.views)
				{
					currentViews.Add (view.Item, view);
				}
			}

			List<ItemView> views = new List<ItemView> ();

			if (items != null)
			{
				lock (items.ItemsSyncRoot)
				{
					if (this.IsRootPanel)
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
					else
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
				}
			}

			this.RefreshLayout (views);

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
		
		private void RefreshLayout(IEnumerable<ItemView> views)
		{
			foreach (ItemView view in views)
			{
				view.UpdateSize ();
			}
			
			this.hasDirtyLayout = false;

			switch (this.GetPanelLayout ())
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
			
			if (this.Aperture.IsValid)
			{
				this.RecreateUserInterface (views, this.Aperture);
			}
		}

		private void CreateItemViews(IList<ItemView> views, System.Collections.IList list, Dictionary<object, ItemView> currentViews)
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

		private ItemView CreateItemView(object item, int index)
		{
			ItemView view = new ItemView (item, this, this.ItemViewDefaultSize);
			
			view.DefineIndex (index);
			view.IsExpanded = this.ItemViewDefaultExpanded;

			if (view.IsGroup)
			{
				view.CreateUserInterface ();
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
					maxDx = System.Math.Max (maxDx, size.Width);
					maxDy = System.Math.Max (maxDy, size.Height);
				}

				dy += size.Height;
			}

			this.minItemWidth  = minDx;
			this.maxItemWidth  = maxDx;
			
			this.minItemHeight = minDy;
			this.maxItemHeight = maxDy;

			double y = dy;
			int row = 0;
			
			this.UpdatePreferredSize (System.Math.Max (this.PreferredLayoutWidth, maxDx), dy);
			
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
			double width = this.PreferredLayoutWidth;

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
					maxDx = System.Math.Max (maxDx, size.Width);
					maxDy = System.Math.Max (maxDy, size.Height);
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
			this.UpdatePreferredSize (System.Math.Max (lx, width), dy);
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

		private static void NotifyLayoutGroupsChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleLayoutGroupsChanged ((ItemPanelLayout) oldValue, (ItemPanelLayout) newValue);
		}

		private static void NotifyLayoutSubgroupsChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ItemPanel panel = (ItemPanel) obj;
			panel.HandleLayoutSubgroupsChanged ((ItemPanelLayout) oldValue, (ItemPanelLayout) newValue);
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

		public event Support.EventHandler<ItemPanelSelectionChangedEventArgs> SelectionChanged;

		public event Support.EventHandler SelectionChanging;

		public event Support.EventHandler CurrentChanged;

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register ("Items", typeof (ICollectionView), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanel.NotifyItemsChanged));
		public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register ("Layout", typeof (ItemPanelLayout), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelLayout.VerticalList, ItemPanel.NotifyLayoutChanged));
		public static readonly DependencyProperty LayoutGroupsProperty = DependencyProperty.Register ("LayoutGroups", typeof (ItemPanelLayout), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelLayout.VerticalList, ItemPanel.NotifyLayoutGroupsChanged));
		public static readonly DependencyProperty LayoutSubgroupsProperty = DependencyProperty.Register ("LayoutSubgroups", typeof (ItemPanelLayout), typeof (ItemPanel), new DependencyPropertyMetadata (ItemPanelLayout.VerticalList, ItemPanel.NotifyLayoutSubgroupsChanged));
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
		double							layoutWidth;
		List<ItemPanelGroup>			groups = new List<ItemPanelGroup> ();
		List<RefreshInfo>				refreshInfos = new List<RefreshInfo> ();

		ItemViewFactories.ItemViewFactoryGetter customItemViewFactoryGetter;
		
		bool							hasDirtyLayout;
		bool							isRefreshing;
		bool							isRefreshPending;
		bool							isCurrentShowPending;
		bool							containsKeyboardFocus;
		int								manualTrackingCounter;

		ItemPanelGroup					parentGroup;

		ItemView						focusedItemView;
		System.WeakReference			firstSelectedItem;
		ItemPanelNavigator				navigator;

		double							minItemWidth;
		double							maxItemWidth;
		double							minItemHeight;
		double							maxItemHeight;

		int								totalRowCount;
		int								totalColumnCount;

		bool							selectionChanged;
		List<ItemView>					selectionSelected = new List<ItemView> ();
		List<ItemView>					selectionDeselected = new List<ItemView> ();
	}
}
