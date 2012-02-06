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
			this.datas.Add (ColumnType.Budget,          new EditionData (this.controller, this.ValidateMontant));
			this.datas.Add (ColumnType.BudgetPrécédent, new EditionData (this.controller, this.ValidateMontant));
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
			this.SetMontant (ColumnType.Budget,          compte.Budget);
			this.SetMontant (ColumnType.BudgetPrécédent, compte.BudgetPrécédent);
			this.SetMontant (ColumnType.BudgetFutur,     compte.BudgetFutur);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var compte = entity as ComptaCompteEntity;

			compte.Budget          = this.GetMontant (ColumnType.Budget);
			compte.BudgetPrécédent = this.GetMontant (ColumnType.BudgetPrécédent);
			compte.BudgetFutur     = this.GetMontant (ColumnType.BudgetFutur);
		}
	}
}