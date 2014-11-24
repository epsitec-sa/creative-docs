//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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

				case BaseTypeKind.AssetsUserFields:
					return ViewTypeKind.AssetsSettings;

				case BaseTypeKind.PersonsUserFields:
					return ViewTypeKind.PersonsSettings;

				case BaseTypeKind.Accounts:
					return ViewTypeKind.Accounts;

				case BaseTypeKind.Methods:
					return ViewTypeKind.Methods;

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
					return "View.Entries";

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
					return "View.Accounts";

				case ViewTypeKind.Methods:
					return "View.Methods";

				default:
					return null;
			}
		}

		public static string GetViewModeIcon(ViewMode mode)
		{
			switch (mode)
			{
				case ViewMode.Single:
					return "Show.TimelineSingle";

				case ViewMode.Event:
					return "Show.TimelineEvent";

				case ViewMode.Multiple:
					return "Show.TimelineMultiple";

				default:
					return null;
			}
		}

		public static string GetViewTypeDescription(ViewTypeKind kind)
		{
			switch (kind)
			{
				case ViewTypeKind.Assets:
					return Res.Strings.Enum.ViewTypeKind.Assets.ToString ();

				case ViewTypeKind.Amortizations:
					return Res.Strings.Enum.ViewTypeKind.Amortizations.ToString ();

				case ViewTypeKind.Entries:
					return Res.Strings.Enum.ViewTypeKind.Entries.ToString ();

				case ViewTypeKind.Categories:
					return Res.Strings.Enum.ViewTypeKind.Categories.ToString ();

				case ViewTypeKind.Groups:
					return Res.Strings.Enum.ViewTypeKind.Groups.ToString ();

				case ViewTypeKind.Persons:
					return Res.Strings.Enum.ViewTypeKind.Persons.ToString ();

				case ViewTypeKind.Reports:
					return Res.Strings.Enum.ViewTypeKind.Reports.ToString ();

				case ViewTypeKind.Warnings:
					return Res.Strings.Enum.ViewTypeKind.Warnings.ToString ();

				case ViewTypeKind.AssetsSettings:
					return Res.Strings.Enum.ViewTypeKind.AssetsSettings.ToString ();

				case ViewTypeKind.PersonsSettings:
					return Res.Strings.Enum.ViewTypeKind.PersonsSettings.ToString ();

				case ViewTypeKind.Accounts:
					return Res.Strings.Enum.ViewTypeKind.Accounts.ToString ();

				case ViewTypeKind.Methods:
					return Res.Strings.Enum.ViewTypeKind.Methods.ToString ();

				default:
					return null;
			}
		}

		public static string GetObjectPageDescription(PageType type)
		{
			switch (type)
			{
				case PageType.OneShot:
					return Res.Strings.Enum.PageType.OneShot.ToString ();

				case PageType.Summary:
					return Res.Strings.Enum.PageType.Summary.ToString ();

				case PageType.Asset:
					return Res.Strings.Enum.PageType.Asset.ToString ();

				case PageType.AmortizationDefinition:
					return Res.Strings.Enum.PageType.AmortizationDefinition.ToString ();

				case PageType.AmortizationValue:
					return Res.Strings.Enum.PageType.AmortizationValue.ToString ();

				case PageType.Groups:
					return Res.Strings.Enum.PageType.Groups.ToString ();

				case PageType.Category:
					return Res.Strings.Enum.PageType.Category.ToString ();

				case PageType.Group:
					return Res.Strings.Enum.PageType.Group.ToString ();

				case PageType.Person:
					return Res.Strings.Enum.PageType.Person.ToString ();

				case PageType.UserFields:
					return Res.Strings.Enum.PageType.UserFields.ToString ();

				case PageType.Account:
					return Res.Strings.Enum.PageType.Account.ToString ();

				case PageType.Method:
					return Res.Strings.Enum.PageType.Method.ToString ();

				default:
					return null;
			}
		}
	}
}
