//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyPersons
	{
		public static void AddSettings(DataMandat mandat)
		{
			DummyPersons.fieldLastName    = DummyPersons.AddSettings (mandat, BaseType.Persons, "Nom",           FieldType.String, 120, 380, 1, 2,     0);
			DummyPersons.fieldFirstName   = DummyPersons.AddSettings (mandat, BaseType.Persons, "Prénom",        FieldType.String, 120, 380, 1, 1,     0);
			DummyPersons.fieldTitle       = DummyPersons.AddSettings (mandat, BaseType.Persons, "Titre",         FieldType.String,  80, 120, 1, null,  0);
			DummyPersons.fieldCompany     = DummyPersons.AddSettings (mandat, BaseType.Persons, "Entreprise",    FieldType.String, 120, 380, 1, 3,     0);
			DummyPersons.fieldAddress     = DummyPersons.AddSettings (mandat, BaseType.Persons, "Adresse",       FieldType.String, 150, 380, 2, null,  0);
			DummyPersons.fieldZip         = DummyPersons.AddSettings (mandat, BaseType.Persons, "NPA",           FieldType.String,  50,  60, 1, null,  0);
			DummyPersons.fieldCity        = DummyPersons.AddSettings (mandat, BaseType.Persons, "Ville",         FieldType.String, 120, 380, 1, null,  0);
			DummyPersons.fieldCountry     = DummyPersons.AddSettings (mandat, BaseType.Persons, "Pays",          FieldType.String, 120, 380, 1, null,  0);
			DummyPersons.fieldPhone1      = DummyPersons.AddSettings (mandat, BaseType.Persons, "Tél. prof.",    FieldType.String, 100, 120, 1, null, 10);
			DummyPersons.fieldPhone2      = DummyPersons.AddSettings (mandat, BaseType.Persons, "Tél. privé",    FieldType.String, 100, 120, 1, null,  0);
			DummyPersons.fieldPhone3      = DummyPersons.AddSettings (mandat, BaseType.Persons, "Tél. portable", FieldType.String, 100, 120, 1, null,  0);
			DummyPersons.fieldMail        = DummyPersons.AddSettings (mandat, BaseType.Persons, "E-mail",        FieldType.String, 200, 380, 1, null,  0);
			DummyPersons.fieldPersonDesc  = DummyPersons.AddSettings (mandat, BaseType.Persons, "Description",   FieldType.String, 200, 380, 5, null, 10);
		}

		internal static ObjectField AddSettings(DataMandat mandat, BaseType baseType, string name, FieldType type, int columnWidth, int? lineWidth, int? lineCount, int? summaryOrder, int topMargin)
		{
			var field = mandat.Settings.GetNewUserField ();
			mandat.Settings.AddUserField (baseType, new UserField (name, field, type, columnWidth, lineWidth, lineCount, summaryOrder, topMargin));
			return field;
		}


		public static void AddPersons(DataMandat mandat)
		{
			var categories = mandat.GetData (BaseType.Persons);

			var start  = new Timestamp (new System.DateTime (2013, 1, 1), 0);

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Daniel"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Roux"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldAddress, "Crésentine 33"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1023"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Crissier"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldPhone1, "021 671 05 92"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldPhone2, "021 671 05 91"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldPhone3, "078 671 95 87"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "roux@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Pierre"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Arnaud"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1400"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Yverdon-les-Bains"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "arnaud@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Madame"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Yédah"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Adjao"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1400"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Yverdon-les-Bains"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "adjao@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "David"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Besuchet"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "besuchet@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Madame"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Sandra"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Nicolet"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldAddress, "Ch. du Levant 12"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1002"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Lausanne"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "snicolet@bluewin.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Jean-Paul"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "André"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Mecano SA"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldAddress, "ZI. en Budron E<br/>Case postale 18"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1025"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Le Mont-sur-Lausanne"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldPhone3, "079 520 44 12"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldPersonDesc, "Réparateur officiel des stores Flexilux depuis 2008"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Madame"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Josianne"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Schmidt"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Mathematika sàrl"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "josianne.schmidt@mathematika.com"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Madame"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Christine"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Mercier"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Mathematika sàrl"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldPhone3, "078 840 12 13"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "christine.mercier@mathematika.com"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Frédérique"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Bonnard"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldAddress, "Ch. des Lys 45"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1009"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Prilly"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Dubosson"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Fixnet AG"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldAddress, "Market Platz 143"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "8003"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Zürich"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "dubosson@fixnet.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Hans"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Klein"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Fixnet AG"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldAddress, "Market Platz 143"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "8003"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Zürich"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "klein@fixnet.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Madame"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Pauline"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Gardaz"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Fixnet AG"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldAddress, "Market Platz 143"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "8003"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Zürich"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "gardaz@fixnet.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Marc-Antoine"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Frutiger"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Garage du Soleil"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1092"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Belmont"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldPhone1, "021 682 40 61"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "François"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Borlandi"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Maxi Store SA"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1004"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Lausanne"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldPhone3, "079 905 33 41"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Monsieur"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Ernesto"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Di Magnolia"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "Merlin Transport SA"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldAddress, "Place du Tunnel 2"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1800"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Vevey"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "support@merlin.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Madame"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Françoise"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Diserens"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCompany, "CHUV"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldZip, "1000"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCity, "Lausanne"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "francoise.diserens@chuv.vd.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Madame"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Emilie"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Franco"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "emilie.franco@bluewin.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Madame"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Paulette"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Sigmund"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "paulette.simoulino@bluewin.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyPersons.fieldTitle, "Madame"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldFirstName, "Géraldine"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldLastName, "Traxel"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldCountry, "Suisse"));
					e.AddProperty (new DataStringProperty (DummyPersons.fieldMail, "geraldine.traxel@bluewin.ch"));
				}
			}
		}

		public static Guid GetPerson(DataMandat mandat, string text)
		{
			var list = mandat.GetData (BaseType.Persons);

			foreach (var person in list)
			{
				var nom = ObjectProperties.GetObjectPropertyString (person, null, DummyPersons.fieldLastName);
				if (nom == text)
				{
					return person.Guid;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("La personne {0} n'existe pas !", text));
			return Guid.Empty;
		}


		private static ObjectField fieldLastName;
		private static ObjectField fieldFirstName;
		private static ObjectField fieldTitle;
		private static ObjectField fieldCompany;
		private static ObjectField fieldAddress;
		private static ObjectField fieldZip;
		private static ObjectField fieldCity;
		private static ObjectField fieldCountry;
		private static ObjectField fieldPhone1;
		private static ObjectField fieldPhone2;
		private static ObjectField fieldPhone3;
		private static ObjectField fieldMail;
		private static ObjectField fieldPersonDesc;
	}
}