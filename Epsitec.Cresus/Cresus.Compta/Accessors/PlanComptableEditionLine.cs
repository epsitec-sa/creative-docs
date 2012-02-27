//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour le plan comptable de la comptabilité.
	/// </summary>
	public class PlanComptableEditionLine : AbstractEditionLine
	{
		public PlanComptableEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Numéro,         new EditionData (this.controller, this.ValidateNuméro));
			this.datas.Add (ColumnType.Titre,          new EditionData (this.controller, this.ValidateTitre));
			this.datas.Add (ColumnType.Catégorie,      new EditionData (this.controller, this.ValidateCatégorie));
			this.datas.Add (ColumnType.Type,           new EditionData (this.controller, this.ValidateType));
			this.datas.Add (ColumnType.Groupe,         new EditionData (this.controller, this.ValidateGroupe));
			this.datas.Add (ColumnType.TVA,            new EditionData (this.controller, this.ValidateTVA));
			this.datas.Add (ColumnType.CompteOuvBoucl, new EditionData (this.controller, this.ValidateCompteOuvBoucl));
			this.datas.Add (ColumnType.IndexOuvBoucl,  new EditionData (this.controller, this.ValidateIndexOuvBoucl));
			this.datas.Add (ColumnType.Monnaie,        new EditionData (this.controller));
		}


		#region Validators
		private void ValidateNuméro(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				if (data.Text.ToSimpleText ().Contains (' '))
				{
					data.Error = "Le numéro du compte ne peut pas contenir d'espace";
					return;
				}

				var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == data.Text).FirstOrDefault ();
				if (compte == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated || this.controller.EditorController.Duplicate) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaCompteEntity;
				if (himself != null && himself.Numéro == data.Text)
				{
					return;
				}

				data.Error = "Ce numéro de compte existe déjà";
			}
			else
			{
				data.Error = "Il manque le numéro du compte";
			}
		}

		private void ValidateTitre(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le titre du compte");
		}

		private void ValidateCatégorie(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				CatégorieDeCompte catégorie;
				if (!PlanComptableDataAccessor.TextToCatégorie (data.Text, out catégorie))
				{
					data.Error = "La catégorie du compte n'est pas correcte";
				}
			}
			else
			{
				data.Error = "Il manque la catégorie du compte";
			}
		}

		private void ValidateType(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				TypeDeCompte type;
				if (!PlanComptableDataAccessor.TextToType (data.Text, out type))
				{
					data.Error = "Le type du compte n'est pas correct";
				}
			}
			else
			{
				data.Error = "Il manque le type du compte";
			}

		}

		private void ValidateTVA(EditionData data)
		{
			data.ClearError ();
		}

		private void ValidateGroupe(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var n = PlanComptableDataAccessor.GetCompteNuméro (data.Text);
				var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();
				if (compte == null)
				{
					data.Error = "Ce compte n'existe pas";
					return;
				}

				if (compte.Type != TypeDeCompte.Groupe)
				{
					data.Error = "Ce n'est pas un compte de groupement";
					return;
				}

				data.Text = n;
			}
		}

		private void ValidateCompteOuvBoucl(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var n = PlanComptableDataAccessor.GetCompteNuméro (data.Text);
				var compte = this.comptaEntity.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();
				if (compte == null)
				{
					data.Error = "Ce compte n'existe pas";
					return;
				}

				if (compte.Type != TypeDeCompte.Normal)
				{
					data.Error = "Ce compte n'a pas le type \"Normal\"";
					return;
				}

				if (compte.Catégorie != CatégorieDeCompte.Exploitation)
				{
					data.Error = "Ce n'est pas un compte d'exploitation";
					return;
				}

				data.Text = n;
			}
		}

		private void ValidateIndexOuvBoucl(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				int n;
				if (int.TryParse (data.Text.ToSimpleText (), out n))
				{
					if (n >= 1 && n <= 9)
					{
						return;
					}
				}

				data.Error = "Vous devez donner un numéro d'ordre compris entre 1 et 9";
			}
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