using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Abstract est la classe de base pour toutes les sections de rubans.
	/// </summary>
	public abstract class AbstractRibbon : RibbonSection
	{
		public AbstractRibbon(DesignerApplication designerApplication)
		{
			this.designerApplication = designerApplication;
		}


		public IconButton SearchIconButton(string command)
		{
			//	Cherche le bouton utilisé pour une commande, dans le ruban.
			Command c = Common.Widgets.Command.Get(command);
			string icon = c.Icon;

			foreach (Widget widget in this.Children)
			{
				IconButton button = widget as IconButton;
				if (button != null)
				{
					if (button.IconUri == icon)
					{
						return button;
					}
				}
			}

			return null;
		}

		protected IconButton CreateIconButton(string command)
		{
			//	Crée un bouton pour une commande.
			return this.CreateIconButton(command, "Normal");
		}

		protected IconButton CreateIconButton(string command, string iconSize)
		{
			//	Crée un bouton pour une commande, en précisant la taille préférée pour l'icône.
			Command c = Widgets.Command.Get(command);
			IconButton button = new IconButton(this);

#if false
			button.CommandLine = command;
			button.IconName = Misc.Icon(c.Icon);
			button.PreferredIconSize = Misc.IconPreferredSize(iconSize);
			button.AutoFocus = false;

			if ( c.Statefull )
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
			}
#else
			button.CommandObject = c;
//-			button.IconName = Misc.Icon(c.Icon);
			button.PreferredIconSize = Misc.IconPreferredSize (iconSize);
			button.AutoFocus = false;
#endif

			button.TabIndex = this.tabIndex++;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(c));
			return button;
		}

		protected IconButtonCombo CreateIconButtonCombo(string command)
		{
			//	Crée un bouton combo pour une commande.
			Command c = Widgets.Command.Get(command);
			IconButtonCombo button = new IconButtonCombo(this);

			button.CommandObject = c;
			button.AutoFocus = false;
			button.IsLiveUpdateEnabled = false;

			button.TabIndex = this.tabIndex++;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(c));
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
			combo.IconButton.IconUri = briefIcon;
		}

		protected IconButtonsCombo CreateIconButtonsCombo(string command)
		{
			//	Crée un bouton combo pour une commande.
			Command c = Widgets.Command.Get(command);
			IconButtonsCombo button = new IconButtonsCombo(this);

			button.CommandObject = c;
			button.AutoFocus = false;
			button.IsLiveUpdateEnabled = false;

			button.TabIndex = this.tabIndex++;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(button, Misc.GetTextWithShortcut(c));
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

		protected GlyphButton CreateMenuButton(string command, string tooltip, Support.EventHandler<MessageEventArgs> handler)
		{
			//	Crée un bouton "v" pour un menu.
			GlyphButton button = new GlyphButton(this);
			button.CommandObject = Command.Get(command);
			button.ButtonStyle = ButtonStyle.ToolItem;
			button.GlyphShape = GlyphShape.Menu;
			button.AutoFocus = false;
			button.Pressed += handler;
			button.TabIndex = this.tabIndex++;
			button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
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


		protected DesignerApplication		designerApplication;
		protected int						tabIndex = 1;
		protected bool						ignoreChange = false;
		protected double					separatorWidth = 8;
	}
}
