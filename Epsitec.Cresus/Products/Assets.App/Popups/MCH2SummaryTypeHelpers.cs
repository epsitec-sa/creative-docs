//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Assets.Data.Reports;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public static class MCH2SummaryTypeHelpers
	{
		public static string Labels
		{
			//	Retourne le texte pour peupler le ComboStackedController.
			get
			{
				return string.Join ("<br/>", MCH2SummaryTypeHelpers.Types.Select (x => MCH2SummaryTypeHelpers.GetTypeName (x)));
			}
		}

		public static MCH2SummaryType IntToType(int? value)
		{
			if (value.HasValue)
			{
				var e = MCH2SummaryTypeHelpers.Types.ToArray ();

				if (value >= 0 && value < e.Length)
				{
					return e[value.Value];
				}
			}

			return MCH2SummaryType.Indirect;
		}

		public static int? TypeToInt(MCH2SummaryType type)
		{
			int value = 0;

			foreach (var e in MCH2SummaryTypeHelpers.Types)
			{
				if (e == type)
				{
					return value;
				}

				value++;
			}

			return null;
		}


		private static string GetTypeName(MCH2SummaryType type)
		{
			if (type == MCH2SummaryType.Direct)
			{
				return Res.Strings.MCH2SummaryType.Direct.ToString ();
			}
			else if (type == MCH2SummaryType.Indirect)
			{
				return Res.Strings.MCH2SummaryType.Indirect.ToString ();
			}
			else
			{
				return null;
			}
		}

		private static IEnumerable<MCH2SummaryType> Types
		{
			//	Retourne la liste des types supportés, dans l'ordre où ils apparaissent dans la UI.
			get
			{
				yield return MCH2SummaryType.Direct;
				yield return MCH2SummaryType.Indirect;
			}
		}
	}
}