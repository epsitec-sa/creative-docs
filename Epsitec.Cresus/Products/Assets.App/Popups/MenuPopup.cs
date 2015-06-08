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

			this.commands = new List<Command> ();
		}


		private void AddItem(Command command)
		{
			if (command == null)  // séparateur ?
			{
				this.commands.Add (null);
			}
			else
			{
				if (this.toolbar.GetEnable (command))
				{
					this.commands.Add (command);
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

				if (c0 == null &&
					c1 == null)  // 2 séparateurs qui se suivent ?
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
				if (this.commands.First () == null)  // séparateur ?
				{
					this.commands.RemoveAt (0);
				}
			}

			//	Si le menu se termine par un séparateur, supprime-le.
			if (this.commands.Any ())
			{
				if (this.commands.Last () == null)  // séparateur ?
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

		protected override void CreateUI()
		{
			this.commands.ForEach (command => this.CreateLine (command));
			this.CreateInvisibleCloseButton ();
		}

		private void CreateLine(Command command)
		{
			//	Crée une ligne du menu, de haut en bas.
			if (command == null)  // séparateur ?
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

		private void CreateItem(Command command)
		{
			//	Crée une ligne contenant un item (icône suivie d'un texte).
#if true
			bool top = this.mainFrameBox.Children.Count == 0;  // première ligne ?
			var desc = command.Description;
			
			var item = new MenuPopupItem
			{
				Parent          = this.mainFrameBox,
				IconUri         = Misc.GetResourceIconUri (command.Icon),
				Text            = desc,
				Dock            = DockStyle.Top,
				PreferredHeight = this.itemHeight,
				Margins         = new Margins (MenuPopup.margins, MenuPopup.margins, top ? MenuPopup.margins : 0, 0),
			};

			item.Clicked += delegate
			{
				this.mainFrameBox.ExecuteCommand (command);

				//	On ferme le popup plus tard, une fois que tout le reste aura été exécuté...
				Application.QueueAsyncCallback (() => this.ClosePopup ());
			};
#else
			bool top = this.mainFrameBox.Children.Count == 0;  // première ligne ?

			new MenuItem
			{
				Parent          = this.mainFrameBox,
				Dock            = DockStyle.Top,
				PreferredHeight = this.itemHeight,
				Margins         = new Margins (MenuPopup.margins, MenuPopup.margins, top ? MenuPopup.margins : 0, 0),
				CommandObject   = command,
			};
#endif
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


		private int RequiredWidth
		{
			//	Calcule la largeur nécessaire en fonction de l'ensemble des cases du menu.
			get
			{
				return this.commands.Max
				(
					command => (command == null) ? 0 : MenuPopupItem.GetRequiredWidth
					(
						this.itemHeight, command.Description
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
					command => this.GetRequiredHeight (command)
				)
				+ MenuPopup.margins*2;
			}
		}

		private int GetRequiredHeight(Command command)
		{
			if (command == null)  // séparateur ?
			{
				return MenuPopup.margins + 1 + MenuPopup.margins;
			}
			else
			{
				return this.itemHeight;
			}
		}


		#region Helpers
		public static void Show(AbstractCommandToolbar toolbar, Widget widget, Point pos, int? itemHeight, params Command[] commands)
		{
			//	Affiche le menu contextuel.
			//	- La toolbar permet d'obtenir les commandes.
			//	- Le widget est quelconque; il sert juste à retrouver la fenêtre.
			//	- La position indique le point cliqué avec le bouton de droite de la souris,
			//	  dans l'espace 'Screen'.
			//	- La liste d'items décrit le contenu du menu.

			var popup = new MenuPopup (toolbar);

			if (itemHeight.HasValue)
			{
				popup.itemHeight = itemHeight.Value;
			}

			foreach (var command in commands)
			{

				popup.AddItem (command);
			}

			popup.Simplify ();  // supprime les séparateurs superflus

			if (popup.commands.Any ())
			{
				popup.Create (widget, pos, leftOrRight: false);
			}
		}
		#endregion


		private const int							margins		= 2;

		private readonly AbstractCommandToolbar		toolbar;
		private readonly List<Command>				commands;

		private int									itemHeight	= 26;
	}
}