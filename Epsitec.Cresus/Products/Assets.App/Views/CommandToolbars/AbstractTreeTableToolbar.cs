//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	/// <summary>
	/// La toolbar s'adapte en fonction de la largeur disponible. Certains
	/// boutons non indispensables disparaissent s'il manque de la place.
	/// </summary>
	public abstract class AbstractTreeTableToolbar : AbstractCommandToolbar
	{
		public AbstractTreeTableToolbar(DataAccessor accessor, CommandContext commandContext)
			: base (accessor, commandContext)
		{
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.toolbar.SizeChanged += delegate
			{
				this.Adjust ();
			};
		}


		#region Magic layout engine
		private void Adjust()
		{
			//	Adapte la toolbar en fonction de la largeur disponible. Certains boutons
			//	non indispensables disparaissent s'il manque de la place.
			if (this.toolbar == null)
			{
				return;
			}

			//	Cache tous les widgets.
			foreach (var widget in this.toolbar.Children.Widgets.Where (x => x.Dock == DockStyle.None))
			{
				widget.SetManualBounds (Rectangle.Empty);
			}

			//	Cherche le level maximal permettant de tout afficher dans la largeur à disposition.
			int level = this.MaxLevel;
			double dispo = this.toolbar.ActualWidth - this.ComputeDockedWidth ();

			while (level >= 0)
			{
				double width = this.ComputeRequiredWidth (level);

				if (width <= dispo)  // largeur toolbar suffisante ?
				{
					//	On positionne les widgets selon leurs largeurs respectives,
					//	de gauche à droite.
					double x = 0;

					foreach (var widget in this.GetWidgetsLevel (level))
					{
						x += widget.Margins.Left;

						var rect = new Rectangle (x, 0, widget.PreferredWidth, widget.PreferredHeight);
						widget.SetManualBounds (rect);

						x += widget.PreferredWidth + widget.Margins.Right;
					}

					break;
				}
				else
				{
					level--;  // on essaie à nouveau avec moins de widgets
				}
			}
		}

		private int MaxLevel
		{
			//	Retourne le level maximal, donc celui qui permet forcémentde voir
			//	tous les widgets.
			get
			{
				return this.toolbar.Children.Widgets
					.Where (x => x.Dock == DockStyle.None)
					.Max (x => x.Index);
			}
		}

		private double ComputeDockedWidth()
		{
			//	Retourne la largeur utilisée par tous les widgets dockés normalement.
			return this.toolbar.Children.Widgets
				.Where (x => x.Dock != DockStyle.None)
				.Sum (x => x.Margins.Left + x.PreferredWidth + x.Margins.Right);
		}

		private double ComputeRequiredWidth(int level)
		{
			//	Retourne la largeur requise pour un level ainsi que pour tous les levels inférieurs.
			return this.GetWidgetsLevel (level)
				.Sum (x => x.Margins.Left + x.PreferredWidth + x.Margins.Right);
		}

		private IEnumerable<Widget> GetWidgetsLevel(int level)
		{
			//	Retourne tous les widgets que l'on souhaite voir présent pour un level
			//	ainsi que pour tous les levels inférieurs.
			return this.toolbar.Children.Widgets
				.Where (x => x.Dock == DockStyle.None && x.Index <= level);
		}
		#endregion


		protected ButtonWithRedDot CreateButton(Command command, int level)
		{
			//	Crée un bouton pour une commande.
			var size = this.toolbar.PreferredHeight;

			return new ButtonWithRedDot
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = DockStyle.None,
				PreferredSize = new Size (size, size),
				CommandObject = command,
				Index         = level,
			};
		}

		protected FrameBox CreateSeparator(int level)
		{
			//	Crée un séparateur sous la forme d'une petite barre verticale.
			var size = this.toolbar.PreferredHeight;

			var sep = new FrameBox
			{
				Parent        = this.toolbar,
				Dock          = DockStyle.None,
				PreferredSize = new Size (1, size),
				Margins       = new Margins (AbstractCommandToolbar.separatorWidth/2, AbstractCommandToolbar.separatorWidth/2, 0, 0),
				BackColor     = ColorManager.SeparatorColor,
				Index         = level,
			};

			return sep;
		}

		protected void CreateSajex(int width, int level)
		{
			//	Crée un espace vide.
			new FrameBox
			{
				Parent        = this.toolbar,
				Dock          = DockStyle.None,
				PreferredSize = new Size (width, this.toolbar.PreferredHeight),
				Index         = level,
			};
		}
	}
}
