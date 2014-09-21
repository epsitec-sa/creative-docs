//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class UniversalLogic
	{
		public static string GetObjectSummary(DataAccessor accessor, BaseType baseType, DataObject obj, Timestamp? timestamp)
		{
			//	Retourne un résumé de n'importe quel objet de n'importe quelle base.
			switch (baseType.Kind)
			{
				case BaseTypeKind.Assets:
					return AssetsLogic.GetSummary (accessor, obj.Guid, timestamp);

				case BaseTypeKind.Categories:
					return CategoriesLogic.GetSummary (accessor, obj.Guid);

				case BaseTypeKind.Groups:
					return GroupsLogic.GetShortName (accessor, obj.Guid);

				case BaseTypeKind.Persons:
					return PersonsLogic.GetSummary (accessor, obj.Guid);

				default:
					return null;
			}
		}

		public static string NiceJoin(params string[] words)
		{
			return string.Join (" — ", words);
		}
	}
}
