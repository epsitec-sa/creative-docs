//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public static class DummyAmortizations
	{
		public static void AddAmortization(DataMandat mandat, string nom, DataEvent e)
		{
			var cat = DummyCategories.GetCategory (mandat, nom);

			if (cat != null)
			{
				var taux   = ObjectProperties.GetObjectPropertyDecimal (cat, null, ObjectField.AmortizationRate);
				var type   = ObjectProperties.GetObjectPropertyInt     (cat, null, ObjectField.AmortizationType);
				var period = ObjectProperties.GetObjectPropertyInt     (cat, null, ObjectField.Periodicity);
				var prorat = ObjectProperties.GetObjectPropertyInt     (cat, null, ObjectField.Prorata);
				var round  = ObjectProperties.GetObjectPropertyDecimal (cat, null, ObjectField.Round);
				var rest   = ObjectProperties.GetObjectPropertyDecimal (cat, null, ObjectField.ResidualValue);
				var c1     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Compte1);
				var c2     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Compte2);
				var c3     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Compte3);
				var c4     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Compte4);
				var c5     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Compte5);
				var c6     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Compte6);
				var c7     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Compte7);
				var c8     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Compte8);

				e.AddProperty (new DataStringProperty  (ObjectField.CategoryName,     nom));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, taux.GetValueOrDefault ()));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, type.GetValueOrDefault (1)));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,      period.GetValueOrDefault (12)));
				e.AddProperty (new DataIntProperty     (ObjectField.Prorata,          prorat.GetValueOrDefault ()));
				e.AddProperty (new DataDecimalProperty (ObjectField.Round,            round.GetValueOrDefault ()));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue,    rest.GetValueOrDefault ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Compte1,          c1));
				e.AddProperty (new DataGuidProperty    (ObjectField.Compte2,          c2));
				e.AddProperty (new DataGuidProperty    (ObjectField.Compte3,          c3));
				e.AddProperty (new DataGuidProperty    (ObjectField.Compte4,          c4));
				e.AddProperty (new DataGuidProperty    (ObjectField.Compte5,          c5));
				e.AddProperty (new DataGuidProperty    (ObjectField.Compte6,          c6));
				e.AddProperty (new DataGuidProperty    (ObjectField.Compte7,          c7));
				e.AddProperty (new DataGuidProperty    (ObjectField.Compte8,          c8));
			}
		}
	}
}