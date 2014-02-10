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
					var e = new DataEvent (date2000, EventType.Input);
					o111.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Immeubles")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Est")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Bureaux")));
					e.AddProperty (new DataStringProperty         (ObjectField.Number,      "1110"));
					e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Siège social"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (3000000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (2500000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Paul"));
					e.AddProperty (new DataGuidProperty           (ObjectField.Person1, DummyMandat.GetPerson (mandat, "Arnaud")));
					e.AddProperty (new DataGuidProperty           (ObjectField.Person2, DummyMandat.GetPerson (mandat, "Schmidt")));
					e.AddProperty (new DataGuidProperty           (ObjectField.Person4, DummyMandat.GetPerson (mandat, "Bonnard")));
					DummyMandat.AddAmortissement (mandat, "Bureaux", e);
				}

				for (int i=1; i<13; i++)
				{
					{
						var e = new DataEvent (new Timestamp (new System.DateTime (date2000.Date.Year+i, 12, 31), 0), EventType.AmortizationAuto);
						o111.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 3000000.0m-(i-1)*100000;
						decimal a2 = 3000000.0m-i*100000;
						e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue, new ComputedAmount (a1, a2)));
					}
				}
			}

			var o112 = new DataObject ();
			objects.Add (o112);
			{
				{
					var e = new DataEvent (date2002, EventType.Input);
					o112.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Immeubles")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Sud")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Bureaux")));
					e.AddProperty (new DataStringProperty         (ObjectField.Number,      "1120"));
					e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Centre logistique"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (4550000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (6000000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Paul"));
					e.AddProperty (new DataGuidProperty           (ObjectField.Person1, DummyMandat.GetPerson (mandat, "Arnaud")));
					e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Mercier")));
					DummyMandat.AddAmortissement (mandat, "Bureaux", e);
				}

				for (int i=1; i<10; i++)
				{
					{
						var e = new DataEvent (new Timestamp (new System.DateTime (date2002.Date.Year+i, 12, 31), 0), EventType.AmortizationAuto);
						o112.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 4550000.0m-(i-1)*200000;
						decimal a2 = 4550000.0m-i*200000;
						e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue, new ComputedAmount (a1, a2)));
					}
				}
			}

			var o113 = new DataObject ();
			objects.Add (o113);
			{
				var e = new DataEvent (date2013, EventType.Input);
				o113.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Immeubles")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Est")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Distribution")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "1130"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Centre d'expédition"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (2100000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (3000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Sandra"));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person1, DummyMandat.GetPerson (mandat, "André")));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person5, DummyMandat.GetPerson (mandat, "Klein")));
				DummyMandat.AddAmortissement (mandat, "Bureaux", e);
			}

			var o121 = new DataObject ();
			objects.Add (o121);
			{
				var e = new DataEvent (date2001, EventType.Input);
				o121.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Etrangères")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Est")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Atelier")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "1210"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Centre d'usinage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (10400000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (13000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Ernest"));
				DummyMandat.AddAmortissement (mandat, "Usines", e);
			}

			var o122 = new DataObject ();
			objects.Add (o122);
			{
				var e = new DataEvent (date2002, EventType.Input);
				o122.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Suisses")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Nord")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Atelier")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "1220"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Centre d'assemblage"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (8000000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (9500000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "René"));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person1, DummyMandat.GetPerson (mandat, "Arnaud")));
				DummyMandat.AddAmortissement (mandat, "Usines", e);
			}

			var o131 = new DataObject ();
			objects.Add (o131);
			{
				{
					var e = new DataEvent (date2002, EventType.Input);
					o131.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Entrepôts")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Nord")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Distribution")));
					e.AddProperty (new DataStringProperty         (ObjectField.Number,      "1310"));
					e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Dépôt principal"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (2100000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (3500000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Anne-Sophie"));
					DummyMandat.AddAmortissement (mandat, "Usines", e);
				}

				{
					var e = new DataEvent (date2010, EventType.Reorganization);
					o131.AddEvent (e);
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatio+0, DummyMandat.GetGroup (mandat, "Immeubles")));
				}

				{
					var e = new DataEvent (date2011, EventType.Reorganization);
					o131.AddEvent (e);
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatio+0, DummyMandat.GetGroup (mandat, "Suisses")));
				}
			}

			var o132 = new DataObject ();
			objects.Add (o132);
			{
				var e = new DataEvent (date2010, EventType.Input);
				o132.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Entrepôts")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Nord")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Distribution")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "1320"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Dépôt secondaire"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (5320000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (5000000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Paul"));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person1, DummyMandat.GetPerson (mandat, "Gardaz")));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "André")));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person4, DummyMandat.GetPerson (mandat, "André")));
				DummyMandat.AddAmortissement (mandat, "Usines", e);
			}

			var o133 = new DataObject ();
			objects.Add (o133);
			{
				{
					var e = new DataEvent (date2012, EventType.Input);
					o133.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Entrepôts", 0.6m)));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+1,   DummyMandat.GetGroup (mandat, "Suisses", 0.4m)));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Sud")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+3,   DummyMandat.GetGroup (mandat, "Atelier")));
					e.AddProperty (new DataStringProperty         (ObjectField.Number,      "1330"));
					e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Centre de recyclage"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (1200000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (1500000.0m)));
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
				var e = new DataEvent (date2003, EventType.Input);
				o211.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2110"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Scania X20"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (150000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (160000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Jean-François"));
				e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Blanc"));
				e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "25004-800-65210-45R"));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Frutiger")));
				DummyMandat.AddAmortissement (mandat, "Camions", e);
			}

			var o212 = new DataObject ();
			objects.Add (o212);
			{
				var e = new DataEvent (date2003, EventType.Input);
				o212.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2120"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Scania X30 semi"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (180000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (200000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Serge"));
				e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Rouge"));
				e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "25004-800-20087-20X"));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Frutiger")));
				DummyMandat.AddAmortissement (mandat, "Camions", e);
			}

			var o213 = new DataObject ();
			objects.Add (o213);
			{
				{
					var e = new DataEvent (date2000, EventType.Input);
					o213.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2130"));
					e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Volvo T-200"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (90000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (75000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Jean-Pierre"));
					e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Blanc"));
					e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Frutiger")));
					DummyMandat.AddAmortissement (mandat, "Camions", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2005, 3, 20), 0), EventType.Output);
					o213.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				}
			}

			var o214 = new DataObject ();
			objects.Add (o214);
			{
				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2008, 9, 1), 0), EventType.Input);
					o214.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2140"));
					e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Volvo R-500"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (110000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (120000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Olivier"));
					e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Jaune/Noir"));
					e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "T40-56-200-65E4"));
					e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Frutiger")));
					DummyMandat.AddAmortissement (mandat, "Camions", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 11, 5), 0), EventType.Output);
					o214.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				}
			}

			var o215 = new DataObject ();
			objects.Add (o215);
			{
				var e = new DataEvent (date2011, EventType.Input);
				o215.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camions")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2150"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Volvo P-810"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (195000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Igor"));
				e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Bleu/Noir"));
				e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "T40-72-300-PW3B"));
				DummyMandat.AddAmortissement (mandat, "Camions", e);
			}

			var o221 = new DataObject ();
			objects.Add (o221);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2007, 4, 17), 0), EventType.Input);
				o221.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camionnettes")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2210"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Renault Doblo"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (28000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Francine"));
				e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Blanc"));
				e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "456-321-132-898908"));
				DummyMandat.AddAmortissement (mandat, "Camionnettes", e);
			}

			var o222 = new DataObject ();
			objects.Add (o222);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 2, 6), 0), EventType.Input);
				o222.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Camionnettes")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2220"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Ford Transit"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (30000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (32000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Jean-Bernard"));
				e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Blanc"));
				DummyMandat.AddAmortissement (mandat, "Camionnettes", e);
			}

			var o231 = new DataObject ();
			objects.Add (o231);
			{
				var e = new DataEvent (date2010, EventType.Input);
				o231.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2310"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Citroën C4 Picasso"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (22000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Simon"));
				e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Noir"));
				e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "D456-0003232-0005"));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Frutiger")));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o232 = new DataObject ();
			objects.Add (o232);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2011, 8, 27), 0), EventType.Input);
				o232.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2320"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Opel Corsa"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (9000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (10000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Frédérique"));
				e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Bleu"));
				e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "45-3292302-544545-8"));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o233 = new DataObject ();
			objects.Add (o233);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2005, 5, 1), 0), EventType.Input);
				o233.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2330"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Fiat Panda"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (8000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (5000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Dominique"));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Frutiger")));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o234 = new DataObject ();
			objects.Add (o234);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2004, 5, 12), 0), EventType.Input);
				o234.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2340"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (11000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Denise"));
				e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Rouge"));
				e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "456000433434002"));
				e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Frutiger")));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o235 = new DataObject ();
			objects.Add (o235);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2011, 2, 1), 0), EventType.Input);
				o235.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2350"));
				e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (12000.0m)));
				e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (13000.0m)));
				e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Marie"));
				e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Gris métalisé"));
				e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "780004563233232"));
				DummyMandat.AddAmortissement (mandat, "Voitures", e);
			}

			var o236 = new DataObject ();
			objects.Add (o236);
			{
				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2002, 11, 19), 0), EventType.Input);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2360"));
					e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Toyota Yaris Verso"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (16000.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Christiane"));
					e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Gris"));
					e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "F40T-500023-40232-30987-M"));
					e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Frutiger")));
					DummyMandat.AddAmortissement (mandat, "Voitures", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2003, 5, 1), 0), EventType.Increase);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1, new ComputedAmount (12000.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2003, 5, 1), 1), EventType.Increase);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1, new ComputedAmount (12000.0m, 12500.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2005, 12, 1), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Maintenance, "Georges"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2009, 8, 25), 0), EventType.Decrease);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue, new ComputedAmount (16000.0m, 14500.0m, true)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (12500.0m, 11000.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2010, 3, 1), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Maintenance, "Damien"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 7, 12), 0), EventType.Decrease);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue, new ComputedAmount (12000.0m)));
				}
			}

			var o237 = new DataObject ();
			objects.Add (o237);
			{
				{
					var e = new DataEvent (date2012, EventType.Input);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+0,   DummyMandat.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatio+2,   DummyMandat.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (ObjectField.Number,      "2370"));
					e.AddProperty (new DataStringProperty         (ObjectField.Name,         "Toyota Corolla"));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue,     new ComputedAmount (5000.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1,     new ComputedAmount (3500.0m)));
					e.AddProperty (new DataStringProperty         (ObjectField.Maintenance, "Georges"));
					e.AddProperty (new DataStringProperty         (ObjectField.Color,     "Noire"));
					e.AddProperty (new DataStringProperty         (ObjectField.SerialNumber, "F30T-340407-52118-40720-R"));
					e.AddProperty (new DataGuidProperty           (ObjectField.Person3, DummyMandat.GetPerson (mandat, "Frutiger")));
					DummyMandat.AddAmortissement (mandat, "Voitures", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 7, 1), 0), EventType.Increase);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue, new ComputedAmount (5000.0m, 5200.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 8, 20), 0), EventType.Modification);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Maintenance, "Frédérique"));
					e.AddProperty (new DataStringProperty (ObjectField.SerialNumber, "F30T-340407-52118-40721-S"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 12, 31), 0), EventType.AmortizationExtra);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue, new ComputedAmount (5200.0m, 4600.0m)));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1, new ComputedAmount (3500.0m, 2400.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 3, 31), 0), EventType.Modification);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (ObjectField.Maintenance, "Daniel"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 4, 14), 0), EventType.Increase);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.Value1, new ComputedAmount (2400.0m, 3000.0m, true)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 6, 1), 0), EventType.Decrease);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (ObjectField.MainValue, new ComputedAmount (4600.0m, 2100.0m, true)));
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
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Daniel"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Roux"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Address, "Crésentine 33"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1023"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Crissier"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Phone1, "021 671 05 92"));
					e.AddProperty (new DataStringProperty (ObjectField.Phone2, "021 671 05 91"));
					e.AddProperty (new DataStringProperty (ObjectField.Phone3, "078 671 95 87"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "roux@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Pierre"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Arnaud"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1400"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Yverdon-les-Bains"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "arnaud@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Yédah"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Adjao"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1400"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Yverdon-les-Bains"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "adjao@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "David"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Besuchet"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Epsitec SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "besuchet@epsitec.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Sandra"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Nicolet"));
					e.AddProperty (new DataStringProperty (ObjectField.Address, "Ch. du Levant 12"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1002"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Lausanne"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "snicolet@bluewin.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Jean-Paul"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "André"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Mecano SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Address, "ZI. en Budron E<br/>Case postale 18"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1025"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Le Mont-sur-Lausanne"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Phone3, "079 520 44 12"));
					e.AddProperty (new DataStringProperty (ObjectField.Description, "Réparateur officiel des stores Flexilux depuis 2008"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Josianne"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Schmidt"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Mathematika sàrl"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "josianne.schmidt@mathematika.com"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Christine"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Mercier"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Mathematika sàrl"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Phone3, "078 840 12 13"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "christine.mercier@mathematika.com"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Frédérique"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Bonnard"));
					e.AddProperty (new DataStringProperty (ObjectField.Address, "Ch. des Lys 45"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1009"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Prilly"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Dubosson"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Fixnet AG"));
					e.AddProperty (new DataStringProperty (ObjectField.Address, "Market Platz 143"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "8003"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Zürich"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "dubosson@fixnet.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Hans"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Klein"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Fixnet AG"));
					e.AddProperty (new DataStringProperty (ObjectField.Address, "Market Platz 143"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "8003"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Zürich"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "klein@fixnet.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Pauline"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Gardaz"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Fixnet AG"));
					e.AddProperty (new DataStringProperty (ObjectField.Address, "Market Platz 143"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "8003"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Zürich"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "gardaz@fixnet.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Marc-Antoine"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Frutiger"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Garage du Soleil"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1092"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Belmont"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Phone1, "021 682 40 61"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "François"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Borlandi"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Maxi Store SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1004"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Lausanne"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Phone3, "079 905 33 41"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Monsieur"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Ernesto"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Di Magnolia"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "Merlin Transport SA"));
					e.AddProperty (new DataStringProperty (ObjectField.Address, "Place du Tunnel 2"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1800"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Vevey"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "support@merlin.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Françoise"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Diserens"));
					e.AddProperty (new DataStringProperty (ObjectField.Company, "CHUV"));
					e.AddProperty (new DataStringProperty (ObjectField.Zip, "1000"));
					e.AddProperty (new DataStringProperty (ObjectField.City, "Lausanne"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "francoise.diserens@chuv.vd.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Emilie"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Franco"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "emilie.franco@bluewin.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Paulette"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Sigmund"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "paulette.simoulino@bluewin.ch"));
				}
			}

			{
				var o = new DataObject ();
				categories.Add (o);
				{
					var e = new DataEvent (start, EventType.Input);
					o.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.Title, "Madame"));
					e.AddProperty (new DataStringProperty (ObjectField.FirstName, "Géraldine"));
					e.AddProperty (new DataStringProperty (ObjectField.Name, "Traxel"));
					e.AddProperty (new DataStringProperty (ObjectField.Country, "Suisse"));
					e.AddProperty (new DataStringProperty (ObjectField.Mail, "geraldine.traxel@bluewin.ch"));
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
				var e = new DataEvent (start, EventType.Input);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "12"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Usines"));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "21"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Camions"));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "22"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Camionnettes"));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "23"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Voitures"));
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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Groupes"));
			}

			///////////////

			var oImmob = new DataObject ();
			categories.Add (oImmob);
			{
				var e = new DataEvent (start, EventType.Input);
				oImmob.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Immobilisations"));
			}

			var o1 = new DataObject ();
			categories.Add (o1);
			{
				var e = new DataEvent (start, EventType.Input);
				o1.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,    "Bâtiments"));
			}

			var o11 = new DataObject ();
			categories.Add (o11);
			{
				var e = new DataEvent (start, EventType.Input);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,    "Immeubles"));
			}

			var o12 = new DataObject ();
			categories.Add (o12);
			{
				var e = new DataEvent (start, EventType.Input);
				o12.AddEvent (e);
				e.AddProperty (new DataStringProperty  (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.GroupParent,      o1.Guid));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,         "Usines"));
			}

			var o121 = new DataObject ();
			categories.Add (o121);
			{
				var e = new DataEvent (start, EventType.Input);
				o121.AddEvent (e);
				e.AddProperty (new DataStringProperty  (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.GroupParent,      o12.Guid));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,         "Suisses"));
			}

			var o122 = new DataObject ();
			categories.Add (o122);
			{
				var e = new DataEvent (start, EventType.Input);
				o122.AddEvent (e);
				e.AddProperty (new DataStringProperty  (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.GroupParent,      o12.Guid));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,         "Etrangères"));
			}

			var o13 = new DataObject ();
			categories.Add (o13);
			{
				var e = new DataEvent (start, EventType.Input);
				o13.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o1.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Entrepôts"));
			}

			///////////////

			var o2 = new DataObject ();
			categories.Add (o2);
			{
				var e = new DataEvent (start, EventType.Input);
				o2.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      oImmob.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Véhicules"));
			}

			var o21 = new DataObject ();
			categories.Add (o21);
			{
				var e = new DataEvent (start, EventType.Input);
				o21.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Camions"));
			}

			var o22 = new DataObject ();
			categories.Add (o22);
			{
				var e = new DataEvent (start, EventType.Input);
				o22.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Camionnettes"));
			}

			var o23 = new DataObject ();
			categories.Add (o23);
			{
				var e = new DataEvent (start, EventType.Input);
				o23.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,      o2.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,         "Voitures"));
			}

			///////////////
			
			var o3 = new DataObject ();
			categories.Add (o3);
			{
				var e = new DataEvent (start, EventType.Input);
				o3.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Secteurs"));
			}

			var o31 = new DataObject ();
			categories.Add (o31);
			{
				var e = new DataEvent (start, EventType.Input);
				o31.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Nord"));
			}

			var o32 = new DataObject ();
			categories.Add (o32);
			{
				var e = new DataEvent (start, EventType.Input);
				o32.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Sud"));
			}

			var o33 = new DataObject ();
			categories.Add (o33);
			{
				var e = new DataEvent (start, EventType.Input);
				o33.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Est"));
			}

			var o34 = new DataObject ();
			categories.Add (o34);
			{
				var e = new DataEvent (start, EventType.Input);
				o34.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o3.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Ouest"));
			}

			///////////////

			var o4 = new DataObject ();
			categories.Add (o4);
			{
				var e = new DataEvent (start, EventType.Input);
				o4.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Centres de frais"));
			}

			var o41 = new DataObject ();
			categories.Add (o41);
			{
				var e = new DataEvent (start, EventType.Input);
				o41.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Atelier"));
			}

			var o42 = new DataObject ();
			categories.Add (o42);
			{
				var e = new DataEvent (start, EventType.Input);
				o42.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Bureaux"));
			}

			var o43 = new DataObject ();
			categories.Add (o43);
			{
				var e = new DataEvent (start, EventType.Input);
				o43.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Distribution"));
			}

			var o44 = new DataObject ();
			categories.Add (o44);
			{
				var e = new DataEvent (start, EventType.Input);
				o44.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Stockage"));
			}

			var o45 = new DataObject ();
			categories.Add (o45);
			{
				var e = new DataEvent (start, EventType.Input);
				o45.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o4.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Transports"));
			}


			///////////////

			var o5 = new DataObject ();
			categories.Add (o5);
			{
				var e = new DataEvent (start, EventType.Input);
				o5.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Responsables"));
			}

			var o51 = new DataObject ();
			categories.Add (o51);
			{
				var e = new DataEvent (start, EventType.Input);
				o51.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Simone"));
			}

			var o52 = new DataObject ();
			categories.Add (o52);
			{
				var e = new DataEvent (start, EventType.Input);
				o52.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Georges-André"));
			}

			var o53 = new DataObject ();
			categories.Add (o53);
			{
				var e = new DataEvent (start, EventType.Input);
				o53.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Béatrice"));
			}

			var o54 = new DataObject ();
			categories.Add (o54);
			{
				var e = new DataEvent (start, EventType.Input);
				o54.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Dominique"));
			}

			var o55 = new DataObject ();
			categories.Add (o55);
			{
				var e = new DataEvent (start, EventType.Input);
				o55.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Joël"));
			}

			var o56 = new DataObject ();
			categories.Add (o56);
			{
				var e = new DataEvent (start, EventType.Input);
				o56.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Paul-Henry"));
			}

			var o57 = new DataObject ();
			categories.Add (o57);
			{
				var e = new DataEvent (start, EventType.Input);
				o57.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Jean-Daniel"));
			}

			var o58 = new DataObject ();
			categories.Add (o58);
			{
				var e = new DataEvent (start, EventType.Input);
				o58.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty (ObjectField.GroupParent, o5.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Sandra"));
			}
			///////////////

			var o6 = new DataObject ();
			categories.Add (o6);
			{
				var e = new DataEvent (start, EventType.Input);
				o6.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent, o0.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name, "Placements"));
			}

			var o61 = new DataObject ();
			categories.Add (o61);
			{
				var e = new DataEvent (start, EventType.Input);
				o61.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Portefeuille d’actions suisses"));
			}

			var o62 = new DataObject ();
			categories.Add (o62);
			{
				var e = new DataEvent (start, EventType.Input);
				o62.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Portefeuille d’actions européennes"));
			}

			var o63 = new DataObject ();
			categories.Add (o63);
			{
				var e = new DataEvent (start, EventType.Input);
				o63.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Portefeuille d’actions nord-américaines"));
			}

			var o64 = new DataObject ();
			categories.Add (o64);
			{
				var e = new DataEvent (start, EventType.Input);
				o64.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Portefeuille d’actions sub-américaines"));
			}

			var o65 = new DataObject ();
			categories.Add (o65);
			{
				var e = new DataEvent (start, EventType.Input);
				o65.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNuméro, (DummyMandat.GroupNumber++).ToString ()));
				e.AddProperty (new DataGuidProperty   (ObjectField.GroupParent,  o6.Guid));
				e.AddProperty (new DataStringProperty (ObjectField.Name,     "Portefeuille d’actions asiatiques"));
			}
		}

		private static void AddAmortissement(DataMandat mandat, string nom, DataEvent e)
		{
			var cat = DummyMandat.GetCategory (mandat, nom);

			if (cat != null)
			{
				var taux   = ObjectCalculator.GetObjectPropertyDecimal (cat, null, ObjectField.AmortizationRate);
				var type   = ObjectCalculator.GetObjectPropertyInt     (cat, null, ObjectField.AmortizationType);
				var period = ObjectCalculator.GetObjectPropertyInt     (cat, null, ObjectField.Periodicity);
				var rest   = ObjectCalculator.GetObjectPropertyDecimal (cat, null, ObjectField.ResidualValue);
				var c1     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte1);
				var c2     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte2);
				var c3     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte3);
				var c4     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte4);
				var c5     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte5);
				var c6     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte6);
				var c7     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte7);
				var c8     = ObjectCalculator.GetObjectPropertyString (cat, null, ObjectField.Compte8);

				e.AddProperty (new DataStringProperty  (ObjectField.CategoryName,      nom));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, taux.GetValueOrDefault ()));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, type.GetValueOrDefault (1)));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       period.GetValueOrDefault (12)));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue,  rest.GetValueOrDefault ()));
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

		private static Guid GetPerson(DataMandat mandat, string text)
		{
			var list = mandat.GetData (BaseType.Persons);

			foreach (var person in list)
			{
				var nom = ObjectCalculator.GetObjectPropertyString (person, null, ObjectField.Name);
				if (nom == text)
				{
					return person.Guid;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("La personne {0} n'existe pas !", text));
			return Guid.Empty;
		}

		private static DataObject GetCategory(DataMandat mandat, string text)
		{
			var list = mandat.GetData (BaseType.Categories);

			foreach (var group in list)
			{
				var nom = ObjectCalculator.GetObjectPropertyString (group, null, ObjectField.Name);
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
				var nom = ObjectCalculator.GetObjectPropertyString (group, null, ObjectField.Name);
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