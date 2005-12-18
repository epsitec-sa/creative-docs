using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Menus
{
	/// <summary>
	/// La classe ContextMenuItem repr�sente une case du menu contextuel.
	/// </summary>
	public class ContextMenuItem
	{
		public ContextMenuItem()
		{
			this.command = "";
			this.name = "";
			this.icon = "";
			this.iconActiveNo = "";
			this.iconActiveYes = "";
			this.text = "";
			this.active = false;
			this.submenu = null;
		}

		public string Command
		{
			get { return this.command; }
			set { this.command = value; }
		}

		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		public string Icon
		{
			get { return this.icon; }
			set { this.icon = value; }
		}

		public string IconActiveNo
		{
			get { return this.iconActiveNo; }
			set { this.iconActiveNo = value; }
		}

		public string IconActiveYes
		{
			get { return this.iconActiveYes; }
			set { this.iconActiveYes = value; }
		}

		public string Text
		{
			get { return this.text; }
			set { this.text = value; }
		}

		public bool Active
		{
			get { return this.active; }
			set { this.active = value; }
		}

		public AbstractMenu Submenu
		{
			get { return this.submenu; }
			set { this.submenu = value; }
		}


		#region StaticMethods
		// Ajoute une case dans le menu.
		public static bool MenuAddItem(System.Collections.ArrayList list, string command)
		{
			CommandState cs = CommandDispatcher.GetFocusedPrimaryDispatcher().GetCommandState(command);
			if ( !cs.Enable )  return false;

			ContextMenuItem item = new ContextMenuItem();
			item.Command = cs.Name;
			item.Name = cs.Name;
			item.Icon = Misc.Icon(cs.IconName);
			item.Text = cs.LongCaption;  // texte sans le raccourci clavier entre parenth�ses
			list.Add(item);
			return true;
		}

		// Ajoute un sous-menu dans le menu.
		public static bool MenuAddSubmenu(System.Collections.ArrayList list, AbstractMenu submenu, string icon, string text)
		{
			if ( submenu == null )  return false;

			ContextMenuItem item = new ContextMenuItem();
			item.submenu = submenu;
			item.Icon = @icon;
			item.Text = text;
			list.Add(item);
			return true;
		}

		// Ajoute un s�parateur dans le menu.
		public static void MenuAddSep(System.Collections.ArrayList list)
		{
			ContextMenuItem item = new ContextMenuItem();
			list.Add(item);  // s�parateur
		}

		// Indique si un futur menu sera actif.
		public static bool IsMenuActive(System.Collections.ArrayList list)
		{
			foreach ( ContextMenuItem cmi in list )
			{
				if ( cmi.submenu != null )  // sous-menu ?
				{
					return true;
				}
				else if ( cmi.Name == "" )  // s�parateur ?
				{
				}
				else	// case normale ?
				{
					return true;
				}
			}
			return false;
		}

		// Cr�e le menu.
		public static void MenuCreate(VMenu menu, System.Collections.ArrayList list)
		{
			foreach ( ContextMenuItem cmi in list )
			{
				if ( cmi.submenu != null )  // sous-menu ?
				{
					MenuItem mi = new MenuItem(cmi.Command, cmi.Icon, cmi.Text, "", cmi.Name);
					mi.Submenu = cmi.submenu;
					menu.Items.Add(mi);
				}
				else if ( cmi.Name == "" )  // s�parateur ?
				{
					menu.Items.Add(new MenuSeparator());
				}
				else	// case normale ?
				{
					MenuItem mi = new MenuItem(cmi.Command, cmi.Icon, cmi.Text, "", cmi.Name);
					mi.IconNameActiveNo = cmi.IconActiveNo;
					mi.IconNameActiveYes = cmi.IconActiveYes;
					mi.ActiveState = cmi.Active ? ActiveState.Yes : ActiveState.No;
					menu.Items.Add(mi);
				}
			}
		}
		#endregion


		protected string			command;
		protected string			name;
		protected string			icon;
		protected string			iconActiveNo;
		protected string			iconActiveYes;
		protected string			text;
		protected bool				active;
		protected AbstractMenu		submenu;
	}
}
