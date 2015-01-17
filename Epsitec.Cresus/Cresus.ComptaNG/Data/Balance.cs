using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	/// <summary>
	/// Données en lecture seule pour la balance de vérification.
	/// </summary>
	public class Balance : AbstractObjetComptable
	{
		public Compte				Compte;
		public decimal?				MontantAuDébit;
		public decimal?				MontantAuCrédit;
		public decimal?				SoldeDébit;
		public decimal?				SoldeCrédit;
		public DonnéesDuBudget		DonnéesDuBudget;
	}
}
