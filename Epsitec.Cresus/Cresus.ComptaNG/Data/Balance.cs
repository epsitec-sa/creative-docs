using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	/// <summary>
	/// Données en lecture seule pour la balance de vérification.
	/// QUESTION: Faut-il mettre les informations sur le compte explicitement
	/// (Niveau, Numéro, Titre et Catégorie) ou mettre un pointeur sur le compte ?
	/// </summary>
	public class Balance : AbstractObjetComptable
	{
		public int					Niveau;
		public FormattedText		Numéro;
		public FormattedText		Titre;
		public CatégorieDeCompte	Catégorie;

		public decimal?				MontantAuDébit;
		public decimal?				MontantAuCrédit;
		public decimal?				SoldeDébit;
		public decimal?				SoldeCrédit;
		public decimal?				PériodePrécédente;
		public decimal?				PériodePénultième;
		public decimal?				Budget;
		public decimal?				BudgetProrata;
		public decimal?				BudgetFutur;
		public decimal?				BudgetFuturProrata;
	}
}
