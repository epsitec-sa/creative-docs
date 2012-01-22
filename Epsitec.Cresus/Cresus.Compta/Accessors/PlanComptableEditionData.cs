//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour le plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableEditionData : AbstractEditionData
	{
		public PlanComptableEditionData(ComptaEntity compta)
			: base (compta)
		{
		}


		public override void Validate(ColumnType columnType)
		{
			//	Valide le contenu d'une colonne, en adaptant éventuellement son contenu.
			var text = this.GetText (columnType);
			var error = FormattedText.Null;

			switch (columnType)
			{
				case ColumnType.Numéro:
					error = this.ValidateNuméro (ref text);
					break;

				case ColumnType.Titre:
					error = this.ValidateTitre (ref text);
					break;

				case ColumnType.Catégorie:
					error = this.ValidateCatégorie (ref text);
					break;

				case ColumnType.Type:
					error = this.ValidateType (ref text);
					break;

				case ColumnType.Groupe:
					error = this.ValidateGroupe (ref text);
					break;

				case ColumnType.TVA:
					error = this.ValidateTVA (ref text);
					break;

				case ColumnType.CompteOuvBoucl:
					error = this.ValidateCompteOuvBoucl (ref text);
					break;

				case ColumnType.IndexOuvBoucl:
					error = this.ValidateIndexOuvBoucl (ref text);
					break;
			}

			this.SetText (columnType, text);
			this.errors[columnType] = error;
		}


		#region Validators
		private FormattedText ValidateNuméro(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le numéro du compte";
			}

			if (text.ToSimpleText ().Contains (' '))
			{
				return "Le numéro du compte ne peut pas contenir d'espace";
			}

			var t = text;
			var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == t).FirstOrDefault ();
			if (compte == null)
			{
				return FormattedText.Empty;
			}

#if false
			var himself = (this.footerController.JustCreate) ? null : this.arrayController.SelectedData as PlanComptableData;
			if (himself != null && himself.Numéro == text)
			{
				return FormattedText.Empty;
			}

			return "Ce numéro de compte existe déjà";
#else
			return FormattedText.Empty;
#endif
		}

		private FormattedText ValidateTitre(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le titre du compte";
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		private FormattedText ValidateCatégorie(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque la catégorie du compte";
			}

			CatégorieDeCompte catégorie;
			if (PlanComptableDataAccessor.TextToCatégorie (text, out catégorie))
			{
				return FormattedText.Empty;
			}
			else
			{
				return "La catégorie du compte n'est pas correcte";
			}
		}

		private FormattedText ValidateType(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le type du compte";
			}

			TypeDeCompte type;
			if (PlanComptableDataAccessor.TextToType (text, out type))
			{
				return FormattedText.Empty;
			}
			else
			{
				return "Le type du compte n'est pas correct";
			}
		}

		private FormattedText ValidateTVA(ref FormattedText text)
		{
			return FormattedText.Empty;  //?
#if false
			if (text.IsNullOrEmpty)
			{
				return "Il manque la TVA du compte";
			}

			VatCode tva;
			if (PlanComptableDataAccessor.TextToTVA (text, out tva))
			{
				return FormattedText.Empty;
			}
			else
			{
				return "La TVA du compte n'est pas correcte";
			}
#endif
		}

		private FormattedText ValidateGroupe(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return FormattedText.Empty;
			}

			var n = PlanComptableDataAccessor.GetCompteNuméro (text);
			var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();
			if (compte == null)
			{
				return "Ce compte n'existe pas";
			}

			if (compte.Type != TypeDeCompte.Groupe)
			{
				return "Ce n'est pas un compte de groupement";
			}

			text = n;
			return FormattedText.Empty;
		}

		private FormattedText ValidateCompteOuvBoucl(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return FormattedText.Empty;
			}

			var n = PlanComptableDataAccessor.GetCompteNuméro (text);
			var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();
			if (compte == null)
			{
				return "Ce compte n'existe pas";
			}

			if (compte.Type != TypeDeCompte.Normal)
			{
				return "Ce compte n'a pas le type \"Normal\"";
			}

			if (compte.Catégorie != CatégorieDeCompte.Exploitation)
			{
				return "Ce n'est pas un compte d'exploitation";
			}

			text = n;
			return FormattedText.Empty;
		}

		private FormattedText ValidateIndexOuvBoucl(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return FormattedText.Empty;
			}

			int n;
			if (int.TryParse (text.ToSimpleText (), out n))
			{
				if (n >= 1 && n <= 9)
				{
					return FormattedText.Empty;
				}
			}

			return "Vous devez donner un numéro d'ordre compris entre 1 et 9";
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var compte = entity as ComptaCompteEntity;

			this.SetText (ColumnType.Numéro,         compte.Numéro);
			this.SetText (ColumnType.Titre,          compte.Titre);
			this.SetText (ColumnType.Catégorie,      PlanComptableDataAccessor.CatégorieToText (compte.Catégorie));
			this.SetText (ColumnType.Type,           PlanComptableDataAccessor.TypeToText (compte.Type));
			this.SetText (ColumnType.Groupe,         PlanComptableDataAccessor.GetNuméro (compte.Groupe));
//			this.SetText (ColumnType.TVA,            PlanComptableAccessor.TVAToText (compte.TVA));
			this.SetText (ColumnType.CompteOuvBoucl, PlanComptableDataAccessor.GetNuméro (compte.CompteOuvBoucl));
			this.SetText (ColumnType.IndexOuvBoucl,  (compte.IndexOuvBoucl == 0) ? FormattedText.Empty : compte.IndexOuvBoucl.ToString ());
			this.SetText (ColumnType.Monnaie,        compte.Monnaie);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var compte = entity as ComptaCompteEntity;

			compte.Numéro = this.GetText (ColumnType.Numéro);
			compte.Titre  = this.GetText (ColumnType.Titre);

			CatégorieDeCompte catégorie;
			if (PlanComptableDataAccessor.TextToCatégorie (this.GetText (ColumnType.Catégorie), out catégorie))
			{
				compte.Catégorie = catégorie;
			}

			TypeDeCompte type;
			if (PlanComptableDataAccessor.TextToType (this.GetText (ColumnType.Type), out type))
			{
				compte.Type = type;
			}

			compte.Groupe = PlanComptableDataAccessor.GetCompte (this.comptaEntity, this.GetText (ColumnType.Groupe));

#if false
			VatCode tva;
			if (PlanComptableDataAccessor.TextToTVA (this.GetText (ColumnType.TVA), out tva))
			{
//				compte.TVA = tva;
			}
#endif

			compte.CompteOuvBoucl = PlanComptableDataAccessor.GetCompte (this.comptaEntity, this.GetText (ColumnType.CompteOuvBoucl));

			int index;
			if (int.TryParse (this.GetText (ColumnType.IndexOuvBoucl).ToSimpleText (), out index) && index >= 1 && index <= 9)
			{
				compte.IndexOuvBoucl = index;
			}
			else
			{
				compte.IndexOuvBoucl = 0;
			}

			compte.Monnaie = this.GetText (ColumnType.Monnaie).ToSimpleText ();
		}
	}
}