//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using Epsitec.Cresus.Compta.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour les budgets du plan comptable de la comptabilité.
	/// </summary>
	public class BudgetsEditionLine : AbstractEditionLine
	{
		public BudgetsEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.BudgetPrécédent, new EditionData (this.controller, this.ValidateMontant));
			this.datas.Add (ColumnType.Budget,          new EditionData (this.controller, this.ValidateMontant));
			this.datas.Add (ColumnType.BudgetFutur,     new EditionData (this.controller, this.ValidateMontant));
		}


		#region Validators
		private void ValidateMontant(EditionData data)
		{
			Validators.ValidateMontant (data, emptyAccepted: true);
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var compte = entity as ComptaCompteEntity;

			this.SetText    (ColumnType.Numéro,          compte.Numéro);
			this.SetText    (ColumnType.Titre,           compte.Titre);
			this.SetMontant (ColumnType.Solde,           this.controller.DataAccessor.SoldesJournalManager.GetSolde (compte));
			this.SetMontant (ColumnType.BudgetPrécédent, this.comptaEntity.GetMontantBudget (this.périodeEntity, -1, compte));
			this.SetMontant (ColumnType.Budget,          this.comptaEntity.GetMontantBudget (this.périodeEntity,  0, compte));
			this.SetMontant (ColumnType.BudgetFutur,     this.comptaEntity.GetMontantBudget (this.périodeEntity,  1, compte));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var compte = entity as ComptaCompteEntity;

			this.SetBudget (compte, this.GetMontant (ColumnType.BudgetPrécédent), -1);
			this.SetBudget (compte, this.GetMontant (ColumnType.Budget         ),  0);
			this.SetBudget (compte, this.GetMontant (ColumnType.BudgetFutur    ),  1);
		}

		private void SetBudget(ComptaCompteEntity compte, decimal? montant, int offset)
		{
			//	Modifie un montant au budget dans une période actuelle, précédente ou suivante.
			//	Selon la nécessité, l'entité ComptaBudgetEntity est créée ou supprimée.
			var période = this.comptaEntity.GetPériode (this.périodeEntity, offset);

			if (période == null)
			{
				return;
			}

			var budget = compte.GetBudget (période);

			if (montant.HasValue)
			{
				if (budget == null)
				{
					//	Si l'entité ComptaBudgetEntity n'existe pas, on la crée.
					if (this.controller.BusinessContext == null)
					{
						budget = new ComptaBudgetEntity ();
					}
					else
					{
						budget = this.controller.BusinessContext.CreateEntity<ComptaBudgetEntity> ();
					}

					budget.Période = période;
					compte.Budgets.Add (budget);
				}

				budget.Montant = montant.Value;
			}
			else
			{
				if (budget != null)
				{
					//	Si l'entité ComptaBudgetEntity n'est plus nécessaire, on la supprime.
					if (this.controller.BusinessContext != null)
					{
						this.controller.BusinessContext.DeleteEntity (budget);
					}

					compte.Budgets.Remove (budget);
				}
			}
		}
	}
}