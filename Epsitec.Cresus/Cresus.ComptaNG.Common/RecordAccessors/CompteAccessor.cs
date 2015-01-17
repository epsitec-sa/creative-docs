using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.RequestData;
using Epsitec.Common.Types;
using Epsitec.Cresus.ComptaNG.Common.Records;

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


		public Compte Compte
		{
			get
			{
				if (this.compte == null)
				{
					this.compte = new Compte ();
					this.TravellingToRecord ();
				}

				return this.compte;
			}
		}

		public override void TravellingToRecord()
		{
			this.compte.Numéro      = this.GetFormattedTextField (FieldType.Numéro);
			this.compte.Titre       = this.GetFormattedTextField (FieldType.Titre);
			this.compte.Commentaire = this.GetFormattedTextField (FieldType.Commentaire);
			// etc.
		}

		public override void RecordToTravelling()
		{
			this.SetFormattedTextField (FieldType.Numéro,      this.compte.Numéro);
			this.SetFormattedTextField (FieldType.Titre,       this.compte.Titre);
			this.SetFormattedTextField (FieldType.Commentaire, this.compte.Commentaire);
			// etc.
		}


		private Compte compte;
	}
}
