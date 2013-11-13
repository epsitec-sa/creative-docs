﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			var mandat = new DataMandat (new System.DateTime (2000, 1, 1), new System.DateTime (2050, 12, 31));
			var objects = mandat.GetData (BaseType.Objects);

			var date2000 = new Timestamp (new System.DateTime (2000, 1, 1), 0);
			var date2001 = new Timestamp (new System.DateTime (2001, 1, 1), 0);
			var date2002 = new Timestamp (new System.DateTime (2002, 1, 1), 0);
			var date2003 = new Timestamp (new System.DateTime (2003, 1, 1), 0);
			var date2010 = new Timestamp (new System.DateTime (2010, 1, 4), 0);
			var date2011 = new Timestamp (new System.DateTime (2011, 1, 1), 0);
			var date2012 = new Timestamp (new System.DateTime (2012, 1, 1), 0);
			var date2013 = new Timestamp (new System.DateTime (2013, 1, 1), 0);

			var o0 = new DataObject ();
			objects.Add (o0);
			{

				var e = new DataEvent (date2000, EventType.Entrée);
				o0.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,   "Immobilisations"));
			}

			var o1 = new DataObject ();
			objects.Add (o1);
			{
				var e = new DataEvent (date2000, EventType.Entrée);
				o1.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "1"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Bâtiments"));
			}

			var o11 = new DataObject ();
			objects.Add (o11);
			{
				var e = new DataEvent (date2000, EventType.Entrée);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o1.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "1.1"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Immeubles"));
			}

			var o111 = new DataObject ();
			objects.Add (o111);
			{
				{
					var e = new DataEvent (date2000, EventType.Entrée);
					o111.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o11.Guid));
					e.AddProperty (new DataIntProperty            (ObjectField.Position,    0));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.1.1"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre administratif"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (3000000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (2500000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Paul"));
					DummyMandat.AddAmortissement1 (e);
				}

				for (int i=1; i<13; i++)
				{
					{
						var e = new DataEvent (new Timestamp (new System.DateTime (date2000.Date.Year+i, 12, 31), 0), EventType.AmortissementAuto);
						o111.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 3000000.0m-(i-1)*100000;
						decimal a2 = 3000000.0m-i*100000;
						e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (a1, a2)));
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
					e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o11.Guid));
					e.AddProperty (new DataIntProperty            (ObjectField.Position,    1));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.1.2"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre logistique"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (4550000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (6000000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Paul"));
					DummyMandat.AddAmortissement1 (e);
				}

				for (int i=1; i<10; i++)
				{
					{
						var e = new DataEvent (new Timestamp (new System.DateTime (date2002.Date.Year+i, 12, 31), 0), EventType.AmortissementAuto);
						o112.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 4550000.0m-(i-1)*200000;
						decimal a2 = 4550000.0m-i*200000;
						e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (a1, a2)));
					}
				}
			}

			var o113 = new DataObject ();
			objects.Add (o113);
			{
				var e = new DataEvent (date2013, EventType.Entrée);
				o113.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o11.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    2));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.1.3"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre d'expédition"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (2100000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (3000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Sandra"));
				DummyMandat.AddAmortissement1 (e);
			}

			var o12 = new DataObject ();
			objects.Add (o12);
			{
				var e = new DataEvent (date2000, EventType.Entrée);
				o12.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,      o1.Guid));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "1.2"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Usines"));
			}

			var o121 = new DataObject ();
			objects.Add (o121);
			{
				var e = new DataEvent (date2001, EventType.Entrée);
				o121.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o12.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.2.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre d'usinage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (10400000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (13000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Ernest"));
				DummyMandat.AddAmortissement1 (e);
			}

			var o122 = new DataObject ();
			objects.Add (o122);
			{
				var e = new DataEvent (date2002, EventType.Entrée);
				o122.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o12.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    1));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.2.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre d'assemblage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (8000000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (9500000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "René"));
				DummyMandat.AddAmortissement1 (e);
			}

			var o13 = new DataObject ();
			objects.Add (o13);
			{
				var e = new DataEvent (date2000, EventType.Entrée);
				o13.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,      o1.Guid));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "1.3"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Entrepôts"));
			}

			var o131 = new DataObject ();
			objects.Add (o131);
			{
				var e = new DataEvent (date2002, EventType.Entrée);
				o131.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o13.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.3.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Dépôt principal"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (2100000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (3500000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Anne-Sophie"));
				DummyMandat.AddAmortissement1 (e);
			}

			var o132 = new DataObject ();
			objects.Add (o132);
			{
				var e = new DataEvent (date2010, EventType.Entrée);
				o132.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o13.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    1));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.3.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Dépôt secondaire"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (5320000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (5000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Paul"));
				DummyMandat.AddAmortissement1 (e);
			}

			var o133 = new DataObject ();
			objects.Add (o133);
			{
				var e = new DataEvent (date2012, EventType.Entrée);
				o133.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o13.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    2));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "1.3.3"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Centre de recyclage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (1200000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (1500000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Victoria"));
				DummyMandat.AddAmortissement1 (e);
			}

			var o2 = new DataObject ();
			objects.Add (o2);
			{
				var e = new DataEvent (date2000, EventType.Entrée);
				o2.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,    0));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,      o0.Guid));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "2"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Véhicules"));
			}

			var o21 = new DataObject ();
			objects.Add (o21);
			{
				var e = new DataEvent (date2000, EventType.Entrée);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,      o2.Guid));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "2.1"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Camions"));
			}

			var o211 = new DataObject ();
			objects.Add (o211);
			{
				var e = new DataEvent (date2003, EventType.Entrée);
				o211.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o21.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.1.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Scania X20"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (150000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (160000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Jean-François"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "25004-800-65210-45R"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o212 = new DataObject ();
			objects.Add (o212);
			{
				var e = new DataEvent (date2003, EventType.Entrée);
				o212.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o21.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    1));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.1.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Scania X30 semi"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (180000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (200000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Serge"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Rouge"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "25004-800-20087-20X"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o213 = new DataObject ();
			objects.Add (o213);
			{
				{
					var e = new DataEvent (date2000, EventType.Entrée);
					o213.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o21.Guid));
					e.AddProperty (new DataIntProperty            (ObjectField.Position,    2));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.1.3"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Volvo T-200"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (90000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (75000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Jean-Pierre"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
					DummyMandat.AddAmortissement2 (e);
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
					e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o21.Guid));
					e.AddProperty (new DataIntProperty            (ObjectField.Position,    3));
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
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o21.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    4));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.1.5"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Volvo P-810"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (195000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Igor"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Bleu/Noir"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "T40-72-300-PW3B"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o22 = new DataObject ();
			objects.Add (o22);
			{
				var e = new DataEvent (date2000, EventType.Entrée);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,      o2.Guid));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "2.2"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Camionnettes"));
			}

			var o221 = new DataObject ();
			objects.Add (o221);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2007, 4, 17), 0), EventType.Entrée);
				o221.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o22.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.2.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Renault Doblo"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (28000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Francine"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "456-321-132-898908"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o222 = new DataObject ();
			objects.Add (o222);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 2, 6), 0), EventType.Entrée);
				o222.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o22.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    1));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.2.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Ford Transit"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (30000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (32000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Jean-Bernard"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Blanc"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o23 = new DataObject ();
			objects.Add (o23);
			{
				var e = new DataEvent (date2000, EventType.Entrée);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,      o2.Guid));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty  (ObjectField.Numéro,      "2.3"));
				e.AddProperty (new DataStringProperty  (ObjectField.Nom,         "Voitures"));
			}

			var o231 = new DataObject ();
			objects.Add (o231);
			{
				var e = new DataEvent (date2010, EventType.Entrée);
				o231.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o23.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    0));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.1"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Citroën C4 Picasso"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (22000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Simon"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Noir"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "D456-0003232-0005"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o232 = new DataObject ();
			objects.Add (o232);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2011, 8, 27), 0), EventType.Entrée);
				o232.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o23.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    1));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.2"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Opel Corsa"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (9000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (10000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Frédérique"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Bleu"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "45-3292302-544545-8"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o233 = new DataObject ();
			objects.Add (o233);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2005, 5, 1), 0), EventType.Entrée);
				o233.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o23.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    2));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.3"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Fiat Panda"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (8000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (5000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Dominique"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o234 = new DataObject ();
			objects.Add (o234);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2004, 5, 12), 0), EventType.Entrée);
				o234.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o23.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    3));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.4"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (11000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Denise"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Rouge"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "456000433434002"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o235 = new DataObject ();
			objects.Add (o235);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2011, 2, 1), 0), EventType.Entrée);
				o235.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o23.Guid));
				e.AddProperty (new DataIntProperty            (ObjectField.Position,    4));
				e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.5"));
				e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (12000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (13000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Marie"));
				e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Gris métalisé"));
				e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "780004563233232"));
				DummyMandat.AddAmortissement2 (e);
			}

			var o236 = new DataObject ();
			objects.Add (o236);
			{
				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2002, 11, 19), 0), EventType.Entrée);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o23.Guid));
					e.AddProperty (new DataIntProperty            (ObjectField.Position,    5));
					e.AddProperty (new DataStringProperty         (ObjectField.Numéro,      "2.3.6"));
					e.AddProperty (new DataStringProperty         (ObjectField.Nom,         "Toyota Yaris Verso"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1,     new ComputedAmount (16000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Responsable, "Christiane"));
					e.AddProperty (new DataStringProperty         (ObjectField.Couleur,     "Gris"));
					e.AddProperty (new DataStringProperty         (ObjectField.NuméroSérie, "F40T-500023-40232-30987-M"));
					DummyMandat.AddAmortissement2 (e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2003, 5, 1), 0), EventType.Augmentation);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2, new ComputedAmount (12000.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2003, 5, 1), 1), EventType.Augmentation);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2, new ComputedAmount (12000.0m, 12500.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2005, 12, 1), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Responsable, "Georges"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2009, 8, 25), 0), EventType.Diminution);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (16000.0m, 14500.0m, true)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2,     new ComputedAmount (12500.0m, 11000.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2010, 3, 1), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Responsable, "Damien"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 7, 12), 0), EventType.Diminution);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (12000.0m)));
				}
			}

			var o237 = new DataObject ();
			objects.Add (o237);
			{
				{
					var e = new DataEvent (date2012, EventType.Entrée);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty           (ObjectField.Parent,      o23.Guid));
					e.AddProperty (new DataIntProperty            (ObjectField.Position,    6));
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
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 7, 1), 0), EventType.Augmentation);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (5000.0m, 5200.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 8, 20), 0), EventType.Modification);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Responsable, "Frédérique"));
					e.AddProperty (new DataStringProperty (ObjectField.NuméroSérie, "F30T-340407-52118-40721-S"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 12, 31), 0), EventType.AmortissementExtra);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (5200.0m, 4600.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2, new ComputedAmount (3500.0m, 2400.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 3, 31), 0), EventType.Modification);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Responsable, "Daniel"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 4, 14), 0), EventType.Augmentation);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur2, new ComputedAmount (2400.0m, 3000.0m, true)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 6, 1), 0), EventType.Diminution);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Valeur1, new ComputedAmount (4600.0m, 2100.0m, true)));
				}
			}

#if false
			for (int j=0; j<2000; j++)
			{
				var o = new DataObject ();
				objects.Add (o);

				{
					var e = new DataEvent (start, EventType.Entrée);
					o.AddEvent (e);
					e.AddProperty (new DataGuidProperty   (ObjectField.OneShotNuméro, DummyMandat.EventNumber++));
					e.AddProperty (new DataGuidProperty           (ObjectField.Parent,       3));
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
#endif

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

			var o0 = new DataObject ();
			categories.Add (o0);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o0.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Catégories"));
			}

			var o1 = new DataObject ();
			categories.Add (o1);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o1.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o0.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "1"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Immobilier"));
			}

			var o11 = new DataObject ();
			categories.Add (o11);
			{
				{
					var e = new DataEvent (start, EventType.Entrée);
					o11.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty    (ObjectField.Parent,            o1.Guid));
					e.AddProperty (new DataIntProperty     (ObjectField.Position,          0));
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
					o11.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
					e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, 0.085m));
					e.AddProperty (new DataDecimalProperty (ObjectField.ValeurRésiduelle,  10000.0m));
				}

				{
					var e = new DataEvent (date2, EventType.Modification);
					o11.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
					e.AddProperty (new DataDecimalProperty (ObjectField.TauxAmortissement, 0.09m));
				}
			}

			var o12 = new DataObject ();
			categories.Add (o12);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o12.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,            o1.Guid));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,          1));
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

			var o2 = new DataObject ();
			categories.Add (o2);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o2.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Numéro, "2"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,    "Véhicule"));
			}

			var o21 = new DataObject ();
			categories.Add (o21);
			{
				var e = new DataEvent (date1, EventType.Entrée);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,            o2.Guid));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,          0));
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

			var o22 = new DataObject ();
			categories.Add (o22);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,            o2.Guid));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,          1));
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

			var o23 = new DataObject ();
			categories.Add (o23);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Parent,            o2.Guid));
				e.AddProperty (new DataIntProperty     (ObjectField.Position,          2));
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

			var o0 = new DataObject ();
			categories.Add (o0);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o0.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Groupes"));
			}

			var o1 = new DataObject ();
			categories.Add (o1);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o1.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Secteurs"));
			}

			var o11 = new DataObject ();
			categories.Add (o11);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o1.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Secteur"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Nord"));
			}

			var o12 = new DataObject ();
			categories.Add (o12);
			{
				{
					var e = new DataEvent (start, EventType.Entrée);
					o12.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o1.Guid));
					e.AddProperty (new DataIntProperty    (ObjectField.Position, 1));
					e.AddProperty (new DataStringProperty (ObjectField.Famille, "Secteur"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Sud"));
				}

				{
					var e = new DataEvent (date1, EventType.Modification);
					o12.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Sud-est"));
				}

				{
					var e = new DataEvent (date2, EventType.Modification);
					o12.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Sud-ouest"));
				}

				{
					var e = new DataEvent (date3, EventType.Modification);
					o12.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Sud"));
				}
			}

			var o13 = new DataObject ();
			categories.Add (o13);
			{
				var e = new DataEvent (date1, EventType.Entrée);
				o13.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o1.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 2));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     ""));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Secteur"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Est"));
			}

			var o14 = new DataObject ();
			categories.Add (o14);
			{
				var e = new DataEvent (date1, EventType.Entrée);
				o14.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o1.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 3));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Secteur"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Ouest"));
			}

			var o2 = new DataObject ();
			categories.Add (o2);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o2.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Centres de frais"));
			}

			var o21 = new DataObject ();
			categories.Add (o21);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o2.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Centre de frais"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Atelier"));
			}

			var o22 = new DataObject ();
			categories.Add (o22);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o2.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 1));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Centre de frais"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Bureaux"));
			}

			var o23 = new DataObject ();
			categories.Add (o23);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o2.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Centre de frais"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Distribution"));
			}

			var o24 = new DataObject ();
			categories.Add (o24);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o24.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o2.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 3));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Centre de frais"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Stockage"));
			}

			var o3 = new DataObject ();
			categories.Add (o3);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o3.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent, o0.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Nom, "Placements"));
			}

			var o31 = new DataObject ();
			categories.Add (o31);
			{
				var e = new DataEvent (start, EventType.Entrée);
				o31.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o3.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 0));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions suisses"));
			}

			var o32 = new DataObject ();
			categories.Add (o32);
			{
				{
					var e = new DataEvent (start, EventType.Entrée);
					o32.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
					e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o3.Guid));
					e.AddProperty (new DataIntProperty    (ObjectField.Position, 1));
					e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
					e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions européennes"));
				}

				{
					var e = new DataEvent (date3, EventType.Sortie);
					o32.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				}
			}

			var o33 = new DataObject ();
			categories.Add (o33);
			{
				var e = new DataEvent (date1, EventType.Entrée);
				o33.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o3.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 2));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions nord-américaines"));
			}

			var o34 = new DataObject ();
			categories.Add (o34);
			{
				var e = new DataEvent (date1, EventType.Entrée);
				o34.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o3.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 3));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions sub-américaines"));
			}

			var o35 = new DataObject ();
			categories.Add (o35);
			{
				var e = new DataEvent (date2, EventType.Entrée);
				o35.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.Parent,  o3.Guid));
				e.AddProperty (new DataIntProperty    (ObjectField.Position, 4));
				e.AddProperty (new DataStringProperty (ObjectField.Famille, "Placement"));
				e.AddProperty (new DataStringProperty (ObjectField.Nom,     "Portefeuille d’actions asiatiques"));
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