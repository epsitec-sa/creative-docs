//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class ArgumentsLogic
	{
		public static string GetSummary(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'une argument d'une méthode d'amortissement.
			var obj = accessor.GetObject (BaseType.Arguments, guid);
			return ArgumentsLogic.GetSummary (obj);
		}

		public static string GetSummary(DataObject obj)
		{
			//	Retourne le nom court d'une argument d'une méthode d'amortissement.
			if (obj == null)
			{
				return null;
			}

			return ObjectProperties.GetObjectPropertyString (obj, null, ObjectField.Name);
		}
	}
}
