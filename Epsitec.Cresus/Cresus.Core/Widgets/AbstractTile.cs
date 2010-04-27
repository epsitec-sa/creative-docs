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
			this.PreferredWidth = AbstractTile.iconSize+AbstractTile.iconMargins*2;

			//	Crée deux panneaux gauche/droite.
			this.leftPanel = new FrameBox
			{
				Parent = this,
				PreferredWidth = AbstractTile.iconSize+AbstractTile.iconMargins*2,
				Dock = DockStyle.Left,
			};

			this.rightPanel = new FrameBox
			{
				Parent = this,
				PreferredWidth = 0,
				Margins = new Margins (0, AbstractTile.ArrowBreadth, 0, 0),
				Dock = DockStyle.Fill,
			};

			//	Crée le contenu du panneau de gauche.
			this.staticTextTopLeftIcon = new StaticText
			{
				Parent = this.leftPanel,
				Margins = new Margins (AbstractTile.iconMargins),
				PreferredSize = new Size (AbstractTile.iconSize, AbstractTile.iconSize),
				Dock = DockStyle.Top,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
			};

			//	Crée le contenu du panneau de droite.
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
				PreferredWidth = 0,
				Dock = DockStyle.Fill,
			};

			//	Crée les boutons +/-.
			this.buttonCreateEntity = new GlyphButton
			{
				Parent = this,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = Common.Widgets.GlyphShape.Plus,
				Anchor = AnchorStyles.Right | AnchorStyles.Top,
				PreferredSize = new Size (AbstractTile.simpleButtonSize, AbstractTile.simpleButtonSize),
				Margins = new Margins (0, 2+TileContainer.ArrowBreadth+AbstractTile.simpleButtonSize-1, 2, 0),
				Visibility = false,
			};

			this.buttonCreateEntity.Clicked += new EventHandler<MessageEventArgs> (this.HandleButtonCreateEntityClicked);

			this.buttonRemoveEntity = new GlyphButton
			{
				Parent = this,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = Common.Widgets.GlyphShape.Minus,
				Anchor = AnchorStyles.Right | AnchorStyles.Top,
				PreferredSize = new Size (AbstractTile.simpleButtonSize, AbstractTile.simpleButtonSize),
				Margins = new Margins (0, 2+TileContainer.ArrowBreadth, 2, 0),
				Visibility = false,
			};

			this.buttonRemoveEntity.Clicked += new EventHandler<MessageEventArgs> (this.HandleButtonRemoveEntityClicked);
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

		public bool EnableCreateAndRemoveButton
		{
			get;
			set;
		}


		protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
		{
			if (newRect.Width <= AbstractTile.WidthWithOnlyIcon)  // icône seule ?
			{
				this.rightPanel.Visibility = false;
			}
			else
			{
				this.rightPanel.Visibility = true;
			}
		}


		public static double WidthWithOnlyIcon
		{
			get
			{
				return AbstractTile.iconSize+AbstractTile.iconMargins*2;
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

		/// <summary>
		/// Toutes les tuiles au seing d'un groupe sont identifiées par ce numéro, unique pour le groupe.
		/// </summary>
		/// <value>The index of the group.</value>
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


		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);

			if (this.buttonCreateEntity != null && this.EnableCreateAndRemoveButton)
			{
				this.buttonCreateEntity.Visibility = true;
				this.buttonRemoveEntity.Visibility = true;
			}
		}

		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);

			if (this.buttonCreateEntity != null)
			{
				this.buttonCreateEntity.Visibility = false;
				this.buttonRemoveEntity.Visibility = false;
			}
		}


		private void HandleButtonCreateEntityClicked(object sender, MessageEventArgs e)
		{
			this.OnCreateEntity ();
		}

		private void HandleButtonRemoveEntityClicked(object sender, MessageEventArgs e)
		{
			this.OnRemoveEntity ();
		}


		#region Create entity event
		private void OnCreateEntity()
		{
			//	Génère un événement pour dire qu'on veut créer une entité.
			EventHandler handler = (EventHandler) this.GetUserEventHandler (AbstractTile.CreateEntityEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler CreateEntity
		{
			add
			{
				this.AddUserEventHandler (AbstractTile.CreateEntityEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (AbstractTile.CreateEntityEvent, value);
			}
		}

		private const string CreateEntityEvent = "CreateEntity";
		#endregion


		#region Remove entity event
		private void OnRemoveEntity()
		{
			//	Génère un événement pour dire qu'on veut créer une entité.
			EventHandler handler = (EventHandler) this.GetUserEventHandler (AbstractTile.RemoveEntityEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler RemoveEntity
		{
			add
			{
				this.AddUserEventHandler (AbstractTile.RemoveEntityEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (AbstractTile.RemoveEntityEvent, value);
			}
		}

		private const string RemoveEntityEvent = "RemoveEntity";
		#endregion


		private static readonly double iconSize = 32;
		private static readonly double iconMargins = 2;
		private static readonly double titleHeight = 18;
		private static readonly double simpleButtonSize = 16;

		protected bool compactFollower;

		private FrameBox leftPanel;
		private FrameBox rightPanel;
		protected FrameBox mainPanel;

		private string topLeftIconUri;
		private StaticText staticTextTopLeftIcon;

		private string title;
		private StaticText staticTextTitle;

		private GlyphButton buttonCreateEntity;
		private GlyphButton buttonRemoveEntity;
	}
}
