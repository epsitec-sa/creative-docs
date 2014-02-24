//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class AssetsLogic
	{
		public static string GetShortName(DataAccessor accessor, Guid guid)
		{
			//	Retourne le nom court d'un objet, du genre:
			//	"Toyota Yaris Verso"
			var asset = accessor.GetObject (BaseType.Assets, guid);
			if (asset != null)
			{
				//	On cherche la première rubrique string, qui devrait être
				//	le nom.
				var userField = accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type == FieldType.String)
					.FirstOrDefault ();

				if (!userField.IsEmpty)
				{
					return ObjectProperties.GetObjectPropertyString (asset, null, userField.Field);
				}
			}

			return null;
		}
	}
}
