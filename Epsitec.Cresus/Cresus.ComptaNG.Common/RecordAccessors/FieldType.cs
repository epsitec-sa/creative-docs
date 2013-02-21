using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.ComptaNG.Common.RecordAccessor
{
	public enum FieldType
	{
		Numéro,
		Titre,
		Commentaire,  // utile ?
		Catégorie,

		Parent,
		Enfants,  // redondant, utile ?
		Niveau,  // redondant, utile ?

		CodeTvaParDéfaut,
		CodesTvaPossibles,

		CompteOuvBoucl,
		IndexOuvBoucl,
		Monnaie,
		Budgets,
	}
}
