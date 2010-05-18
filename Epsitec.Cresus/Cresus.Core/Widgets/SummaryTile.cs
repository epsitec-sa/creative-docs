//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Cette tuile contient un résumé non éditable d'une entité.
	/// Son parent est forcément un TileGrouping.
	/// </summary>
	public class SummaryTile : AbstractTile
	{
		public SummaryTile()
		{
			this.staticTextSummary = new StaticText
			{
				Parent = this,
				PreferredWidth = 0,
				Dock = DockStyle.Fill,
				Margins = new Margins (2, TileArrow.Breadth, 0, 0),
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode = Common.Drawing.TextBreakMode.Ellipsis | Common.Drawing.TextBreakMode.Split,  // TODO: il manque le bon mode !
			};

			//	Crée les boutons +/-.
			this.buttonCreateEntity = new GlyphButton
			{
				Parent = this,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = Common.Widgets.GlyphShape.Plus,
				Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
				PreferredSize = new Size (SummaryTile.simpleButtonSize, SummaryTile.simpleButtonSize),
				Margins = new Margins (0, TileArrow.Breadth+SummaryTile.simpleButtonSize-1, 0, 0),
				Visibility = false,
			};

			this.buttonCreateEntity.Clicked += new EventHandler<MessageEventArgs> (this.HandleButtonCreateEntityClicked);

			this.buttonRemoveEntity = new GlyphButton
			{
				Parent = this,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = Common.Widgets.GlyphShape.Minus,
				Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
				PreferredSize = new Size (SummaryTile.simpleButtonSize, SummaryTile.simpleButtonSize),
				Margins = new Margins (0, TileArrow.Breadth, 0, 0),
				Visibility = false,
			};

			this.buttonRemoveEntity.Clicked += new EventHandler<MessageEventArgs> (this.HandleButtonRemoveEntityClicked);
		}

		public SummaryTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public double ContentHeight
		{
			get
			{
				// TODO: faire cela mieux !
				string[] lines = this.Summary.Split (new string[] { "<br/>" }, System.StringSplitOptions.None);
				return lines.Length*16;
			}
		}

		/// <summary>
		/// Résumé multilignes affiché sous le titre.
		/// </summary>
		/// <value>The content.</value>
		public string Summary
		{
			get
			{
				return this.staticTextSummary.Text;
			}
			set
			{
				this.staticTextSummary.Text = value;
			}
		}


		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);

			if (this.buttonCreateEntity != null && this.EnableCreateAndRemoveButton && this.IsRightColumnParent)
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


		private bool IsRightColumnParent
		{
			get
			{
				Widget widget = this.Parent;

				while (widget != null)
				{
					if (widget is Widgets.ContainerTiles)
					{
						var container = widget as Widgets.ContainerTiles;
						return container.IsRightColumn;
					}

					widget = widget.Parent;
				}

				return false;
			}
		}


		private void HandleButtonCreateEntityClicked(object sender, MessageEventArgs e)
		{
			//	Parfois, lorsque la touche Tab est pressée, ce handler est appelé suite à un Widget.SimulateClicked(),
			//	ce qui ne devrait pas arriver. Pour palier à cette erreur, je teste la visibilité du widget. En effet,
			//	un vrai événement ne peut pas subvenir si le bouton est caché.
			//	TODO: Pour Pierre, à corriger !
			if (this.buttonCreateEntity.Visibility)
			{
				this.OnCreateEntity ();
			}
		}

		private void HandleButtonRemoveEntityClicked(object sender, MessageEventArgs e)
		{
			//	Voir remarque dans HandleButtonCreateEntityClicked !
			if (this.buttonRemoveEntity.Visibility)
			{
				this.OnRemoveEntity ();
			}
		}


		#region Create entity event
		private void OnCreateEntity()
		{
			//	Génère un événement pour dire qu'on veut créer une entité.
			EventHandler handler = (EventHandler) this.GetUserEventHandler (SummaryTile.CreateEntityEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler CreateEntity
		{
			add
			{
				this.AddUserEventHandler (SummaryTile.CreateEntityEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (SummaryTile.CreateEntityEvent, value);
			}
		}

		private const string CreateEntityEvent = "CreateEntity";
		#endregion


		#region Remove entity event
		private void OnRemoveEntity()
		{
			//	Génère un événement pour dire qu'on veut créer une entité.
			EventHandler handler = (EventHandler) this.GetUserEventHandler (SummaryTile.RemoveEntityEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler RemoveEntity
		{
			add
			{
				this.AddUserEventHandler (SummaryTile.RemoveEntityEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (SummaryTile.RemoveEntityEvent, value);
			}
		}

		private const string RemoveEntityEvent = "RemoveEntity";
		#endregion

	
		private static readonly double simpleButtonSize = 16;

		private StaticText staticTextSummary;

		private GlyphButton buttonCreateEntity;
		private GlyphButton buttonRemoveEntity;
	}
}
