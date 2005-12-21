using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.Menus
{
	public class LeadingMenu : System.IComparable
	{
		public LeadingMenu(Document document, double value, string units)
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
				this.text = string.Format("{0} {1}", document.Modifier.RealToString(this.value), document.Modifier.ShortNameUnitDimension);
			}
		}

		public int CompareTo(object obj)
		{
			//	Défini par System.IComparable.
			LeadingMenu that = obj as LeadingMenu;

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

		public static VMenu CreateLeadingMenu(Document document, double currentValue, string currentUnits, MessageEventHandler message)
		{
			//	Construit le menu pour choisir un interligne.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			LeadingMenu current = new LeadingMenu(document, currentValue, currentUnits);

			LeadingMenu.Add(document, list, currentValue, currentUnits);

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
			string lastUnits = "%";
			foreach ( LeadingMenu fs in list )
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
