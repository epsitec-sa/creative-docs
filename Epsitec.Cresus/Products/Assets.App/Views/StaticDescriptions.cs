//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class StaticDescriptions
	{
		public static ViewTypeKind GetViewTypeKind(BaseTypeKind kind)
		{
			switch (kind)
			{
				case BaseTypeKind.Assets:
					return ViewTypeKind.Assets;

				case BaseTypeKind.Entries:
					return ViewTypeKind.Entries;

				case BaseTypeKind.Categories:
					return ViewTypeKind.Categories;

				case BaseTypeKind.Groups:
					return ViewTypeKind.Groups;

				case BaseTypeKind.Persons:
					return ViewTypeKind.Persons;

				default:
					return ViewTypeKind.Unknown;
			}
		}

		public static string GetViewTypeIcon(ViewTypeKind kind)
		{
			switch (kind)
			{
				case ViewTypeKind.Assets:
					return "View.Assets";

				case ViewTypeKind.Amortizations:
					return "View.Amortizations";

				case ViewTypeKind.Entries:
					return "View.Ecritures";

				case ViewTypeKind.Categories:
					return "View.Categories";

				case ViewTypeKind.Groups:
					return "View.Groups";

				case ViewTypeKind.Persons:
					return "View.Persons";

				case ViewTypeKind.Reports:
					return "View.Reports";

				case ViewTypeKind.Warnings:
					return "View.Warnings";

				case ViewTypeKind.AssetsSettings:
					return "View.AssetsSettings";

				case ViewTypeKind.PersonsSettings:
					return "View.PersonsSettings";

				case ViewTypeKind.Accounts:
					return "View.AccountsSettings";

				default:
					return null;
			}
		}

		public static string GetViewTypeDescription(ViewTypeKind kind)
		{
			if (kind == ViewTypeKind.Unknown)
			{
				return null;
			}
			else
			{
				return EnumKeyValues.GetEnumKeyValue (kind).Values.Last ().ToString ();
			}
		}

		public static string GetObjectPageDescription(PageType type)
		{
			if (type == PageType.Unknown)
			{
				return null;
			}
			else
			{
				return EnumKeyValues.GetEnumKeyValue (type).Values.Last ().ToString ();
			}
		}
	}
}
