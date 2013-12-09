//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyMandat
	{
		public static DataMandat GetMandat()
		{
			var mandat = new DataMandat ("Exemple", new System.DateTime (2000, 1, 1), new System.DateTime (2050, 12, 31));

			DummyMandat.AddPersons (mandat);
			DummyMandat.AddCategories (mandat);
			DummyMandat.AddGroups (mandat);
			DummyMandat.AddObjects (mandat);

			return mandat;
		}

		internal static void AddObjects(DataMandat mandat)
		{
			var objects = mandat.GetData (BaseType.Objects);

			var date2000 = new Timestamp (new System.DateTime (2000, 1, 1), 0);
			var date2001 = new Timestamp (new System.DateTime (2001, 1, 1), 0);
			var date2002 = new Timestamp (new System.DateTime (2002, 1, 1), 0);
			var date2003 = new Timestamp (new System.DateTime (2003, 1, 1), 0);
			var date2010 = new Timestamp (new System.DateTime (2010, 1, 4), 0);
			var date2011 = new Timestamp (new System.DateTime (2011, 1, 1), 0);
			var date2012 = new Timestamp (new System.DateTime (2012, 1, 1), 0);
			var date2013 = new Timestamp (new System.DateTime (2013, 1, 1), 0);

			var o111 = new DataObject ();
			objects.Add (o111);
			{
				{
					var e = new DataEvent (date2000, EventType.Entrée);
					o111.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Immeubles")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Est")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Bureaux")));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1110"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Siège social"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (3000000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (2500000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Paul"));
					DummyMandat.AddAmortissement (mandat, "Bureaux", e);
				}

				for (int i=1; i<13; i++)
				{
					{
						var e = new DataEvent (new Timestamp (new System.DateTime (date2000.Date.Year+i, 12, 31), 0), EventType.AmortissementAuto);
						o111.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 3000000.0m-(i-1)*100000;
						decimal a2 = 3000000.0m-i*100000;
						e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable, new ComputedAmount (a1, a2)));
					}
				}
			}

			var o112 = new DataObject ();
			objects.Add (o112);
			{
				{
					var e = new DataEvent (date2002, EventType.Entrée);
					o112.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Immeubles")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Sud")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Bureaux")));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1120"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre logistique"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (4550000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (6000000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Paul"));
					DummyMandat.AddAmortissement (mandat, "Bureaux", e);
				}

				for (int i=1; i<10; i++)
				{
					{
						var e = new DataEvent (new Timestamp (new System.DateTime (date2002.Date.Year+i, 12, 31), 0), EventType.AmortissementAuto);
						o112.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 4550000.0m-(i-1)*200000;
						decimal a2 = 4550000.0m-i*200000;
						e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable, new ComputedAmount (a1, a2)));
					}
				}
			}

			var o113 = new DataObject ();
			objects.Add (o113);
			{
				var e = new DataEvent (date2013, EventType.Entrée);
				o113.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Immeubles")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Est")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Distribution")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1130"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre d'expédition"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (2100000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (3000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Sandra"));
				DummyMandat.AddAmortissement (mandat, "Bureaux", e);
			}

			var o121 = new DataObject ();
			objects.Add (o121);
			{
				var e = new DataEvent (date2001, EventType.Entrée);
				o121.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Etrangères")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Est")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Atelier")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1210"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre d'usinage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (10400000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (13000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Ernest"));
				DummyMandat.AddAmortissement (mandat, "Usines", e);
			}

			var o122 = new DataObject ();
			objects.Add (o122);
			{
				var e = new DataEvent (date2002, EventType.Entrée);
				o122.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Suisses")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Nord")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Atelier")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1220"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre d'assemblage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (8000000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (9500000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "René"));
				DummyMandat.AddAmortissement (mandat, "Usines", e);
			}

			var o131 = new DataObject ();
			objects.Add (o131);
			{
				{
					var e = new DataEvent (date2002, EventType.Entrée);
					o131.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Entrepôts")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Nord")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Distribution")));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1310"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Dépôt principal"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (2100000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (3500000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Anne-Sophie"));
					DummyMandat.AddAmortissement (mandat, "Usines", e);
				}

				{
					var e = new DataEvent (date2010, EventType.Réorganisation);
					o131.AddEvent (e);
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatio+0, DummyMandat.GetGroup (mandat, "Immeubles")));
				}

				{
					var e = new DataEvent (date2011, EventType.Réorganisation);
					o131.AddEvent (e);
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatio+0, DummyMandat.GetGroup (mandat, "Suisses")));
				}
			}

			var o132 = new DataObject ();
			objects.Add (o132);
			{
				var e = new DataEvent (date2010, EventType.Entrée);
				o132.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Entrepôts")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Nord")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Distribution")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1320"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Dépôt secondaire"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (5320000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (5000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Paul"));
				DummyMandat.AddAmortissement (mandat, "Usines", e);
			}

			var o133 = new DataObject ();
			objects.Add (o133);
			{
				{
					var e = new DataEvent (date2012, EventType.Entrée);
					o133.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Entrepôts", 0.6m)));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Suisses", 0.4m)));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Sud")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+3,   DummyMandat.GetGroup (mandat, "Atelier")));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1330"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre de recyclage"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (1200000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (1500000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Victoria"));
					DummyMandat.AddAmortissement (mandat, "Usines", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 10, 18), 0), EventType.Modification);
					o133.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Maintenance, "Patrick"));
				}

				{
					var e = new DataEvent (date2013, EventType.Modification);
					o133.AddEvent (e);
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Entrepôts", 0.65m)));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Suisses", 0.35m)));
				}
			}

			var o211 = new DataObject ();
			objects.Add (o211);
			{
				var e = new DataEvent (date2003, EventType.Entrée);
				o211.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2110"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Scania X20"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (150000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (160000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Jean-François"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "25004-800-65210-45R"));
				DummyMandat.AddAmortissement (mandat, "Camions", e);
			}

			var o212 = new DataObject ();
			objects.Add (o212);
			{
				var e = new DataEvent (date2003, EventType.Entrée);
				o212.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2120"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Scania X30 semi"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (180000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (200000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Serge"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Rouge"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "25004-800-20087-20X"));
				DummyMandat.AddAmortissement (mandat, "Camions", e);
			}

			var o213 = new DataObject ();
			objects.Add (o213);
			{
				{
					var e = new DataEvent (date2000, EventType.Entrée);
					o213.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2130"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Volvo T-200"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (90000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (75000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Jean-Pierre"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
					DummyMandat.AddAmortissement (mandat, "Camions", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2005, 3, 20), 0), EventType.Sortie);
					o213.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				}
			}

			var o214 = new DataObject ();
			objects.Add (o214);
			{
				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2008, 9, 1), 0), EventType.Entrée);
					o214.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2140"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Volvo R-500"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (110000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (120000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Olivier"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Jaune/Noir"));
					e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "T40-56-200-65E4"));
					DummyMandat.AddAmortissement (mandat, "Camions", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 11, 5), 0), EventType.Sortie);
					o214.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				}
			}

			var o215 = new DataObject ();
			objects.Add (o215);
			{
				var e = new DataEvent (date2011, EventType.Entrée);
				o215.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2150"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Volvo P-810"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (195000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Igor"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Bleu/Noir"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "T40-72-300-PW3B"));
				DummyMandat.AddAmortissement (mandat, "Camions", e);
			}

			var o221 = new DataObject ();
			objects.Add (o221);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2007, 4, 17), 0), EventType.Entrée);
				o221.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camionnettes")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2210"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Renault Doblo"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (28000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Francine"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "456-321-132-898908"));
				DummyMandat.AddAmortissement (mandat, "Camionnettes", e);
			}

			var o222 = new DataObject ();
			objects.Add (o222);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 2, 6), 0), EventType.Entrée);
				o222.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camionnettes")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2220"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Ford Transit"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (30000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (32000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Jean-Bernard"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
				DummyMandat.AddAmortissement (mandat, "Camionnettes", e);
			}

			var o231 = new DataObject ();
			objects.Add (o231);
			{
				var e = new DataEvent (date2010, EventType.Entrée);
				o231.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2310"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Citroën C4 Picasso"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (22000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Simon"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Noir"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "D456-0003232-0005"));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o232 = new DataObject ();
			objects.Add (o232);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2011, 8, 27), 0), EventType.Entrée);
				o232.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2320"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Opel Corsa"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (9000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (10000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Frédérique"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Bleu"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "45-3292302-544545-8"));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o233 = new DataObject ();
			objects.Add (o233);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2005, 5, 1), 0), EventType.Entrée);
				o233.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2330"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Fiat Panda"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (8000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (5000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Dominique"));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o234 = new DataObject ();
			objects.Add (o234);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2004, 5, 12), 0), EventType.Entrée);
				o234.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2340"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (11000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Denise"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Rouge"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "456000433434002"));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o235 = new DataObject ();
			objects.Add (o235);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2011, 2, 1), 0), EventType.Entrée);
				o235.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2350"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (12000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (13000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Marie"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Gris métalisé"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "780004563233232"));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o236 = new DataObject ();
			objects.Add (o236);
			{
				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2002, 11, 19), 0), EventType.Entrée);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2360"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Toyota Yaris Verso"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (16000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Christiane"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Gris"));
					e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "F40T-500023-40232-30987-M"));
					DummyMandat.AddAmortissement (mandat, "Voitures", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2003, 5, 1), 0), EventType.Augmentation);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (12000.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2003, 5, 1), 1), EventType.Augmentation);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (12000.0m, 12500.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2005, 12, 1), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Maintenance, "Georges"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2009, 8, 25), 0), EventType.Diminution);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable, new ComputedAmount (16000.0m, 14500.0m, true)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (12500.0m, 11000.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2010, 3, 1), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Maintenance, "Damien"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 7, 12), 0), EventType.Diminution);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable, new ComputedAmount (12000.0m)));
				}
			}

			var o237 = new DataObject ();
			objects.Add (o237);
			{
				{
					var e = new DataEvent (date2012, EventType.Entrée);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2370"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Toyota Corolla"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable,     new ComputedAmount (5000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (3500.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Georges"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Noire"));
					e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "F30T-340407-52118-40720-R"));
					DummyMandat.AddAmortissement (mandat, "Voitures", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 7, 1), 0), EventType.Augmentation);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable, new ComputedAmount (5000.0m, 5200.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 8, 20), 0), EventType.Modification);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Maintenance, "Frédérique"));
					e.AddProperty (new DataStringProperty (ObjectField.NuméroSérie, "F30T-340407-52118-40721-S"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 12, 31), 0), EventType.AmortissementExtra);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable, new ComputedAmount (5200.0m, 4600.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (3500.0m, 2400.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 3, 31), 0), EventType.Modification);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Maintenance, "Daniel"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 4, 14), 0), EventType.Augmentation);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (2400.0m, 3000.0m, true)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 6, 1), 0), EventType.Diminution);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.ValeurComptable, new ComputedAmount (4600.0m, 2100.0m, true)));
				}
			}
		}

		private static void AddPersons(DataMandat mandat)
		{
			var categories = mandat.GetData (BaseType.Persons);

			var start  = new Timestamp (new System.DateTime (2013, 1, 1), 0);

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Daniel"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Roux"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Adresse, "Crésentine 33"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "1023"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Crissier"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Téléphone1, "021 671 05 92"));
					e.AddProperty (new DataStringProperty (ObjectField.Téléphone2, "021 671 05 91"));
					e.AddProperty (new DataStringProperty (ObjectField.Téléphone3, "078 671 95 87"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "roux@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Pierre"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Arnaud"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "1400"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Yverdon-les-Bains"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "arnaud@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Yédah"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Adjao"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "1400"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Yverdon-les-Bains"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "adjao@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "David"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Besuchet"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "besuchet@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Sandra"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Nicolet"));
					e.AddProperty (new DataStringProperty (ObjectField.Adresse, "Ch. du Levant 12"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "1002"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Lausanne"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "snicolet@bluewin.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Jean-Paul"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "André"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Mecano SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Adresse, "ZI. en Budron E<br/>Case postale 18"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "1025"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Le Mont-sur-Lausanne"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Téléphone3, "079 520 44 12"));
					e.AddProperty (new DataStringProperty (ObjectField.Description, "Réparateur officiel des stores Flexilux depuis 2008"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Josianne"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Schmidt"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Mathematika sàrl"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "josianne.schmidt@mathematika.com"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Christine"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Mercier"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Mathematika sàrl"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Téléphone3, "078 840 12 13"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "mercier.schmidt@mathematika.com"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Frédérique"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Bonnard"));
					e.AddProperty (new DataStringProperty (ObjectField.Adresse, "Ch. des Lys 45"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "1009"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Prilly"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Dubosson"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Fixnet AG"));
					e.AddProperty (new DataStringProperty (ObjectField.Adresse, "Market Platz 143"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "8003"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Zürich"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "dubosson@fixnet.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Hans"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Klein"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Fixnet AG"));
					e.AddProperty (new DataStringProperty (ObjectField.Adresse, "Market Platz 143"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "8003"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Zürich"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "klein@fixnet.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Pauline"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Gardaz"));
					e.AddProperty (new DataStringProperty (ObjectField.Entreprise, "Fixnet AG"));
					e.AddProperty (new DataStringProperty (ObjectField.Adresse, "Market Platz 143"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "8003"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Zürich"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "gardaz@fixnet.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Titre, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.Prénom, "Marie-José"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom, "Frutiger"));
					e.AddProperty (new DataStringProperty (ObjectField.Npa, "1092"));
					e.AddProperty (new DataStringProperty (ObjectField.Ville, "Belmont"));
					e.AddProperty (new DataStringProperty (ObjectField.Pays, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Téléphone2, "021 682 40 61"));
				}
			}
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "12"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,               "Usines"));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "21"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,               "Camions"));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "22"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,               "Camionnettes"));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "23"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,               "Voitures"));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Groupes"));
			}

			///////////////

			var oImmob = new DataObject ();
			categories.Add (oImmob);
			{
				var e = new DataEvent (start, EventType.Entrée);
				oImmob.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Immobilisations"));
			}

			var o1 = new DataObject ();
			categories.Add (o1);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o1.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Bâtiments"));
			}

			var o11 = new DataObject ();
			categories.Add (o11);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Immeubles"));
			}

			var o12 = new DataObject ();
			categories.Add (o12);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o12.AddEvent (e);
				e.AddProperty (new DataStringProperty  (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,      o1.Guid));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Usines"));
			}

			var o121 = new DataObject ();
			categories.Add (o121);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o121.AddEvent (e);
				e.AddProperty (new DataStringProperty  (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,      o12.Guid));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Suisses"));
			}

			var o122 = new DataObject ();
			categories.Add (o122);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o122.AddEvent (e);
				e.AddProperty (new DataStringProperty  (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,      o12.Guid));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Etrangères"));
			}

			var o13 = new DataObject ();
			categories.Add (o13);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o13.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,      o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Entrepôts"));
			}

			///////////////

			var o2 = new DataObject ();
			categories.Add (o2);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o2.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,      oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Véhicules"));
			}

			var o21 = new DataObject ();
			categories.Add (o21);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Camions"));
			}

			var o22 = new DataObject ();
			categories.Add (o22);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Camionnettes"));
			}

			var o23 = new DataObject ();
			categories.Add (o23);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,         "Voitures"));
			}

			///////////////
			
			var o3 = new DataObject ();
			categories.Add (o3);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o3.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Secteurs"));
			}

			var o31 = new DataObject ();
			categories.Add (o31);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o31.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Nord"));
			}

			var o32 = new DataObject ();
			categories.Add (o32);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o32.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Sud"));
			}

			var o33 = new DataObject ();
			categories.Add (o33);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o33.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Est"));
			}

			var o34 = new DataObject ();
			categories.Add (o34);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o34.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Ouest"));
			}

			///////////////

			var o4 = new DataObject ();
			categories.Add (o4);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o4.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Centres de frais"));
			}

			var o41 = new DataObject ();
			categories.Add (o41);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o41.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Atelier"));
			}

			var o42 = new DataObject ();
			categories.Add (o42);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o42.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Bureaux"));
			}

			var o43 = new DataObject ();
			categories.Add (o43);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o43.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Distribution"));
			}

			var o44 = new DataObject ();
			categories.Add (o44);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o44.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Stockage"));
			}

			var o45 = new DataObject ();
			categories.Add (o45);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o45.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Transports"));
			}


			///////////////

			var o5 = new DataObject ();
			categories.Add (o5);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o5.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Responsables"));
			}

			var o51 = new DataObject ();
			categories.Add (o51);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o51.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Simone"));
			}

			var o52 = new DataObject ();
			categories.Add (o52);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o52.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Georges-André"));
			}

			var o53 = new DataObject ();
			categories.Add (o53);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o53.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Béatrice"));
			}

			var o54 = new DataObject ();
			categories.Add (o54);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o54.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Dominique"));
			}

			var o55 = new DataObject ();
			categories.Add (o55);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o55.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Joël"));
			}

			var o56 = new DataObject ();
			categories.Add (o56);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o56.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Paul-Henry"));
			}

			var o57 = new DataObject ();
			categories.Add (o57);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o57.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Jean-Daniel"));
			}

			var o58 = new DataObject ();
			categories.Add (o58);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o58.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.Parent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Sandra"));
			}
			///////////////

			var o6 = new DataObject ();
			categories.Add (o6);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o6.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Placements"));
			}

			var o61 = new DataObject ();
			categories.Add (o61);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o61.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions suisses"));
			}

			var o62 = new DataObject ();
			categories.Add (o62);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o62.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions européennes"));
			}

			var o63 = new DataObject ();
			categories.Add (o63);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o63.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions nord-américaines"));
			}

			var o64 = new DataObject ();
			categories.Add (o64);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o64.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions sub-américaines"));
			}

			var o65 = new DataObject ();
			categories.Add (o65);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o65.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions asiatiques"));
			}
		}

		private static void AddAmortissement(DataMandat mandat, string nom, DataEvent e)
		{
			var cat = DummyMandat.GetCategory (mandat, nom);

			if (cat != null)
			{
				var taux   = ObjectCalculator.GetObjectPropertyDecimal (cat, null, ObjectField.TauxAmortissement);
				var type   = ObjectCalculator.GetObjectPropertyString  (cat, null, ObjectField.TypeAmortissement);
				var period = ObjectCalculator.GetObjectPropertyString  (cat, null, ObjectField.Périodicité);
				var rest   = ObjectCalculator.GetObjectPropertyDecimal (cat, null, ObjectField.ValeurRésiduelle);
				var c1     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte1);
				var c2     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte2);
				var c3     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte3);
				var c4     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte4);
				var c5     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte5);
				var c6     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte6);
				var c7     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte7);
				var c8     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte8);

				e.AddProperty (new DataStringProperty  (ObjectField.NomCatégorie,      nom));
				e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, taux.GetValueOrDefault ()));
				e.AddProperty (new DataStringProperty  (ObjectField.TypeAmortissement, type));
				e.AddProperty (new DataStringProperty  (ObjectField.Périodicité,       period));
				e.AddProperty (new DataDecimalProperty (ObjectField.ValeurRésiduelle,  rest.GetValueOrDefault ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Compte1,           c1));
				e.AddProperty (new DataStringProperty  (ObjectField.Compte2,           c2));
				e.AddProperty (new DataStringProperty  (ObjectField.Compte3,           c3));
				e.AddProperty (new DataStringProperty  (ObjectField.Compte4,           c4));
				e.AddProperty (new DataStringProperty  (ObjectField.Compte5,           c5));
				e.AddProperty (new DataStringProperty  (ObjectField.Compte6,           c6));
				e.AddProperty (new DataStringProperty  (ObjectField.Compte7,           c7));
				e.AddProperty (new DataStringProperty  (ObjectField.Compte8,           c8));
			}
		}

		private static DataObject GetCategory(DataMandat mandat, string text)
		{
			var list = mandat.GetData (BaseType.Categories);

			foreach (var group in list)
			{
				var nom = ObjectCalculator.GetObjectPropertyString (group, null, ObjectField.Nom);
				if (nom == text)
				{
					return group;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("La catégorie {0} n'existe pas !", text));
			return null;
		}

		private static GuidRatio GetGroup(DataMandat mandat, string text, decimal? ratio = null)
		{
			var list = mandat.GetData (BaseType.Groups);

			foreach (var group in list)
			{
				var nom = ObjectCalculator.GetObjectPropertyString (group, null, ObjectField.Nom);
				if (nom == text)
				{
					return new GuidRatio(group.Guid, ratio);
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("Le groupe {0} n'existe pas !", text));
			return GuidRatio.Empty;
		}


		private static int EventNumber = 1;
		private static int CategoryNumber = 1;
		private static int GroupNumber = 1;
	}

}