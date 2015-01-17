using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.ComptaNG.Common.RecordAccessor
{
	/// <summary>
	/// Cette �num�ration permet de d�signer les champs de tous les Record.
	/// </summary>
	public enum FieldType
	{
		//	Champs d'un compte:
		Num�ro,
		Titre,
		Commentaire,
		Cat�gorie,
		Parent,
		Enfants,
		Niveau,
		CodeTvaParD�faut,
		CodesTvaPossibles,
		CompteOuvBoucl,
		IndexOuvBoucl,
		Monnaie,
		Budgets,

		//	Champs d'une �criture:
		Date,
		Pi�ce,
		Libell�,

		//	etc.
	}
}
