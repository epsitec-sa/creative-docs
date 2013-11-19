//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public static class StaticDescriptions
	{
		public static string GetObjectPageDescription(EditionObjectPageType type)
		{
			switch (type)
			{
				case EditionObjectPageType.OneShot:
					return "Evénement";

				case EditionObjectPageType.Summary:
					return "Résumé";

				case EditionObjectPageType.Grouping:
				case EditionObjectPageType.Object:
					return "Général";

				case EditionObjectPageType.Values:
					return "Valeurs";

				case EditionObjectPageType.Amortissements:
					return "Amortissements";

				case EditionObjectPageType.Category:
					return "Général";

				case EditionObjectPageType.Compta:
					return "Comptabilisation";

				case EditionObjectPageType.Group:
					return "Général";

				case EditionObjectPageType.Groups:
					return "Regroupements";

				default:
					return null;
			}
		}
	}
}
