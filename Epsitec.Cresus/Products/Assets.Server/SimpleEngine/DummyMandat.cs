//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyMandat
	{
		public static DataMandat GetMandat()
		{
			var mandat = new DataMandat ("Exemple", new System.DateTime (2000, 1, 1), new System.DateTime (2050, 12, 31));

			DummyMandat.AddSettings (mandat);
			DummyAccounts.AddAccounts (mandat);
			DummyPersons.AddPersons (mandat);
			DummyCategories.AddCategories (mandat);
			DummyGroups.AddGroups (mandat);
			DummyMandat.AddObjects (mandat);

			return mandat;
		}


		private static void AddSettings(DataMandat mandat)
		{
			DummyMandat.fieldName        = DummyMandat.AddSettings (mandat, BaseType.Assets, "Nom",              FieldType.String,         180, 380,  1,    1,     0);
			DummyMandat.fieldNumber      = DummyMandat.AddSettings (mandat, BaseType.Assets, "Numéro",           FieldType.String,          90,  90,  1,    null,  0);
			DummyMandat.fieldOwner       = DummyMandat.AddSettings (mandat, BaseType.Assets, "Propriétaire",     FieldType.String,         120, 380,  1,    null,  0);
			DummyMandat.fieldColor       = DummyMandat.AddSettings (mandat, BaseType.Assets, "Couleur",          FieldType.String,         100, 380,  1,    null,  0);
			DummyMandat.fieldSerial      = DummyMandat.AddSettings (mandat, BaseType.Assets, "Numéro de série",  FieldType.String,         150, 380,  1,    null,  0);
			DummyMandat.fieldPerson1     = DummyMandat.AddSettings (mandat, BaseType.Assets, "Responsable",      FieldType.GuidPerson,     150, null, null, null, 10);
			DummyMandat.fieldPerson2     = DummyMandat.AddSettings (mandat, BaseType.Assets, "Fournisseur",      FieldType.GuidPerson,     150, null, null, null,  0);
			DummyMandat.fieldPerson3     = DummyMandat.AddSettings (mandat, BaseType.Assets, "Maintenance",      FieldType.GuidPerson,     150, null, null, null,  0);
			DummyMandat.fieldPerson4     = DummyMandat.AddSettings (mandat, BaseType.Assets, "Concierge",        FieldType.GuidPerson,     150, null, null, null,  0);
			DummyMandat.fieldPerson5     = DummyMandat.AddSettings (mandat, BaseType.Assets, "Conseiller",       FieldType.GuidPerson,     150, null, null, null,  0);
			DummyMandat.fieldValue1      = DummyMandat.AddSettings (mandat, BaseType.Assets, "Valeur assurance", FieldType.ComputedAmount, 110, null, null, null, 10);
			DummyMandat.fieldValue2      = DummyMandat.AddSettings (mandat, BaseType.Assets, "Valeur fiscale",   FieldType.ComputedAmount, 110, null, null, null,  0);
			DummyMandat.fieldAssetDesc   = DummyMandat.AddSettings (mandat, BaseType.Assets, "Description",      FieldType.String,         120, 380,  5,    null, 10);

			DummyPersons.AddSettings (mandat);
		}

		internal static ObjectField AddSettings(DataMandat mandat, BaseType baseType, string name, FieldType type, int columnWidth, int? lineWidth, int? lineCount, int? summaryOrder, int topMargin)
		{
			var field = mandat.Settings.GetNewUserField ();
			mandat.Settings.AddUserField (baseType, new UserField (name, field, type, columnWidth, lineWidth, lineCount, summaryOrder, topMargin));
			return field;
		}


		internal static void AddObjects(DataMandat mandat)
		{
			var objects = mandat.GetData (BaseType.Assets);

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
					e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Immeubles")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+1,   DummyGroups.GetGroup (mandat, "Est")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Bureaux")));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "1110"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Siège social"));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (3000000.0m)));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (2500000.0m)));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Paul"));
					e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "Arnaud")));
					e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson2, DummyPersons.GetPerson (mandat, "Schmidt")));
					e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson4, DummyPersons.GetPerson (mandat, "Bonnard")));
					DummyAmortizations.AddAmortization (mandat, "Bureaux", e);
				}

				for (int i=1; i<13; i++)
				{
					{
						var e = new DataEvent (new Timestamp (new System.DateTime (date2000.Date.Year+i, 12, 31), 0), EventType.AmortizationAuto);
						o111.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 3000000.0m-(i-1)*100000;
						decimal a2 = 3000000.0m-i*100000;
						e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (a1, a2)));
					}
				}
			}

			var o112 = new DataObject ();
			objects.Add (o112);
			{
				{
					var e = new DataEvent (date2002, EventType.Input);
					o112.AddEvent (e);
					e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Immeubles")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+1,   DummyGroups.GetGroup (mandat, "Sud")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Bureaux")));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "1120"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Centre logistique"));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (4550000.0m)));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (6000000.0m)));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Paul"));
					e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "Arnaud")));
					e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Mercier")));
					DummyAmortizations.AddAmortization (mandat, "Bureaux", e);
				}

				for (int i=1; i<10; i++)
				{
					{
						var e = new DataEvent (new Timestamp (new System.DateTime (date2002.Date.Year+i, 12, 31), 0), EventType.AmortizationAuto);
						o112.AddEvent (e);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));

						decimal a1 = 4550000.0m-(i-1)*200000;
						decimal a2 = 4550000.0m-i*200000;
						e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (a1, a2)));
					}
				}
			}

			var o113 = new DataObject ();
			objects.Add (o113);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 4, 10), 0), EventType.Input);
				o113.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Immeubles")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+1,   DummyGroups.GetGroup (mandat, "Est")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Distribution")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "1130"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Centre d'expédition"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (2000000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (3000000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Sandra"));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "André")));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson5, DummyPersons.GetPerson (mandat, "Klein")));
				DummyAmortizations.AddAmortization (mandat, "Bureaux", e);
			}

			var o121 = new DataObject ();
			objects.Add (o121);
			{
				var e = new DataEvent (date2001, EventType.Input);
				o121.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Etrangères")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+1,   DummyGroups.GetGroup (mandat, "Est")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Atelier")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "1210"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Centre d'usinage"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (10400000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (13000000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Ernest"));
				DummyAmortizations.AddAmortization (mandat, "Usines", e);
			}

			var o122 = new DataObject ();
			objects.Add (o122);
			{
				var e = new DataEvent (date2002, EventType.Input);
				o122.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Suisses")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+1,   DummyGroups.GetGroup (mandat, "Nord")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Atelier")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "1220"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Centre d'assemblage"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (8000000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (9500000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "René"));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "Arnaud")));
				DummyAmortizations.AddAmortization (mandat, "Usines", e);
			}

			var o131 = new DataObject ();
			objects.Add (o131);
			{
				{
					var e = new DataEvent (date2002, EventType.Input);
					o131.AddEvent (e);
					e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Entrepôts")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+1,   DummyGroups.GetGroup (mandat, "Nord")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Distribution")));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "1310"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Dépôt principal"));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (2100000.0m)));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (3500000.0m)));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Anne-Sophie"));
					DummyAmortizations.AddAmortization (mandat, "Usines", e);
				}

				{
					var e = new DataEvent (date2010, EventType.Modification);
					o131.AddEvent (e);
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Immeubles")));
				}

				{
					var e = new DataEvent (date2011, EventType.Modification);
					o131.AddEvent (e);
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Suisses")));
				}
			}

			var o132 = new DataObject ();
			objects.Add (o132);
			{
				var e = new DataEvent (date2010, EventType.Input);
				o132.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Entrepôts")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+1,   DummyGroups.GetGroup (mandat, "Nord")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Distribution")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "1320"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Dépôt secondaire"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (5320000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (5000000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Paul"));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "Gardaz")));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "André")));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson4, DummyPersons.GetPerson (mandat, "André")));
				DummyAmortizations.AddAmortization (mandat, "Usines", e);
			}

			var o133 = new DataObject ();
			objects.Add (o133);
			{
				{
					var e = new DataEvent (date2012, EventType.Input);
					o133.AddEvent (e);
					e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Entrepôts", 0.6m)));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+1,   DummyGroups.GetGroup (mandat, "Suisses", 0.4m)));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Sud")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+3,   DummyGroups.GetGroup (mandat, "Atelier")));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "1330"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Centre de recyclage"));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (1200000.0m)));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (1500000.0m)));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Victoria"));
					DummyAmortizations.AddAmortization (mandat, "Usines", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 10, 18), 0), EventType.Modification);
					o133.AddEvent (e);
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Patrick"));
				}

				{
					var e = new DataEvent (date2013, EventType.Modification);
					o133.AddEvent (e);
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Entrepôts", 0.65m)));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+1,   DummyGroups.GetGroup (mandat, "Suisses", 0.35m)));
				}
			}

			var o211 = new DataObject ();
			objects.Add (o211);
			{
				var e = new DataEvent (date2003, EventType.Input);
				o211.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Camions")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2110"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Scania X20"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (150000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (160000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Jean-François"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Blanc"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "25004-800-65210-45R"));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
				DummyAmortizations.AddAmortization (mandat, "Camions", e);
			}

			var o212 = new DataObject ();
			objects.Add (o212);
			{
				var e = new DataEvent (date2003, EventType.Input);
				o212.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Camions")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2120"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Scania X30 semi"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (180000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (200000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Serge"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Rouge"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "25004-800-20087-20X"));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
				DummyAmortizations.AddAmortization (mandat, "Camions", e);
			}

			var o213 = new DataObject ();
			objects.Add (o213);
			{
				{
					var e = new DataEvent (date2000, EventType.Input);
					o213.AddEvent (e);
					e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Camions")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2130"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Volvo T-200"));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (90000.0m)));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (75000.0m)));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Jean-Pierre"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Blanc"));
					e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
					DummyAmortizations.AddAmortization (mandat, "Camions", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2005, 3, 20), 0), EventType.Output);
					o213.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				}
			}

			var o214 = new DataObject ();
			objects.Add (o214);
			{
				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2008, 9, 1), 0), EventType.Input);
					o214.AddEvent (e);
					e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Camions")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2140"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Volvo R-500"));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (110000.0m)));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (120000.0m)));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Olivier"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Jaune/Noir"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "T40-56-200-65E4"));
					e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
					DummyAmortizations.AddAmortization (mandat, "Camions", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 11, 5), 0), EventType.Output);
					o214.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				}
			}

			var o215 = new DataObject ();
			objects.Add (o215);
			{
				var e = new DataEvent (date2011, EventType.Input);
				o215.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Camions")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2150"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Volvo P-810"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (195000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Igor"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Bleu/Noir"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "T40-72-300-PW3B"));
				DummyAmortizations.AddAmortization (mandat, "Camions", e);
			}

			var o221 = new DataObject ();
			objects.Add (o221);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2007, 4, 17), 0), EventType.Input);
				o221.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Camionnettes")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2210"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Renault Doblo"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (25000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (28000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Francine"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Blanc"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "456-321-132-898908"));
				DummyAmortizations.AddAmortization (mandat, "Camionnettes", e);
			}

			var o222 = new DataObject ();
			objects.Add (o222);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2013, 2, 6), 0), EventType.Input);
				o222.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Camionnettes")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2220"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Ford Transit"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (30000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (32000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Jean-Bernard"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Blanc"));
				DummyAmortizations.AddAmortization (mandat, "Camionnettes", e);
			}

			var o231 = new DataObject ();
			objects.Add (o231);
			{
				var e = new DataEvent (date2010, EventType.Input);
				o231.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2310"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Citroën C4 Picasso"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (22000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (25000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Simon"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Noir"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "D456-0003232-0005"));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
				DummyAmortizations.AddAmortization (mandat, "Voitures", e);
			}

			var o232 = new DataObject ();
			objects.Add (o232);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2011, 8, 27), 0), EventType.Input);
				o232.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2320"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Opel Corsa"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (9000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (10000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Frédérique"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Bleu"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "45-3292302-544545-8"));
				DummyAmortizations.AddAmortization (mandat, "Voitures", e);
			}

			var o233 = new DataObject ();
			objects.Add (o233);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2005, 5, 1), 0), EventType.Input);
				o233.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2330"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Fiat Panda"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (8000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (5000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Dominique"));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
				DummyAmortizations.AddAmortization (mandat, "Voitures", e);
			}

			var o234 = new DataObject ();
			objects.Add (o234);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2004, 5, 12), 0), EventType.Input);
				o234.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2340"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Fiat Uno"));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (11000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Denise"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Rouge"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "456000433434002"));
				e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
				DummyAmortizations.AddAmortization (mandat, "Voitures", e);
			}

			var o235 = new DataObject ();
			objects.Add (o235);
			{
				var e = new DataEvent (new Timestamp (new System.DateTime (2011, 2, 1), 0), EventType.Input);
				o235.AddEvent (e);
				e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Voitures")));
				e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2350"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Fiat Uno"));
				e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (12000.0m)));
				e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (13000.0m)));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Marie"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Gris métalisé"));
				e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "780004563233232"));
				DummyAmortizations.AddAmortization (mandat, "Voitures", e);
			}

			var o236 = new DataObject ();
			objects.Add (o236);
			{
				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2002, 11, 19), 0), EventType.Input);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2360"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Toyota Yaris Verso"));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (16000.0m)));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Christiane"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Gris"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "F40T-500023-40232-30987-M"));
					e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
					DummyAmortizations.AddAmortization (mandat, "Voitures", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2003, 5, 1), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (12000.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2003, 5, 1), 1), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (12000.0m, 12500.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2005, 12, 1), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Georges"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2009, 8, 25), 0), EventType.AmortizationExtra);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (16000.0m, 14500.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2009, 8, 25), 1), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (12500.0m, 11000.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2010, 3, 1), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Damien"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 7, 12), 0), EventType.Modification);
					o236.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (12000.0m)));
				}
			}

			var o237 = new DataObject ();
			objects.Add (o237);
			{
				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 3, 10), 0), EventType.Input);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty         (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+0,   DummyGroups.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty      (ObjectField.GroupGuidRatioFirst+2,   DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldNumber,      "2370"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldName,         "Toyota Corolla"));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (5000.0m)));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1,     new ComputedAmount (3500.0m)));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldOwner, "Georges"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldColor,     "Noire"));
					e.AddProperty (new DataStringProperty         (DummyMandat.fieldSerial, "F30T-340407-52118-40720-R"));
					e.AddProperty (new DataGuidProperty           (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
					DummyAmortizations.AddAmortization (mandat, "Voitures", e);
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 7, 1), 0), EventType.AmortizationExtra);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (5000.0m, 5200.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 8, 20), 0), EventType.Modification);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Frédérique"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "F30T-340407-52118-40721-S"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2012, 12, 31), 0), EventType.AmortizationExtra);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (5200.0m, 4600.0m)));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (3500.0m, 2400.0m)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 3, 31), 0), EventType.Modification);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Daniel"));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 4, 14), 0), EventType.Modification);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (2400.0m, 3000.0m, true)));
				}

				{
					var e = new DataEvent (new Timestamp (new System.DateTime (2013, 6, 1), 0), EventType.AmortizationExtra);
					o237.AddEvent (e);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataAmortizedAmountProperty (ObjectField.MainValue, new AmortizedAmount (4600.0m, 2100.0m)));
				}
			}
		}


		private static int EventNumber = 1;

		private static ObjectField fieldName;
		private static ObjectField fieldNumber;
		private static ObjectField fieldAssetDesc;
		private static ObjectField fieldValue1;
		private static ObjectField fieldValue2;
		private static ObjectField fieldOwner;
		private static ObjectField fieldColor;
		private static ObjectField fieldSerial;
		private static ObjectField fieldPerson1;
		private static ObjectField fieldPerson2;
		private static ObjectField fieldPerson3;
		private static ObjectField fieldPerson4;
		private static ObjectField fieldPerson5;
	}
}