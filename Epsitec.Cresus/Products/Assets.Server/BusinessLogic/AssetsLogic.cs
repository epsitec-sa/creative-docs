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
			if (asset == null)
			{
				return null;
			}

			return ObjectCalculator.GetObjectPropertyString (asset, null, ObjectField.Name);
		}
	}
}
