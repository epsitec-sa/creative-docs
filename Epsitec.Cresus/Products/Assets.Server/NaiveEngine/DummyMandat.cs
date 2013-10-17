//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public class DummyMandat
	{
		public static DataMandat GetDummyMandat()
		{
			var mandat = new DataMandat (new System.DateTime (2013, 1, 1));
			int objectId = 0;
			var start = new Timestamp (new System.DateTime (2013, 1, 1), 0);
			var date1 = new Timestamp (new System.DateTime (2013, 1, 7), 0);
			var date2 = new Timestamp (new System.DateTime (2013, 1, 7), 1);
			var date3 = new Timestamp (new System.DateTime (2013, 1, 15), 0);
			var date4 = new Timestamp (new System.DateTime (2013, 2, 1), 0);
			var date5 = new Timestamp (new System.DateTime (2013, 2, 4), 0);
			var date6 = new Timestamp (new System.DateTime (2013, 3, 31), 0);
			var date7 = new Timestamp (new System.DateTime (2013, 8, 21), 0);
			var date8 = new Timestamp (new System.DateTime (2013, 9, 18), 0);

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty    ((int) ObjectField.Level, 0));
				e.Properties.Add (new DataStringProperty ((int) ObjectField.Nom,   "Immobilisations"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty    ((int) ObjectField.Level,  1));
				e.Properties.Add (new DataStringProperty ((int) ObjectField.Numéro, "1"));
				e.Properties.Add (new DataStringProperty ((int) ObjectField.Nom,    "Bâtiments"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty    ((int) ObjectField.Level,  2));
				e.Properties.Add (new DataStringProperty ((int) ObjectField.Numéro, "1.1"));
				e.Properties.Add (new DataStringProperty ((int) ObjectField.Nom,    "Immeubles"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				{
					var e = new DataEvent (1, start, EventType.Entrée);
					o.AddEvent (e);
					e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "1.1.1"));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Centre administratif"));
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (3000000.0m)));
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (2500000.0m)));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Paul"));
				}

				for (int i=1; i<200; i++)
				{
					{
						var e = new DataEvent (1, new Timestamp (start.Date.AddDays (i*3), 0), EventType.Diminution);
						o.AddEvent (e);
						e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (3000000.0m-i*10000)));
					}
				}
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, date1, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "1.1.2"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Centre logistique"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (4550000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (6000000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Paul"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				{
					var e = new DataEvent (1, start, EventType.Entrée);
					o.AddEvent (e);
					e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "1.1.3"));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Centre d'expédition"));
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (2100000.0m)));
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (3000000.0m)));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Sandra"));
				}

				{
					var e = new DataEvent (1, date2, EventType.Modification);
					o.AddEvent (e);
					e.Properties.Add (new DataStringProperty  ((int) ObjectField.NomCatégorie,           "Immobilier"));
					e.Properties.Add (new DataDecimalProperty ((int) ObjectField.TauxAmortissement,      0.075m));
					e.Properties.Add (new DataStringProperty  ((int) ObjectField.TypeAmortissement,      "Dégressif"));
					e.Properties.Add (new DataIntProperty     ((int) ObjectField.FréquenceAmortissement, 12));
					e.Properties.Add (new DataDecimalProperty ((int) ObjectField.ValeurRésiduelle,       1.0m));
				}

				{
					var e = new DataEvent (1, date3, EventType.Modification);
					o.AddEvent (e);
					e.Properties.Add (new DataDecimalProperty ((int) ObjectField.TauxAmortissement,      0.085m));
					e.Properties.Add (new DataStringProperty  ((int) ObjectField.TypeAmortissement,      "Linéaire"));
				}
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Numéro,      "1.2"));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Nom,         "Usines"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "1.2.1"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Centre d'usinage"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (10400000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (13000000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Ernest"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "1.2.2"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Centre d'assemblage"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (8000000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (9500000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "René"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Numéro,      "1.3"));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Nom,         "Entrepôts"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "1.3.1"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Dépôt principal"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (2100000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (3500000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Anne-Sophie"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "1.3.2"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Dépôt secondaire"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (5320000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (5000000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Paul"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "1.3.3"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Centre de recyclage"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (1200000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (1500000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Vistoria"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty     ((int) ObjectField.Level,       1));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Numéro,      "2"));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Nom,         "Véhicules"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Numéro,      "2.1"));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Nom,         "Camions"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.1"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Scania X20"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (150000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (160000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Jean-François"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Blanc"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "25004-800-65210-45R"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.2"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Scania X30 semi"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (180000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (200000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Serge"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Rouge"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "25004-800-20087-20X"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.3"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Volvo T-200"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (90000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (75000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Jean-Pierre"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Blanc"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.4"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Volvo R-500"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (110000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (120000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Olivier"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Jaune/Noir"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "T40-56-200-65E4"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.1.5"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Volvo P-810"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (195000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Igor"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Bleu/Noir"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "T40-72-300-PW3B"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Numéro,      "2.2"));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Nom,         "Camionnettes"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.2.1"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Renault Doblo"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (25000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (28000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Francine"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Blanc"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "456-321-132-898908"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.2.2"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Ford Transit"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (30000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (32000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Jean-Bernard"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Blanc"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty     ((int) ObjectField.Level,       2));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Numéro,      "2.3"));
				e.Properties.Add (new DataStringProperty  ((int) ObjectField.Nom,         "Voitures"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.1"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Citroën C4 Picasso"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (22000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (25000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Simon"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Noir"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "D456-0003232-0005"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.2"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Opel Corsa"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (9000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (10000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Frédérique"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Bleu"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "45-3292302-544545-8"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.3"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Fiat Panda"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (8000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (5000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Dominique"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.4"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Fiat Uno"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (11000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Denise"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Rouge"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "456000433434002"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				var e = new DataEvent (1, start, EventType.Entrée);
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.5"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Fiat Uno"));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (12000.0m)));
				e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (13000.0m)));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Marie"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Gris métalisé"));
				e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "780004563233232"));
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				{
					var e = new DataEvent (1, start, EventType.Entrée);
					o.AddEvent (e);
					e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.6"));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Toyota Yaris Verso"));
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (16000.0m)));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Christiane"));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Gris"));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "F40T-500023-40232-30987-M"));
				}

				{
					var e = new DataEvent (1, date1, EventType.Modification);
					o.AddEvent (e);
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2, new ComputedAmount (12000.0m)));
				}

				{
					var e = new DataEvent (1, date2, EventType.Augmentation);
					o.AddEvent (e);
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2, new ComputedAmount (12000.0m, 500.0m, 12500.0m, false, false)));
				}

				{
					var e = new DataEvent (1, date3, EventType.Modification);
					o.AddEvent (e);
					e.Properties.Add (new DataStringProperty  ((int) ObjectField.Responsable, "Georges"));
				}

				{
					var e = new DataEvent (1, date4, EventType.Modification);
					o.AddEvent (e);
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (16000.0m, 1500.0m, 14500.0m, true, false)));
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (12500.0m, 1500.0m, 11000.0m, true, false)));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Damien"));
				}

				{
					var e = new DataEvent (1, date5, EventType.Diminution);
					o.AddEvent (e);
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (12000.0m)));
				}
			}

			{
				var o = new DataObject (objectId++);
				mandat.Objects.Add (o);

				{
					var e = new DataEvent (1, start, EventType.Entrée);
					o.AddEvent (e);
					e.Properties.Add (new DataIntProperty            ((int) ObjectField.Level,       3));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Numéro,      "2.3.7"));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Nom,         "Toyota Corolla"));
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1,     new ComputedAmount (5000.0m)));
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2,     new ComputedAmount (3500.0m)));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Responsable, "Georges"));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.Couleur,     "Noire"));
					e.Properties.Add (new DataStringProperty         ((int) ObjectField.NuméroSérie, "F30T-340407-52118-40720-R"));
				}

				{
					var e = new DataEvent (1, date1, EventType.Augmentation);
					o.AddEvent (e);
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (5000.0m, 200.0m, 5200.0m, false, false)));
				}

				{
					var e = new DataEvent (1, date4, EventType.Diminution);
					o.AddEvent (e);
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (5200.0m, 600.0m, 4600.0m, true, false)));
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2, new ComputedAmount (3500.0m, 1100.0m, 2400.0m, true, false)));
				}

				{
					var e = new DataEvent (1, date6, EventType.Modification);
					o.AddEvent (e);
					e.Properties.Add (new DataStringProperty  ((int) ObjectField.Responsable, "Daniel"));
				}

				{
					var e = new DataEvent (1, date7, EventType.Augmentation);
					o.AddEvent (e);
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur2, new ComputedAmount (2400.0m, 600.0m, 3000.0m, false, false)));
				}

				{
					var e = new DataEvent (1, date8, EventType.Diminution);
					o.AddEvent (e);
					e.Properties.Add (new DataComputedAmountProperty ((int) ObjectField.Valeur1, new ComputedAmount (4600.0m, 2500.0m, 2100.0m, true, false)));
				}
			}

			return mandat;
		}
	}

}