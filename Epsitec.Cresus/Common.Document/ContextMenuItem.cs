using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ContextMenuItem représente une case du menu contextuel.
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
		public static void MenuAddItem(System.Collections.ArrayList list, CommandDispatcher cd, string cmd, string icon, string text)
		{
			CommandDispatcher.CommandState state = cd[cmd];
			if ( state != null )
			{
				if ( !state.Enabled )  return;
			}

			ContextMenuItem item = new ContextMenuItem();
			item.Command = cmd;
			item.Name = cmd;
			item.Icon = @icon;
			item.Text = text;
			list.Add(item);
		}

		// Ajoute un sous-menu dans le menu.
		public static void MenuAddSubmenu(System.Collections.ArrayList list, AbstractMenu submenu, string icon, string text)
		{
			ContextMenuItem item = new ContextMenuItem();
			item.submenu = submenu;
			item.Icon = @icon;
			item.Text = text;
			list.Add(item);
		}

		// Ajoute un séparateur dans le menu.
		public static void MenuAddSep(System.Collections.ArrayList list)
		{
			ContextMenuItem item = new ContextMenuItem();
			list.Add(item);  // séparateur
		}

		// Crée le menu.
		public static void MenuCreate(VMenu menu, System.Collections.ArrayList list)
		{
			foreach ( ContextMenuItem cmi in list )
			{
				if ( cmi.submenu != null )
				{
					MenuItem mi = new MenuItem(cmi.Command, cmi.Icon, cmi.Text, "", cmi.Name);
					mi.Submenu = cmi.submenu;
					menu.Items.Add(mi);
				}
				else if ( cmi.Name == "" )
				{
					menu.Items.Add(new MenuSeparator());
				}
				else
				{
					MenuItem mi = new MenuItem(cmi.Command, cmi.Icon, cmi.Text, "", cmi.Name);
					mi.IconNameActiveNo = cmi.IconActiveNo;
					mi.IconNameActiveYes = cmi.IconActiveYes;
					mi.ActiveState = cmi.Active ? WidgetState.ActiveYes : WidgetState.ActiveNo;
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
