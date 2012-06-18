//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;

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
			this.viewSettingsList = this.mainWindowController.GetViewSettingsList (controller.ViewSettingsKey);
			this.searchData       = this.mainWindowController.GetSettingsSearchData (controller.SearchKey);
			this.filterData       = this.viewSettingsList.Selected.CurrentFilter;

			this.soldesJournalManager.Initialize (this.période.Journal);
		}


		public override bool IsEditionCreationEnable
		{
			get
			{
				return false;
			}
		}


		public override void UpdateFilter()
		{
			this.UpdateAfterOptionsChanged ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			this.UpdateMergedFilter ();
			this.planComptableAll = this.compta.PlanComptable;

			if (!this.HasFilter)
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
					if (this.FilterLine (row))
					{
						this.planComptable.Add (this.planComptableAll[row]);
					}
				}
			}

			Date? dateDébut, dateFin;
			this.mergedFilterData.GetBeginnerDates (out dateDébut, out dateFin);
			this.mainWindowController.TemporalData.MergeDates (ref dateDébut, ref dateFin);
			this.soldesJournalManager.Initialize (this.période.Journal, dateDébut, dateFin);

			base.UpdateAfterOptionsChanged ();
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


		public override int GetIndexOf(AbstractEntity entity)
		{
			return this.planComptable.IndexOf (entity as ComptaCompteEntity);
		}


		public override AbstractEntity GetEditionEntity(int row)
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
					return Converters.MontantToString (this.soldesJournalManager.GetSolde (compte), compte.Monnaie);

				case ColumnType.BudgetPrécédent:
					return Converters.MontantToString (this.compta.GetMontantBudget (this.période, -1, compte), compte.Monnaie);

				case ColumnType.Budget:
					return Converters.MontantToString (this.compta.GetMontantBudget (this.période, 0, compte), compte.Monnaie);

				case ColumnType.BudgetFutur:
					return Converters.MontantToString (this.compta.GetMontantBudget (this.période, 1, compte), compte.Monnaie);

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


		public override void StartCreationLine()
		{
			this.editionLine.Clear ();

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isCreation = true;
			this.isModification = false;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.planComptable.Count)
			{
				var data = new BudgetsEditionLine (this.controller);
				var compte = this.planComptable[row];
				data.EntityToData (compte);

				this.editionLine.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isCreation = false;
			this.isModification = true;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public override void UpdateEditionLine()
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
			this.editionLine[0].DataToEntity (compte);
		}


		private IList<ComptaCompteEntity>			planComptableAll;
		private IList<ComptaCompteEntity>			planComptable;
	}
}