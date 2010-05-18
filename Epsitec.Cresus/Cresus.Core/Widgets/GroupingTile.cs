//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Cette tuile regroupe plusieurs tuiles simples (AbstractTile) dans son conteneur (Container).
	/// Elle affiche une icône en haut à gauche (TopLeftIconUri) et un titre (Title).
	/// </summary>
	public class GroupingTile : Tile
	{
		public GroupingTile()
		{
			this.PreferredWidth = GroupingTile.iconSize+GroupingTile.iconMargins*2;
			this.childrenTiles = new List<ContainerTile> ();
			this.CreateUI ();
		}

		public GroupingTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		static GroupingTile()
		{
			DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (GroupingTile.iconSize+GroupingTile.iconMargins*2);

			Common.Widgets.Visual.PreferredHeightProperty.OverrideMetadata (typeof (GroupingTile), metadataDy);
		}


		public List<ContainerTile> ChildrenTiles
		{
			get
			{
				return this.childrenTiles;
			}
		}

		/// <summary>
		/// Détermine si le widget est sensible au survol de la souris.
		/// </summary>
		/// <value><c>true</c> if [entered sensitivity]; otherwise, <c>false</c>.</value>
		public bool EnteredSensitivity
		{
			get
			{
				return this.enteredSensitivity;
			}
			set
			{
				this.enteredSensitivity = value;
			}
		}

		public override TileArrowMode ArrowMode
		{
			get
			{
				return this.GetPaintingArrowMode ();
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

	
		public virtual FrameBox Container
		{
			get
			{
				return this.mainPanel;
			}
		}


		public static double WidthWithOnlyIcon
		{
			get
			{
				return GroupingTile.iconSize+GroupingTile.iconMargins*2;
			}
		}
	

		/// <summary>
		/// Icône visible en haut à gauche de la tuile.
		/// Si on donne un seul caractère, il est affiché tel quel.
		/// </summary>
		/// <value>Nom brut de l'icône, sans prefix ni extension.</value>
		public string TopLeftIconUri
		{
			get
			{
				return this.topLeftIconUri;
			}
			set
			{
				this.topLeftIconUri = value;

				if (string.IsNullOrEmpty (this.topLeftIconUri) || this.topLeftIconUri.Length == 1)  // un seul caractère ?
				{
					this.staticTextTopLeftIcon.Text = string.Concat ("<font size=\"200%\">", this.topLeftIconUri, "</font>");
				}
				else
				{
					this.staticTextTopLeftIcon.Text = Misc.GetResourceIconImageTag (value);
				}
			}
		}

		/// <summary>
		/// Titre affiché en haut de la tuile.
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				this.title = value;
				this.staticTextTitle.Text = string.Concat ("<b><font size=\"120%\">", this.title, "</font></b>");
			}
		}


		private void CreateUI()
		{
			//	Crée deux panneaux gauche/droite.
			this.leftPanel = new FrameBox
			{
				Parent = this,
				PreferredWidth = GroupingTile.iconSize+GroupingTile.iconMargins*2,
				Dock = DockStyle.Left,
			};

			this.rightPanel = new FrameBox
			{
				Parent = this,
				PreferredWidth = 0,
				Dock = DockStyle.Fill,
			};

			//	Crée le contenu du panneau de gauche.
			this.staticTextTopLeftIcon = new StaticText
			{
				Parent = this.leftPanel,
				Margins = new Margins (GroupingTile.iconMargins),
				PreferredSize = new Size (GroupingTile.iconSize, GroupingTile.iconSize),
				Dock = DockStyle.Top,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
			};

			//	Crée le contenu du panneau de droite.
			this.staticTextTitle = new StaticText
			{
				Parent = this.rightPanel,
				PreferredHeight = GroupingTile.titleHeight,
				PreferredWidth = 0,
				Dock = DockStyle.Top,
				Margins = new Margins (2, TileArrow.Breadth, 0, 0),
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
			};

			this.mainPanel = new FrameBox
			{
				Parent = this.rightPanel,
				PreferredWidth = 0,
				Dock = DockStyle.Fill,
			};
		}


		protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
		{
			if (newRect.Width <= GroupingTile.WidthWithOnlyIcon)  // icône seule ?
			{
				this.rightPanel.Visibility = false;
			}
			else
			{
				this.rightPanel.Visibility = true;
			}
		}



		private TileArrowMode GetPaintingArrowMode()
		{
			if (this.IsReadOnly == false)
			{
				return Widgets.TileArrowMode.None;
			}
			else
			{
				if (this.enteredSensitivity && this.IsEntered && !this.HasManyChildren && this.HasSelectedChildren)
				{
					return Widgets.TileArrowMode.VisibleReverse;
				}

				if (this.enteredSensitivity && ((this.IsEntered && !this.HasManyChildren) || this.HasEnteredChildren))
				{
					return Widgets.TileArrowMode.VisibleDirect;
				}

				if (this.HasSelectedChildren || this.HasEnteredChildren)
				{
					return Widgets.TileArrowMode.VisibleDirect;
				}
			}

			return Widgets.TileArrowMode.None;
		}

		private Color GetSurfaceColor()
		{
			if (this.IsReadOnly == false)
			{
				return Tile.SurfaceEditingColor;
			}
			else
			{
				if (this.enteredSensitivity && (this.IsEntered || this.HasEnteredChildren))
				{
					return Tile.SurfaceHilitedColor;
				}

				if (this.HasSelectedChildren)
				{
					return Tile.SurfaceSelectedGroupingColor;
				}
			}

			return Tile.SurfaceSummaryColor;
		}

		private Color GetOutlineColor()
		{
			return Tile.BorderColor;
		}

		private Color GetThicknessColor()
		{
			if (this.IsReadOnly == false)
			{
				return Color.Empty;
			}
			else
			{
				if (this.enteredSensitivity && ((this.IsEntered && !this.HasManyChildren) || this.HasEnteredChildren) && (!this.HasSelectedChildren || this.HasManyChildren))
				{
					return Tile.ThicknessHilitedColor;
				}
			}

			return Color.Empty;
		}


		private Color GetReverseSurfaceColor()
		{
			if (this.IsReadOnly == false)
			{
				return Color.Empty;
			}
			else
			{
				return Tile.SurfaceHilitedColor;
			}
		}

		private Color GetReverseOutlineColor()
		{
			return Tile.BorderColor;
		}

		private Color GetReverseThicknessColor()
		{
			if (this.IsReadOnly == false)
			{
				return Color.Empty;
			}
			else
			{
				return Tile.ThicknessHilitedColor;
			}
		}


		private bool HasManyChildren
		{
			get
			{
				return this.childrenTiles.Count > 1;
			}
		}

		private bool HasEnteredChildren
		{
			get
			{
				foreach (var containerTile in this.childrenTiles)
				{
					if (containerTile.IsEntered)
					{
						return true;
					}
				}

				return false;
			}
		}

		private bool HasSelectedChildren
		{
			get
			{
				foreach (var containerTile in this.childrenTiles)
				{
					if (containerTile.IsSelected)
					{
						return true;
					}
				}

				return false;
			}
		}


		private static readonly double iconSize = 32;
		private static readonly double iconMargins = 2;
		private static readonly double titleHeight = 20;


		private readonly List<ContainerTile> childrenTiles;

		private bool enteredSensitivity;

		private FrameBox leftPanel;
		private FrameBox rightPanel;
		protected FrameBox mainPanel;

		private string topLeftIconUri;
		private StaticText staticTextTopLeftIcon;

		private string title;
		private StaticText staticTextTitle;
	}
}
