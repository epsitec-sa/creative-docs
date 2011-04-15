//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>PanelTitleTile</c> class is a variation of the <see cref="StaticTitleTile"/>
	/// where the panel is below the title and occupies the whole available width. Contrast
	/// this with the <see cref="TitleTile"/> where the panel occupies only the right part.
	/// </summary>
	public class PanelTitleTile : StaticTitleTile
	{
		public PanelTitleTile()
		{
			var topFrame = new FrameBox
			{
				Parent = this,
				Dock = DockStyle.Top,
			};

			var bottomFrame = new FrameBox
			{
				Parent = this,
				Dock = DockStyle.Fill,
				Margins = this.ContainerPadding,
			};

			this.leftPanel.Parent  = topFrame;
			this.rightPanel.Parent = topFrame;
			this.mainPanel.Parent  = bottomFrame;
		}


		/// <summary>
		/// Gets the panel below the icon and the text.
		/// </summary>
		/// <value>The panel.</value>
		public Widget Panel
		{
			get
			{
				return this.mainPanel;
			}
		}
		
		
		public override TileArrow Arrow
		{
			get
			{
				this.tileArrow.SetOutlineColors (TileColors.BorderColors);
				this.tileArrow.SetSurfaceColors (TileColors.SurfaceSummaryColors);
				this.tileArrow.MouseHilite = false;

				return this.tileArrow;
			}
		}
	}
}
