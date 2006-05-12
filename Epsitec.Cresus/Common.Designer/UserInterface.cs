using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Designer
{
	public class UserInterface
	{
		public static UI.Panel CreateEmptyPanel()
		{
			//	Crée un panneau "UI" vide pour expérimenter.

			UI.Panel panel = new Epsitec.Common.UI.Panel ();
			UI.DataSourceCollection sources = new Epsitec.Common.UI.DataSourceCollection ();

			CustomerRecord customer = new CustomerRecord ();

			customer.SetValue (CustomerRecord.NameProperty, "Arnaud");
			customer.SetValue (CustomerRecord.SurameProperty, "Pierre");
			customer.SetValue (CustomerRecord.AddressProperty, "Ch. du Fontenay 6");
			customer.SetValue (CustomerRecord.PostalNumberProperty, 1400);
			customer.SetValue (CustomerRecord.CityProperty, "Yverdon-les-Bains");

			sources.AddDataSource ("Customer", customer);
			
			panel.DataSource = sources;

			return panel;
		}
		
		
		private class CustomerRecord : Types.DependencyObject
		{
			//	Class "bidon" pour avoir au moins un source de données disponible.
			
			public CustomerRecord()
			{
			}
			
			public static readonly Types.DependencyProperty NameProperty = Types.DependencyProperty.Register ("Name", typeof (string), typeof (CustomerRecord));
			public static readonly Types.DependencyProperty SurameProperty = Types.DependencyProperty.Register ("Surname", typeof (string), typeof (CustomerRecord));
			public static readonly Types.DependencyProperty AddressProperty = Types.DependencyProperty.Register ("Address", typeof (string), typeof (CustomerRecord));
			public static readonly Types.DependencyProperty PostalNumberProperty = Types.DependencyProperty.Register ("PostalNumber", typeof (int), typeof (CustomerRecord));
			public static readonly Types.DependencyProperty CityProperty = Types.DependencyProperty.Register ("City", typeof (string), typeof (CustomerRecord));

		}
	}
}
