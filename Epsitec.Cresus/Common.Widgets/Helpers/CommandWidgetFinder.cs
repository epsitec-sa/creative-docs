//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Epsitec.Common.Widgets.Helpers
{
	public class CommandWidgetFinder
	{
		public CommandWidgetFinder()
		{
		}

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
			get
			{
				return this.list.ToArray ();
			}
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
							this.list.Add (widget);
						}
						else if (this.filter == widget.CommandName)
						{
							this.list.Add (widget);
						}
					}
					else if (this.command == widget.CommandObject)
					{
						this.list.Add (widget);
					}
				}
				else
				{
					//	Une expression régulière a été définie pour filtrer les widgets en
					//	fonction de leur nom. On applique cette expression pour voir si le
					//	nom de la commande est conforme...

					Match match = this.regex.Match (widget.CommandName);

					//	...en cas de succès, on prend note du widget, sinon on passe simplement
					//	au suivant.

					if (match.Success)
					{
						this.list.Add (widget);
					}
				}
			}

			return true;
		}



		private readonly List<Widget>			list = new List<Widget> ();
		private Regex							regex;
		private string							filter;
		private Command							command;
	}
}
