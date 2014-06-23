﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public static class AccountsImportHelpers
	{
		public static AccountsMergeMode GetMode(int rank)
		{
			//	Retourne un format d'après son rang.
			var list = AccountsImportHelpers.AccountsMergeModes.ToArray ();

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
			var list = AccountsImportHelpers.AccountsMergeModes.ToList ();
			return list.IndexOf (mode);
		}

		public static string MultiLabels
		{
			//	Retourne le texte permettant de créer des boutons radios.
			get
			{
				return string.Join ("<br/>", AccountsImportHelpers.AccountsMergeModes.Select (x => AccountsImportHelpers.GetModeName (x)));
			}
		}

		public static string GetModeName(AccountsMergeMode mode)
		{
			//	Retourne le nom en clair d'un format.
			switch (mode)
			{
				case AccountsMergeMode.XferAll:
					return "Remplace le plan comptable actuel par le nouveau";

				case AccountsMergeMode.PriorityNumber:
					return "Priorité aux numéros des comptes";

				case AccountsMergeMode.PriorityTitle:
					return "Priorité aux titres des comptes";

				default:
					throw new System.InvalidOperationException (string.Format ("Invalid mode", mode));
			}
		}

		private static IEnumerable<AccountsMergeMode> AccountsMergeModes
		{
			//	Enumère tous les formats disponibles, par ordre d'importance.
			get
			{
				yield return AccountsMergeMode.XferAll;
				yield return AccountsMergeMode.PriorityNumber;
				yield return AccountsMergeMode.PriorityTitle;
			}
		}
	}
}