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

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0), EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    (1, 0));
				e.AddProperty (new DataStringProperty (2, "Immobilisations"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0), EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    (1, 1));
				e.AddProperty (new DataStringProperty (2, "1"));
				e.AddProperty (new DataStringProperty (3, "Bâtiments"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0), EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    (1, 2));
				e.AddProperty (new DataStringProperty (2, "1.1"));
				e.AddProperty (new DataStringProperty (3, "Immeubles"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0), EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataIntProperty     (1, 3));
					e.AddProperty (new DataStringProperty  (2, "1.1.1"));
					e.AddProperty (new DataStringProperty  (3, "Centre administratif"));
					e.AddProperty (new DataDecimalProperty (4, 2450000.0m));
					e.AddProperty (new DataStringProperty  (6, "Paul"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 3, 1), 0), EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataDecimalProperty (5, 4000000.0m));
					e.AddProperty (new DataStringProperty  (6, "René"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 2, 1), 0), EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataDecimalProperty (5, 3000000.0m));
				}

				var e1 = o.GetEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0));
				var e2 = o.GetEvent (new Timestamp (new System.DateTime (2013, 2, 1), 0));
				var e3 = o.GetEvent (new Timestamp (new System.DateTime (2013, 3, 1), 0));

				System.Diagnostics.Debug.Assert (e1.PropertiesCount == 5);
				System.Diagnostics.Debug.Assert (e2.PropertiesCount == 1);
				System.Diagnostics.Debug.Assert (e3.PropertiesCount == 2);

				var p11 = o.GetSingleProperty (new Timestamp (new System.DateTime (2013, 1, 1), 0), 1) as DataIntProperty;
				System.Diagnostics.Debug.Assert (p11 != null && p11.Value == 3);

				var p21 = o.GetSingleProperty (new Timestamp (new System.DateTime (2013, 2, 1), 0), 1) as DataIntProperty;
				System.Diagnostics.Debug.Assert (p21 == null);

				var p25 = o.GetSingleProperty (new Timestamp (new System.DateTime (2013, 2, 1), 0), 5) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (p25 != null && p25.Value == 3000000.0m);

				var ps1 = o.GetSyntheticProperty (new Timestamp (new System.DateTime (2013, 1, 15), 0), 4) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (ps1 != null && ps1.Value == 2450000.0m);

				var ps2 = o.GetSyntheticProperty (new Timestamp (new System.DateTime (2013, 2, 15), 0), 5) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (ps2 != null && ps2.Value == 3000000.0m);

				var ps3 = o.GetSyntheticProperty (new Timestamp (new System.DateTime (2013, 3, 15), 0), 5) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (ps3 != null && ps3.Value == 4000000.0m);
			}
		}
	}

}