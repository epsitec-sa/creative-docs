//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.CommandToolbars
{
	public abstract class AbstractCommandToolbar : System.IDisposable
	{
		public AbstractCommandToolbar(DataAccessor accessor, CommandContext commandContext)
		{
			this.accessor       = accessor;
			this.commandContext = commandContext;

			this.commandDispatcher = new CommandDispatcher (this.GetType ().FullName, CommandDispatcherLevel.Primary, CommandDispatcherOptions.AutoForwardCommands);
			this.commandDispatcher.RegisterController (this);  // nécesaire pour [Command (Res.CommandIds...)]

			this.adjustRequired = true;
		}

		public void Dispose()
		{
			this.commandDispatcher.Dispose ();
		}

		public void Close()
		{
			if (this.toolbar != null)
			{
				this.toolbar.Children.Clear ();
			}
		}


		public bool								Visibility
		{
			get
			{
				if (this.toolbar == null)
				{
					return false;
				}
				else
				{
					return this.toolbar.Visibility;
				}
			}
			set
			{
				if (this.toolbar != null)
				{
					this.toolbar.Visibility = value;
				}
			}
		}


		public Widget GetTarget(CommandEventArgs e)
		{
			//	Cherche le widget ayant la plus grande surface.
			var targets = this.commandDispatcher.FindVisuals (e.Command)
				.Where  (x => x.Window != null)  // voir (*)
				.OrderByDescending (x => x.PreferredHeight * x.PreferredWidth)
				.ToArray ();

			return targets.FirstOrDefault () as Widget ?? e.Source as Widget;
		}

		// (*)	Cette correction provisoire ne devrait pas être nécessaire. Les boutons
		//		appartenant à des toolbars supprinmées continuent d'êtres trouvés par
		//		FindVisuals ! C'est-ce moi qui supprime mal les widgets d'une toolbar,
		//		ou le bug est-il ailleurs ???


		public virtual void CreateUI(Widget parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
			};

			CommandDispatcher.SetDispatcher (this.toolbar, this.commandDispatcher);  // nécesaire pour [Command (Res.CommandIds...)]

			if (this.adjustRequired)
			{
				this.toolbar.SizeChanged += delegate
				{
					this.MagicLayoutEngine ();
				};
			}
		}


		public void SetVisibility(Command command, bool visibility)
		{
			this.commandContext.GetCommandState (command).Visibility = visibility;

			if (!visibility)
			{
				this.SetEnable (command, false);
			}
		}

		public bool GetEnable(Command command)
		{
			return this.commandContext.GetCommandState (command).Enable;
		}

		public void SetEnable(Command command, bool enable)
		{
			this.commandContext.GetCommandState (command).Enable = enable;
		}

		public void SetActiveState(Command command, bool active)
		{
			this.commandContext.GetCommandState (command).ActiveState = active ? ActiveState.Yes : ActiveState.No;
		}


		protected ButtonWithRedDot CreateButton(DockStyle dock, Command command)
		{
			//	Crée un bouton pour une commande docké normalement.
			var size = this.toolbar.PreferredHeight;

			return new ButtonWithRedDot
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = dock,
				PreferredSize = new Size (size, size),
				CommandObject = command,
			};
		}

		protected ButtonWithRedDot CreateButton(Command command, int triviality)
		{
			//	Crée un bouton pour une commande, qui pourra apparaître ou disparaître
			//	selon le choix du "magic layout engine".
			var size = this.toolbar.PreferredHeight;

			return new ButtonWithRedDot
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = DockStyle.None,
				PreferredSize = new Size (size, size),
				CommandObject = command,
				Index         = triviality,
				Name          = (AbstractCommandToolbar.toto++).ToString (),
			};
		}

		protected FrameBox CreateSeparator(int triviality)
		{
			//	Crée un séparateur sous la forme d'une petite barre verticale, qui
			//	pourra apparaître ou disparaître selon le choix du "magic layout engine".
			var size = this.toolbar.PreferredHeight;

			var sep = new FrameBox
			{
				Parent        = this.toolbar,
				Dock          = DockStyle.None,
				PreferredSize = new Size (1, size),
				Margins       = new Margins (AbstractCommandToolbar.separatorWidth/2, AbstractCommandToolbar.separatorWidth/2, 0, 0),
				BackColor     = ColorManager.SeparatorColor,
				Index         = triviality,
			};

			return sep;
		}

		protected void CreateSajex(int width, int triviality)
		{
			//	Crée un espace vide, qui pourra apparaître ou disparaître
			//	selon le choix du "magic layout engine".
			new FrameBox
			{
				Parent        = this.toolbar,
				Dock          = DockStyle.None,
				PreferredSize = new Size (width, this.toolbar.PreferredHeight),
				Index         = triviality,
			};
		}

		protected void CreateSajex(int width)
		{
			//	Crée un espace vide docké normalement.
			new FrameBox
			{
				Parent        = this.toolbar,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (width, this.toolbar.PreferredHeight),
			};
		}


		#region Magic layout engine
		//	Le "magic layout engine" permet de peupler intelligemment une toolbar, en fonction
		//	de la largeur disponible. Plus la largeur devient petite et plus on supprime des
		//	commandes jugées peu importantes. Pour cela, le paramètre triviality détermine
		//	l’importance, zéro était les commandes les plus importantes. Par exemple, si on
		//	détermine qu’il faut utiliser le niveau deux, toutes les commandes de niveau zéro,
		//	un et deux seront présentes. Les commandes de niveau trois et plus seront absentes.
		//	Pour cela, les widgets utilisent le mode DockStyle.None. Ils seront positionnés
		//	manuellement avec SetManualBounds lorsque la taille de la toolbar change. Pour cacher
		//	les widgets en trop, on ne peut pas utiliser Visibility (cela ne les cache pas). Il
		//	faut définir un Bounds avec un rectangle vide.
		//	Le niveau est stocké dans la propriété Index des widgets, ce qui facilite l'écriture
		//	du code et ne devrait pas interférer avec le fonctionnement standard.

		private void MagicLayoutEngine()
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

			//	Cherche le triviality maximal permettant de tout afficher dans la largeur à disposition.
			int triviality = this.MaxTriviality;
			double dispo = this.toolbar.ActualWidth - this.ComputeDockedWidth ();

			while (triviality >= 0)
			{
				double width = this.ComputeRequiredWidth (triviality);

				if (width <= dispo)  // largeur toolbar suffisante ?
				{
					//	On positionne les widgets selon leurs largeurs respectives,
					//	de gauche à droite.
					double x = 0;

					foreach (var widget in this.GetWidgets (triviality))
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
					triviality--;  // on essaie à nouveau avec moins de widgets
				}
			}
		}

		private int MaxTriviality
		{
			//	Retourne le triviality maximal, donc celui qui permet forcémentde voir
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

		private double ComputeRequiredWidth(int triviality)
		{
			//	Retourne la largeur requise pour un triviality donné ainsi que pour
			//	tous les triviality inférieurs.
			return this.GetWidgets (triviality)
				.Sum (x => x.Margins.Left + x.PreferredWidth + x.Margins.Right);
		}

		private IEnumerable<Widget> GetWidgets(int triviality)
		{
			//	Retourne tous les widgets que l'on souhaite voir présent pour un triviality
			//	donné ainsi que pour tous les triviality inférieurs.
			return this.toolbar.Children.Widgets
				.Where (x => x.Dock == DockStyle.None && x.Index <= triviality);
		}
		#endregion


		public const int primaryToolbarHeight   = 32 + 10;
		public const int secondaryToolbarHeight = 24 + 2;
		public const int separatorWidth         = 11;


		protected readonly DataAccessor			accessor;
		protected readonly CommandDispatcher	commandDispatcher;
		protected readonly CommandContext		commandContext;

		protected FrameBox						toolbar;
		protected bool							adjustRequired;
		private static int toto;
	}
}
