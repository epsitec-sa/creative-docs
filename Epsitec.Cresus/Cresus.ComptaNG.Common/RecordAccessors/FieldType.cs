using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.ComptaNG.Common.RecordAccessor
{
	public enum FieldType
	{
		Num�ro,
		Titre,
		Commentaire,  // utile ?
		Cat�gorie,

		Parent,
		Enfants,  // redondant, utile ?
		Niveau,  // redondant, utile ?

		CodeTvaParD�faut,
		CodesTvaPossibles,

		CompteOuvBoucl,
		IndexOuvBoucl,
		Monnaie,
		Budgets,
	}
}
