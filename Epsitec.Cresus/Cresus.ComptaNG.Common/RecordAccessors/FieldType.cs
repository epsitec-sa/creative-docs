using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.ComptaNG.Common.RecordAccessor
{
	/// <summary>
	/// Cette énumération permet de désigner les champs de tous les Record.
	/// </summary>
	public enum FieldType
	{
		//	Champs d'un compte:
		Numéro,
		Titre,
		Commentaire,
		Catégorie,
		Parent,
		Enfants,
		Niveau,
		CodeTvaParDéfaut,
		CodesTvaPossibles,
		CompteOuvBoucl,
		IndexOuvBoucl,
		Monnaie,
		Budgets,

		//	Champs d'une écriture:
		Date,
		Pièce,
		Libellé,

		//	etc.
	}
}
