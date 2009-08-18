using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Menus
{
	public class LeadingMenu : System.IComparable
	{
		public LeadingMenu(Document document, double value, string units)
		{
			//	Constructeur unique.
			value *= 1000000.0;
			value = System.Math.Floor(value+0.5);  // arrondi à la 6ème décimale
			value /= 1000000.0;
			this.value = value;
			this.units = units;

			if ( double.IsNaN(this.value) )
			{
				this.name = "";
				this.text = Res.Strings.TextPanel.Leading.None;
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
					
					this.text = string.Format("{0} {1}", document.Modifier.RealToString(this.value), document.Modifier.ShortNameUnitDimension);
				}
			}
		}

		public int CompareTo(object obj)
		{
			//	Défini par System.IComparable.
			LeadingMenu that = obj as LeadingMenu;

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


		protected static void Add(Document document, System.Collections.ArrayList list, double value, string units)
		{
			//	Ajoute une entrée dans une liste si elle n'existe pas encore.
			LeadingMenu fs = new LeadingMenu(document, value, units);

			foreach ( LeadingMenu current in list )
			{
				if ( current.CompareTo(fs) == 0 )  return;  // n'ajoute pas si existe déjà
			}

			list.Add(fs);
		}

		public static VMenu CreateLeadingMenu(Document document, double currentValue, string currentUnits, bool isDefault, Support.EventHandler<MessageEventArgs> message)
		{
			//	Construit le menu pour choisir un interligne.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			LeadingMenu current = new LeadingMenu(document, currentValue, currentUnits);

			LeadingMenu.Add(document, list, currentValue, currentUnits);

			if ( isDefault )
			{
				LeadingMenu.Add(document, list, double.NaN, "");
			}

			LeadingMenu.Add(document, list, 0.5, "%");
			LeadingMenu.Add(document, list, 0.8, "%");
			LeadingMenu.Add(document, list, 1.0, "%");
			LeadingMenu.Add(document, list, 1.5, "%");
			LeadingMenu.Add(document, list, 2.0, "%");
			LeadingMenu.Add(document, list, 3.0, "%");

			if ( document.Modifier.RealUnitDimension == RealUnitType.DimensionInch )
			{
				LeadingMenu.Add(document, list, 0.20*254, "");
				LeadingMenu.Add(document, list, 0.25*254, "");
				LeadingMenu.Add(document, list, 0.30*254, "");
				LeadingMenu.Add(document, list, 0.35*254, "");
				LeadingMenu.Add(document, list, 0.40*254, "");
				LeadingMenu.Add(document, list, 0.45*254, "");
				LeadingMenu.Add(document, list, 0.50*254, "");
				LeadingMenu.Add(document, list, 0.60*254, "");
				LeadingMenu.Add(document, list, 0.70*254, "");
				LeadingMenu.Add(document, list, 0.80*254, "");
				LeadingMenu.Add(document, list, 0.90*254, "");
				LeadingMenu.Add(document, list, 1.00*254, "");
			}
			else
			{
				LeadingMenu.Add(document, list,  40.0, "");  // 4mm
				LeadingMenu.Add(document, list,  50.0, "");
				LeadingMenu.Add(document, list,  60.0, "");
				LeadingMenu.Add(document, list,  70.0, "");
				LeadingMenu.Add(document, list,  80.0, "");
				LeadingMenu.Add(document, list,  90.0, "");
				LeadingMenu.Add(document, list, 100.0, "");
				LeadingMenu.Add(document, list, 120.0, "");
				LeadingMenu.Add(document, list, 150.0, "");
				LeadingMenu.Add(document, list, 200.0, "");
				LeadingMenu.Add(document, list, 300.0, "");
				LeadingMenu.Add(document, list, 400.0, "");
				LeadingMenu.Add(document, list, 500.0, "");
			}

			list.Sort();

			VMenu menu = new VMenu();
			string lastUnits = isDefault ? "" : "%";
			foreach ( LeadingMenu fs in list )
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
