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
	/// Données éditables pour les budgets du plan comptable de la comptabilité.
	/// </summary>
	public class BudgetsEditionData : AbstractEditionData
	{
		public BudgetsEditionData(ComptabilitéEntity comptabilité)
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
				case ColumnType.Budget:
				case ColumnType.BudgetPrécédent:
				case ColumnType.BudgetFutur:
					error = this.ValidateMontant (ref text);
					break;
			}

			this.SetText (columnType, text);
			this.errors[columnType] = error;
		}


		#region Validators
		private FormattedText ValidateMontant(ref FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return FormattedText.Empty;
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
			var compte = entity as ComptabilitéCompteEntity;

			this.SetText    (ColumnType.Numéro,          compte.Numéro);
			this.SetText    (ColumnType.Titre,           compte.Titre);
			this.SetMontant (ColumnType.Solde,           this.comptabilité.GetSoldeCompte (compte));
			this.SetMontant (ColumnType.Budget,          compte.Budget);
			this.SetMontant (ColumnType.BudgetPrécédent, compte.BudgetPrécédent);
			this.SetMontant (ColumnType.BudgetFutur,     compte.BudgetFutur);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var compte = entity as ComptabilitéCompteEntity;

			compte.Budget          = this.GetMontant (ColumnType.Budget);
			compte.BudgetPrécédent = this.GetMontant (ColumnType.BudgetPrécédent);
			compte.BudgetFutur     = this.GetMontant (ColumnType.BudgetFutur);
		}


		private void SetMontant(ColumnType columnType, decimal? value)
		{
			if (value.HasValue)
			{
				this.SetText (columnType, value.Value.ToString ("0.00"));
			}
			else
			{
				this.SetText (columnType, FormattedText.Empty);
			}
		}

		private decimal? GetMontant(ColumnType columnType)
		{
			var text = this.GetText (columnType);

			if (!text.IsNullOrEmpty)
			{
				decimal d;
				if (decimal.TryParse (text.ToSimpleText (), out d))
				{
					return d;
				}
			}

			return null;
		}
	}
}