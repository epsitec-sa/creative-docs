//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Compta.Accessors
{
	public enum ColumnType
	{
		None,

		Date,
		Débit,
		Crédit,
		SoldeDébit,
		SoldeCrédit,
		Pièce,
		Libellé,
		Titre,
		Montant,
		Solde,
		Numéro,
		CP,
		Catégorie,
		Type,
		Groupe,
		TVA,
		CompteOuvBoucl,
		IndexOuvBoucl,
		Monnaie,
		TotalAutomatique,
		SoldeGraphique,
		Budget,
		BudgetPrécédent,
		BudgetFutur,
		Journal,

		//---------------------

		NuméroGauche,
		TitreGauche,
		SoldeGauche,
		SoldeGraphiqueGauche,
		BudgetGauche,

		Espace,
		
		NuméroDroite,
		TitreDroite,
		SoldeDroite,
		SoldeGraphiqueDroite,
		BudgetDroite,
	}
}
