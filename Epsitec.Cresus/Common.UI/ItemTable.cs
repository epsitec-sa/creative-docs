//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ItemTable))]

namespace Epsitec.Common.UI
{
	public class ItemTable : Widgets.FrameBox
	{
		public ItemTable()
		{
			this.vScroller = new VScroller (this);
			this.hScroller = new HScroller (this);
			this.columnHeader = new ItemPanelColumnHeader (this);
			this.surface = new Widget (this);
			this.itemPanel = new ItemPanel (this.surface);

			this.vScroller.IsInverted = false;
			this.vScroller.Value = 1;
			
			this.vScroller.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Right;
			this.hScroller.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Bottom;
			this.columnHeader.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.Top;
			this.surface.Anchor = AnchorStyles.All;
			
			this.vScroller.Margins = new Drawing.Margins (0, 0, 0, this.hScroller.PreferredHeight);
			this.hScroller.Margins = new Drawing.Margins (0, this.vScroller.PreferredWidth, 0, 0);
			this.columnHeader.Margins = new Drawing.Margins (0, this.vScroller.PreferredWidth, 0, 0);
			this.surface.Margins = new Drawing.Margins (0, this.vScroller.PreferredWidth, this.columnHeader.PreferredHeight, this.hScroller.PreferredHeight);

			this.hScroller.ValueChanged += this.HandleScrollerValueChanged;
			this.vScroller.ValueChanged += this.HandleScrollerValueChanged;
			this.surface.SizeChanged += this.HandleSurfaceSizeChanged;
			
			this.itemPanel.AddEventHandler (Visual.PreferredHeightProperty, this.HandleItemPanelSizeChanged);
			this.itemPanel.AddEventHandler (Visual.PreferredWidthProperty, this.HandleItemPanelSizeChanged);
		}


		public ItemPanel ItemPanel
		{
			get
			{
				return this.itemPanel;
			}
		}

		public ItemPanelColumnHeader ColumnHeader
		{
			get
			{
				return this.columnHeader;
			}
		}

		
		private void UpdateAperture(Drawing.Size aperture)
		{
			double hRatio = System.Math.Max (0, System.Math.Min (1, (aperture.Width+1) / (this.itemPanel.PreferredWidth+1)));
			double vRatio = System.Math.Max (0, System.Math.Min (1, (aperture.Height+1) / (this.itemPanel.PreferredHeight+1)));

			this.hScroller.VisibleRangeRatio = (decimal) hRatio;
			this.vScroller.VisibleRangeRatio = (decimal) vRatio;

			double ox = (double) this.hScroller.Value * System.Math.Max (0, this.itemPanel.PreferredWidth - aperture.Width);
			double oy = (double) this.vScroller.Value * System.Math.Max (0, this.itemPanel.PreferredHeight - aperture.Height);
			
			this.itemPanel.Aperture = new Drawing.Rectangle (ox, oy, aperture.Width, aperture.Height);

			if (this.itemPanel.PreferredHeight < aperture.Height)
			{
				oy = this.itemPanel.PreferredHeight - aperture.Height;
			}
			
			this.itemPanel.SetManualBounds (new Drawing.Rectangle (-ox, -oy, this.itemPanel.PreferredWidth, this.itemPanel.PreferredHeight));
		}

		private void HandleSurfaceSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateAperture ((Drawing.Size) e.NewValue);
		}

		private void HandleScrollerValueChanged(object sender)
		{
			if (this.surface.IsActualGeometryValid)
			{
				this.UpdateAperture (this.surface.ActualSize);
			}
		}

		private void HandleItemPanelSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.surface.IsActualGeometryValid)
			{
				this.UpdateAperture (this.surface.ActualSize);
			}
		}
		
		private VScroller vScroller;
		private HScroller hScroller;
		private ItemPanelColumnHeader columnHeader;
		private Widget surface;
		private ItemPanel itemPanel;
	}
}
