//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class Test
	{
		public static void Test1()
		{
			var mandat = new DataMandat (new System.DateTime (2013, 1, 1));
			var objects = mandat.GetData (BaseType.Objects);

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0), EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    (ObjectField.Level, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Immobilisations"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0), EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    (ObjectField.Level, 1));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "1"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Bâtiments"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0), EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    (ObjectField.Level, 2));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "1.1"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Immeubles"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0), EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataIntProperty     (ObjectField.Level, 3));
					e.AddProperty (new DataStringProperty  (ObjectField.Numéro, "1.1.1"));
					e.AddProperty (new DataStringProperty  (ObjectField.Nom, "Centre administratif"));
					e.AddProperty (new DataDecimalProperty (ObjectField.Valeur1, 2450000.0m));
					e.AddProperty (new DataStringProperty  (ObjectField.Responsable, "Paul"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 3, 1), 0), EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataDecimalProperty (ObjectField.Valeur1, 4000000.0m));
					e.AddProperty (new DataStringProperty  (ObjectField.Responsable, "René"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 2, 1), 0), EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataDecimalProperty (ObjectField.Valeur1, 3000000.0m));
				}

				var e1 = o.GetEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0));
				var e2 = o.GetEvent (new Timestamp (new System.DateTime (2013, 2, 1), 0));
				var e3 = o.GetEvent (new Timestamp (new System.DateTime (2013, 3, 1), 0));

				System.Diagnostics.Debug.Assert (e1.PropertiesCount == 5);
				System.Diagnostics.Debug.Assert (e2.PropertiesCount == 1);
				System.Diagnostics.Debug.Assert (e3.PropertiesCount == 2);

				var p11 = o.GetSingleProperty (new Timestamp (new System.DateTime (2013, 1, 1), 0), ObjectField.Level) as DataIntProperty;
				System.Diagnostics.Debug.Assert (p11 != null && p11.Value == 3);

				var p21 = o.GetSingleProperty (new Timestamp (new System.DateTime (2013, 2, 1), 0), ObjectField.Level) as DataIntProperty;
				System.Diagnostics.Debug.Assert (p21 == null);

				var p25 = o.GetSingleProperty (new Timestamp (new System.DateTime (2013, 2, 1), 0), ObjectField.Valeur1) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (p25 != null && p25.Value == 3000000.0m);

				var ps1 = o.GetSyntheticProperty (new Timestamp (new System.DateTime (2013, 1, 15), 0), ObjectField.Valeur1) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (ps1 != null && ps1.Value == 2450000.0m);

				var ps2 = o.GetSyntheticProperty (new Timestamp (new System.DateTime (2013, 2, 15), 0), ObjectField.Valeur1) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (ps2 != null && ps2.Value == 3000000.0m);

				var ps3 = o.GetSyntheticProperty (new Timestamp (new System.DateTime (2013, 3, 15), 0), ObjectField.Valeur1) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (ps3 != null && ps3.Value == 4000000.0m);
			}
		}
	}

}