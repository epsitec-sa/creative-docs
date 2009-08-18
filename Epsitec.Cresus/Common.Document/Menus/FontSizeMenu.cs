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

			if ( double.IsNaN(this.value) )
			{
				this.name = "";
				this.text = Res.Strings.Panel.Font.None;
			}
			else
			{
				if ( this.units == "%" )
				{
					this.name = string.Format("{0}{1}", this.value.ToString(System.Globalization.CultureInfo.InvariantCulture), this.units);
					
					value = this.value*100;
					value *= 1000000.0;
					value = System.Math.Floor(value+0.5);  // arrondi à la 6ème décimale
					value /= 1000000.0;
					this.text = string.Format("{0}{1}", value.ToString(), this.units);
				}
				else
				{
					this.name = string.Format("{0}", this.value.ToString(System.Globalization.CultureInfo.InvariantCulture));
					
					value = this.value/Modifier.FontSizeScale;
					value *= 1000000.0;
					value = System.Math.Floor(value+0.5);  // arrondi à la 6ème décimale
					value /= 1000000.0;
					this.text = string.Format("{0}", value.ToString());
				}
			}
		}

		public int CompareTo(object obj)
		{
			//	Défini par System.IComparable.
			FontSizeMenu that = obj as FontSizeMenu;

			if ( double.IsNaN(this.value) &&
				 double.IsNaN(that.value) )  return 0;

			if ( double.IsNaN(this.value) )  return -1;  // "Aucun" toujours au début
			if ( double.IsNaN(that.value) )  return  1;

			if ( this.units != that.units )
			{
				if ( this.units == "%" )  return -1;  // les % toujours au début
				if ( that.units == "%" )  return  1;
			}

			if ( System.Math.Abs(this.value-that.value) < 0.00001 )  return 0;
			return this.value.CompareTo(that.value);
		}


		protected static void Add(System.Collections.ArrayList list, double value, string units)
		{
			//	Ajoute une entrée dans une liste si elle n'existe pas encore.
			FontSizeMenu fs = new FontSizeMenu(value, units);

			foreach ( FontSizeMenu current in list )
			{
				if ( current.CompareTo(fs) == 0 )  return;  // n'ajoute pas si existe déjà
			}

			list.Add(fs);
		}

		public static VMenu CreateFontSizeMenu(double currentValue, string currentUnits, double factor, bool isPercent, bool isDefault, Support.EventHandler<MessageEventArgs> message)
		{
			//	Construit le menu pour choisir une taille.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			FontSizeMenu current = new FontSizeMenu(currentValue, currentUnits);

			FontSizeMenu.Add(list, currentValue, currentUnits);

			if ( isDefault )
			{
				FontSizeMenu.Add(list, double.NaN, "");
			}

			if ( isPercent )
			{
				FontSizeMenu.Add(list, 0.50, "%");
				FontSizeMenu.Add(list, 0.75, "%");
				FontSizeMenu.Add(list, 1.00, "%");
				FontSizeMenu.Add(list, 1.50, "%");
				FontSizeMenu.Add(list, 2.00, "%");
				FontSizeMenu.Add(list, 3.00, "%");
			}

			FontSizeMenu.Add(list,   8*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,   9*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  10*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  11*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  12*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  14*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  16*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  20*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  26*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  36*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  48*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  72*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list,  96*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list, 120*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list, 144*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list, 192*Modifier.FontSizeScale*factor, "");
			FontSizeMenu.Add(list, 240*Modifier.FontSizeScale*factor, "");

			list.Sort();

			VMenu menu = new VMenu();
			string lastUnits = isDefault ? "" : (isPercent ? "%" : "");
			foreach ( FontSizeMenu fs in list )
			{
				if ( lastUnits != fs.units )
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
