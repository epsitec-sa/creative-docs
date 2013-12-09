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
				var e = new DataEvent (start, EventType.Entrée);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "11"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,               "Bureaux"));
				e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, 0.075m));
				e.AddProperty (new DataStringProperty  (ObjectField.TypeAmortissement, "Linéaire"));
				e.AddProperty (new DataStringProperty  (ObjectField.Périodicité,       "Annuelle"));
				e.AddProperty (new DataDecimalProperty (ObjectField.ValeurRésiduelle,  1000.0m));

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
				var e = new DataEvent (start, EventType.Entrée);
				o12.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "12"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,               "Usine"));
				e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, 0.12m));
				e.AddProperty (new DataStringProperty  (ObjectField.TypeAmortissement, "Linéaire"));
				e.AddProperty (new DataStringProperty  (ObjectField.Périodicité,       "Annuelle"));
				e.AddProperty (new DataDecimalProperty (ObjectField.ValeurRésiduelle,  10000.0m));

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
				var e = new DataEvent (start, EventType.Entrée);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "21"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,               "Poid lourd"));
				e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, 0.15m));
				e.AddProperty (new DataStringProperty  (ObjectField.TypeAmortissement, "Dégressif"));
				e.AddProperty (new DataStringProperty  (ObjectField.Périodicité,       "Trimestrielle"));
				e.AddProperty (new DataDecimalProperty (ObjectField.ValeurRésiduelle,  100.0m));

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
				var e = new DataEvent (start, EventType.Entrée);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "22"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,               "Camionnette"));
				e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, 0.21m));
				e.AddProperty (new DataStringProperty  (ObjectField.TypeAmortissement, "Dégressif"));
				e.AddProperty (new DataStringProperty  (ObjectField.Périodicité,       "Semestrielle"));
				e.AddProperty (new DataDecimalProperty (ObjectField.ValeurRésiduelle,  100.0m));

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
				var e = new DataEvent (start, EventType.Entrée);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "23"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,               "Voiture"));
				e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, 0.25m));
				e.AddProperty (new DataStringProperty  (ObjectField.TypeAmortissement, "Dégressif"));
				e.AddProperty (new DataStringProperty  (ObjectField.Périodicité,       "Semestrielle"));
				e.AddProperty (new DataDecimalProperty (ObjectField.ValeurRésiduelle,  100.0m));

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
				var e = new DataEvent (start, EventType.Entrée);
				o0.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Groupes"));
			}

			///////////////

			var oImmob = new DataObject ();
			categories.Add (oImmob);
			{
				var e = new DataEvent (start, EventType.Entrée);
				oImmob.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Immobilisations"));
			}

			var o1 = new DataObject ();
			categories.Add (o1);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o1.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Bâtiments"));
			}

			var o11 = new DataObject ();
			categories.Add (o11);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Immeubles"));
			}

			var o12 = new DataObject ();
			categories.Add (o12);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o12.AddEvent (e);
				e.AddProperty (new DataStringProperty  (ObjectField.OneShotNuméro, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.GroupParent,      o1.Guid));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Usines"));
			}

			var o13 = new DataObject ();
			categories.Add (o13);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o13.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Entrepôts"));
			}

			///////////////

			var o2 = new DataObject ();
			categories.Add (o2);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o2.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Véhicules"));
			}

			var o21 = new DataObject ();
			categories.Add (o21);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Camions"));
			}

			var o22 = new DataObject ();
			categories.Add (o22);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Camionnettes"));
			}

			var o23 = new DataObject ();
			categories.Add (o23);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Voitures"));
			}

			///////////////
			
			var o3 = new DataObject ();
			categories.Add (o3);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o3.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Secteurs"));
			}

			var o31 = new DataObject ();
			categories.Add (o31);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o31.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Nord"));
			}

			var o32 = new DataObject ();
			categories.Add (o32);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o32.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Sud"));
			}

			var o33 = new DataObject ();
			categories.Add (o33);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o33.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Est"));
			}

			var o34 = new DataObject ();
			categories.Add (o34);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o34.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Ouest"));
			}

			///////////////

			var o4 = new DataObject ();
			categories.Add (o4);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o4.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Centres de frais"));
			}

			var o41 = new DataObject ();
			categories.Add (o41);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o41.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Atelier"));
			}

			var o42 = new DataObject ();
			categories.Add (o42);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o42.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Bureaux"));
			}

			var o43 = new DataObject ();
			categories.Add (o43);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o43.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Distribution"));
			}

			var o44 = new DataObject ();
			categories.Add (o44);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o44.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Stockage"));
			}

			var o45 = new DataObject ();
			categories.Add (o45);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o45.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Transports"));
			}

			///////////////

			var o5 = new DataObject ();
			categories.Add (o5);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o5.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Responsables"));
			}

			var o51 = new DataObject ();
			categories.Add (o51);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o51.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Simone"));
			}

			var o52 = new DataObject ();
			categories.Add (o52);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o52.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Georges-André"));
			}

			var o53 = new DataObject ();
			categories.Add (o53);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o53.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Béatrice"));
			}

			var o54 = new DataObject ();
			categories.Add (o54);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o54.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Dominique"));
			}

			var o55 = new DataObject ();
			categories.Add (o55);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o55.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Joël"));
			}

			var o56 = new DataObject ();
			categories.Add (o56);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o56.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Paul-Henry"));
			}

			var o57 = new DataObject ();
			categories.Add (o57);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o57.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Jean-Daniel"));
			}

			var o58 = new DataObject ();
			categories.Add (o58);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o58.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (SkeletonMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Sandra"));
			}
		}

		private static void AddAmortissement1(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  (ObjectField.NomCatégorie, "Bureaux"));
		}

		private static void AddAmortissement2(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  (ObjectField.NomCatégorie, "Voiture"));
		}

		private static Guid GetGroup(DataMandat mandat, string text)
		{
			var list = mandat.GetData (BaseType.Groups);

			foreach (var group in list)
			{
				var nom = ObjectCalculator.GetObjectPropertyString (group, null, ObjectField.Nom);
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