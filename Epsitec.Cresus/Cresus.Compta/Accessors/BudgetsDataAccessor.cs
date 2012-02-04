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
		public BudgetsDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.Budgets.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.Budgets.Filter");

			this.soldesJournalManager.Initialize (this.périodeEntity.Journal);
			this.StartCreationData ();
		}


		public override bool IsEditionCreationEnable
		{
			get
			{
				return false;
			}
		}


		public override void FilterUpdate()
		{
			this.UpdateAfterOptionsChanged ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.planComptableAll = this.comptaEntity.PlanComptable;

			if (this.filterData == null || this.filterData.IsEmpty)
			{
				this.planComptable = this.planComptableAll;
			}
			else
			{
				this.planComptable = new List<ComptaCompteEntity> ();
				this.planComptable.Clear ();

				int count = this.planComptableAll.Count;
				for (int row = 0; row < count; row++)
				{
					int founds = this.FilterLine (row);

					if (founds != 0 && (this.filterData.OrMode || founds == this.filterData.TabsData.Count))
					{
						this.planComptable.Add (this.planComptableAll[row]);
					}
				}
			}

			Date? dateDébut, dateFin;
			this.filterData.GetBeginnerDates (out dateDébut, out dateFin);
			this.soldesJournalManager.Initialize (this.périodeEntity.Journal, dateDébut, dateFin);
		}


		public override int AllCount
		{
			get
			{
				return this.planComptableAll.Count;
			}
		}

		public override int Count
		{
			get
			{
				return this.planComptable.Count;
			}
		}

		public override AbstractEntity GetEditionData(int row)
		{
			if (row < 0 || row >= this.planComptable.Count)
			{
				return null;
			}
			else
			{
				return this.planComptable[row];
			}
		}

		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var planComptable = all ? this.planComptableAll : this.planComptable;

			if (row < 0 || row >= planComptable.Count)
			{
				return FormattedText.Null;
			}

			var compte = planComptable[row];

			switch (column)
			{
				case ColumnType.Numéro:
					return compte.Numéro;

				case ColumnType.Titre:
					return compte.Titre;

				case ColumnType.Solde:
					decimal? solde = this.soldesJournalManager.GetSolde (compte);
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

			var compte1 = this.planComptable[row];
			var compte2 = this.planComptable[row+1];

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

			if (row >= 0 && row < this.planComptable.Count)
			{
				var data = new BudgetsEditionData (this.controller);
				var compte = this.planComptable[row];
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

			this.SearchUpdate ();
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var compte = this.planComptable[row];
			this.editionData[0].DataToEntity (compte);
		}


		private IList<ComptaCompteEntity>			planComptableAll;
		private IList<ComptaCompteEntity>			planComptable;
	}
}