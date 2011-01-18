//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// Cette tuile contient tout ce qu'il faut pour éditer une entité.
	/// Son parent est forcément un TileGrouping.
	/// </summary>
	public class EditionTile : GenericTile
	{
		public EditionTile()
		{
			this.container = new FrameBox
			{
				Parent = this,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, TileArrow.Breadth, 0, 3),
			};
		}

		public EditionTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		/// <summary>
		/// Donne le conteneur dans lequel on va mettre tous les widgets permettant d'éditer l'entité associée à la tuile.
		/// </summary>
		/// <value>The container.</value>
		public FrameBox Container
		{
			get
			{
				return this.container;
			}
		}

		public bool AllowSelection
		{
			get;
			set;
		}

		public bool TileArrowHilite
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


		public override TileArrowMode ArrowMode
		{
			get
			{
				return this.GetArrowMode ();
			}
			set
			{
				throw new System.NotImplementedException ();
			}
		}

		public override TileArrow TileArrow
		{
			get
			{
				var arrow = new TileArrow ();

				arrow.SetOutlineColors   (this.OutlineColors);
				arrow.SetThicknessColors (this.ThicknessColors);
				arrow.SetSurfaceColors   (this.SurfaceColors);
				arrow.MouseHilite = this.MouseHilite;

				return arrow;
			}
		}


		private TileArrowMode GetArrowMode()
		{
			if (this.AllowSelection)
			{
				if (this.IsSelected)
				{
					return Widgets.TileArrowMode.Selected;
				}
			}

			return Widgets.TileArrowMode.Normal;
		}

		private bool MouseHilite
		{
			get
			{
				return Misc.ColorsCompare (this.SurfaceColors, Tile.SurfaceHilitedColors) || 
					   Misc.ColorsCompare (this.SurfaceColors, Tile.SurfaceHilitedSelectedColors);
			}
		}

		private IEnumerable<Color> SurfaceColors
		{
			get
			{
				if (this.AllowSelection)
				{
					if (this.TileArrowHilite)
					{
						if (this.IsSelected)
						{
							return Tile.SurfaceHilitedSelectedColors;
						}
						else
						{
							return Tile.SurfaceHilitedColors;
						}
					}

					if (this.IsSelected)
					{
						return Tile.SurfaceSelectedGroupingColors;
					}
				}

				return null;
			}
		}

		private IEnumerable<Color> OutlineColors
		{
			get
			{
				if (this.AllowSelection)
				{
					if (this.TileArrowHilite || this.IsSelected)
					{
						return Tile.BorderColors;
					}
				}

				return null;
			}
		}

		private IEnumerable<Color> ThicknessColors
		{
			get
			{
				return null;
			}
		}


		private FrameBox container;
		private bool tileHilite;
	}
}
