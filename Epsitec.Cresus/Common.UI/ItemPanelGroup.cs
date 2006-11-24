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
			this.panel.SetParentGroup (this);
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

					if (this.parentPanel != null)
					{
						this.RefreshAperture (this.parentPanel.Aperture);
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

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);
			this.RefreshAperture (this.parentPanel.Aperture);
		}

		private ItemPanel panel;
		private ItemPanel parentPanel;
		private ItemView parentView;
	}
}
