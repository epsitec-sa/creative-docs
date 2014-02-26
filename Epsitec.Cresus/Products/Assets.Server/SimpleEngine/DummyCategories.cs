﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyCategories
	{
		public static void AddCategories(DataMandat mandat)
		{
			var categories = mandat.GetData (BaseType.Categories);

			var start  = new Timestamp (new System.DateTime (2013, 1, 1), 0);

			var o11 = new DataObject ();
			categories.Add (o11);
			{
				var e = new DataEvent (start, EventType.Input);
				o11.AddEvent (e);
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyCategories.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "11"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Bureaux"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.1m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Linear));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Annual));
				e.AddProperty (new DataIntProperty     (ObjectField.Prorata,           (int) ProrataType.Prorata365));
				e.AddProperty (new DataDecimalProperty (ObjectField.Round, 1000.0m));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue, 1000.0m));

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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyCategories.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "12"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Usines"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.12m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Linear));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Annual));
				e.AddProperty (new DataIntProperty     (ObjectField.Prorata,           (int) ProrataType.Prorata365));
				e.AddProperty (new DataDecimalProperty (ObjectField.Round, 1000.0m));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue, 10000.0m));

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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyCategories.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "21"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Camions"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.15m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Degressive));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Trimestrial));
				e.AddProperty (new DataIntProperty     (ObjectField.Prorata,           (int) ProrataType.Prorata365));
				e.AddProperty (new DataDecimalProperty (ObjectField.Round, 1.0m));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue, 100.0m));

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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyCategories.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "22"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Camionnettes"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.21m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Degressive));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Semestrial));
				e.AddProperty (new DataIntProperty     (ObjectField.Prorata,           (int) ProrataType.Prorata365));
				e.AddProperty (new DataDecimalProperty (ObjectField.Round, 1.0m));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue, 100.0m));

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
				e.AddProperty (new DataStringProperty (ObjectField.OneShotNumber, (DummyCategories.CategoryNumber++).ToString ()));
				e.AddProperty (new DataStringProperty  (ObjectField.Number,            "23"));
				e.AddProperty (new DataStringProperty  (ObjectField.Name,               "Voitures"));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, 0.25m));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, (int) AmortizationType.Degressive));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,       (int) Periodicity.Semestrial));
				e.AddProperty (new DataIntProperty     (ObjectField.Prorata,           (int) ProrataType.Prorata365));
				e.AddProperty (new DataDecimalProperty (ObjectField.Round, 1.0m));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue, 100.0m));

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

		public static DataObject GetCategory(DataMandat mandat, string text)
		{
			var list = mandat.GetData (BaseType.Categories);

			foreach (var group in list)
			{
				var nom = ObjectProperties.GetObjectPropertyString (group, null, ObjectField.Name);
				if (nom == text)
				{
					return group;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("La catégorie {0} n'existe pas !", text));
			return null;
		}

		private static int CategoryNumber = 1;
	}
}