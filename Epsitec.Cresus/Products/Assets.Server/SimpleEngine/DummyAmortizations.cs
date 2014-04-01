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
				var c1     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account1);
				var c2     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account2);
				var c3     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account3);
				var c4     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account4);
				var c5     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account5);
				var c6     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account6);
				var c7     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account7);
				var c8     = ObjectProperties.GetObjectPropertyGuid    (cat, null, ObjectField.Account8);

				e.AddProperty (new DataStringProperty  (ObjectField.CategoryName,     nom));
				e.AddProperty (new DataDecimalProperty (ObjectField.AmortizationRate, taux.GetValueOrDefault ()));
				e.AddProperty (new DataIntProperty     (ObjectField.AmortizationType, type.GetValueOrDefault (1)));
				e.AddProperty (new DataIntProperty     (ObjectField.Periodicity,      period.GetValueOrDefault (12)));
				e.AddProperty (new DataIntProperty     (ObjectField.Prorata,          prorat.GetValueOrDefault ()));
				e.AddProperty (new DataDecimalProperty (ObjectField.Round,            round.GetValueOrDefault ()));
				e.AddProperty (new DataDecimalProperty (ObjectField.ResidualValue,    rest.GetValueOrDefault ()));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account1,         c1));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account2,         c2));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account3,         c3));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account4,         c4));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account5,         c5));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account6,         c6));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account7,         c7));
				e.AddProperty (new DataGuidProperty    (ObjectField.Account8,         c8));
			}
		}
	}
}