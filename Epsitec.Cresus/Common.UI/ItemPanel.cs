//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ItemPanel))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemPanel</c> class represents a collection of items in a panel,
	/// where the collection is defined through a <see cref="ICollectionView"/>.
	/// </summary>
	public class ItemPanel : FrameBox
	{
		public ItemPanel()
		{
			this.AutoDoubleClick = true;
			this.InternalState |= InternalState.Focusable;
		}

		public ItemPanel(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		static ItemPanel()
		{
			DependencyPropertyMetadata metadata = Widget.AutoDoubleClickProperty.DefaultMetadata.Clone ();

			metadata.DefineDefaultValue (true);

			Widget.AutoDoubleClickProperty.OverrideMetadata (typeof (ItemPanel), metadata);
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

		public ItemPanel RootPanel
		{
			get
			{
				if ((this.parentGroup != null) &&
					(this.parentGroup.ParentPanel != null))
				{
					return this.parentGroup.ParentPanel.RootPanel;
				}
				else
				{
					return this;
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
				return (ItemPanelSelectionMode) this.GetValue (ItemPanel.ItemSelectionModeProperty);
			}
			set
			{
				this.SetValue (ItemPanel.ItemSelectionModeProperty, value);
			}
		}

		public ItemPanelSelectionMode GroupSelectionMode
		{
			get
			{
				return (ItemPanelSelectionMode) this.GetValue (ItemPanel.GroupSelectionModeProperty);
			}
			set
			{
				this.SetValue (ItemPanel.GroupSelectionModeProperty, value);
			}
		}

		public Size ItemViewDefaultSize
		{
			get
			{
				return (Size) this.GetValue (ItemPanel.ItemViewDefaultSizeProperty);
			}
			set
			{
				this.SetValue (ItemPanel.ItemViewDefaultSizeProperty, value);
			}
		}

		public Rectangle Aperture
		{
			get
			{
				double yTop = this.PreferredHeight - System.Math.Max (this.apertureY, 0);
				double yBot = System.Math.Max (yTop - this.apertureHeight, 0);

				return new Rectangle (this.apertureX, yBot, this.apertureWidth, yTop - yBot);
			}
			set
			{
				Rectangle oldValue = this.Aperture;
				Rectangle newValue = value;
				
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

		public Margins AperturePadding
		{
			get
			{
				return (Margins) this.GetValue (ItemPanel.AperturePaddingProperty);
			}
			set
			{
				this.SetValue (ItemPanel.AperturePaddingProperty, value);
			}
		}

		public ItemView Detect(Point pos)
		{
			return this.Detect (this.SafeGetViews (), pos);
		}

		public int GetItemViewCount()
		{
			return this.SafeGetViews ().Count;
		}

		public int GetTotalLineCount()
		{
			IList<ItemView> views = this.SafeGetViews ();

			switch (this.Layout)
			{
				case ItemPanelLayout.VerticalList:
					return views.Count;

				default:
					return -1;
			}
		}

		public double GetLineHeight()
		{
			return (this.minItemHeight + this.maxItemHeight) / 2;
		}
		
		public int GetVisibleLineCount(double height)
		{
			double itemHeight = this.GetLineHeight ();

			if (itemHeight <= 0)
			{
				return -1;
			}
			else
			{
				return (int) (height / itemHeight);
			}
		}

		public ItemView GetItemView(int index)
		{
			return ItemPanel.TryGetItemView (this.SafeGetViews (), index);
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

		public ItemView DetectItemView(Point pos)
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
			}

			return null;
		}
		
		public void DeselectAllItemViews()
		{
			SelectionState state = new SelectionState (this);

			IList<ItemView> selectedViews = this.RootPanel.GetSelectedItemViews ();

			if (selectedViews.Count > 0)
			{
				this.DeselectItemViews (selectedViews);
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
						this.DeselectItemViews (selectedViews);
						break;

					case ItemPanelSelectionMode.ExactlyOne:
					case ItemPanelSelectionMode.ZeroOrOne:
						this.DeselectItemViews (selectedViews);
						this.SetItemViewSelection (view, true);
						break;

					case ItemPanelSelectionMode.Multiple:
					case ItemPanelSelectionMode.OneOrMore:
						this.SetItemViewSelection (view, true);
						break;
				}
			}

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
						this.DeselectItemViews (selectedViews);
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

			state.GenerateEvents ();
		}

		public void ExpandItemView(ItemView view, bool expand)
		{
			if (ItemPanel.ContainsGroup (view))
			{
				view.CreateUserInterface (this);
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
			
			this.GetSelectedItemViews (filter, list);

			return list;
		}

		public Size GetContentsSize()
		{
			return this.PreferredSize;
		}

		public void AsyncRefresh()
		{
			if (this.isRefreshPending == false)
			{
				this.isRefreshPending = true;
				Application.QueueAsyncCallback (this.Refresh);
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

			if (this.Aperture.IsSurfaceZero)
			{
				//	Nothing to do : there is no visible aperture, so don't bother
				//	moving it around !
			}
			else
			{
				if (this.hasDirtyLayout)
				{
					this.RefreshLayout ();
				}

				Rectangle aperture = this.Aperture;
				Rectangle bounds   = view.Bounds;

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
			return (view.Item is CollectionViewGroup) ? true : false;
		}

		#endregion
		
		internal void NotifyItemViewSizeChanged(ItemView view, Size oldSize, Size newSize)
		{
			this.InvalidateLayout ();
		}

		private void InvalidateLayout()
		{
			if (this.hasDirtyLayout == false)
			{
				this.hasDirtyLayout = true;
				Application.QueueAsyncCallback (this.RefreshLayout);
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

				ItemPanelGroup childPanelGroup = view.Widget as ItemPanelGroup;

				if (childPanelGroup != null)
				{
					childPanelGroup.GetSelectedItemViews (filter, list);
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
			if (this.CurrentChanged != null)
			{
				this.CurrentChanged (this);
			}
		}

		protected virtual void HandleItemViewDefaultSizeChanged(Size oldValue, Size newValue)
		{
			this.AsyncRefresh ();
		}
		
		protected virtual void HandleApertureChanged(Rectangle oldValue, Rectangle newValue)
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

		protected override void OnPressed(MessageEventArgs e)
		{
			base.OnPressed (e);

			if (!e.Suppress)
			{
				if (e.Message.Button == MouseButtons.Left)
				{
					ItemView view = this.Detect (e.Point);

					if (view != null)
					{
						this.ManualSelection (view, e.Message.IsControlPressed);
						e.Message.Consumer = this;
					}
				}
			}
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			base.ProcessMessage(message, pos);

			if (message.IsMouseType)
			{
				if (message.MessageType == MessageType.MouseDown)
				{
					this.Focus ();
				}

				if ((message.MessageType == MessageType.MouseMove) &&
					(message.Button == MouseButtons.None))
				{
					ItemView item = this.Detect (pos);

					if (this.enteredItem != item)
					{
						this.enteredItem = item;
						this.Invalidate ();
					}
				}
			}
			else if (message.IsKeyType)
			{
				if (message.MessageType == MessageType.KeyDown)
				{
					if (message.KeyCode == KeyCode.Space)
					{
						//	Behaves as if the user had pressed the mouse button; this will
						//	either select just the current item or add/remove the item from
						//	the selection if CTRL is pressed.

						ItemView view = this.GetCurrentItemView ();
						this.ManualSelection (view, message.IsControlPressed);
					}
					else
					{
						//	Change the current item (this does not select the item).

						ItemView oldCurrent = this.GetCurrentItemView ();
						
						switch (message.KeyCode)
						{
							case KeyCode.ArrowUp:
								this.Items.MoveCurrentToPrevious ();
								break;

							case KeyCode.ArrowDown:
								this.Items.MoveCurrentToNext ();
								break;

							case KeyCode.Home:
								this.Items.MoveCurrentToFirst ();
								break;

							case KeyCode.End:
								this.Items.MoveCurrentToLast ();
								break;

							case KeyCode.PageUp:
							case KeyCode.PageDown:
								//	TODO: déplace sur le premier/dernier visible entièrement ou, s'il est
								//	déjà l'élément courant, une page avant/après, en tenant compte de la
								//	géométrie des items...
								break;
						}

						ItemView newCurrent = this.GetCurrentItemView ();
						
						if ((!message.IsControlPressed) &&
							(!message.IsAltPressed))
						{
							IList<ItemView> list = this.GetSelectedItemViews ();

							
							
							if (list.Count > 0)
							{
								ItemView first = ItemPanel.GetFirstSelectedItemView (list);
								ItemView last  = ItemPanel.GetLastSelectedItemView (list);

								if (message.KeyCode == KeyCode.ArrowUp)
								{
									ItemView item = list[0];  // premier item sélectionné
									if (item.Index > 0)
									{
										item = this.GetItemView (item.Index-1);
										if (!message.IsShiftPressed)
										{
											this.DeselectAllItemViews ();
										}
										this.SelectItemView (item);
									}
								}

								if (message.KeyCode == KeyCode.ArrowDown)
								{
									ItemView item = list[list.Count-1];  // dernier item sélectionné
									if (item.Index < this.GetTotalLineCount ()-1)
									{
										item = this.GetItemView (item.Index+1);
										if (!message.IsShiftPressed)
										{
											this.DeselectAllItemViews ();
										}
										this.SelectItemView (item);
									}
								}
							}
						}
					}

					this.Show (newCurrent);
				}
			}
		}

		private void ManualSelection(ItemView view, bool expand)
		{
			if (view == null)
			{
				return;
			}
			
			bool select = !view.IsSelected;
			bool action = false;

			switch (this.SelectionBehavior)
			{
				case ItemPanelSelectionBehavior.Automatic:
					action = true;
					break;

				case ItemPanelSelectionBehavior.Manual:
					action = true;
					if (!expand)
					{
						this.DeselectAllItemViews ();
					}
					break;

				case ItemPanelSelectionBehavior.ManualOne:
					action = true;
					if (!expand)
					{
						this.DeselectAllItemViews ();
						select = true;
					}
					break;
			}

			if (action)
			{
				if (select)
				{
					this.SelectItemView (view);
				}
				else
				{
					this.DeselectItemView (view);
				}
			}
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
		
		private ItemView GetCurrentItemView()
		{
			object item = this.Items.CurrentItem;

			if (item == null)
			{
				return null;
			}
			else
			{
				return this.FindItemView (
					delegate (ItemView view)
					{
						return (view.Item == item);
					});
			}
		}

		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited(e);

			if (this.enteredItem != null)
			{
				this.enteredItem = null;
				this.Invalidate();
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IEnumerable<ItemView> views = this.SafeGetViews ();
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Rectangle rect = Rectangle.Intersection (clipRect, this.Aperture);
			ICollectionView items = this.RootPanel.Items;

#if false
			if (!this.BackColor.IsEmpty)
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (this.BackColor);
			}
#endif

			object focusedItem = items == null ? null : items.CurrentItem;

			foreach (ItemView view in views)
			{
				if (view.Bounds.IntersectsWith (rect))
				{
					WidgetPaintState state = WidgetPaintState.Enabled;
					Rectangle viewBounds = Rectangle.Intersection (rect, view.Bounds);

					if (view.IsSelected)
					{
						state |= WidgetPaintState.Selected;
					}
					if (view.Item == focusedItem)
					{
						state |= WidgetPaintState.Focused;
					}
					if (view == this.enteredItem)
					{
						state |= WidgetPaintState.Entered;
					}

					adorner.PaintCellBackground (graphics, view.Bounds, state);
				}
			}
		}

		protected override void DispatchMessage(Message message, Point pos)
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
				view.Select (selection);
				
				if (view.IsVisible)
				{
					this.Invalidate (Rectangle.Intersection (this.Aperture, view.Bounds));
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

		private void RecreateUserInterface(IEnumerable<ItemView> views, Rectangle aperture)
		{
			System.Diagnostics.Debug.WriteLine ("Recreate user interface");
			List<ItemView> dispose = new List<ItemView> ();
			List<ItemView> create  = new List<ItemView> ();
			
			foreach (ItemView view in views)
			{
				if ((view.Widget != null) &&
					(view.IsUserInterfaceDirty == false))
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

			System.Diagnostics.Debug.WriteLine ("Recreate user interface: disposing");
			
			dispose.ForEach (
				delegate (ItemView view)
				{
					view.DisposeUserInterface ();
				} );
			
			System.Diagnostics.Debug.WriteLine ("Recreate user interface: creating");

			create.ForEach (
				delegate (ItemView view)
				{
					view.CreateUserInterface (this);
				} );

			System.Diagnostics.Debug.WriteLine ("Recreate user interface: done");
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
			this.RecreateUserInterface (views, this.Aperture);

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
		
		private void RefreshLayout()
		{
			RefreshState state = new RefreshState (this);

			this.RefreshLayout (this.SafeGetViews ());

			state.GenerateEvents ();
		}

		private void RefreshLayout(IEnumerable<ItemView> views)
		{
			foreach (ItemView view in views)
			{
				view.UpdateSize (this);
			}
			
			this.hasDirtyLayout = false;
			
			switch (this.Layout)
			{
				case ItemPanelLayout.VerticalList:
					this.LayoutVerticalList (views);
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
			ItemView view = new ItemView (item, this.ItemViewDefaultSize);
			
			view.DefineIndex (index);

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
				Size size = view.Size;

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
			
			foreach (ItemView view in views)
			{
				double h = view.Size.Height;
				y -= h;
				view.Bounds = new Rectangle (0, y, maxDx, h);
			}

			this.UpdatePreferredSize (maxDx, dy);
		}

		private void UpdatePreferredSize(double width, double height)
		{
			Size oldValue = this.PreferredSize;

			this.PreferredWidth  = width;
			this.PreferredHeight = height;
		}

		private ItemView Detect(IList<ItemView> views, Point pos)
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
				this.contentsSize = this.panel.PreferredSize;
			}


			public void GenerateEvents()
			{
				Size oldContentsSize = this.contentsSize;
				Size newContentsSize = this.panel.PreferredSize;

				if (oldContentsSize != newContentsSize)
				{
					this.panel.OnContentsSizeChanged (new DependencyPropertyChangedEventArgs ("ContentsSize", oldContentsSize, newContentsSize));
				}
			}

			private ItemPanel panel;
			private Size contentsSize;
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
			panel.HandleItemViewDefaultSizeChanged ((Size) oldValue, (Size) newValue);
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
		public static readonly DependencyProperty ItemViewDefaultSizeProperty = DependencyProperty.Register ("ItemViewDefaultSize", typeof (Size), typeof (ItemPanel), new DependencyPropertyMetadata (new Size (80, 20), ItemPanel.NotifyItemViewDefaultSizeChanged));
		public static readonly DependencyProperty AperturePaddingProperty = DependencyProperty.Register ("AperturePadding", typeof (Margins), typeof (ItemPanel), new DependencyPropertyMetadata (Margins.Zero));

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
		ItemPanelGroup					parentGroup;
		
		bool							hasDirtyLayout;
		bool							isRefreshPending;
		bool							isCurrentShowPending;
		ItemView						enteredItem;

		double							minItemWidth;
		double							maxItemWidth;
		double							minItemHeight;
		double							maxItemHeight;
	}
}
