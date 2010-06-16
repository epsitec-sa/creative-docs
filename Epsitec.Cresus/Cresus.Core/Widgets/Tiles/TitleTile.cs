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

		public bool ContainsCollectionItemTiles
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

		public override Controllers.ITileController Controller
		{
			get
			{
				return this.Items.Select (item => item.Controller).FirstOrDefault ();
			}
			set
			{
				throw new System.InvalidOperationException ();
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
				var arrow = new TileArrow ();

				arrow.SetOutlineColors   (this.OutlineColors);
				arrow.SetThicknessColors (this.ThicknessColors);
				arrow.SetSurfaceColors   (this.SurfaceColors);
				arrow.MouseHilite = this.MouseHilite;

				return arrow;
			}
		}

		public override TileArrow ReverseArrow
		{
			get
			{
				var arrow = new TileArrow ();

				arrow.SetOutlineColors   (this.ReverseOutlineColors);
				arrow.SetThicknessColors (this.ReverseThicknessColors);
				arrow.SetSurfaceColors   (this.ReverseSurfaceColors);
				arrow.MouseHilite = false;

				return arrow;
			}
		}

		public override bool IsDragAndDropEnabled
		{
			get
			{
				return !this.ContainsCollectionItemTiles;
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

		private void UpdateDefaultChildHilite()
		{
#if false
			if (this.HasManyChildren)
			{
				bool hilite = this.IsEntered;

				if (hilite && this.items.Any (x => x.IsEntered))
				{
					hilite = false;
				}

				this.Items[0].Hilited = hilite;
				this.Items[0].Invalidate ();
			}
#endif
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
				Margins = new Margins (GenericTile.leftRightGap, TileArrow.Breadth, 0, 0),
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
				Anchor			= AnchorStyles.BottomLeft,
				PreferredSize	= new Size (TitleTile.buttonSize, TitleTile.buttonSize),
				Margins			= new Margins (0, 0, TitleTile.iconSize - TitleTile.buttonSize, 0),
				Visibility		= false,
			};

			this.buttonAdd.Clicked += (sender, e) => this.OnAddClicked (e);
		}


		private void SetButtonVisibility(bool visibility)
		{
			this.buttonAdd.Visibility = visibility && this.ContainsCollectionItemTiles;
		}




		private TileArrowMode GetArrowMode()
		{
			if (this.IsReadOnly)
			{
				if (this.IsEntered && this.AutoReverse && this.HasSelectedChild)
				{
					return Widgets.TileArrowMode.VisibleReverse;
				}

				if (this.HasSelectedChild)
				{
					return Widgets.TileArrowMode.VisibleDirect;
				}
			}

			return Widgets.TileArrowMode.None;
		}

		private bool MouseHilite
		{
			get
			{
				List<Color> surfaceColors = this.SurfaceColors;

				return surfaceColors != null && surfaceColors.Count > 0 &&
					   (surfaceColors[0] == Tile.SurfaceHilitedColors[0] || surfaceColors[0] == Tile.SurfaceHilitedSelectedColors[0]);
			}
		}

		private List<Color> SurfaceColors
		{
			get
			{
				if (this.IsReadOnly)
				{
					if (this.IsEntered)
					{
						if (this.HasSelectedChild)
						{
							return Tile.SurfaceHilitedSelectedColors;
						}
						else
						{
							return Tile.SurfaceHilitedColors;
						}
					}

					if (this.HasSelectedChild)
					{
						return Tile.SurfaceSelectedGroupingColors;
					}

					return Tile.SurfaceSummaryColors;
				}

				return Tile.SurfaceEditingColors;
			}
		}

		private List<Color> OutlineColors
		{
			get
			{
				return Tile.BorderColors;
			}
		}

		private List<Color> ThicknessColors
		{
			get
			{
				return null;
			}
		}


		private List<Color> ReverseSurfaceColors
		{
			get
			{
				if (this.IsReadOnly)
				{
					return Tile.SurfaceHilitedColors;
				}

				return null;
			}
		}

		private List<Color> ReverseOutlineColors
		{
			get
			{
				return Tile.BorderColors;
			}
		}

		private List<Color> ReverseThicknessColors
		{
			get
			{
				if (this.IsReadOnly)
				{
					return Tile.ThicknessHilitedColors;
				}

				return null;
			}
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

		protected virtual void OnAddClicked(MessageEventArgs e)
		{
			var handler = this.AddClicked;
			
			if (handler != null)
			{
				handler (this, e);
			}
		}
		
		static TitleTile()
		{
			DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (TitleTile.iconSize+TitleTile.iconMargins*2);

			Common.Widgets.Visual.PreferredHeightProperty.OverrideMetadata (typeof (TitleTile), metadataDy);
		}


		public event EventHandler<MessageEventArgs> AddClicked;


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
