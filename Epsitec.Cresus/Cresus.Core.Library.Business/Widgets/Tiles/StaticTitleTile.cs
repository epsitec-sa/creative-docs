//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>StaticTitleTile</c> class implements a basic title tile which has
	/// just an icon and a title text. See <see cref="TitleTile"/> for a specialized
	/// version which manages sub-tiles.
	/// </summary>
	public abstract class StaticTitleTile : ControllerTile, Epsitec.Common.Widgets.Collections.IWidgetCollectionHost<GenericTile>
	{
		protected StaticTitleTile()
			: base (Direction.Right)
		{
			this.items = new TileCollection (this);
			
			this.CreateUI ();
		}


		public ActionViewController				ActionViewController
		{
			get;
			set;
		}

		public bool ContainsFrozenTiles
		{
			get
			{
				return this.Items.Any (item => item.IsFrozen);
			}
		}

		public TileCollection Items
		{
			get
			{
				return this.items;
			}
		}

		public override ITileController Controller
		{
			get
			{
				if ((this.EnableAddItems || this.EnableRemoveItems) && this.ContainsFrozenTiles)
				{
					return this.Items.Select (item => item.Controller).FirstOrDefault ();
				}
				else
				{
					return null;
				}
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
				throw new System.InvalidOperationException ("TitleTile.ArrowMode is read-only");
			}
		}

		protected override bool IsDragAndDropEnabled
		{
			get
			{
				return this.ContainsFrozenTiles;
			}
		}

		protected bool ContainsAnySelectedChildren
		{
			get
			{
				return this.items.Any (x => x.IsSelected);
			}
		}

		public bool CanExpandSubTile
		{
			get;
			set;
		}

		public bool EnableAddItems
		{
			get;
			set;
		}

		public bool EnableRemoveItems
		{
			get;
			set;
		}

		public abstract double GetFullHeight();

		/// <summary>
		/// Icône visible en haut à gauche de la tuile.
		/// Si on donne un seul caractère, il est affiché tel quel.
		/// </summary>
		/// <value>Nom brut de l'icône, sans prefix ni extension.</value>
		public new string						IconUri
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
					this.UpdateStaticIcon ();
				}
			}
		}

		/// <summary>
		/// Titre affiché en haut de la tuile.
		/// </summary>
		/// <value>The title.</value>
		public FormattedText					Title
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
					this.UpdateStaticTitle ();
				}
			}
		}


		protected void OnAddClicked(MessageEventArgs e)
		{
			this.AddClicked.Raise (this, e);
		}

		protected void OnRemoveClicked(MessageEventArgs e)
		{
			this.RemoveClicked.Raise (this, e);
		}

		private void CreateUI()
		{
			this.PreferredWidth = StaticTitleTile.IconSize + 2*StaticTitleTile.IconMargins;

			this.CreateLeftPanel ();
			this.CreateLeftPanelIcon ();
			this.CreateLeftActionPanel ();
			this.CreateRightPanel ();
			this.CreateRightPanelText ();
			this.CreateRightPanelContainer ();
		}

		private void CreateLeftPanel()
		{
			this.leftPanel = new FrameBox
			{
				Parent         = this,
				PreferredWidth = StaticTitleTile.IconSize + 2*StaticTitleTile.IconMargins,
				Dock           = DockStyle.Left,
			};
		}

		private void CreateLeftPanelIcon()
		{
			this.staticTextIcon = new StaticText
			{
				Parent           = this.leftPanel,
				Margins          = new Margins (StaticTitleTile.IconMargins),
				PreferredSize    = new Size (StaticTitleTile.IconSize, StaticTitleTile.IconSize),
				Dock             = DockStyle.Top,
				ContentAlignment = ContentAlignment.MiddleCenter,
			};
		}

		private void CreateLeftActionPanel()
		{
			var leftActionPanel = new FrameBox
			{
				Parent = this.leftPanel,
				Anchor = AnchorStyles.All,
			};

			leftActionPanel.Entered  += new EventHandler<MessageEventArgs> (this.HandleLeftActionPanel_Entered);
			leftActionPanel.Exited   += new EventHandler<MessageEventArgs> (this.HandleLeftActionPanel_Exited);
			leftActionPanel.Pressed  += new EventHandler<MessageEventArgs> (this.HandleLeftActionPanel_Pressed);
			leftActionPanel.Released += new EventHandler<MessageEventArgs> (this.HandleLeftActionPanel_Released);
		}

		private void HandleLeftActionPanel_Entered(object sender, MessageEventArgs e)
		{
			this.ActionPanelEntered ();
		}

		private void HandleLeftActionPanel_Exited(object sender, MessageEventArgs e)
		{
			this.ActionPanelExited ();
		}

		private void HandleLeftActionPanel_Pressed(object sender, MessageEventArgs e)
		{
			this.ActionPanelPressed ();
			e.Message.Swallowed = true;
		}

		private void HandleLeftActionPanel_Released(object sender, MessageEventArgs e)
		{
			e.Message.Swallowed = true;
		}


		private void CreateRightPanel()
		{
			this.rightPanel = new FrameBox
			{
				Parent         = this,
				PreferredWidth = 0,
				Dock           = DockStyle.Fill,
			};
		}

		private void CreateRightPanelText()
		{
			this.staticTextTitle = new StaticText
			{
				Parent           = this.rightPanel,
				PreferredHeight  = StaticTitleTile.TitleHeight,
				PreferredWidth   = 0,
				Dock             = DockStyle.Top,
				Margins          = this.ContainerPadding + new Margins (GenericTile.LeftRightGap, 0, 0, 0),
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
			};
		}
		
		private void CreateRightPanelContainer()
		{
			this.mainPanel = new FrameBox
			{
				Parent         = this.rightPanel,
				PreferredWidth = 0,
				Dock           = DockStyle.Fill,
				Margins        = new Margins (0, 0, 0, 1),
			};
		}


		private void ActionPanelEntered()
		{
			//	Feedback visuel lorsque la souris entre dans la zone d'action.
			if (this.ActionViewControllerMode != ActionViewControllerMode.Full)
			{
				this.leftPanel.BackColor = StaticTitleTile.GetHiliteColor (this.Arrow.GetSurfaceColors ().FirstOrDefault ());
				this.leftPanel.DrawFullFrame = true;

				this.ActionViewControllerMode = ActionViewControllerMode.Dimmed;
			}
		}

		private void ActionPanelExited()
		{
			//	Feedback visuel lorsque la souris sort de la zone d'action.
			if (this.ActionViewControllerMode != ActionViewControllerMode.Full)
			{
				this.leftPanel.BackColor = Color.Empty;
				this.leftPanel.DrawFullFrame = false;

				this.ActionViewControllerMode = ActionViewControllerMode.Hide;
			}
		}

		private void ActionPanelPressed()
		{
			//	Agit lorsque la zone d'action est cliquée.
			this.leftPanel.BackColor = Color.Empty;
			this.leftPanel.DrawFullFrame = false;

			this.ActionViewControllerMode = ActionViewControllerMode.Full;
		}

		private ActionViewControllerMode ActionViewControllerMode
		{
			get
			{
				if (this.ActionViewController == null)
				{
					return ActionViewControllerMode.Hide;
				}
				else
				{
					return this.ActionViewController.ShowMode;
				}
			}
			set
			{
				if (this.ActionViewController != null)
				{
					this.ActionViewController.ShowMode = value;
				}
			}
		}

		private static Color GetHiliteColor(Color color)
		{
			//	Retourne une couleur claire permettant de faire un hilite.
			double h,s,v;
			color.GetHsv (out h, out s, out v);

			//	+10.0	->	teinte légèrement différente, mais sans excès pour éviter le kitch
			//	0.3		->	teinte très claire
			return Color.FromHsv ((h+10.0)%360.0, 0.3, v);
		}


		private void UpdateStaticIcon()
		{
			if (string.IsNullOrEmpty (this.iconUri) || this.iconUri == "none")
			{
				this.staticTextIcon.Text = "";
			}
			else if (this.iconUri.Length == 1)  // un seul caractère ?
			{
				this.staticTextIcon.FormattedText = FormattedText.FromSimpleText (this.iconUri).ApplyFontSizePercent (200);
			}
			else
			{
				this.staticTextIcon.Text = string.Format (@"<img src=""{0}""/>", Misc.GetResourceIconUri (this.iconUri));
			}
		}

		private void UpdateStaticTitle()
		{
			this.staticTextTitle.FormattedText = this.title.ApplyBold ().ApplyFontSizePercent (120);
			this.staticTextTitle.Visibility    = string.IsNullOrEmpty (this.title.ToSimpleText ()) == false;
		}

		private TileArrowMode GetArrowMode()
		{
			if (this.IsReadOnly)
			{
				if (this.ContainsAnySelectedChildren)
				{
					return Tiles.TileArrowMode.Selected;
				}
			}
			else if (this.CanExpandSubTile)
			{
				if (this.ContainsAnySelectedChildren)
				{
					return Tiles.TileArrowMode.Selected;
				}
			}

			return Tiles.TileArrowMode.Normal;
		}


		#region IWidgetCollectionHost<GroupingTile> Members

		void Common.Widgets.Collections.IWidgetCollectionHost<GenericTile>.NotifyInsertion(GenericTile widget)
		{
			widget.Dock   = this.Dock;
			widget.Parent = this.mainPanel;

			this.AttachEventHandlers (widget);
		}

		void Common.Widgets.Collections.IWidgetCollectionHost<GenericTile>.NotifyRemoval(GenericTile widget)
		{
			widget.Parent  = null;
			//-			widget.Hilited = false;

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
			public TileCollection(StaticTitleTile host)
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
			this.Invalidate ();
		}

		private void HandleChildWidgetSelectedOrDeselected(object sender)
		{
			this.Invalidate ();
		}
		

		
		
		public event EventHandler<MessageEventArgs>		AddClicked;
		public event EventHandler<MessageEventArgs>		RemoveClicked;

		protected static readonly double IconSize         = 32;
		protected static readonly double IconMargins      = 2;
		protected static readonly double TitleHeight      = 20;
		protected static readonly double MinimumTileWidth = StaticTitleTile.IconSize + StaticTitleTile.IconMargins*2;

		protected readonly TileCollection		items;

		private string							iconUri;
		private FormattedText					title;

		protected FrameBox						leftPanel;
		protected FrameBox						rightPanel;
		protected FrameBox						mainPanel;

		private StaticText						staticTextIcon;
		private StaticText						staticTextTitle;
	}
}
