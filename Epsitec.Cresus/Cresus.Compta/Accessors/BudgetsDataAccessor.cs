//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données des budgets du plan comptable de la comptabilité.
	/// </summary>
	public class BudgetsDataAccessor : AbstractDataAccessor
	{
		public BudgetsDataAccessor(BusinessContext businessContext, ComptaEntity comptaEntity, MainWindowController windowController)
			: base (businessContext, comptaEntity, windowController)
		{
			this.comptaEntity.PlanComptableUpdate ();
			this.StartCreationData ();
		}


		public override bool IsEditionCreationEnable
		{
			get
			{
				return false;
			}
		}


		public override int Count
		{
			get
			{
				return this.comptaEntity.PlanComptable.Count;
			}
		}

		public override AbstractEntity GetEditionData(int row)
		{
			if (row < 0 || row >= this.comptaEntity.PlanComptable.Count)
			{
				return null;
			}
			else
			{
				return this.comptaEntity.PlanComptable[row];
			}
		}

		public override FormattedText GetText(int row, ColumnType column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var compte = this.comptaEntity.PlanComptable[row];

			switch (column)
			{
				case ColumnType.Numéro:
					return compte.Numéro;

				case ColumnType.Titre:
					return compte.Titre;

				case ColumnType.Solde:
					decimal? solde = this.comptaEntity.GetSoldeCompte (compte);
					if (solde.HasValue && solde.Value != 0)
					{
						return AbstractDataAccessor.GetMontant (solde);
					}
					else
					{
						return FormattedText.Empty;
					}

				case ColumnType.Budget:
					return AbstractDataAccessor.GetMontant (compte.Budget);

				case ColumnType.BudgetPrécédent:
					return AbstractDataAccessor.GetMontant (compte.BudgetPrécédent);

				case ColumnType.BudgetFutur:
					return AbstractDataAccessor.GetMontant (compte.BudgetFutur);

				default:
					return FormattedText.Null;
			}
		}

		public override bool HasBottomSeparator(int row)
		{
			if (row < 0 || row >= this.Count-1)
			{
				return false;
			}

			var compte1 = this.comptaEntity.PlanComptable[row];
			var compte2 = this.comptaEntity.PlanComptable[row+1];

			return compte1.Catégorie != compte2.Catégorie;
		}


		public override void StartCreationData()
		{
			this.editionData.Clear ();

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isModification = false;
			this.justCreated = false;
		}

		public override void StartModificationData(int row)
		{
			this.editionData.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.comptaEntity.PlanComptable.Count)
			{
				var data = new BudgetsEditionData (this.comptaEntity);
				var compte = this.comptaEntity.PlanComptable[row];
				data.EntityToData (compte);

				this.editionData.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isModification = true;
			this.justCreated = false;
		}

		public override void UpdateEditionData()
		{
			if (this.isModification)
			{
				this.UpdateModificationData ();
				this.justCreated = false;
			}

			this.comptaEntity.PlanComptableUpdate ();
			this.SearchUpdate ();
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var compte = this.comptaEntity.PlanComptable[row];
			this.editionData[0].DataToEntity (compte);
		}
	}
}