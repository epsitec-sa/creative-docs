//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
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
			this.leftPanel = new FrameBox (this);
			this.leftPanel.PreferredWidth = AbstractTile.iconSize+AbstractTile.iconMargins*2;
			this.leftPanel.Dock = DockStyle.Left;

			this.rightPanel = new FrameBox (this);
			this.rightPanel.Margins = new Margins (0, AbstractTile.ArrowBreadth, 0, 0);
			this.rightPanel.MinWidth = AbstractTile.ArrowBreadth;
			this.rightPanel.Dock = DockStyle.Fill;

			this.staticTextIconUri = new StaticText (this.leftPanel);
			this.staticTextIconUri.Margins = new Margins (AbstractTile.iconMargins);
			this.staticTextIconUri.PreferredSize = new Size (AbstractTile.iconSize, AbstractTile.iconSize);
			this.staticTextIconUri.Dock = DockStyle.Top;
			this.staticTextIconUri.ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter;

			this.staticTextTitle = new StaticText (this.rightPanel);
			this.staticTextTitle.PreferredHeight = AbstractTile.titleHeight;
			this.staticTextTitle.MinWidth = 0;
			this.staticTextTitle.Dock = DockStyle.Top;

			this.mainPanel = new FrameBox (this.rightPanel);
			this.mainPanel.MinWidth = 0;
			this.mainPanel.Dock = DockStyle.Fill;
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
		public string IconUri
		{
			get
			{
				return this.iconUri;
			}
			set
			{
				this.iconUri = value;

				if (string.IsNullOrEmpty (this.iconUri) || this.iconUri.Length == 1)  // un seul caractère ?
				{
					this.staticTextIconUri.Text = string.Concat ("<font size=\"200%\">", this.iconUri, "</font>");
				}
				else
				{
					this.staticTextIconUri.Text = Misc.GetResourceIconImage (value);
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


		private static readonly double iconSize = 32;
		private static readonly double iconMargins = 5;
		private static readonly double titleHeight = 18;

		private FrameBox leftPanel;
		private FrameBox rightPanel;
		protected FrameBox mainPanel;

		private string iconUri;
		private StaticText staticTextIconUri;

		private string title;
		private StaticText staticTextTitle;
	}
}
