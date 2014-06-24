//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public static class AccountsMergeModeHelpers
	{
		public static AccountsMergeMode GetMode(int rank)
		{
			//	Retourne un format d'après son rang.
			var list = AccountsMergeModeHelpers.AccountsMergeModes.ToArray ();

			if (rank >= 0 && rank < list.Length)
			{
				return list[rank];
			}
			else
			{
				return AccountsMergeMode.Unknown;
			}
		}

		public static int GetRank(AccountsMergeMode mode)
		{
			//	Retourne le rang d'un format, ou -1.
			var list = AccountsMergeModeHelpers.AccountsMergeModes.ToList ();
			return list.IndexOf (mode);
		}

		public static string MultiLabels
		{
			//	Retourne le texte permettant de créer des boutons radios.
			get
			{
				return string.Join ("<br/>", AccountsMergeModeHelpers.AccountsMergeModes.Select (x => AccountsMergeModeHelpers.GetModeName (x)));
			}
		}

		public static string GetModeName(AccountsMergeMode mode)
		{
			//	Retourne le nom en clair d'un format.
			switch (mode)
			{
				case AccountsMergeMode.Xfer:
					return "Remplacer";

				case AccountsMergeMode.Merge:
					return "Fusionner";

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid mode", mode));
			}
		}

		private static IEnumerable<AccountsMergeMode> AccountsMergeModes
		{
			//	Enumère tous les formats disponibles, par ordre d'importance.
			get
			{
				yield return AccountsMergeMode.Xfer;
				yield return AccountsMergeMode.Merge;
			}
		}
	}
}