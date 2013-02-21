using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Data;

namespace Epsitec.Cresus.ComptaNG.Record
{
	public class CompteRecord : AbstractRecord
	{
		public CompteRecord(AbstractObjetComptable data)
			: base (data)
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

		public override AbstractField GetField(FieldType field)
		{
			switch (field)
			{
				case FieldType.Numéro:
					return new FormattedTextField ()
					{
						Content = this.Data.Numéro,
					};

				case FieldType.Titre:
					return new FormattedTextField ()
					{
						Content = this.Data.Titre,
					};

				case FieldType.Commentaire:
					return new FormattedTextField ()
					{
						Content = this.Data.Commentaire,
					};
			}

			return null;
		}

		public override void SetField(FieldType field, AbstractField content)
		{
			switch (field)
			{
				case FieldType.Numéro:
					this.Data.Numéro = (content as FormattedTextField).Content;
					break;

				case FieldType.Titre:
					this.Data.Titre = (content as FormattedTextField).Content;
					break;

				case FieldType.Commentaire:
					this.Data.Commentaire = (content as FormattedTextField).Content;
					break;
			}
		}

		private Compte Data
		{
			get
			{
				return this.data as Compte;
			}
		}
	}
}
