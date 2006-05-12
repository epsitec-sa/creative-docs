using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Abstract est la classe de base pour toutes les sections de rubans.
	/// </summary>
	public abstract class Abstract : RibbonSection
	{
		public Abstract()
		{
		}

		
		protected IconButton CreateIconButton(string command)
		{
			//	Crée un bouton pour une commande.
			return this.CreateIconButton(command, "Normal");
		}

		protected IconButton CreateIconButton(string command, string iconSize)
		{
			//	Crée un bouton pour une commande, en précisant la taille préférée pour l'icône.
			CommandState cs = CommandState.Get (command);
			IconButton button = new IconButton(this);

			button.Command = command;
			button.IconName = Misc.Icon(cs.IconName);
			button.PreferredIconSize = Misc.IconPreferredSize(iconSize);
			button.AutoFocus = false;

			if ( cs.Statefull )
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
			}

			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(cs));
			return button;
		}

		protected IconButtonCombo CreateIconButtonCombo(string command)
		{
			//	Crée un bouton combo pour une commande.
			CommandState cs = CommandState.Get (command);
			IconButtonCombo button = new IconButtonCombo(this);

			button.Command = command;
			button.AutoFocus = false;
			button.IsLiveUpdateEnabled = false;

			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(cs));
			return button;
		}

		protected void AddIconButtonCombo(IconButtonCombo combo, string name, string icon, string text)
		{
			//	Ajoute une ligne au menu d'un IconButtonCombo.
			string regularText  = string.Format("{0} {1}", Misc.Image(icon, -5), text);
			string selectedText = string.Format("{0} {1}", Misc.Image(icon, -5), Misc.Bold(text));
			string briefIcon    = Misc.Icon(icon);
			IconButtonCombo.Item item = new IconButtonCombo.Item(name, briefIcon, regularText, selectedText);
			combo.Items.Add(item);
		}

		protected void AddIconButtonComboDyn(IconButtonCombo combo, string name, string briefIcon, string menuIcon, string parameter)
		{
			//	Ajoute une ligne au menu d'un IconButtonCombo.
			string regularText  = Misc.ImageDyn(menuIcon, parameter);
			string selectedText = Misc.ImageDyn(menuIcon, parameter);
			briefIcon           = Misc.IconDyn(briefIcon, parameter);
			IconButtonCombo.Item item = new IconButtonCombo.Item(name, briefIcon, regularText, selectedText);
			combo.Items.Add(item);
		}

		protected void BriefIconButtonComboDyn(IconButtonCombo combo, string briefIcon, string parameter)
		{
			//	Spécifie le contenu (au repos, càd menu fermé) d'un IconButtonCombo.
			briefIcon = Misc.IconDyn(briefIcon, parameter);
			combo.IconButton.IconName = briefIcon;
		}

		protected IconButtonsCombo CreateIconButtonsCombo(string command)
		{
			//	Crée un bouton combo pour une commande.
			CommandState cs = CommandState.Get (command);
			IconButtonsCombo button = new IconButtonsCombo(this);

			button.Command = command;
			button.AutoFocus = false;
			button.IsLiveUpdateEnabled = false;

			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(cs));
			return button;
		}

		protected void AddIconButtonsComboDyn(IconButtonsCombo combo, string name, string briefIcon, string menuIcon, string parameter, string tooltip)
		{
			//	Ajoute une ligne au menu d'un IconButtonsCombo.
			string regularText  = Misc.ImageDyn(menuIcon, parameter);
			string selectedText = Misc.ImageDyn(menuIcon, parameter);
			briefIcon           = Misc.IconDyn(briefIcon, parameter);
			IconButtonsCombo.Item item = new IconButtonsCombo.Item(name, briefIcon, regularText, selectedText, tooltip);
			combo.Items.Add(item);
		}

		protected GlyphButton CreateMenuButton(string command, string tooltip, MessageEventHandler handler)
		{
			//	Crée un bouton "v" pour un menu.
			GlyphButton button = new GlyphButton(this);
			button.Command = command;
			button.ButtonStyle = ButtonStyle.ToolItem;
			button.GlyphShape = GlyphShape.Menu;
			button.AutoFocus = false;
			button.Pressed += handler;
			button.TabIndex = this.tabIndex++;
			button.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, tooltip);
			return button;
		}


		protected void MenuAdd(VMenu vmenu, string icon, string command, string text, string shortcut)
		{
			//	Ajoute une icône.
			this.MenuAdd(vmenu, icon, command, text, shortcut, command);
		}
		
		protected void MenuAdd(VMenu vmenu, string icon, string command, string text, string shortcut, string name)
		{
			if ( text == "" )
			{
				vmenu.Items.Add(new MenuSeparator());
			}
			else
			{
				MenuItem item;
				
				if ( icon == "y/n" )
				{
					item = MenuItem.CreateYesNo(command, text, shortcut, name);
				}
				else
				{
					item = new MenuItem(command, icon, text, shortcut, name);
				}
				
				vmenu.Items.Add(item);
			}
		}


		protected int						tabIndex = 0;
		protected bool						ignoreChange = false;
		protected double					separatorWidth = 8;
	}
}
