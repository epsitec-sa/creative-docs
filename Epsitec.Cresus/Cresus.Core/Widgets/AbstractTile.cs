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
	public abstract class AbstractTile : TileContainer
	{
		public AbstractTile()
		{
			this.leftPanel = new FrameBox
			{
				Parent = this,
				PreferredWidth = AbstractTile.iconSize+AbstractTile.iconMargins*2,
				Dock = DockStyle.Left,
			};

			this.rightPanel = new FrameBox
			{
				Parent = this,
				Margins = new Margins (0, AbstractTile.ArrowBreadth, 0, 0),
				Dock = DockStyle.Fill,
			};

			this.staticTextTopLeftIcon = new StaticText
			{
				Parent = this.leftPanel,
				Margins = new Margins (AbstractTile.iconMargins),
				PreferredSize = new Size (AbstractTile.iconSize, AbstractTile.iconSize),
				Dock = DockStyle.Top,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
			};

			this.staticTextTitle = new StaticText
			{
				Parent = this.rightPanel,
				PreferredHeight = AbstractTile.titleHeight,
				PreferredWidth = 0,
				Dock = DockStyle.Top,
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split | Common.Drawing.TextBreakMode.SingleLine,
			};

			this.mainPanel = new FrameBox
			{
				Parent = this.rightPanel,
				Dock = DockStyle.Fill,
			};
		}

		public AbstractTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		static AbstractTile()
		{
			DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (AbstractTile.iconSize+AbstractTile.iconMargins*2);

			Common.Widgets.Visual.PreferredHeightProperty.OverrideMetadata (typeof (AbstractTile), metadataDy);
		}


		public AbstractEntity Entity
		{
			get;
			set;
		}

		public Controllers.ViewControllerMode Mode
		{
			get;
			set;
		}

		public Controllers.ViewControllerMode ChildrenMode
		{
			get;
			set;
		}

		public bool CompactFollower
		{
			get
			{
				return this.compactFollower;
			}
			set
			{
				if (this.compactFollower != value)
				{
					this.compactFollower = value;
					this.UpdateCompactFollower ();
				}
			}
		}


		virtual public double ContentHeight
		{
			get
			{
				return this.PreferredHeight;
			}
		}

		/// <summary>
		/// Icône visible en haut à gauche de la tuile.
		/// Si on donne un sul caractère, il est affiché tel quel.
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

		public object Data
		{
			get;
			set;
		}

		public int GroupIndex
		{
			get;
			set;
		}


		protected virtual void UpdateCompactFollower()
		{
			this.staticTextTopLeftIcon.Visibility = !this.compactFollower;
			this.staticTextTitle.Visibility = !this.compactFollower;
		}


		private static readonly double iconSize = 32;
		private static readonly double iconMargins = 2;
		private static readonly double titleHeight = 18;

		protected bool compactFollower;

		private FrameBox leftPanel;
		private FrameBox rightPanel;
		protected FrameBox mainPanel;

		private string topLeftIconUri;
		private StaticText staticTextTopLeftIcon;

		private string title;
		private StaticText staticTextTitle;
	}
}
