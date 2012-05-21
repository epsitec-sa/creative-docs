//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>ArrowedTileTabPageButton</c> class implements a widget which looks like a
	/// tab page header button, with a 'v' arrow below.
	/// </summary>
	public sealed class ArrowedTileTabPageButton : ArrowedTile
	{
		public ArrowedTileTabPageButton(TabPageDef tabPageDef)
			: base (Direction.Down)
		{
			this.tabPageDef    = tabPageDef;
			this.Name          = tabPageDef.Name;
			this.FormattedText = tabPageDef.Text;
		}


		public TabPageDef TabPageDef
		{
			get
			{
				return this.tabPageDef;
			}
		}

		protected override TileArrowMode GetPaintingArrowMode()
		{
			return (this.IsSelected) ? TileArrowMode.Selected : TileArrowMode.Normal;
		}

		private bool GetMouseHilite()
		{
			return Comparer.EqualValues (this.GetSurfaceColors (), TileColors.SurfaceHilitedColors);
		}

		private IEnumerable<Color> GetSurfaceColors()
		{
			if (this.IsEntered)
			{
				return (this.IsSelected) ? TileColors.SurfaceHilitedSelectedColors : TileColors.SurfaceHilitedColors;
			}
			else
			{
				return (this.IsSelected) ? TileColors.SurfaceSelectedContainerColors : TileColors.SurfaceSummaryColors;
			}
		}


		protected override void UpdateTileArrow()
		{
			this.tileArrow.SetOutlineColors (TileColors.BorderColors);
			this.tileArrow.SetSurfaceColors (this.GetSurfaceColors ());
			this.tileArrow.MouseHilite = this.GetMouseHilite ();
		}
		
		
		private readonly TabPageDef tabPageDef;
	}
}
