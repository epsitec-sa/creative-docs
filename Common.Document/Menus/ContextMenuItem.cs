/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Epsitec.Common.Widgets;
using System.Collections.Generic;

namespace Epsitec.Common.Document.Menus
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
        public static bool MenuAddItem(
            CommandContext context,
            List<ContextMenuItem> list,
            string command
        )
        {
            //	Ajoute une case dans le menu.
            CommandState cs = context.GetCommandState(Common.Widgets.Command.Get(command));
            if (!cs.Enable)
                return false;

            ContextMenuItem item = new ContextMenuItem();
            item.Command = cs.Command.CommandId;
            item.Name = cs.Command.CommandId;
            item.Icon = cs.Command.Icon;
            item.Text = cs.Command.Description; // texte sans le raccourci clavier entre parenthèses
            list.Add(item);
            return true;
        }

        public static bool MenuAddSubmenu(
            List<ContextMenuItem> list,
            AbstractMenu submenu,
            string icon,
            string text
        )
        {
            //	Ajoute un sous-menu dans le menu.
            if (submenu == null)
                return false;

            ContextMenuItem item = new ContextMenuItem();
            item.submenu = submenu;
            item.Icon = @icon;
            item.Text = text;
            list.Add(item);
            return true;
        }

        public static void MenuAddSep(List<ContextMenuItem> list)
        {
            //	Ajoute un séparateur dans le menu.
            ContextMenuItem item = new ContextMenuItem();
            list.Add(item); // séparateur
        }

        public static bool IsMenuActive(List<ContextMenuItem> list)
        {
            //	Indique si un futur menu sera actif.
            foreach (ContextMenuItem cmi in list)
            {
                if (cmi.submenu != null) // sous-menu ?
                {
                    return true;
                }
                else if (cmi.Name == "") // séparateur ?
                { }
                else // case normale ?
                {
                    return true;
                }
            }
            return false;
        }

        public static void MenuCreate(VMenu menu, List<ContextMenuItem> list)
        {
            //	Crée le menu.
            foreach (ContextMenuItem cmi in list)
            {
                if (cmi.submenu != null) // sous-menu ?
                {
                    MenuItem mi = new MenuItem(cmi.Command, cmi.Icon, cmi.Text, "", cmi.Name);
                    mi.Submenu = cmi.submenu;
                    menu.Items.Add(mi);
                }
                else if (cmi.Name == "") // séparateur ?
                {
                    menu.Items.Add(new MenuSeparator());
                }
                else // case normale ?
                {
                    MenuItem mi = new MenuItem(cmi.Command, cmi.Icon, cmi.Text, "", cmi.Name);
                    mi.IconUriActiveNo = cmi.IconActiveNo;
                    mi.IconUriActiveYes = cmi.IconActiveYes;
                    mi.ActiveState = cmi.Active ? ActiveState.Yes : ActiveState.No;
                    menu.Items.Add(mi);
                }
            }
        }
        #endregion


        protected string command;
        protected string name;
        protected string icon;
        protected string iconActiveNo;
        protected string iconActiveYes;
        protected string text;
        protected bool active;
        protected AbstractMenu submenu;
    }
}
