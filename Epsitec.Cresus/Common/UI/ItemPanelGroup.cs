//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

			this.hasClearedUserInterface = true;
		}

		public ItemPanel ChildPanel
		{
			get
			{
				return this.panel;
			}
		}

		public bool HasValidUserInterface
		{
			get
			{
				return !this.hasClearedUserInterface && !this.hasClearedWidgets;
			}
		}

		public Drawing.Rectangle FocusBounds
		{
			get
			{
				double dx = this.ActualWidth;
				double dy = this.ActualHeight;

				return new Drawing.Rectangle (0, dy - 20, dx, 20);
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
		
		/// <summary>
		/// Clears information related to the user interface. This is a softer
		/// version of a dispose where the object can be turned alive again by
		/// calling <see cref="RefreshUserInterface"/>.
		/// </summary>
		internal void ClearUserInterface()
		{
			if (this.hasClearedUserInterface)
			{
				//	Nothing to do.
			}
			else
			{
				this.panel.ClearUserInterface ();
				this.hasClearedUserInterface = true;
			}
		}

		internal void RefreshUserInterface()
		{
			if (this.hasClearedUserInterface)
			{
				this.hasClearedUserInterface = false;
				this.hasClearedWidgets = !this.panel.Aperture.IsValid;
				
				this.panel.RefreshUserInterface ();
			}
			else if (this.hasClearedWidgets)
			{
				this.RefreshAperture (this.ParentPanel.Aperture);

				if (this.panel.Aperture.IsValid)
				{
					this.hasClearedWidgets = false;
					this.panel.RecreateUserInterface ();
				}
			}
		}

		/// <summary>
		/// Gets the first sibling group in the next parent.
		/// </summary>
		/// <param name="group">The current group.</param>
		/// <returns>The first sibling group in the next parent, or <c>null</c>.</returns>
		internal static ItemPanelGroup GetFirstSiblingInNextParent(ItemPanelGroup group)
		{
			int index;
			int count = 0;

			ItemPanel panel = group.ParentPanel;
			ItemView  view  = group.ItemView;

			group = panel.ParentGroup;

			while (count == 0)
			{
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
					group = ItemPanelGroup.GetFirstSiblingInNextParent (group);

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
			}

			view  = panel.GetItemView (0);
			group = view.Group;

			return group;
		}

		/// <summary>
		/// Gets the last sibling group in the previous parent.
		/// </summary>
		/// <param name="group">The current group.</param>
		/// <returns>The last sibling group in the previous parent, or <c>null</c>.</returns>
		internal static ItemPanelGroup GetLastSiblingInPreviousParent(ItemPanelGroup group)
		{
			int index;
			int count = 0;

			ItemPanel panel = group.ParentPanel;
			ItemView  view  = group.ItemView;

			group = panel.ParentGroup;

			while (count == 0)
			{
				if (group == null)
				{
					return null;
				}

				panel = group.ParentPanel;
				index = group.ItemView.Index-1;
				count = panel.GetItemViewCount ();

				while (index < 0)
				{
					group = ItemPanelGroup.GetLastSiblingInPreviousParent (group);

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
			}

			view  = panel.GetItemView (count-1);
			group = view.Group;

			return group;
		}

		public override Drawing.Margins GetInternalPadding()
		{
			return new Drawing.Margins (0, 0, 20, 0);
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

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);
			this.RefreshAperture (this.ParentPanel.Aperture);
		}

		protected override void PaintBackgroundImplementation(Epsitec.Common.Drawing.Graphics graphics, Epsitec.Common.Drawing.Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			CollectionViewGroup group = this.CollectionViewGroup;
			Drawing.Rectangle bounds = this.FocusBounds;

			double dx = bounds.Width;
			double dy = bounds.Height;
			double y  = bounds.Bottom;

			Drawing.TextStyle style = this.TextLayout.Style;
			

			if (group != null)
			{
				graphics.AddText (dy, y, dx-dy, dy, group.Name, style.Font, style.FontSize, Epsitec.Common.Drawing.ContentAlignment.MiddleLeft);
				graphics.RenderSolid (style.FontColor);
			}

			string text = this.ItemView.IsExpanded ? "-" : "+";

			double r = 9;

			graphics.AddFilledRectangle ((dy-r)/2, y+(dy-r)/2-1, r, r);
			graphics.RenderSolid (Drawing.Color.FromBrightness (1));
			graphics.AddRectangle ((dy-r)/2, y+(dy-r)/2-1, r, r);
			graphics.AddText (0, y, dy, dy, text, style.Font, style.FontSize, Epsitec.Common.Drawing.ContentAlignment.MiddleCenter);
			graphics.RenderSolid (style.FontColor);
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

		private ItemPanel panel;
		
		private readonly object exclusion = new object ();
		private bool hasClearedUserInterface;
		private bool hasClearedWidgets;
	}
}
