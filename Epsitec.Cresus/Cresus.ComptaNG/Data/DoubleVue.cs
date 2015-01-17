using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Data
{
	/// <summary>
	/// Données pour une colonne (gauche ou droite) du Bilan ou du PP.
	/// On effectue deux demandes, l'une pour la colonne gauche et l'autre la droite.
	/// Bilan: gauche = actifs,  droite = passifs.
	/// PP:    gauche = charges, droite = produits.
	/// REMARQUE: Trouver un nom meilleur !?
	/// </summary>
	public class DoubleVue : AbstractObjetComptable
	{
		public Compte			Compte;
		public decimal?			Solde;
		public DonnéesDuBudget	DonnéesDuBudget;
	}
}
