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
	/// Données éditables pour un code TVA de la comptabilité.
	/// </summary>
	public class CodesTVAEditionLine : AbstractEditionLine
	{
		public CodesTVAEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.Désactivé,     new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Code,          new EditionData (this.controller, this.ValidateCode));
			this.dataDict.Add (ColumnType.Titre,         new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Taux,          new EditionData (this.controller, this.ValidateTaux));
			this.dataDict.Add (ColumnType.Compte,        new EditionData (this.controller, this.ValidateCompte));
			this.dataDict.Add (ColumnType.Chiffre,       new EditionData (this.controller, this.ValidateChiffre));
			this.dataDict.Add (ColumnType.MontantFictif, new EditionData (this.controller, this.ValidateMontant));
			this.dataDict.Add (ColumnType.Erreur,        new EditionData (this.controller, this.ValidateError));
		}


		#region Validators
		private void ValidateCode(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var code = this.compta.CodesTVA.Where (x => x.Code == data.Text).FirstOrDefault ();
				if (code == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated || this.controller.EditorController.Duplicate) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaCodeTVAEntity;
				if (himself != null && himself.Code == data.Text)
				{
					return;
				}

				data.Error = "Ce code TVA existe déjà";
			}
			else
			{
				data.Error = "Il manque le nom du code TVA";
			}
		}

		private void ValidateTaux(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var code = this.compta.ListesTVA.Where (x => x.Nom == data.Text).FirstOrDefault ();

				if (code == null)
				{
					data.Error = "Cette liste de taux n'existe pas";
					return;
				}
			}
			else
			{
				data.Error = "Il manque la liste de taux";
			}
		}

		private void ValidateCompte(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				if (data.Text == JournalDataAccessor.multi)
				{
					return;
				}

				var compte = this.compta.PlanComptable.Where (x => x.Numéro == data.Text).FirstOrDefault ();

				if (compte == null)
				{
					data.Error = "Ce compte n'existe pas";
					return;
				}

				if (compte.Type != TypeDeCompte.TVA)
				{
					data.Error = "Ce compte n'a pas le type \"TVA\"";
					return;
				}
			}
			else
			{
				data.Error = "Il manque le numéro du compte";
			}
		}

		private void ValidateChiffre(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				int n;
				if (int.TryParse (data.Text.ToSimpleText (), out n))
				{
					if (n >= 100 && n <= 999)
					{
						return;
					}
				}

				data.Error = "Vous devez donner un chiffre compris entre 100 et 999";
			}
		}

		private void ValidateMontant(EditionData data)
		{
			Validators.ValidateMontant (data, emptyAccepted: true);
		}

		private void ValidateError(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				data.Error = "Il y a une erreur dans la liste de taux de TVA";
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var codeTVA = entity as ComptaCodeTVAEntity;

			this.SetText (ColumnType.Désactivé,     codeTVA.Désactivé ? "0" : "1");  // logique inversée !
			this.SetText (ColumnType.Code,          codeTVA.Code);
			this.SetText (ColumnType.Titre,         codeTVA.Description);
			this.SetText (ColumnType.Taux,          this.GetTaux (codeTVA));
			this.SetText (ColumnType.Compte,        JournalDataAccessor.GetNuméro (codeTVA.Compte));
			this.SetText (ColumnType.Chiffre,       Converters.IntToString (codeTVA.Chiffre));
			this.SetText (ColumnType.MontantFictif, Converters.MontantToString (codeTVA.MontantFictif));
			this.SetText (ColumnType.Erreur,        TVA.GetError (codeTVA.ListeTaux.Taux));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var codeTVA = entity as ComptaCodeTVAEntity;

			codeTVA.Désactivé     = this.GetText (ColumnType.Désactivé) == "0";  // logique inversée !
			codeTVA.Code          = this.GetText (ColumnType.Code);
			codeTVA.Description   = this.GetText (ColumnType.Titre);
			codeTVA.ListeTaux     = this.SetTaux (this.GetText (ColumnType.Taux));
			codeTVA.Compte        = JournalDataAccessor.GetCompte (this.compta, this.GetText (ColumnType.Compte));
			codeTVA.Chiffre       = Converters.ParseInt (this.GetText (ColumnType.Chiffre));
			codeTVA.MontantFictif = Converters.ParseMontant (this.GetText (ColumnType.MontantFictif));
		}

		private FormattedText GetTaux(ComptaCodeTVAEntity codeTVA)
		{
			if (codeTVA.ListeTaux == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return codeTVA.ListeTaux.Nom;
			}
		}

		private ComptaListeTVAEntity SetTaux(FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return null;
			}
			else
			{
				return this.compta.ListesTVA.Where (x => x.Nom == text).FirstOrDefault ();
			}
		}
	}
}