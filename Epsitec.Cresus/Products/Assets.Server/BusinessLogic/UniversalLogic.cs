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
		public static string GetObjectSummary(DataAccessor accessor, BaseType baseType, DataObject obj, Timestamp? timestamp = null)
		{
			//	Retourne un résumé de n'importe quel objet de n'importe quelle base.
			if (obj == null)
			{
				return null;
			}
			else
			{
				return UniversalLogic.GetObjectSummary (accessor, baseType, obj.Guid, timestamp);
			}
		}

		public static string GetObjectSummary(DataAccessor accessor, BaseType baseType, Guid guid, Timestamp? timestamp = null)
		{
			//	Retourne un résumé de n'importe quel objet de n'importe quelle base.
			if (!guid.IsEmpty)
			{
				switch (baseType.Kind)
				{
					case BaseTypeKind.Assets:
						return AssetsLogic.GetSummary (accessor, guid, timestamp);

					case BaseTypeKind.Categories:
						return CategoriesLogic.GetSummary (accessor, guid);

					case BaseTypeKind.Groups:
						return GroupsLogic.GetShortName (accessor, guid);

					case BaseTypeKind.Persons:
						return PersonsLogic.GetSummary (accessor, guid);

					case BaseTypeKind.Methods:
						return MethodsLogic.GetSummary (accessor, guid);

					case BaseTypeKind.Arguments:
						return ArgumentsLogic.GetSummary (accessor, guid);
				}
			}

			return null;
		}


		public static string NiceJoin(params string[] words)
		{
			//	Appond plusieurs textes en les séparant par de jolis tirets longs.
			return string.Join (" — ", words.Where (x => !string.IsNullOrEmpty (x)));
		}
	}
}
