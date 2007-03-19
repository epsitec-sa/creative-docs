//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ItemPanelGroup))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemPanelGroup</c> class represents a specialized <see cref="ItemPanel"/>
	/// which represents a group of items.
	/// </summary>
	public class ItemPanelGroup : ItemViewWidget
	{
		public ItemPanelGroup(ItemView view)
			: base (view)
		{
			view.DefineGroup (this);
			
			this.panel = new ItemPanel (this);
			this.panel.Dock = Widgets.DockStyle.Fill;
			this.panel.Layout = this.ParentPanel.Layout;

			this.panel.ItemSelectionMode  = this.ParentPanel.ItemSelectionMode;
			this.panel.GroupSelectionMode = this.ParentPanel.GroupSelectionMode;
			
			this.panel.CurrentItemTrackingMode = this.ParentPanel.CurrentItemTrackingMode;
			
			this.panel.ItemViewDefaultSize = this.ParentPanel.ItemViewDefaultSize;
			
			this.SetGroupPanelEnable (view.IsExpanded);

			this.ParentPanel.AddPanelGroup (this);
		}

		internal bool HasValidUserInterface
		{
			get
			{
				return this.hasValidUserInterface;
			}
		}
		
		public ItemPanel ParentPanel
		{
			get
			{
				return this.ItemView.Owner;
			}
		}

		public ItemPanel ChildPanel
		{
			get
			{
				return this.panel;
			}
		}

		public CollectionViewGroup CollectionViewGroup
		{
			get
			{
				return this.ItemView.Item as CollectionViewGroup;
			}
		}

		internal void SetGroupPanelEnable(bool enable)
		{
			if (this.panel.IsGroupPanelEnabled != enable)
			{
				if (enable)
				{
					this.panel.SetGroupPanelEnable (enable);
				}
				else
				{
					//	Remember the state of the contents when the group panel gets
					//	collapsed; this is required, as the selected state of the items
					//	would otherwise be lost.

					State state = this.SaveState ();

					lock (this.exclusion)
					{
						this.savedState = state;
					}

					this.panel.SetGroupPanelEnable (enable);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.ParentPanel != null)
				{
					this.ParentPanel.RemovePanelGroup (this);
				}
			}
			
			base.Dispose (disposing);
		}

		internal void RefreshAperture(Drawing.Rectangle aperture)
		{
			System.Diagnostics.Debug.Assert (this.ItemView != null);
			System.Diagnostics.Debug.Assert (this.ParentPanel != null);
			
			Drawing.Rectangle bounds = this.ItemView.Bounds;

			bounds.Deflate (this.Padding);
			bounds.Deflate (this.GetInternalPadding ());
			bounds.Deflate (this.panel.Margins);
			
			aperture = Drawing.Rectangle.Intersection (aperture, bounds);

			if (aperture.IsSurfaceZero)
			{
				this.panel.Aperture = Drawing.Rectangle.Empty;
			}
			else
			{
				this.panel.Aperture = Drawing.Rectangle.Offset (aperture, -bounds.Left, -bounds.Bottom);
			}
		}

		internal void NotifyItemViewChanged(ItemView view)
		{
			System.Diagnostics.Debug.Assert (this.ItemView == view);
			System.Diagnostics.Debug.Assert (this.ParentPanel != null);

			this.UpdateItemViewSize ();
			this.Invalidate ();
		}
		
		internal void MarkUserInterfaceAsValid()
		{
			this.hasValidUserInterface = true;
		}

		/// <summary>
		/// Clears information related to the user interface. This is a softer
		/// version of a dispose where the object can be turned alive again by
		/// calling <see cref="RefreshUserInterface"/>. This takes a snapshot
		/// of the <c>ItemView</c> states before they get lost.
		/// </summary>
		internal void ClearUserInterface()
		{
			//	Remember all items which were selected, so we can reselect
			//	them when the ViewItem objects get recreated...

			if (this.panel.IsGroupPanelEnabled)
			{
				State state = this.SaveState ();

				lock (this.exclusion)
				{
					this.savedState = state;
				}
			}

			this.panel.ClearUserInterface ();
			this.hasValidUserInterface = false;
		}

		internal void RefreshUserInterface()
		{
			this.hasValidUserInterface = true;

			this.panel.RefreshUserInterface ();

			State state = null;

			lock (this.exclusion)
			{
				state = this.savedState;
				this.savedState = null;
			}

			this.RestoreSavedState (state);
		}

		internal void RestoreSavedState(State state)
		{
			if (state != null)
			{
				ItemView view;

				foreach (object item in state.SelectedItems)
				{
					view = this.panel.GetItemView (item);

					if (view != null)
					{
						view.Select (true);
					}
				}
			}
		}

		internal State SaveState()
		{
			State state = new State ();
			
			foreach (ItemView view in this.panel.GetSelectedItemViews ())
			{
				state.AddSelectedItem (view.Item);
			}

			return state;
		}

		internal class State
		{
			public State()
			{
				this.selectedItems = new List<System.WeakReference> ();
			}

			public IEnumerable<object> SelectedItems
			{
				get
				{
					foreach (System.WeakReference item in this.selectedItems)
					{
						object value = item.Target;
						
						if (value != null)
						{
							yield return value;
						}
					}
				}
			}
			
			public void AddSelectedItem(object item)
			{
				this.selectedItems.Add (new System.WeakReference (item));
			}

			public void Remove(object item)
			{
				List<System.WeakReference> list = new List<System.WeakReference> ();

				foreach (System.WeakReference weak in this.selectedItems)
				{
					if ((weak.Target == null) ||
						(weak.Target == item))
					{
						//	Skip element
					}
					else
					{
						list.Add (weak);
					}
				}

				this.selectedItems = list;
			}

			List<System.WeakReference> selectedItems;
		}

		/// <summary>
		/// Gets the selected item views, including the ghost item views which do
		/// not really exist when a group is in compact mode.
		/// </summary>
		/// <param name="filter">The item view filter.</param>
		/// <param name="list">The item view list which gets filled.</param>
		internal void GetSelectedItemViews(System.Predicate<ItemView> filter, List<ItemView> list)
		{
			System.Diagnostics.Debug.Assert (this.ParentPanel != null);

			this.panel.GetSelectedItemViews (filter, list);
				
			Drawing.Size size = this.panel.ItemViewDefaultSize;

			//	If we have selected ghost items for this group, we will provide
			//	dummy (ghost) item view instances so that the root panel can
			//	enforce the selection mode (e.g. only one selected item).

			State state;
			
			lock (this.exclusion)
			{
				state = this.savedState;
			}

			if (state != null)
			{
				foreach (object item in state.SelectedItems)
				{
					ItemViewGhost view = new ItemViewGhost (this, item, size);

					if ((filter == null) ||
						(filter (view)))
					{
						list.Add (view);
					}
				}
			}
		}

		public override Drawing.Margins GetInternalPadding()
		{
			return new Drawing.Margins (0, 0, 20, 0);
		}

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);
			this.RefreshAperture (this.ParentPanel.Aperture);
		}

		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			CollectionViewGroup group = this.CollectionViewGroup;

			double dx = this.ActualWidth;
			double dy = 20;
			double y  = this.ActualHeight - dy;

			if (group != null)
			{
				graphics.AddText (dy, y, dx-dy, dy, group.Name, this.DefaultFont, this.DefaultFontSize, Epsitec.Common.Drawing.ContentAlignment.MiddleLeft);
				graphics.RenderSolid (Drawing.Color.FromBrightness (0));
			}

			string text = this.ItemView.IsExpanded ? "-" : "+";

			double r = 9;

			graphics.AddFilledRectangle ((dy-r)/2, y+(dy-r)/2-1, r, r);
			graphics.RenderSolid (Drawing.Color.FromBrightness (1));
			graphics.AddRectangle ((dy-r)/2, y+(dy-r)/2-1, r, r);
			graphics.AddText (0, y, dy, dy, text, this.DefaultFont, this.DefaultFontSize, Epsitec.Common.Drawing.ContentAlignment.MiddleCenter);
			graphics.RenderSolid (Drawing.Color.FromBrightness (0));
		}

		protected override void OnClicked(Epsitec.Common.Widgets.MessageEventArgs e)
		{
			base.OnClicked (e);

			if ((e.Message.Button == Widgets.MouseButtons.Left) &&
				(this.ItemView != null) &&
				(this.ParentPanel != null))
			{
				this.ParentPanel.ExpandItemView (this.ItemView, !this.ItemView.IsExpanded);
				e.Message.Consumer = this;
			}
		}

		private void UpdateItemViewSize()
		{
			System.Diagnostics.Debug.Assert (this.ItemView != null);

			Drawing.Size oldSize = this.ItemView.Size;
			Drawing.Size newSize = this.GetBestFitSize ();

			if (this.ItemView.IsExpanded)
			{
				this.RefreshUserInterface ();
			}
			else
			{
				this.ClearUserInterface ();
			}

			if (oldSize != newSize)
			{
				this.ItemView.DefineSize (newSize);
			}
		}

		public override Drawing.Size GetBestFitSize()
		{
			Drawing.Size size;

			if (this.ItemView.IsExpanded)
			{
				size  = this.panel.GetContentsSize ();
				size += this.Padding.Size;
				size += this.GetInternalPadding ().Size;
				size += this.panel.Margins.Size;
			}
			else
			{
				double width  = this.ParentPanel.ItemViewDefaultSize.Width;
				double height = this.GetInternalPadding ().Height;

				size = new Drawing.Size (width, height);
			}

			return size;
		}

		#region ItemViewGhost Class

		/// <summary>
		/// The <c>ItemViewGhost</c> class is used to monitor the change from
		/// <c>IsSelected=true</c> to <c>IsSelected=false</c>, which is used
		/// to remove the corresponding item from the selected item list.
		/// </summary>
		private class ItemViewGhost : ItemView
		{
			public ItemViewGhost(ItemPanelGroup group, object item, Drawing.Size defaultSize)
				: base (item, group.ChildPanel, defaultSize)
			{
				this.group = group;
				base.Select (true);
			}

			internal override void Select(bool value)
			{
				if (this.IsSelected != value)
				{
					System.Diagnostics.Debug.Assert (value == false);

					base.Select (value);

					//	Rebuild the list of selected items: remove dead object
					//	references and also remove this item view's item.
					
					List<System.WeakReference> list = new List<System.WeakReference> ();

					lock (this.group.exclusion)
					{
						if (this.group.savedState != null)
						{
							this.group.savedState.Remove (this.Item);
						}
					}
				}
			}
			

			private ItemPanelGroup group;
		}

		#endregion

		private ItemPanel panel;
		
		private State savedState;
		private readonly object exclusion = new object ();
		private bool hasValidUserInterface;
	}
}
