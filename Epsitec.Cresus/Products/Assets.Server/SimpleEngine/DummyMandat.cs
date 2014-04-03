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
			return new DataMandat ("Exemple", new System.DateTime (2000, 1, 1), new System.DateTime (2050, 12, 31));
		}

		public static void AddDummyData(DataAccessor accessor)
		{
			DummyMandat.Accessor = accessor;

			DummyMandat.AddSettings (accessor.Mandat);
			DummyAccounts.AddAccounts (accessor.Mandat);
			DummyEntries.AddEntries (accessor.Mandat);
			DummyPersons.AddPersons (accessor.Mandat);
			DummyCategories.AddCategories (accessor.Mandat);
			DummyGroups.AddGroups (accessor.Mandat);
			DummyMandat.AddObjects (accessor.Mandat);
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

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2000, 1, 1), "Siège social", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					{
						var e = o.GetEvent (0);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Bâtiments")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+1, DummyGroups.GetGroup (mandat, "Est")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Bureaux")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "1110"));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (2500000.0m)));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Paul"));
						e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "Arnaud")));
						e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson2, DummyPersons.GetPerson (mandat, "Schmidt")));
						e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson4, DummyPersons.GetPerson (mandat, "Bonnard")));
						DummyAmortizations.AddAmortization (mandat, "Bureaux", e);
						DummyMandat.NewAmortizedAmount (o, e, 3000000.0m);
					}

					for (int i=1; i<13; i++)
					{
						{
							var e = DummyMandat.Accessor.CreateAssetEvent(o, new System.DateTime (2000+i, 12, 31), EventType.AmortizationAuto);
							e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));

							decimal a1 = 3000000.0m-(i-1)*100000;
							decimal a2 = 3000000.0m-i*100000;
							DummyMandat.NewAmortizedAmount (o, e, a1, a2);
						}
					}

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2002, 1, 1), "Centre logistique", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					{
						var e = o.GetEvent (0);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Bâtiments")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+1, DummyGroups.GetGroup (mandat, "Sud")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Bureaux")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "1120"));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (6000000.0m)));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Paul"));
						e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "Arnaud")));
						e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Mercier")));
						DummyAmortizations.AddAmortization (mandat, "Bureaux", e);
						DummyMandat.NewAmortizedAmount (o, e, 4550000.0m);
					}

					for (int i=1; i<10; i++)
					{
						{
							var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2002+i, 12, 31), EventType.AmortizationAuto);
							e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));

							decimal a1 = 4550000.0m-(i-1)*200000;
							decimal a2 = 4550000.0m-i*200000;
							DummyMandat.NewAmortizedAmount (o, e, a1, a2);
						}
					}

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2003, 4, 10), "Centre d'expédition", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Bâtiments")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+1, DummyGroups.GetGroup (mandat, "Est")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Distribution")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "1130"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (3000000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Sandra"));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "André")));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson5, DummyPersons.GetPerson (mandat, "Klein")));
					DummyAmortizations.AddAmortization (mandat, "Bureaux", e);
					DummyMandat.NewAmortizedAmount (o, e, 2000000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2001, 3, 20), "Centre d'usinage", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Bâtiments")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+1, DummyGroups.GetGroup (mandat, "Est")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Atelier")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "1210"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (13000000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Ernest"));
					DummyAmortizations.AddAmortization (mandat, "Usines", e);
					DummyMandat.NewAmortizedAmount (o, e, 10400000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2002, 1, 1), "Centre d'assemblage", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Bâtiments")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+1, DummyGroups.GetGroup (mandat, "Nord")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Atelier")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "1220"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (9500000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "René"));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "Arnaud")));
					DummyAmortizations.AddAmortization (mandat, "Usines", e);
					DummyMandat.NewAmortizedAmount (o, e, 8000000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2002, 1, 1), "Dépôt principal", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					{
						var e = o.GetEvent (0);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Entrepôts")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+1, DummyGroups.GetGroup (mandat, "Nord")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Distribution")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "1310"));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (3500000.0m)));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Anne-Sophie"));
						DummyAmortizations.AddAmortization (mandat, "Usines", e);
						DummyMandat.NewAmortizedAmount (o, e, 2100000.0m);
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2010, 2, 5), EventType.Modification);
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Bâtiments")));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2011, 1, 10), EventType.Modification);
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Entrepôts")));
					}

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2010, 3, 31), "Dépôt secondaire", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Entrepôts")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+1, DummyGroups.GetGroup (mandat, "Nord")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Distribution")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "1320"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (5000000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Paul"));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson1, DummyPersons.GetPerson (mandat, "Gardaz")));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "André")));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson4, DummyPersons.GetPerson (mandat, "André")));
					DummyAmortizations.AddAmortization (mandat, "Usines", e);
					DummyMandat.NewAmortizedAmount (o, e, 5320000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2012, 1, 1), "Centre de recyclage", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					{
						var e = o.GetEvent (0);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Entrepôts", 0.6m)));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+1, DummyGroups.GetGroup (mandat, "Bâtiments", 0.4m)));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Sud")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+3, DummyGroups.GetGroup (mandat, "Atelier")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "1330"));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (1500000.0m)));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Victoria"));
						DummyAmortizations.AddAmortization (mandat, "Usines", e);
						DummyMandat.NewAmortizedAmount (o, e, 1200000.0m);
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2012, 10, 18), EventType.Modification);
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Patrick"));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2013, 1, 1), EventType.Modification);
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Entrepôts", 0.65m)));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+1, DummyGroups.GetGroup (mandat, "Bâtiments", 0.35m)));
					}

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2003, 1, 1), "Scania X20", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Camions")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2110"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (160000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Jean-François"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Blanc"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "25004-800-65210-45R"));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
					DummyAmortizations.AddAmortization (mandat, "Camions", e);
					DummyMandat.NewAmortizedAmount (o, e, 150000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2003, 1, 1), "Scania X30 semi", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Camions")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2120"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (200000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Serge"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Rouge"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "25004-800-20087-20X"));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
					DummyAmortizations.AddAmortization (mandat, "Camions", e);
					DummyMandat.NewAmortizedAmount (o, e, 180000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2000, 1, 1), "Volvo T-200", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					{
						var e = o.GetEvent (0);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Camions")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2130"));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (75000.0m)));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Jean-Pierre"));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Blanc"));
						e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
						DummyAmortizations.AddAmortization (mandat, "Camions", e);
						DummyMandat.NewAmortizedAmount (o, e, 90000.0m);
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2005, 3, 20), EventType.Output);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						DummyMandat.NewAmortizedAmount (o, e, 50000.0m);
					}

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2008, 9, 1), "Volvo R-500", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					{
						var e = o.GetEvent (0);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Camions")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2140"));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (120000.0m)));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Olivier"));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Jaune/Noir"));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "T40-56-200-65E4"));
						e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
						DummyAmortizations.AddAmortization (mandat, "Camions", e);
						DummyMandat.NewAmortizedAmount (o, e, 110000.0m);
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2012, 11, 5), EventType.Output);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						DummyMandat.NewAmortizedAmount (o, e, 95000.0m);
					}

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2011, 1, 1), "Volvo P-810", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Camions")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2150"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Igor"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Bleu/Noir"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "T40-72-300-PW3B"));
					DummyAmortizations.AddAmortization (mandat, "Camions", e);
					DummyMandat.NewAmortizedAmount (o, e, 195000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2007, 4, 17), "Renault Doblo", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Camionnettes")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2210"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (28000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Francine"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Blanc"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "456-321-132-898908"));
					DummyAmortizations.AddAmortization (mandat, "Camionnettes", e);
					DummyMandat.NewAmortizedAmount (o, e, 25000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2013, 2, 6), "Ford Transit", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Camionnettes")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2220"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (32000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Jean-Bernard"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Blanc"));
					DummyAmortizations.AddAmortization (mandat, "Camionnettes", e);
					DummyMandat.NewAmortizedAmount (o, e, 30000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2010, 1, 1), "Citroën C4 Picasso", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2310"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (25000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Simon"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Noir"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "D456-0003232-0005"));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
					DummyAmortizations.AddAmortization (mandat, "Voitures", e);
					DummyMandat.NewAmortizedAmount (o, e, 22000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2011, 8, 27), "Opel Corsa", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2320"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (10000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Frédérique"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Bleu"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "45-3292302-544545-8"));
					DummyAmortizations.AddAmortization (mandat, "Voitures", e);
					DummyMandat.NewAmortizedAmount (o, e, 9000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2005, 5, 1), "Fiat Panda", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2330"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (5000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Dominique"));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
					DummyAmortizations.AddAmortization (mandat, "Voitures", e);
					DummyMandat.NewAmortizedAmount (o, e, 8000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2004, 5, 12), "Fiat Uno", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2340"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Denise"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Rouge"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "456000433434002"));
					e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
					DummyAmortizations.AddAmortization (mandat, "Voitures", e);
					DummyMandat.NewAmortizedAmount (o, e, 11000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2011, 2, 1), "Fiat Uno", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					var e = o.GetEvent (0);
					e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Voitures")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
					e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2350"));
					e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (13000.0m)));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Marie"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Gris métalisé"));
					e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "780004563233232"));
					DummyAmortizations.AddAmortization (mandat, "Voitures", e);
					DummyMandat.NewAmortizedAmount (o, e, 12000.0m);

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2002, 11, 19), "Toyota Yaris Verso", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					{
						var e = o.GetEvent (0);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Voitures")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2360"));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Christiane"));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Gris"));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "F40T-500023-40232-30987-M"));
						e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
						DummyAmortizations.AddAmortization (mandat, "Voitures", e);
						DummyMandat.NewAmortizedAmount (o, e, 16000.0m);
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2003, 5, 1), EventType.Modification);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (12000.0m)));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2003, 5, 1), EventType.Modification);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (12000.0m, 12500.0m)));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2005, 12, 1), EventType.Modification);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Georges"));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2009, 8, 25), EventType.AmortizationExtra);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						DummyMandat.NewAmortizedAmount (o, e, 16000.0m, 14500.0m);
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2009, 8, 25), EventType.Modification);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (12500.0m, 11000.0m)));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2010, 3, 1), EventType.Modification);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Damien"));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2012, 7, 12), EventType.MainValue);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						DummyMandat.NewAmortizedAmount (o, e, 12000.0m);
					}

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}

			{
				var guid = DummyMandat.Accessor.CreateObject (BaseType.Assets, new System.DateTime (2012, 3, 10), "Toyota Corolla", Guid.Empty);
				var o = DummyMandat.Accessor.GetObject (BaseType.Assets, guid);
				{
					{
						var e = o.GetEvent (0);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+0, DummyGroups.GetGroup (mandat, "Voitures")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+2, DummyGroups.GetGroup (mandat, "Transports")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+5, DummyGroups.GetGroup (mandat, "Immobilisations corporelles")));
						e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+6, DummyGroups.GetGroup (mandat, "Administratif")));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldNumber, "2370"));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (3500.0m)));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Georges"));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldColor, "Noire"));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "F30T-340407-52118-40720-R"));
						e.AddProperty (new DataGuidProperty (DummyMandat.fieldPerson3, DummyPersons.GetPerson (mandat, "Frutiger")));
						DummyAmortizations.AddAmortization (mandat, "Voitures", e);
						DummyMandat.NewAmortizedAmount (o, e, 5000.0m);
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2012, 7, 1), EventType.AmortizationExtra);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						DummyMandat.NewAmortizedAmount (o, e, 5000.0m, 5200.0m);
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2012, 8, 20), EventType.Modification);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Frédérique"));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldSerial, "F30T-340407-52118-40721-S"));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2012, 12, 31), EventType.MainValue);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						DummyMandat.NewAmortizedAmount (o, e, 4600.0m);
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2012, 12, 31), EventType.Modification);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (3500.0m, 2400.0m)));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2013, 3, 31), EventType.Modification);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataStringProperty (DummyMandat.fieldOwner, "Daniel"));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2013, 4, 14), EventType.Modification);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						e.AddProperty (new DataComputedAmountProperty (DummyMandat.fieldValue1, new ComputedAmount (2400.0m, 3000.0m, true)));
					}

					{
						var e = DummyMandat.Accessor.CreateAssetEvent (o, new System.DateTime (2013, 6, 1), EventType.AmortizationExtra);
						e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyMandat.EventNumber++).ToString ()));
						DummyMandat.NewAmortizedAmount (o, e, 4600.0m, 2100.0m);
					}

					Amortizations.UpdateAmounts (DummyMandat.Accessor, o);
				}
			}
		}


		private static void NewAmortizedAmount(DataObject obj, DataEvent e, decimal value)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
			var aa = p.Value;

			aa.InitialAmount = value;
		}

		private static void NewAmortizedAmount(DataObject obj, DataEvent e, decimal initialAmount, decimal finalAmount)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
			var aa = p.Value;

			aa.AmortizationType = AmortizationType.Degressive;
			aa.InitialAmount = initialAmount;
			aa.BaseAmount = initialAmount;
			aa.EffectiveRate = 1.0m - (finalAmount / initialAmount);
		}


		private static DataAccessor Accessor;
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