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

			this.panel.DefineParentGroup (this);
			this.panel.ItemViewDefaultSize = this.ParentPanel.ItemViewDefaultSize;
			
			this.SetPanelIsExpanded (view.IsExpanded);

			this.ParentPanel.AddPanelGroup (this);
		}

		internal bool HasValidUserInterface
		{
			get
			{
				return this.hasValidUserInterface;
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

		internal void SetPanelIsExpanded(bool expanded)
		{
			if (!expanded)
			{
				this.ItemView.ClearUserInterface ();
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
		/// calling <see cref="RefreshUserInterface"/>.
		/// </summary>
		internal void ClearUserInterface()
		{
			this.panel.ClearUserInterface ();
			
			this.hasClearedUserInterface |= this.hasValidUserInterface;
			this.hasValidUserInterface    = false;
		}

		internal void RefreshUserInterface()
		{
			if (this.hasValidUserInterface == false)
			{
				this.hasValidUserInterface   = true;
				this.hasClearedUserInterface = false;
				
				this.panel.RefreshUserInterface ();
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
				this.ParentPanel.NotifyItemViewSizeChanged (this.ItemView, oldSize, newSize);
			}
		}

		public override Drawing.Size GetBestFitSize()
		{
			Drawing.Size size;

			if (this.ItemView.IsExpanded)
			{
				if (this.hasClearedUserInterface)
				{
					this.RefreshUserInterface ();
				}
				else
				{
					this.panel.RefreshLayoutIfNeeded ();
				}

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

		private ItemPanel panel;
		
		private readonly object exclusion = new object ();
		private bool hasValidUserInterface;
		private bool hasClearedUserInterface;

		internal static ItemPanelGroup GetNextSibling(ItemPanelGroup group)
		{
			int index;
			int count;

			ItemPanel panel = group.ParentPanel;
			ItemView  view  = group.ItemView;

			group = panel.ParentGroup;

		again:
			if (group == null)
			{
				return null;
			}
			
			panel = group.ParentPanel;
			index = group.ItemView.Index+1;
			count = panel.GetItemViewCount ();

			while (index >= count)
			{
				index = 0;
				group = ItemPanelGroup.GetNextSibling (group);

				if (group == null)
				{
					return null;
				}

				panel = group.ParentPanel;
				count = panel.GetItemViewCount ();
			}

			view  = panel.GetItemView (index);
			group = view.Group;
			panel = group.ChildPanel;
			count = panel.GetItemViewCount ();

			if (count == 0)
			{
				goto again;
			}

			view  = panel.GetItemView (0);
			group = view.Group;

			return group;
		}
		
		internal static ItemPanelGroup GetPrevSibling(ItemPanelGroup group)
		{
			int index;
			int count;

			ItemPanel panel = group.ParentPanel;
			ItemView  view  = group.ItemView;

			group = panel.ParentGroup;

		again:
			if (group == null)
			{
				return null;
			}

			panel = group.ParentPanel;
			index = group.ItemView.Index-1;
			count = panel.GetItemViewCount ();

			while (index < 0)
			{
				group = ItemPanelGroup.GetPrevSibling (group);

				if (group == null)
				{
					return null;
				}

				panel = group.ParentPanel;
				count = panel.GetItemViewCount ();
				index = count-1;
			}

			view  = panel.GetItemView (index);
			group = view.Group;
			panel = group.ChildPanel;
			count = panel.GetItemViewCount ();

			if (count == 0)
			{
				goto again;
			}

			view  = panel.GetItemView (count-1);
			group = view.Group;

			return group;
		}
	}
}
