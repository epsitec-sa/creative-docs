//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DummyMandat
	{
		public static DataMandat GetDummyMandat()
		{
			var mandat = new DataMandat (new System.DateTime (2013, 1, 1));
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
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    ((int) ObjectField.Level, 0));
				e.AddProperty (new DataStringProperty ((int) ObjectField.Nom,   "Immobilisations"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    ((int) ObjectField.Level,  1));
				e.AddProperty (new DataStringProperty ((int) ObjectField.Numéro, "1"));
				e.AddProperty (new DataStringProperty ((int) ObjectField.Nom,    "Bâtiments"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty    ((int) ObjectField.Level,  2));
				e.AddProperty (new DataStringProperty ((int) ObjectField.Numéro, "1.1"));
				e.AddProperty (new DataStringProperty ((int) ObjectField.Nom,    "Immeubles"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "1.1.1"));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Centre administratif"));
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (3000000.0m)));
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (2500000.0m)));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Paul"));
					DummyMandat.AddAmortissement1 (e);
				}

				for (int i=1; i<200; i++)
				{
					{
						var e = new DataEvent (new Timestamp (start.Date.AddDays (i*3), 0), EventType.AmortissementAuto);
						o.AddEvent (e);

						decimal a1 = 3000000.0m-(i-1)*10000;
						decimal a2 = 3000000.0m-i*10000;
						e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (a1, a2)));
					}
				}
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (date1, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "1.1.2"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Centre logistique"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (4550000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (6000000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Paul"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "1.1.3"));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Centre d'expédition"));
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (2100000.0m)));
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (3000000.0m)));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Sandra"));
					DummyMandat.AddAmortissement1 (e);
				}

				{
					var e = new DataEvent (date1, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty  ((int) ObjectField.NomCatégorie,           "Immobilier"));
					e.AddProperty (new DataDecimalProperty ((int) ObjectField.TauxAmortissement,      0.075m));
					e.AddProperty (new DataStringProperty  ((int) ObjectField.TypeAmortissement,      "Dégressif"));
					e.AddProperty (new DataIntProperty     ((int) ObjectField.FréquenceAmortissement, 12));
					e.AddProperty (new DataDecimalProperty ((int) ObjectField.ValeurRésiduelle,       1.0m));
				}

				{
					var e = new DataEvent (date3, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataDecimalProperty ((int) ObjectField.TauxAmortissement,      0.085m));
					e.AddProperty (new DataStringProperty  ((int) ObjectField.TypeAmortissement,      "Linéaire"));
				}
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Numéro,      "1.2"));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Nom,         "Usines"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "1.2.1"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Centre d'usinage"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (10400000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (13000000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Ernest"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "1.2.2"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Centre d'assemblage"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (8000000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (9500000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "René"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Numéro,      "1.3"));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Nom,         "Entrepôts"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "1.3.1"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Dépôt principal"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (2100000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (3500000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Anne-Sophie"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "1.3.2"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Dépôt secondaire"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (5320000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (5000000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Paul"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "1.3.3"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Centre de recyclage"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (1200000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (1500000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Victoria"));
				DummyMandat.AddAmortissement1 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty     ((int) ObjectField.Level,       1));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Numéro,      "2"));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Nom,         "Véhicules"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Numéro,      "2.1"));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Nom,         "Camions"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.1"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Scania X20"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (150000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (160000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Jean-François"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Blanc"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "25004-800-65210-45R"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.2"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Scania X30 semi"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (180000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (200000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Serge"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Rouge"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "25004-800-20087-20X"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.3"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Volvo T-200"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (90000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (75000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Jean-Pierre"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Blanc"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.4"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Volvo R-500"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (110000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (120000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Olivier"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Jaune/Noir"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "T40-56-200-65E4"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.5"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Volvo P-810"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (195000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Igor"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Bleu/Noir"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "T40-72-300-PW3B"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Numéro,      "2.2"));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Nom,         "Camionnettes"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.2.1"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Renault Doblo"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (28000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Francine"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Blanc"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "456-321-132-898908"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.2.2"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Ford Transit"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (30000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (32000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Jean-Bernard"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Blanc"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Numéro,      "2.3"));
				e.AddProperty (new DataStringProperty  ((int) ObjectField.Nom,         "Voitures"));
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.1"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Citroën C4 Picasso"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (22000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Simon"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Noir"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "D456-0003232-0005"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.2"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Opel Corsa"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (9000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (10000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Frédérique"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Bleu"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "45-3292302-544545-8"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.3"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Fiat Panda"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (8000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (5000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Dominique"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.4"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (11000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Denise"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Rouge"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "456000433434002"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				var e = new DataEvent (start, EventType.Entrée);
				o.AddEvent (e);
				e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.5"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (12000.0m)));
				e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (13000.0m)));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Marie"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Gris métalisé"));
				e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "780004563233232"));
				DummyMandat.AddAmortissement2 (e);
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.6"));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Toyota Yaris Verso"));
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (16000.0m)));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Christiane"));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Gris"));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "F40T-500023-40232-30987-M"));
					DummyMandat.AddAmortissement2 (e);
				}

				{
					var e = new DataEvent (date1, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2, new ComputedAmount (12000.0m)));
				}

				{
					var e = new DataEvent (date1b, EventType.Augmentation);
					o.AddEvent (e);
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2, new ComputedAmount (12000.0m, 12500.0m)));
				}

				{
					var e = new DataEvent (date3, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty  ((int) ObjectField.Responsable, "Georges"));
				}

				{
					var e = new DataEvent (date4, EventType.Diminution);
					o.AddEvent (e);
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (16000.0m, 14500.0m, true)));
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (12500.0m, 11000.0m)));
				}

				{
					var e = new DataEvent (date4b, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty ((int) ObjectField.Responsable, "Damien"));
				}

				{
					var e = new DataEvent (date5, EventType.Diminution);
					o.AddEvent (e);
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (12000.0m)));
				}
			}

			{
				var o = new DataObject ();
				mandat.Objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataIntProperty            ((int) ObjectField.Level,       3));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.7"));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Nom,         "Toyota Corolla"));
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (5000.0m)));
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (3500.0m)));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Responsable, "Georges"));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.Couleur,     "Noire"));
					e.AddProperty (new DataStringProperty         ((int) ObjectField.NuméroSérie, "F30T-340407-52118-40720-R"));
					DummyMandat.AddAmortissement2 (e);
				}

				{
					var e = new DataEvent (date1, EventType.Augmentation);
					o.AddEvent (e);
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (5000.0m, 5200.0m)));
				}

				{
					var e = new DataEvent (date2, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty ((int) ObjectField.Responsable, "Frédérique"));
					e.AddProperty (new DataStringProperty ((int) ObjectField.NuméroSérie, "F30T-340407-52118-40721-S"));
				}

				{
					var e = new DataEvent (date4, EventType.AmortissementExtra);
					o.AddEvent (e);
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (5200.0m, 4600.0m)));
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2, new ComputedAmount (3500.0m, 2400.0m)));
				}

				{
					var e = new DataEvent (date6, EventType.Modification);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty  ((int) ObjectField.Responsable, "Daniel"));
				}

				{
					var e = new DataEvent (date7, EventType.Augmentation);
					o.AddEvent (e);
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur2, new ComputedAmount (2400.0m, 3000.0m, true)));
				}

				{
					var e = new DataEvent (date8, EventType.Diminution);
					o.AddEvent (e);
					e.AddProperty (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (4600.0m, 2100.0m, true)));
				}
			}

			return mandat;
		}

		private static void AddAmortissement1(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  ((int) ObjectField.NomCatégorie,           "Immobilier"));
			e.AddProperty (new DataDateProperty    ((int) ObjectField.DateAmortissement1,     new System.DateTime (2014, 1, 1)));
			e.AddProperty (new DataDecimalProperty ((int) ObjectField.TauxAmortissement,      0.035m));
			e.AddProperty (new DataStringProperty  ((int) ObjectField.TypeAmortissement,      "Dégressif"));
			e.AddProperty (new DataIntProperty     ((int) ObjectField.FréquenceAmortissement, 6));
			e.AddProperty (new DataDecimalProperty ((int) ObjectField.ValeurRésiduelle,       1.0m));

			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte1, "1300 - Actifs transitoires"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte2, "1410 - Conptes de placement"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte3, "1530 - Véhicules"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte4, "1600 - Immeubles"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte5, "2440 - Hypothèques"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte6, "1510 - Outillage"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte7, "1520 - Informatique"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte8, "1601 - Terrains"));
		}

		private static void AddAmortissement2(DataEvent e)
		{
			e.AddProperty (new DataStringProperty  ((int) ObjectField.NomCatégorie,           "Véhicule"));
			e.AddProperty (new DataDateProperty    ((int) ObjectField.DateAmortissement1,     new System.DateTime (2013, 3, 1)));
			e.AddProperty (new DataDateProperty    ((int) ObjectField.DateAmortissement2,     new System.DateTime (2014, 1, 1)));
			e.AddProperty (new DataDecimalProperty ((int) ObjectField.TauxAmortissement,      0.18m));
			e.AddProperty (new DataStringProperty  ((int) ObjectField.TypeAmortissement,      "Linéaire"));
			e.AddProperty (new DataIntProperty     ((int) ObjectField.FréquenceAmortissement, 12));
			e.AddProperty (new DataDecimalProperty ((int) ObjectField.ValeurRésiduelle,       1.0m));

			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte1, "1300 - Actifs transitoires"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte2, "1410 - Conptes de placement"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte3, "1530 - Véhicules"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte4, "1600 - Immeubles"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte5, "2440 - Hypothèques"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte6, "1510 - Outillage"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte7, "1520 - Informatique"));
			e.AddProperty (new DataStringProperty ((int) ObjectField.Compte8, "1601 - Terrains"));
		}
	}

}