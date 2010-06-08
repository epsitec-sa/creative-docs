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
				Margins = new Margins (0, TileArrow.Breadth, 0, 0),
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

		public override TileArrow DirectArrow
		{
			get
			{
				return new TileArrow ()
				{
					OutlineColor = this.GetOutlineColor (),
					ThicknessColor = this.GetThicknessColor (),
					SurfaceColor = this.GetSurfaceColor (),
				};
			}
		}

		public override TileArrow ReverseArrow
		{
			get
			{
				return new TileArrow ()
				{
					OutlineColor = this.GetReverseOutlineColor (),
					ThicknessColor = this.GetReverseThicknessColor (),
					SurfaceColor = this.GetReverseSurfaceColor (),
				};
			}
		}


		private TileArrowMode GetArrowMode()
		{
			if (this.AllowSelection)
			{
				if (this.TileArrowHilite && this.AutoReverse)
				{
					return Widgets.TileArrowMode.VisibleReverse;
				}

				if (this.TileArrowHilite)
				{
					return Widgets.TileArrowMode.VisibleDirect;
				}

				if (this.IsSelected)
				{
					return Widgets.TileArrowMode.VisibleDirect;
				}
			}

			return Widgets.TileArrowMode.None;
		}

		private Color GetSurfaceColor()
		{
			if (this.AllowSelection)
			{
				if (this.TileArrowHilite)
				{
					return Tile.SurfaceHilitedColor;
				}

				if (this.IsSelected)
				{
					return Tile.SurfaceSelectedGroupingColor;
				}
			}

			return Tile.SurfaceEditingColor;
		}

		private Color GetOutlineColor()
		{
			if (this.AllowSelection)
			{
				if (this.TileArrowHilite || this.IsSelected)
				{
					return Tile.BorderColor;
				}
			}

			return Color.Empty;
		}

		private Color GetThicknessColor()
		{
			if (this.AllowSelection)
			{
				if (this.TileArrowHilite)
				{
					return Tile.ThicknessHilitedColor;
				}
			}

			return Color.Empty;
		}


		private Color GetReverseSurfaceColor()
		{
			return Tile.SurfaceHilitedColor;
		}

		private Color GetReverseOutlineColor()
		{
			return Tile.BorderColor;
		}

		private Color GetReverseThicknessColor()
		{
			return Tile.ThicknessHilitedColor;
		}


		private FrameBox container;
		private bool tileHilite;
	}
}
