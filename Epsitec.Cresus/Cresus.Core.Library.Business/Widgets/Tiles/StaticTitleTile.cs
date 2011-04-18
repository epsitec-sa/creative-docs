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
	/// The <c>StaticTitleTile</c> class implements a basic title tile which has
	/// just an icon and a title text. See <see cref="TitleTile"/> for a specialized
	/// version which manages sub-tiles.
	/// </summary>
	public abstract class StaticTitleTile : ControllerTile
	{
		public StaticTitleTile()
			: base (Direction.Right)
		{
			this.CreateUI ();
		}


		/// <summary>
		/// Icône visible en haut à gauche de la tuile.
		/// Si on donne un seul caractère, il est affiché tel quel.
		/// </summary>
		/// <value>Nom brut de l'icône, sans prefix ni extension.</value>
		public string TitleIconUri
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

					if ((string.IsNullOrEmpty (this.iconUri)) ||
						(this.iconUri == "none"))
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
					this.staticTextTitle.Visibility = !string.IsNullOrEmpty (this.title);
				}
			}
		}


		private void CreateUI()
		{
			this.PreferredWidth = StaticTitleTile.iconSize + 2*StaticTitleTile.iconMargins;

			this.CreateLeftPanel ();
			this.CreateLeftPanelIcon ();
			this.CreateRightPanel ();
			this.CreateRightPanelText ();
			this.CreateRightPanelContainer ();
		}

		private void CreateLeftPanel()
		{
			this.leftPanel = new FrameBox
			{
				Parent         = this,
				PreferredWidth = StaticTitleTile.iconSize + 2*StaticTitleTile.iconMargins,
				Dock           = DockStyle.Left,
			};
		}

		private void CreateLeftPanelIcon()
		{
			this.staticTextIcon = new StaticText
			{
				Parent           = this.leftPanel,
				Margins          = new Margins (StaticTitleTile.iconMargins),
				PreferredSize    = new Size (StaticTitleTile.iconSize, StaticTitleTile.iconSize),
				Dock             = DockStyle.Top,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
			};
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
				PreferredHeight  = StaticTitleTile.titleHeight,
				PreferredWidth   = 0,
				Dock             = DockStyle.Top,
				Margins          = this.ContainerPadding + new Margins (GenericTile.LeftRightGap, 0, 0, 0),
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode    = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
			};
		}
		
		private void CreateRightPanelContainer()
		{
			this.mainPanel = new FrameBox
			{
				Parent         = this.rightPanel,
				PreferredWidth = 0,
				Dock           = DockStyle.Fill,
			};
		}


		protected static readonly double iconSize		= 32;
		protected static readonly double iconMargins	= 2;
		protected static readonly double titleHeight	= 20;

		private string iconUri;
		private string title;
		
		protected FrameBox leftPanel;
		protected FrameBox rightPanel;
		protected FrameBox mainPanel;

		private StaticText staticTextIcon;
		private StaticText staticTextTitle;
	}
}
