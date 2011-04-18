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
		protected StaticTitleTile()
			: base (Direction.Right)
		{
			this.CreateUI ();
		}


		/// <summary>
		/// Icône visible en haut à gauche de la tuile.
		/// Si on donne un seul caractère, il est affiché tel quel.
		/// </summary>
		/// <value>Nom brut de l'icône, sans prefix ni extension.</value>
		public string							IconUri
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


		private void CreateUI()
		{
			this.PreferredWidth = StaticTitleTile.IconSize + 2*StaticTitleTile.IconMargins;

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
				PreferredHeight  = StaticTitleTile.TitleHeight,
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


		private void UpdateStaticIcon()
		{
			if ((string.IsNullOrEmpty (this.iconUri)) ||
				(this.iconUri == "none"))
			{
				this.staticTextIcon.Text = "";
			}
			else if (this.iconUri.Length == 1)  // un seul caractère ?
			{
				this.staticTextIcon.FormattedText = FormattedText.FromSimpleText (this.iconUri).ApplyFontSizePercent (200);
			}
			else
			{
				this.staticTextIcon.Text = string.Format (@"<img src=""{0}""/>", FormattedText.Escape (this.iconUri));
			}
		}

		private void UpdateStaticTitle()
		{
			this.staticTextTitle.FormattedText = this.title.ApplyBold ().ApplyFontSizePercent (120);
			this.staticTextTitle.Visibility    = string.IsNullOrEmpty (this.title.ToSimpleText ());
		}

		protected static readonly double IconSize		= 32;
		protected static readonly double IconMargins	= 2;
		protected static readonly double TitleHeight	= 20;
		protected static readonly double MinimumTileWidth = StaticTitleTile.IconSize + StaticTitleTile.IconMargins*2;

		private string							iconUri;
		private FormattedText					title;
		
		protected FrameBox						leftPanel;
		protected FrameBox						rightPanel;
		protected FrameBox						mainPanel;

		private StaticText						staticTextIcon;
		private StaticText						staticTextTitle;
	}
}
