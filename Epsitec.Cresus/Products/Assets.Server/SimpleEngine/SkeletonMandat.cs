//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class SkeletonMandat
	{
		public static DataMandat GetMandat()
		{
			var mandat = new DataMandat ("Squelette", new System.DateTime (2000, 1, 1), new System.DateTime (2050, 12, 31));

			SkeletonMandat.AddCategories (mandat);
			SkeletonMandat.AddGroups (mandat);
			SkeletonMandat.AddObjects (mandat);

			return mandat;
		}

		private static void AddObjects(DataMandat mandat)
		{
		}

		private static void AddCategories(DataMandat mandat)
		{
			var categories = mandat.GetData (BaseType.Categories);

			var start  = new Timestamp (new System.DateTime (2013, 1, 1), 0);

			var o11 = new DataObject ();
			categories.Add (o11);
			{
				var e = new DataEvent (start, EventType.Input);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "11"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Bureaux"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.075m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Linear));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Annual));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue,  1000.0m));

				e.AddProperty (new DataStringProperty (ObjectField.Compte1, "1300 - Actifs transitoires"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte2, "1410 - Conptes de placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte3, "1530 - Véhicules"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte4, "1600 - Immeubles"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte5, "2440 - Hypothèques"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte6, "1510 - Outillage"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte7, "1520 - Informatique"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte8, "1601 - Terrains"));
			}

			var o12 = new DataObject ();
			categories.Add (o12);
			{
				var e = new DataEvent (start, EventType.Input);
				o12.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "12"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Usine"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.12m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Linear));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Annual));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue,  10000.0m));

				e.AddProperty (new DataStringProperty (ObjectField.Compte1, "1300 - Actifs transitoires"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte2, "1410 - Conptes de placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte3, "1530 - Véhicules"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte4, "1600 - Immeubles"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte5, "2440 - Hypothèques"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte6, "1510 - Outillage"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte7, "1520 - Informatique"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte8, "1601 - Terrains"));
			}

			var o21 = new DataObject ();
			categories.Add (o21);
			{
				var e = new DataEvent (start, EventType.Input);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "21"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Poid lourd"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.15m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Degressive));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Trimestrial));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue,  100.0m));

				e.AddProperty (new DataStringProperty (ObjectField.Compte1, "1300 - Actifs transitoires"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte2, "1410 - Conptes de placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte3, "1530 - Véhicules"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte4, "1600 - Immeubles"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte5, "2440 - Hypothèques"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte6, "1510 - Outillage"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte7, "1520 - Informatique"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte8, "1601 - Terrains"));
			}

			var o22 = new DataObject ();
			categories.Add (o22);
			{
				var e = new DataEvent (start, EventType.Input);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "22"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Camionnette"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.21m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Degressive));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Semestrial));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue,  100.0m));

				e.AddProperty (new DataStringProperty (ObjectField.Compte1, "1300 - Actifs transitoires"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte2, "1410 - Conptes de placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte3, "1530 - Véhicules"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte4, "1600 - Immeubles"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte5, "2440 - Hypothèques"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte6, "1510 - Outillage"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte7, "1520 - Informatique"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte8, "1601 - Terrains"));
			}

			var o23 = new DataObject ();
			categories.Add (o23);
			{
				var e = new DataEvent (start, EventType.Input);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "23"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Voiture"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.25m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Degressive));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Semestrial));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue,  100.0m));

				e.AddProperty (new DataStringProperty (ObjectField.Compte1, "1300 - Actifs transitoires"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte2, "1410 - Conptes de placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte3, "1530 - Véhicules"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte4, "1600 - Immeubles"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte5, "2440 - Hypothèques"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte6, "1510 - Outillage"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte7, "1520 - Informatique"));
				e.AddProperty (new DataStringProperty (ObjectField.Compte8, "1601 - Terrains"));
			}
		}

		private static void AddGroups(DataMandat mandat)
		{
			var categories = mandat.GetData (BaseType.Groups);

			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var o0 = new DataObject ();
			categories.Add (o0);
			{
				var e = new DataEvent (start, EventType.Input);
				o0.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Groupes"));
			}

			///////////////

			var oImmob = new DataObject ();
			categories.Add (oImmob);
			{
				var e = new DataEvent (start, EventType.Input);
				oImmob.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Immobilisations"));
			}

			var o1 = new DataObject ();
			categories.Add (o1);
			{
				var e = new DataEvent (start, EventType.Input);
				o1.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,    "Bâtiments"));
			}

			var o11 = new DataObject ();
			categories.Add (o11);
			{
				var e = new DataEvent (start, EventType.Input);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,    "Immeubles"));
			}

			var o12 = new DataObject ();
			categories.Add (o12);
			{
				var e = new DataEvent (start, EventType.Input);
				o12.AddEvent (e);
				e.AddProperty (new DataStringProperty  (ObjectField.OneShotNumber, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.GroupParent,      o1.Guid));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,         "Usines"));
			}

			var o13 = new DataObject ();
			categories.Add (o13);
			{
				var e = new DataEvent (start, EventType.Input);
				o13.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Entrepôts"));
			}

			///////////////

			var o2 = new DataObject ();
			categories.Add (o2);
			{
				var e = new DataEvent (start, EventType.Input);
				o2.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Véhicules"));
			}

			var o21 = new DataObject ();
			categories.Add (o21);
			{
				var e = new DataEvent (start, EventType.Input);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Camions"));
			}

			var o22 = new DataObject ();
			categories.Add (o22);
			{
				var e = new DataEvent (start, EventType.Input);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Camionnettes"));
			}

			var o23 = new DataObject ();
			categories.Add (o23);
			{
				var e = new DataEvent (start, EventType.Input);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Voitures"));
			}

			///////////////
			
			var o3 = new DataObject ();
			categories.Add (o3);
			{
				var e = new DataEvent (start, EventType.Input);
				o3.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Secteurs"));
			}

			var o31 = new DataObject ();
			categories.Add (o31);
			{
				var e = new DataEvent (start, EventType.Input);
				o31.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Nord"));
			}

			var o32 = new DataObject ();
			categories.Add (o32);
			{
				var e = new DataEvent (start, EventType.Input);
				o32.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Sud"));
			}

			var o33 = new DataObject ();
			categories.Add (o33);
			{
				var e = new DataEvent (start, EventType.Input);
				o33.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Est"));
			}

			var o34 = new DataObject ();
			categories.Add (o34);
			{
				var e = new DataEvent (start, EventType.Input);
				o34.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Ouest"));
			}

			///////////////

			var o4 = new DataObject ();
			categories.Add (o4);
			{
				var e = new DataEvent (start, EventType.Input);
				o4.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Centres de frais"));
			}

			var o41 = new DataObject ();
			categories.Add (o41);
			{
				var e = new DataEvent (start, EventType.Input);
				o41.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Atelier"));
			}

			var o42 = new DataObject ();
			categories.Add (o42);
			{
				var e = new DataEvent (start, EventType.Input);
				o42.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Bureaux"));
			}

			var o43 = new DataObject ();
			categories.Add (o43);
			{
				var e = new DataEvent (start, EventType.Input);
				o43.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Distribution"));
			}

			var o44 = new DataObject ();
			categories.Add (o44);
			{
				var e = new DataEvent (start, EventType.Input);
				o44.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Stockage"));
			}

			var o45 = new DataObject ();
			categories.Add (o45);
			{
				var e = new DataEvent (start, EventType.Input);
				o45.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Transports"));
			}

			///////////////

			var o5 = new DataObject ();
			categories.Add (o5);
			{
				var e = new DataEvent (start, EventType.Input);
				o5.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Responsables"));
			}

			var o51 = new DataObject ();
			categories.Add (o51);
			{
				var e = new DataEvent (start, EventType.Input);
				o51.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Simone"));
			}

			var o52 = new DataObject ();
			categories.Add (o52);
			{
				var e = new DataEvent (start, EventType.Input);
				o52.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Georges-André"));
			}

			var o53 = new DataObject ();
			categories.Add (o53);
			{
				var e = new DataEvent (start, EventType.Input);
				o53.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Béatrice"));
			}

			var o54 = new DataObject ();
			categories.Add (o54);
			{
				var e = new DataEvent (start, EventType.Input);
				o54.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Dominique"));
			}

			var o55 = new DataObject ();
			categories.Add (o55);
			{
				var e = new DataEvent (start, EventType.Input);
				o55.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Joël"));
			}

			var o56 = new DataObject ();
			categories.Add (o56);
			{
				var e = new DataEvent (start, EventType.Input);
				o56.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Paul-Henry"));
			}

			var o57 = new DataObject ();
			categories.Add (o57);
			{
				var e = new DataEvent (start, EventType.Input);
				o57.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Jean-Daniel"));
			}

			var o58 = new DataObject ();
			categories.Add (o58);
			{
				var e = new DataEvent (start, EventType.Input);
				o58.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Sandra"));
			}
		}

		private static void AddAmortissement1(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  (ObjectField.CategoryName, "Bureaux"));
		}

		private static void AddAmortissement2(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  (ObjectField.CategoryName, "Voiture"));
		}

		private static Guid GetGroup(DataMandat mandat, string text)
		{
			var list = mandat.GetData (BaseType.Groups);

			foreach (var group in list)
			{
				var nom = ObjectCalculator.GetObjectPropertyString (group, null, ObjectField.Name);
				if (nom == text)
				{
					return group.Guid;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("Le groupe {0} n'existe pas !", text));
			return Guid.Empty;
		}


		private static int EventNumber = 1;
		private static int CategoryNumber = 1;
		private static int GroupNumber = 1;
	}

}