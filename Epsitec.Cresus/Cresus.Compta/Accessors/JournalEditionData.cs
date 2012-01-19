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
	/// Données éditables pour le journal de la comptabilité.
	/// </summary>
	public class JournalEditionData : AbstractEditionData
	{
		public JournalEditionData(ComptabilitéEntity comptabilité)
			: base (comptabilité)
		{
		}


		public override void Validate(ColumnType columnType)
		{
			//	Valide le contenu d'une colonne, en adaptant éventuellement son contenu.
			var text = this.GetText (columnType);
			var error = FormattedText.Null;

			switch (columnType)
            {
				case ColumnType.Date:
					error = this.ValidateDate (ref text);
					break;

				case ColumnType.Débit:
					error = this.ValidateCompte (ref text);
					break;

				case ColumnType.Crédit:
					error = this.ValidateCompte (ref text);
					break;

				case ColumnType.Libellé:
					error = this.ValidateLibellé (ref text);
					break;

				case ColumnType.Montant:
					error = this.ValidateMontant (ref text);
					break;
			}

			this.SetText (columnType, text);
			this.errors[columnType] = error;
		}


		#region Validators
		private FormattedText ValidateDate(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque la date";
			}

			Date? date;
			if (this.comptabilité.ParseDate (text, out date) && date.HasValue)
			{
				text = date.ToString ();
				return FormattedText.Empty;
			}
			else
			{
				var b = (this.comptabilité.BeginDate.HasValue) ? this.comptabilité.BeginDate.Value.ToString () : "?";
				var e = (this.comptabilité  .EndDate.HasValue) ? this.comptabilité  .EndDate.Value.ToString () : "?";

				return string.Format ("La date est incorrecte<br/>Elle devrait être comprise entre {0} et {1}", b, e);
			}
		}

		private FormattedText ValidateCompte(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le numéro du compte";
			}

			if (text == JournalDataAccessor.multi)
			{
				return FormattedText.Empty;
			}

			var n = PlanComptableDataAccessor.GetCompteNuméro (text);
			var compte = this.comptabilité.PlanComptable.Where (x => x.Numéro == n).FirstOrDefault ();

			if (compte == null)
			{
				return "Ce compte n'existe pas";
			}

			if (compte.Type != TypeDeCompte.Normal)
			{
				return "Ce compte n'a pas le type \"Normal\"";
			}

			text = n;
			return FormattedText.Empty;
		}

		private FormattedText ValidateLibellé(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le libellé";
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		private FormattedText ValidateMontant(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return "Il manque le montant";
			}

			decimal montant;
			if (decimal.TryParse (text.ToSimpleText (), out montant))
			{
				text = montant.ToString ("0.00");
				return FormattedText.Empty;
			}
			else
			{
				return "Le montant n'est pas correct";
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var écriture = entity as ComptabilitéEcritureEntity;

			this.SetText (ColumnType.Date,             écriture.Date.ToString ());
			this.SetText (ColumnType.Débit,            JournalDataAccessor.GetNuméro (écriture.Débit));
			this.SetText (ColumnType.Crédit,           JournalDataAccessor.GetNuméro (écriture.Crédit));
			this.SetText (ColumnType.Pièce,            écriture.Pièce);
			this.SetText (ColumnType.Libellé,          écriture.Libellé);
			this.SetText (ColumnType.Montant,          écriture.Montant.ToString ("0.00"));
			this.SetText (ColumnType.TotalAutomatique, écriture.TotalAutomatique ? "True" : "False");
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var écriture = entity as ComptabilitéEcritureEntity;

			Date? date;
			if (this.comptabilité.ParseDate (this.GetText (ColumnType.Date), out date))
			{
				écriture.Date = date.Value;
			}

			écriture.Débit  = JournalDataAccessor.GetCompte (this.comptabilité, this.GetText (ColumnType.Débit));
			écriture.Crédit = JournalDataAccessor.GetCompte (this.comptabilité, this.GetText (ColumnType.Crédit));

			écriture.Pièce   = this.GetText (ColumnType.Pièce);
			écriture.Libellé = this.GetText (ColumnType.Libellé);

			decimal montant;
			if (decimal.TryParse (this.GetText (ColumnType.Montant).ToSimpleText (), out montant))
			{
				écriture.Montant = montant;
			}

			écriture.TotalAutomatique = (this.GetText (ColumnType.TotalAutomatique) == "True");
		}
	}
}