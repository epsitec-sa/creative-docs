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
			this.dataDict.Add (ColumnType.Numéro,         new EditionData (this.controller, this.ValidateNuméro));
			this.dataDict.Add (ColumnType.Titre,          new EditionData (this.controller, this.ValidateTitre));
			this.dataDict.Add (ColumnType.Catégorie,      new EditionData (this.controller, this.ValidateCatégorie));
			this.dataDict.Add (ColumnType.Type,           new EditionData (this.controller, this.ValidateType));
			this.dataDict.Add (ColumnType.Groupe,         new EditionData (this.controller, this.ValidateGroupe));
			this.dataDict.Add (ColumnType.CodeTVA,        new EditionData (this.controller, this.ValidateCodeTVA));
			this.dataDict.Add (ColumnType.CodesTVA,       new EditionData (this.controller));
			this.dataDict.Add (ColumnType.CompteOuvBoucl, new EditionData (this.controller, this.ValidateCompteOuvBoucl));
			this.dataDict.Add (ColumnType.IndexOuvBoucl,  new EditionData (this.controller, this.ValidateIndexOuvBoucl));
			this.dataDict.Add (ColumnType.Monnaie,        new EditionData (this.controller, this.ValidateMonnaie));
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

				var compte = this.compta.PlanComptable.Where (x => x.Numéro == data.Text).FirstOrDefault ();
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
				var catégorie = Converters.StringToCatégorie (data.Text.ToSimpleText ());
				if (catégorie == CatégorieDeCompte.Inconnu)
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
				var type = Converters.StringToType (data.Text.ToSimpleText ());
				if (type == TypeDeCompte.Inconnu)
				{
					data.Error = "Le type du compte n'est pas correct";
				}
			}
			else
			{
				data.Error = "Il manque le type du compte";
			}

		}

		private void ValidateCodeTVA(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var edit = data.Text.ToSimpleText ();
				var code = this.compta.CodesTVA.Where (x => x.Code == edit).FirstOrDefault ();
				var type = Converters.StringToType (this.GetText (ColumnType.Type).ToSimpleText ());

				if (code == null)
				{
					data.Error = "Ce code TVA n'existe pas";
					return;
				}
				else
				{
					if (type != TypeDeCompte.Normal)
					{
						data.Error = "Seuls les comptes de type \"Normal\" peuvent utiliser un code TVA";
						return;
					}
				}
			}
		}

		private void ValidateGroupe(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var compte = this.compta.PlanComptable.Where (x => x.Numéro == data.Text).FirstOrDefault ();
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
			}
		}

		private void ValidateCompteOuvBoucl(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var compte = this.compta.PlanComptable.Where (x => x.Numéro == data.Text).FirstOrDefault ();
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

		private void ValidateMonnaie(EditionData data)
		{
			Validators.ValidateText (data, "Il manque la monnaie");
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var compte = entity as ComptaCompteEntity;

			this.SetText (ColumnType.Numéro,         compte.Numéro);
			this.SetText (ColumnType.Titre,          compte.Titre);
			this.SetText (ColumnType.Catégorie,      Converters.CatégorieToString(compte.Catégorie));
			this.SetText (ColumnType.Type,           Converters.TypeToString (compte.Type));
			this.SetText (ColumnType.Groupe,         PlanComptableDataAccessor.GetNuméro (compte.Groupe));
			this.SetText (ColumnType.CodeTVA,        JournalEditionLine.GetCodeTVADescription (compte.CodeTVAParDéfaut));
			this.SetText (ColumnType.CompteOuvBoucl, PlanComptableDataAccessor.GetNuméro (compte.CompteOuvBoucl));
			this.SetText (ColumnType.IndexOuvBoucl,  (compte.IndexOuvBoucl == 0) ? FormattedText.Empty : compte.IndexOuvBoucl.ToString ());
			this.SetText (ColumnType.Monnaie,        this.GetMonnaie (compte));

			this.SetCodesTVA (compte);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var compte = entity as ComptaCompteEntity;

			compte.Numéro           = this.GetText (ColumnType.Numéro);
			compte.Titre            = this.GetText (ColumnType.Titre);
			compte.Catégorie        = Converters.StringToCatégorie (this.GetText (ColumnType.Catégorie).ToSimpleText ());
			compte.Type             = Converters.StringToType (this.GetText (ColumnType.Type).ToSimpleText ());
			compte.Groupe           = PlanComptableDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Groupe));
			compte.CodeTVAParDéfaut = JournalEditionLine.TextToCodeTVA (this.compta, this.GetText (ColumnType.CodeTVA));
			compte.CompteOuvBoucl   = PlanComptableDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.CompteOuvBoucl));

			int index;
			if (int.TryParse (this.GetText (ColumnType.IndexOuvBoucl).ToSimpleText (), out index) && index >= 1 && index <= 9)
			{
				compte.IndexOuvBoucl = index;
			}
			else
			{
				compte.IndexOuvBoucl = 0;
			}

			compte.Monnaie = this.SetMonnaie (this.GetText (ColumnType.Monnaie));

			this.GetCodesTVA (compte);
		}

		private void SetCodesTVA(ComptaCompteEntity compte)
		{
			var parameters = this.GetParameters (ColumnType.CodesTVA);
			parameters.Clear ();

			foreach (var codeTVA in this.compta.CodesTVA)
			{
				parameters.Add (codeTVA.Code);
			}

			var texts = this.GetTexts (ColumnType.CodesTVA);
			texts.Clear ();

			foreach (var codeTVA in compte.CodesTVAPossibles)
			{
				texts.Add (codeTVA.Code);
			}
		}

		private void GetCodesTVA(ComptaCompteEntity compte)
		{
			compte.CodesTVAPossibles.Clear ();

			foreach (var text in this.GetTexts (ColumnType.CodesTVA))
			{
				var codeTVA = this.compta.CodesTVA.Where (x => x.Code == text).FirstOrDefault ();

				if (codeTVA != null)
				{
					compte.CodesTVAPossibles.Add (codeTVA);
				}
			}
		}

		private FormattedText GetMonnaie(ComptaCompteEntity compte)
		{
			if (compte.Monnaie == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return compte.Monnaie.CodeISO;
			}
		}

		private ComptaMonnaieEntity SetMonnaie(FormattedText codeISO)
		{
			return this.compta.Monnaies.Where (x => x.CodeISO == codeISO).FirstOrDefault ();
		}
	}
}