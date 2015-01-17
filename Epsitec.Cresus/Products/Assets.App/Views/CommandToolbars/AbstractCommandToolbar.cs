//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.App.Settings;

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
			if (this.toolbar != null)
			{
				this.toolbar.Children.Clear ();
			}

			this.commandDispatcher.Dispose ();
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

		public bool								ShowHelpline
		{
			get
			{
				if (this.helplineFrame == null || this.helplineTargetButton == null)
				{
					return false;
				}
				else
				{
					return this.helplineFrame.Visibility;
				}
			}
			set
			{
				if (this.helplineFrame != null && this.helplineTargetButton != null)
				{
					this.helplineFrame.Visibility = value;
					this.MagicLayoutEngine ();
				}
			}
		}

		public int								RightMargin;


		public Widget GetTarget(CommandEventArgs e)
		{
			//	Cherche le widget ayant la plus grande surface.
			return this.GetTarget (e.Command);
		}

		public Widget GetTarget(Command command)
		{
			//	Cherche le widget ayant la plus grande surface.
			return AbstractCommandToolbar.GetTarget (this.commandDispatcher, command);
		}

		public static Widget GetTarget(CommandDispatcher commandDispatcher, CommandEventArgs e)
		{
			//	Cherche le widget ayant la plus grande surface.
			return AbstractCommandToolbar.GetTarget (commandDispatcher, e.Command);
		}

		private static Widget GetTarget(CommandDispatcher commandDispatcher, Command command)
		{
			//	Cherche le widget ayant la plus grande surface.
			var targets = commandDispatcher.FindVisuals (command)
				.Where (x => !x.ActualBounds.IsEmpty && x.Name != "NoTarget")
				.OrderByDescending (x => x.PreferredHeight * x.PreferredWidth)
				.ToArray ();

			if (targets.Any ())
			{
				return targets.FirstOrDefault () as Widget;
			}
			else
			{
				return null;
			}
		}


		public virtual void CreateUI(Widget parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.secondaryToolbarHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
				Margins         = new Margins (0, this.RightMargin, 0, 0),
			};

			this.helplineFrame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractCommandToolbar.helplineTriangleHeight + AbstractCommandToolbar.helplineButtonHeight,
				BackColor       = ColorManager.ToolbarBackgroundColor,
				Margins         = new Margins (0, this.RightMargin, 0, 0),
				Visibility      = false,
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

		public void SetHelpLineButton(ButtonWithRedDot button)
		{
			this.helplineTargetButton = button;
			this.MagicLayoutEngine ();
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


		protected ButtonWithRedDot CreateButton(Widget parent, Command command, double widthScale = 1.0)
		{
			//	Crée un bouton pour une commande docké normalement.
			var size = this.toolbar.PreferredHeight;

			return new ButtonWithRedDot
			{
				Parent        = parent,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (size*widthScale, size),
				CommandObject = command,
			};
		}

		protected ButtonWithRedDot CreateButton(DockStyle dock, Command command, double widthScale = 1.0)
		{
			//	Crée un bouton pour une commande docké normalement.
			var size = this.toolbar.PreferredHeight;

			return new ButtonWithRedDot
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = dock,
				PreferredSize = new Size (size*widthScale, size),
				CommandObject = command,
			};
		}

		protected ButtonWithRedDot CreateButton(Command command, int superficiality, bool rightDock = false)
		{
			//	Crée un bouton pour une commande, qui pourra apparaître ou disparaître
			//	selon le choix du "magic layout engine".
			var size = this.toolbar.PreferredHeight;

			var button = new ButtonWithRedDot
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = DockStyle.None,
				PreferredSize = new Size (size, size),
				CommandObject = command,
				Index         = superficiality,
			};

			if (rightDock)
			{
				this.RightDock (button);
			}

			return button;
		}

		protected void CreateSearchController(SearchKind kind, int superficiality, bool rightDock = true)
		{
			//	Crée une zone de recherche, qui pourra apparaître ou disparaître
			//	selon le choix du "magic layout engine".
			var controller = new SearchController (kind);

			var box = new FrameBox
			{
				Parent        = this.toolbar,
				AutoFocus     = false,
				Dock          = DockStyle.None,
				PreferredSize = new Size (200, this.toolbar.PreferredHeight),
				Index         = superficiality,
			};

			if (rightDock)
			{
				this.RightDock (box);
			}

			controller.CreateUI (box);

			controller.Search += delegate (object sender, SearchDefinition definition, int direction)
			{
				this.OnSearch (definition, direction);
			};
		}

		protected void CreateSeparator(int superficiality, bool rightDock = false)
		{
			//	Crée un séparateur sous la forme d'une petite barre verticale, qui
			//	pourra apparaître ou disparaître selon le choix du "magic layout engine".
			var size = this.toolbar.PreferredHeight;
			const int verticalGap = 4;

			var sep = new FrameBox
			{
				Parent        = this.toolbar,
				Dock          = DockStyle.None,
				PreferredSize = new Size (1, size),
				Margins       = new Margins (AbstractCommandToolbar.separatorWidth/2, AbstractCommandToolbar.separatorWidth/2, 0, 0),
				Index         = superficiality,
			};

			if (rightDock)
			{
				this.RightDock (sep);
			}

			//	C'est un peu lourd, mais c'est le seul moyen que j'ai trouvé pour avoir
			//	de petites marges en haut et en bas du séparateur.
			new FrameBox
			{
				Parent          = sep,
				Dock            = DockStyle.Top,
				PreferredHeight = verticalGap,
			};

			new FrameBox
			{
				Parent          = sep,
				Dock            = DockStyle.Top,
				PreferredHeight = size-verticalGap-verticalGap,
				BackColor       = ColorManager.SeparatorColor,
			};
		}

		protected void CreateSajex(int width, int superficiality, bool rightDock = false)
		{
			//	Crée un espace vide, qui pourra apparaître ou disparaître
			//	selon le choix du "magic layout engine".
			var sajex = new FrameBox
			{
				Parent        = this.toolbar,
				Dock          = DockStyle.None,
				PreferredSize = new Size (width, this.toolbar.PreferredHeight),
				Index         = superficiality,
			};

			if (rightDock)
			{
				this.RightDock (sajex);
			}
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

		private void RightDock(Widget widget)
		{
			widget.Name = AbstractCommandToolbar.rightDock;
		}


		#region Magic layout engine
		//	Le "magic layout engine" permet de peupler intelligemment une toolbar, en fonction
		//	de la largeur disponible. Plus la largeur devient petite et plus on supprime des
		//	commandes jugées peu importantes (superficielles). Pour cela, le paramètre superficiality
		//	détermine l’importance, zéro étant les commandes les plus importantes, et une grande
		//	valeur correspondant à des commandes superficielles. Par exemple, si on détermine qu’il
		//	faut utiliser le niveau deux, toutes les commandes de niveau zéro, un et deux seront
		//	présentes. Les commandes de niveau trois et plus seront absentes.
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

			//	Cherche le superficiality maximal permettant de tout afficher dans
			//	la largeur à disposition.
			int superficiality = this.MaxSuperficiality;
			double dispo = this.toolbar.ActualWidth - this.ComputeDockedWidth ();

			while (superficiality >= 0)
			{
				double width = this.ComputeRequiredWidth (superficiality);

				if (width <= dispo)  // largeur toolbar suffisante ?
				{
					//	On positionne les widgets selon leurs largeurs respectives,
					//	de gauche à droite.
					double xLeft = 0;
					double xRight = this.toolbar.ActualWidth;

					foreach (var widget in this.GetWidgets (superficiality))
					{
						if (widget.Name == AbstractCommandToolbar.rightDock)
						{
							xRight -= widget.Margins.Right + widget.PreferredWidth;

							var rect = new Rectangle (xRight, 0, widget.PreferredWidth, widget.PreferredHeight);
							widget.SetManualBounds (rect);

							xRight -= widget.Margins.Left;
						}
						else
						{
							xLeft += widget.Margins.Left;

							var rect = new Rectangle (xLeft, 0, widget.PreferredWidth, widget.PreferredHeight);
							widget.SetManualBounds (rect);

							xLeft += widget.PreferredWidth + widget.Margins.Right;
						}

						if (widget == this.helplineTargetButton)
						{
							this.UpdateHelpline (widget.ActualBounds.Center.X);
						}
					}

					break;
				}
				else
				{
					superficiality--;  // on essaie à nouveau avec moins de widgets
				}
			}
		}

		private void UpdateHelpline(double x)
		{
			//	Met à jour la ligne d'aide en dessous de la toolbar.
			if (this.helplineTextButton == null)
			{
				//	Si le bouton d'aide n'existe pas encore, on le crée une fois pour toutes.
				var color = ColorManager.HelplineColor;

				this.helplineTriangle = new Foreground
				{
					Parent = this.helplineFrame,
					Dock   = DockStyle.None,
				};

				this.helplineTriangle.AddSurface (this.TrianglePath, color);

				this.helplineTextButton = new ColoredButton
				{
					Parent               = this.helplineFrame,
					Name                 = "NoTarget",
					AutoFocus            = false,
					Dock                 = DockStyle.None,
					NormalColor          = color,
					HoverColor           = color,
					SameColorWhenDisable = true,
				};
			}

			if (this.ShowHelpline)  // ligne d'aide visible ?
			{
				this.helplineTextButton.CommandId = this.helplineTargetButton.CommandId;
				int width = this.helplineTextButton.Text.GetTextWidth () + 14;

				//	Petit triangle au dessus du bouton rectangulaire. Il fait office
				//	de queue de la bulle.
				this.helplineTriangle.SetManualBounds (new Rectangle (
					x-AbstractCommandToolbar.helplineTriangleWidth/2,
					AbstractCommandToolbar.helplineButtonHeight,
					AbstractCommandToolbar.helplineTriangleWidth,
					AbstractCommandToolbar.helplineTriangleHeight));

				//	Bouton rectangulaire au dessous. Il fait office de corps de la bulle.
				this.helplineTextButton.SetManualBounds (new Rectangle (
					x-width/2,
					0,
					width,
					AbstractCommandToolbar.helplineButtonHeight));
			}
			else  // ligne d'aide cachée ?
			{
				this.helplineTriangle  .SetManualBounds (Rectangle.Empty);
				this.helplineTextButton.SetManualBounds (Rectangle.Empty);
			}
		}

		private Path TrianglePath
		{
			//	Retourne le chemin du triangle de la bulle d'aide, pointe orientée vers le haut.
			get
			{
				var path = new Path ();

				path.MoveTo (AbstractCommandToolbar.helplineTriangleWidth/2, AbstractCommandToolbar.helplineTriangleHeight);
				path.LineTo (0, 0);
				path.LineTo (AbstractCommandToolbar.helplineTriangleWidth, 0);
				path.Close ();

				return path;
			}
		}

		private int MaxSuperficiality
		{
			//	Retourne le superficiality maximal, donc celui qui permet forcément de voir
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

		private double ComputeRequiredWidth(int superficiality)
		{
			//	Retourne la largeur requise pour un superficiality donné ainsi que pour
			//	tous les superficiality inférieurs.
			return this.GetWidgets (superficiality)
				.Sum (x => x.Margins.Left + x.PreferredWidth + x.Margins.Right);
		}

		private IEnumerable<Widget> GetWidgets(int superficiality)
		{
			//	Retourne tous les widgets que l'on souhaite voir présent pour un superficiality
			//	donné ainsi que pour tous les superficiality inférieurs.
			return this.toolbar.Children.Widgets
				.Where (x => x.Dock == DockStyle.None && x.Index <= superficiality);
		}
		#endregion


		#region Events handler
		private void OnSearch(SearchDefinition definition, int direction)
		{
			this.Search.Raise (this, definition, direction);
		}

		public event EventHandler<SearchDefinition, int> Search;
		#endregion


		public const int primaryToolbarHeight   = 32 + 10;
		public const int secondaryToolbarHeight = 24 + 2;
		public const int separatorWidth         = 11;

		private const int helplineTriangleHeight = 10;
		private const int helplineTriangleWidth  = 12;
		private const int helplineButtonHeight   = 20;

		private const string rightDock = "RightDock";


		protected readonly DataAccessor			accessor;
		protected readonly CommandDispatcher	commandDispatcher;
		protected readonly CommandContext		commandContext;

		protected FrameBox						toolbar;
		protected FrameBox						helplineFrame;
		protected ButtonWithRedDot				helplineTargetButton;
		protected Foreground					helplineTriangle;
		protected ColoredButton					helplineTextButton;
		protected bool							adjustRequired;
	}
}
