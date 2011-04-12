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
				Margins = this.ContainerPadding + new Margins (0, 0, 0, 3),
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
				this.tileArrow.SetOutlineColors (this.OutlineColors);
				this.tileArrow.SetSurfaceColors (this.SurfaceColors);
				this.tileArrow.MouseHilite = this.MouseHilite;

				return this.tileArrow;
			}
		}


		private TileArrowMode GetArrowMode()
		{
			if (this.AllowSelection)
			{
				if (this.IsSelected)
				{
					return Tiles.TileArrowMode.Selected;
				}
			}

			return Tiles.TileArrowMode.Normal;
		}

		private bool MouseHilite
		{
			get
			{
				return Comparer.EqualValues (this.SurfaceColors, TileColors.SurfaceHilitedColors) || 
					   Comparer.EqualValues (this.SurfaceColors, TileColors.SurfaceHilitedSelectedColors);
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
		}

		private IEnumerable<Color> OutlineColors
		{
			get
			{
				if (this.AllowSelection)
				{
					if (this.TileArrowHilite || this.IsSelected)
					{
						return TileColors.BorderColors;
					}
				}

				return null;
			}
		}


		private FrameBox container;
		private bool tileHilite;
	}
}
