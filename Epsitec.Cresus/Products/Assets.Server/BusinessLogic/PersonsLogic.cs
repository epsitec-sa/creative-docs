//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class PersonsLogic
	{
		public static string GetShortName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'une personne, du genre:
			//	"Dupond"
			var obj = accessor.GetObject (BaseType.Persons, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				return ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);
			}
		}


		public static string GetFullName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom complet d'une personne, du genre:
			//	"Jean Dupond Epsitec SA"
			var obj = accessor.GetObject (BaseType.Persons, guid);
			if (obj == null)
			{
				return null;
			}
			else
			{
				var t1 = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Prénom);
				var t2 = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);
				var t3 = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Entreprise);

				return string.Join (" ", t1, t2, t3);
			}
		}
	}
}
