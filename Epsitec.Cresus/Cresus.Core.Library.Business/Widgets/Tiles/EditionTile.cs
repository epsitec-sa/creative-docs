//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>EditionTile</c> class displays the widgets used to edit data.
	/// Its parent is a <see cref="TileGrouping"/>.
	/// </summary>
	public sealed class EditionTile : GenericTile
	{
		public EditionTile()
		{
			this.container = new FrameBox
			{
				Parent  = this,
				Dock    = DockStyle.Fill,
				Margins = this.ContainerPadding + new Margins (0, 0, 0, 3),
			};
		}


		/// <summary>
		/// Gets the container which will host the widgets used to edit the data represented
		/// by this tile.
		/// </summary>
		/// <value>
		/// The container.
		/// </value>
		public FrameBox							Container
		{
			get
			{
				return this.container;
			}
		}

		public bool								AllowSelection
		{
			get;
			set;
		}

		public bool								Hilite
		{
			get
			{
				return this.tileHilite;
			}
			set
			{
				if (this.tileHilite != value)
				{
					this.tileHilite = value;
					this.Invalidate ();
				}
			}
		}


		protected override TileArrowMode GetPaintingArrowMode()
		{
			if ((this.AllowSelection) &&
				(this.IsSelected))
			{
				return Tiles.TileArrowMode.Selected;
			}
			else
			{
				return Tiles.TileArrowMode.Normal;
			}
		}

		protected override bool GetMouseHilite()
		{
			return Comparer.EqualValues (this.GetSurfaceColors (), TileColors.SurfaceHilitedColors)
				|| Comparer.EqualValues (this.GetSurfaceColors (), TileColors.SurfaceHilitedSelectedColors);
		}

		protected override IEnumerable<Color> GetSurfaceColors()
		{
			if (this.AllowSelection)
			{
				if (this.Hilite)
				{
					if (this.IsSelected)
					{
						return TileColors.SurfaceHilitedSelectedColors;
					}
					else
					{
						return TileColors.SurfaceHilitedColors;
					}
				}
				if (this.IsSelected)
				{
					return TileColors.SurfaceSelectedGroupingColors;
				}
			}
			return null;
		}

		protected override IEnumerable<Color> GetOutlineColors()
		{
			if (this.AllowSelection)
			{
				if (this.Hilite || this.IsSelected)
				{
					return TileColors.BorderColors;
				}
			}
			return null;
		}


		private readonly FrameBox				container;
		private bool							tileHilite;
	}
}
