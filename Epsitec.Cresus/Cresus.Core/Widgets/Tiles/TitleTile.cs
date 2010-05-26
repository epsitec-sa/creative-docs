//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// Cette tuile regroupe plusieurs tuiles simples (AbstractTile) dans son conteneur (Container).
	/// Elle affiche une icône en haut à gauche (TopLeftIconUri) et un titre (Title).
	/// </summary>
	public class TitleTile : Tile, Epsitec.Common.Widgets.Collections.IWidgetCollectionHost<GenericTile>
	{
		public TitleTile()
		{
			this.items = new TileCollection (this);
			
			this.CreateUI ();
		}


		public bool AutoReverse
		{
			get;
			set;
		}


		public TileCollection Items
		{
			get
			{
				return this.items;
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

		private bool HasSingleChild
		{
			get
			{
				return this.items.Count < 2;
			}
		}

		private bool HasManyChildren
		{
			get
			{
				return this.items.Count > 1;
			}
		}

		private bool HasEnteredChild
		{
			get
			{
				return this.items.Any (x => x.IsEntered);
			}
		}

		private bool HasSelectedChild
		{
			get
			{
				return this.items.Any (x => x.IsSelected);
			}
		}

	
		public static double MinimumTileWidth
		{
			get
			{
				return TitleTile.iconSize+TitleTile.iconMargins*2;
			}
		}
	

		/// <summary>
		/// Icône visible en haut à gauche de la tuile.
		/// Si on donne un seul caractère, il est affiché tel quel.
		/// </summary>
		/// <value>Nom brut de l'icône, sans prefix ni extension.</value>
		public string IconUri
		{
			get
			{
				return this.iconUri;
			}
			set
			{
				if (this.iconUri != value)
				{
					this.iconUri = value;

					if (string.IsNullOrEmpty (this.iconUri))
					{
						this.staticTextIcon.Text = "";
					}
					else if (this.iconUri.Length == 1)  // un seul caractère ?
					{
						this.staticTextIcon.Text = string.Concat ("<font size=\"200%\">", this.iconUri, "</font>");
					}
					else
					{
						this.staticTextIcon.Text = Misc.GetResourceIconImageTag (value);
					}
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
				if (this.title != value)
				{
					this.title = value;
					this.staticTextTitle.Text = string.Concat ("<b><font size=\"120%\">", this.title, "</font></b>");
				}
			}
		}


		public double GetFullHeight()
		{
			double height = TitleTile.titleHeight;

			foreach (var item in this.Items)
			{
				height += item.PreferredHeight;
			}

			return System.Math.Max (height, TitleTile.iconMargins + TitleTile.iconSize);
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

			this.UpdateDefaultChildHilite ();
			this.SetButtonVisibility (true);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);

			if (this.HasChildren)
			{
				this.Items[0].Hilited = false;
				this.Items[0].Invalidate ();
			}

			this.SetButtonVisibility (false);
		}

		private void UpdateDefaultChildHilite()
		{
			if (this.HasManyChildren)
			{
				bool hilite = this.IsEntered;

				if ((hilite) &&
					(this.items.Any (x => x.IsEntered)))
				{
					hilite = false;
				}

				this.Items[0].Hilited = hilite;
				this.Items[0].Invalidate ();
			}
		}

		private void CreateUI()
		{
			this.PreferredWidth = TitleTile.iconSize+TitleTile.iconMargins*2;

			this.CreateLeftPanel ();
			this.CreateLeftPanelIcon ();
			this.CreateLeftPanelButtons ();
			this.CreateRightPanel ();
			this.CreateRightPanelText ();
			this.CreateRightPanelContainer ();
		}

		private void CreateLeftPanel()
		{
			this.leftPanel = new FrameBox
			{
				Parent = this,
				PreferredWidth = TitleTile.iconSize+TitleTile.iconMargins*2,
				Dock = DockStyle.Left,
			};
		}

		private void CreateLeftPanelIcon()
		{
			this.staticTextIcon = new StaticText
			{
				Parent = this.leftPanel,
				Margins = new Margins (TitleTile.iconMargins),
				PreferredSize = new Size (TitleTile.iconSize, TitleTile.iconSize),
				Dock = DockStyle.Top,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
			};
		}

		private void CreateLeftPanelButtons()
		{
			this.CreateAddButton ();
		}

		private void CreateRightPanel()
		{
			this.rightPanel = new FrameBox
			{
				Parent = this,
				PreferredWidth = 0,
				Dock = DockStyle.Fill,
			};
		}

		private void CreateRightPanelText()
		{
			this.staticTextTitle = new StaticText
						{
							Parent = this.rightPanel,
							PreferredHeight = TitleTile.titleHeight,
							PreferredWidth = 0,
							Dock = DockStyle.Top,
							Margins = new Margins (2, TileArrow.Breadth, 0, 0),
							ContentAlignment = ContentAlignment.TopLeft,
							TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
						};
		}
		
		private void CreateRightPanelContainer()
		{
			this.mainPanel = new FrameBox
			{
				Parent = this.rightPanel,
				PreferredWidth = 0,
				Dock = DockStyle.Fill,
			};
		}

		private void CreateAddButton()
		{
			this.buttonAdd = new GlyphButton
			{
				Parent			= this.leftPanel,
				ButtonStyle		= Common.Widgets.ButtonStyle.Normal,
				GlyphShape		= Common.Widgets.GlyphShape.Plus,
				Anchor			= AnchorStyles.TopLeft,
				PreferredSize	= new Size (TitleTile.buttonSize, TitleTile.buttonSize),
				Margins			= new Margins (TitleTile.iconSize - TitleTile.buttonSize, 0, TitleTile.iconSize - TitleTile.buttonSize, 0),
				Visibility		= false,
			};
			
			this.buttonAdd.Clicked +=
				delegate
				{
					//	TODO: ...
				};
		}


		private void SetButtonVisibility(bool visibility)
		{
			this.buttonAdd.Visibility = visibility;
		}




		private TileArrowMode GetArrowMode()
		{
			if (this.IsReadOnly)
			{
				if (this.IsEntered && this.AutoReverse && this.HasSelectedChild)
				{
					return Widgets.TileArrowMode.VisibleReverse;
				}

				if (this.IsEntered)
				{
					return Widgets.TileArrowMode.VisibleDirect;
				}

				if (this.HasSelectedChild)
				{
					return Widgets.TileArrowMode.VisibleDirect;
				}
			}

			return Widgets.TileArrowMode.None;
		}

		private Color GetSurfaceColor()
		{
			if (this.IsReadOnly)
			{
				if (this.IsEntered)
				{
					return Tile.SurfaceHilitedColor;
				}

				if (this.HasSelectedChild)
				{
					return Tile.SurfaceSelectedGroupingColor;
				}
				return Tile.SurfaceSummaryColor;
			}

			return Tile.SurfaceEditingColor;
		}

		private Color GetOutlineColor()
		{
			return Tile.BorderColor;
		}

		private Color GetThicknessColor()
		{
			if (this.IsReadOnly)
			{
				if (this.IsEntered)
				{
					return Tile.ThicknessHilitedColor;
				}
			}

			return Color.Empty;
		}


		private Color GetReverseSurfaceColor()
		{
			if (this.IsReadOnly)
			{
				return Tile.SurfaceHilitedColor;
			}
			
			return Color.Empty;
		}

		private Color GetReverseOutlineColor()
		{
			return Tile.BorderColor;
		}

		private Color GetReverseThicknessColor()
		{
			if (this.IsReadOnly)
			{
				return Tile.ThicknessHilitedColor;
			}
			
			return Color.Empty;
		}


		#region IWidgetCollectionHost<GroupingTile> Members

		void Common.Widgets.Collections.IWidgetCollectionHost<GenericTile>.NotifyInsertion(GenericTile widget)
		{
			widget.Dock = DockStyle.Top;
			widget.ArrowDirection = Direction.Right;
			widget.Parent = this.mainPanel;

			this.AttachEventHandlers (widget);
		}

		void Common.Widgets.Collections.IWidgetCollectionHost<GenericTile>.NotifyRemoval(GenericTile widget)
		{
			widget.Parent  = null;
			widget.Hilited = false;

			this.DetachEventHandlers (widget);
		}

		void Common.Widgets.Collections.IWidgetCollectionHost<GenericTile>.NotifyPostRemoval(GenericTile widget)
		{
		}

		Common.Widgets.Collections.WidgetCollection<GenericTile> Common.Widgets.Collections.IWidgetCollectionHost<GenericTile>.GetWidgetCollection()
		{
			return this.Items;
		}

		#endregion

		#region TileCollection Class

		
		public class TileCollection : Epsitec.Common.Widgets.Collections.WidgetCollection<GenericTile>
		{
			public TileCollection(TitleTile host)
				: base (host)
			{
			}
		}

		#endregion


		private void AttachEventHandlers(GenericTile widget)
		{
			widget.Entered    += this.HandleChildWidgetEnteredOrExited;
			widget.Exited     += this.HandleChildWidgetEnteredOrExited;
			widget.Selected   += this.HandleChildWidgetSelectedOrDeselected;
			widget.Deselected += this.HandleChildWidgetSelectedOrDeselected;
		}

		private void DetachEventHandlers(GenericTile widget)
		{
			widget.Entered    -= this.HandleChildWidgetEnteredOrExited;
			widget.Exited     -= this.HandleChildWidgetEnteredOrExited;
			widget.Selected   -= this.HandleChildWidgetSelectedOrDeselected;
			widget.Deselected -= this.HandleChildWidgetSelectedOrDeselected;
		}

		private void HandleChildWidgetEnteredOrExited(object sender, MessageEventArgs e)
		{
			this.UpdateDefaultChildHilite ();
			this.Invalidate ();
		}

		private void HandleChildWidgetSelectedOrDeselected(object sender)
		{
			this.Invalidate ();
		}

		static TitleTile()
		{
			DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (TitleTile.iconSize+TitleTile.iconMargins*2);

			Common.Widgets.Visual.PreferredHeightProperty.OverrideMetadata (typeof (TitleTile), metadataDy);
		}


		private static readonly double iconSize		= 32;
		private static readonly double iconMargins	= 2;
		private static readonly double titleHeight	= 20;

		private static readonly double buttonSize	= 16;

		private readonly TileCollection items;

		private string iconUri;
		private string title;
		
		private FrameBox leftPanel;
		private FrameBox rightPanel;
		private FrameBox mainPanel;
		
		private GlyphButton buttonAdd;

		private StaticText staticTextIcon;
		private StaticText staticTextTitle;
	}
}
