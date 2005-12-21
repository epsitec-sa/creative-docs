using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Menus
{
	public class FontSizeMenu : System.IComparable
	{
		public FontSizeMenu(double value, string units)
		{
			//	Constructeur unique.
			this.value = value;
			this.units = units;

			if ( this.units == "%" )
			{
				this.name = string.Format("{0}{1}", this.value.ToString(System.Globalization.CultureInfo.InvariantCulture), this.units);
				this.text = string.Format("{0}{1}", (this.value*100).ToString(System.Globalization.CultureInfo.CurrentUICulture), this.units);
			}
			else
			{
				this.name = string.Format("{0}", this.value.ToString(System.Globalization.CultureInfo.InvariantCulture));
				this.text = string.Format("{0}", (this.value/Modifier.fontSizeScale).ToString(System.Globalization.CultureInfo.CurrentUICulture));
			}
		}

		public int CompareTo(object obj)
		{
			//	D�fini par System.IComparable.
			FontSizeMenu that = obj as FontSizeMenu;

			if ( this.units != that.units )
			{
				if ( this.units == "%" )  return -1;  // les % toujours au d�but
				if ( that.units == "%" )  return  1;
			}

			if ( System.Math.Abs(this.value-that.value) < 0.00001 )  return 0;
			return this.value.CompareTo(that.value);
		}


		protected static void Add(System.Collections.ArrayList list, double value, string units)
		{
			//	Ajoute une entr�e dans une liste si elle n'existe pas encore.
			FontSizeMenu fs = new FontSizeMenu(value, units);

			foreach ( FontSizeMenu current in list )
			{
				if ( current.CompareTo(fs) == 0 )  return;  // n'ajoute pas si existe d�j�
			}

			list.Add(fs);
		}

		public static VMenu CreateFontSizeMenu(double currentValue, string currentUnits, MessageEventHandler message)
		{
			//	Construit le menu pour choisir une taille.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			FontSizeMenu current = new FontSizeMenu(currentValue, currentUnits);

			FontSizeMenu.Add(list, currentValue, currentUnits);

			FontSizeMenu.Add(list, 0.50, "%");
			FontSizeMenu.Add(list, 0.75, "%");
			FontSizeMenu.Add(list, 1.00, "%");
			FontSizeMenu.Add(list, 1.50, "%");
			FontSizeMenu.Add(list, 2.00, "%");
			FontSizeMenu.Add(list, 3.00, "%");

			FontSizeMenu.Add(list,  8*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list,  9*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 10*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 11*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 12*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 14*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 16*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 20*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 26*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 36*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 48*Modifier.fontSizeScale, "");
			FontSizeMenu.Add(list, 72*Modifier.fontSizeScale, "");

			list.Sort();

			VMenu menu = new VMenu();
			string lastUnits = "%";
			foreach ( FontSizeMenu fs in list )
			{
				if ( lastUnits == "%" && fs.units == "" )
				{
					menu.Items.Add(new MenuSeparator());
				}

				string text = fs.text;

				string icon = Misc.Icon("RadioNo");
				if ( fs.CompareTo(current) == 0 )
				{
					icon = Misc.Icon("RadioYes");
					text = Misc.Bold(text);
				}

				MenuItem item = new MenuItem("", icon, text, "", fs.name);

				if ( message != null )
				{
					item.Pressed += message;
				}

				menu.Items.Add(item);

				lastUnits = fs.units;
			}
			menu.AdjustSize();
			return menu;
		}


		protected double		value;
		protected string		units;
		protected string		name;
		protected string		text;
	}
}
