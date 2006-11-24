//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.panel = new ItemPanel ();
			
			this.panel.Dock = Widgets.DockStyle.Fill;
			this.panel.SetEmbedder (this);
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
			}
			else
			{
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

		private ItemPanel panel;
		private ItemPanel parentPanel;
		private ItemView parentView;
		private Drawing.Size defaultCompactSize;
	}
}
