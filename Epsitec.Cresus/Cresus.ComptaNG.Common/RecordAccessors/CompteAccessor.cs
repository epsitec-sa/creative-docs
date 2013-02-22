using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.RequestData;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.RecordAccessor
{
	public class CompteAccessor : AbstractAccessor
	{
		public CompteAccessor(TravellingRecord serverToClientData)
			: base (serverToClientData)
		{
		}

		public override IEnumerable<FieldType> Fields
		{
			get
			{
				yield return FieldType.Numéro;
				yield return FieldType.Titre;
				yield return FieldType.Commentaire;
				yield return FieldType.Catégorie;

				yield return FieldType.Parent;
				yield return FieldType.Enfants;
				yield return FieldType.Niveau;

				yield return FieldType.CodeTvaParDéfaut;
				yield return FieldType.CodesTvaPossibles;

				yield return FieldType.CompteOuvBoucl;
				yield return FieldType.IndexOuvBoucl;
				yield return FieldType.Monnaie;
				yield return FieldType.Budgets;
			}
		}

	}
}
