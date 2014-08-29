//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Menu composé d'une icône suivie d'un texte. Ce composant est utilisé pour les
	/// menus contextuels actionnés avec le bouton de droite de la souris.
	/// </summary>
	public class MenuPopup : AbstractPopup
	{
		private MenuPopup(AbstractCommandToolbar toolbar)
		{
			this.toolbar = toolbar;

			this.commands = new List<ToolbarCommand> ();
			this.actions = new Dictionary<ToolbarCommand, System.Action> ();
		}


		private void AddItem(ToolbarCommand command, System.Action action)
		{
			if (action == null)  // séparateur ?
			{
				this.commands.Add (ToolbarCommand.Unknown);
			}
			else
			{
				var state = this.toolbar.GetCommandState (command);

				if (state == ToolbarCommandState.Enable)
				{
					this.commands.Add (command);
					this.actions.Add (command, action);
				}
			}
		}


		private void Simplify()
		{
			//	Si le menu contient des sections vides, on supprime les séparateurs
			//	superflus.
			int i = 0;
			while (i < this.commands.Count-1)
			{
				var c0 = this.commands[i];
				var c1 = this.commands[i+1];

				if (c0 == ToolbarCommand.Unknown &&
					c1 == ToolbarCommand.Unknown)  // 2 séparateurs qui se suivent ?
				{
					this.commands.RemoveAt (i);
				}
				else
				{
					i++;
				}
			}

			//	Si le menu commence par un séparateur, supprime-le.
			if (this.commands.Any ())
			{
				if (this.commands.First () == ToolbarCommand.Unknown)  // séparateur ?
				{
					this.commands.RemoveAt (0);
				}
			}

			//	Si le menu se termine par un séparateur, supprime-le.
			if (this.commands.Any ())
			{
				if (this.commands.Last () == ToolbarCommand.Unknown)  // séparateur ?
				{
					this.commands.RemoveAt (this.commands.Count-1);
				}
			}
		}


		protected override Size DialogSize
		{
			get
			{
				return new Size (this.RequiredWidth, this.RequiredHeight);
			}
		}

		public override void CreateUI()
		{
			this.commands.ForEach (command => this.CreateLine (command));
			this.CreateInvisibleCloseButton ();
		}

		private void CreateLine(ToolbarCommand command)
		{
			//	Crée une ligne du menu, de haut en bas.
			if (command == ToolbarCommand.Unknown)  // séparateur ?
			{
				this.CreateSeparator ();
			}
			else
			{
				this.CreateItem (command);
			}
		}

		private void CreateSeparator()
		{
			//	Crée une ligne contenant un trait horizontal de séparation.
			new FrameBox
			{
				Parent          = this.mainFrameBox,
				Dock            = DockStyle.Top,
				PreferredHeight = 1,  // trait horizontal d'un pixel d'épaisseur
				BackColor       = ColorManager.PopupBorderColor,
				Margins         = new Margins (0, 0, MenuPopup.margins, MenuPopup.margins),
			};
		}

		private void CreateItem(ToolbarCommand command)
		{
			//	Crée une ligne contenant un item (icône suivie d'un texte).
			bool top = this.mainFrameBox.Children.Count == 0;  // première ligne ?
			var desc = this.toolbar.GetCommandDescription (command);

			var item = new MenuPopupItem
			{
				Parent          = this.mainFrameBox,
				IconUri         = Misc.GetResourceIconUri (desc.Icon),
				Text            = desc.Tooltip,
				Dock            = DockStyle.Top,
				PreferredHeight = MenuPopup.itemHeight,
				Margins         = new Margins (MenuPopup.margins, MenuPopup.margins, top ? MenuPopup.margins : 0, 0),
			};

			item.Clicked += delegate
			{
				this.DoAction (command);
			};
		}

		private void CreateInvisibleCloseButton()
		{
			//	Crée un bouton de fermeture invisible. Il permet de fermer le menu lorsque
			//	les touches Esc ou Return sont pressées.
			var button = new IconButton
			{
				Parent        = this.mainFrameBox,
				AutoFocus     = false,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = Size.Zero,
			};

			button.Shortcuts.Add (Epsitec.Common.Widgets.Feel.Factory.Active.CancelShortcut);
			button.Shortcuts.Add (Epsitec.Common.Widgets.Feel.Factory.Active.AcceptShortcut);

			button.Clicked += delegate
			{
				this.ClosePopup ();
			};
		}

		private void DoAction(ToolbarCommand command)
		{
			//	Effectue l'action correspondant à une commande.
			this.ClosePopup ();
			this.actions[command] ();  // effectue l'action
		}


		private int RequiredWidth
		{
			//	Calcule la largeur nécessaire en fonction de l'ensemble des cases du menu.
			get
			{
				return this.commands.Max
				(
					command => MenuPopupItem.GetRequiredWidth
					(
						MenuPopup.itemHeight, this.toolbar.GetCommandDescription (command).Tooltip
					)
				)
				+ MenuPopup.margins*2;
			}
		}

		private int RequiredHeight
		{
			//	Calcule la hauteur nécessaire en fonction de l'ensemble des cases du menu.
			get
			{
				return this.commands.Sum
				(
					command => MenuPopup.GetRequiredHeight (command)
				)
				+ MenuPopup.margins*2;
			}
		}

		private static int GetRequiredHeight(ToolbarCommand command)
		{
			if (command == ToolbarCommand.Unknown)  // séparateur ?
			{
				return MenuPopup.margins + 1 + MenuPopup.margins;
			}
			else
			{
				return MenuPopup.itemHeight;
			}
		}


		#region Helpers
		public static void Show(AbstractCommandToolbar toolbar, Widget widget, Point pos, params Item[] items)
		{
			//	Affiche le menu contextuel.
			//	- La toolbar permet d'obtenir les commandes.
			//	- Le widget est quelconque; il sert juste à retrouver la fenêtre.
			//	- La position indique le point cliqué avec le bouton de droite de la souris,
			//	  dans l'espace 'Screen'.
			//	- La liste d'items décrit le contenu du menu.

			var popup = new MenuPopup (toolbar);

			foreach (var item in items)
			{

				popup.AddItem (item.Command, item.Action);
			}

			popup.Simplify ();  // supprime les séparateurs superflus

			if (popup.commands.Any ())
			{
				popup.Create (widget, pos, leftOrRight: false);
			}
		}

		public struct Item
		{
			public Item(ToolbarCommand command, System.Action action)
			{
				this.Command = command;
				this.Action  = action;
			}

			public readonly ToolbarCommand		Command;
			public readonly System.Action		Action;
		}
		#endregion


		private const int							margins		= 2;
		private const int							itemHeight	= 26;

		private readonly AbstractCommandToolbar		toolbar;
		private readonly List<ToolbarCommand>		commands;
		private readonly Dictionary<ToolbarCommand, System.Action> actions;
	}
}