//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyMandat
	{
		public static DataMandat GetDummyMandat()
		{
			var mandat = new DataMandat (new System.DateTime (2013, 1, 1));
			var objects = mandat.GetData (BaseType.Objects);

			var start  = new Timestamp (new System.DateTime (2013, 1, 1), 0);
			var date1  = new Timestamp (new System.DateTime (2013, 1, 7), 0);
			var date1b = new Timestamp (new System.DateTime (2013, 1, 7), 1);
			var date2  = new Timestamp (new System.DateTime (2013, 1, 14), 0);
			var date3  = new Timestamp (new System.DateTime (2013, 1, 15), 0);
			var date4  = new Timestamp (new System.DateTime (2013, 2, 1), 0);
			var date4b = new Timestamp (new System.DateTime (2013, 2, 1), 1);
			var date5  = new Timestamp (new System.DateTime (2013, 2, 4), 0);
			var date6  = new Timestamp (new System.DateTime (2013, 3, 31), 0);
			var date7  = new Timestamp (new System.DateTime (2013, 8, 21), 0);
			var date8  = new Timestamp (new System.DateTime (2013, 9, 19), 0);

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,   "Immobilisations"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,  1));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "1"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Bâtiments"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,  2));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "1.1"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Immeubles"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.1.1"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre administratif"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (3000000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (2500000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Paul"));
					DummyMandat.AddAmortissement1 (e);
				}

				for (int i=1; i<200; i++)
				{
					{
						var e = new DataEvent (new Timestamp (start.Date.AddDays (i*3), 0), EventType.AmortissementAuto);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 3000000.0m-(i-1)*10000;
						decimal a2 = 3000000.0m-i*10000;
						e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (a1, a2)));
					}
				}
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.1.2"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre logistique"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (4550000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (6000000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Paul"));
					DummyMandat.AddAmortissement1 (e);
				}

				for (int i=1; i<50; i++)
				{
					{
						var e = new DataEvent (new Timestamp (start.Date.AddDays (i*13), 0), EventType.AmortissementAuto);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 4550000.0m-(i-1)*100000;
						decimal a2 = 4550000.0m-i*100000;
						e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (a1, a2)));
					}
				}
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (date1, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.1.3"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre d'expédition"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (2100000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (3000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Sandra"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "1.2"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Usines"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.2.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre d'usinage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (10400000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (13000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Ernest"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.2.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre d'assemblage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (8000000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (9500000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "René"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "1.3"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Entrepôts"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.3.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Dépôt principal"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (2100000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (3500000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Anne-Sophie"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.3.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Dépôt secondaire"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (5320000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (5000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Paul"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.3.3"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre de recyclage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (1200000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (1500000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Victoria"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,       1));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "2"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Véhicules"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "2.1"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Camions"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.1.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Scania X20"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (150000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (160000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Jean-François"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "25004-800-65210-45R"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.1.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Scania X30 semi"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (180000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (200000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Serge"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Rouge"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "25004-800-20087-20X"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.1.3"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Volvo T-200"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (90000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (75000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Jean-Pierre"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
					DummyMandat.AddAmortissement2 (e);
				}

				{
					var e = new DataEvent (date4, EventType.Sortie);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				}
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.1.4"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Volvo R-500"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (110000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (120000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Olivier"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Jaune/Noir"));
					e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "T40-56-200-65E4"));
					DummyMandat.AddAmortissement2 (e);
				}

				{
					var e = new DataEvent (date3, EventType.Sortie);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				}
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.1.5"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Volvo P-810"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (195000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Igor"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Bleu/Noir"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "T40-72-300-PW3B"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "2.2"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Camionnettes"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.2.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Renault Doblo"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (28000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Francine"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "456-321-132-898908"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.2.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Ford Transit"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (30000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (32000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Jean-Bernard"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "2.3"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Voitures"));
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    (ObjectField.OneShotNuméro, DummyMandat.EventNumber++));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Citroën C4 Picasso"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (22000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Simon"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Noir"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "D456-0003232-0005"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Opel Corsa"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (9000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (10000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Frédérique"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Bleu"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "45-3292302-544545-8"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.3"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Fiat Panda"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (8000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (5000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Dominique"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.4"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (11000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Denise"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Rouge"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "456000433434002"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.5"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (12000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (13000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Marie"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Gris métalisé"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "780004563233232"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.6"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Toyota Yaris Verso"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (16000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Christiane"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Gris"));
					e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "F40T-500023-40232-30987-M"));
					DummyMandat.AddAmortissement2 (e);
				}

				{
					var e = new DataEvent (date1, EventType.Augmentation);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2, new ComputedAmount (12000.0m)));
				}

				{
					var e = new DataEvent (date1b, EventType.Augmentation);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2, new ComputedAmount (12000.0m, 12500.0m)));
				}

				{
					var e = new DataEvent (date3, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Responsable, "Georges"));
				}

				{
					var e = new DataEvent (date4, EventType.Diminution);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (16000.0m, 14500.0m, true)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (12500.0m, 11000.0m)));
				}

				{
					var e = new DataEvent (date4b, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Responsable, "Damien"));
				}

				{
					var e = new DataEvent (date5, EventType.Diminution);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (12000.0m)));
				}
			}

			{
				var o = new DataObject ();
				objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.7"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Toyota Corolla"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (5000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (3500.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Georges"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Noire"));
					e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "F30T-340407-52118-40720-R"));
					DummyMandat.AddAmortissement2 (e);
				}

				{
					var e = new DataEvent (date1, EventType.Augmentation);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (5000.0m, 5200.0m)));
				}

				{
					var e = new DataEvent (date2, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Responsable, "Frédérique"));
					e.AddProperty (new DataStringProperty (ObjectField.NuméroSérie, "F30T-340407-52118-40721-S"));
				}

				{
					var e = new DataEvent (date4, EventType.AmortissementExtra);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (5200.0m, 4600.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2, new ComputedAmount (3500.0m, 2400.0m)));
				}

				{
					var e = new DataEvent (date6, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Responsable, "Daniel"));
				}

				{
					var e = new DataEvent (date7, EventType.Augmentation);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2, new ComputedAmount (2400.0m, 3000.0m, true)));
				}

				{
					var e = new DataEvent (date8, EventType.Diminution);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (4600.0m, 2100.0m, true)));
				}
			}

			for (int j=0; j<2000; j++)
			{
				var o = new DataObject ();
				objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataIntProperty    (ObjectField.OneShotNuméro, DummyMandat.EventNumber++));
					e.AddProperty (new DataIntProperty            (ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.99"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Ford T "+j));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (22000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (25000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Simon"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Noir"));
					e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "D456-0003232-0005"));
					DummyMandat.AddAmortissement2 (e);
				}

				for (int i=1; i<20; i++)
				{
					{
						var e = new DataEvent (new Timestamp (start.Date.AddDays (i*30), 0), EventType.AmortissementAuto);
						o.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 22000.0m-(i-1)*1000;
						decimal a2 = 22000.0m-i*1000;
						e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (a1, a2)));
					}
				}
			}

			DummyMandat.AddCategories (mandat);
			DummyMandat.AddGroups (mandat);

			return mandat;
		}

		private static void AddCategories(DataMandat mandat)
		{
			var categories = mandat.GetData (BaseType.Categories);

			var start  = new Timestamp (new System.DateTime (2013, 1, 1), 0);
			var date1  = new Timestamp (new System.DateTime (2013, 3, 12), 0);
			var date2  = new Timestamp (new System.DateTime (2013, 7, 2), 0);

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Catégories"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,  1));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "1"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Immobilier"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
					e.AddProperty (new DataIntProperty     (ObjectField.Level,             2));
					e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "1.1"));
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

				{
					var e = new DataEvent (date1, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
					e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, 0.085m));
					e.AddProperty (new DataDecimalProperty (ObjectField.ValeurRésiduelle,  10000.0m));
				}

				{
					var e = new DataEvent (date2, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
					e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, 0.09m));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,             2));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "1.2"));
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

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,  1));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "2"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Véhicule"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (date1, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,             2));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "2.1"));
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

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,             2));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "2.2"));
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

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Level,             2));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,            "2.3"));
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

			var start  = new Timestamp (new System.DateTime (2013, 1, 1), 0);
			var date1  = new Timestamp (new System.DateTime (2013, 1, 19), 0);
			var date2  = new Timestamp (new System.DateTime (2013, 1, 22), 0);
			var date3  = new Timestamp (new System.DateTime (2013, 1, 28), 0);

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   0));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Groupes"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   1));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Secteurs"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Secteur"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Nord"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
					e.AddProperty (new DataStringProperty (ObjectField.Famille, "Secteur"));
					e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Sud"));
				}

				{
					var e = new DataEvent (date1, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Sud-est"));
				}

				{
					var e = new DataEvent (date2, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Sud-ouest"));
				}

				{
					var e = new DataEvent (date3, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Sud"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (date1, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro,  "1.3"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     ""));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Secteur"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Est"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (date1, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Secteur"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Ouest"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   1));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Centres de frais"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Centre de frais"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Atelier"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Centre de frais"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Bureaux"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Centre de frais"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Distribution"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Centre de frais"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Stockage"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   1));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placements"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Portefeuille d’actions suisses"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
					e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
					e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Portefeuille d’actions européennes"));
				}

				{
					var e = new DataEvent (date3, EventType.Sortie);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (date1, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Portefeuille d’actions nord-américaines"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (date1, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Portefeuille d’actions sub-américaines"));
			}

			{
				var o = new DataObject ();
				categories.Add (o);

				var e = new DataEvent (date2, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataIntProperty    (ObjectField.Level,   2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Membre,  "Portefeuille d’actions asiatiques"));
			}
		}

		private static void AddAmortissement1(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  (ObjectField.NomCatégorie1, "Bureaux"));
		}

		private static void AddAmortissement2(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  (ObjectField.NomCatégorie1, "Voiture"));
		}


		private static int EventNumber = 1;
		private static int CategoryNumber = 1;
		private static int GroupNumber = 1;
	}

}