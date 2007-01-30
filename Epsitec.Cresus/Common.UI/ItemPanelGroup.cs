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
	public class ItemPanelGroup : Widgets.FrameBox
	{
		public ItemPanelGroup()
		{
			this.panel = new ItemPanel (this);
			this.panel.Dock = Widgets.DockStyle.Fill;
		}

		public ItemPanel ParentPanel
		{
			get
			{
				return this.parentPanel;
			}
			set
			{
				if (this.parentPanel != value)
				{
					if (this.parentPanel != null)
					{
						this.parentPanel.RemovePanelGroup (this);
					}

					this.parentPanel = value;

					if (this.parentPanel != null)
					{
						this.panel.Layout = this.parentPanel.Layout;
						
						this.panel.ItemSelection  = this.parentPanel.ItemSelection;
						this.panel.GroupSelection = this.parentPanel.GroupSelection;

						this.panel.ItemViewDefaultSize = this.parentPanel.ItemViewDefaultSize;
						
						this.parentPanel.AddPanelGroup (this);

						if (this.parentView != null)
						{
							this.RefreshAperture (this.parentPanel.Aperture);
						}
					}
				}
			}
		}

		public ItemPanel ChildPanel
		{
			get
			{
				return this.panel;
			}
		}

		public ItemView ParentView
		{
			get
			{
				return this.parentView;
			}
			set
			{
				if (this.parentView != value)
				{
					this.parentView = value;

					if (this.parentView != null)
					{
						this.defaultCompactSize = this.parentView.Size;
						this.UpdateItemViewSize ();
						
						if (this.parentPanel != null)
						{
							this.RefreshAperture (this.parentPanel.Aperture);
						}
					}

					this.Invalidate ();
				}
			}
		}

		public CollectionViewGroup CollectionViewGroup
		{
			get
			{
				return this.parentView.Item as CollectionViewGroup;
			}
		}

		internal void RefreshAperture(Drawing.Rectangle aperture)
		{
			System.Diagnostics.Debug.Assert (this.parentView != null);
			System.Diagnostics.Debug.Assert (this.parentPanel != null);
			
			Drawing.Rectangle bounds = this.parentView.Bounds;

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
			System.Diagnostics.Debug.Assert (this.parentView == view);
			System.Diagnostics.Debug.Assert (this.parentPanel != null);

			this.UpdateItemViewSize ();
			this.Invalidate ();
		}

		/// <summary>
		/// Clears information related to the user interface. This is a softer
		/// version of a dispose where the object can be turned alive again by
		/// calling <see cref="RefreshUserInterface"/>. This takes a snapshot
		/// of the <c>ItemView</c> states before they get lost.
		/// </summary>
		internal void ClearUserInterface()
		{
			if (this.panel != null)
			{
				//	Remember all items which were selected, so we can reselect
				//	them when the ViewItem objects get recreated...

				List<System.WeakReference> selectedGhostItems = new List<System.WeakReference> ();
				
				foreach (ItemView view in this.panel.GetSelectedItemViews ())
				{
					selectedGhostItems.Add (new System.WeakReference (view.Item));
				}

				lock (this.exclusion)
				{
					this.selectedGhostItems = selectedGhostItems;
				}
				
				this.panel.ClearUserInterface ();
			}
		}

		internal void RefreshUserInterface()
		{
			if (this.panel != null)
			{
				this.panel.RefreshUserInterface ();

				lock (this.exclusion)
				{
					foreach (System.WeakReference ghostItem in this.selectedGhostItems)
					{
						object item = ghostItem.Target;

						if (item != null)
						{
							ItemView view = this.panel.GetItemView (item);

							if (view != null)
							{
								view.IsSelected = true;
							}
						}
					}
					
					this.selectedGhostItems.Clear ();
				}
			}
		}

		/// <summary>
		/// Gets the selected item views, including the ghost item views which do
		/// not really exist when a group is in compact mode.
		/// </summary>
		/// <param name="filter">The item view filter.</param>
		/// <param name="list">The item view list which gets filled.</param>
		internal void GetSelectedItemViews(System.Predicate<ItemView> filter, List<ItemView> list)
		{
			System.Diagnostics.Debug.Assert (this.parentPanel != null);

			Drawing.Size size;

			if (this.panel != null)
			{
				this.panel.GetSelectedItemViews (filter, list);
				
				size = this.panel.ItemViewDefaultSize;
			}
			else
			{
				size = this.parentPanel.ItemViewDefaultSize;
			}

			//	If we have selected ghost items for this group, we will provide
			//	dummy (ghost) item view instances so that the root panel can
			//	enforce the selection mode (e.g. only one selected item).

			System.WeakReference[] ghostItems;
			
			lock (this.exclusion)
			{
				ghostItems = this.selectedGhostItems.ToArray ();
			}

			foreach (System.WeakReference ghostItem in ghostItems)
			{
				object item = ghostItem.Target;
				
				if (item != null)
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
			this.RefreshAperture (this.parentPanel.Aperture);
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

			string text = this.parentView.IsExpanded ? "-" : "+";

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
				(this.parentView != null) &&
				(this.parentPanel != null))
			{
				this.parentPanel.ExpandItemView (this.parentView, !this.parentView.IsExpanded);
				e.Message.Consumer = this;
			}
		}

		private void UpdateItemViewSize()
		{
			System.Diagnostics.Debug.Assert (this.parentView != null);

			Drawing.Size oldSize = this.parentView.Size;
			Drawing.Size newSize;

			if (this.parentView.IsExpanded)
			{
				this.panel.SetParentGroup (this);
				
				newSize  = this.panel.PreferredSize;
				newSize += this.Padding.Size;
				newSize += this.GetInternalPadding ().Size;
				newSize += this.panel.Margins.Size;

				this.RefreshUserInterface ();
			}
			else
			{
				this.ClearUserInterface ();
				
				newSize = this.defaultCompactSize;
				
				this.panel.SetParentGroup (null);
			}

			if (oldSize != newSize)
			{
				this.parentView.Size = newSize;
				
				if (this.parentPanel != null)
				{
					this.parentPanel.NotifyItemViewSizeChanged (this.parentView, oldSize, newSize);
				}
			}
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
				: base (item, defaultSize)
			{
				this.group = group;
				base.IsSelected = true;
			}

			public override bool IsSelected
			{
				get
				{
					return base.IsSelected;
				}
				internal set
				{
					if (this.IsSelected != value)
					{
						System.Diagnostics.Debug.Assert (value == false);

						base.IsSelected = value;

						//	Rebuild the list of selected items: remove dead object
						//	references and also remove this item view's item.
						
						List<System.WeakReference> list = new List<System.WeakReference> ();

						lock (this.group.exclusion)
						{
							foreach (System.WeakReference ghostItem in this.group.selectedGhostItems)
							{
								object item = ghostItem.Target;

								if ((item != null) &&
									(item != this.Item))
								{
									list.Add (new System.WeakReference (item));
								}
							}

							this.group.selectedGhostItems = list;
						}
					}
				}
			}

			private ItemPanelGroup group;
		}

		#endregion

		private ItemPanel panel;
		private ItemPanel parentPanel;
		private ItemView parentView;
		private Drawing.Size defaultCompactSize;
		
		private List<System.WeakReference> selectedGhostItems = new List<System.WeakReference> ();
		private readonly object exclusion = new object ();
	}
}
