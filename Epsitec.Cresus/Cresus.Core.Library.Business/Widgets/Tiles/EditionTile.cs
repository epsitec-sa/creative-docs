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


		protected override TileArrowMode GetPaintingArrowMode()
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

		protected override bool GetMouseHilite()
		{
			return Comparer.EqualValues (this.GetSurfaceColors (), TileColors.SurfaceHilitedColors) || 
				   Comparer.EqualValues (this.GetSurfaceColors (), TileColors.SurfaceHilitedSelectedColors);
		}

		protected override IEnumerable<Color> GetSurfaceColors()
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

		protected override IEnumerable<Color> GetOutlineColors()
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


		private FrameBox container;
		private bool tileHilite;
	}
}
