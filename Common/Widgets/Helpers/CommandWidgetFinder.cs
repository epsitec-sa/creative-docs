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


using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Epsitec.Common.Widgets.Helpers
{
    public class CommandWidgetFinder
    {
        public CommandWidgetFinder() { }

        public CommandWidgetFinder(string filter)
        {
            this.filter = filter;
        }

        public CommandWidgetFinder(Command command)
        {
            this.command = command;
        }

        public CommandWidgetFinder(Regex regex)
        {
            this.regex = regex;
        }

        public Widget[] Widgets
        {
            get { return this.list.ToArray(); }
        }

        public bool Analyze(Widget widget)
        {
            if (widget.HasCommand)
            {
                if (this.regex == null)
                {
                    if (this.command == null)
                    {
                        if (this.filter == null)
                        {
                            this.list.Add(widget);
                        }
                        else if (this.filter == widget.CommandName)
                        {
                            this.list.Add(widget);
                        }
                    }
                    else if (this.command == widget.CommandObject)
                    {
                        this.list.Add(widget);
                    }
                }
                else
                {
                    //	Une expression régulière a été définie pour filtrer les widgets en
                    //	fonction de leur nom. On applique cette expression pour voir si le
                    //	nom de la commande est conforme...

                    Match match = this.regex.Match(widget.CommandName);

                    //	...en cas de succès, on prend note du widget, sinon on passe simplement
                    //	au suivant.

                    if (match.Success)
                    {
                        this.list.Add(widget);
                    }
                }
            }

            return true;
        }

        private readonly List<Widget> list = new List<Widget>();
        private Regex regex;
        private string filter;
        private Command command;
    }
}
