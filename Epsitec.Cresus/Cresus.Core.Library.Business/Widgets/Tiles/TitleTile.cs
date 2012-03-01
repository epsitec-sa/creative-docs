//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// Cette tuile regroupe plusieurs tuiles simples (AbstractTile) dans son conteneur (Container).
	/// Elle affiche une icône en haut à gauche (TitleIconUri) et un titre (Title).
	/// </summary>
	public sealed class TitleTile : StaticTitleTile
	{
		public TitleTile()
		{
		}


		public void SetTileVisibility(string name, bool visibility)
		{
			//	Montre ou cache une tuile d'après son nom.
			//	Si la tuile TitleTile contient une tuile GenericTile utilisant un contrôleur du nom
			//	cherché, elle est montrée/cachée.

			foreach (var item in this.Items.Select (x => x.Controller).OfType<TileDataItem> ())
			{
				if (item.Name == name)
				{
					this.Visibility = visibility;
					break;
				}
			}
		}

		public override double GetFullHeight()
		{
			double height = TitleTile.TitleHeight;

			foreach (var item in this.Items)
			{
				height += item.PreferredHeight;
			}

			return System.Math.Max (height, TitleTile.IconMargins + TitleTile.IconSize);
		}

		
		protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
		{
			if (newRect.Width <= TitleTile.MinimumTileWidth)  // icône seule ?
			{
				this.rightPanel.Visibility = false;
			}
			else
			{
				this.rightPanel.Visibility = true;
			}
		}

		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);

			this.SetButtonVisibility (true);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);

// Suppression de la sélection de la première tuile contenue lorsque le parent TitleTile est cliqué.
// Cela n'était pas logique et causait des interférences avec le drag & drop.
#if false
			if (this.HasChildren)
			{
				this.Items[0].Hilited = false;
				this.Items[0].Invalidate ();
			}
#endif

			this.SetButtonVisibility (false);
		}

		protected override void UpdateTileArrow()
		{
			this.tileArrow.SetOutlineColors (this.GetOutlineColors ());
			this.tileArrow.SetSurfaceColors (this.GetSurfaceColors ());
			this.tileArrow.MouseHilite = this.GetMouseHilite ();
		}

		private void CreateButtons()
		{
#if false
			this.CreateAddButton ();
			this.CreateRemoveButton ();
#endif
		}

		private void CreateAddButton()
		{
			if (this.buttonAdd == null)
			{
				this.buttonAdd = new GlyphButton
				{
					Parent			= this,
					ButtonStyle		= Common.Widgets.ButtonStyle.Normal,
					GlyphShape		= Common.Widgets.GlyphShape.Plus,
					Anchor			= AnchorStyles.TopLeft,
					PreferredSize	= new Size (TitleTile.IconSize, TitleTile.IconSize),
					Margins			= new Margins (TitleTile.IconMargins, 0, TitleTile.IconMargins, 0),
					Visibility		= false,
				};

				this.buttonAdd.Clicked += (sender, e) => this.OnAddClicked (e);
			}
		}

		private void CreateRemoveButton()
		{
			if (this.buttonRemove == null)
			{
				this.buttonRemove = new GlyphButton
				{
					Parent			= this,
					ButtonStyle		= Common.Widgets.ButtonStyle.Normal,
					GlyphShape		= Common.Widgets.GlyphShape.Minus,
					Anchor			= AnchorStyles.BottomRight,
					PreferredSize	= new Size (TitleTile.ButtonSize, TitleTile.ButtonSize),
					Margins         = this.ContainerPadding,
					Visibility		= false,
				};

				this.buttonRemove.Clicked += (sender, e) => this.OnRemoveClicked (e);
			}
		}


		private void SetButtonVisibility(bool visibility)
		{
			bool showAdd    = visibility && this.EnableAddItems;
			bool showRemove = visibility && this.EnableRemoveItems && this.ContainsFrozenTiles;

			if (showAdd || showRemove)
			{
				this.CreateButtons ();
			}

			if (this.buttonAdd != null)
			{
				this.buttonAdd.Visibility = showAdd;
			}

			if (this.buttonRemove != null)
			{
				this.buttonRemove.Visibility = showRemove;
			}
		}


		private bool GetMouseHilite()
		{
			return Comparer.EqualValues (this.GetSurfaceColors (), TileColors.SurfaceHilitedColors)
				|| Comparer.EqualValues (this.GetSurfaceColors (), TileColors.SurfaceHilitedSelectedColors);
		}

		private IEnumerable<Color> GetSurfaceColors()
		{
			if (this.IsReadOnly)
			{
				if (this.IsEntered)
				{
					if (this.ContainsAnySelectedChildren)
					{
						return TileColors.SurfaceHilitedSelectedColors;
					}
					else
					{
						return TileColors.SurfaceHilitedColors;
					}
				}
				if (this.ContainsAnySelectedChildren)
				{
					return TileColors.SurfaceSelectedGroupingColors;
				}

				return TileColors.SurfaceSummaryColors;
			}
			else
			{
				if (this.CanExpandSubTile)
				{
					if (this.IsEntered)
					{
						if (this.ContainsAnySelectedChildren)
						{
							return TileColors.SurfaceHilitedSelectedColors;
						}
						else
						{
							return TileColors.SurfaceHilitedColors;
						}
					}
					if (this.ContainsAnySelectedChildren)
					{
						return TileColors.SurfaceSelectedGroupingColors;
					}
				}
			}
			
			return TileColors.SurfaceEditingColors;
		}

		private IEnumerable<Color> GetOutlineColors()
		{
			return TileColors.BorderColors;
		}


		static TitleTile()
		{
			DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (TitleTile.IconSize+TitleTile.IconMargins*2);

			Common.Widgets.Visual.PreferredHeightProperty.OverrideMetadata (typeof (TitleTile), metadataDy);
		}


		private static readonly double			ButtonSize	= 16;

		private GlyphButton						buttonAdd;
		private GlyphButton						buttonRemove;
	}
}
